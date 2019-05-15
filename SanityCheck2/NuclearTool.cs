using System;
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
            // TODO: send log to server
            bool created = CreateImageStamp(Android.Net.Uri.Parse(nukeImage.ToURI().ToString()), 0, null);

        }



    }
}