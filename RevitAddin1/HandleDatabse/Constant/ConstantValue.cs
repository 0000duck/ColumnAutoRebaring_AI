using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandleDatabse.Constant
{
    public static class ConstantValue
    {
        public static string ConnectionString = "data source=103.252.252.163;initial catalog=ColumnStandardRebar_AI;persist security info=True;user id=misery;password=Skarner116!;MultipleActiveResultSets=True;App=EntityFramework";
        public static string NegativeLength = "Kích thước bị âm.";
        public static string InvalidData = "Không tồn tại giá trị trong bảng.";
        public static string InvalidInput = "Tham biến truyền vào không hợp lệ.";
        public static string InvalidID = "ID không tồn tại.";

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
