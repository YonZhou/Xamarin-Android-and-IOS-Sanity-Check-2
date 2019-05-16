using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;

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

        protected override void OnListItemClick(ListView l, View v, int position, long id)
        {
            var item = items[position];
            System.Diagnostics.Debug.WriteLine("Some text");
        }

        private JavaList<ServerFile> GetFiles()
        {

            files = new JavaList<ServerFile>();


            // get file names from server
            HttpClient client = new HttpClient();
            var uri = new Uri("http://10.0.3.2:8080/getFiles");

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