using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Addin1Python
{
    public class AssemblyInstanceInfo
    {
        public int IndexX { get; set; } = -1;
        public int CountX { get; set; } = -1;
        public RebarType RebarType { get; set; }
        public AssemblyInstance AssemblyInstance { get; set; }
        public void AddRebar(RebarInfo rebarInfo)
        {
            if (AssemblyInstance == null)
            {
                AssemblyInstance = AssemblyInstance.Create(Singleton.Instance.Document, new List<ElementId> { rebarInfo.Rebar.Id }, rebarInfo.Rebar.Category.Id);
                AssemblyInstance.LookupParameter("Workset").Set(Utility.GetWorkset().Id.IntegerValue);
                string name = $"{SingleWPF.Instance.Prefix}-{SingleWPF.Instance.Layer}-{rebarInfo.IndexX}_{rebarInfo.CountX}x{rebarInfo.CountY}";
            }
            else
            {
                AssemblyInstance.AddMemberIds(new List<ElementId> { rebarInfo.Rebar.Id });
            }
        }
    }
}
