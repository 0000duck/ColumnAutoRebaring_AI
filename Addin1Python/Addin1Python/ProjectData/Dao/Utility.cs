using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Geometry;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Addin1Python
{
    public static class Utility
    {
        public static Workset GetWorkset(string layer)
        {
            return Singleton.Instance.Worksets.Where(x => x.Name == $"{SingleWPF.Instance.Prefix}-ThepSan{layer}").First();
        }
        public static Workset GetWorkset()
        {
            var res = Singleton.Instance.Worksets.Where(x => x.Name == $"{SingleWPF.Instance.Prefix}-ThepSan{SingleWPF.Instance.Layer}");
            if (res.Count() != 0) return res.First();
            Workset ws= Workset.Create(Singleton.Instance.Document, $"{SingleWPF.Instance.Prefix}-ThepSan{SingleWPF.Instance.Layer}");
            Singleton.Instance.WorksetDefaultVisibilitySettings.SetWorksetVisibility(ws.Id, false);
            Singleton.Instance.Worksets.Add(ws);
            return ws;
        }

        public static void CreateRebarDetail(Reference rf)
        {
            Rebar rb = Singleton.Instance.Document.GetElement(rf) as Rebar;
            RebarShapeDrivenAccessor rsda = rb.GetShapeDrivenAccessor();
            RebarShape rbShape = Singleton.Instance.Document.GetElement(rb.GetShapeId()) as RebarShape;
            RebarShapeDefinitionBySegments defBySegment = rbShape.GetRebarShapeDefinition() as RebarShapeDefinitionBySegments;
            RebarStyle rbStyle = rbShape.RebarStyle;
            List<string> paraNames = new List<string>();
            for (int i = 0; i < defBySegment.NumberOfSegments; i++)
            {
                RebarShapeSegment rss = defBySegment.GetSegment(i);
                List<RebarShapeConstraint> constraints = rss.GetConstraints() as List<RebarShapeConstraint>;
                foreach (RebarShapeConstraint rsc in constraints)
                {
                    if (!(rsc is RebarShapeConstraintSegmentLength))
                        continue;
                    ElementId paramId = rsc.GetParamId();
                    if ((paramId == ElementId.InvalidElementId))
                        continue;
                    foreach (Parameter p in rbShape.Parameters)
                    {
                        if (p.Id.IntegerValue == paramId.IntegerValue)
                        {
                            paraNames.Add(p.Definition.Name);
                            break;
                        }
                    }
                }
            }
            RebarBendData rbd = rb.GetBendData();
            List<double> paraVals = new List<double>();
            double hookLen = 0;
            if (rbd.HookAngle0 > 0)
            {
                hookLen = GeomUtil.feet2Milimeter(rb.get_Parameter(BuiltInParameter.REBAR_SHAPE_START_HOOK_LENGTH).AsDouble());
                paraVals.Add(hookLen);
            }

            paraNames.ForEach(x => paraVals.Add(Math.Round(GeomUtil.feet2Milimeter(rb.LookupParameter(x).AsDouble()))));
            if (rbd.HookAngle1 > 0)
            {
                paraVals.Add(hookLen);
            }
            double roundingNum = Singleton.Instance.RebarRoundingManager.ApplicableSegmentLengthRounding;
            if (GeomUtil.IsEqual(roundingNum, 0)) roundingNum = 1;
            for (int i = 0; i < paraVals.Count; i++)
            {
                if (Singleton.Instance.RebarRoundingManager.ApplicableSegmentLengthRoundingMethod == RoundingMethod.Nearest)
                {
                    paraVals[i] = Math.Round(paraVals[i] / roundingNum) * roundingNum;
                }
                else if (Singleton.Instance.RebarRoundingManager.ApplicableSegmentLengthRoundingMethod == RoundingMethod.Up)
                {
                    paraVals[i] = Math.Ceiling(paraVals[i] / roundingNum) * roundingNum;
                }
                else
                {
                    paraVals[i] = Math.Floor(paraVals[i] / roundingNum) * roundingNum;
                }
            }

            int numRb = rb.Quantity;
            double spacRb = 0;
            if (numRb != 1)
            {
                double arrLen = rsda.ArrayLength;
                spacRb = arrLen / (numRb - 1);
            }
            List<Curve> cs = rb.GetCenterlineCurves(false, false, false, MultiplanarOption.IncludeOnlyPlanarCurves, 0) as List<Curve>;
            XYZ normal = rsda.Normal;
            Transform fstTranslate = Transform.CreateTranslation(normal * spacRb * numRb / 2);
            cs = cs.Select(x => x.CreateTransformed(fstTranslate)).ToList();

            List<Curve> lines = cs.Where(x => x is Line).ToList();
            double maxLen = 0;
            XYZ mainVec = null;
            int midPntIndex = 0;
            XYZ midPnt = null;
            Curve tempLine = null;
            for (int i = 0; i < cs.Count; i++)
            {
                if (cs[i] is Arc) continue;
                if (maxLen > cs[i].Length) continue;
                maxLen = cs[i].Length;
                mainVec = (cs[i] as Line).Direction;
                midPntIndex = i;
                midPnt = (cs[i].GetEndPoint(0) + cs[i].GetEndPoint(1)) / 2;
                tempLine = Line.CreateBound(cs[i].GetEndPoint(0), midPnt);
            }
            BoundingBoxXYZ bb = rb.get_BoundingBox(null);
            XYZ maxPnt = bb.Max, minPnt = bb.Min;
            List<XYZ> pnts = cs.Select(x => x.GetEndPoint(0)).ToList();
            pnts.Add(cs[cs.Count - 1].GetEndPoint(1));

            Plane plane = Plane.CreateByNormalAndOrigin(Singleton.Instance.ActiveView.ViewDirection, Singleton.Instance.ActiveView.Origin);
            if (Singleton.Instance.ActiveView.SketchPlane == null)
            {
                Singleton.Instance.ActiveView.SketchPlane = SketchPlane.Create(Singleton.Instance.Document, plane);
            }
            Singleton.Instance.ActiveView.HideActiveWorkPlane();
            int viewScale = Singleton.Instance.ActiveView.Scale;

            #region Create FamilyDocument
            string rdFolderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string rftFilePath = Path.Combine(rdFolderPath, ConstantValue.Library, "Metric Detail Item.rft");
            Document famDoc = Singleton.Instance.Application.NewFamilyDocument(rftFilePath);
            if (famDoc == null)
            {
                throw new Exception("Cannot create family document");
            }
            #endregion

            Autodesk.Revit.Creation.FamilyItemFactory factory = famDoc.FamilyCreate;
            View famView = new FilteredElementCollector(famDoc).OfClass(typeof(View)).Cast<View>()
                .First(x => x.Name == "Ref. Level");

            #region Rotate Transform
            Transform rotate = Transform.Identity;
            double dotZ = normal.DotProduct(XYZ.BasisZ);
            //Vector phương rải thép song song với trục Z
            if (GeomUtil.IsEqual(dotZ, 1) || GeomUtil.IsEqual(dotZ, -1))
            {
                XYZ vecX = Singleton.Instance.ActiveView.RightDirection;
                XYZ vecY = Singleton.Instance.ActiveView.UpDirection;
                double dotViewZ = XYZ.BasisZ.DotProduct(Singleton.Instance.ActiveView.ViewDirection);
                //View song song với mặt phẳng ngang
                if (GeomUtil.IsEqual(dotViewZ, 1) || GeomUtil.IsEqual(dotViewZ, -1))
                {
                    double angle = GeomUtil.GetAngle(Singleton.Instance.ActiveView.RightDirection, XYZ.BasisX, XYZ.BasisY);
                    rotate = Transform.CreateRotation(XYZ.BasisZ, -angle) * rotate;
                }
                //View song song với mặt phẳng đứng
                else if (GeomUtil.IsEqual(dotViewZ, 0))
                {
                    double angle = GeomUtil.GetAngle(mainVec, XYZ.BasisX, XYZ.BasisY);
                    rotate = Transform.CreateRotation(XYZ.BasisZ, -angle) * rotate;
                }
                //View nghiêng
                else
                {
                    throw new Exception("This case have not checked yet!");
                }
            }
            //Vector phương rải thép vuông góc với trục Z
            else if (GeomUtil.IsEqual(dotZ, 0))
            {
                XYZ rebarVecY = XYZ.BasisZ.CrossProduct(normal);
                rebarVecY = GeomUtil.GetPositionVector(rebarVecY, XYZ.BasisX, XYZ.BasisY);
                rotate = Transform.CreateRotation(rebarVecY, -Math.PI / 2) * rotate;

                XYZ vecX = Singleton.Instance.ActiveView.RightDirection;
                XYZ vecY = Singleton.Instance.ActiveView.UpDirection;
                double dotViewZ = XYZ.BasisZ.DotProduct(Singleton.Instance.ActiveView.ViewDirection);
                //View song song với mặt phẳng ngang
                if (GeomUtil.IsEqual(dotViewZ, 1) || GeomUtil.IsEqual(dotViewZ, -1))
                {
                    double angle = GeomUtil.GetAngle(Singleton.Instance.ActiveView.RightDirection, XYZ.BasisX, XYZ.BasisY);
                    rotate = Transform.CreateRotation(XYZ.BasisZ, -angle) * rotate;
                }
                //View song song với mặt phẳng đứng
                else if (GeomUtil.IsEqual(dotViewZ, 0))
                {
                    double rebarAngle = GeomUtil.GetAngle(rebarVecY, XYZ.BasisX, XYZ.BasisY);
                    double viewAngle = GeomUtil.GetAngle(Singleton.Instance.ActiveView.RightDirection, XYZ.BasisX, XYZ.BasisY);
                    double relAngle = Math.Abs(rebarAngle - viewAngle);

                    rotate = Transform.CreateRotation(XYZ.BasisZ, -rebarAngle) * rotate;
                    if (GeomUtil.IsEqual(relAngle, Math.PI / 2) || relAngle < Math.PI / 2)
                    {
                        //rebarAngle += Math.PI;
                    }
                    else
                    {
                        rotate = Transform.CreateRotation(XYZ.BasisY, Math.PI) * rotate;
                    }
                }
                //View nghiêng
                else
                {
                    throw new Exception("This case have not checked yet!");
                }
            }
            else
            {
                //Get angle of Normal vector to Oxy
                if (GeomUtil.IsSmaller(normal, -normal))
                {
                    normal *= -1;
                }
                XYZ rebarVecY = XYZ.BasisZ.CrossProduct(normal);
                double normAngle = GeomUtil.GetAngle2(normal, XYZ.BasisX, XYZ.BasisY, XYZ.BasisZ);
                rotate = Transform.CreateRotation(rebarVecY, normAngle) * rotate;

                rebarVecY = CheckGeometry.GetProjectPoint(Plane.CreateByOriginAndBasis(XYZ.Zero, XYZ.BasisX, XYZ.BasisY), rebarVecY);
                rebarVecY = GeomUtil.GetPositionVector(rebarVecY, XYZ.BasisX, XYZ.BasisY);
                rotate = Transform.CreateRotation(rebarVecY, -Math.PI / 2) * rotate;

                XYZ vecX = Singleton.Instance.ActiveView.RightDirection;
                XYZ vecY = Singleton.Instance.ActiveView.UpDirection;
                double dotViewZ = XYZ.BasisZ.DotProduct(Singleton.Instance.ActiveView.ViewDirection);
                //View song song với mặt phẳng ngang
                if (GeomUtil.IsEqual(dotViewZ, 1) || GeomUtil.IsEqual(dotViewZ, -1))
                {
                    double angle = GeomUtil.GetAngle(Singleton.Instance.ActiveView.RightDirection, XYZ.BasisX, XYZ.BasisY);
                    rotate = Transform.CreateRotation(XYZ.BasisZ, -angle) * rotate;
                }
                //View song song với mặt phẳng đứng
                else if (GeomUtil.IsEqual(dotViewZ, 0))
                {
                    double rebarAngle = GeomUtil.GetAngle(rebarVecY, XYZ.BasisX, XYZ.BasisY);
                    double viewAngle = GeomUtil.GetAngle(Singleton.Instance.ActiveView.RightDirection, XYZ.BasisX, XYZ.BasisY);
                    double relAngle = Math.Abs(rebarAngle - viewAngle);

                    rotate = Transform.CreateRotation(XYZ.BasisZ, -rebarAngle) * rotate;
                    if (GeomUtil.IsEqual(relAngle, Math.PI / 2) || relAngle < Math.PI / 2)
                    {
                        //rebarAngle += Math.PI;
                    }
                    else
                    {
                        rotate = Transform.CreateRotation(XYZ.BasisY, Math.PI) * rotate;
                    }
                }
                //View nghiêng
                else
                {
                    throw new Exception("This case have not checked yet!");
                }
            }
            #endregion
            tempLine = tempLine.CreateTransformed(rotate);
            Transform translate = Transform.CreateTranslation(famView.Origin - tempLine.GetEndPoint(1));
            List<Curve> tfCs = cs.Select(x => x.CreateTransformed(translate * rotate)).ToList();

            Transaction subTx = new Transaction(famDoc, "RebarDetailing");
            subTx.Start();

            Family lenTextFamily = null;
            famDoc.LoadFamily(Path.Combine(rdFolderPath, ConstantValue.Library, "LengthTag.rfa"), out lenTextFamily);
            FamilySymbol lenTextSym = lenTextFamily.GetFamilySymbolIds().Select(x => famDoc.GetElement(x) as FamilySymbol).First();

            Transform fvTranform = Transform.Identity;
            fvTranform.BasisX = famView.RightDirection;
            fvTranform.BasisY = famView.UpDirection;
            fvTranform.BasisZ = famView.ViewDirection;
            fvTranform.Origin = famView.Origin;

            int j = 0;
            for (int i = 0; i < tfCs.Count; i++)
            {
                try
                {
                    factory.NewDetailCurve(famView, tfCs[i]);
                    if (tfCs[i] is Arc) continue;

                    #region Get Insert Text Point
                    int position = 1;
                    double offset = 0;
                    XYZ midLine = (tfCs[i].GetEndPoint(0) + tfCs[i].GetEndPoint(1)) / 2;
                    XYZ vecLine = (tfCs[i] as Line).Direction;
                    XYZ vecY = null;
                    double ang = GeomUtil.GetAngle(vecLine, XYZ.BasisX, XYZ.BasisY);

                    if (GeomUtil.IsEqual(Math.Abs(ang), Math.PI / 2))
                    {
                        ang = Math.PI / 2;
                    }
                    else if (Math.Abs(ang) < Math.PI / 2)
                    {
                    }
                    else
                    {
                        if (ang > 0)
                        {
                            ang -= Math.PI;
                        }
                        else
                        {
                            ang = Math.PI + ang;
                        }
                    }
                    if (GeomUtil.IsEqual(ang, Math.PI / 4) || ang < Math.PI / 4)
                    {
                        offset = 200;
                        if (GeomUtil.IsEqual(midLine.Y, 0))
                        { }
                        else if (midLine.Y < 0)
                        {
                            position = -1;
                        }
                    }
                    else
                    {
                        offset = 200;
                        if (GeomUtil.IsEqual(midLine.X, 0))
                        {

                        }
                        else if (midLine.X > 0)
                        {
                            position = -1;
                        }
                    }

                    #endregion
                    FamilyInstance fi1 = factory.NewFamilyInstance(midLine + famView.UpDirection * (viewScale / offset) * position, lenTextSym, famView);
                    ElementTransformUtils.RotateElement(famDoc, fi1.Id, Line.CreateBound(midLine, midLine + famView.ViewDirection), ang);
                    fi1.LookupParameter("Length").Set(paraVals[j].ToString());
                    j++;

                }
                catch (Exception ex)
                {
                    throw;
                }
            }
            subTx.Commit();

            #region Save Family Document
            SaveAsOptions opt = new SaveAsOptions();
            opt.OverwriteExistingFile = true;
            string famName = "RebarId_" + rb.Id + "_ViewId_" + Singleton.Instance.ActiveView.Id;
            famDoc.SaveAs(Path.Combine(Path.GetTempPath(), famName + ".rfa"), opt);
            #endregion

            Family f = null;
            List<Family> fs = new FilteredElementCollector(Singleton.Instance.Document).OfClass(typeof(Family)).Where(x => x.Name == famName).Cast<Family>().ToList();
            if (fs.Count == 0)
            {
                Singleton.Instance.Document.LoadFamily(famDoc.PathName, out f);
            }
            else
            {
                Singleton.Instance.Document.LoadFamily(famDoc.PathName, new LoadFamilyOption(), out f);
            }
            famDoc.Close(true);

            List<FamilySymbol> syms = f.GetFamilySymbolIds().Select(x => Singleton.Instance.Document.GetElement(x) as FamilySymbol).ToList();
            FamilySymbol sym = syms.First();
            if (!sym.IsActive) sym.Activate();
            List<FamilyInstance> fis = new FilteredElementCollector(Singleton.Instance.Document, Singleton.Instance.ActiveView.Id).OfClass(typeof(FamilyInstance)).Cast<FamilyInstance>()
                .Where(x => x.Symbol.FamilyName == f.Name).ToList();
            LocationPoint lp = null;

            FamilyInstance fi = null;
            bool isExisted = false;
            if (fis.Count == 0)
            {

                fi = Singleton.Instance.Document.Create.NewFamilyInstance(Singleton.Instance.Selection.PickPoint(), sym, Singleton.Instance.ActiveView);
                Singleton.Instance.Document.Regenerate();
                lp = fi.Location as LocationPoint;
            }
            else
            {
                fi = fis.First();
                lp = fi.Location as LocationPoint;
                isExisted = true;
            }

            double dotViewY = XYZ.BasisZ.DotProduct(Singleton.Instance.ActiveView.UpDirection);
            if (GeomUtil.IsEqual(dotViewY, 1) || GeomUtil.IsEqual(dotViewY, -1))
            {
                #region View đứng
                XYZ locPnt = (lp as LocationPoint).Point;
                Plane pl = Plane.CreateByOriginAndBasis(Singleton.Instance.ActiveView.Origin, Singleton.Instance.ActiveView.RightDirection, Singleton.Instance.ActiveView.UpDirection);

                double maxU = 0, minU = 0, maxV = 0, minV = 0;
                bool isSet = false;
                foreach (XYZ pnt in pnts)
                {
                    XYZ pjPnt = CheckGeometry.GetProjectPoint(pl, pnt);
                    UV pjUV = GeomUtil.GetUVCoordinate(pjPnt - pl.Origin, pl.XVec, pl.YVec);
                    if (!isSet)
                    {
                        isSet = true;
                        maxU = pjUV.U; minU = pjUV.U;
                        maxV = pjUV.V; minV = pjUV.V;
                        continue;
                    }
                    if (maxU < pjUV.U) maxU = pjUV.U;
                    if (minU > pjUV.U) minU = pjUV.U;
                    if (maxV < pjUV.V) maxV = pjUV.V;
                    if (minV > pjUV.V) minV = pjUV.V;
                }
                XYZ pjLocPnt = CheckGeometry.GetProjectPoint(pl, locPnt);
                XYZ pjMidPnt = CheckGeometry.GetProjectPoint(pl, midPnt);
                UV pjLocUV = GeomUtil.GetUVCoordinate(pjLocPnt - pl.Origin, pl.XVec, pl.YVec);
                UV pjMidUV = GeomUtil.GetUVCoordinate(pjMidPnt - pl.Origin, pl.XVec, pl.YVec);

                UV pjMaxUV = new UV(maxU, maxV); UV pjMinUV = new UV(minU, minV);

                UV transUV = UV.Zero;
                if (pjMinUV.V < pjLocUV.V && pjLocUV.V < pjMaxUV.V)
                {
                    transUV += new UV(0, pjMidUV.V - pjLocUV.V);
                }
                if (pjMinUV.U < pjLocUV.U && pjLocUV.U < pjMaxUV.U)
                {
                    transUV += new UV(pjMidUV.U - pjLocUV.U, 0);
                }
                XYZ transVec = pl.XVec * transUV.U + pl.YVec * transUV.V;
                lp.Move(transVec);
                #endregion
            }
            else if (GeomUtil.IsEqual(dotViewY, 0))
            {
                if (rbStyle == RebarStyle.Standard)
                {
                    double dotNorm = normal.DotProduct(XYZ.BasisZ);
                    if (GeomUtil.IsEqual(dotNorm, 0) || GeomUtil.IsEqual(dotNorm, 1) || GeomUtil.IsEqual(dotNorm, -1))
                    {
                        double editLen = 0;
                        if (GeomUtil.IsEqual(dotNorm, 0) && !isExisted) editLen = GeomUtil.milimeter2Feet(0);
                        XYZ vecX = null, vecY = null;
                        if (!GeomUtil.IsEqual(mainVec.DotProduct(XYZ.BasisZ), 0))
                        {
                            vecY = normal.Normalize();
                            if (GeomUtil.IsSameOrOppositeDirection(vecY, XYZ.BasisX))
                            {
                                vecY = XYZ.BasisX;
                            }
                            else
                            {
                                vecY = vecY.Y > -vecY.Y ? vecY : -vecY;
                            }
                            vecX = vecY.CrossProduct(XYZ.BasisZ);
                        }
                        else
                        {
                            vecX = mainVec.Normalize();
                            if (GeomUtil.IsSameOrOppositeDirection(vecX, XYZ.BasisY))
                            {
                                vecX = -XYZ.BasisY;
                            }
                            else
                            {
                                vecX = vecX.X > -vecX.X ? vecX : -vecX;
                            }
                            vecY = XYZ.BasisZ.CrossProduct(vecX);
                        }

                        #region View ngang
                        XYZ locPnt = (lp as LocationPoint).Point;
                        Plane pl = Plane.CreateByOriginAndBasis(XYZ.Zero, vecX, vecY);
                        XYZ pjLocPnt = CheckGeometry.GetProjectPoint(pl, locPnt);
                        XYZ pjMidPnt = CheckGeometry.GetProjectPoint(pl, midPnt);
                        UV pjLocUV = GeomUtil.GetUVCoordinate(pjLocPnt, pl.XVec, pl.YVec);
                        UV pjMidUV = GeomUtil.GetUVCoordinate(pjMidPnt, pl.XVec, pl.YVec);
                        UV transUV = new UV(pjMidUV.U - pjLocUV.U, editLen);
                        XYZ transVec = pl.XVec * transUV.U + pl.YVec * transUV.V;
                        lp.Move(transVec);
                        #endregion
                    }
                }
            }
            else
            {
                throw new Exception("This case have not checked yet!");
            }

            IndependentTag tag = null;
            bool isHaveTag = false;
            if (isExisted)
            {
                string mark = "0";
                try
                {
                    mark = fi.LookupParameter("Comments").AsString();
                }
                catch (Exception)
                {
                }
                if (mark == "0")
                {
                }
                else
                {
                    int d = 0;
                    if (int.TryParse(mark, out d))
                    {
                        ElementId elemId = new ElementId(d);
                        Element e = Singleton.Instance.Document.GetElement(elemId);
                        if (e != null)
                        {
                            if (e is IndependentTag)
                            {
                                tag = e as IndependentTag;
                                isHaveTag = true;
                            }
                        }
                    }
                }
            }

            double angleMainVec = mainVec.AngleTo(Singleton.Instance.ActiveView.RightDirection);
            angleMainVec = Math.Abs(angleMainVec) > Math.PI / 2 ? Math.PI - Math.Abs(angleMainVec) : Math.Abs(angleMainVec);
            XYZ insPnt = null;
            if (0 <= angleMainVec && angleMainVec <= Math.PI / 4)
            {
                insPnt = (lp as LocationPoint).Point;
                if (!isHaveTag)
                {
                    tag = IndependentTag.Create(Singleton.Instance.Document, Singleton.Instance.ActiveView.Id, rf, false, TagMode.TM_ADDBY_CATEGORY, TagOrientation.Horizontal, insPnt);
                }
                insPnt += Singleton.Instance.ActiveView.UpDirection / 150 * viewScale;
                tag.TagHeadPosition = insPnt;
                tag.TagOrientation = TagOrientation.Horizontal;
            }
            else if (Math.PI / 4 <= angleMainVec && angleMainVec <= Math.PI / 2)
            {
                insPnt = (lp as LocationPoint).Point;
                if (!isHaveTag)
                {
                    tag = IndependentTag.Create(Singleton.Instance.Document, Singleton.Instance.ActiveView.Id, rf, false, TagMode.TM_ADDBY_CATEGORY, TagOrientation.Vertical, insPnt);
                }
                insPnt += Singleton.Instance.ActiveView.RightDirection / 150 * viewScale;
                tag.TagHeadPosition = insPnt;
                tag.TagOrientation = TagOrientation.Vertical;
            }
            else
            {
                throw new Exception("The code should not go here!");
            }
            fi.LookupParameter("Comments").Set(tag.Id.IntegerValue.ToString());
            tag.ChangeTypeId(Singleton.Instance.SelectedRebarTagType.Id);

            for (int i = 0; i < 15; i++)
            {
                string pathName = string.Empty;
                if (i == 0)
                {
                    pathName = Path.Combine(Path.GetTempPath(), famName + ".rfa");
                }
                else
                {
                    pathName = Path.Combine(Path.GetTempPath(), famName + ".000" + i.ToString() + ".rfa");
                }
                try
                {
                    if (File.Exists(pathName))
                    {
                        File.Delete(pathName);
                    }
                }
                catch (IOException)
                {

                }
            }
        }
        public static Rebar CreateCircleRebar(List<Arc> arcs)
        {
            return Rebar.CreateFromCurves(Singleton.Instance.Document, RebarStyle.Standard, SingleWPF.Instance.SelectedRebarType, null, null, Singleton.Instance.SelectedElement,
                XYZ.BasisZ, arcs.Cast<Curve>().ToList(), RebarHookOrientation.Left, RebarHookOrientation.Right, true, true);
        }
        public static List<Rebar> CreateCentrifugalRebar(Rebar rb, CircleEquation ce)
        {
            int sum = ce.Number;
            List<int> nums = new List<int>();
            while (sum > 200)
            {
                nums.Add(200);
                sum -= 199;
            }
            nums.Add(sum);

            foreach (int num in nums)
            {
                var res = RadialArray.ArrayElementWithoutAssociation(Singleton.Instance.Document, Singleton.Instance.ActiveView, rb.Id, num,
                    Line.CreateBound(Singleton.Instance.SelectedXYZ, Singleton.Instance.SelectedXYZ + XYZ.BasisZ), ce.Angle, ArrayAnchorMember.Second);
                rb = Singleton.Instance.Document.GetElement(res.Last()) as Rebar;
            }
            
            return null;
        }
    }
}
