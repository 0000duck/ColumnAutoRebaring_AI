using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Addin1Python
{
    public class ModelData
    {
        #region Variables
        private InputForm inputForm;
        private InputCentrifugalForm inputCentrifugalForm;
        private InputViewCircleRebar inputViewCircleRebar;
        private TextNoteType inputTextNoteType;
        private GroupType inputGroupType;
        private XYZ inputXYZ;
        private Element inputElement;
        private View inputPlanView;
        private View inputRebarPlanTemplateView;
        #endregion

        #region Properties
        public Element InputRebarTagType { get; set; }
        public InputForm InputForm
        {
            get
            {
                if (inputForm == null) inputForm = new InputForm();
                return inputForm;
            }
        }
        public InputCentrifugalForm InputCentrifugalForm
        {
            get
            {
                if (inputCentrifugalForm == null) inputCentrifugalForm = new InputCentrifugalForm();
                return inputCentrifugalForm;
            }
        }
        public InputViewCircleRebar InputViewCircleRebar
        {
            get
            {
                if (inputViewCircleRebar == null) inputViewCircleRebar = new InputViewCircleRebar();
                return inputViewCircleRebar;
            }
        }
        public TextNoteType InputTextNoteType
        {
            get
            {
                if (inputTextNoteType == null)
                {
                    inputTextNoteType = new FilteredElementCollector(Singleton.Instance.RevitData.Document).OfClass(typeof(TextNoteType))
                        .Where(x => x.Name == Singleton.Instance.WPFData.InputTextStyle).Cast<TextNoteType>().First();
                }
                return inputTextNoteType;
            }
        }
        public GroupType InputGroupType
        {
            get
            {
                if (inputGroupType == null)
                {
                    inputGroupType = new FilteredElementCollector(Singleton.Instance.RevitData.Document).OfClass(typeof(GroupType))
                        .Where(x => x.Name == Singleton.Instance.WPFData.InputGroupTypeName).Cast<GroupType>().First();
                }
                return inputGroupType;
            }
        }
        public XYZ InputXYZ
        {
            get
            {
                if (inputXYZ == null)
                {
                    List<double> values = Singleton.Instance.WPFData.InputXYZValue.Split('(', ',').Select(x => double.Parse(x)).ToList();
                    inputXYZ = new XYZ(values[0], values[1], values[2]);
                }
                return inputXYZ;
            }
        }
        public Element InputElement
        {
            get
            {
                if (inputElement == null) inputElement = Singleton.Instance.RevitData.Document.GetElement(
                    new ElementId(Singleton.Instance.WPFData.InputElementId));
                return inputElement;
            }
        }
        public View InputPlanView
        {
            get
            {
                if (inputPlanView == null)
                {
                    foreach (View view in Singleton.Instance.RevitData.Views)
                    {
                        ViewFamilyType rft = Singleton.Instance.RevitData.Document.GetElement(view.GetTypeId()) as ViewFamilyType;
                        if (rft == null) continue;
                        if (rft.ViewFamily != ViewFamily.StructuralPlan) continue;
                        if (view.Name != Singleton.Instance.WPFData.InputPlanViewName) continue;
                        inputPlanView = view;
                        break;
                    }
                }
                return inputPlanView;
            }
        }
        public View InputRebarPlanTemplateView
        {
            get
            {
                if (inputRebarPlanTemplateView == null)
                    inputRebarPlanTemplateView = Singleton.Instance.RevitData.Views.Where(x => x.Name == Singleton.Instance.WPFData.InputRebarPlanTemplateName).First();
                return inputRebarPlanTemplateView;
            }
        }
        #endregion
    }
}

