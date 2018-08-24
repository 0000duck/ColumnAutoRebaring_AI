using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_CAR
{
    public static class ConstantValue
    {
        public static double milimeter2feet = 0.00328084;
        public static List<UV> selectedUVs = new List<UV>() { new UV(-40.3, 25.5), new UV(-39.7, 25.5), new UV(-34.8, 25.5), new UV(-35.4, 25.5) };
        public static int start1 = 2900;
        public static int start2 = 1500;

        public static string ConnectionString = "data source=103.252.252.163;initial catalog=ColumnStandardRebar_AI;persist security info=True;user id=misery;password=Skarner116!;MultipleActiveResultSets=True;App=EntityFramework";
        public static string NegativeLength = "Kích thước bị âm.";
        public static string InvalidData = "Không tồn tại giá trị trong bảng.";
        public static string InvalidInput = "Tham biến truyền vào không hợp lệ.";
        public static string InvalidID = "ID không tồn tại.";
        public static string NotContainValue1 = "Không thể thêm giá trị là 1 vào bảng.";

        public static int MaxLength = 11700;

        public static string CombineOrder(List<int> order)
        {
            string s = string.Empty;
            for (int i = 0; i < order.Count; i++)
            {
                s += order[i];
                if (i != order.Count - 1) s += "-";
            }
            return s;
        }
    }
}
