#region Namespaces
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
#endregion

namespace Geometry
{
    /// <summary>
    /// Kiểu dữ liệu định nghĩa tập hợp gồm một Polygon mẹ lồng các Polygon con bên trong
    /// Các Polygon con phải nằm hoàn toàn bên trong Polygon mẹ
    /// </summary>
    public class MultiPolygon
    {
        /// <summary>
        /// Trả về tập hợp các điểm là đỉnh của Polygon mẹ
        /// </summary>
        public List<XYZ> ListXYZPoint { get; private set; }

        /// <summary>
        /// Trả về tập hợp gồm 2 điểm tạo thành hình chữ nhật chứa MutiPolygon
        /// Cạnh của hình chữ nhật song song với 2 hệ trục địa phương của mặt phẳng chứa MultiPolygon
        /// </summary>
        public List<XYZ> TwoXYZPointsBoundary { get; private set; }

        /// <summary>
        /// Trả về tập hợp gồm 2 điểm tạo thành hình chữ nhật chứa MutiPolygon
        /// Là mô phỏng của 2 điểm tạo thành hình chữ nhật chứa MutliPolygon lên hệ trục địa phương của mặt phẳng chứa MultiPolygon
        /// </summary>
        public List<UV> TwoUVPointsBoundary { get; private set; }

        /// <summary>
        /// Trả về điểm trọng tâm của Polygon mẹ
        /// </summary>
        public XYZ CentralXYZPoint { get; private set; }

        /// <summary>
        /// Trả về trục Ox tự định nghĩa trên mặt phẳng chứa MultiPolygon
        /// </summary>
        public XYZ XVecManual { get; private set; }

        /// <summary>
        /// Trả về trục Oy tự định nghĩa trên mặt phẳng chứa MultiPolygon
        /// </summary>
        public XYZ YVecManual { get; private set; }

        /// <summary>
        /// Trả về vector pháp tuyến của mặt phẳng chứa MultiPolygon
        /// </summary>
        public XYZ Normal { get; private set; }

        /// <summary>
        /// Trả về mặt phẳng chứa MultiPolygon
        /// </summary>
        public Plane Plane { get; private set; }

        /// <summary>
        /// Trả về mặt phẳng có hệ trục tự định nghĩa chứa MultiPolygon
        /// </summary>
        public Plane PlaneManual { get; private set; }

        /// <summary>
        /// Trả về Polygon mẹ
        /// </summary>
        public Polygon SurfacePolygon { get; private set; }

        /// <summary>
        /// Trả về tập hợp các Polygon con
        /// Trả về null khi không có Polygon con nào
        /// </summary>
        public List<Polygon> OpeningPolygons { get; private set; }

        /// <summary>
        /// Hàm khởi tạo MultiPolygon từ mặt đang xét
        /// </summary>
        /// <param name="f">Mặt đang xét</param>
        public MultiPolygon(PlanarFace f)
        {
            List<Polygon> pls = new List<Polygon>();
            EdgeArrayArray eAA = f.EdgeLoops;
            foreach (EdgeArray eA in eAA)
            {
                List<Curve> cs = new List<Curve>();
                foreach (Edge e in eA)
                {
                    List<XYZ> points = e.Tessellate() as List<XYZ>;
                    cs.Add(Line.CreateBound(points[0], points[1]));
                }
                pls.Add(new Polygon(cs));
                if (eAA.Size == 1)
                {
                    SurfacePolygon = pls[0];
                    OpeningPolygons = new List<Polygon>();
                    goto GetParameters;
                }
            }
            for (int i = 0; i < pls.Count; i++)
            {
                Plane plane = pls[i].Plane;
                for (int j = i + 1; j < pls.Count; j++)
                {
                    Polygon tempPoly = CheckGeometry.GetProjectPolygon(plane, pls[j]);
                    PolygonComparePolygonResult res = new PolygonComparePolygonResult(pls[i], tempPoly);
                    if (res.IntersectType == PolygonComparePolygonIntersectType.AreaOverlap)
                    {
                        if (res.ListPolygon[0] == pls[i])
                        {
                            SurfacePolygon = pls[j];
                            goto FinishLoops;
                        }
                        if (res.ListPolygon[0] == pls[j])
                        {
                            SurfacePolygon = pls[i];
                            goto FinishLoops;
                        }
                        else throw new Exception("Face must contain polygons inside polygon!");
                    }
                }
            }
            FinishLoops:
            if (SurfacePolygon == null) throw new Exception("Error when retrieve surface polygon!");
            Plane = SurfacePolygon.Plane;
            OpeningPolygons = new List<Polygon>();
            foreach (Polygon pl in pls)
            {
                if (pl == SurfacePolygon) continue;
                Polygon tempPoly = CheckGeometry.GetProjectPolygon(Plane, pl);
                OpeningPolygons.Add(tempPoly);
            }

            GetParameters:
            GetParameters();
        }

