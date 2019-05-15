using System;

using pdftron.PDF;
using pdftron.PDF.Tools;
using pdftron.PDF.Tools.Utils;
using pdftron.SDF;
using Polygon = pdftronprivate.PDF.Annots.Polygon;

namespace SanityCheck2
{
    public class NuclearTool : RectCreate
    {
        public static ToolManager.IToolModeBase MODE = ToolManager.ToolMode.AddNewMode((int)Annot.Type.e_Polygon);

        public NuclearTool(PDFViewCtrl ctrl) : base(ctrl)
        {

        }

        public override ToolManager.IToolModeBase ToolMode
        {
            get
            {
                return MODE;
            }
        }

        protected override pdftronprivate.PDF.Annot CreateMarkup(pdftronprivate.PDF.PDFDoc doc, pdftronprivate.PDF.Rect bbox)
        {
            var poly = new Polygon(Polygon.Create(doc, (int)Annot.Type.e_Polygon, bbox));
            var color = Utils.Color2ColorPt(Android.Graphics.Color.Red);
            poly.SetColor(color, 3);
            poly.SetVertex(0, new pdftronprivate.PDF.Point(bbox.X1, bbox.Y1));
            poly.SetVertex(1, new pdftronprivate.PDF.Point(bbox.X1, bbox.Y2));
            poly.SetVertex(2, new pdftronprivate.PDF.Point(bbox.X2, bbox.Y2));
            poly.SetVertex(3, new pdftronprivate.PDF.Point(bbox.X2, bbox.Y1));
            poly.IntentName = pdftronprivate.PDF.Annots.PolyLine.EPolygonCloud;
            poly.BorderEffect = pdftronprivate.PDF.Annots.Markup.ECloudy;
            poly.BorderEffectIntensity = 2.0;
            poly.Rect = bbox;

            ElementWriter writer = new ElementWriter();
            ElementBuilder builder = new ElementBuilder();


            return poly;
        }

        public void SetCustomImage(Annot annot, PDFDoc doc, string imagePath)
        {
            // Initialize a new ElementWriter and ElementBuilder
            ElementWriter writer = new ElementWriter();
            ElementBuilder builder = new ElementBuilder();

            writer.Begin(doc.GetSDFDoc(), true);

            // Initialize the new image
            Image image = Image.Create(doc.GetSDFDoc(), imagePath);
            int w = image.GetImageWidth();
            int h = image.GetImageHeight();

            // Initialize a new image element
            Element element = builder.CreateImage(image, 0, 0, w, h);

            // Write the element
            writer.WritePlacedElement(element);

            // Get the bounding box of the new element
            Rect bbox = new Rect();
            element.GetBBox(bbox);

            // Configure the appearance stream that will be written to the annotation
            Obj new_appearance_stream = writer.End();

            // Set the bounding box to be the rect of the new element
            new_appearance_stream.PutRect(
                "BBox",
                bbox.x1,
                bbox.y1,
                bbox.x2,
                bbox.y2);

            // Overwrite the annotation's appearance with the new appearance stream
            annot.SetAppearance(new_appearance_stream);
        }
    }
}