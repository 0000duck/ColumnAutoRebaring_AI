using HandleDatabse.Database.Dao;
using HandleDatabse.Database.EF;
using HandleDatabse.ProjectData.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandleDatabse.ProjectData.Dao
{
    public static class DesignInformationDao
    {
        public static List<DesignInformation> GetDesignInformations()
        {
            List<int> heights = ElevationDesignDao.GetHeights(Singleton.Instance.DataCombine.IDElevationDesign);
            List<int> beamHeights = BeamElevationDesignDao.GetHeights(Singleton.Instance.DataCombine.IDBeamElevationDesign);
            Tuple<List<int>, List<int>> diasAndLocs = RebarDesignDao.GetDiametersAndLocations(Singleton.Instance.DataCombine.IDRebarDesign);
            ElevationOffset eleOff = ElevationOffsetDao.GetElevationOffset(Singleton.Instance.DataCombine.IDElevationOffset);
            Offset botOff = OffsetDao.GetOffset(eleOff.IDBottomOffset);
            Offset topOff = OffsetDao.GetOffset(eleOff.IDTopOffset);

            List<DesignInformation> desInfos = new List<DesignInformation>();
            int sum = 0;
            int j = 0;

            for (int i = 0; i < heights.Count; i++)
            {
                int botValue=sum+ GetOffsetFromHeight(heights[i], botOff);
                int topValue = sum + heights[i] - beamHeights[i] - GetOffsetFromHeight(heights[i], topOff);

                if (j < diasAndLocs.Item2.Count-1)
                {
                    while (diasAndLocs.Item2[j] < i + 1) j++;
                }

                desInfos.Add(new DesignInformation()
                {
                    ID = i,
                    Bottom = botValue,
                    Top = topValue,
                    Diameter = diasAndLocs.Item1[j]
                });

                sum += heights[i];
            }

            Singleton.Instance.GeneralInfomation.TopLimit = sum;

            return desInfos;
        }
        public static int GetOffsetFromHeight(int height, Offset off)
        {
            return (off.OffsetRatio == 0) ? off.OffsetValue : Math.Max(off.OffsetValue, (int)(height / (double)off.OffsetRatio));
        }
    }
}
