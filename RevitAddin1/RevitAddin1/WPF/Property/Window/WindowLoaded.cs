using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace RevitAddin1
{
    public partial class BaseAttachedProperty
    {
        public static readonly DependencyProperty WindowLoadedProperty = DependencyProperty.RegisterAttached(
            "WindowLoaded", typeof(bool), typeof(BaseAttachedProperty), new PropertyMetadata(false, new PropertyChangedCallback(OnWindowLoadedPropertyChanged)));

        public static bool GetWindowLoadedProperty(DependencyObject obj)
        {
            return (bool)obj.GetValue(WindowLoadedProperty);
        }
        public static void SetWindowLoadedProperty(DependencyObject obj, bool value)
        {
            obj.SetValue(WindowLoadedProperty, value);
        }
        private static void OnWindowLoadedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Window window = d as Window;
            if (window == null) return;

            window.Loaded -= WindowLoaded;
            if ((bool)e.NewValue)
            {
                window.Loaded += WindowLoaded;
            }
        }

        private static void WindowLoaded(object sender, RoutedEventArgs e)
        {
            ViewInfomationDao.GetViewInfomations();
        }
    }
}
