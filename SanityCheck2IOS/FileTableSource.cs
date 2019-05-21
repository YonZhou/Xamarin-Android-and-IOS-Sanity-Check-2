using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using UIKit;

namespace SanityCheck2IOS
{
    public class FileTableSource : UITableViewSource
    {
        string[] FileNames;
        //string[] ImageURLs;
        string CellIdentifier = "fileTableCell";
        public FileTableSource(string[] fileNames)
        {
            this.FileNames = fileNames;
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
                cell.ImageView.Image = FromUrl("http://LAPTOP-EJBJ9OK5:8080/A17_FlightPlan.png");
            }

            cell.TextLabel.Text = name;

            return cell;
        }

        public override nint RowsInSection(UITableView tableview, nint section)
        {
            return FileNames.Length;
        }

        public override void RowSelected(UITableView tableView, NSIndexPath indexPath)
        {
            UIAlertController okAlertController = UIAlertController.Create("Row Selected", FileNames[indexPath.Row], UIAlertControllerStyle.Alert);
            okAlertController.AddAction(UIAlertAction.Create("OK", UIAlertActionStyle.Default, null));

	        tableView.DeselectRow(indexPath, true);
        }
    }
}