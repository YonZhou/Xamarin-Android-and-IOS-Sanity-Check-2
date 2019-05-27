using Foundation;
using Newtonsoft.Json;
using pdftron.PDF.Controls;
using System;
using System.Collections.Generic;
using System.Net.Http;
using UIKit;

namespace SanityCheck2IOS
{
    public partial class ViewController : UIViewController
    {
        public ViewController (IntPtr handle) : base (handle)
        {
            
        }

        public override void ViewDidLoad ()
        {
            base.ViewDidLoad ();
            // Perform any additional setup after loading the view, typically from a nib.
            pdftron.PDFNet.Initialize("demo:yzhou@pdftron.com:7458c53d015f540837d0782dcc022ec2e8f2864adea4cc4f8a");
            var table = new UITableView(View.Bounds); // defaults to Plain style

            var filetableObject = new FileTableSource(GetFiles(), this.NavigationController);
            table.Source = filetableObject;
            table.RowHeight = UITableView.AutomaticDimension;
            table.EstimatedRowHeight = 100f;

            UIBarButtonItem compareFilesButton = new UIBarButtonItem("Compare Files", UIBarButtonItemStyle.Plain, null);
            //UIBarButtonItem backButton = new UIBarButtonItem("Cancel", UIBarButtonItemStyle.Plain, null);

            //compareFilesButton.Frame = new CoreGraphics.CGRect(25,25,300,150);

            compareFilesButton.Clicked += (sender, e) =>
            {
                var diffController = new PTDiffViewController();
                if(!(Xamarin.Essentials.DeviceInfo.Idiom == Xamarin.Essentials.DeviceIdiom.Phone))
                {
                    NavigationController.ModalPresentationStyle = UIModalPresentationStyle.Popover;
                    //NavigationController.PopoverPresentationController.SourceView = btn;
                    //NavigationController.PopoverPresentationController.SourceRect = btn.Bounds;
                }

                diffController.DidCreateDiffFileAtURL += (sender2, e2) =>
                {
                    PTDocumentViewController controller = new PTDocumentViewController();
                    //filetableObject.documentController.OpenDocumentWithURL(e2.FileURL);
                    controller.OpenDocumentWithURL(e2.FileURL);
                    this.NavigationController.PushViewController(controller, true);
                };

                this.NavigationController.PushViewController(diffController, true);
            };

            //backButton.Clicked += (sender, e) =>
            //{
            //    this.NavigationController.PopViewController(true);
            //};


            table.ReloadData();
            Add(table);
            NavigationItem.BackBarButtonItem = new UIBarButtonItem { Title = "Back" };
            NavigationController.NavigationBar.TopItem.RightBarButtonItem = compareFilesButton;
        }

        public override void ViewDidAppear(bool animated)
        {
            base.ViewDidAppear(animated);
        }

        public List<String> GetFiles()
        {

            HttpClient client = new HttpClient();
            var uri = new Uri("http://LAPTOP-EJBJ9OK5:8080/getFiles");
            var response = client.GetAsync(uri).Result;
            var responsecontent = response.Content;
            string responseString = responsecontent.ReadAsStringAsync().Result;

            System.Diagnostics.Debug.WriteLine("reached response " + responseString);

            List<String> allFiles = JsonConvert.DeserializeObject<List<String>>(responseString);
            System.Diagnostics.Debug.WriteLine("converted list output " + allFiles);
            List<String> files = new List<String>();
            foreach(String s in allFiles)
            {
                if (System.IO.Path.GetExtension(s) == ".pdf" || System.IO.Path.GetExtension(s) == ".docx")
                {
                    try
                    {
                        System.Diagnostics.Debug.WriteLine(files);
                        files.Add(s);
                    } catch(Exception e)
                    {
                        System.Diagnostics.Debug.WriteLine(e);
                    }
                }
            }
            return files;

        }

        public override void DidReceiveMemoryWarning ()
        {
            base.DidReceiveMemoryWarning ();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}