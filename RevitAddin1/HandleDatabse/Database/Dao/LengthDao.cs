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
    public static class LengthDao
    {
        public static List<int> GetLengths(ColumnStandardRebar_AI_DbContext db = null)
        {
            if (db == null) db = new ColumnStandardRebar_AI_DbContext();
            return db.Lengths.Select(x => x.Value).ToList();
        }
        public static List<int> GetLengths(int min, int max, ColumnStandardRebar_AI_DbContext db = null)
        {
            if (db == null) db = new ColumnStandardRebar_AI_DbContext();
            return db.Lengths.Select(x => x.Value).Where(x=> x>= min && x<= max).ToList();
        }
        public static void Insert(int length, ColumnStandardRebar_AI_DbContext db = null)
        {
            if (db== null) db = new ColumnStandardRebar_AI_DbContext();

            if (length < 0) return;
            try
            {
                GetId(length, db);
            }
            catch
            {
                var res = new Length()
                {
                    CreateDate = DateTime.Now,
                    Value = length
                };
                db.Lengths.Add(res);
                db.SaveChanges();
            }
        }
        public static int GetId(int length, ColumnStandardRebar_AI_DbContext db = null)
        {
            if (db == null) db = new ColumnStandardRebar_AI_DbContext();

            var obj = db.Lengths.Where(x => x.Value == length);
            if (obj.Count() == 0) throw new InvalidDataException();
            return obj.First().ID;
        }
        public static int GetLength(int id, ColumnStandardRebar_AI_DbContext db = null)
        {
            if (db == null) db = new ColumnStandardRebar_AI_DbContext();

            var obj = db.Lengths.Where(x => x.ID == id);
            if (obj.Count() == 0) throw new InvalidIDException();
            return obj.First().Value;
        }
    }
}
