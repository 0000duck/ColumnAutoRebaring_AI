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
    public static class DataCombineDao
    {
        public static void Insert(int idRebarDes, int idEleDes, int idBeamEleDes, int idEleOff, int idDevLen, int idLenInfo, int idStartOff, int idFullStandLenOrder, ColumnStandardRebar_AI_DbContext db = null)
        {
            if (db == null) db = new ColumnStandardRebar_AI_DbContext();

            try
            {
                GetId(idRebarDes, idEleDes,idBeamEleDes, idEleOff, idDevLen, idLenInfo, idStartOff, idFullStandLenOrder);
            }
            catch
            {
                var res = new DataCombine()
                {
                    CreateDate = DateTime.Now,
                    IDRebarDesign = idRebarDes,
                    IDElevationDesign = idEleDes,
                    IDBeamElevationDesign = idBeamEleDes,
                    IDElevationOffset = idEleOff,
                    IDDevelopmentLength = idDevLen,
                    IDLengthInformation = idLenInfo,
                    IDStartOffset = idStartOff,
                    IDFullStandardLengthOrder = idFullStandLenOrder
                };
                db.DataCombines.Add(res);
                db.SaveChanges();
            }
        }
        public static int GetId(int idRebarDes, int idEleDes, int idBeamEleDes, int idEleOff, int idDevLen, int idLenInfo, int idStartOff, int idFullStandLenOrder, ColumnStandardRebar_AI_DbContext db = null)
        {
            if (db == null) db = new ColumnStandardRebar_AI_DbContext();

            var obj = db.DataCombines.Where(x => x.IDRebarDesign == idRebarDes && x.IDElevationDesign == idEleDes && x.IDElevationOffset==idEleOff && x.IDBeamElevationDesign== idBeamEleDes && 
                    x.IDStartOffset == idStartOff && x.IDDevelopmentLength == idDevLen && x.IDLengthInformation == idLenInfo && x.IDFullStandardLengthOrder == idFullStandLenOrder);
            if (obj.Count() == 0) throw new InvalidDataException(); ;

            return obj.First().ID;
        }
        public static DataCombine GetDataCombine(int idRebarDes, int idEleDes, int idBeamEleDes, int idEleOff, int idDevLen, int idLenInfo, int idStartOff, int idFullStandLenOrder, ColumnStandardRebar_AI_DbContext db = null)
        {
            if (db == null) db = new ColumnStandardRebar_AI_DbContext();

            var obj = db.DataCombines.Where(x => x.IDRebarDesign == idRebarDes && x.IDElevationDesign == idEleDes && x.IDElevationOffset == idEleOff && x.IDBeamElevationDesign == idBeamEleDes &&
                    x.IDStartOffset == idStartOff && x.IDDevelopmentLength == idDevLen && x.IDLengthInformation == idLenInfo && x.IDFullStandardLengthOrder == idFullStandLenOrder);
            if (obj.Count() == 0) throw new InvalidDataException(); ;

            return obj.First();
        }
    }
}
