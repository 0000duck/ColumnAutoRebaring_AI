using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_CAR
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Singleton.Instance = new Singleton();
            Singleton.Instance.RevitData.UIApplication = commandData.Application;

            Singleton.Instance.OtherData.InputForm.ShowDialog();
            if (!Singleton.Instance.WPFData.IsClosedFormOK)
                return Result.Succeeded;

            Transaction tx = new Transaction(Singleton.Instance.RevitData.Document, "AutoRebaring_AI");
            tx.Start();

            #region Setup Column AutoRebaring AI
            List<string> rebarNames = new List<string> { Singleton.Instance.WPFData.SelectedRebarType.Name };
            List<int> rebarOrders = new List<int> { 1 };
            List<int> eleOrders = new List<int> { 4900, 3300, 3300, 3300 };
            List<int> shortenOrders = null;
            List<int> beamEleOrders = new List<int> { 500, 500, 500, 500 };
            int count = eleOrders.Count;
            int divCount = (int)Math.Ceiling(((double)eleOrders.Count) / 2);

            int idRebarDes = RebarDesignDao.InsertAndGetId(rebarNames, rebarOrders);
            int idEleDes = ElevationDesignDao.InsertAndGetId(eleOrders, shortenOrders);
            int idBeamEleDes = BeamElevationDesignDao.InsertAndGetId(beamEleOrders);
            int idDataCombine = DataCombineDao.InsertAndGetId(idRebarDes, idEleDes, idBeamEleDes, 
                Singleton.Instance.OtherData.IdElementOffset, Singleton.Instance.OtherData.IdDevelopLength, Singleton.Instance.OtherData.IdLengthInfo, 
                Singleton.Instance.OtherData.IdStartOffset, Singleton.Instance.OtherData.IdFullStandardLengthOrder);
            bool allowOverLevel = true;
            Singleton.Instance.OtherData.DataCombine = DataCombineDao.GetDataCombine(idDataCombine);
            #endregion

            #region Calculate and Insert DataLearning
            try
            {
                DataLearningDao.GetId(idDataCombine);
            }
            catch (InvalidDataException)
            {
                //int sIdStartOff = i == 0 ? Singleton.Instance.OtherData.IdStartOffset : -1;
                for (int i = 0; i < count; i++)
                {
                    List<int> sEleOrders = new List<int>();
                    List<int> sShortenOrders = new List<int>();
                    List<int> sBeamEleOrders = new List<int>();
                    for (int j = 0; j < 3; j++)
                    {
                        int k = 2 * i + j;
                        if (k == count) break;
                        sEleOrders.Add(eleOrders[k]);
                        sBeamEleOrders.Add(beamEleOrders[k]);
                    }
                }


                LengthChosenDao.InsertLengthChosens();
                LengthInfoCollectionDao.GetLengthInfoCollection();
                DataLearningDao.Insert(Singleton.Instance.OtherData.DataCombine.ID, Singleton.Instance.OtherData.AOLLengthInfoCollection, Singleton.Instance.OtherData.NAOLLengthInfoCollection);
            }
            #endregion

            #region Get DataLearning
            Singleton.Instance.OtherData.ChosenLengthInfoCollection = DataLearningDao.GetLengthInfoCollectionFromDataCombine(idDataCombine, allowOverLevel);
            #endregion

            #region Create Rebar from Length Info Collection
            LengthInfoCollectionDao.CreateRebar();
            #endregion

            tx.Commit();
            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    public class Command1 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Singleton.Instance = new Singleton();

            Singleton.Instance.RevitData.UIApplication = commandData.Application;
            Singleton.Instance.RevitData.Transaction.Start();

            #region InputData
            Singleton.Instance.WPFData.AllowOverLevel = true;

            Singleton.Instance.ModelData.InputRebarNames = new List<string> { "T16", "T20" };
            Singleton.Instance.ModelData.InputRebarLocations = new List<int> { 1, 3 };
            Singleton.Instance.ModelData.InputElevationOrders = new List<int> { 4970, 3300, 3300 };
            Singleton.Instance.ModelData.InputShortenOrders = new List<int> { 2 };
            Singleton.Instance.ModelData.InputBeamElevationOrders = new List<int> { 500, 500, 500 };
            Singleton.Instance.ModelData.InputOrderLs = new List<int> {7800,5850,3900,2925 };
            Singleton.Instance.ModelData.InputOrderL2s = new List<int> {11700,5850 };
            Singleton.Instance.ModelData.InputOrderL3s = new List<int> { 11700};
            Singleton.Instance.ModelData.InputOrderIs = new List<int> {3900,2925,2340,1950 };
            Singleton.Instance.ModelData.InputOrderI2s = new List<int> { 5850,3900,2925};
            Singleton.Instance.ModelData.InputLengthInfo = new List<int> { 1900,7800,3900};
            Singleton.Instance.ModelData.InputElevationOffsets = new List<int> { 200, 0, 0, 0 };
            Singleton.Instance.ModelData.InputDevelopLength = new List<int> { 40, 0, 40 };
            Singleton.Instance.ModelData.InputStartOffsets = new List<int> { 2900,1500};
            #endregion

            ModelDao.Categorize();

            Singleton.Instance.RevitData.Transaction.Commit();
            return Result.Succeeded;
        }
    }
}
