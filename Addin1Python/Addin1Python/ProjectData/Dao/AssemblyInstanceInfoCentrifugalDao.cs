using Autodesk.Revit.DB.Structure;
using Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Addin1Python
{
    public static class AssemblyInstanceInfoCentrifugalDao
    {
        public static void CreateAssemblyInstanceInfoCentrifugals()
        {
            foreach (List<Rebar> rebars in Singleton.Instance.CentrifugalRebarsList)
            {
                Singleton.Instance.AssemblyInstanceCentrifugalInfos.Add(new AssemblyInstanceCentrifugalInfo(rebars));
            }
        }
    }
}
