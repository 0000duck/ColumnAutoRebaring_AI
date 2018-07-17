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
        public static readonly DependencyProperty OkClickedProperty = DependencyProperty.RegisterAttached(
            "OkClicked", typeof(bool), typeof(BaseAttachedProperty), new PropertyMetadata(false, new PropertyChangedCallback(OnOkClickedPropertyChanged)));

        public static bool GetOkClickedProperty(DependencyObject obj)
        {
            return (bool)obj.GetValue(OkClickedProperty);
        }
        public static void SetOkClickedProperty(DependencyObject obj, bool value)
        {
            obj.SetValue(OkClickedProperty, value);
        }
        private static void OnOkClickedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Button btn = d as Button;
            if (btn == null) return;

            btn.Click -= OkClicked;
            if ((bool)e.NewValue)
            {
                btn.Click += OkClicked;
            }
        }

        private static void OkClicked(object sender, RoutedEventArgs e)
        {
            Singleton.Instance.Input.Close();
        }
    }
}
