using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Addin1Python
{
    public class GroupInfo
    {
        public int IndexX { get; set; } = -1;
        public int CountX { get; set; } = -1;
        public RebarType RebarType { get; set; }
        public Group Group { get; set; }
        public GroupInfo(List<RebarInfo> rebarInfos)
        {
            IndexX = rebarInfos.First().IndexX;
            CountX = rebarInfos.First().CountX;
            RebarType = rebarInfos.First().RebarType;

            Group = Singleton.Instance.Document.Create.NewGroup(rebarInfos.Select(x => x.Rebar.Id).ToList());
            Group.Name = Guid.NewGuid().ToString();
            Group.LookupParameter("Workset").Set(Utility.GetWorkset().Id.IntegerValue);
        }
    }
}
