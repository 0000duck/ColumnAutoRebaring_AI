using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.IO;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System.Text;

namespace Geometry
{
    /// <summary>
    /// Kiểu dữ liệu để lựa chọn chỉ những đối tượng cột và vách kết cấu trong Revit
    /// </summary>
    public class WallAndColumnSelection : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if (elem == null) return false;
            if (elem is Wall)
            {
                return true;
            }
            if (elem.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralColumns)
            {
                return true;
            }
            return false;
        }
        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }

    /// <summary>
    /// Kiểu dữ liệu để lựa chọn chỉ những đối tượng dầm và sàn kết cấu trong Revit
    /// </summary>
    public class FloorAndBeamSelection : ISelectionFilter
    {
        private Document doc;
        public FloorAndBeamSelection(Document doc)
        {
            this.doc = doc;
        }
        public bool AllowElement(Element elem)
        {
            if (elem == null) return false;
            if (elem is Floor)
            {
                return true;
            }
            if (elem.Category.Id.IntegerValue == (int)BuiltInCategory.OST_StructuralFraming)
            {
                return true;
            }
            return false;
        }
        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }

    /// <summary>
    /// Kiểu dữ liệu để lựa chọn chỉ những đối tượng Detail Item trong Revit
    /// </summary>
    public class DetailItemSeletion : ISelectionFilter
    {
        private Document doc;
        public DetailItemSeletion(Document doc)
        {
            this.doc = doc;
        }
        public bool AllowElement(Element elem)
        {
            if (elem == null) return false;
            if (elem.Category.Id.IntegerValue == (int)BuiltInCategory.OST_DetailComponents)
            {
                return true;
            }
            return false;
        }
        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }

    /// <summary>
    /// Kiểu dữ liệu để lựa chọn chỉ những đối tượng Generic Model trong Revit
    /// </summary>
    public class GenericModelSeletion : ISelectionFilter
    {
        private Document doc;
        public GenericModelSeletion(Document doc)
        {
            this.doc = doc;
        }
        public bool AllowElement(Element elem)
        {
            if (elem == null) return false;
            if (elem.Category.Id.IntegerValue == (int)BuiltInCategory.OST_GenericModel)
            {
                return true;
            }
            return false;
        }
        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
    /// <summary>
    /// Kiểu dữ liệu để lựa chọn các đối tượng trong các tập CategorySet nhất định
    /// </summary>
    public class CategoriesSelection : ISelectionFilter
    {
        private List<Category> cates;
        public CategoriesSelection(List<Category> cates)
        {
            this.cates = cates;
        }
        public bool AllowElement(Element elem)
        {
            if (elem == null) return false;
            foreach (Category c in cates)
            {
                if (elem.Category.Id.IntegerValue == c.Id.IntegerValue)
                {
                    return true;
                }
            }
            return false;
        }
        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
    /// <summary>
    /// Kiểu dữ liệu để chọn các Grid trong document
    /// </summary>
    public class GridSelection : ISelectionFilter
    {
        public bool AllowElement(Element e)
        {
            if (e == null) return false;
            if (e is Grid) return true;
            return false;
        }
        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
    /// <summary>
    /// Kiểu dữ liệu để lựa chọn chỉ những đối tượng rebar trong Revit
    /// </summary>
    public class RebarSelection : ISelectionFilter
    {
        public bool AllowElement(Element elem)
        {
            if (elem == null) return false;
            if (elem is Rebar)
            {
                return true;
            }
            return false;
        }
        public bool AllowReference(Reference reference, XYZ position)
        {
            return false;
        }
    }
}
