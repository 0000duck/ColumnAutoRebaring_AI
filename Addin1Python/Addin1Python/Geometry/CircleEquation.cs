using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Addin1Python
{
    public class CircleEquation
    {
        // Chiều dương là chiều ngược kim đồng hồ
        private double perimeter = -1;
        private XYZ center;
        

        public UV CenterUV { get; set; }
        public double Z { get; set; }
        public double Radius { get; set; }
        public List<Arc> StandardArcs { get; set; } = new List<Arc>();
        public double Perimeter
        {
            get
            {
                if (perimeter == -1)
                    perimeter = Radius * 2 * Math.PI;
                return perimeter;
            }
        }
        public XYZ Center
        {
            get
            {
                if (center == null)
                    center = new XYZ(CenterUV.U, CenterUV.V, Z);
                return center;
            }
        }


        public CircleEquation(UV center, double radius)
        {
            CenterUV = center; Radius = radius; Z = 0;
        }
        public CircleEquation(XYZ center, double radius) : this(new UV(center.X, center.Y), radius) { Z = center.Z; }
        public CircleEquation(Arc arc) : this(arc.Center, arc.Radius) { }

        public double ConvertLength2Angle(double length)
        {
            return length / Perimeter * (Math.PI * 2);
        }
        public XYZ ConvertUV2XYZ(UV uv)
        {
            return new XYZ(uv.U, uv.V, Z);
        }
        public XYZ GetEndPoint(double angle)
        {
            return ConvertUV2XYZ(GetEndPointUV(angle));
        }
        public UV GetEndPointUV(double angle)
        {
            return new UV(CenterUV.U + Radius * Math.Cos(angle),CenterUV.V +Radius * Math.Sin(angle));
        }
        public Arc GetArc(double startAngle, double plusAngle)
        {
            return Arc.Create(GetEndPoint(startAngle), GetEndPoint(startAngle, plusAngle), GetEndPoint(startAngle, plusAngle / 2));
        }
        public XYZ GetEndPoint(double startAngle, double plusAngle)
        {
            return ConvertUV2XYZ(GetEndPointUV(startAngle, plusAngle));
        }
        public UV GetEndPointUV(double startAngle, double plusAngle)
        {
            return GetEndPointUV(startAngle + plusAngle);
        }
        public Arc GetArc(double startAngle, double length, bool isOtherwiseClock)
        {
            double plusAngle = isOtherwiseClock ? ConvertLength2Angle(length) : -ConvertLength2Angle(length);
            return GetArc(startAngle, plusAngle);
        }
        public XYZ GetEndPoint(double startAngle, double length, bool isOtherwiseClock)
        {
            return ConvertUV2XYZ(GetEndPointUV(startAngle, length, isOtherwiseClock));
        }
        public UV GetEndPointUV(double startAngle, double length, bool isOtherwiseClock)
        {
            double plusAngle = isOtherwiseClock ? ConvertLength2Angle(length) : -ConvertLength2Angle(length);
            return GetEndPointUV(startAngle, plusAngle);
        }

        public Arc GetArc(double firstInitAngle, double distance1, double plusLength, bool isOtherwiseClock)
        {
            double startAngle = firstInitAngle + (isOtherwiseClock ? ConvertLength2Angle(distance1) : -ConvertLength2Angle(distance1));
            return GetArc(startAngle, plusLength, isOtherwiseClock);
        }
        public void CalculateDistancesList(double targetLength)
        {
            double trueLength = targetLength - Singleton.Instance.SelectedBarDiameter * ConstantValue.MultiplyDevelopment;
            double num = Math.Floor(Perimeter / trueLength);
            for (int i = 0; i < num; i++)
            {
                StandardArcs.Add(GetArc(Math.PI, i * trueLength, targetLength, false));
            }
            StandardArcs.Add(GetArc(Math.PI, num * trueLength, perimeter - num *trueLength + Singleton.Instance.SelectedBarDiameter * ConstantValue.MultiplyDevelopment, false));
        }
    }
}
