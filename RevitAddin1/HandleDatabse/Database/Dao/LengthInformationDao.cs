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
    public static  class LengthInformationDao
    {
        public static void Insert(int min, int max, int implantMax, ColumnStandardRebar_AI_DbContext db = null)
        {
            if (db== null) db = new ColumnStandardRebar_AI_DbContext();

            try
            {
                GetId(min, max, implantMax, db);
            }
            catch
            {
                var res = new LengthInformation()
                {
                    CreateDate = DateTime.Now,
                    Min = min,
                    Max = max,
                    ImplantMax = implantMax
                };
                db.LengthInformations.Add(res);
                db.SaveChanges();
            }
        }
        public static int GetId(int min, int max, int implantMax, ColumnStandardRebar_AI_DbContext db = null)
        {
            if (db == null) db = new ColumnStandardRebar_AI_DbContext();

            var obj = db.LengthInformations.Where(x => x.Min == min && x.Max == max && x.ImplantMax == implantMax);
            if (obj.Count() == 0) throw new InvalidDataException();
            return obj.First().ID;
        }
        public static int InsertAndGetId(int min, int max, int implantMax, ColumnStandardRebar_AI_DbContext db = null)
        {
            if (db == null) db = new ColumnStandardRebar_AI_DbContext();

            Insert(min, max, implantMax, db);
            return GetId(min, max, implantMax, db);
        }
        public static LengthInformation GetLengthInformation(int id, ColumnStandardRebar_AI_DbContext db = null)
        {
            if (db == null) db = new ColumnStandardRebar_AI_DbContext();

            var obj = db.LengthInformations.Where(x => x.ID == id);
            if (obj.Count() == 0) throw new InvalidIDException();
            return obj.First();
        }
    }
}
