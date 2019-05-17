using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using Android;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using static Android.Manifest;

namespace SanityCheck2
{
    [Activity(Label = "Activity1", MainLauncher = true)]
    public class FileBrowserActivity : ListActivity
    {
        List<String> items;
        JavaList<ServerFile> files;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            var FileAdapter = new FileBrowserAdapter(this, GetFiles());
            this.ListAdapter = FileAdapter;

            //ListAdapter = new ArrayAdapter<string>(this, Resource.Layout.filebrowser_list_item, items);

            //ListView.TextFilterEnabled = true;
            string[] permissions = { Manifest.Permission.ReadExternalStorage };

            AlertDialog.Builder builder = new AlertDialog.Builder(this);
            builder.SetTitle("Asking for storage permissions");
            builder.SetMessage("This app needs external storage permission to continue");
            builder.SetPositiveButton("Request Permissions", (senderAlert, args) =>
            {
                RequestPermissions(permissions, 0);
            });

            builder.SetNegativeButton("Cancel", (senderAlert, args) =>
            {
                Toast.MakeText(this, "Canceled", ToastLength.Short).Show();
            });

            Dialog dialog = builder.Create();
            dialog.Show();

            ListView.ItemClick += delegate (object sender, AdapterView.ItemClickEventArgs args)
            {
                String filename = files[args.Position].Name;
                System.Diagnostics.Debug.WriteLine(filename);
                Intent intent = new Intent(this, typeof(MainActivity));
                intent.PutExtra("DOCUMENT_TO_LOAD", "http://10.0.3.2:8080/" + filename);
                var extension = System.IO.Path.GetExtension(filename);
                extension = extension.TrimStart('.');
                if (extension == "pdf")
                    intent.PutExtra("ISDOCX", false);
                else if (extension == "docx")
                    intent.PutExtra("ISDOCX", true);
                StartActivity(intent);
            };

        }


        private JavaList<ServerFile> GetFiles()
        {

            files = new JavaList<ServerFile>();


            // get file names from server
            HttpClient client = new HttpClient();
            var uri = new Uri("http://10.0.3.2:8080/getFiles");
            client.Timeout = TimeSpan.FromSeconds(60);
            var response = client.GetAsync(uri).Result;
            var responsecontent = response.Content;
            string responseString = responsecontent.ReadAsStringAsync().Result;
            System.Diagnostics.Debug.WriteLine("reached response " + responseString);
            items = JsonConvert.DeserializeObject<List<String>>(responseString);

            foreach(String s in items)
            {
                ServerFile nextFile = new ServerFile(s, Resource.Drawable.test);
                files.Add(nextFile);
            }

            return files;
        }

    }
}