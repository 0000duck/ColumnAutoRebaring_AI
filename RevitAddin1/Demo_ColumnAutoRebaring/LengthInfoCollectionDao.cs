using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using HandleDatabse.ProjectData.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PSingleton = Project.Singleton;
using LSingleton = HandleDatabse.ProjectData.EF.Singleton;

namespace Project
{
    public class LengthInfoCollectionDao
    {
        public static void CreateRebar()
        {
            LengthInfoCollection lenInfoColl = LSingleton.Instance.ChosenLengthInfoCollection;
            for (int i = 0; i < LSingleton.Instance.LoopCount-1; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    UV pnt = null;
                    if (i % 2 == 0)
                    {
                        pnt = PSingleton.Instance.UVs[j == 0 ? 0 : 2];
                    }
                    else
                    {
                        pnt = PSingleton.Instance.UVs[j == 0 ? 1 : 3];
                    }

                    LengthInfo lenInfo = lenInfoColl.GetIndex(i, j);
                    int start = lenInfo.Start;
                    int end = lenInfo.End;
                    CreateRebar(lenInfo.DiameterName, pnt, lenInfo.Start, lenInfo.End, $"{lenInfo.LengthType}", $"{lenInfo.LengthPosition}");
                }
            }
            
        }
        public static Rebar CreateRebar(string rebarName, UV pnt, double start, double end, string type, string position)
        {
            start = start * Project.ConstantValue.milimeter2feet;
            end = end * Project.ConstantValue.milimeter2feet;
            Rebar rb= Rebar.CreateFromCurves(PSingleton.Instance.Document, RebarStyle.Standard, PSingleton.Instance.GetRebarType(rebarName), null, null, PSingleton.Instance.Element, XYZ.BasisY,
                new List<Curve> { Line.CreateBound(new XYZ(pnt.U, pnt.V, start), new XYZ(pnt.U, pnt.V, end)) }, RebarHookOrientation.Left, RebarHookOrientation.Left, true, false);
            rb.SetUnobscuredInView(PSingleton.Instance.Document.ActiveView, true);
            rb.LookupParameter("Comments").Set("add-in");
            rb.LookupParameter("Type").Set(type);
            rb.LookupParameter("Position").Set(position);
            return rb;
        }
    }
}
