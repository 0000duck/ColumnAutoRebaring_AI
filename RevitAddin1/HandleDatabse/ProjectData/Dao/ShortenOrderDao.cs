using HandleDatabse.Database.Dao;
using HandleDatabse.ProjectData.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandleDatabse.ProjectData.Dao
{
    public class ShortenOrderDao
    {
        public static List<int> GetShortenOrder()
        {
            List<int> shortenOrder = ElevationDesignDao.GetShortenOrder(Singleton.Instance.DataCombine.IDElevationDesign);
            Tuple<List<int>, List<int>> diasAndLocs = RebarDesignDao.GetDiametersAndLocations(Singleton.Instance.DataCombine.IDRebarDesign);
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
