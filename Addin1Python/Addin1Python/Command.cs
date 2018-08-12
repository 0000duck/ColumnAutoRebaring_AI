using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.UI.Selection;
using Geometry;

namespace Addin1Python
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Singleton.Instance = new Singleton();
            Singleton.Instance.UIApplication = commandData.Application;
            Singleton.Instance.Transaction.Start();

            SingleWPF.Instance.Prefix = "18B";
            Singleton.Instance.Level = "SSLB3-TS";

            View viewPlan = null;
            foreach (var v in Singleton.Instance.Views)
            {
                ViewFamilyType viewType = Singleton.Instance.Document.GetElement(v.GetTypeId()) as ViewFamilyType;
                if (viewType == null) continue;
                if (viewType.ViewFamily != ViewFamily.StructuralPlan) continue;
                if (v.Name != $"{SingleWPF.Instance.Prefix}-{Singleton.Instance.Level}-DV") continue;
                viewPlan = v; break;
            }

            List<string> layers = new List<string> { "B1", "B2", "BE2", "T1", "T2", "TE2" };
            foreach (string layer in layers)
            {
                View rebarView = Singleton.Instance.Document.GetElement(ElementTransformUtils.CopyElement(Singleton.Instance.Document, viewPlan.Id, XYZ.BasisZ).First()) as View;
                rebarView.Name = $"{SingleWPF.Instance.Prefix}-{Singleton.Instance.Level}-{layer}";
                rebarView.ViewTemplateId = Singleton.Instance.RebarPlanTemplateView.Id;
                rebarView.SetWorksetVisibility(Utility.GetWorkset(layer).Id, WorksetVisibility.Visible);
            }

            Singleton.Instance.Transaction.Commit();
            return Result.Succeeded;
        }
    }
    [Transaction(TransactionMode.Manual)]
    public class CreateCircleRebar : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Singleton.Instance = new Singleton();
            SingleWPF.Instance = new SingleWPF();

            Singleton.Instance.UIApplication = commandData.Application;
            Singleton.Instance.InputForm.ShowDialog();
            if (!SingleWPF.Instance.IsFormClosedOk) return Result.Succeeded;

            Singleton.Instance.Transaction.Start();

            Singleton.Instance.ActiveView.SketchPlane = Singleton.Instance.ActiveSketchPlane;
            Singleton.Instance.ActiveView.HideActiveWorkPlane();

            List<XYZ> points = new List<XYZ>();
            while (true)
            {
                try
                {
                    points.Add(Singleton.Instance.Selection.PickPoint());
                }
                catch (Autodesk.Revit.Exceptions.OperationCanceledException)
                {
                    break;
                }
            }

            foreach (XYZ point in points)
            {
                double length = GeomUtil.GetLength(new XYZ(point.X, 0, 0), new XYZ(Singleton.Instance.SelectedXYZ.X, 0, 0));

                CircleEquation ce = new CircleEquation(Singleton.Instance.SelectedXYZ, length, point.Z);
                ce.CalculateDistancesList(GeomUtil.milimeter2Feet(11700), SingleWPF.Instance.IsOtherwiseClock);

                int countX = ce.StandardArcs.Count;
                RebarType rebarType = ce.StandardArcs[0].Count == 1 ? RebarType.Type1 : RebarType.Type2;
                for (int i = 0; i < ce.StandardArcs.Count; i++)
                {
                    Rebar rebar = Utility.CreateCircleRebar(ce.StandardArcs[i]);
                    rebar.LookupParameter("Workset").Set(Utility.GetWorkset().Id.IntegerValue);

                    Singleton.Instance.RebarInfos.Add(new RebarInfo(rebar, i, countX, ce.Radius, rebarType));
                }
            }

            RebarInfoDao.Categozie();
            Singleton.Instance.Document.Regenerate();
            AssemblyInstanceInfoDao.CreateAssemblyInstances();

            Singleton.Instance.Transaction.Commit();
            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    public class CreateCentrifugalRebar : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Singleton.Instance = new Singleton();
            SingleWPF.Instance = new SingleWPF();

            Singleton.Instance.UIApplication = commandData.Application;
            Singleton.Instance.InputCentrifugalForm.ShowDialog();
            if (!SingleWPF.Instance.IsFormClosedOk) return Result.Succeeded;

            Singleton.Instance.Transaction.Start();

            Singleton.Instance.ActiveView.SketchPlane = Singleton.Instance.ActiveSketchPlane;
            Singleton.Instance.ActiveView.HideActiveWorkPlane();

            List<Rebar> rebars = new List<Rebar>();
            while (true)
            {
                try
                {
                    rebars.Add(Singleton.Instance.Document.GetElement(Singleton.Instance.Selection.PickObject(ObjectType.Element, new RebarSelectionFilter())) as Rebar);
                }
                catch (Autodesk.Revit.Exceptions.OperationCanceledException)
                {
                    break;
                }
            }
            XYZ point = Singleton.Instance.Selection.PickPoint();

            double length = GeomUtil.GetLength(new XYZ(point.X, 0, 0), new XYZ(Singleton.Instance.SelectedXYZ.X, 0, 0));

            CircleEquation ce = new CircleEquation(Singleton.Instance.SelectedXYZ, length, point.Z);
            ce.CalculateNumberWithAngle(SingleWPF.Instance.Spacing, SingleWPF.Instance.Angle, false);

            rebars.ForEach(x => Utility.CreateCentrifugalRebar(x, ce));

            Singleton.Instance.Transaction.Commit();
            return Result.Succeeded;
        }
    }
}
