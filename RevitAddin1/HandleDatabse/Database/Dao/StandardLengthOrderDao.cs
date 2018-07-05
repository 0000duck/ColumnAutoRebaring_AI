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
    public static class StandardLengthOrderDao
    {
        public static void Insert(List<int> lens, StandardLengthEnum standLenType, ColumnStandardRebar_AI_DbContext db = null)
        {
            if (db == null) db = new ColumnStandardRebar_AI_DbContext();

            int idStandLenType = StandardLengthTypeDao.GetId(standLenType, db);
            List<int> idStandLenOrder = lens.OrderByDescending(x => x).Select(x => StandardLengthDao.GetId(x, null)).ToList();
            string sStandLenOrder = ConstantValue.CombineOrder(idStandLenOrder);

            try
            {
                GetId(lens, standLenType, db);
            }
            catch
            {
                var res = new StandardLengthOrder()
                {
                    CreateDate = DateTime.Now,
                    IDType = idStandLenType,
                    StandardLengthOrder1 = sStandLenOrder
                };
                db.StandardLengthOrders.Add(res);
                db.SaveChanges();
            }
        }
        public static int GetId(List<int> lens, StandardLengthEnum standLenType, ColumnStandardRebar_AI_DbContext db = null)
        {
            if (db == null) db = new ColumnStandardRebar_AI_DbContext();

            int idStandLenType = StandardLengthTypeDao.GetId(standLenType, db);
            List<int> idStandLenOrder = lens.OrderByDescending(x=>x).Select(x => StandardLengthDao.GetId(x, null)).ToList();
            string sStandLenOrder = ConstantValue.CombineOrder(idStandLenOrder);

            var obj = db.StandardLengthOrders.Where(x => x.IDType == idStandLenType && x.StandardLengthOrder1== sStandLenOrder);
            if (obj.Count() == 0) throw new InvalidDataException();

            return obj.First().ID;
        }
        public static bool Check(int id, StandardLengthEnum standLenType, ColumnStandardRebar_AI_DbContext db = null)
        {
            if (db == null) db = new ColumnStandardRebar_AI_DbContext();

            var obj = db.StandardLengthOrders.Where(x => x.ID == id);
            if (obj.Count() == 0) throw new InvalidIDException();

            var standLenObj = StandardLengthTypeDao.GetStandardLengthType(obj.First().IDType);
            if (standLenObj.Value == (standLenType.ToString()))
            {
                return true;
            }
            return false;
        }
        public static StandardLengthOrder GetStandardLengthOrder(int id, ColumnStandardRebar_AI_DbContext db = null)
        {
            if (db == null) db = new ColumnStandardRebar_AI_DbContext();

            var obj = db.StandardLengthOrders.Where(x => x.ID == id);
            if (obj.Count() == 0) throw new InvalidIDException();

            return obj.First();
        }
        public static List<int> GetStandardLengths(int id, ColumnStandardRebar_AI_DbContext db = null)
        {
            if (db == null) db = new ColumnStandardRebar_AI_DbContext();

            var standLenOrder = GetStandardLengthOrder(id, db);
            return standLenOrder.StandardLengthOrder1.Split('-').Select(x => StandardLengthDao.GetLength(int.Parse(x))).ToList();
        }
    }
}
