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
    public static class StandardLengthDao
    {
        public static void Insert(int length, ColumnStandardRebar_AI_DbContext db =null)
        {
            if (db == null) db = new ColumnStandardRebar_AI_DbContext();

            try
            {
                GetId(length, db);
            }
            catch
            {
                var res = new StandardLength()
                {
                    CreateDate = DateTime.Now,
                    IDLength = LengthDao.GetId(length, db)
                };
                db.StandardLengths.Add(res);
                db.SaveChanges();
            }
        }
        public static int GetId(int length, ColumnStandardRebar_AI_DbContext db = null)
        {
            if (db == null) db = new ColumnStandardRebar_AI_DbContext();

            if (length <= 0) throw new NegativeLengthException();
            int idLength = LengthDao.GetId(length, db);

            var obj = db.StandardLengths.Where(x => x.IDLength == idLength);
            if (obj.Count() == 0) throw new InvalidDataException();
            return obj.First().ID;
        }
        public static StandardLength GetStandardLength(int id, ColumnStandardRebar_AI_DbContext db = null)
        {
            if (db == null) db = new ColumnStandardRebar_AI_DbContext();

            var obj = db.StandardLengths.Where(x => x.ID == id);
            if (obj.Count() == 0) throw new InvalidIDException();

            return obj.First();
        }
        public static int GetLength(int id, ColumnStandardRebar_AI_DbContext db = null)
        {
            if (db == null) db = new ColumnStandardRebar_AI_DbContext();

            var stanLen = GetStandardLength(id, db);
            return LengthDao.GetLength(stanLen.IDLength);
        }
    }
}
