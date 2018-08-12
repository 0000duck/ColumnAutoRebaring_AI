using Autodesk.Revit.DB.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Addin1Python
{
    public class RebarInfo
    {
        public Rebar Rebar { get; set; }
        public int Count { get; set; }
        public int Index { get; set; }
        public double Radius { get; set; }
        public RebarType RebarType { get; set; }
    }

    public enum RebarType
    {
        Type1, Type2
    }
}
