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
    public static class BeamElevationDesignDao
    {
        public static void Insert(List<int> eleOrder, ColumnStandardRebar_AI_DbContext db = null)
        {
            if (db == null) db = new ColumnStandardRebar_AI_DbContext();

            List<int> idEleOrder = eleOrder.Select(x => BeamElevationDao.GetId(x)).ToList();

            try
            {
                GetId(eleOrder, db);
            }
            catch
            {
                var res = new BeamElevationDesign()
                {
                    CreateDate = DateTime.Now,
                    ElevationOrder = ConstantValue.CombineOrder(idEleOrder)
                };
                db.BeamElevationDesigns.Add(res);
                db.SaveChanges();
            }
        }

        public static int GetId(List<int> eleOrder, ColumnStandardRebar_AI_DbContext db = null)
        {
            if (db == null) db = new ColumnStandardRebar_AI_DbContext();

            if (eleOrder.Count < 2) throw new InvalidInputException();

            List<int> idEleOrder = eleOrder.Select(x => BeamElevationDao.GetId(x)).ToList();
            string sEleOrder = ConstantValue.CombineOrder(idEleOrder);

            var obj = db.BeamElevationDesigns.Where(x => x.ElevationOrder == sEleOrder);
            if (obj.Count() == 0) throw new InvalidDataException();

            return obj.First().ID;
        }
        public static BeamElevationDesign GetElevationDesign(int id, ColumnStandardRebar_AI_DbContext db= null)
        {
            if (db == null) db = new ColumnStandardRebar_AI_DbContext();

            var obj = db.BeamElevationDesigns.Where(x => x.ID == id);
            if (obj.Count() == 0) throw new InvalidIDException();

            return obj.First();
        }
        public static List<int> GetHeights(int id, ColumnStandardRebar_AI_DbContext db = null)
        {
            return GetElevationDesign(id, db).ElevationOrder.Split('-').Select(x => BeamElevationDao.GetElevation(int.Parse(x), db)).ToList();
        }
    }
}
