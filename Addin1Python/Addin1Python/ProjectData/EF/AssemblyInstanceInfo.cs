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
<<<<<<< HEAD
        public int Count { get; set; }
        public int Index { get; set; }
        public RebarType RebarType { get; set; }
        public AssemblyInstance AssemblyInstance { get; set; }

        public void AddRebar(Rebar rebar)
        {
            if (AssemblyInstance == null)
            {
                string abc = $"{SingleWPF.Instance.Prefix}-{SingleWPF.Instance.Layer}-{RebarType}-{Count}-{Index}";
                AssemblyInstance = AssemblyInstance.Create(Singleton.Instance.Document, new List<ElementId> { rebar.Id }, rebar.Category.Id);
            }

            AssemblyInstance.AddMemberIds(new List<ElementId> { rebar.Id });
=======
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
>>>>>>> 38c4cfde3c38e33ff6981985bf2686d613a92af5
        }
    }
}
