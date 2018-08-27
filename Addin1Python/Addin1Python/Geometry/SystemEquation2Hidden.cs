using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Geometry;

namespace Addin1Python
{
    public static class EquationSolver
    {
        public static List<double> System2Hidden(List<double> inputs1, List<double> inputs2)
        {
            // y = -(c2a1 - c1a2) / (b2a1 - b1a2)
            double y = -(inputs1[0] * inputs2[2] - inputs2[0] * inputs1[2]) / (inputs1[0] * inputs2[1] - inputs2[0] * inputs1[1]);
            double x = -(inputs1[2] + inputs1[1] * y) / inputs1[0];
            return new List<double> { x, y };
        }
        public static bool CheckValueInSystem2Hidden(List<double> values, List<double> inputs)
        {
            if (GeomUtil.IsEqual(inputs[0] * values[0] + inputs[1] * values[1] + inputs[2], 0)) return true;
            return false;
        }
    }
}
