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
        private RebarBarType selectedRebarBarType;
        private RebarHookType selectedRebarHookType;
        private double selectedBarDiameter = -1;
        private Plane activePlane;
        private SketchPlane activeSketchPlane;
        #endregion

        #region Properties
        public static Singleton Instance { get; set; }
        public string Prefix { get; set; }
        public string Level { get; set; }
        public UIApplication UIApplication { get; set; }
        public Element SelectedRebarTagType { get; set; }
        public XYZ SelectedXYZ { get; set; }
        public List<Rebar> Rebars { get; set; } = new List<Rebar>();
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
        public RebarBarType SelectedRebarBarType
        {
            get
            {
                if (selectedRebarBarType == null)
                    selectedRebarBarType = new FilteredElementCollector(Document).OfClass(typeof(RebarBarType)).Cast<RebarBarType>().Where(x => x.Name == ConstantValue.SelectedRebarDiameter).First();
                return selectedRebarBarType;
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
        public double SelectedBarDiameter
        {
            get
            {
                if (selectedBarDiameter == -1)
                    selectedBarDiameter = SelectedRebarBarType.BarDiameter;
                return selectedBarDiameter;
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
        #endregion

    }
}
