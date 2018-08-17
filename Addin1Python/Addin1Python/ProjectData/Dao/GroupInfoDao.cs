using Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Addin1Python
{
    public static class GroupInfoDao
    {
        public static void CreateGroupInfos()
        {
            foreach (List<RebarInfo> rebarInfos in Singleton.Instance.CircleRebarInfosList)
            {
                Singleton.Instance.GroupInfos.Add(new GroupInfo(rebarInfos));
            }
        }
    }
}