        /// <summary>
        /// Hàm khởi tạo MultiPolygon từ Polygon mẹ và các Polygon con
        /// Các Polygon phải đồng phẳng và các Polygon con phải nằm hoàn toàn bên trong Polygon mẹ
        /// </summary>
        /// <param name="surPolygon">Polygon mẹ đang xét</param>
        /// <param name="openPolygons">Tập hợp các Polygon con đang xét</param>
        public MultiPolygon(Polygon surPolygon, List<Polygon> openPolygons)
        {
            SurfacePolygon = surPolygon;
            OpeningPolygons = openPolygons;
            
            GetParameters();
        }

        /// <summary>
        /// Hàm khởi tạo MultiPolygon từ Polygon mẹ và một Polygon con
        /// Các Polygon phải đồng phẳng và Polygon con phải nằm hoàn toàn bên trong Polygon mẹ
        /// </summary>
        /// <param name="surPolygon">Polygon mẹ đang xét</param>
        /// <param name="openPolygon">Polygon con đang xét</param>
        public MultiPolygon(Polygon surPolygon, Polygon openPolygon)
        {
            SurfacePolygon = surPolygon;
            OpeningPolygons = new List<Polygon> { openPolygon };

            GetParameters();
        }

        /// <summary>
        /// Hàm gán các thuộc tính ListXYZPoint, Normal, CentralXYZPoint
        /// </summary>
        private void GetParameters()
        {
            ListXYZPoint = SurfacePolygon.ListXYZPoint;
            Normal = SurfacePolygon.Normal;
            CentralXYZPoint = SurfacePolygon.CentralXYZPoint;
        }

        /// <summary>
        /// Hàm gán các thuộc tính XVecManual, YVecManual, PlaneManual
        /// </summary>
        /// <param name="vec">Vector của một trong 2 hệ trục tự định nghĩa</param>
        /// <param name="isXVector">True: vector trước là VecX, False, vector trước là VecY</param>
        public void SetManualDirection(XYZ vec, bool isXVector = true)
        {
            SurfacePolygon.SetManualDirection(vec, isXVector);
            this.XVecManual = SurfacePolygon.XVecManual;
            this.YVecManual = SurfacePolygon.YVecManual;
            this.PlaneManual = SurfacePolygon.PlaneManual;
        }

        /// <summary>
        /// Hàm gán các thuộc tính TwoUVPointsBoundary, TwoXYZPointsBoundary
        /// </summary>
        /// <param name="vec">Vector của một trong 2 hệ trục tự định nghĩa</param>
        /// <param name="isXVector">True: vector trước là VecX, False, vector trước là VecY</param>
        public void SetTwoPointsBoundary(XYZ vec, bool isXVector = true)
        {
            SetManualDirection(vec, isXVector);
            SurfacePolygon.SetTwoPointsBoundary(vec, isXVector);
            this.TwoUVPointsBoundary = SurfacePolygon.TwoUVPointsBoundary;
            this.TwoXYZPointsBoundary = SurfacePolygon.TwoXYZPointsBoundary;
        }
    }

    /// <summary>
    /// Kiểu dữ liệu định nghĩa kết quả trả về khi so sánh vị trí tương đối của một Polygon và một MultiPolygon
    /// </summary>
    public class PolygonCompareMultiPolygonResult
    {
        /// <summary>
        /// Kiểu enum định nghĩa kết quả khi so sánh vị trí tương đối của một Polygon và một MultiPolygon
        /// </summary>
        public PolygonCompareMultiPolygonPositionType PositionType { get; private set; }

        /// <summary>
        /// Kiểu enum định nghĩa kết quả khi kiểm tra phần giao nhau của một Polygon và một MultiPolygon
        /// </summary>
        public PolygonCompareMultiPolygonIntersectType IntersectType { get; private set; }

        /// <summary>
        /// Trả về tập hợp các đoạn thẳng là phần giao giữa nhau một Polygon và một MultiPolygon
        /// </summary>
        public List<Line> ListLine { get; private set; }

        /// <summary>
        /// Trả về tập hợp các điểm là phần giao nhau giữa nhau một Polygon và một MultiPolygon
        /// </summary>
        public List<XYZ> ListPoint { get; private set; }

        /// <summary>
        /// Trả về tập hợp các Polygon là phần giao nhau giữa một Polygon và một MultiPolygon
        /// </summary>
        public List<Polygon> ListPolygon { get; private set; }

        /// <summary>
        /// Trả về Polygon đang xét so sánh
        /// </summary>
        private Polygon polygon;

        /// <summary>
        /// Trả về MultiPolygon đang xét so sánh
        /// </summary>
        private MultiPolygon multiPolygon;

        /// <summary>
        /// Trả về MultiPolygon là phần giao giữa một Polygon và một MultiPolygon
        /// </summary>
        public MultiPolygon MultiPolygon;

