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
            BuiltInCategory.OST_Walls, BuiltInCategory.OST_Floors, BuiltInCategory.OST_Stairs, BuiltInCategory.OST_Columns,
            BuiltInCategory.OST_StructuralColumns, BuiltInCategory.OST_StructuralFraming, BuiltInCategory.OST_GenericModel};
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

                    List<string> subStrings = new FilteredElementCollector(Document).OfClass(typeof(View3D)).Select(x => x.Name.Substring(1, pLen)).ToList();
                    view3Ds = new FilteredElementCollector(Document).OfClass(typeof(View3D)).Cast<View3D>().Where(x => x.Name.Substring(1, pLen) == prefix).OrderBy(x=>x.Name).ToList();
                    if (view3Ds.Count == 0) throw new Exception($"Bạn cần tạo View3d có tên là \"V{prefix}1\" trước khi chạy add-in.");
                    if (view3Ds[0].Name != "V1") throw new Exception($"Bạn cần tạo View3d có tên là \"V{prefix}1\" trước khi chạy add-in.");
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
        #endregion
    }
}
