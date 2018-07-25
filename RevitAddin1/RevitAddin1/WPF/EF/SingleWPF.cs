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
        private ObservableCollection<ViewInformation> viewInformations = new ObservableCollection<ViewInformation>();
        private List<int> timelines;
        private EditTimelineForm editTimelineForm;
        private int selectedViewInformationIndex = -1;
        private string editTimelineText;
        private string prefix="";
        private bool isSpecific = false;
        #endregion

        #region Property
        public static SingleWPF Instance { get; set; }
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
        public List<int> Timelines
        {
            get
            {
                if (timelines == null) timelines = TimelineDao.GetTimelines(SingleWPF.Instance.prefix);
                return timelines;
            }
            set
            {
                timelines = value;
            }
        }
        public EditTimelineForm EditTimelineForm
        {
            get
            {
                if (editTimelineForm == null) editTimelineForm = new EditTimelineForm();
                return editTimelineForm;
            }
        }
        public int SelectedViewInformationIndex
        {
            get
            {
                return selectedViewInformationIndex;
            }
            set
            {
                if (selectedViewInformationIndex != value) selectedViewInformationIndex = value;
                OnPropertyChanged("SelectedTimeline");
            }
        }
        public string EditTimelineText
        {
            get
            {
                return editTimelineText;
            }
            set
            {
                editTimelineText = value;
                OnPropertyChanged();
            }
        }
        public EditTimelineEnum EditTImelineEnum { get; set; }
        public string SelectedTimeline
        {
            get
            {
                if (EditTImelineEnum == EditTimelineEnum.Edit)
                {
                    int i = SelectedViewInformationIndex < 0 ? 0 : SelectedViewInformationIndex;
                    return $"{Timelines[i]}";
                }
                return "180101";
            }
        }
        public bool IsCloseFormOK { get; set; } = false;
        public bool IsPrefixOK { get; set; } = false;
        public string Prefix
        {
            get
            {
                return prefix;
            }
            set
            {
                prefix = value;
                OnPropertyChanged();
            }
        }
        public bool IsSpecific
        {
            get
            {
                return isSpecific;
            }
            set
            {
                isSpecific = value;
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
