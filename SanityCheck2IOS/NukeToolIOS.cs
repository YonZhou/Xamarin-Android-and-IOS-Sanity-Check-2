using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Foundation;
using ObjCRuntime;
using pdftron.PDF;
using pdftron.PDF.Tools;
using UIKit;

namespace SanityCheck2IOS
{
    public class NukeToolIOS : pdftron.PDF.Tools.PTCreateToolBase
    {

        public NukeToolIOS(PTPDFViewCtrl ctrl) : base(ctrl)
        {
        }

        public override bool CreatesAnnotation
        {
            get
            {
                return true;
            }
        }

        public override Class AnnotClass
        {
            get
            {
                try
                {
                    return new Class(typeof(pdftron.PDF.Tools.PTFreeTextCreate));
                } catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e);
                    return null;
                }
            }
        }

        CoreGraphics.CGContext current = UIGraphics.GetCurrentContext();

    }

}