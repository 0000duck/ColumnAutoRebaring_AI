using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using HandleDatabse.Database.Dao;
using HandleDatabse.Database.Others;
using HandleDatabse.ProjectData.Dao;
using HandleDatabse.ProjectData.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PSingleton = Project.Singleton;

namespace Demo_ColumnAutoRebaring
{
    [Transaction(TransactionMode.Manual)]
    public class DeleteCommand : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            PSingleton.Instance.Document = commandData.Application.ActiveUIDocument.Document;
            Transaction tx = new Transaction(PSingleton.Instance.Document, "AutoRebaring_AI");
            tx.Start();

            List<Element> elems = new FilteredElementCollector(PSingleton.Instance.Document).OfClass(typeof(Rebar)).ToList();
            List<ElementId> elemIds = new List<ElementId>();
            foreach (Element elem in elems)
            {
                if (elem == null) continue;
                string s = "";
                try
                {
                    s = elem.LookupParameter("Comments").AsString();
                }
                catch { }
                if (s != "add-in") continue;
                elemIds.Add(elem.Id);
            }
            PSingleton.Instance.Document.Delete(elemIds);

            tx.Commit();
            return Result.Succeeded;
        }
    }
}
