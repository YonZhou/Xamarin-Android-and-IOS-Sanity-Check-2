using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace SanityCheck2
{
    [Activity(Label = "Activity1", MainLauncher = true)]
    public class FileBrowserActivity : Activity
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.fileBrowserLayout);

            Button button = FindViewById<Button>(Resource.Id.load1);
            button.Click += delegate
            {
                Intent toSwitch = new Intent(this, typeof(MainActivity));
                this.StartActivity(toSwitch);

            };
        }
    }
}