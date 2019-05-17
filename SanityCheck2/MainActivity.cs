using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Android;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using Java.IO;
using pdftron.FDF;
using pdftron.PDF;
using pdftron.PDF.Config;
using pdftron.PDF.Controls;
using pdftron.PDF.Dialog;
using pdftron.PDF.Tools;
using pdftron.PDF.Tools.Utils;
using static Android.Manifest;
using AlertDialog = Android.Support.V7.App.AlertDialog;

namespace SanityCheck2
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = false)]
    public class MainActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        protected PDFViewCtrl mPdfViewCtrl;
        protected ToolManager mToolManager;
        protected PDFDoc currentPDFDoc;
        public static String debugXML;
        public static bool isDebug = false;
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            pdftron.PDFNet.Initialize(this, Resource.Raw.pdfnet, "demo:yzhou@pdftron.com:7458c53d015f540837d0782dcc022ec2e8f2864adea4cc4f8a");

            SetContentView(Resource.Layout.activity_main);
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClick;

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(this, drawer, toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            drawer.AddDrawerListener(toggle);
            toggle.SyncState();

            NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            navigationView.SetNavigationItemSelectedListener(this);

            //var files = System.IO.Directory.GetFiles("/filesfolder");

            //if (!files.Any())
            //{

            //} else
            //{
            //    foreach (var file in files)
            //    {
            //        pdfViewCtrl.Thumb
            //    }
            //}

            // pdfviewctrl stuff

            mPdfViewCtrl = FindViewById<PDFViewCtrl>(Resource.Id.pdfviewctrlfirst);
            AppUtils.SetupPDFViewCtrl(mPdfViewCtrl);
            var httpRequestOptions = new PDFViewCtrl.HTTPRequestOptions();
            httpRequestOptions.RestrictDownloadUsage(false);

            mToolManager = pdftron.PDF.Config.ToolManagerBuilder.From()
                .SetEditInk(true)
                .SetOpenToolbar(true)
                .SetCopyAnnot(true)
                .Build(this, mPdfViewCtrl);

            // ---------------- SETUP THUMBNAIL SLIDER CONTAINER ------------------ //

            var thumbDiag = ThumbnailsViewFragment.NewInstance();
            thumbDiag.SetPdfViewCtrl(mPdfViewCtrl);

            // ---------------- SETUP BOOKMARKS AND TABLE OF CONTENTS CONTAINER ------------------ //

            List<DialogFragmentTab> tabs = new List<DialogFragmentTab>();

            // add all tabs
            var annotationsTab = new DialogFragmentTab(
    Java.Lang.Class.FromType(typeof(AnnotationDialogFragment)), BookmarksTabLayout.TagTabAnnotation, null, "Annotations", "Bookmarks Dialog", null);
            var outlineDialog = new DialogFragmentTab(
                Java.Lang.Class.FromType(typeof(OutlineDialogFragment)), BookmarksTabLayout.TagTabOutline, null, "Outline", "Bookmarks Dialog", null);
            var userBookmarksDialog = new DialogFragmentTab(
                Java.Lang.Class.FromType(typeof(UserBookmarkDialogFragment)), BookmarksTabLayout.TagTabBookmark, null, "User Bookmarks", "Bookmarks Dialog", null);
            tabs.Add(annotationsTab);
            tabs.Add(userBookmarksDialog);
            tabs.Add(outlineDialog);

            var bookmarksList = BookmarksDialogFragment.NewInstance();
            bookmarksList.SetPdfViewCtrl(mPdfViewCtrl);
            bookmarksList.SetDialogFragmentTabs(tabs);
            bookmarksList.SetStyle((int)DialogFragmentStyle.NoTitle, Resource.Style.AppTheme);

            PDFDoc document = mPdfViewCtrl.GetDoc();
            //Page firstpage = document.GetPage(1);

            String URLtoLoad = this.Intent.GetStringExtra("DOCUMENT_TO_LOAD");
            bool isDOCX = this.Intent.GetBooleanExtra("ISDOCX", false);


            if (isDOCX)
            {
                loadNonPDF(URLtoLoad);
            }
            else
                mPdfViewCtrl.OpenUrlAsync(URLtoLoad, this.CacheDir.AbsolutePath, null, httpRequestOptions);

            // ------------------- SETUP COMPARE DOCUMENTS OPTION --------------------- //
            // TODO

            // ------------------------ setup listeners --------------------------- //
           
            // Thumbnail slider
            var firstThumbnailSlider = FindViewById<NativeThumbnailSlider>(Resource.Id.thumbnailSliderFirst);
            firstThumbnailSlider.MenuItemClicked += (sender, e) =>
            {
                if (e.MenuItemPosition == NativeThumbnailSlider.PositionLeft)
                {
                    thumbDiag.Show(this.SupportFragmentManager, "thumbnails_dialog");

                }
                else
                {
                    bookmarksList.Show(this.SupportFragmentManager, "bookmarks_dialog");
                }
            };

            thumbDiag.ThumbnailsViewDialogDismiss += (sender, e) =>
            {
                firstThumbnailSlider.SetProgress(mPdfViewCtrl.CurrentPage);
            };

            // listeners for bookmarks dialog tabs

            bookmarksList.AnnotationClicked += (sender, e) =>
            {
                bookmarksList.Dismiss();
            };
            bookmarksList.ExportAnnotations += (sender, e) =>
            {
                var doc = e.OutputDoc;
                bookmarksList.Dismiss();
            };
            bookmarksList.OutlineClicked += (sender, e) =>
            {
                bookmarksList.Dismiss();
            };
            bookmarksList.UserBookmarkClick += (sender, e) =>
            {
                mPdfViewCtrl.SetCurrentPage(e.PageNum);
                bookmarksList.Dismiss();
            };

            // listener for page changed
            mPdfViewCtrl.PageNumberChanged += (sender, e) =>
            {
                firstThumbnailSlider.SetProgress(mPdfViewCtrl.CurrentPage);
            };


            // setup search

            var searchToolbar = FindViewById<pdftron.PDF.Controls.SearchToolbar>(Resource.Id.searchText1);
            var searchOverlay = FindViewById<pdftron.PDF.Controls.FindTextOverlay>(Resource.Id.find_text_view);
            searchOverlay.SetPdfViewCtrl(mPdfViewCtrl);

            searchToolbar.ExitSearch += (sender, e) =>
            {
                searchToolbar.Visibility = ViewStates.Gone;
                searchOverlay.Visibility = ViewStates.Gone;
                searchOverlay.ExitSearchMode();
            };
            searchToolbar.ClearSearchQuery += (sender, e) =>
            {
                searchOverlay?.CancelFindText();
            };
            searchToolbar.SearchQuerySubmit += (sender, e) =>
            {
                searchOverlay?.QueryTextSubmit(e.Query);
            };
            searchToolbar.SearchQueryChange += (sender, e) =>
            {
                searchOverlay?.SetSearchQuery(e.Query);
            };
            searchToolbar.SearchOptionsItemSelected += (sender, e) =>
            {
                int id = e.Item.ItemId;
                if (id == Resource.Id.action_match_case)
                {
                    bool isChecked = e.Item.IsChecked;
                    searchOverlay?.SetSearchMatchCase(!isChecked);
                    searchOverlay?.ResetFullTextResults();
                    e.Item.SetChecked(!isChecked);
                }
                else if (id == Resource.Id.action_whole_word)
                {
                    bool isChecked = e.Item.IsChecked;
                    searchOverlay?.SetSearchWholeWord(!isChecked);
                    searchOverlay?.ResetFullTextResults();
                    e.Item.SetChecked(!isChecked);
                }
            };

        }

        public override void OnBackPressed()
        {
            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            if(drawer.IsDrawerOpen(GravityCompat.Start))
            {
                drawer.CloseDrawer(GravityCompat.Start);
            }
            else
            {
                base.OnBackPressed();
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            Button button = FindViewById<Button>(Resource.Id.loadDocumentButton);
            button.Click += delegate
            {
                // setup for documentviewer activity, integrates pdfviewctrl configurations
                var pdfviewctrlconfig = PDFViewCtrlConfig.GetDefaultConfig(this)
                    .SetUrlExtraction(true);

                var config = new pdftron.PDF.Config.ViewerConfig.Builder()
                .OpenUrlCachePath(this.CacheDir.AbsolutePath)
                    .PdfViewCtrlConfig(pdfviewctrlconfig)
                    .ShowOpenFileOption(true)
                    .ShowOpenUrlOption(true)
                .Build();

                //pdftron.PDF.Controls.DocumentActivity.OpenDocument(this, Resource.Raw.test);

                var fileLink = Android.Net.Uri.Parse("https://www.hq.nasa.gov/alsj/a17/A17_FlightPlan.pdf");
                pdftron.PDF.Controls.DocumentActivity.OpenDocument(this, fileLink, config);
            };

            return true;
        }

        // Function which handles menu options on right of header toolbar
        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_search)
            {
                var searchToolbar = FindViewById<pdftron.PDF.Controls.SearchToolbar>(Resource.Id.searchText1);
                var searchOverlay = FindViewById<pdftron.PDF.Controls.FindTextOverlay>(Resource.Id.find_text_view);
                searchToolbar.Visibility = ViewStates.Visible;
                searchOverlay.Visibility = ViewStates.Visible;
                return true;
            }else if(id == Resource.Id.stampButton)
            {
                File nukeImage = Utils.CopyResourceToLocal(mPdfViewCtrl.Context, Resource.Raw.nuclear_hazard, "nuclear_hazard", ".png");
                var nukeTool = new NuclearTool(mPdfViewCtrl, nukeImage);

                mToolManager.Tool = nukeTool;
                //mToolManager.EnableAnnotManager();

                return true;
            } else if(id == Resource.Id.saveAnnotationsButton)
            {
                WebClient client = new WebClient();
                currentPDFDoc = mPdfViewCtrl.GetDoc();

                if (!isDebug)
                {
                    FDFDoc annotationsDoc = currentPDFDoc.FDFExtract(PDFDoc.ExtractFlag.e_annots_only);
                    debugXML = annotationsDoc.SaveAsXFDF();
                    System.Diagnostics.Debug.WriteLine("annotations " + annotationsDoc.SaveAsXFDF());
                    // save XFDF file to server
                    byte[] outputXFDF = annotationsDoc.Save();
                    isDebug = true;
                }
                // below only for debugging purposes
                //String testXML = File.readalltext();
                //String testXML = "";
                //System.Diagnostics.Debug.WriteLine(testXML);

                FDFDoc newAnnotationsLoaded = FDFDoc.CreateFromXFDF(debugXML);
                currentPDFDoc.FDFUpdate(newAnnotationsLoaded);
                mPdfViewCtrl.SetDoc(currentPDFDoc);
            }

            return base.OnOptionsItemSelected(item);
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            View view = (View) sender;
            Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
                .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();
        }

        public bool OnNavigationItemSelected(IMenuItem item)
        {
            int id = item.ItemId;

            if (id == Resource.Id.nav_camera)
            {
                // Handle the camera action
            }
            else if (id == Resource.Id.nav_gallery)
            {

            }
            else if (id == Resource.Id.nav_slideshow)
            {

            }
            else if (id == Resource.Id.nav_manage)
            {

            }
            else if (id == Resource.Id.nav_share)
            {

            }
            else if (id == Resource.Id.nav_send)
            {

            }

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            drawer.CloseDrawer(GravityCompat.Start);
            return true;
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            //PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);

        }

        protected override void OnPause()
        {
            base.OnPause();
            mPdfViewCtrl.Pause();
        }

        protected override void OnResume()
        {
            base.OnResume();
            mPdfViewCtrl.Resume();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            //var mPdfDoc = mPdfViewCtrl.GetDoc();
            //mPdfDoc.Close();
            // bug here
            //mPdfViewCtrl?.Destroy();
        }

        private void loadNonPDF(String URLtoLoad)
        {
            System.Diagnostics.Debug.WriteLine("detected docx");

            Android.Net.Uri docxURI = Android.Net.Uri.Parse(URLtoLoad);

            WebClient client = new WebClient();
            try
            {
                client.DownloadFile(URLtoLoad, Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + "/downloadedDoc.docx");
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("docdownload " + e);
            }
            System.Diagnostics.Debug.WriteLine("docx uri " + docxURI.ToString());
            DocumentConversion documentConversion = null;

            try
            {
                //documentConversion = mPdfViewCtrl.OpenNonPDFUri(Android.Net.Uri.Parse(URLtoLoad), null);
                documentConversion = mPdfViewCtrl.OpenNonPDFUri(Android.Net.Uri.Parse(Android.OS.Environment.ExternalStorageDirectory.AbsolutePath + "/downloadedDoc.docx"), null);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("opennonpdfuri exception " + e);
            }

            mPdfViewCtrl.UniversalDocumentConversion += (sender, e) =>
            {
                if (e.State == PDFViewCtrl.ConversionState.Progress)
                {
                    System.Diagnostics.Debug.WriteLine("in progress");
                }
                else if (e.State == PDFViewCtrl.ConversionState.Finished)
                {
                    System.Diagnostics.Debug.WriteLine("success");
                    PDFDoc converted = documentConversion.GetDoc();
                    // below is unnessecary
                    //mPdfViewCtrl.SetDoc(converted);
                }
                else if (e.State == PDFViewCtrl.ConversionState.Failed)
                {
                    System.Diagnostics.Debug.WriteLine("failed");
                }
            };
        }


    }
}

