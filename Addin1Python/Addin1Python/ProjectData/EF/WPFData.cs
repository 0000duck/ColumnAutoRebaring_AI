using Autodesk.Revit.DB.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Addin1Python
{
    public class WPFData : INotifyPropertyChanged
    {
        #region Variables
        private string inputTextStyle = "1.5mm Arial";
        private string inputGroupTypeName = "18E-NoiThep_Ln";
        private string inputXYZValue = "(575.110526845776, 1501.69476477745, 14.7637795275591)";
        private int inputElementId = 346404;
        private string inputPlanViewName = "18E-SSL02-CW";
        private string inputRebarPlanTemplateName = "HB_MB_ThepSanMongCauthangRamp (1/100)";
        private string inputPrefix = "18E";
        private string inputLayer = "D1";
        private string inputType = "ThepSan";
        private RebarBarType inputRebarType;
        private int inputDevelopMultiply = 40;
        private List<RebarBarType> rebarTypes;
        private bool inputIsOtherwiseClock = false;
        private double inputSpacingMM = 150;
        private double inputAngleDeg = 360;
        #endregion

        #region Properties
        public string InputTextStyle
        {
            get
            {
                return inputTextStyle;
            }
            set
            {
                if (inputTextStyle == value) return;
                inputTextStyle = value; OnPropertyChanged();
            }
        }
        public string InputGroupTypeName
        {
            get
            {
                return inputGroupTypeName;
            }
            set
            {
                if (inputGroupTypeName == value) return;
                inputGroupTypeName = value; OnPropertyChanged();
            }
        }
        public string InputXYZValue
        {
            get
            {
                return inputXYZValue;
            }
            set
            {
                if (inputXYZValue == value) return;
                inputXYZValue = value; OnPropertyChanged();
            }
        }
        public int InputElementId
        {
            get
            {
                return inputElementId;
            }
            set
            {
                if (inputElementId == value) return;
                inputElementId = value; OnPropertyChanged();
            }
        }
        public string InputPlanViewName
        {
            get
            {
                return inputPlanViewName;
            }
            set
            {
                if (inputPlanViewName == value) return;
                inputPlanViewName = value; OnPropertyChanged();
            }
        }
        public string InputRebarPlanTemplateName
        {
            get
            {
                return inputRebarPlanTemplateName;
            }
            set
            {
                if (inputRebarPlanTemplateName == value) return;
                inputRebarPlanTemplateName = value; OnPropertyChanged();
            }
        }
        public string InputPrefix
        {
            get
            {
                return inputPrefix;
            }
            set
            {
                if (inputPrefix == value) return;
                inputPrefix = value; OnPropertyChanged();
            }
        }
        public string InputLayer
        {
            get
            {
                return inputLayer;
            }
            set
            {
                if (inputLayer == value) return;
                inputLayer = value; OnPropertyChanged();
            }
        }
        public string InputType
        {
            get
            {
                return inputType;
            }
            set
            {
                if (inputType == value) return;
                inputType = value; OnPropertyChanged();
            }
        }
        public RebarBarType InputRebarType
        {
            get
            {
                return inputRebarType;
            }
            set
            {
                inputRebarType = value; OnPropertyChanged();
            }
        }
        public int InputDevelopMultiply
        {
            get
            {
                return inputDevelopMultiply;
            }
            set
            {
                if (inputDevelopMultiply == value) return;
                inputDevelopMultiply = value; OnPropertyChanged();
            }
        }
        public List<RebarBarType> RebarTypes
        {
            get
            {
                if (rebarTypes == null) rebarTypes = Singleton.Instance.RevitData.RebarBarTypes;
                return rebarTypes;
            }
        }
        public bool InputIsOtherwiseClock
        {
            get
            {
                return inputIsOtherwiseClock;
            }
            set
            {
                if (inputIsOtherwiseClock == value) return;
                inputIsOtherwiseClock = value; OnPropertyChanged();
            }
        }
        public double InputSpacingMM
        {
            get
            {
                return inputSpacingMM;
            }
            set
            {
                if (inputSpacingMM == value) return;
                inputSpacingMM = value; OnPropertyChanged();
            }
        }
        public double InputAngleDeg
        {
            get
            {
                return inputAngleDeg;
            }
            set
            {
                if (inputAngleDeg == value) return;
                inputAngleDeg = value; OnPropertyChanged();
            }
        }
        #endregion

        #region Event PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
