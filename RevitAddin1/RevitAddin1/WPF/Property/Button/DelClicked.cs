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
        public static readonly DependencyProperty DelClickedProperty = DependencyProperty.RegisterAttached(
            "DelClicked", typeof(string), typeof(BaseAttachedProperty), new PropertyMetadata(null, new PropertyChangedCallback(OnDelClickedPropertyChanged)));

        public static string GetDelClickedProperty(DependencyObject obj)
        {
            return (string)obj.GetValue(DelClickedProperty);
        }
        public static void SetDelClickedProperty(DependencyObject obj, string value)
        {
            obj.SetValue(DelClickedProperty, value);
        }
        private static void OnDelClickedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Button btn = d as Button;
            if (btn == null) return;

            btn.Click -= DelClicked;
            if ((string)e.NewValue != null)
            {
                btn.Click += DelClicked;
            }
        }

        private static void DelClicked(object sender, RoutedEventArgs e)
        {
            int count = SingleWPF.Instance.ViewInformations.Count;
            int index = SingleWPF.Instance.SelectedViewInformationIndex;
            if (index == -1) return;
            if (index == 0 || index == count - 1)
            {
                SingleWPF.Instance.ViewInformations.RemoveAt(index);
                return;
            }

            SingleWPF.Instance.ViewInformations[index + 1] = new ViewInformation()
            {
                ID= SingleWPF.Instance.ViewInformations[index + 1].ID,
                Name = SingleWPF.Instance.ViewInformations[index].Name,
                FromDate = SingleWPF.Instance.ViewInformations[index - 1].ToDate + 1,
                ToDate = SingleWPF.Instance.ViewInformations[index + 1].ToDate
            };

            SingleWPF.Instance.ViewInformations.RemoveAt(index);
        }
    }
}
