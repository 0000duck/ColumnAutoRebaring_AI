using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Addin1Python
{
    public static class AssemblyInstanceInfoDao
    {
        public static AssemblyInstanceInfo GetAssemblyInstanceInfo(RebarType rebarType, int count, int index)
        {
            var res = Singleton.Instance.AssemblyInstanceInfos.Where(x => x.RebarType == rebarType && x.Count == count && x.Index == index);
            if (res.Count() != 0) return res.First();
            var obj = new AssemblyInstanceInfo { RebarType = rebarType, Count = count, Index = index };
            Singleton.Instance.AssemblyInstanceInfos.Add(obj);
            return Singleton.Instance.AssemblyInstanceInfos.Last();
        }
        public static AssemblyInstanceInfo GetAssemblyInstanceInfo(RebarInfo rebarInfo)
        {
            return GetAssemblyInstanceInfo(rebarInfo.RebarType, rebarInfo.Count, rebarInfo.Index);
        }
    }
}
