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

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // get file names from server
            HttpClient client = new HttpClient();
            var uri = new Uri("http://10.0.3.2:8080/getFiles");

            var response = client.GetAsync(uri).Result;
            var responsecontent = response.Content;
            string responseString = responsecontent.ReadAsStringAsync().Result;
            System.Diagnostics.Debug.WriteLine("reached response " + responseString);
            items = JsonConvert.DeserializeObject<List<String>>(responseString);

            ListAdapter = new ArrayAdapter<string>(this, Resource.Layout.filebrowser_list_item, items);

            ListView.TextFilterEnabled = true;

            ListView.ItemClick += delegate (object sender, AdapterView.ItemClickEventArgs args)
            {
                System.Diagnostics.Debug.WriteLine(((TextView)args.View).Text);
                Intent intent = new Intent(this, typeof(MainActivity));
                intent.PutExtra("DOCUMENT_TO_LOAD", "http://10.0.3.2:8080/" + ((TextView)args.View).Text);
                StartActivity(intent);
            };

        }

        protected override void OnListItemClick(ListView l, View v, int position, long id)
        {
            var item = items[position];
            System.Diagnostics.Debug.WriteLine("Some text");
        }

        public async void MakeRequestGetLists()
        {
            HttpClient client = new HttpClient();
            var uri = new Uri("http://10.0.3.2:8080/getFiles");

            var response = await client.GetAsync(uri);

            var content = response.Content.ReadAsStringAsync();
            System.Diagnostics.Debug.WriteLine(content);
        }

    }
}