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
        public int IndexX { get; set; } = -1;
        public int IndexY { get; set; } = -1;
        public int CountX { get; set; } = -1;
        public int CountY { get; set; } = -1;
        public double Radius { get; set; } = -1;
        public RebarType RebarType { get; set; }
        public RebarInfo(Rebar rebar, int indexX, int countX, double radius, RebarType rebarType)
        {
            Rebar = rebar;
            IndexX = indexX;
            CountX = countX;
            Radius = radius;
            RebarType = rebarType;
        }
    }
    public enum RebarType
    {
        Type1, Type2
    }
}
