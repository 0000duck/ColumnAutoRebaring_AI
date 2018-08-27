using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_CAR
{
    public static class FullStandardLengthOrderDao
    {
        public static void Insert(int idOrderL, int idOrderL2, int idOrderL3, int idOrderI, int idOrderI2, ColumnStandardRebar_AI_DbContext db = null)
        {
            if (db == null) db = new ColumnStandardRebar_AI_DbContext();

            try
            {
                GetId(idOrderL, idOrderL2, idOrderL3, idOrderI, idOrderI2, db);
            }
            catch (InvalidDataException)
            {
                string sFullStandLenOrder = $"{idOrderL}-{idOrderL2}-{idOrderL3}-{idOrderI}-{idOrderI2}";
                var res = new FullStandardLengthOrder()
                {
                    CreateDate = DateTime.Now,
                    FullStandardLengthOrder1 = sFullStandLenOrder
                };
                db.FullStandardLengthOrders.Add(res);
                db.SaveChanges();
            }
        }
        public static int GetId(int idOrderL, int idOrderL2, int idOrderL3, int idOrderI, int idOrderI2, ColumnStandardRebar_AI_DbContext db = null)
        {
            if (db == null) db = new ColumnStandardRebar_AI_DbContext();

            if (!StandardLengthOrderDao.Check(idOrderL, StandardLengthEnum.L)) throw new InvalidInputException();
            if (!StandardLengthOrderDao.Check(idOrderL2, StandardLengthEnum.L2)) throw new InvalidInputException();
            if (!StandardLengthOrderDao.Check(idOrderL3, StandardLengthEnum.L3)) throw new InvalidInputException();
            if (!StandardLengthOrderDao.Check(idOrderI, StandardLengthEnum.I)) throw new InvalidInputException();
            if (!StandardLengthOrderDao.Check(idOrderI2, StandardLengthEnum.I2)) throw new InvalidInputException();

            string sFullStandLenOrder = $"{idOrderL}-{idOrderL2}-{idOrderL3}-{idOrderI}-{idOrderI2}";
            var obj = db.FullStandardLengthOrders.Where(x => x.FullStandardLengthOrder1 == sFullStandLenOrder);
            if (obj.Count() == 0) throw new InvalidDataException(); ;

            return obj.First().ID;
        }
        public static int InsertAndGetId(int idOrderL, int idOrderL2, int idOrderL3, int idOrderI, int idOrderI2, ColumnStandardRebar_AI_DbContext db = null)
        {
            if (db == null) db = new ColumnStandardRebar_AI_DbContext();

            Insert(idOrderL, idOrderL2, idOrderL3, idOrderI, idOrderI2, db);
            return GetId(idOrderL, idOrderL2, idOrderL3, idOrderI, idOrderI2, db);
        }
        public static FullStandardLengthOrder GetFullStandardLengthOrder(int id, ColumnStandardRebar_AI_DbContext db = null)
        {
            if (db == null) db = new ColumnStandardRebar_AI_DbContext();

            var obj = db.FullStandardLengthOrders.Where(x => x.ID == id);
            if (obj.Count() == 0) throw new InvalidIDException();

            return obj.First();
        }
        public static List<int> GetStandardLengths(int idFullStandLenOrder, StandardLengthEnum stanLenEnum, ColumnStandardRebar_AI_DbContext db = null)
        {
            if (db == null) db = new ColumnStandardRebar_AI_DbContext();

            var fullStandLenOrder = GetFullStandardLengthOrder(idFullStandLenOrder, db);
            int idStandLenOrder = int.Parse(fullStandLenOrder.FullStandardLengthOrder1.Split('-')[(int)stanLenEnum]);
            return StandardLengthOrderDao.GetStandardLengths(idStandLenOrder, db);
        }
    }
}
