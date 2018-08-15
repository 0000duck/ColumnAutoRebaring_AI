using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using Geometry;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Addin1Python
{
    public class SingleWPF : INotifyPropertyChanged
    {
        #region Variables
        private string prefix = "18G";
        private string layer = "T2L1";
        private RebarBarType selectedRebarType;
        private int developMultiply = 40;
        private List<RebarBarType> rebarBarTypes;
        private bool isOtherwiseClock;
        private double spacingMM = 150;
        private double angleDeg = 360;
        private string viewName = "18G-SSL00-0";
        #endregion

        #region Properties
        public static SingleWPF Instance { get; set; }
        public bool IsFormClosedOk { get; set; } = false;
        public string Prefix
        {
            get
            {
                return prefix;
            }
            set
            {
                if (prefix == value) return;
                prefix = value;
                OnPropertyChanged();
            }
        }
        public string Layer
        {
            get
            {
                return layer;
            }
            set
            {
                if (layer == value) return;
                layer = value;
                OnPropertyChanged();
            }
        }
        public RebarBarType SelectedRebarType
        {
            get
            {
                return selectedRebarType;
            }
            set
            {
                selectedRebarType = value;
                OnPropertyChanged();
            }
        }
        public int DevelopMultiply
        {
            get
            {
                return developMultiply;
            }
            set
            {
                if (developMultiply == value) return;
                developMultiply = value;
                OnPropertyChanged();
            }
        }
        public double SelectedBarDiameter
        {
            get { return SelectedRebarType.BarDiameter; }
        }
        public List<RebarBarType> RebarBarTypes
        {
            get
            {
                if (rebarBarTypes == null) rebarBarTypes = Singleton.Instance.RebarBarTypes;
                return rebarBarTypes;
            }
        }
        public bool IsOtherwiseClock
        {
            get
            {
                return isOtherwiseClock;
            }
            set
            {
                if (isOtherwiseClock == value) return;
                isOtherwiseClock = value;
                OnPropertyChanged();
            }
        }
        public double SpacingMM
        {
            get
            {
                return spacingMM;
            }
            set
            {
                if (spacingMM == value) return;
                spacingMM = value;
                OnPropertyChanged();
            }
        }
        public double Spacing
        {
            get
            {
                return GeomUtil.milimeter2Feet(SpacingMM);
            }
        }
        public double AngleDef
        {
            get
            {
                return angleDeg;
            }
            set
            {
                if (angleDeg == value) return;
                angleDeg = value;
                OnPropertyChanged();
            }
        }
        public double Angle
        {
            get
            {
                return Math.PI * angleDeg / 180;
            }
        }
        public string ViewName
        {
            get
            {
                return viewName;
            }
            set
            {
                if (viewName == value) return;
                viewName = value;
                OnPropertyChanged();
            }
        }
        #endregion

        #region PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion
    }
}
