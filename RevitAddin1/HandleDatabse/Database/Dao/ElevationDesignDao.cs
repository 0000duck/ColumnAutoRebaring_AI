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
    public static class ElevationDesignDao
    {
        public static void Insert(List<int> eleOrder, List<int> shortenOrder, ColumnStandardRebar_AI_DbContext db = null)
        {
            if (db == null) db = new ColumnStandardRebar_AI_DbContext();

            List<int> idEleOrder = eleOrder.Select(x => ElevationDao.GetId(x)).ToList();

            try
            {
                GetId(eleOrder,shortenOrder, db);
            }
            catch
            {
                string sShortenOrder = (shortenOrder == null) ? null : ConstantValue.CombineOrder(shortenOrder);
                var res = new ElevationDesign()
                {
                    CreateDate = DateTime.Now,
                    ElevationOrder = ConstantValue.CombineOrder(idEleOrder),
                    ShortenOrder = sShortenOrder
                };
                db.ElevationDesigns.Add(res);
                db.SaveChanges();
            }
        }

        public static int GetId(List<int> eleOrder, List<int> shortenOrder, ColumnStandardRebar_AI_DbContext db = null)
        {
            if (db == null) db = new ColumnStandardRebar_AI_DbContext();

            if (eleOrder.Count < 2) throw new InvalidInputException();

            List<int> idEleOrder = eleOrder.Select(x => ElevationDao.GetId(x)).ToList();
            string sEleOrder = ConstantValue.CombineOrder(idEleOrder);

            string sShortenOrder = (shortenOrder == null) ? null : ConstantValue.CombineOrder(shortenOrder);

            var obj = db.ElevationDesigns.Where(x => x.ElevationOrder == sEleOrder && x.ShortenOrder == sShortenOrder);
            if (obj.Count() == 0) throw new InvalidDataException();

            return obj.First().ID;
        }
        public static ElevationDesign GetElevationDesign(int id, ColumnStandardRebar_AI_DbContext db= null)
        {
            if (db == null) db = new ColumnStandardRebar_AI_DbContext();

            var obj = db.ElevationDesigns.Where(x => x.ID == id);
            if (obj.Count() == 0) throw new InvalidIDException();

            return obj.First();
        }
        public static ElevationDesign GetElevationDesign(List<int> eleOrder, List<int> shortenOrder, ColumnStandardRebar_AI_DbContext db = null)
        {
            if (db == null) db = new ColumnStandardRebar_AI_DbContext();

            return GetElevationDesign(GetId(eleOrder, shortenOrder, db));
        }
        public static List<int> GetHeights(int id, ColumnStandardRebar_AI_DbContext db = null)
        {
            if (db == null) db = new ColumnStandardRebar_AI_DbContext();

            return GetElevationDesign(id, db).ElevationOrder.Split('-').Select(x => ElevationDao.GetElevation(int.Parse(x), db)).ToList();
        }
        public static List<int> GetShortenOrder(int id, ColumnStandardRebar_AI_DbContext db = null)
        {
            if (db == null) db = new ColumnStandardRebar_AI_DbContext();

            string sShortenOrder = GetElevationDesign(id, db).ShortenOrder;
            return sShortenOrder == null ? new List<int>() : sShortenOrder.Split('-').Select(x => int.Parse(x)).ToList();
        }
    }
}
