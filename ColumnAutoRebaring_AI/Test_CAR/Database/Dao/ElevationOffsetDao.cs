using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_CAR
{
    public static class ElevationOffsetDao
    {
        public static void Insert(int botOffValue, int botOffRatio, int topOffValue, int topOffRatio, ColumnStandardRebar_AI_DbContext db = null)
        {
            if (db == null) db = new ColumnStandardRebar_AI_DbContext();

            try
            {
                GetId(botOffValue, botOffRatio, topOffValue, topOffRatio, db);
            }
            catch
            {
                int idBotOff = OffsetDao.GetId(botOffValue, botOffRatio, db);
                int idTopOff = OffsetDao.GetId(topOffValue, topOffRatio, db);

                var res = new ElevationOffset()
                {
                    CreateDate = DateTime.Now,
                    IDBottomOffset = idBotOff,
                    IDTopOffset = idTopOff
                };
                db.ElevationOffsets.Add(res);
                db.SaveChanges();
            }
        }
        public static int GetId(int botOffValue, int botOffRatio, int topOffValue, int topOffRatio, ColumnStandardRebar_AI_DbContext db = null)
        {
            if (db == null) db = new ColumnStandardRebar_AI_DbContext();

            int idBotOff = OffsetDao.GetId(botOffValue, botOffRatio, db);
            int idTopOff = OffsetDao.GetId(topOffValue, topOffRatio, db);
            var obj = db.ElevationOffsets.Where(x => x.IDBottomOffset == idBotOff && x.IDTopOffset == idTopOff);

            if (obj.Count() == 0) throw new InvalidDataException();
            return obj.First().ID;
        }
        public static int InsertAndGetId(int botOffValue, int botOffRatio, int topOffValue, int topOffRatio, ColumnStandardRebar_AI_DbContext db = null)
        {
            if (db == null) db = new ColumnStandardRebar_AI_DbContext();

            Insert(botOffValue, botOffRatio, topOffValue, topOffRatio, db);
            return GetId(botOffValue, botOffRatio, topOffValue, topOffRatio, db);
        }
        public static int InsertAndGetId(List<int> values)
        {
            return InsertAndGetId(values[0], values[1], values[2], values[3]);
        }
        public static ElevationOffset GetElevationOffset(int id, ColumnStandardRebar_AI_DbContext db = null)
        {
            if (db == null) db = new ColumnStandardRebar_AI_DbContext();

            var obj = db.ElevationOffsets.Where(x => x.ID == id);
            if (obj.Count() == 0) throw new InvalidIDException();
            return obj.First();
        }
    }
}
