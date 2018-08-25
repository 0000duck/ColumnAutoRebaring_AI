using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Addin1Python
{
    public class RevitData
    {
        #region Variables
        private Application application;
        private UIDocument uiDocument;
        private Document document;
        private Selection selection;
        private Transaction transaction;
        private Plane activePlane;
        private SketchPlane activeSketchPlane;
        private View activeView;
        private List<View> views;
        private RebarRoundingManager rebarRoundingManager;
        private List<Workset> worksets;
        private WorksetDefaultVisibilitySettings worksetDefaultVisibilitySettings;
        private List<RebarBarType> rebarTypes;
        private List<Rebar> rebars;
        private List<Category> categories;
        #endregion

        #region Properties
        public UIApplication UIApplication { get; set; }
        public Application Application
        {
            get
            {
                if (application == null)
                    application = UIApplication.Application;
                return application;
            }
        }
        public UIDocument UIDocument
        {
            get
            {
                if (uiDocument == null)
                    uiDocument = UIApplication.ActiveUIDocument;
                return uiDocument;
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
        public Selection Selection
        {
            get
            {
                if (selection == null)
                    selection = UIDocument.Selection;
                return selection;
            }
        }
        public Transaction Transaction
        {
            get
            {
                if (transaction == null) transaction = new Transaction(Document, "Addin");
                return transaction;
            }
        }
        public Plane ActivePlane
        {
            get
            {
                if (activePlane == null)
                    activePlane = Plane.CreateByOriginAndBasis(ActiveView.Origin, ActiveView.RightDirection, ActiveView.UpDirection);
                return activePlane;
            }
        }
        public SketchPlane ActiveSketchPlane
        {
            get
            {
                if (activeSketchPlane == null)
                    activeSketchPlane = SketchPlane.Create(Document, ActivePlane);
                return activeSketchPlane;
            }
        }
        public View ActiveView
        {
            get
            {
                if (activeView == null)
                    activeView = Document.ActiveView;
                return activeView;
            }
        }
        public List<View> Views
        {
            get
            {
                if (views == null)
                    views = new FilteredElementCollector(Document).OfClass(typeof(View)).Cast<View>().ToList();
                return views;
            }
        }
        public RebarRoundingManager RebarRoundingManager
        {
            get
            {
                if (rebarRoundingManager == null)
                {
                    ReinforcementSettings rs = new FilteredElementCollector(Document).OfClass(typeof(ReinforcementSettings)).Cast<ReinforcementSettings>().First();
                    rebarRoundingManager = rs.GetRebarRoundingManager();
                }
                return rebarRoundingManager;
            }
        }
        public List<Workset> Worksets
        {
            get
            {
                if (worksets == null)
                    worksets = new FilteredWorksetCollector(Document).OfKind(WorksetKind.UserWorkset).ToList();
                return worksets;
            }
        }
        public WorksetDefaultVisibilitySettings WorksetDefaultVisibilitySettings
        {
            get
            {
                if (worksetDefaultVisibilitySettings == null)
                    worksetDefaultVisibilitySettings = new FilteredElementCollector(Document).OfClass(typeof(WorksetDefaultVisibilitySettings)).Cast<WorksetDefaultVisibilitySettings>().First();
                return worksetDefaultVisibilitySettings;
            }
        }
        public List<RebarBarType> RebarBarTypes
        {
            get
            {
                if (rebarTypes == null)
                    rebarTypes = new FilteredElementCollector(Document).OfClass(typeof(RebarBarType)).Cast<RebarBarType>().ToList();
                return rebarTypes;
            }
        }
        public List<Rebar> Rebars
        {
            get
            {
                if (rebars == null)
                    rebars = new FilteredElementCollector(Document).OfClass(typeof(Rebar)).Cast<Rebar>().ToList();
                return rebars;
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
        #endregion
    }
}
