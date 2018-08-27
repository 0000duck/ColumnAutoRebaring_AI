using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Addin1Python
{
    public class LineEquation
    {
        // x = x0 + ta
        // y = y0 + tb
        // z = z0 + tc
        public XYZ BasePoint { get; set; }
        public XYZ Direction { get; set; }
        public LineEquation(XYZ point, XYZ vector)
        {
            BasePoint = point;

            Direction = vector.Normalize();
        }
        public LineEquation(List<XYZ> list2Pnts): this(list2Pnts[0], list2Pnts[1]- list2Pnts[0])
        {
        }
        public XYZ Evaluate(double t)
        {
            return BasePoint + Direction * t;
        }
    }
    public static class AnalyticGeometryUtilitiy
    {
        public static double SquareDistance(XYZ point)
        {
            return point.X * point.X + point.Y * point.Y + point.Z * point.Z;
        }
        public static XYZ GetProjectPointOnLine(XYZ point, LineEquation le)
        {
            double t = ((point.X - le.BasePoint.X) * le.Direction.X + (point.Y - le.BasePoint.Y) * le.Direction.Y + (point.Z - le.BasePoint.Z) * le.Direction.Z) / (SquareDistance(le.Direction));
            return le.Evaluate(t);
        }
        public static LineEquation GetPerpendicularLineEquationOnLine(XYZ point, LineEquation le)
        {
            return new LineEquation(new List<XYZ> { point, GetProjectPointOnLine(point, le) });
        }
        public static XYZ GetIntersectionPoints(LineEquation le1, LineEquation le2)
        {
            // a1t1 - a2t2 + x1 - x2 =0
            // b1t1 - b2t2 + y1 - y2 =0
            // c1t1 - c2t2 + z1 - z2 =0
            List<double> inputs1 = new List<double> { le1.Direction.X, -le2.Direction.X, le1.BasePoint.X - le2.BasePoint.X };
            List<double> inputs2 = new List<double> { le1.Direction.Y, -le2.Direction.Y, le1.BasePoint.Y - le2.BasePoint.Y };
            List<double> inputs3 = new List<double> { le1.Direction.Z, -le2.Direction.Z, le1.BasePoint.Z - le2.BasePoint.Z };
            List<double> valueTs = EquationSolver.System2Hidden(inputs1, inputs2);
            if (!(EquationSolver.CheckValueInSystem2Hidden(valueTs, inputs3))) throw new Exception("Hai line không giao nhau");
            return le1.Evaluate(valueTs[0]);
        }
    }
}
