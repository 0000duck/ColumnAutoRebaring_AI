using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_CAR
{
    public static class RebarDesignDao
    {
        public static void Insert(List<string> diaOrder, List<int> locOrder, ColumnStandardRebar_AI_DbContext db = null)
        {
            if (db == null) db = new ColumnStandardRebar_AI_DbContext();

            if (diaOrder.Count != locOrder.Count) throw new Exception(ConstantValue.InvalidInput);
            List<int> idRebarOrder = diaOrder.Select(x => RebarDiameterDao.GetId(x)).ToList();

            try
            {
                GetId(diaOrder, locOrder, db);
            }
            catch
            {
                var res = new RebarDesign()
                {
                    CreateDate = DateTime.Now,
                    DiameterOrder = ConstantValue.CombineOrder(idRebarOrder),
                    LocationOrder = ConstantValue.CombineOrder(locOrder)
                };
                db.RebarDesigns.Add(res);
                db.SaveChanges();
            }
        }
        public static int GetId(List<string> diaOrder, List<int> locOrder, ColumnStandardRebar_AI_DbContext db = null)
        {
            if (db == null) db = new ColumnStandardRebar_AI_DbContext();

            if (diaOrder.Count != locOrder.Count) throw new InvalidInputException();
            List<int> idRebarOrder = diaOrder.Select(x => RebarDiameterDao.GetId(x)).ToList();

            string sdiaOrder = ConstantValue.CombineOrder(idRebarOrder);
            string slocOrder = ConstantValue.CombineOrder(locOrder);
            var obj = db.RebarDesigns.Where(x => x.DiameterOrder == sdiaOrder && x.LocationOrder == slocOrder);
            if (obj.Count() == 0) throw new InvalidDataException();

            return obj.First().ID;
        }
        public static int InsertAndGetId(List<string> diaOrder, List<int> locOrder, ColumnStandardRebar_AI_DbContext db = null)
        {
            if (db == null) db = new ColumnStandardRebar_AI_DbContext();

            Insert(diaOrder, locOrder, db);
            return GetId(diaOrder, locOrder, db);
        }
        public static RebarDesign GetRebarDesign(int id, ColumnStandardRebar_AI_DbContext db = null)
        {
            if (db == null) db = new ColumnStandardRebar_AI_DbContext();

            var obj = db.RebarDesigns.Where(x => x.ID == id);
            if (obj.Count() == 0) throw new InvalidIDException();

            return obj.First();
        }
        public static Tuple<List<int>, List<int>> GetDiametersAndLocations(int id, ColumnStandardRebar_AI_DbContext db = null)
        {
            var obj = GetRebarDesign(id);
            List<int> diameters = obj.DiameterOrder.Split('-').Select(x => RebarDiameterDao.GetRebarDiameter(int.Parse(x), db)).ToList();
            List<int> locations = obj.LocationOrder.Split('-').Select(x => int.Parse(x)).ToList();

            return Tuple.Create(diameters, locations);
        }
        public static Tuple<List<int>, List<int>> GetDiametersAndLocations(List<string> diaOrder, List<int> locOrder, ColumnStandardRebar_AI_DbContext db = null)
        {
            return GetDiametersAndLocations(InsertAndGetId(diaOrder, locOrder));
        }
    }
}