        /// <summary>
        /// Hàm khởi tạo kiểu dữ liệu định nghĩa kết quả so sánh vị trí tương đối của một Polygon và một MultiPolygon
        /// </summary>
        /// <param name="pl">Polygon đang xét</param>
        /// <param name="mpl">MultiPolygon đang xét</param>
        public PolygonCompareMultiPolygonResult(Polygon pl, MultiPolygon mpl)
        {
            this.polygon = pl; this.multiPolygon = mpl; GetPositionType(); GetIntersectTypeAndOtherParameter();
        }

        /// <summary>
        /// Hàm kiểm tra vị trí tương đối của Polygon và MultiPolygon, trả về thuộc tính PositionType
        /// </summary>
        private void GetPositionType()
        {
            if (GeomUtil.IsSameOrOppositeDirection(polygon.Normal, multiPolygon.Normal))
            {
                if (multiPolygon.SurfacePolygon.CheckXYZPointPosition(polygon.ListXYZPoint[0]) != PointComparePolygonResult.NonPlanar)
                {
                    PositionType = PolygonCompareMultiPolygonPositionType.Planar;
                    return;
                }
                PositionType = PolygonCompareMultiPolygonPositionType.Parallel; return;
            }
            PositionType = PolygonCompareMultiPolygonPositionType.NonPlarnar; return;
        }

        /// <summary>
        /// Hàm kiểm tra phần giao nhau giữa Polygon và MultiPolygon
        /// Trả về các thuộc tính IntersectType, ListLine, ListPoint, ListPolygon, MultiPolygon
        /// </summary>
        private void GetIntersectTypeAndOtherParameter()
        {
            switch (PositionType)
            {
                case PolygonCompareMultiPolygonPositionType.Parallel: this.IntersectType = PolygonCompareMultiPolygonIntersectType.NonIntersect; return;
                case PolygonCompareMultiPolygonPositionType.NonPlarnar: throw new Exception("Code for this case hasn't finished yet!");
                case PolygonCompareMultiPolygonPositionType.Planar:
                    Polygon surPL = multiPolygon.SurfacePolygon;
                    List<Polygon> openPLs = multiPolygon.OpeningPolygons;
                    PolygonComparePolygonResult res = new PolygonComparePolygonResult(polygon, surPL);
                    switch (res.IntersectType)
                    {
                        case PolygonComparePolygonIntersectType.NonIntersect: this.IntersectType = PolygonCompareMultiPolygonIntersectType.NonIntersect; return;
                        case PolygonComparePolygonIntersectType.Boundary: this.IntersectType = PolygonCompareMultiPolygonIntersectType.Boundary; ListLine = res.ListLine; return;
                        case PolygonComparePolygonIntersectType.Point: IntersectType = PolygonCompareMultiPolygonIntersectType.Point; ListPoint = res.ListPoint; return;
                        case PolygonComparePolygonIntersectType.AreaOverlap:
                            ListPolygon = new List<Polygon>();
                            this.MultiPolygon = null;
                            foreach (Polygon pl in res.ListPolygon)
                            {
                                Polygon temp = pl;
                                foreach (Polygon openPl in multiPolygon.OpeningPolygons)
                                {
                                    PolygonComparePolygonResult ppRes = new PolygonComparePolygonResult(temp, openPl);
                                    if (ppRes.IntersectType == PolygonComparePolygonIntersectType.AreaOverlap)
                                    {
                                        object polyorMultiPolygonCut = null;
                                        ppRes.GetOuterPolygon(temp, out polyorMultiPolygonCut);
                                        if (polyorMultiPolygonCut == null)
                                            goto Here;
                                        if (polyorMultiPolygonCut is MultiPolygon)
                                        {
                                            this.MultiPolygon = polyorMultiPolygonCut as MultiPolygon;
                                            return;
                                        }
                                        temp = polyorMultiPolygonCut as Polygon;
                                    }
                                }
                                ListPolygon.Add(temp);
                                Here: continue;
                            }
                            break;
                    }
                    break;
            }
        }
    }

    /// <summary>
    /// Kiểu enum định nghĩa kết quả khi so sánh vị trí tương đối của một Polygon và một MultiPolygon, có các giá trị:
    /// Planar: đồng phẳng, NonPlanar: không đồng phẳng, Parallel: song song
    /// </summary>
    public enum PolygonCompareMultiPolygonPositionType
    {
        Planar, NonPlarnar, Parallel
    }

    /// <summary>
    /// Kiểu enum định nghĩa kết quả khi kiểm tra phần giao nhau của một Polygon và một MultiPolygon, có các giá trị:
    /// AreaOverlap: trùng diện tích, Point: trùng điểm, Boundary: trùng cạnh, NonIntersect: không trùng nhau
    /// </summary>
    public enum PolygonCompareMultiPolygonIntersectType
    {
        AreaOverlap, Point, Boundary, NonIntersect
    }
}
