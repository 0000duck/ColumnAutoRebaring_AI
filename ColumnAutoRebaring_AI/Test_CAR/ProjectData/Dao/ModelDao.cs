using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_CAR
{
    public static class ModelDao
    {
        public static void Categorize()
        {
            if (Singleton.Instance.ModelData.GeneralModel.IsHaveDataLearning()) return;

            int count = Singleton.Instance.ModelData.InputElevationOrders.Count;
            var desInfos = Singleton.Instance.ModelData.DesignInformations;

            for (int i = 0; i < Math.Ceiling((double)count / 2); i++)
            {
                List<int> eleOrders = new List<int>();
                List<int> beamEleOrders = new List<int>();
                for (int j = 0; j < 3; j++)
                {
                    int k = i * 2 + j;
                    if (k == count) break;
                    eleOrders.Add(Singleton.Instance.ModelData.InputElevationOrders[k]);
                    beamEleOrders.Add(Singleton.Instance.ModelData.InputBeamElevationOrders[k]);


                }
                var model = new Model()
                {
                    RebarNames = InputRebarNames,
                    RebarOrders = InputRebarOrders,
                    ElevationOrders = Singleton.Instance.ModelData.InputElevationOrders.,
                    ShortenOrders = InputShortenOrders,
                    BeamElevationOrders = InputBeamElevationOrders,
                    StartOffsets = InputStartOffsets
                };
            }
        }
    }
}
