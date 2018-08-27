using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_CAR
{
    public class Model
    {
        #region Variables
        
        #endregion

        #region Properties
        public List<string> RebarNames { get; set; }
        public List<int> RebarOrders { get; set; }
        public List<int> ElevationOrders { get; set; }
        public List<int> ShortenOrders { get; set; }
        public List<int> BeamElevationOrders { get; set; }
        public List<int> StartOffsets { get; set; }
        public int IdDataLearning { get; set; } = -1;
        #endregion

        #region Methods
        public bool IsHaveDataLearning()
        {
            int idRebarDes = RebarDesignDao.InsertAndGetId(RebarNames, RebarOrders);
            int idEleDes = ElevationDesignDao.InsertAndGetId(ElevationOrders, ShortenOrders);
            int idBeamEleDes = BeamElevationDesignDao.InsertAndGetId(BeamElevationOrders);
            int idStartOff = StartOffsetDao.InsertAndGetId(StartOffsets);

            int idDataCombine = DataCombineDao.InsertAndGetId(idRebarDes, idEleDes, idBeamEleDes,
                Singleton.Instance.ModelData.IdElevationOffset, Singleton.Instance.ModelData.IdDevelopLength,
                Singleton.Instance.ModelData.IdLengthInfo,
                idStartOff, Singleton.Instance.ModelData.IdFullStandardLengthOrder);
            try
            {
                IdDataLearning = DataLearningDao.GetId(idDataCombine);
            }
            catch (InvalidDataException)
            {
                return false;
            }
            return true;
        }
        #endregion
    }
}
