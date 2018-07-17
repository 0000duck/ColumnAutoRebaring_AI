using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RevitAddin1
{
    class SingleWPF: INotifyPropertyChanged
    {
        #region Variable
        private static SingleWPF instance;
        private ObservableCollection<ViewInformation> viewInformations = new ObservableCollection<ViewInformation>();
        #endregion

        #region Property
        public static SingleWPF Instance
        {
            get
            {
                if (instance == null) instance = new SingleWPF();
                return instance;
            }
        }
        public ObservableCollection<ViewInformation> ViewInformations
        {
            get
            {
                return viewInformations;
            }
            set
            {
                viewInformations = value; OnPropertyChanged();
            }
        }
        public int SelectedViewInformationIndex { get; set; } = -1;
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
