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
    public static class RebarDiameterDao
    {
        public static void Insert(string name, int dia, ColumnStandardRebar_AI_DbContext db = null)
        {
            if (db == null) db = new ColumnStandardRebar_AI_DbContext();

            try
            {
                GetId(name);
            }
            catch
            {
                var res = new RebarDiameter()
                {
                    CreateDate = DateTime.Now,
                    Name = name,
                    Diameter = dia
                };
                db.RebarDiameters.Add(res);
                db.SaveChanges();
            }
        }
        public static int GetId(string name, ColumnStandardRebar_AI_DbContext db = null)
        {
            if (db == null) db = new ColumnStandardRebar_AI_DbContext();

            var obj = db.RebarDiameters.Where(x => x.Name == name);
            if (obj.Count() == 0) throw new InvalidDataException();

            return obj.First().ID;
        }
        public static int GetRebarDiameter(int id, ColumnStandardRebar_AI_DbContext db = null)
        {
            if (db == null) db = new ColumnStandardRebar_AI_DbContext();

            var obj = db.RebarDiameters.Where(x => x.ID == id);
            if (obj.Count() == 0) throw new InvalidIDException();

            return obj.First().Diameter;
        }
    }
}
