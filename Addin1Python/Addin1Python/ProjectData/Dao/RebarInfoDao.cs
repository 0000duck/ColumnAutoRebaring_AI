using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
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
            int countX = -1; RebarType rebarType = RebarType.Type1;
            foreach (RebarInfo rebarInfo in Singleton.Instance.CircleRebarInfos)
            {
                if (countX != rebarInfo.CountX || rebarType != rebarInfo.RebarType)
                {
                    Singleton.Instance.CircleRebarInfosList.Add(new List<RebarInfo> { rebarInfo });
                    countX = rebarInfo.CountX;
                    rebarType = rebarInfo.RebarType;
                }
                else
                {
                    Singleton.Instance.CircleRebarInfosList.Last().Add(rebarInfo);
                }
            }

            foreach (List<RebarInfo> rebarInfos in Singleton.Instance.CircleRebarInfosList)
            {
                int indexY = -1; double radius = -1;
                foreach (RebarInfo rebarInfo in rebarInfos)
                {
                    rebarInfo.CountY = rebarInfos.Count/rebarInfo.CountX;
                    if (rebarInfo.ArcInfo.CircleEquation.Radius != radius)
                    {
                        indexY++;
                        radius = rebarInfo.ArcInfo.CircleEquation.Radius;
                        
                    }
                    rebarInfo.IndexY = indexY;
                }
            }
        }
        public static void ShowValue()
        {
            foreach (RebarInfo rebarInfo in Singleton.Instance.CircleRebarInfos)
            {
                ArcInfo arcInfo = rebarInfo.ArcInfo;
                
                string value = $"R:({arcInfo.CircleEquation.Radius}_{arcInfo.StartAngle}_{arcInfo.EndAngle}_{arcInfo.RebarType})-X:{rebarInfo.IndexX}_{rebarInfo.CountX}-Y:{rebarInfo.IndexY}_{rebarInfo.CountY}_{Math.Ceiling(((double)rebarInfo.CountY)/2)-1}";
                    rebarInfo.Rebar.LookupParameter("Comments").Set(value);
            }
        }
        public static void RefreshRebars()
        {
            List<Rebar> deleteRebars = Singleton.Instance.CircleRebarInfos.Select(x => x.Rebar).ToList();
            List<Rebar> newRebars = ElementTransformUtils.CopyElements(Singleton.Instance.ActiveView, deleteRebars.Select(x => x.Id).ToList(),
                                        Singleton.Instance.ActiveView, Transform.Identity, new CopyPasteOptions()).Select(x=> Singleton.Instance.Document.GetElement(x)).Cast<Rebar>().ToList();
            newRebars.ForEach(x => x.LookupParameter("Workset").Set(Utility.GetWorkset().Id.IntegerValue));
            for (int i = 0; i < Singleton.Instance.CircleRebarInfos.Count; i++)
            {
                Singleton.Instance.CircleRebarInfos[i].Rebar = newRebars[i];
            }
            Singleton.Instance.Document.Delete(deleteRebars.Select(x => x.Id).ToList());
        }
    }
}
