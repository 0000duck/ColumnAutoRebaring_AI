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

                List<ArcInfo> arcInfos = Singleton.Instance.ArcInfos.Where(x => x.CircleEquation.Radius == ce.Radius).ToList();
                int countX = arcInfos.Count;
                RebarType rebarType = arcInfos[0].Arcs.Count == 1 ? RebarType.Type1 : RebarType.Type2;
                for (int i = 0; i < arcInfos.Count; i++)
                {
                    Rebar rebar = Utility.CreateCircleRebar(arcInfos[i].Arcs);
                    Singleton.Instance.Document.Regenerate();
                    rebar.LookupParameter("Workset").Set(Utility.GetWorkset().Id.IntegerValue);
                    Singleton.Instance.CircleRebarInfos.Add(new RebarInfo(rebar, arcInfos[i], i, countX, rebarType));
                }
            }

            RebarInfoDao.Categozie();
            RebarInfoDao.ShowValue();
            RebarInfoDao.RefreshRebars();
            Singleton.Instance.Document.Regenerate();
            AssemblyInstanceInfoDao.CreateAssemblyInstances();

            Singleton.Instance.Transaction.Commit();
            return Result.Succeeded;
        }
    }
    [Transaction(TransactionMode.Manual)]
    public class SetViewCircleRebar : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Singleton.Instance = new Singleton();
            SingleWPF.Instance = new SingleWPF();

            Singleton.Instance.UIApplication = commandData.Application;
            Singleton.Instance.InputViewCircleRebar.ShowDialog();
            if (!SingleWPF.Instance.IsFormClosedOk) return Result.Succeeded;

            Singleton.Instance.Transaction.Start();

            Singleton.Instance.ActiveView.SketchPlane = Singleton.Instance.ActiveSketchPlane;
            Singleton.Instance.ActiveView.HideActiveWorkPlane();

            View view = Utility.CreateView();
            //view.SetWorksetVisibility(Utility.GetWorkset().Id, WorksetVisibility.Visible);
            for (int i = 0; i < Singleton.Instance.AssemblyInstances.Count; i++)
            {
                List<Rebar> rebars = Singleton.Instance.AssemblyInstances[i].GetMemberIds().Select(x => Singleton.Instance.Document.GetElement(x) as Rebar).ToList();
                int index = int.Parse(rebars.First().LookupParameter("Comments").AsString().Split('_').Last());
                string[] arcInfos = rebars[index].LookupParameter("Comments").AsString().Split('(', ')')[1].Split('_');
                List<double> arcInputs = new List<double>();
                for (int j = 0; j < arcInfos.Length - 1; j++)
                {
                    arcInputs.Add(double.Parse(arcInfos[j]));
                }
                RebarType rebarType = (RebarType)Enum.Parse(typeof(RebarType), arcInfos.Last());
                double offset = 0;
                if (i % 2 == 1)
                {
                    offset = GeomUtil.milimeter2Feet(200);
                }
                if (i % 2 == 0 && i == Singleton.Instance.AssemblyInstances.Count - 1)
                {
                    offset = GeomUtil.milimeter2Feet(-200);
                }
                ArcInfo arcInfo = new ArcInfo(arcInputs[0], arcInputs[1], arcInputs[2], offset, rebarType);
                arcInfo.Curves.ForEach(x => Singleton.Instance.Document.Create.NewDetailCurve(view, x));
                Utility.CreateTextNote(view, Singleton.Instance.AssemblyInstances[i], arcInfo);
                if (rebarType == RebarType.Type1) Utility.CreateGroup(view, arcInfo);
            }

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

            double length = GeomUtil.GetLength(new XYZ(point.X, point.Y, 0), new XYZ(Singleton.Instance.SelectedXYZ.X, Singleton.Instance.SelectedXYZ.Y, 0));

            CircleEquation ce = new CircleEquation(Singleton.Instance.SelectedXYZ, length, point.Z);
            ce.CalculateNumberWithAngle(SingleWPF.Instance.Spacing, SingleWPF.Instance.Angle, false);

            rebars.ForEach(x => Utility.CreateCentrifugalRebar(x, ce));
            AssemblyInstanceInfoCentrifugalDao.CreateAssemblyInstanceInfoCentrifugals();

            Singleton.Instance.Transaction.Commit();
            return Result.Succeeded;
        }
    }
    [Transaction(TransactionMode.Manual)]
    public class SelectRebars : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Singleton.Instance = new Singleton();
            SingleWPF.Instance = new SingleWPF();

            Singleton.Instance.UIApplication = commandData.Application;
            Singleton.Instance.Transaction.Start();
            SingleWPF.Instance.Prefix = "18E";
            SingleWPF.Instance.Layer = "T1";

            List<Rebar> rebars = Singleton.Instance.Selection.PickElementsByRectangle(new RebarSelectionFilter()).Cast<Rebar>().ToList();
            List<Rebar> filRebars = new List<Rebar>();
            List<string> types = new List<string> { "Ø20 : Shape TC_L_01", "T20 : Shape TC_L_01" };
            foreach (Rebar rb in rebars)
            {
                if (rb.LookupParameter("Workset").AsInteger() != Utility.GetWorkset().Id.IntegerValue) continue;
                foreach (string type in types)
                {
                    if (rb.Name == type)
                    {
                        filRebars.Add(rb);
                    }
                }
            }

            Singleton.Instance.Selection.SetElementIds(filRebars.Select(x => x.Id).ToList());
            Singleton.Instance.Transaction.Commit();
            return Result.Succeeded;
        }
    }
    [Transaction(TransactionMode.Manual)]
    public class CreateViewCircleBarDetailLine : IExternalCommand
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

            View view = Utility.CreateView();

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

                List<ArcInfo> arcInfos = Singleton.Instance.ArcInfos.Where(x => x.CircleEquation.Radius == ce.Radius).ToList();
                foreach (ArcInfo arcInfo in arcInfos)
                {
                    arcInfo.Arcs.ForEach(x => Singleton.Instance.Document.Create.NewDetailCurve(view, x));
                }
            }

            Singleton.Instance.Transaction.Commit();
            return Result.Succeeded;
        }
    }
}
