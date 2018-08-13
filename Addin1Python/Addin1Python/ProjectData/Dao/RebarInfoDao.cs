using Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Addin1Python
{
    public static class RebarInfoDao
    {
        public static void Categozie()
        {
            List<List<RebarInfo>> rebarInfosList = new List<List<RebarInfo>>();
            int countX = -1; RebarType rebarType = RebarType.Type1;
            foreach (RebarInfo rebarInfo in Singleton.Instance.CircleRebarInfos)
            {
                if (countX != rebarInfo.CountX || rebarType != rebarInfo.RebarType)
                {
                    rebarInfosList.Add(new List<RebarInfo> { rebarInfo });
                    countX = rebarInfo.CountX;
                    rebarType = rebarInfo.RebarType;
                }
                else
                {
                    rebarInfosList.Last().Add(rebarInfo);
                }
            }

            foreach (List<RebarInfo> rebarInfos in rebarInfosList)
            {
                int indexY = -1; double radius = -1;
                foreach (RebarInfo rebarInfo in rebarInfos)
                {
                    rebarInfo.CountY = rebarInfos.Count/rebarInfo.CountX;
                    if (rebarInfo.Radius != radius)
                    {
                        indexY++;
                        radius = rebarInfo.Radius;
                        
                    }
                    rebarInfo.IndexY = indexY;
                }
            }
        }
        public static void ShowValue()
        {
            foreach (RebarInfo rebarInfo in Singleton.Instance.CircleRebarInfos)
            {
                string value = $"R:{GeomUtil.feet2Milimeter(rebarInfo.Radius):0}-X:{rebarInfo.IndexX}_{rebarInfo.CountX}-Y:{rebarInfo.IndexY}_{rebarInfo.CountY}";
                    rebarInfo.Rebar.LookupParameter("Comments").Set(value);
            }
        }
    }
}
