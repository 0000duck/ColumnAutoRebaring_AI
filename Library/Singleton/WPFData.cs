using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Single
{
    public class WPFData : INotifyPropertyChanged
    {
        #region Variables
        private string prefix;
        private string layer;
        private string level;
        private int developMultiply;
        private bool isOtherwiseClock;
        private double spacingMM;
        private double angleDeg;
        #endregion

        #region Properties
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
