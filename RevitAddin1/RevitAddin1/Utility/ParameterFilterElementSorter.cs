using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAddin1
{
    public class ParameterFilterElementSorter : IComparer<ParameterFilterElement>
    {
        public int Compare(ParameterFilterElement x, ParameterFilterElement y)
        {
            int i =int.Parse(x.Name.Split('-','_')[2]);
            int j = int.Parse(y.Name.Split('-', '_')[2]);
            return i.CompareTo(j);
        }
    }
}
