﻿using Autodesk.Revit.Attributes;
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
    public class Command: IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Singleton.Instance = new Singleton();
            Singleton.Instance.UIApplication = commandData.Application;
            Singleton.Instance.Transaction.Start();

            Singleton.Instance.Prefix = "18B";
            Singleton.Instance.Level = "SSLB3-TS";

            View viewPlan = null;
            foreach (var v in Singleton.Instance.Views)
            {
                ViewFamilyType viewType = Singleton.Instance.Document.GetElement(v.GetTypeId()) as ViewFamilyType;
                if (viewType == null) continue;
                if (viewType.ViewFamily != ViewFamily.StructuralPlan) continue;
                if (v.Name != $"{Singleton.Instance.Prefix}-{Singleton.Instance.Level}-DV") continue;
                viewPlan = v; break;
            }

            List<string> layers = new List<string> { "B1", "B2", "BE2", "T1", "T2", "TE2" };
            foreach (string layer in layers)
            {
                View rebarView = Singleton.Instance.Document.GetElement(ElementTransformUtils.CopyElement(Singleton.Instance.Document, viewPlan.Id, XYZ.BasisZ).First()) as View;
                rebarView.Name = $"{Singleton.Instance.Prefix}-{Singleton.Instance.Level}-{layer}";
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
            Singleton.Instance.UIApplication = commandData.Application;
            Singleton.Instance.Transaction.Start();

            Singleton.Instance.ActiveView.SketchPlane = Singleton.Instance.ActiveSketchPlane;
            Singleton.Instance.ActiveView.HideActiveWorkPlane();

            Singleton.Instance.SelectedXYZ = new XYZ(577.742794242221, 1379.77900850647, 14.7637795275591);

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
                double length = GeomUtil.GetLength(point, Singleton.Instance.SelectedXYZ);

                CircleEquation ce = new CircleEquation(Singleton.Instance.SelectedXYZ, length);
                ce.CalculateDistancesList(GeomUtil.milimeter2Feet(11700));

                foreach (var item in ce.StandardArcs)
                {
                    Singleton.Instance.Rebars.Add(Utility.CreateRebar(item));
                }
            }

            Singleton.Instance.Selection.SetElementIds(Singleton.Instance.Rebars.Select(x => x.Id).ToList());
            Singleton.Instance.Transaction.Commit();
            return Result.Succeeded;
        }
    }
}
