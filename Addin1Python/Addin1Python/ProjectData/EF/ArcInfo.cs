using Autodesk.Revit.DB;
using Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Addin1Python
{
    public class ArcInfo
    {
        public CircleEquation CircleEquation { get; set; }
        public double StartAngle { get; set; }
        public double EndAngle { get; set; }
        public List<Arc> Arcs { get; set; }
        public List<Curve> Curves { get; set; }
        public RebarType RebarType { get; set; } = RebarType.Type1;
        public XYZ Direction
        {
            get
            {
                return (CircleEquation.GetEndPoint(EndAngle) - CircleEquation.GetEndPoint(StartAngle)).Normalize();
            }
        }
        public XYZ MidPoint
        {
            get
            {
                return CircleEquation.GetEndPoint((StartAngle + EndAngle)/2);
            }
        }
        public XYZ PlaceGroupPoint
        {
            get
            {
                double len = 40 * GeomUtil.milimeter2Feet(20);
                double angle = CircleEquation.ConvertLength2Angle(len);
                XYZ vec = -XYZ.BasisZ.CrossProduct(DirectionGroupPoint);
                return CircleEquation.GetEndPoint(EndAngle) + vec * GeomUtil.milimeter2Feet(250);
            }
        }
        public XYZ DirectionGroupPoint
        {
            get
            {
                double len = 40 * GeomUtil.milimeter2Feet(20);
                double angle = CircleEquation.ConvertLength2Angle(len);
                return (CircleEquation.GetEndPoint(EndAngle) - CircleEquation.GetEndPoint(EndAngle - angle)).Normalize();
            }
        }
        public ArcInfo(CircleEquation ce, double startAngle, double endAngle, RebarType rebarType = RebarType.Type1)
        {
            CircleEquation = ce;
            StartAngle = startAngle;
            EndAngle = endAngle;
            switch (rebarType)
            {
                case RebarType.Type1:
                    Arcs = new List<Arc> { Arc.Create(ce.GetEndPoint(startAngle), ce.GetEndPoint(endAngle), ce.GetEndPoint(startAngle, (endAngle - startAngle) / 2)) };
                    Curves = Arcs.Cast<Curve>().ToList();
                    break;
                case RebarType.Type2:
                    Curves = new List<Curve>();
                    Curves.Add(Arc.Create(ce.Center, ce.Radius, 0, Math.PI * 2, XYZ.BasisX, XYZ.BasisY));
                    XYZ startPnt = ce.GetEndPoint(Math.PI);
                    Curves.Add(Line.CreateBound(startPnt, startPnt + XYZ.BasisY * (40 * GeomUtil.milimeter2Feet(20))));
                    break;
            }
        }
        public ArcInfo(double radius, double startAngle, double endAngle, double offset, RebarType rebarType = RebarType.Type1): this(new CircleEquation(Singleton.Instance.SelectedXYZ, radius- offset), startAngle, endAngle, rebarType)
        {
        }
        public void AddArc(double startAngle, double endAngle)
        {
            Arcs.Add(Arc.Create(CircleEquation.GetEndPoint(startAngle), CircleEquation.GetEndPoint(endAngle), CircleEquation.GetEndPoint(startAngle, (endAngle - startAngle) / 2)));
            RebarType = RebarType.Type2;
        }
        public void AddArc(ArcInfo arcInfo)
        {
            Arcs.AddRange(arcInfo.Arcs);
            RebarType = RebarType.Type2;
        }
    }
}
