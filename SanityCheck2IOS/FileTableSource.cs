using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

using Foundation;
using ObjCRuntime;
using pdftron.FDF;
using pdftron.PDF;
using pdftron.PDF.Controls;
using pdftron.PDF.Tools;
using UIKit;

namespace SanityCheck2IOS
{
    public class FileTableSource : UITableViewSource
    {
        List<String> FileNames;
        //string[] ImageURLs;
        string CellIdentifier = "fileTableCell";
        PTToolManager mToolManager;

        private UINavigationController primNav { get; set; }
        public FileTableSource(List<String> fileNames, UINavigationController primNav)
        {
            this.FileNames = fileNames;
            this.primNav = primNav;
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
            //UIAlertController okAlertController = UIAlertController.Create("Row Selected", FileNames[indexPath.Row], UIAlertControllerStyle.Alert);
            //okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));
            System.Diagnostics.Debug.WriteLine(FileNames[indexPath.Row]);
	        tableView.DeselectRow(indexPath, true);

            // Create a PTDocumentViewController
            PTDocumentViewController documentController = null;
            try
            {
                documentController = new PTDocumentViewController();

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }

            // The PTDocumentViewController must be in a navigation controller before a document can be opened
            //var docnavigationController = new UINavigationController(documentController);

            // Open an existing local file URL.
            try
            {
                documentController.OpenDocumentWithURL(new NSUrl("http://laptop-ejbj9ok5:8080/" + FileNames[indexPath.Row]));
            } catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e);
            }

            // documentcontroller should be initialized at this point
            var mpdfviewctrl = documentController.PdfViewCtrl;
            var doc = mpdfviewctrl.GetDoc();
            logAnnotations(doc);

            mToolManager = documentController.ToolManager;

            var nukeButton = new UIBarButtonItem();
            var rightButtons = documentController.NavigationItem.RightBarButtonItems;
            nukeButton.Image = UIImage.FromFile("nukeToolIcon.png");
            nukeButton.Width = rightButtons[0].Width;
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


            };



            Array.Resize(ref rightButtons, rightButtons.Length+1);
            rightButtons[rightButtons.Length-1] = nukeButton;

            if (rightButtons != null)
            {
                documentController.NavigationItem.RightBarButtonItems = rightButtons;
            }

            primNav.PushViewController(documentController, true);
        }

        public override nfloat GetHeightForRow(UITableView tableView, NSIndexPath indexPath)
        {
            return 120;
        }

        public void logAnnotations(pdftronprivate.PTPDFDoc document)
        {
            pdftron.PDF.PDFViewCtrl ctrl = new pdftron.PDF.PDFViewCtrl();

            pdftron.PDF.PDFDoc doc = new pdftron.PDF.PDFDoc();
        }
    }
}