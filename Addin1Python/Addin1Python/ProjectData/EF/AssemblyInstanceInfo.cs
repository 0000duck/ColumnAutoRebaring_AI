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
        }
    }
}
