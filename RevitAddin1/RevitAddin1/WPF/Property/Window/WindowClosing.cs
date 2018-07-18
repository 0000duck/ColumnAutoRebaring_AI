using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace RevitAddin1
{
    public partial class BaseAttachedProperty
    {
        public static readonly DependencyProperty WindowClosingProperty = DependencyProperty.RegisterAttached(
            "WindowClosing", typeof(bool), typeof(BaseAttachedProperty), new PropertyMetadata(false, new PropertyChangedCallback(OnWindowClosingPropertyChanged)));

        public static bool GetWindowClosingProperty(DependencyObject obj)
        {
            return (bool)obj.GetValue(WindowClosingProperty);
        }
        public static void SetWindowClosingProperty(DependencyObject obj, bool value)
        {
            obj.SetValue(WindowClosingProperty, value);
        }
        private static void OnWindowClosingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Window window = d as Window;
            if (window == null) return;

            window.Closing -= WindowClosing;
            if ((bool)e.NewValue)
            {
                window.Closing += WindowClosing;
            }
        }

        private static void WindowClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            Singleton.Instance.ViewInfoForm.Hide();
        }
    }
}
