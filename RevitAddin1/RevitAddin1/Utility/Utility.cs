using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAddin1
{
    public static class Utility
    {
        public static View3D Clone3DView(View3D v3d)
        {
            View3D v3dClone = ElementTransformUtils.CopyElement(Singleton.Instance.Document, v3d.Id, XYZ.BasisZ).Select(x=> Singleton.Instance.Document.GetElement(x)).First() as View3D;
            v3dClone.Name = $"{v3d.Name[0]}{int.Parse(v3d.Name.Substring(1)) + 1}";
            return v3dClone;
        }
    }
}
