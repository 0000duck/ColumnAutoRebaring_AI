using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Addin1Python
{
    public class AssemblyInstanceCentrifugalInfo
    {
        public RebarType RebarType { get; set; }
        public AssemblyInstance AssemblyInstance { get; set; }
        public AssemblyInstanceCentrifugalInfo(List<Rebar> rebars)
        {
            AssemblyInstance = AssemblyInstance.Create(Singleton.Instance.Document, rebars.Select(x => x.Id).ToList(), rebars.First().Category.Id);
            AssemblyInstance.LookupParameter("Workset").Set(rebars.First().WorksetId.IntegerValue);
        }
    }
}
