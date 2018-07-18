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
            int index = SingleWPF.Instance.Prefix.Length+1;

            int i =int.Parse(x.Name.Split('-')[1].Substring(index));
            int j = int.Parse(y.Name.Split('-')[1].Substring(index));
            return i.CompareTo(j);
        }
    }
}
