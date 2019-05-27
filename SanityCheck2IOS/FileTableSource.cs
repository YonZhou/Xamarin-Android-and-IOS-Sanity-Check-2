using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;

using Foundation;
using ObjCRuntime;
using pdftron.FDF;
using pdftron.PDF;
using pdftron.PDF.Controls;
using pdftron.PDF.Tools;
using pdftronprivate;
using UIKit;
using pdftron.Common;
using pdftron.SDF;
using System.IO;

namespace SanityCheck2IOS
{
    public class FileTableSource : UITableViewSource
    {
        List<String> FileNames;
        //string[] ImageURLs;
        string CellIdentifier = "fileTableCell";
        PTToolManager mToolManager;
        public string currentFileName;
        PTPDFViewCtrl mpdfviewctrl;
        public PTDocumentViewController documentController;

        private UINavigationController primNav { get; set; }
        public FileTableSource(List<String> fileNames, UINavigationController primNav)
        {
            this.FileNames = fileNames;
            this.primNav = primNav;

            //UIAlertController okAlertController = UIAlertController.Create("Row Selected", FileNames[indexPath.Row], UIAlertControllerStyle.Alert);
            //okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));

            // Create a PTDocumentViewController
            try
            {
                documentController = new PTDocumentViewController();
                documentController.Delegate = new AfterLoadViewDelegateWrapper();

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }

            // The PTDocumentViewController must be in a navigation controller before a document can be opened
            //var docnavigationController = new UINavigationController(documentController);

            // documentcontroller should be initialized at this point
            mpdfviewctrl = documentController.PdfViewCtrl;
            mpdfviewctrl.SetBackgroundColor(255,105,180,255);

            //mpdfviewctrl.DownloadEventType += (sender, e) =>
            //{
            //    if (e.Type == DownloadTypes.e_downloadtype_finished)
            //    {
            //        integrateAnnotations(mpdfviewctrl.GetDoc());
            //    }

            //};

            mToolManager = documentController.ToolManager;

            var nukeButton = new UIBarButtonItem();
            var rightButtons = documentController.NavigationItem.RightBarButtonItems;
            nukeButton.Image = UIImage.FromFile("nukeToolIcon.png");
            nukeButton.Width = 40f;
            nukeButton.Clicked += (sender, e) => {
                var newTool = mToolManager.ChangeTool(new Class(typeof(pdftron.PDF.Tools.PTImageStampCreate)));
                ((pdftron.PDF.Tools.PTTool)newTool).BackToPanToolAfterUse = true;
                System.Diagnostics.Debug.WriteLine("reached");

                //mToolManager.Tool = new pdftron.PDF.Tools.PTImageStampCreate();
                //Stamper s = new Stamper(Stamper.SizeType.e_relative_scale, .05, .05);
                //s.SetAlignment(Stamper.HorizontalAlignment.e_horizontal_left, Stamper.VerticalAlignment.e_vertical_center);
                //s.SetFontColor(new ColorPt(0, 0, 0, 0));
                //s.SetRotation(180);
                //s.SetAsBackground(false);
                //s.StampImage(doc., null, new PageSet(1));

                var client = new HttpClient();
                client.BaseAddress = new Uri("http://laptop-ejbj9ok5:8080");
                client.PostAsync("/logNuclear", null);

            };

            UIBarButtonItem uploadAnnotsButton = new UIBarButtonItem();
            uploadAnnotsButton.Image = UIImage.FromFile("upload_arrow.png");
            uploadAnnotsButton.Width = 40f;
            uploadAnnotsButton.Clicked += (sender, e) =>
            {
                var currentDoc = mpdfviewctrl.GetDoc();
                logAnnotations(currentDoc);
            };

            UIBarButtonItem downloadAnnotsButton = new UIBarButtonItem();
            downloadAnnotsButton.Image = UIImage.FromFile("download_arrow.png");
            downloadAnnotsButton.Width = 40f;
            downloadAnnotsButton.Clicked += (sender, e) =>
            {
                var currentDoc = mpdfviewctrl.GetDoc();
                integrateAnnotations(currentDoc);
            };

            List<UIBarButtonItem> rightButtonsList = new List<UIBarButtonItem>();
            for (int i = 0; i < rightButtons.Length; i++)
            {
                rightButtonsList.Add(rightButtons[i]);
            }
            //Array.Resize(ref rightButtons, rightButtons.Length+1);
            //rightButtons[rightButtons.Length-1] = nukeButton;
            rightButtonsList.Add(nukeButton);
            rightButtonsList.Add(downloadAnnotsButton);
            rightButtonsList.Add(uploadAnnotsButton);

            if (rightButtons != null)
            {
                documentController.NavigationItem.RightBarButtonItems = rightButtonsList.ToArray();
            }
        }

