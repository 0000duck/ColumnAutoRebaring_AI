using HandleDatabse.Constant;
using HandleDatabse.Database.EF;
using HandleDatabse.ExceptionCreation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandleDatabse.Database.Dao
{
    public static class StartOffsetDao
    {
        public static void Insert(int offset1, int offset2, ColumnStandardRebar_AI_DbContext db = null)
        {
            if (db == null) db = new ColumnStandardRebar_AI_DbContext();

            try
            {
                GetId(offset1, offset2, db);
            }
            catch
            {
                var res = new StartOffset()
                {
                    CreateDate = DateTime.Now,
                    Offset1 = Math.Max(offset1, offset2),
                    Offset2 = Math.Min(offset1, offset2)
                };
                db.StartOffsets.Add(res);
                db.SaveChanges();
            }
        }
        public static int GetId(int offset1, int offset2, ColumnStandardRebar_AI_DbContext db = null)
        {
            if (db == null) db = new ColumnStandardRebar_AI_DbContext();

            int max = Math.Max(offset1, offset2);
            int min = Math.Min(offset1, offset2);

            var obj = db.StartOffsets.Where(x => x.Offset1 == max && x.Offset2 == min);
            if (obj.Count() == 0) throw new InvalidDataException();
            return obj.First().ID;
        }
        public static int InsertAndGetId(int offset1, int offset2, ColumnStandardRebar_AI_DbContext db = null)
        {
            if (db == null) db = new ColumnStandardRebar_AI_DbContext();

            Insert(offset1, offset2, db);
            return GetId(offset1, offset2, db);
        }

        public static StartOffset GetStartOffset(int id, ColumnStandardRebar_AI_DbContext db = null)
        {
            if (db == null) db = new ColumnStandardRebar_AI_DbContext();

            var obj = db.StartOffsets.Where(x => x.ID == id);
            if (obj.Count() == 0) throw new InvalidIDException();
            return obj.First();
        }
    }
}
