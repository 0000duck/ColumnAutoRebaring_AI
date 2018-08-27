using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAddin1
{
    public class Singleton
    {
        #region Variable
        private Document document;
        private Selection selection;
        private ViewInfoForm viewInfoForm;
        private List<Category> categories;
        private List<Category> targetCategories;
        private List<BuiltInCategory> builtInCates = new List<BuiltInCategory> {
            // General
            BuiltInCategory.OST_GenericModel, BuiltInCategory.OST_Parts, 
            // Construction
            BuiltInCategory.OST_Site, BuiltInCategory.OST_Entourage,
            // Stutural
            BuiltInCategory.OST_Floors, BuiltInCategory.OST_StructuralColumns, BuiltInCategory.OST_StructuralFoundation, BuiltInCategory.OST_StructuralFraming, BuiltInCategory.OST_Rebar, BuiltInCategory.OST_Walls,
            // Architect
            BuiltInCategory.OST_Columns, BuiltInCategory.OST_Doors, BuiltInCategory.OST_StairsRailing, BuiltInCategory.OST_Roofs, BuiltInCategory.OST_Windows, BuiltInCategory.OST_Stairs
        };

        private List<View3D> view3Ds;
        private List<ParameterFilterElement> parameterFilterElements;
        private View activeView;
        private ChoosePrefixForm choosePrefixForm;
        #endregion

        #region Property
        public static Singleton Instance { get; set; }
        public Document Document
        {
            get
            {
                if (document == null) document = UIDocument.Document;
                return document;
            }
        }
        public Selection Selection
        {
            get
            {
                if (selection == null) selection = UIDocument.Selection;
                return selection;
            }
        }
        public UIDocument UIDocument { get; set; }
        public ViewInfoForm ViewInfoForm
        {
            get
            {
                if (viewInfoForm == null) viewInfoForm = new ViewInfoForm();
                return viewInfoForm;
            }
        }
        public List<Category> Categories
        {
            get
            {
                if (categories == null) categories = CategoryDao.GetCategories();
                return categories;
            }
        }
        public List<Category> TargetCategories
        {
            get
            {
                if (targetCategories == null) targetCategories = Categories.Where(x => builtInCates.Contains((BuiltInCategory)x.Id.IntegerValue)).ToList();
                return targetCategories;
            }
        }
        public List<ElementId> TargetCategoryIds
        {
            get { return TargetCategories.Select(x => x.Id).ToList(); }
        }
        public List<ViewInformation> ViewInformations { get; set; }
        public List<string> FilterNames
        {
            get
            {
                return ViewInformations.Select(x => x.RevitName).ToList();
            }
        }
        public List<View3D> View3Ds
        {
            get
            {
                if (view3Ds == null)
                {
                    string prefix = SingleWPF.Instance.Prefix;
                    int pLen = prefix.Length;

                    view3Ds = new List<View3D>();
                    var v3ds = new FilteredElementCollector(Document).OfClass(typeof(View3D)).Cast<View3D>().ToList();
                    foreach (View3D v3d in v3ds)
                    {
                        string name = v3d.Name;
                        if (name.Length < pLen + 2) continue;
                        if (name.Substring(0, pLen + 2) != $"V{prefix}_") continue;
                        view3Ds.Add(v3d);
                    }

                    view3Ds.Sort(new View3DSorter());
                    if (view3Ds.Count == 0) throw new Exception($"Bạn cần tạo View3d có tên là \"V{prefix}_1\" trước khi chạy add-in.");
                    if (view3Ds[0].Name != $"V{prefix}_1") throw new Exception($"Bạn cần tạo View3d có tên là \"V{prefix}_1\" trước khi chạy add-in.");
                }
                return view3Ds;
            }
        }
        public List<ParameterFilterElement> ParameterFilterElements
        {
            get
            {
                if (parameterFilterElements == null) parameterFilterElements = RevitUtility.CreateParameterFilterElements();
                return parameterFilterElements;
            }
        }
        public View ActiveView
        {
            get
            {
                if (activeView == null) activeView = Document.ActiveView;
                return activeView;
            }
        }
        public ChoosePrefixForm ChoosePrefixForm
        {
            get
            {
                if (choosePrefixForm == null) choosePrefixForm = new ChoosePrefixForm();
                return choosePrefixForm;
            }
        }
        public PartsVisibility PartsVisibility { get { return View3Ds[0].PartsVisibility; } }
        #endregion
    }
}
