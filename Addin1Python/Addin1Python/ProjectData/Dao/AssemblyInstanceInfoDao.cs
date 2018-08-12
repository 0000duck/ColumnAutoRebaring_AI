<<<<<<< HEAD
﻿using System;
=======
﻿using Geometry;
using System;
>>>>>>> 38c4cfde3c38e33ff6981985bf2686d613a92af5
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Addin1Python
{
    public static class AssemblyInstanceInfoDao
    {
<<<<<<< HEAD
        public static AssemblyInstanceInfo GetAssemblyInstanceInfo(RebarType rebarType, int count, int index)
        {
            var res = Singleton.Instance.AssemblyInstanceInfos.Where(x => x.RebarType == rebarType && x.Count == count && x.Index == index);
            if (res.Count() != 0) return res.First();
            var obj = new AssemblyInstanceInfo { RebarType = rebarType, Count = count, Index = index };
            Singleton.Instance.AssemblyInstanceInfos.Add(obj);
=======
        public static AssemblyInstanceInfo GetAssemblyInstanceInfo(int indexX, int countX, RebarType rebarType)
        {
            var res = Singleton.Instance.AssemblyInstanceInfos.Where(x => x.IndexX == indexX && x.CountX == countX && x.RebarType == rebarType);
            if (res.Count() != 0) return res.First();
            AssemblyInstanceInfo assInsInfo = new AssemblyInstanceInfo() { IndexX = indexX, CountX = countX, RebarType = rebarType };
            Singleton.Instance.AssemblyInstanceInfos.Add(assInsInfo);
>>>>>>> 38c4cfde3c38e33ff6981985bf2686d613a92af5
            return Singleton.Instance.AssemblyInstanceInfos.Last();
        }
        public static AssemblyInstanceInfo GetAssemblyInstanceInfo(RebarInfo rebarInfo)
        {
<<<<<<< HEAD
            return GetAssemblyInstanceInfo(rebarInfo.RebarType, rebarInfo.Count, rebarInfo.Index);
=======
            return GetAssemblyInstanceInfo(rebarInfo.IndexX, rebarInfo.CountX, rebarInfo.RebarType);
        }
        public static void CreateAssemblyInstances()
        {
            foreach (RebarInfo rebarInfo in Singleton.Instance.RebarInfos)
            {
                GetAssemblyInstanceInfo(rebarInfo).AddRebar(rebarInfo);
            }
>>>>>>> 38c4cfde3c38e33ff6981985bf2686d613a92af5
        }
    }
}
