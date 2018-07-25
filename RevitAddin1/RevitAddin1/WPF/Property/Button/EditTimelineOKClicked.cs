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
            int d = -1;
            if (!int.TryParse(SingleWPF.Instance.EditTimelineText, out d))
            {
                MessageBox.Show("Thông tin Timeline bị sai! Yêu cầu nhập lại.");
                SingleWPF.Instance.EditTimelineText = "";
                return;
            }

            int index = SingleWPF.Instance.SelectedViewInformationIndex;
            var timelines = SingleWPF.Instance.Timelines;
            int newTimeline = int.Parse(SingleWPF.Instance.EditTimelineText);
            switch (SingleWPF.Instance.EditTImelineEnum)
            {
                case EditTimelineEnum.Edit:
                    timelines[index] = newTimeline;
                    break;
                case EditTimelineEnum.Add:
                    if (!timelines.Contains(newTimeline)) timelines.Add(newTimeline);
                    break;
            }

            SingleWPF.Instance.Timelines.Sort();
            ViewInfomationDao.GetViewInfomations();
            SingleWPF.Instance.EditTimelineText = "";
            SingleWPF.Instance.EditTimelineForm.Hide();

            if (Singleton.Instance.ViewInfoForm.Visibility == Visibility.Hidden)
            {
                Singleton.Instance.ViewInfoForm.ShowDialog();
            }
        }
    }
}
