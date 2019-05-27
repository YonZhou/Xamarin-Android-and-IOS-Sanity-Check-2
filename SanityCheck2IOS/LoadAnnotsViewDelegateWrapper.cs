using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

using Foundation;
using pdftron.FDF;
using pdftron.PDF;
using pdftron.PDF.Controls;
using pdftronprivate;
using UIKit;

namespace SanityCheck2IOS
{
    public class LoadAnnotsViewDelegateWrapper : PTDocumentViewControllerDelegate
    {
        public string fileName { get; set; }

        public override void DidOpenDocument(PTDocumentViewController documentViewController)
        {
            //base.DidOpenDocument(documentViewController);


            WebClient client = new WebClient();
            PTPDFViewCtrl mpdfviewctrl = documentViewController.PdfViewCtrl;
            PTPDFDoc document = mpdfviewctrl.GetDoc();

            string annotationsFile = Path.GetFileNameWithoutExtension(fileName) + "_annots.xml";
            string annotations = null;
            try
            {
                annotations = client.DownloadString("http://laptop-ejbj9ok5:8080" + "/" + annotationsFile);
                //System.Diagnostics.Debug.WriteLine("exported string is " + annotations);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("download annotations failed");
                System.Diagnostics.Debug.WriteLine(e);
            }
            if (annotations != null)
            {
                try
                {
                    mpdfviewctrl.DocLock(true);

                    int previousPage = mpdfviewctrl.GetCurrentPage();
                    var annotationsFDFDoc = FDFDoc.CreateFromXFDF(annotations);

                    var currentPDFDoc = TypeConvertHelper.ConvPdfDocToManaged(document);
                    currentPDFDoc.FDFUpdate(annotationsFDFDoc);
                    //ContentReplacer replacer = new ContentReplacer();
                    //Page page = currentPDFDoc.GetPage(1);
                    //replacer.AddString("FIRST_NAME", "John");
                    //replacer.Process(page);
                    //PTPDFDoc newDocument = TypeConvertHelper.ConvPDFDocToNative(currentPDFDoc);
                    mpdfviewctrl.Update(true);
                    mpdfviewctrl.DocUnlock();


                    //mpdfviewctrl.SetDoc(newDocument);
                    //mpdfviewctrl.SetCurrentPage(previousPage);

                    //mpdfviewctrl.DocUnlock();
                }
                catch (Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e);
                }
            }
        }
    }


}