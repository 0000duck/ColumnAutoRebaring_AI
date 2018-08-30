using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Single
{
    public class RevitData
    {
        #region Variables
        private UIDocument uIDocument;
        private Document document;
        private View activeView;
        private Selection selection;
        private Plane activePlane;
        private SketchPlane activeSketchPlane;
        private Transaction transaction;
        private List<Element> instanceElements;
        private List<Element> typeElements;
        private List<View> views;
        private List<Workset> userWorksets;
        private WorksetDefaultVisibilitySettings worksetDefaultVisibilitySettings;
        private List<RebarBarType> rebarTypes;
        private List<Category> categories;
        private List<TextNoteType> textNoteTypes;
        #endregion

        #region Properties
        public UIApplication UIApplication { get; set; }
        public UIDocument UIDocument
        {
            get
            {
                if (uIDocument == null) uIDocument = UIApplication.ActiveUIDocument;
                return uIDocument;
            }
        }
        public Document Document
        {
            get
            {
                if (document == null) document = UIDocument.Document;
                return document;
            }
        }
        public View ActiveView
        {
            get
            {
                if (activeView == null) activeView = UIDocument.ActiveView;
                return activeView;
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
        public Plane ActivePlane
        {
            get
            {
                if (activePlane == null) activePlane = Plane.CreateByOriginAndBasis(ActiveView.Origin, ActiveView.RightDirection, ActiveView.UpDirection);
                return activePlane;
            }
        }
        public SketchPlane ActiveSketchPlane
        {
            get
            {
                if (activeSketchPlane == null) activeSketchPlane = SketchPlane.Create(Document, ActivePlane);
                return activeSketchPlane;
            }
        }
        public Transaction Transaction
        {
            get
            {
                if (transaction == null) transaction = new Transaction(Document, "RebarTools");
                return transaction;
            }
        }
        public List<Element> InstanceElements
        {
            get
            {
                if (instanceElements == null) instanceElements = new FilteredElementCollector(Document).WhereElementIsNotElementType().ToList();
                return instanceElements;
            }
        }
        public List<Element> TypeElements
        {
            get
            {
                if (typeElements == null) typeElements = new FilteredElementCollector(Document).WhereElementIsElementType().ToList();
                return typeElements;
            }
        }
        public List<View> Views
        {
            get
            {
                if (views == null) views = instanceElements.Where(x => x is View).Cast<View>().ToList();
                return views;
            }
        }
        public List<Workset> UserWorksets
        {
            get
            {
                if (userWorksets == null) userWorksets = new FilteredWorksetCollector(Document).OfKind(WorksetKind.UserWorkset).ToList();
                return userWorksets;
            }
        }
        public WorksetDefaultVisibilitySettings WorksetDefaultVisibilitySettings
        {
            get
            {
                if (worksetDefaultVisibilitySettings == null) worksetDefaultVisibilitySettings = new FilteredElementCollector(Document).OfClass(typeof(WorksetDefaultVisibilitySettings)).Cast<WorksetDefaultVisibilitySettings>().First();
                return worksetDefaultVisibilitySettings;
            }
        }
        public List<RebarBarType> RebarTypes
        {
            get
            {
                if (rebarTypes == null) rebarTypes = TypeElements.Where(x => x is RebarBarType).Cast<RebarBarType>().ToList();
                return rebarTypes;
            }
        }
        public List<Category> Categories
        {
            get
            {
                if (categories == null)
                {
                    categories = new List<Category>();
                    foreach (Category cate in Document.Settings.Categories)
                    {
                        categories.Add(cate);
                    }
                }
                return categories;
            }
        }
        public List<TextNoteType> TextNoteTypes
        {
            get
            {
                if (textNoteTypes == null) textNoteTypes = TypeElements.Where(x => x is TextNoteType).Cast<TextNoteType>().ToList();
                return textNoteTypes;
            }
        }
        #endregion
    }
}
