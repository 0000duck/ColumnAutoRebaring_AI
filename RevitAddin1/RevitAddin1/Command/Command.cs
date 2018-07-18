using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAddin1
{
    #region ConvertToNumber
    [Transaction(TransactionMode.Manual)]
    public class ConvertToNumber : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet easdfasf)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;
            Selection sel = commandData.Application.ActiveUIDocument.Selection;
            Transaction tx = new Transaction(doc, "CovertToNumber");
            tx.Start();

            List<Element> elems = new FilteredElementCollector(doc).WhereElementIsNotElementType().ToList();
            //List<Element> elems = sel.PickObjects(ObjectType.Element).Select(x => doc.GetElement(x)).ToList();
            foreach (Element e in elems)
            {
                string sDate = "", fDate = "";
                try
                {
                    sDate = e.LookupParameter("StartDate").AsString();
                    if (sDate == null || sDate == "") continue;
                    try
                    {
                        fDate = e.LookupParameter("FinishDate").AsString();
                        if (fDate == null || fDate == "")
                        {
                            fDate = "01/01/25";
                        }
                    }
                    catch
                    {
                        fDate = "01/01/25";
                    }
                }
                catch { continue; }

                string[] sDates = sDate.Split('/');
                for (int i = 0; i < sDates.Length; i++)
                {
                    try
                    {
                        e.LookupParameter("CreateDate").Set(long.Parse($"{sDates[2]}{sDates[1]}{sDates[0]}"));
                    }
                    catch
                    {
                        TaskDialog.Show("Revit", $"{e.Id}");
                        throw;
                    }
                }
                string[] fDates = fDate.Split('/');
                for (int i = 0; i < fDates.Length; i++)
                {
                    try
                    {
                        e.LookupParameter("RemoveDate").Set(long.Parse($"{fDates[2]}{fDates[1]}{fDates[0]}"));
                    }
                    catch
                    {
                        TaskDialog.Show("Revit", $"{e.Id}");
                        TaskDialog.Show("Revit", fDate);
                        throw;
                    }
                }
            }

            tx.Commit();
            return Result.Succeeded;
        }
    }
    #endregion

    #region SetView
    //[Transaction(TransactionMode.Manual)]
    //public class SetView : IExternalCommand
    //{
    //    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet easdfasf)
    //    {
    //        Singleton.Instance.UIDocument = commandData.Application.ActiveUIDocument;

    //        Transaction tx = new Transaction(Singleton.Instance.Document, "CovertToNumber");
    //        tx.Start();

    //        View3D v3d = Singleton.Instance.ActiveView as View3D;
    //        //XYZ eyePos = v3d.GetOrientation().EyePosition;
    //        XYZ eyePos = new XYZ(516.711576714, 1501.694764777, 0);
    //        ViewOrientation3D vOri3d = new ViewOrientation3D(new XYZ(eyePos.X, eyePos.Y, ConstantValue.milimet2feet* 40000), XYZ.BasisX, XYZ.BasisY);
    //        v3d.SetOrientation(vOri3d);
    //        v3d.LookupParameter("Target Elevation").Set(ConstantValue.milimet2feet * 8000);

    //        tx.Commit();
    //        return Result.Succeeded;
    //    }
    //}
    #endregion

    #region DeleteAllParameterFilterElements
    [Transaction(TransactionMode.Manual)]
    public class DeleteAllParameterFilterElements : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet easdfasf)
        {
            Singleton.Instance = new Singleton();
            SingleWPF.Instance = new SingleWPF();

            Singleton.Instance.UIDocument = commandData.Application.ActiveUIDocument;

            Transaction tx = new Transaction(Singleton.Instance.Document, "CovertToNumber");
            tx.Start();

            Singleton.Instance.Document.Delete(new FilteredElementCollector(Singleton.Instance.Document).OfClass(typeof(ParameterFilterElement)).Select(x => x.Id).ToList());
            
            tx.Commit();
            return Result.Succeeded;
        }
    }
    #endregion

    #region Auto Viewer
    [Transaction(TransactionMode.Manual)]
    public class ShowForm : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet easdfasf)
        {
            Singleton.Instance = new Singleton();
            SingleWPF.Instance = new SingleWPF();

            Singleton.Instance.UIDocument = commandData.Application.ActiveUIDocument;

            Transaction tx = new Transaction(Singleton.Instance.Document, "CovertToNumber");
            tx.Start();

            Singleton.Instance.ChoosePrefixForm.ShowDialog();
            Singleton.Instance.ViewInfoForm.ShowDialog();
            if (!SingleWPF.Instance.IsCloseFormOK) goto L1;
            RevitUtility.CreateAndDeleteView3Ds();

            L1:
            tx.Commit();
            return Result.Succeeded;
        }
    }
    #endregion
}
