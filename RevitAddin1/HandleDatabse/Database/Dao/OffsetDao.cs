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
    public static  class OffsetDao
    {
        public static void Insert(int offValue, int offRatio, ColumnStandardRebar_AI_DbContext db = null)
        {
            if (db== null) db = new ColumnStandardRebar_AI_DbContext();

            try
            {
                GetId(offValue, offRatio, db);
            }
            catch
            {
                var res = new Offset()
                {
                    CreateDate = DateTime.Now,
                    OffsetValue = offValue,
                    OffsetRatio = offRatio
                };
                db.Offsets.Add(res);
                db.SaveChanges();
            }
        }
        public static int GetId(int offValue, int offRatio, ColumnStandardRebar_AI_DbContext db = null)
        {
            if (db == null) db = new ColumnStandardRebar_AI_DbContext();

            var obj = db.Offsets.Where(x => x.OffsetValue == offValue && x.OffsetRatio == offRatio);
            if (obj.Count() == 0) throw new InvalidDataException();
            return obj.First().ID;
        }
        public static Offset GetOffset(int id, ColumnStandardRebar_AI_DbContext db = null)
        {
            if (db == null) db = new ColumnStandardRebar_AI_DbContext();

            var obj = db.Offsets.Where(x => x.ID == id);
            if (obj.Count() == 0) throw new InvalidIDException();
            return obj.First();
        }
    }
}
