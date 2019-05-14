using System;
using System.Linq;
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
using pdftron.PDF;
using pdftron.PDF.Config;
using pdftron.PDF.Controls;
using pdftron.PDF.Tools;
using pdftron.PDF.Tools.Utils;

namespace SanityCheck2
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {
        protected PDFViewCtrl mPdfViewCtrl;
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
            httpRequestOptions.RestrictDownloadUsage(true);

            var mToolManager = pdftron.PDF.Config.ToolManagerBuilder.From()
                .SetEditInk(true)
                .SetOpenToolbar(true)
                .SetBuildInPageIndicator(false)
                .SetCopyAnnot(true)
                .Build(this, mPdfViewCtrl);

            mPdfViewCtrl.ToolManager = mToolManager;
            mPdfViewCtrl.OpenUrlAsync("https://www.hq.nasa.gov/alsj/a17/A17_FlightPlan.pdf", this.CacheDir.AbsolutePath, null, httpRequestOptions);


            // setup for documentviewer activity, integrates pdfviewctrl configurations
            var pdfviewctrlconfig = PDFViewCtrlConfig.GetDefaultConfig(this)
                .SetUrlExtraction(true);

            var config = new pdftron.PDF.Config.ViewerConfig.Builder()
            .OpenUrlCachePath(this.CacheDir.AbsolutePath)
                .PdfViewCtrlConfig(pdfviewctrlconfig)
                .ShowOpenFileOption(true)
                .ShowOpenUrlOption(true)
            .Build();

            Button button = FindViewById<Button>(Resource.Id.loadDocumentButton);
            button.Click += delegate
            {
                //pdftron.PDF.Controls.DocumentActivity.OpenDocument(this, Resource.Raw.test);

                var fileLink = Android.Net.Uri.Parse("https://www.hq.nasa.gov/alsj/a17/A17_FlightPlan.pdf");
                pdftron.PDF.Controls.DocumentActivity.OpenDocument(this, fileLink, config);
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
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
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

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        protected override void OnPause()
        {
            base.OnPause();
            mPdfViewCtrl?.Pause();
        }
    }
}

