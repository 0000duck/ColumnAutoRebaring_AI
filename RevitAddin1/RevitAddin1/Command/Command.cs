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

    [Transaction(TransactionMode.Manual)]
    public class Create3DViews : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet easdfasf)
        {
            Singleton.Instance.Document = commandData.Application.ActiveUIDocument.Document;
            Selection sel = commandData.Application.ActiveUIDocument.Selection;

            Transaction tx = new Transaction(Singleton.Instance.Document, "CovertToNumber");
            tx.Start();

            RevitUtility.CloneView3D(2);

            tx.Commit();
            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    public class ShowForm : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet easdfasf)
        {
            Singleton.Instance.Document = commandData.Application.ActiveUIDocument.Document;
            Selection sel = commandData.Application.ActiveUIDocument.Selection;

            Transaction tx = new Transaction(Singleton.Instance.Document, "CovertToNumber");
            tx.Start();

            Singleton.Instance.ViewInfoForm.ShowDialog();
            //RevitUtility.CreateParameterFilterElements();
            RevitUtility.CreateAndDeleteView3Ds();

            tx.Commit();
            return Result.Succeeded;
        }
    }
}
