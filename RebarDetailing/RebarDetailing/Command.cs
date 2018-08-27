#region Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Autodesk.Revit.DB.Structure;
using System.IO;
using System.Linq;
using System.Reflection;
using Geometry;
using Software_Key_Manager_Beta;
#endregion

namespace RebarDetailing
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        const string r = "Revit";
        const string Folder = "Library";
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet eles)
        {
            //String assemblyFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            //String assemplyDirPath = System.IO.Path.GetDirectoryName(assemblyFilePath);
            //MainForm validate = new MainForm();
            //validate.RegKeyPath = assemplyDirPath;

            //if (validate.checkFromRegFile() != MainForm.StatusKey.Matched)
            //{
            //    MainForm regForm = new MainForm();
            //    regForm.ShowDialog();
            //    return Result.Succeeded;
            //}

            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;
            var watch = System.Diagnostics.Stopwatch.StartNew();

            Transaction tx = new Transaction(doc, "RebarDetailing");
            tx.Start();

            List<Element> rbts = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_RebarTags).OfClass(typeof(FamilySymbol)).ToList();
            List<Element> rbtFs = new FilteredElementCollector(doc).OfClass(typeof(Family)).Where(x => ((Family)x).FamilyCategory.Id.IntegerValue == (int)BuiltInCategory.OST_RebarTags).ToList();
            ElementId eId = doc.GetDefaultFamilyTypeId(new ElementId(BuiltInCategory.OST_RebarTags));
            Element defaultSymbol = null;
            if (eId != ElementId.InvalidElementId)
            {
                defaultSymbol = doc.GetElement(eId);
            }

            List<string> symbolName = rbts.Select(x => x.Name).ToList();
            List<string> symbolFamilyName = rbts.Select(x => (x as FamilySymbol).FamilyName).ToList();
            List<string> familyName = rbtFs.Select(x => x.Name).ToList();

            ReinforcementSettings rs = new FilteredElementCollector(doc).OfClass(typeof(ReinforcementSettings)).Cast<ReinforcementSettings>().First();
            RebarRoundingManager rrm = rs.GetRebarRoundingManager();

            System.Windows.Forms.Screen screen = System.Windows.Forms.Screen.FromPoint(System.Windows.Forms.Cursor.Position);
            System.Drawing.Rectangle rec = screen.WorkingArea;
            System.Windows.Window window = new System.Windows.Window();
            window.WindowStartupLocation = System.Windows.WindowStartupLocation.Manual;
            window.Height = 260; window.Width = 240;
            window.Title = "Input Offset";
            window.ResizeMode = System.Windows.ResizeMode.NoResize;
            window.Topmost = true;
            //window.Left = System.Windows.SystemParameters.WorkArea.Right - window.Width - 400;
            //window.Top = System.Windows.SystemParameters.WorkArea.Top + 200;
            window.Left = rec.Right - window.Width - 400;
            window.Top = rec.Top + 200;

            UserControl1 formMaster = new UserControl1(rbtFs, rbts, defaultSymbol,window);
            window.Content = formMaster;
            if (formMaster.isShowAgain)
            {
                window.Show();
            }

            int count = 0;
            while (true)
            {
                List<Reference> rfs = null;
                try
                {
                    Loop:
                    bool MultiPick = formMaster.isMultiPick;
                    while (!MultiPick)
                    {
                        Reference rf = sel.PickObject(ObjectType.Element, new RebarSelection());
                        rfs = new List<Reference>() { rf };
                        break;
                    }
                    while (MultiPick)
                    {
                        List<Element> elems = sel.PickElementsByRectangle(new RebarSelection()) as List<Element>;
                        rfs = elems.Select(x => new Reference(x)).ToList();
                        break;
                    }
                    if (MultiPick != formMaster.isMultiPick) goto Loop;
                }
                catch (Autodesk.Revit.Exceptions.OperationCanceledException ex)
                {
                    break;
                }
                foreach (Reference rf in rfs)
                {
                    bool isCancel;
                    RebarDetailing(app, doc, sel, rf, formMaster, out isCancel, rrm);
                    if (isCancel) goto Escape;
                }
                count += rfs.Count;
            }
            Escape:
            window.Close();

            doc.SetDefaultFamilyTypeId(new ElementId(BuiltInCategory.OST_RebarTags), formMaster.selectedRebarTagType.Id);
            tx.Commit();
            //watch.Stop();
            //int saving_times = (int)(20000 * count - watch.ElapsedMilliseconds) / 1000;
            //validate.insertRecord2DB(System.Reflection.Assembly.GetExecutingAssembly().GetName().Name, saving_times);
            return Result.Succeeded;
        }
        public void RebarDetailing(Application app, Document doc, Selection sel, Reference rf, UserControl1 form, out bool isCancel, RebarRoundingManager rrm)
        {
            isCancel = false;
            Rebar rb = doc.GetElement(rf) as Rebar;
#if Debug2017
            RebarShape rbShape = doc.GetElement(rb.RebarShapeId) as RebarShape;
#elif Debug2018
            RebarShapeDrivenAccessor rsda = rb.GetShapeDrivenAccessor();
            RebarShape rbShape = doc.GetElement(rb.GetShapeId()) as RebarShape;
#elif Debug2019
            RebarShapeDrivenAccessor rsda = rb.GetShapeDrivenAccessor();
            RebarShape rbShape = doc.GetElement(rb.GetShapeId()) as RebarShape;
#endif
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
            double roundingNum = rrm.ApplicableSegmentLengthRounding;
            if (GeomUtil.IsEqual(roundingNum, 0)) roundingNum = 1;
            for (int i = 0; i < paraVals.Count; i++)
            {
                if (rrm.ApplicableSegmentLengthRoundingMethod == RoundingMethod.Nearest)
                {
                    paraVals[i] = Math.Round(paraVals[i] / roundingNum) * roundingNum;
                }
                else if (rrm.ApplicableSegmentLengthRoundingMethod == RoundingMethod.Up)
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
#if Debug2017
                double arrLen = rb.ArrayLength;
#elif Debug2018
                double arrLen = rsda.ArrayLength;
#elif Debug2019
                double arrLen = rsda.ArrayLength;
#endif
                spacRb = arrLen / (numRb - 1);
            }
            List<Curve> cs = rb.GetCenterlineCurves(false, false, false, MultiplanarOption.IncludeOnlyPlanarCurves, 0) as List<Curve>;
#if Debug2017
            XYZ normal = rb.Normal;
#elif Debug2018
            XYZ normal = rsda.Normal;
#elif Debug2019
            XYZ normal = rsda.Normal;
#endif
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

            Plane plane = Plane.CreateByNormalAndOrigin(doc.ActiveView.ViewDirection, doc.ActiveView.Origin);
            if (doc.ActiveView.SketchPlane == null)
            {
                doc.ActiveView.SketchPlane = SketchPlane.Create(doc, plane);
            }
            doc.ActiveView.HideActiveWorkPlane();
            int viewScale = doc.ActiveView.Scale;

#region Create FamilyDocument
            string rdFolderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string rftFilePath = Path.Combine(rdFolderPath, Folder, "Metric Detail Item.rft");
            Document famDoc = app.NewFamilyDocument(rftFilePath);
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
                XYZ vecX = doc.ActiveView.RightDirection;
                XYZ vecY = doc.ActiveView.UpDirection;
                double dotViewZ = XYZ.BasisZ.DotProduct(doc.ActiveView.ViewDirection);
                //View song song với mặt phẳng ngang
                if (GeomUtil.IsEqual(dotViewZ, 1) || GeomUtil.IsEqual(dotViewZ, -1))
                {
                    double angle = GeomUtil.GetAngle(doc.ActiveView.RightDirection, XYZ.BasisX, XYZ.BasisY);
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

                XYZ vecX = doc.ActiveView.RightDirection;
                XYZ vecY = doc.ActiveView.UpDirection;
                double dotViewZ = XYZ.BasisZ.DotProduct(doc.ActiveView.ViewDirection);
                //View song song với mặt phẳng ngang
                if (GeomUtil.IsEqual(dotViewZ, 1) || GeomUtil.IsEqual(dotViewZ, -1))
                {
                    double angle = GeomUtil.GetAngle(doc.ActiveView.RightDirection, XYZ.BasisX, XYZ.BasisY);
                    rotate = Transform.CreateRotation(XYZ.BasisZ, -angle) * rotate;
                }
                //View song song với mặt phẳng đứng
                else if (GeomUtil.IsEqual(dotViewZ, 0))
                {
                    double rebarAngle = GeomUtil.GetAngle(rebarVecY, XYZ.BasisX, XYZ.BasisY);
                    double viewAngle = GeomUtil.GetAngle(doc.ActiveView.RightDirection, XYZ.BasisX, XYZ.BasisY);
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

                XYZ vecX = doc.ActiveView.RightDirection;
                XYZ vecY = doc.ActiveView.UpDirection;
                double dotViewZ = XYZ.BasisZ.DotProduct(doc.ActiveView.ViewDirection);
                //View song song với mặt phẳng ngang
                if (GeomUtil.IsEqual(dotViewZ, 1) || GeomUtil.IsEqual(dotViewZ, -1))
                {
                    double angle = GeomUtil.GetAngle(doc.ActiveView.RightDirection, XYZ.BasisX, XYZ.BasisY);
                    rotate = Transform.CreateRotation(XYZ.BasisZ, -angle) * rotate;
                }
                //View song song với mặt phẳng đứng
                else if (GeomUtil.IsEqual(dotViewZ, 0))
                {
                    double rebarAngle = GeomUtil.GetAngle(rebarVecY, XYZ.BasisX, XYZ.BasisY);
                    double viewAngle = GeomUtil.GetAngle(doc.ActiveView.RightDirection, XYZ.BasisX, XYZ.BasisY);
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
            famDoc.LoadFamily(Path.Combine(rdFolderPath, Folder, "LengthTag.rfa"), out lenTextFamily);
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

                    if (form.isHaveSegment)
                    {
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
                        FamilyInstance fi1 = factory.NewFamilyInstance(midLine + famView.UpDirection*(viewScale / offset)*position, lenTextSym, famView);
                        ElementTransformUtils.RotateElement(famDoc, fi1.Id, Line.CreateBound(midLine, midLine + famView.ViewDirection), ang);
                        fi1.LookupParameter("Length").Set(paraVals[j].ToString());
                        j++;
                    }

                }
                catch (Exception ex)
                {
                    TaskDialog.Show(r, ex.Message);
                }
            }
            subTx.Commit();

#region Save Family Document
            SaveAsOptions opt = new SaveAsOptions();
            opt.OverwriteExistingFile = true;
            string famName = "RebarId_" + rb.Id + "_ViewId_" + doc.ActiveView.Id;
            famDoc.SaveAs(Path.Combine(Path.GetTempPath(), famName + ".rfa"), opt);
#endregion

            Family f = null;
            List<Family> fs = new FilteredElementCollector(doc).OfClass(typeof(Family)).Where(x => x.Name == famName).Cast<Family>().ToList();
            if (fs.Count == 0)
            {
                doc.LoadFamily(famDoc.PathName, out f);
            }
            else
            {
                doc.LoadFamily(famDoc.PathName, new LoadFamilyOption(), out f);
            }
            famDoc.Close(true);

            List<FamilySymbol> syms = f.GetFamilySymbolIds().Select(x => doc.GetElement(x) as FamilySymbol).ToList();
            FamilySymbol sym = syms.First();
            if (!sym.IsActive) sym.Activate();
            List<FamilyInstance> fis = new FilteredElementCollector(doc, doc.ActiveView.Id).OfClass(typeof(FamilyInstance)).Cast<FamilyInstance>()
                .Where(x => x.Symbol.FamilyName == f.Name).ToList();
            LocationPoint lp = null;

            FamilyInstance fi = null;
            bool isExisted = false;
            if (fis.Count == 0)
            {
                if (form.isMultiPick)
                {
                    XYZ pjMidPnt = CheckGeometry.GetProjectPoint(plane, midPnt);
                    fi = doc.Create.NewFamilyInstance(pjMidPnt, sym, doc.ActiveView);
                }
                else
                {
                    try
                    {
                        fi = doc.Create.NewFamilyInstance(sel.PickPoint(), sym, doc.ActiveView);
                    }
                    catch (Autodesk.Revit.Exceptions.OperationCanceledException ex)
                    {
                        isCancel = true;
                        return;
                    }
                }
                doc.Regenerate();
                lp = fi.Location as LocationPoint;
            }
            else
            {
                fi = fis.First();
                lp = fi.Location as LocationPoint;
                isExisted = true;
            }

            double dotViewY = XYZ.BasisZ.DotProduct(doc.ActiveView.UpDirection);
            if (GeomUtil.IsEqual(dotViewY, 1) || GeomUtil.IsEqual(dotViewY, -1))
            {
#region View đứng
                XYZ locPnt = (lp as LocationPoint).Point;
                Plane pl = Plane.CreateByOriginAndBasis(doc.ActiveView.Origin, doc.ActiveView.RightDirection, doc.ActiveView.UpDirection);

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
                        if (GeomUtil.IsEqual(dotNorm, 0) && !isExisted) editLen = GeomUtil.milimeter2Feet(form.len);
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
                catch (Exception ex)
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
                        Element e = doc.GetElement(elemId);
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

            //TaskDialog.Show(r, form.haveTag.ToString());
            if (form.haveTag && form.isHaveRebarTag)
            {
                double angleMainVec = mainVec.AngleTo(doc.ActiveView.RightDirection);
                angleMainVec = Math.Abs(angleMainVec) > Math.PI / 2 ? Math.PI - Math.Abs(angleMainVec) : Math.Abs(angleMainVec);
                XYZ insPnt = null;
                if (0 <= angleMainVec && angleMainVec <= Math.PI / 4)
                {
                    insPnt = (lp as LocationPoint).Point;
                    if (!isHaveTag)
                    {
#if Debug2017
                        tag = doc.Create.NewTag(doc.ActiveView, rb, false, TagMode.TM_ADDBY_CATEGORY, TagOrientation.Horizontal, insPnt);
#elif Debug2018
                        tag = IndependentTag.Create(doc, doc.ActiveView.Id, rf, false, TagMode.TM_ADDBY_CATEGORY,TagOrientation.Horizontal, insPnt);
#elif Debug2019
                        tag = IndependentTag.Create(doc, doc.ActiveView.Id, rf, false, TagMode.TM_ADDBY_CATEGORY, TagOrientation.Horizontal, insPnt);
#endif
                    }
                    insPnt += -doc.ActiveView.UpDirection / 150 * viewScale;
                    tag.TagHeadPosition = insPnt;
                    tag.TagOrientation = TagOrientation.Horizontal;
                }
                else if (Math.PI / 4 <= angleMainVec && angleMainVec <= Math.PI / 2)
                {
                    insPnt = (lp as LocationPoint).Point;
                    if (!isHaveTag)
                    {
#if Debug2017
                        tag = doc.Create.NewTag(doc.ActiveView, rb, false, TagMode.TM_ADDBY_CATEGORY, TagOrientation.Vertical, insPnt);
#elif Debug2018
                        tag = IndependentTag.Create(doc, doc.ActiveView.Id, rf, false, TagMode.TM_ADDBY_CATEGORY,TagOrientation.Vertical, insPnt);
#elif Debug2019
                        tag = IndependentTag.Create(doc, doc.ActiveView.Id, rf, false, TagMode.TM_ADDBY_CATEGORY, TagOrientation.Vertical, insPnt);
#endif
                    }
                    insPnt += doc.ActiveView.RightDirection / 150 * viewScale;
                    tag.TagHeadPosition = insPnt;
                    tag.TagOrientation = TagOrientation.Vertical;
                }
                else
                {
                    throw new Exception("The code should not go here!");
                }
                fi.LookupParameter("Comments").Set(tag.Id.IntegerValue.ToString());
                tag.ChangeTypeId(form.selectedRebarTagType.Id);
            }
            else
            {
                if (isHaveTag)
                {
                    doc.Delete(tag.Id);
                }
                fi.LookupParameter("Comments").Set("0");
            }

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
                catch (IOException ex)
                {

                }
            }
        }
    }
    class LoadFamilyOption : IFamilyLoadOptions
    {
        public bool OnFamilyFound(bool familyInUse, out bool overwriteParameterValues)
        {
            overwriteParameterValues = true;
            return true;
        }
        public bool OnSharedFamilyFound(Family sharedFamily, bool familyInUse, out FamilySource source, out bool overwriteParameterValues)
        {
            source = FamilySource.Family;
            overwriteParameterValues = true;
            return true;
        }
    }
    [Transaction(TransactionMode.Manual)]
    public class Command1 : IExternalCommand
    {
        const string r = "Revit";
        const string Folder = "RebarDetailing";
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet eles)
        {
            String assemblyFilePath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            String assemplyDirPath = System.IO.Path.GetDirectoryName(assemblyFilePath);
            MainForm validate = new MainForm();
            validate.RegKeyPath = assemplyDirPath;
            if (validate.checkFromRegFile() != MainForm.StatusKey.Matched)
            {
                MainForm regForm = new MainForm();
                regForm.ShowDialog();
                return Result.Succeeded;
            }

            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;
            Transaction tx = new Transaction(doc, "RebarDetailing");
            tx.Start();

            List<Element> rbts = new FilteredElementCollector(doc).OfCategory(BuiltInCategory.OST_RebarTags).OfClass(typeof(FamilySymbol)).ToList();
            List<Element> rbtFs = new FilteredElementCollector(doc).OfClass(typeof(Family)).Where(x => ((Family)x).FamilyCategory.Id.IntegerValue == (int)BuiltInCategory.OST_RebarTags).ToList();
            ElementId eId = doc.GetDefaultFamilyTypeId(new ElementId(BuiltInCategory.OST_RebarTags));
            Element defaultSymbol = null;
            if (eId != ElementId.InvalidElementId)
            {
                defaultSymbol = doc.GetElement(eId);
            }

            List<string> symbolName = rbts.Select(x => x.Name).ToList();
            List<string> symbolFamilyName = rbts.Select(x => (x as FamilySymbol).FamilyName).ToList();
            List<string> familyName = rbtFs.Select(x => x.Name).ToList();

            System.Windows.Forms.Screen screen = System.Windows.Forms.Screen.FromPoint(System.Windows.Forms.Cursor.Position);
            System.Drawing.Rectangle rec = screen.WorkingArea;
            System.Windows.Window window = new System.Windows.Window();
            window.WindowStartupLocation = System.Windows.WindowStartupLocation.Manual;
            window.Height = 260; window.Width = 240;
            window.Title = "Input Offset";
            window.ResizeMode = System.Windows.ResizeMode.NoResize;
            window.Topmost = true;
            //window.Left = System.Windows.SystemParameters.WorkArea.Right - window.Width - 400;
            //window.Top = System.Windows.SystemParameters.WorkArea.Top + 200;
            window.Left = rec.Right - window.Width - 400;
            window.Top = rec.Top + 200;

            UserControl1 formMaster = new UserControl1(rbtFs, rbts, defaultSymbol, window);
            window.Content = formMaster;
            window.ShowDialog();
            doc.SetDefaultFamilyTypeId(new ElementId(BuiltInCategory.OST_RebarTags), formMaster.selectedRebarTagType.Id);

            tx.Commit();
            return Result.Succeeded;
        }
    }
    [Transaction(TransactionMode.Manual)]
    public class Command2 : IExternalCommand
    {
        const string r = "Revit";
        const string Folder = "RebarDetailing";
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet eles)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;
            Selection sel = uidoc.Selection;
            Transaction tx = new Transaction(doc, "RebarDetailing");
            tx.Start();

            ReinforcementSettings rs = new FilteredElementCollector(doc).OfClass(typeof(ReinforcementSettings)).Cast<ReinforcementSettings>().First();
            RebarRoundingManager rrm = rs.GetRebarRoundingManager();

            tx.Commit();
            return Result.Succeeded;
        }
    }
}