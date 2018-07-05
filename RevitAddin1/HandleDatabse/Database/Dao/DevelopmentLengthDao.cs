﻿using HandleDatabse.Constant;
using HandleDatabse.Database.EF;
using HandleDatabse.ExceptionCreation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandleDatabse.Database.Dao
{
    public static  class DevelopmentLengthDao
    {
        public static void Insert(int devMulti, int devOff, int implantMulti, ColumnStandardRebar_AI_DbContext db = null)
        {
            if (db== null) db = new ColumnStandardRebar_AI_DbContext();

            try
            {
                GetId(devMulti, devOff, implantMulti, db);
            }
            catch
            {
                var res = new DevelopmentLength()
                {
                    CreateDate = DateTime.Now,
                    DevelopMultilply = devMulti,
                    DevelopOffset = devOff,
                    ImplantMultilply = implantMulti
                };
                db.DevelopmentLengths.Add(res);
                db.SaveChanges();
            }
        }
        public static int GetId(int devMulti, int devOff, int implantMulti, ColumnStandardRebar_AI_DbContext db = null)
        {
            if (db == null) db = new ColumnStandardRebar_AI_DbContext();

            var obj = db.DevelopmentLengths.Where(x => x.DevelopMultilply == devMulti && x.DevelopOffset == devOff && x.ImplantMultilply == implantMulti);
            if (obj.Count() == 0) throw new InvalidDataException();
            return obj.First().ID;
        }
        public static DevelopmentLength GetDevelopmentLength(int id, ColumnStandardRebar_AI_DbContext db = null)
        {
            if (db == null) db = new ColumnStandardRebar_AI_DbContext();

            var obj = db.DevelopmentLengths.Where(x => x.ID == id);
            if (obj.Count() == 0) throw new InvalidIDException();
            return obj.First();
        }
    }
}