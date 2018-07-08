using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TestRevit1.EF;

namespace TestRevit1.Dao
{
    public static  class ElevationDao
    {
        public static void Insert(int length, ProjectDbContext db = null)
        {
            if (db== null) db = new ProjectDbContext();

            if (length < 0) return;
            try
            {
                GetId(length, db);
            }
            catch
            {
                var res = new Elevation()
                {
                    CreateDate = DateTime.Now,
                    Value = length
                };
                db.Elevations.Add(res);
                db.SaveChanges();
            }
        }
        public static int GetId(int length, ProjectDbContext db = null)
        {
            if (db == null) db = new ProjectDbContext();

            var obj = db.Elevations.Where(x => x.Value == length);
            if (obj.Count() == 0) throw new Exception();
            return obj.First().ID;
        }
        public static int GetElevation(int id, ProjectDbContext db = null)
        {
            if (db == null) db = new ProjectDbContext();

            var obj = db.Elevations.Where(x => x.ID == id);
            if (obj.Count() == 0) throw new Exception();

            return obj.First().Value;
        }
    }
}
