using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_CAR
{
    public class ShortenOrderDao
    {
        public static List<int> GetShortenOrder()
        {
            List<int> shortenOrder = ElevationDesignDao.GetShortenOrder(Singleton.Instance.OtherData.DataCombine.IDElevationDesign);
            Tuple<List<int>, List<int>> diasAndLocs = RebarDesignDao.GetDiametersAndLocations(Singleton.Instance.OtherData.DataCombine.IDRebarDesign);
            for (int i = 1; i < diasAndLocs.Item1.Count; i++)
            {
                if (diasAndLocs.Item1[i] > diasAndLocs.Item1[i - 1] && !shortenOrder.Contains(diasAndLocs.Item2[i]))
                {
                    shortenOrder.Add(diasAndLocs.Item2[i]);
                }
            }
            return shortenOrder;
        }
    }
}
