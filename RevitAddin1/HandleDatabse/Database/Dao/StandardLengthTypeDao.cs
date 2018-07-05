using HandleDatabse.Constant;
using HandleDatabse.Database.EF;
using HandleDatabse.Database.Others;
using HandleDatabse.ExceptionCreation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandleDatabse.Database.Dao
{
    public static  class StandardLengthTypeDao
    {
        public static void Insert(StandardLengthEnum standLenType, ColumnStandardRebar_AI_DbContext db = null)
        {
            if (db== null) db = new ColumnStandardRebar_AI_DbContext();

            try
            {
                GetId(standLenType, db);
            }
            catch
            {
                var res = new StandardLengthType()
                {
                    CreateDate = DateTime.Now,
                    Value = standLenType.ToString()
                };
                db.StandardLengthTypes.Add(res);
                db.SaveChanges();
            }
        }
        public static int GetId(StandardLengthEnum standLenType, ColumnStandardRebar_AI_DbContext db = null)
        {
            if (db == null) db = new ColumnStandardRebar_AI_DbContext();

            string type = standLenType.ToString();
            var obj = db.StandardLengthTypes.Where(x => x.Value == type);
            if (obj.Count() == 0) throw new InvalidDataException();
            return obj.First().ID;
        }
        public static StandardLengthType GetStandardLengthType(int id, ColumnStandardRebar_AI_DbContext db = null)
        {
            if (db == null) db = new ColumnStandardRebar_AI_DbContext();

            var obj = db.StandardLengthTypes.Where(x => x.ID == id);
            if (obj.Count() == 0) throw new InvalidIDException();
            return obj.First();
        }
    }
}
