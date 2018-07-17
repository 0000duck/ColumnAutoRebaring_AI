using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace RevitAddin1
{
    public partial class BaseAttachedProperty
    {
        public static readonly DependencyProperty EditTimelineOKClickedProperty = DependencyProperty.RegisterAttached(
            "EditTimelineOKClicked", typeof(string), typeof(BaseAttachedProperty), new PropertyMetadata(null, new PropertyChangedCallback(OnEditTimelineOKClickedPropertyChanged)));

        public static string GetEditTimelineOKClickedProperty(DependencyObject obj)
        {
            return (string)obj.GetValue(EditTimelineOKClickedProperty);
        }
        public static void SetEditTimelineOKClickedProperty(DependencyObject obj, string value)
        {
            obj.SetValue(EditTimelineOKClickedProperty, value);
        }
        private static void OnEditTimelineOKClickedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Button btn = d as Button;
            if (btn == null) return;

            btn.Click -= EditTimelineOKClicked;
            if ((string)e.NewValue != null)
            {
                btn.Click += EditTimelineOKClicked;
            }
        }

        private static void EditTimelineOKClicked(object sender, RoutedEventArgs e)
        {
            int index = SingleWPF.Instance.SelectedViewInformationIndex;
            switch (SingleWPF.Instance.EditTImelineEnum)
            {
                case EditTimelineEnum.Edit:
                    SingleWPF.Instance.Timelines[SingleWPF.Instance.SelectedViewInformationIndex] =
                        int.Parse(SingleWPF.Instance.EditTimelineText);
                    break;
                case EditTimelineEnum.Add:
                    SingleWPF.Instance.Timelines.Add(int.Parse(SingleWPF.Instance.EditTimelineText));
                    break;
            }

            SingleWPF.Instance.Timelines.Sort();
            ViewInfomationDao.GetViewInfomations();
            SingleWPF.Instance.EditTimelineText = "";
            SingleWPF.Instance.EditTimelineForm.Hide();
        }
    }
}
