using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Geometry;
using Autodesk.Revit.UI.Selection;

namespace RevitAddin2
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Singleton.Instance = new Singleton();
            Singleton.Instance.UIDocument = commandData.Application.ActiveUIDocument;

            Transaction transaction = new Transaction(Singleton.Instance.Document, "BC");
            transaction.Start();

            Singleton.Instance.ActiveView.SketchPlane = Singleton.Instance.SketchPlane;
            Singleton.Instance.ActiveView.HideActiveWorkPlane();

            while (true)
            {
                try
                {
                    AddFilterRebars();
                }
                catch (Autodesk.Revit.Exceptions.OperationCanceledException)
                {
                    break;
                }
            }

            Singleton.Instance.Selection.SetElementIds(Singleton.Instance.FilterRebars.Select(x => x.Id).ToList());
            transaction.Commit();
            return Result.Succeeded;
        }
        public void AddFilterRebars()
        {
            List<Rebar> rebars = new FilteredElementCollector(Singleton.Instance.Document).OfClass(typeof(Rebar)).Cast<Rebar>().ToList();

            XYZ p0 = Singleton.Instance.Selection.PickPoint();
            double measure = new XYZ(p0.X - Singleton.Instance.OriginPoint.X, p0.Y - Singleton.Instance.OriginPoint.Y, 0).GetLength();
            foreach (Rebar rb in rebars)
            {
                XYZ p1 = rb.GetCenterlineCurves(true, false, false, MultiplanarOption.IncludeOnlyPlanarCurves, 0).First().GetEndPoint(0);
                double dist = new XYZ(p1.X - Singleton.Instance.OriginPoint.X, p1.Y - Singleton.Instance.OriginPoint.Y, 0).GetLength();
                if (GeomUtil.IsEqual(measure, dist))
                {
                    if (GeomUtil.IsEqual(p0.Z, p1.Z))
                        Singleton.Instance.FilterRebars.Add(rb);
                }
            }
        }
    }

    [Transaction(TransactionMode.Manual)]
    public class Command1 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Singleton.Instance = new Singleton();
            Singleton.Instance.UIDocument = commandData.Application.ActiveUIDocument;

            Transaction transaction = new Transaction(Singleton.Instance.Document, "BC");
            transaction.Start();

            Singleton.Instance.ActiveView.SketchPlane = Singleton.Instance.SketchPlane;
            Singleton.Instance.ActiveView.HideActiveWorkPlane();

            Group group = Singleton.Instance.Document.GetElement(Singleton.Instance.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element, new GroupSelectionFilter())) as Group;
            XYZ pnt = (group.Location as LocationPoint).Point;
            DetailArc detailArc1 = Singleton.Instance.Document.GetElement(Singleton.Instance.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element, new DetailArcSelectionFilter())) as DetailArc;
            DetailArc detailArc2 = Singleton.Instance.Document.GetElement(Singleton.Instance.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element, new DetailArcSelectionFilter())) as DetailArc;
            Curve c1 = detailArc1.GeometryCurve;
            Curve c2 = detailArc2.GeometryCurve;
            XYZ vec = c1.GetEndPoint(1) - c2.GetEndPoint(0);

            double rad = vec.Y < 0 ? vec.AngleTo(XYZ.BasisX) + Math.PI / 2 : -vec.AngleTo(XYZ.BasisX);
            ElementTransformUtils.RotateElement(Singleton.Instance.Document, group.Id, Line.CreateBound(pnt, pnt + XYZ.BasisZ), rad);
            //txtNote.Coord = (c.GetEndPoint(1) + c.GetEndPoint(0)) / 2;

            transaction.Commit();
            return Result.Succeeded;
        }
    }
    [Transaction(TransactionMode.Manual)]
    public class Command2 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Singleton.Instance = new Singleton();
            Singleton.Instance.UIDocument = commandData.Application.ActiveUIDocument;

            Transaction transaction = new Transaction(Singleton.Instance.Document, "BC");
            transaction.Start();

            Singleton.Instance.ActiveView.SketchPlane = Singleton.Instance.SketchPlane;
            Singleton.Instance.ActiveView.HideActiveWorkPlane();

            List<GraphicsStyle> linePatElems = new FilteredElementCollector(Singleton.Instance.Document).OfClass(typeof(GraphicsStyle)).Cast<GraphicsStyle>().ToList();
            GraphicsStyle lineReinfBarPatElem = linePatElems.Where(x => x.Name == "HB_Reinf Bar").First();

            AssemblyInstance assIns = Singleton.Instance.Document.GetElement(Singleton.Instance.Selection.PickObject(Autodesk.Revit.UI.Selection.ObjectType.Element, new AssemblyInstanceSelectionFilter())) as AssemblyInstance;
            List<Rebar> rebars = assIns.GetMemberIds().Select(x => Singleton.Instance.Document.GetElement(x)).Cast<Rebar>().ToList();
            rebars.Sort(new RebarSorter());

            Curve curve = rebars[1].GetCenterlineCurves(true, false, false, MultiplanarOption.IncludeOnlyPlanarCurves, 0).First();

            XYZ vec = curve.GetEndPoint(1) - curve.GetEndPoint(0);
            XYZ midpnt = (curve.GetEndPoint(1) + curve.GetEndPoint(0)) / 2;
            string index1 = rebars[0].LookupParameter("Rebar Number").AsString();
            string index2 = rebars[rebars.Count - 1].LookupParameter("Rebar Number").AsString();
            double len1 = Math.Round(GeomUtil.feet2Milimeter(rebars[0].LookupParameter("Bar Length").AsDouble()));
            double len2 = Math.Round(GeomUtil.feet2Milimeter(rebars[rebars.Count - 1].LookupParameter("Bar Length").AsDouble()));
            string type = Singleton.Instance.Document.GetElement(rebars[0].GetTypeId()).Name;

            string combine = $"({index1}-{index2}) {rebars.Count} {type}/{len1}~{len2}";

            var detailCurve = Singleton.Instance.Document.Create.NewDetailCurve(Singleton.Instance.Document.ActiveView, curve);
            var txtNote = TextNote.Create(Singleton.Instance.Document, Singleton.Instance.ActiveView.Id, midpnt, combine, new ElementId(309328));
            detailCurve.LineStyle = lineReinfBarPatElem;

            Singleton.Instance.Document.Regenerate();
            double rad = vec.Y < 0 ? -vec.AngleTo(XYZ.BasisX) : vec.AngleTo(XYZ.BasisX);
            ElementTransformUtils.RotateElement(Singleton.Instance.Document, txtNote.Id, Line.CreateBound(midpnt, midpnt + XYZ.BasisZ), rad);

            assIns.LookupParameter("Temp").Set("F");

            Singleton.Instance.ActiveView.SetCategoryHidden(new ElementId(BuiltInCategory.OST_Rebar), true);
            Singleton.Instance.ActiveView.SetCategoryHidden(new ElementId(BuiltInCategory.OST_Rebar), false);

            transaction.Commit();
            return Result.Succeeded;
        }
    }
    [Transaction(TransactionMode.Manual)]
    public class Command3 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Singleton.Instance = new Singleton();
            Singleton.Instance.UIDocument = commandData.Application.ActiveUIDocument;

            Transaction transaction = new Transaction(Singleton.Instance.Document, "BC");
            transaction.Start();

            Singleton.Instance.ActiveView.SketchPlane = Singleton.Instance.SketchPlane;
            Singleton.Instance.ActiveView.HideActiveWorkPlane();

            List<GraphicsStyle> linePatElems = new FilteredElementCollector(Singleton.Instance.Document).OfClass(typeof(GraphicsStyle)).Cast<GraphicsStyle>().ToList();
            GraphicsStyle lineReinfBarPatElem = linePatElems.Where(x => x.Name == "HB_Reinf Bar").First();

            List<Rebar> rebars = Singleton.Instance.Selection.PickElementsByRectangle(new RebarSelectionFilter()).Cast<Rebar>().ToList();
            rebars.Sort(new RebarSorter());

            Curve curve = rebars[rebars.Count - 3].GetCenterlineCurves(true, false, false, MultiplanarOption.IncludeOnlyPlanarCurves, 0).First();

            var detailCurve = Singleton.Instance.Document.Create.NewDetailCurve(Singleton.Instance.Document.ActiveView, curve);
            detailCurve.LineStyle = lineReinfBarPatElem;

            transaction.Commit();
            return Result.Succeeded;
        }
    }

    class RebarSorter : IComparer<Rebar>
    {
        public int Compare(Rebar x, Rebar y)
        {
            double l1 = x.LookupParameter("Bar Length").AsDouble();
            double l2 = y.LookupParameter("Bar Length").AsDouble();
            if (!GeomUtil.IsEqual(l1, l2))
                return l1.CompareTo(l2);
            XYZ p1 = x.GetCenterlineCurves(true, false, false, MultiplanarOption.IncludeOnlyPlanarCurves, 0).First().GetEndPoint(0);
            XYZ p2 = y.GetCenterlineCurves(true, false, false, MultiplanarOption.IncludeOnlyPlanarCurves, 0).First().GetEndPoint(0);
            if (!GeomUtil.IsEqual(p1.X, p2.X)) return p1.X.CompareTo(p2.X);
            return p1.Y.CompareTo(p2.Y);
        }
    }
    class RebarSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if (elem is Rebar) return true;
            return false;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            throw new NotImplementedException();
        }
    }
    class AssemblyInstanceSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if (elem is AssemblyInstance) return true;
            return false;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            throw new NotImplementedException();
        }
    }
    class GroupSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if (elem is Group) return true;
            return false;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            throw new NotImplementedException();
        }
    }
    class TextNoteSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if (elem is TextNote) return true;
            return false;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            throw new NotImplementedException();
        }
    }
    class DetailArcSelectionFilter : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if (elem is DetailArc) return true;
            return false;
        }

        public bool AllowReference(Reference reference, XYZ position)
        {
            throw new NotImplementedException();
        }
    }
    [Transaction(TransactionMode.Manual)]
    public class Command5 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Singleton.Instance = new Singleton();
            Singleton.Instance.UIDocument = commandData.Application.ActiveUIDocument;

            Transaction transaction = new Transaction(Singleton.Instance.Document, "BC");
            transaction.Start();

            Singleton.Instance.ActiveView.SketchPlane = Singleton.Instance.SketchPlane;
            Singleton.Instance.ActiveView.HideActiveWorkPlane();

            List<Rebar> rebars = Singleton.Instance.Selection.PickElementsByRectangle(new RebarSelectionFilter()).Cast<Rebar>().ToList();
            foreach (Rebar rb in rebars)
            {
                //string shape = "";
                //try
                //{
                //    rb.LookupParameter("Shape").AsValueString();
                //}
                //catch { }
                if (rb.Name == "Ø20 : Shape TC_V_360_03")
                {
                    Singleton.Instance.FilterRebars.Add(rb);
                }
            }

            Singleton.Instance.Selection.SetElementIds(Singleton.Instance.FilterRebars.Select(x => x.Id).ToList());
            transaction.Commit();
            return Result.Succeeded;
        }
    }
}
