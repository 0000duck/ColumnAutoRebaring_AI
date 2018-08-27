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
        public static readonly DependencyProperty AddClickedProperty = DependencyProperty.RegisterAttached(
            "AddClicked", typeof(string), typeof(BaseAttachedProperty), new PropertyMetadata(null, new PropertyChangedCallback(OnAddClickedPropertyChanged)));

        public static string GetAddClickedProperty(DependencyObject obj)
        {
            return (string)obj.GetValue(AddClickedProperty);
        }
        public static void SetAddClickedProperty(DependencyObject obj, string value)
        {
            obj.SetValue(AddClickedProperty, value);
        }
        private static void OnAddClickedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Button btn = d as Button;
            if (btn == null) return;

            btn.Click -= AddClicked;
            if ((string)e.NewValue != null)
            {
                btn.Click += AddClicked;
            }
        }

        private static void AddClicked(object sender, RoutedEventArgs e)
        {
            SingleWPF.Instance.EditTImelineEnum = EditTimelineEnum.Add;
            SingleWPF.Instance.EditTimelineForm.ShowDialog();
        }
    }
}
