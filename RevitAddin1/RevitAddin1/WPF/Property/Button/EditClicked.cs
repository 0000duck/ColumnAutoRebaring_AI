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
        public static readonly DependencyProperty EditClickedProperty = DependencyProperty.RegisterAttached(
            "EditClicked", typeof(string), typeof(BaseAttachedProperty), new PropertyMetadata(null, new PropertyChangedCallback(OnEditClickedPropertyChanged)));

        public static string GetEditClickedProperty(DependencyObject obj)
        {
            return (string)obj.GetValue(EditClickedProperty);
        }
        public static void SetEditClickedProperty(DependencyObject obj, string value)
        {
            obj.SetValue(EditClickedProperty, value);
        }
        private static void OnEditClickedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Button btn = d as Button;
            if (btn == null) return;

            btn.Click -= EditClicked;
            if ((string)e.NewValue != null)
            {
                btn.Click += EditClicked;
            }
        }

        private static void EditClicked(object sender, RoutedEventArgs e)
        {
            SingleWPF.Instance.EditTImelineEnum = EditTimelineEnum.Edit;
            SingleWPF.Instance.EditTimelineForm.ShowDialog();
        }
    }
}
