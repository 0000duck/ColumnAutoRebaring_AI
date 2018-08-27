using Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Addin1Python
{
    public static class AssemblyInstanceInfoDao
    {
        public static AssemblyInstanceInfo GetAssemblyInstanceInfo(int indexX, int countX, RebarType rebarType)
        {
            var res = Singleton.Instance.AssemblyInstanceInfos.Where(x => x.IndexX == indexX && x.CountX == countX && x.RebarType == rebarType);
            if (res.Count() != 0) return res.First();
            AssemblyInstanceInfo assInsInfo = new AssemblyInstanceInfo() { IndexX = indexX, CountX = countX, RebarType = rebarType };
            Singleton.Instance.AssemblyInstanceInfos.Add(assInsInfo);
            return Singleton.Instance.AssemblyInstanceInfos.Last();
        }
        public static AssemblyInstanceInfo GetAssemblyInstanceInfo(RebarInfo rebarInfo)
        {
            return GetAssemblyInstanceInfo(rebarInfo.IndexX, rebarInfo.CountX, rebarInfo.RebarType);
        }
        public static void CreateAssemblyInstances()
        {
            foreach (RebarInfo rebarInfo in Singleton.Instance.CircleRebarInfos)
            {
                GetAssemblyInstanceInfo(rebarInfo).AddRebar(rebarInfo);
            }
        }
    }
}
