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
    public class Singleton
    {
        #region Variables
        private XYZ selectedXYZ;
        private Document document;
        private Transaction transaction;
        private List<View> views;
        private View rebarPlanTemplateView;
        private List<Workset> worksets;
        private RebarRoundingManager rebarRoundingManager;
        private View activeView;
        private UIDocument uIDocument;
        private Application application;
        private Selection selection;
        private Element selectedElement;
        private RebarHookType selectedRebarHookType;
        private Plane activePlane;
        private SketchPlane activeSketchPlane;
        private InputForm inputForm;
        private WorksetDefaultVisibilitySettings worksetDefaultVisibilitySettings;
        private List<RebarBarType> rebarTypes;
        private InputCentrifugalForm inputCentrifugalForm;
        private List<Category> categories;
        private View selectedPlanView;
        #endregion

        #region Properties
        public static Singleton Instance { get; set; }
        public string Level { get; set; }
        public UIApplication UIApplication { get; set; }
        public Element SelectedRebarTagType { get; set; }
        public List<RebarInfo> CircleRebarInfos { get; set; } = new List<RebarInfo>();
        public List<AssemblyInstanceInfo> AssemblyInstanceInfos { get; set; } = new List<AssemblyInstanceInfo>();
        public XYZ SelectedXYZ
        {
            get
            {
                if (selectedXYZ == null) selectedXYZ = ConstantValue.SelectedXYZ;
                return selectedXYZ;
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
        public Transaction Transaction
        {
            get
            {
                if (transaction == null) transaction = new Transaction(Document, "Addin");
                return transaction;
            }
        }
        public List<View> Views
        {
            get
            {
                if (views == null)
                    views = new FilteredElementCollector(Singleton.Instance.Document).OfClass(typeof(View)).Cast<View>().ToList();
                return views;
            }
        }
        public View RebarPlanTemplateView
        {
            get
            {
                if (rebarPlanTemplateView == null)
                    rebarPlanTemplateView = views.Where(x => x.Name == ConstantValue.RebarTemplateName).First();
                return rebarPlanTemplateView;
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
        public View ActiveView
        {
            get
            {
                if (activeView == null)
                    activeView = Document.ActiveView;
                return activeView;
            }
        }
        public UIDocument UIDocument
        {
            get
            {
                if (uIDocument == null)
                    uIDocument = UIApplication.ActiveUIDocument;
                return uIDocument;
            }
        }
        public Application Application
        {
            get
            {
                if (application == null)
                    application = UIApplication.Application;
                return application;
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
        public Element SelectedElement
        {
            get
            {
                if (selectedElement == null)
                    selectedElement = Document.GetElement(new ElementId(ConstantValue.SelectedIndex));
                return selectedElement;
            }
        }
        public RebarHookType SelectedRebarHookType
        {
            get
            {
                if (selectedRebarHookType == null)
                    selectedRebarHookType = new FilteredElementCollector(Document).OfClass(typeof(RebarHookType)).Cast<RebarHookType>().First();
                return selectedRebarHookType;
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
        public InputForm InputForm
        {
            get
            {
                if (inputForm == null)
                    inputForm = new InputForm();
                return inputForm;
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
        public InputCentrifugalForm InputCentrifugalForm
        {
            get
            {
                if (inputCentrifugalForm == null)
                    inputCentrifugalForm = new InputCentrifugalForm();
                return inputCentrifugalForm;
            }
        }
        public List<Category> Categories
        {
            get
            {
                if (categories == null)
                {
                    categories = new List<Category>();
                    foreach (Category cate in Singleton.Instance.Document.Settings.Categories)
                    {
                        categories.Add(cate);
                    } 
                }
                return categories;
            }
        }
        public View SelectedPlanView
        {
            get
            {
                if (selectedPlanView == null)
                {
                    foreach (View view in Views)
                    {
                        ViewFamilyType vft = Document.GetElement(view.GetTypeId()) as ViewFamilyType;
                        if (vft == null) continue;
                        if (vft.ViewFamily != ViewFamily.StructuralPlan) continue;
                        if (view.Name != SingleWPF.Instance.ViewName) continue;
                        selectedPlanView = view;
                        break;
                    }
                }
                return selectedPlanView;
            }
        }
        #endregion

    }
}
