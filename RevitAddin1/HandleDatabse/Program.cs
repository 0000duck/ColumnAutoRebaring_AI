using HandleDatabse.Database.Dao;
using HandleDatabse.Database.Others;
using HandleDatabse.ExceptionCreation;
using HandleDatabse.ProjectData.Dao;
using HandleDatabse.ProjectData.EF;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandleDatabse
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            #region Setup Column AutoRebaring AI
            int idOrderL = StandardLengthOrderDao.InsertAndGetId(new List<int> { 7800, 5850, 3900, 2925 }, StandardLengthEnum.L);
            int idOrderL2 = StandardLengthOrderDao.InsertAndGetId(new List<int> { 11700, 5850 }, StandardLengthEnum.L2);
            int idOrderL3 = StandardLengthOrderDao.InsertAndGetId(new List<int> { 11700 }, StandardLengthEnum.L3);
            int idOrderI = StandardLengthOrderDao.InsertAndGetId(new List<int> { 3900, 2925, 2340, 1950 }, StandardLengthEnum.I);
            int idOrderI2 = StandardLengthOrderDao.InsertAndGetId(new List<int> { 5850, 3900, 2925 }, StandardLengthEnum.I2);
            int idFullStandLenOrder = FullStandardLengthOrderDao.InsertAndGetId(idOrderL, idOrderL2, idOrderL3, idOrderI, idOrderI2);
            int idRebarDes = RebarDesignDao.InsertAndGetId(new List<string> { "T16" }, new List<int> { 1 });
            int idEleDes = ElevationDesignDao.InsertAndGetId(new List<int> { 4970, 3300, 3300 }, null);
            int idBeamEleDes = BeamElevationDesignDao.InsertAndGetId(new List<int> { 500, 500, 500 });
            int idEleOff = ElevationOffsetDao.InsertAndGetId(200, 0, 0, 0);
            int idDevLen = DevelopmentLengthDao.InsertAndGetId(45, 0, 40);
            int idLenInfo = LengthInformationDao.InsertAndGetId(1900, 7800, 3900);
            int idStartOff = StartOffsetDao.InsertAndGetId(2900, 1500);
            int idDataCombine = DataCombineDao.InsertAndGetId(idRebarDes, idEleDes, idBeamEleDes, idEleOff, idDevLen, idLenInfo, idStartOff, idFullStandLenOrder);
            bool allowOverLevel = true;
            Singleton.Instance.DataCombine = DataCombineDao.GetDataCombine(idDataCombine);
            #endregion

            #region Calculate and Insert DataLearning
            try
            {
                DataLearningDao.GetId(idDataCombine, allowOverLevel);
            }
            catch (InvalidDataException)
            {
                LengthChosenDao.InsertLengthChosens();
                LengthInfoCollectionDao.GetLengthInfoCollection();
                DataLearningDao.Insert(Singleton.Instance.DataCombine.ID, Singleton.Instance.ResidualLengthInfoCollection, true);
                DataLearningDao.Insert(Singleton.Instance.DataCombine.ID, Singleton.Instance.Residual2LengthInfoCollection, false);
            }
            #endregion

            var obj = Singleton.Instance.LengthInfoCollections.Where(x => x.Residual == Singleton.Instance.LengthInfoCollections.Min(y => y.Residual));
            var obj1 = obj.OrderBy(x => x.Residual2).First();
            var obj2 = Singleton.Instance.LengthInfoCollections.OrderBy(x => x.Residual2).First();


            #region Get DataLearning
            Singleton.Instance.ChosenLengthInfoCollection = DataLearningDao.GetLengthInfoCollectionFromDataCombine(idDataCombine, allowOverLevel);
            #endregion

            Console.WriteLine("Done!");
            Console.ReadKey();
        }


    }
}
