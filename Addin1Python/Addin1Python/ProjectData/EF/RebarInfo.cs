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
        public ArcInfo ArcInfo { get; set; }
        public RebarType RebarType { get; set; }
        public RebarInfo(Rebar rebar, ArcInfo arcInfo, int indexX, int countX, RebarType rebarType)
        {
            Rebar = rebar;
            ArcInfo = arcInfo;
            IndexX = indexX;
            CountX = countX;
            RebarType = rebarType;
        }
    }
    public enum RebarType
    {
        Type1, Type2
    }
}
