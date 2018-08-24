using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_CAR
{
    public static class DesignInformationDao
    {
        public static List<DesignInformation> GetDesignInformations()
        {
            List<int> heights = Singleton.Instance.ModelData.InputElevationOrders;
            List<int> beamHeights = Singleton.Instance.ModelData.InputBeamElevationOrders;
            List<int> dias = Singleton.Instance.ModelData.InputRebarNames.Select(x => RebarDiameterDao.GetRebarDiameter(x)).ToList();
            List<int> diaLocs = Singleton.Instance.ModelData.InputRebarLocations;
            List<int> shortenOrders = Singleton.Instance.ModelData.InputShortenOrders;

            Offset botOff = new Offset { OffsetValue = Singleton.Instance.ModelData.InputElevationOffsets[0], OffsetRatio = Singleton.Instance.ModelData.InputElevationOffsets[1] };
            Offset topOff = new Offset { OffsetValue = Singleton.Instance.ModelData.InputElevationOffsets[2], OffsetRatio = Singleton.Instance.ModelData.InputElevationOffsets[3] };

            int sum = 0;
            int j = 0;

            List<DesignInformation> desInfos = new List<DesignInformation>();
            for (int i = 0; i < heights.Count; i++)
            {
                int botValue=sum+ GetOffsetFromHeight(heights[i], botOff);
                int topValue = sum + heights[i] - beamHeights[i] - GetOffsetFromHeight(heights[i], topOff);

                if (j < diaLocs.Count-1)
                {
                    while (diaLocs[j] < i + 1 && diaLocs[j+1] <= i+1) j++;
                }

                bool isShorten = false;
                if (j != 0)
                {
                    if (dias[j] > dias[j - 1] && diaLocs[j] == i+1) isShorten = true;
                }
                if (shortenOrders != null)
                {
                    if (shortenOrders.Contains(i + 1)) isShorten = true;
                }
                desInfos.Add(new DesignInformation()
                {
                    ID = i,
                    BottomTrue = sum,
                    Bottom = botValue,
                    Top = topValue,
                    Diameter = dias[j],
                    IsShorten = isShorten
                });

                sum += heights[i];
            }

            Singleton.Instance.ModelData.GeneralInfomation.TopLimit = sum;
            return desInfos;
        }
        public static int GetOffsetFromHeight(int height, Offset off)
        {
            return (off.OffsetRatio == 0) ? off.OffsetValue : Math.Max(off.OffsetValue, (int)(height / (double)off.OffsetRatio));
        }
    }
}
