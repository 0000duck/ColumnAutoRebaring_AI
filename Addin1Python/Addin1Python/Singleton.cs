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
        
        #endregion

        #region Properties
        public static Singleton Instance { get; set; }
        public string Prefix { get; set; }
        public string Level { get; set; }
        public UIApplication UIApplication { get; set; }
        public Element SelectedRebarTagType { get; set; }
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
        #endregion

    }
}
