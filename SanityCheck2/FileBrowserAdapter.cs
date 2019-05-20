using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Object = Java.Lang.Object;

namespace SanityCheck2
{
    class FileBrowserAdapter : BaseAdapter
    {
        private readonly Context c;
        private readonly JavaList<ServerFile> files;
        private LayoutInflater inflater;

        /*
         * CONSTRUCTOR
         */
        public FileBrowserAdapter(Context c, JavaList<ServerFile> files)
        {
            this.c = c;
            this.files = files;
        }

        /*
         * RETURN SPACECRAFT
         */
        public override Object GetItem(int position)
        {
            return files.Get(position);
        }

        /*
         * SPACECRAFT ID
         */
        public override long GetItemId(int position)
        {
            return position;
        }

        /*
         * RETURN INFLATED VIEW
         */
        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            //INITIALIZE INFLATER
            if (inflater == null)
            {
                inflater = (LayoutInflater)c.GetSystemService(Context.LayoutInflaterService);
            }

            //INFLATE OUR MODEL LAYOUT
            if (convertView == null)
            {
                convertView = inflater.Inflate(Resource.Layout.filebrowser_list_item2, parent, false);
            }

            //BIND DATA
            CustomAdapterViewHolder holder = new CustomAdapterViewHolder(convertView)
            {
                NameTxt = { Text = files[position].Name }
            };
            holder.Img.SetImageResource(files[position].Image);

            //convertView.SetBackgroundColor(Color.LightBlue);

            return convertView;
        }

        /*
         * TOTAL NUM OF FILES
         */
        public override int Count
        {
            get { return files.Size(); }
        }
    }

    class CustomAdapterViewHolder : Java.Lang.Object
    {
        //adapter views to re-use
        public TextView NameTxt;
        public ImageView Img;

        public CustomAdapterViewHolder(View itemView)
        {
            NameTxt = itemView.FindViewById<TextView>(Resource.Id.nameTxt);
            Img = itemView.FindViewById<ImageView>(Resource.Id.fileThumbnail);
        }
    }
}