using System;
using System.Net.Http;
using Java.IO;
using pdftron.PDF;
using pdftron.PDF.Tools;
using pdftron.PDF.Tools.Utils;
using pdftron.SDF;
using Stamper = pdftron.PDF.Tools.Stamper;

namespace SanityCheck2
{
    public class NuclearTool : Stamper
    {
        public static ToolManager.IToolModeBase MODE = ToolManager.ToolMode.AddNewMode((int)Annot.Type.e_Stamp);
        public File nukeImage;

        public NuclearTool(PDFViewCtrl ctrl, File image) : base(ctrl)
        {
            this.nukeImage = image;
        }

        public override ToolManager.IToolModeBase ToolMode
        {
            get
            {
                return MODE;
            }
        }

        protected override void AddStamp()

        {
            var client = new HttpClient();
            client.BaseAddress = new Uri("http://10.0.3.2:8080");
            client.PostAsync("/logNuclear", null);
            bool created = CreateImageStamp(Android.Net.Uri.Parse(nukeImage.ToURI().ToString()), 0, null);
            // parsed URI becomes file:/data/user/0/com.companyname.SanityCheck2/files/nuclear_hazard.png
            System.Diagnostics.Debug.WriteLine("flag1 " + Android.Net.Uri.Parse(nukeImage.ToURI().ToString()));
        }



    }
}