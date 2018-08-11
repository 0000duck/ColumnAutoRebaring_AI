using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Addin1Python
{
    public class RebarArcSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if (!(elem is Rebar)) return false;
            if ((elem as Rebar).LookupParameter("Shape").AsValueString() != ConstantValue.SeletedRebarShape) return false;
            return true;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            throw new NotImplementedException();
        }
    }
    public class RebarSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if (!(elem is Rebar)) return false;
            return true;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            throw new NotImplementedException();
        }
    }
}
