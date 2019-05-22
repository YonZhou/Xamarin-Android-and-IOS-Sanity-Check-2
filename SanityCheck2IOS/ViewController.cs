using Foundation;
using Newtonsoft.Json;
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

            var table = new UITableView(View.Bounds); // defaults to Plain style

            table.Source = new FileTableSource(GetFiles());
            Add(table);
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