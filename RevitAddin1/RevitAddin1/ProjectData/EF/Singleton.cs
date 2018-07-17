using Autodesk.Revit.DB;
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
        private static Singleton instance;
        private ViewInfoForm viewInfoForm;
        private List<Category> categories;
        private List<Category> targetCategories;
        private List<BuiltInCategory> builtInCates = new List<BuiltInCategory> {
            BuiltInCategory.OST_Walls, BuiltInCategory.OST_Floors, BuiltInCategory.OST_Stairs, BuiltInCategory.OST_Columns,
            BuiltInCategory.OST_StructuralColumns, BuiltInCategory.OST_StructuralFraming, BuiltInCategory.OST_GenericModel};
        private List<View3D> view3Ds;
        private List<ParameterFilterElement> parameterFilterElements;
        #endregion

        #region Property
        public static Singleton Instance
        {
            get
            {
                if (instance == null) instance = new Singleton();
                return instance;
            }
        }
        public Document Document { get; set; }
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
                    view3Ds = new FilteredElementCollector(Document).OfClass(typeof(View3D)).Cast<View3D>().Where(x => x.Name[0] == 'V').OrderBy(x=>x.Name).ToList();
                    if (view3Ds.Count == 0) throw new Exception("Bạn cần tạo View3d có tên là \"V1\" trước khi chảy add-in.");
                    if (view3Ds[0].Name != "V1") throw new Exception("Bạn cần tạo View3d có tên là \"V1\" trước khi chảy add-in.");
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
        #endregion
    }
}
