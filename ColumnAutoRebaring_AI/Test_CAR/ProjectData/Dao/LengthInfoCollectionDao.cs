using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_CAR
{
    public static class LengthInfoCollectionDao
    {
        public static void GetLengthInfoCollection()
        {
            int residual = Singleton.Instance.OtherData.LengthInfoCollections[0].Residual;
            int residual2Temp = Singleton.Instance.OtherData.LengthInfoCollections[0].Residual2;
            int residual2 = Singleton.Instance.OtherData.LengthInfoCollections[0].Residual2;

            int indexI = 0, indexJ = 0;

            for (int i = 0; i < Singleton.Instance.OtherData.LengthInfoCollections.Count; i++)
            {
                if (residual > Singleton.Instance.OtherData.LengthInfoCollections[i].Residual)
                {
                    indexI = i;
                    residual = Singleton.Instance.OtherData.LengthInfoCollections[i].Residual;
                    residual2Temp = Singleton.Instance.OtherData.LengthInfoCollections[i].Residual2;
                }
                else if (residual == Singleton.Instance.OtherData.LengthInfoCollections[i].Residual)
                {
                    if (residual2Temp > Singleton.Instance.OtherData.LengthInfoCollections[i].Residual2)
                    {
                        indexI = i;
                        residual2Temp = Singleton.Instance.OtherData.LengthInfoCollections[i].Residual2;
                    }
                }

                if (residual2 > Singleton.Instance.OtherData.LengthInfoCollections[i].Residual2)
                {
                    indexJ = i;
                    residual2 = Singleton.Instance.OtherData.LengthInfoCollections[i].Residual2;
                }
            }

            Singleton.Instance.OtherData.AOLLengthInfoCollection = Singleton.Instance.OtherData.LengthInfoCollections[indexI];
            Singleton.Instance.OtherData.NAOLLengthInfoCollection = Singleton.Instance.OtherData.LengthInfoCollections[indexJ];
        }
        public static void CreateRebar()
        {
            LengthInfoCollection lenInfoColl = Singleton.Instance.OtherData.ChosenLengthInfoCollection;
            for (int i = 0; i < Singleton.Instance.OtherData.LoopCount - 1; i++)
            {
                for (int j = 0; j < 2; j++)
                {
                    UV pnt = null;
                    if (i % 2 == 0)
                    {
                        pnt = Singleton.Instance.RevitData.UVs[j == 0 ? 0 : 2];
                    }
                    else
                    {
                        pnt = Singleton.Instance.RevitData.UVs[j == 0 ? 1 : 3];
                    }

                    LengthInfo lenInfo = lenInfoColl.GetIndex(i, j);
                    string lenType = (Singleton.Instance.OtherData.ChosenAllowOverLevel ? lenInfo.LengthType : lenInfo.LengthType2).ToString();
                    string lenPos = (Singleton.Instance.OtherData.ChosenAllowOverLevel ? lenInfo.LengthPosition : lenInfo.LengthPosition2).ToString();
                    CreateRebar(lenInfo.DiameterName, pnt, lenInfo.Start, lenInfo.End, lenType, lenPos);
                }
            }

        }
        public static Rebar CreateRebar(string rebarName, UV pnt, double start, double end, string type, string position)
        {
            start = start * ConstantValue.milimeter2feet;
            end = end * ConstantValue.milimeter2feet;
            Rebar rb = Rebar.CreateFromCurves(Singleton.Instance.RevitData.Document, RebarStyle.Standard, Singleton.Instance.WPFData.SelectedRebarType, null, null, 
                Singleton.Instance.RevitData.Element, XYZ.BasisY, new List<Curve> { Line.CreateBound(new XYZ(pnt.U, pnt.V, start), new XYZ(pnt.U, pnt.V, end)) }, 
                RebarHookOrientation.Left, RebarHookOrientation.Left, true, false);
            rb.SetUnobscuredInView(Singleton.Instance.RevitData.Document.ActiveView, true);
            rb.LookupParameter("Comments").Set($"AllowOverLevel:{Singleton.Instance.WPFData.AllowOverLevel}");
            rb.LookupParameter("Type").Set(type);
            rb.LookupParameter("Position").Set(position);
            return rb;
        }
    }
}
