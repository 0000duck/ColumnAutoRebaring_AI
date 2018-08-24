using Autodesk.Revit.DB.Structure;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Project
{
    public class WPFData : INotifyPropertyChanged
    {
        #region Variables
        private bool allowOverLevel = false;
        private int developMultiply = 40;
        private RebarBarType selectedRebarType;
        #endregion

        #region Properties
        public bool AllowOverLevel
        {
            get
            {
                return allowOverLevel;
            }
            set
            {
                if (allowOverLevel == value) return;
                allowOverLevel = value;
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
        public bool IsClosedFormOK { get; set; } = false;
        public List<RebarBarType> RebarTypes { get { return Singleton.Instance.RebarBarTypes; } }
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
