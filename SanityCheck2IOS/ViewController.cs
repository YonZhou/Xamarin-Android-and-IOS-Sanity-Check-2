using Foundation;
using System;
using System.Net;
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

            WebClient client = new WebClient();

            string[] tableItems = new string[] { "A17_FlightPlan" };
            table.Source = new FileTableSource(tableItems);
            Add(table);
        }

        public override void DidReceiveMemoryWarning ()
        {
            base.DidReceiveMemoryWarning ();
            // Release any cached data, images, etc that aren't in use.
        }
    }
}