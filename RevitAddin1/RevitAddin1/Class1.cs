using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI.Selection;
using System.Drawing;

namespace RevitAddin1
{
    [Transaction(TransactionMode.Manual)]
    public class RenameFilterParameter : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;
            Transaction tx = new Transaction(doc, "tx");
            tx.Start();

            Singleton.Instance.Input.ShowDialog();
            Parameter levelParam = new FilteredElementCollector(doc).OfClass(typeof(Rebar)).Where(x => x != null).First().LookupParameter("Level");
            ParameterFilterElement paramfilterElem = new FilteredElementCollector(doc).OfClass(typeof(ParameterFilterElement)).Cast<ParameterFilterElement>().Where(x => x.Name == "0-StandardLevel").First();
            FilterRule filterRule = ParameterFilterRuleFactory.CreateNotEqualsRule(levelParam.Id, Singleton.Instance.InputParameter, true);
            paramfilterElem.SetRules(new List<FilterRule> { filterRule });


            tx.Commit();
            return Result.Succeeded;
        }
    }
    [Transaction(TransactionMode.Manual)]
    public class ShowLocationRebar : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;
            Transaction tx = new Transaction(doc, "tx");
            tx.Start();

            Parameter levelParam = new FilteredElementCollector(doc).OfClass(typeof(Rebar)).Where(x => x != null).First().LookupParameter("Location");
            ParameterFilterElement paramfilterElem = new FilteredElementCollector(doc).OfClass(typeof(ParameterFilterElement)).Cast<ParameterFilterElement>().Where(x => x.Name == "0-LocationRebar").First();

            TaskDialog.Show("Revit","Show L1");
            FilterRule filterRule = ParameterFilterRuleFactory.CreateEqualsRule(levelParam.Id, "L2", true);
            paramfilterElem.SetRules(new List<FilterRule> { filterRule });
            doc.Regenerate();

            TaskDialog.Show("Revit", "Show L2");
            filterRule = ParameterFilterRuleFactory.CreateEqualsRule(levelParam.Id, "L1", true);
            paramfilterElem.SetRules(new List<FilterRule> { filterRule });
            doc.Regenerate();

            TaskDialog.Show("Revit", "Show All");
            filterRule = ParameterFilterRuleFactory.CreateEqualsRule(levelParam.Id, "L", true);
            paramfilterElem.SetRules(new List<FilterRule> { filterRule });

            tx.Commit();
            return Result.Succeeded;
        }
    }
    //[Transaction(TransactionMode.Manual)]
    //public class Imager : IExternalCommand
    //{
    //    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    //    {
    //        Document doc = commandData.Application.ActiveUIDocument.Document;
    //        Selection sel = commandData.Application.ActiveUIDocument.Selection;
    //        Transaction tx = new Transaction(doc, "ABC");
    //        tx.Start();

    //        List<ImageType> imageTypes = new FilteredElementCollector(doc).OfClass(typeof(ImageType)).Cast<ImageType>().ToList();
    //        List<string> names = imageTypes.Select(x => x.Name).ToList(); ;

    //        tx.Commit();
    //        return Result.Succeeded;
    //    }
    //}

    //[Transaction(TransactionMode.Manual)]
    //public class Class2 : IExternalCommand
    //{
    //    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    //    {
    //        Document doc = commandData.Application.ActiveUIDocument.Document;
    //        Transaction tx = new Transaction(doc, "tx");
    //        tx.Start();

    //        List<Element> rebars = new FilteredElementCollector(doc).OfClass(typeof(Rebar)).ToList();
    //        foreach (Element rebar in rebars)
    //        {
    //            if (rebar == null) continue;
    //            string comment = "";
    //            try
    //            {
    //                comment = rebar.LookupParameter("Comments").AsString();
    //            }
    //            catch { }
    //            if (comment != "add-in") continue;
    //            string partition = "";
    //            try
    //            {
    //                partition = rebar.LookupParameter("Partition").AsString();
    //            }
    //            catch { }

    //            if (partition == "") continue;
    //            List<string> splitString = partition.Split('_', '-').ToList();
    //            string newPartition = "F1-F3-" + splitString[0];
    //            rebar.LookupParameter("Partition").Set(newPartition);
    //        }


    //        tx.Commit();
    //        return Result.Succeeded;
    //    }
    //}
    //[Transaction(TransactionMode.Manual)]
    //public class ChangeStirrupFamily : IExternalCommand
    //{
    //    public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
    //    {
    //        Document doc = commandData.Application.ActiveUIDocument.Document;
    //        Transaction tx = new Transaction(doc, "tx");
    //        tx.Start();

    //        List<Rebar> rebars = new FilteredElementCollector(doc).OfClass(typeof(Rebar)).Cast<Rebar>().ToList();
    //        RebarShape shapeTD_01 = new FilteredElementCollector(doc).OfClass(typeof(RebarShape)).Cast<RebarShape>().Where(x => x.Name == "TD_01").First();
    //        foreach (Rebar rebar in rebars)
    //        {
    //            if (rebar == null) continue;

    //            string comments = "";
    //            try
    //            {
    //                comments = rebar.LookupParameter("Comments").AsString();
    //            }
    //            catch { }
    //            if (comments != "add-in") continue;

    //            RebarShape shape = doc.GetElement(rebar.LookupParameter("Shape").AsElementId()) as RebarShape;
    //            if (shape.Name != "TD_02") continue;

    //            rebar.LookupParameter("Shape").Set(shapeTD_01.Id);
    //        }

    //        tx.Commit();
    //        return Result.Succeeded;
    //    }
    //}
}