        public static UIImage FromUrl(string uri)
        {
            using (var url = new NSUrl(uri))
            using (var data = NSData.FromUrl(url))
                return UIImage.LoadFromData(data);
        }
        public override UITableViewCell GetCell(UITableView tableView, NSIndexPath indexPath)
        {

            UITableViewCell cell = tableView.DequeueReusableCell(CellIdentifier);
            string name = FileNames[indexPath.Row];

            //---- if there are no cells to reuse, create a new one
            if (cell == null)
            {
                cell = new UITableViewCell(UITableViewCellStyle.Default, CellIdentifier);
                UIImageView cellImg = new UIImageView(new CoreGraphics.CGRect(0,0,100,100));
                cellImg.Image = FromUrl("http://laptop-ejbj9ok5:8080/" + System.IO.Path.GetFileNameWithoutExtension(name) + ".png");
                cell.AddSubview(cellImg);

                UILabel celltxt = new UILabel(new CoreGraphics.CGRect(150, -25, 200, 100));
                celltxt.Text = name;
                cell.AddSubview(celltxt);

                //cell.ImageView.Image = FromUrl("http://laptop-ejbj9ok5:8080/" + System.IO.Path.GetFileNameWithoutExtension(name) + ".png");
                System.Diagnostics.Debug.WriteLine("loading image from " + "http://laptop-ejbj9ok5:8080/" + System.IO.Path.GetFileNameWithoutExtension(name) + ".png");
            }

            //cell.TextLabel.Text = name;

            return cell;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return FileNames.Count;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            tableView.DeselectRow(indexPath, true);

            // Open an existing local file URL.
            try
            {
                currentFileName = FileNames[indexPath.Row];
                AfterLoadViewDelegateWrapper del = (AfterLoadViewDelegateWrapper)documentController.Delegate;
                del.fileName = currentFileName;
                documentController.OpenDocumentWithURL(new NSUrl("http://laptop-ejbj9ok5:8080/" + FileNames[indexPath.Row]));
                //documentController.opendocument2(new NSUrl("http://laptop-ejbj9ok5:8080/" + FileNames[indexPath.Row]));
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }

            primNav.PushViewController(documentController, true);
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return 120;
        }

        public void logAnnotations(PTPDFDoc document)
        {
            var currentPDFDoc = TypeConvertHelper.ConvPdfDocToManaged(document);
            WebClient client = new WebClient();

            FDFDoc annotationsDoc = currentPDFDoc.FDFExtract(PDFDoc.ExtractFlag.e_both);
            String outputXFDF = annotationsDoc.SaveAsXFDF();

            StringContent bytestreamcontent = new StringContent(outputXFDF);

            var multi = new MultipartFormDataContent();
            multi.Add(bytestreamcontent);

            string filenameToSend = Path.GetFileNameWithoutExtension(currentFileName);
            bytestreamcontent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data")
            {
                Name = "annotations",
                FileName = filenameToSend
            };

            var Httpclient = new HttpClient();
            Httpclient.BaseAddress = new Uri("http://laptop-ejbj9ok5:8080");
            Httpclient.PostAsync("/saveAnnotations", multi);

        }

        public void integrateAnnotations(PTPDFDoc document)
        {
            WebClient client = new WebClient();
            string annotationsFile = Path.GetFileNameWithoutExtension(currentFileName) + "_annots.xml";
            string annotations = null;
            try
            {
                annotations = client.DownloadString("http://laptop-ejbj9ok5:8080" + "/" + annotationsFile);
                //System.Diagnostics.Debug.WriteLine("exported string is " + annotations);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("download annotations failed");
                System.Diagnostics.Debug.WriteLine(e);
            }
            if (annotations != null)
            {
                try
                {
                    mpdfviewctrl.DocLock(true);
                  
                    int previousPage = mpdfviewctrl.GetCurrentPage();
                    var annotationsFDFDoc = FDFDoc.CreateFromXFDF(annotations);

                    var currentPDFDoc = TypeConvertHelper.ConvPdfDocToManaged(document);
                    currentPDFDoc.FDFUpdate(annotationsFDFDoc);
                    //ContentReplacer replacer = new ContentReplacer();
                    //Page page = currentPDFDoc.GetPage(1);
                    //replacer.AddString("FIRST_NAME", "John");
                    //replacer.Process(page);
                    //PTPDFDoc newDocument = TypeConvertHelper.ConvPDFDocToNative(currentPDFDoc);
                    mpdfviewctrl.Update(true);
                    mpdfviewctrl.DocUnlock();


                    //mpdfviewctrl.SetDoc(newDocument);
                    //mpdfviewctrl.SetCurrentPage(previousPage);

                    //mpdfviewctrl.DocUnlock();

                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e);
                }
            }

        }
    }
}