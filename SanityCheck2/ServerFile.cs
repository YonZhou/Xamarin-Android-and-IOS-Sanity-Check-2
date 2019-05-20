using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace SanityCheck2
{
    class ServerFile
    {
        private String name;
        private Bitmap image;

        public ServerFile(string name, String imageURL)
        {
            this.name = name;
            this.image = GetImageBitmapFromUrl(imageURL);
        }

        public string Name
        {
            get { return name; }
        }

        public Bitmap Image
        {
            get { return image; }
        }
        private Bitmap GetImageBitmapFromUrl(string url)
        {
            Bitmap imageBitmap = null;

            using (var webClient = new WebClient())
            {
                var imageBytes = webClient.DownloadData(url);
                if (imageBytes != null && imageBytes.Length > 0)
                {
                    imageBitmap = BitmapFactory.DecodeByteArray(imageBytes, 0, imageBytes.Length);
                }
            }

            return imageBitmap;
        }
    }

}