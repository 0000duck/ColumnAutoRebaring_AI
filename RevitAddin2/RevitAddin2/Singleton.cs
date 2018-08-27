using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAddin2
{
    public class Singleton
    {
        private Document document;
        private Selection selection;
        private List<Rebar> filterRebars;
        private View activeView;
        private Plane plane;
        private SketchPlane sketchPlane;

        public static Singleton Instance { get; set; }
        public UIDocument UIDocument { get; set; }
        public Document Document
        {
            get
            {
                if (document == null)
                    document = UIDocument.Document;
                return document;
            }
        }
        public Selection Selection
        {
            get
            {
                if (selection == null)
                    selection = UIDocument.Selection;
                return selection;
            }
        }
        public List<Rebar> FilterRebars
        {
            get
            {
                if (filterRebars == null)
                    filterRebars = new List<Rebar>();
                return filterRebars;
            }
        }
        public View ActiveView
        {
            get
            {
                if (activeView == null)
                    activeView = Document.ActiveView;
                return activeView;
            }
        }
        public Plane Plane
        {
            get
            {
                if (plane == null)
                    plane = Plane.CreateByOriginAndBasis(ActiveView.Origin, ActiveView.RightDirection, ActiveView.UpDirection);
                return plane;
            }
        }
        public SketchPlane SketchPlane
        {
            get
            {
                if (sketchPlane == null)
                    sketchPlane = SketchPlane.Create(Document, Plane);
                return sketchPlane;
            }
        }
        public XYZ OriginPoint { get; set; } = new XYZ(575.110526845, 1501.694764777, 0);
    }
}
