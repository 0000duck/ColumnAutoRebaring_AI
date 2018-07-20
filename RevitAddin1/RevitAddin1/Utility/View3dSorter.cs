using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAddin1
{
    public class View3DSorter : IComparer<View3D>
    {
        public int Compare(View3D x, View3D y)
        {
            int i = int.Parse(x.Name.Split('_')[1]);
            int j = int.Parse(y.Name.Split('_')[1]);
            return i.CompareTo(j);
        }
    }
}
