using HandleDatabse.Database.Dao;
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
            int idOrderL = StandardLengthOrderDao.GetId(new List<int> { 7800, 5850, 3900, 2925 }, Database.Others.StandardLengthEnum.L);
            int idOrderL2 = StandardLengthOrderDao.GetId(new List<int> { 11700, 5850 }, Database.Others.StandardLengthEnum.L2);
            int idOrderL3 = StandardLengthOrderDao.GetId(new List<int> { 11700 }, Database.Others.StandardLengthEnum.L3);
            int idOrderI = StandardLengthOrderDao.GetId(new List<int> { 3900, 2925, 2340, 1950 }, Database.Others.StandardLengthEnum.I);
            int idOrderI2 = StandardLengthOrderDao.GetId(new List<int> { 5850, 3900, 2925 }, Database.Others.StandardLengthEnum.I2);
            int idFullStandLenOrder = FullStandardLengthOrderDao.GetId(idOrderL, idOrderL2, idOrderL3, idOrderI, idOrderI2);
            int idRebarDes = RebarDesignDao.GetId(new List<string> { "T16", "T20" }, new List<int> { 1, 3 });
            int idEleDes = ElevationDesignDao.GetId(new List<int> { 4970, 3300, 3300 }, null);
            int idBeamEleDes = BeamElevationDesignDao.GetId(new List<int> { 500, 500, 500 });
            int idEleOff = ElevationOffsetDao.GetId(200, 0, 0, 0);
            int idDevLen = DevelopmentLengthDao.GetId(45, 0, 40);
            int idLenInfo = LengthInformationDao.GetId(1900, 7800, 3900);
            int idStartOff = StartOffsetDao.GetId(2900, 1500);

            Singleton.Instance.DataCombine = DataCombineDao.GetDataCombine(idRebarDes, idEleDes, idBeamEleDes, idEleOff, idDevLen, idLenInfo, idStartOff, idFullStandLenOrder);
            LengthChosenDao.InsertLengthChosens(0);
            Singleton.Instance.LengthInfoCollections = Singleton.Instance.LengthChosens.Select(x => new LengthInfoCollection(x.ID)).ToList();

            //DataLearningDao.Insert(Singleton.Instance.DataCombine.ID, Singleton.Instance.OptimizeLengthInfoCollection);
            

            //int idDataCombine = DataCombineDao.GetId(idRebarDes, idEleDes, idBeamEleDes, idEleOff, idDevLen, idLenInfo, idStartOff, idFullStandLenOrder);
            //LengthInfoCollection lenInfoColl = DataLearningDao.GetLengthInfoCollectionFromDataCombine(idDataCombine);

            Console.WriteLine("Done!");
            Console.ReadKey();
        }


    }
}
