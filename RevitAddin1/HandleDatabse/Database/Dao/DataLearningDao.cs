using HandleDatabse.Constant;
using HandleDatabse.Database.EF;
using HandleDatabse.ExceptionCreation;
using HandleDatabse.ProjectData.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandleDatabse.Database.Dao
{
    public static class DataLearningDao
    {
        public static void Insert(int idDataCombine, LengthInfoCollection AOLlenInfoColl, LengthInfoCollection NAOLlenInfoColl, ColumnStandardRebar_AI_DbContext db = null)
        {
            if (db == null) db = new ColumnStandardRebar_AI_DbContext();

            try
            {
                GetId(idDataCombine, db);
            }
            catch
            {
                var res = new DataLearning()
                {
                    IDDataCombine = idDataCombine,
                    CreateDate = DateTime.Now,
                    AOLLengthOrder = ConvertLengthOrder2String(AOLlenInfoColl),
                    NAOLLengthOrder = ConvertLengthOrder2String(NAOLlenInfoColl),
                    AOLResidual = AOLlenInfoColl.Residual,
                    NAOLResidual = NAOLlenInfoColl.Residual2
                };
                db.DataLearnings.Add(res);
                db.SaveChanges();
            }
        }
        public static int GetId(int idDataCombine, ColumnStandardRebar_AI_DbContext db = null)
        {
            if (db == null) db = new ColumnStandardRebar_AI_DbContext();

            var obj = db.DataLearnings.Where(x => x.IDDataCombine == idDataCombine);
            if (obj.Count() == 0) throw new InvalidDataException();
            return obj.First().ID;
        }
        public static DataLearning GetDataLearning(int id, ColumnStandardRebar_AI_DbContext db = null)
        {
            if (db == null) db = new ColumnStandardRebar_AI_DbContext();

            var obj = db.DataLearnings.Where(x => x.ID == id);
            if (obj.Count() == 0) throw new InvalidIDException();

            return obj.First();
        }
        public static LengthInfoCollection GetLengthInfoCollection(int id,bool allowOverLevel, ColumnStandardRebar_AI_DbContext db = null)
        {
            if (db == null) db = new ColumnStandardRebar_AI_DbContext();

            var dataLearning = GetDataLearning(id, db);
            return new LengthInfoCollection(allowOverLevel ? dataLearning.AOLLengthOrder : dataLearning.NAOLLengthOrder, true);
        }
        public static LengthInfoCollection GetLengthInfoCollectionFromDataCombine(int idDataCombine, bool allowOverLevel, ColumnStandardRebar_AI_DbContext db = null)
        {
            if (db == null) db = new ColumnStandardRebar_AI_DbContext();

            Singleton.Instance.ChosenAllowOverLevel = allowOverLevel;
            return GetLengthInfoCollection(GetId(idDataCombine, db), allowOverLevel, db);
        }
        public static string ConvertLengthOrder2String(LengthInfoCollection lenInfoColl)
        {
            string lenOrder = "";
            for (int i = 0; i < Singleton.Instance.LoopCount-1; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    LengthInfo lenInfo = lenInfoColl.GetIndex(i, j);
                    lenOrder += LengthDao.GetId(lenInfo.Value);
                    if (j < 1)
                        lenOrder += ",";
                }
                if (i < Singleton.Instance.LoopCount - 2)
                    lenOrder += "-";
            }
            return lenOrder;
        }
    }
}
