using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project
{
    public static  class ConstantValue
    {
        public static double milimeter2feet = 0.00328084;
        public static List<UV> selectedUVs = new List<UV>() { new UV(-40.3, 25.5), new UV(-39.7, 25.5), new UV(-34.8, 25.5), new UV(-35.4, 25.5) };
        public static string selectedRebarName = "T20";
        public static int start1 = 2900;
        public static int start2 = 1500;
    }
}
