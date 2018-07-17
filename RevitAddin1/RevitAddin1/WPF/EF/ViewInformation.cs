using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAddin1
{
    public class ViewInformation
    {
        public string Name { get; set; }
        public int FromDate { get; set; }
        public int ToDate { get; set; }
        public string RevitName
        {
            get
            {
                return $"0-{Name}-{FromDate}-{ToDate}";
            }
        }
    }
}
