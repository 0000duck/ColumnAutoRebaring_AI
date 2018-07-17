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
            int index = SingleWPF.Instance.SelectedViewInformationIndex;

            SingleWPF.Instance.Timelines.RemoveAt(index);
            ViewInfomationDao.GetViewInfomations();
        }
    }
}
