using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAddin1
{
    public static class RevitUtility
    {
        public static void CreateAndDeleteView3Ds()
        {
            List<string> view3DNames = Singleton.Instance.View3Ds.Select(x => x.Name).ToList();

            int viewInfoCount = Singleton.Instance.ViewInformations.Count;
            int view3dCount = Singleton.Instance.View3Ds.Count;
            for (int i = 0; i < viewInfoCount; i++)
            {
                View3D view3d = CloneView3D(i + 1);
                view3d.SaveOrientationAndLock();
                var filters = view3d.GetFilters();
                if (filters.Count != 0)
                {
                    filters.ToList().ForEach(x => view3d.RemoveFilter(x));
                }
                var obj = Singleton.Instance.ParameterFilterElements;
                ElementId paramFilerElemId = Singleton.Instance.ParameterFilterElements[i].Id;
                view3d.AddFilter(paramFilerElemId);
                view3d.SetFilterVisibility(paramFilerElemId, false);

                for (int j = view3dCount - 1; j >= viewInfoCount; j--)
                {
                    //DeleteView3D(j + 1);
                }
            }
        }
        public static View3D CloneView3D(int index)
        {
            string prefix = SingleWPF.Instance.Prefix;
            var view3ds = Singleton.Instance.View3Ds.Where(x => x.Name == $"V{prefix}{index}");
            if (view3ds.Count() != 0) return view3ds.First();

            View3D v3dClone = ElementTransformUtils.CopyElement(Singleton.Instance.Document, Singleton.Instance.View3Ds[0].Id, XYZ.BasisZ).Select(x => Singleton.Instance.Document.GetElement(x)).First() as View3D;
            v3dClone.Name = $"V{prefix}{index}";
            return v3dClone;
        }
        public static void DeleteView3D(int index)
        {
            var view3ds = Singleton.Instance.View3Ds.Where(x => x.Name == $"V{index}");
            if (view3ds.Count() == 0) return;

            Singleton.Instance.Document.Delete(view3ds.First().Id);
            Singleton.Instance.View3Ds.RemoveAt(index - 1);
        }
        public static List<ParameterFilterElement> CreateParameterFilterElements()
        {
            List<ParameterFilterElement> paramFilterElems = new FilteredElementCollector(Singleton.Instance.Document)
                .OfClass(typeof(ParameterFilterElement)).Where(x => Singleton.Instance.FilterNames.Contains(x.Name)).Cast<ParameterFilterElement>().ToList();
            List<string> paramFilterNames = paramFilterElems.Select(x => x.Name).ToList();
            foreach (ViewInformation viewInfo in Singleton.Instance.ViewInformations)
            {
                if (paramFilterNames.Contains(viewInfo.RevitName)) continue;

                paramFilterElems.Add(CreateParameterFilterELement(viewInfo));
            }

            paramFilterElems.Sort(new ParameterFilterElementSorter());
            return paramFilterElems;
        }
        public static ParameterFilterElement CreateParameterFilterELement(ViewInformation viewInfo)
        {
            Element tempElem = new FilteredElementCollector(Singleton.Instance.Document).OfClass(typeof(Floor)).First();

            Parameter worksetParam = tempElem.LookupParameter("Workset");
            Parameter createDateParam = tempElem.LookupParameter("CreateDate");
            Parameter removeDateParam = tempElem.LookupParameter("RemoveDate");

            FilterRule invisibleContainRule = ParameterFilterRuleFactory.CreateContainsRule(worksetParam.Id, "Invisible", true);
            FilterRule createDateLessRule = ParameterFilterRuleFactory.CreateGreaterRule(createDateParam.Id, viewInfo.ToDate);
            FilterRule removeDateLessRule = ParameterFilterRuleFactory.CreateLessRule(removeDateParam.Id, viewInfo.FromDate);


            ElementParameterFilter invisibleFilter = new ElementParameterFilter(new List<FilterRule> { invisibleContainRule });
            ElementParameterFilter createDateFilter = new ElementParameterFilter(new List<FilterRule> { createDateLessRule });
            ElementParameterFilter removeDateFilter = new ElementParameterFilter(new List<FilterRule> { removeDateLessRule });

            LogicalOrFilter filter = null;
            if (!SingleWPF.Instance.IsSpecific)
            {
                FilterRule specificContainRule = ParameterFilterRuleFactory.CreateContainsRule(worksetParam.Id, "Specific", true);
                ElementParameterFilter specificFilter = new ElementParameterFilter(new List<FilterRule> { specificContainRule });
                filter = new LogicalOrFilter(new List<ElementFilter> { invisibleFilter, createDateFilter, removeDateFilter, specificFilter });
            }
            else
            {
                filter = new LogicalOrFilter(new List<ElementFilter> { invisibleFilter, createDateFilter, removeDateFilter });
            }

            ParameterFilterElement paramFilterElem =
                ParameterFilterElement.Create(Singleton.Instance.Document, viewInfo.RevitName, Singleton.Instance.TargetCategoryIds);
            paramFilterElem.SetElementFilter(filter);

            return paramFilterElem;
        }
    }
}
