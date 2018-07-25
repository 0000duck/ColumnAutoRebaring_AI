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
        public static readonly DependencyProperty ChoosePrefixClosingProperty = DependencyProperty.RegisterAttached(
            "ChoosePrefixClosing", typeof(bool), typeof(BaseAttachedProperty), new PropertyMetadata(false, new PropertyChangedCallback(OnChoosePrefixClosingPropertyChanged)));

        public static bool GetChoosePrefixClosingProperty(DependencyObject obj)
        {
            return (bool)obj.GetValue(ChoosePrefixClosingProperty);
        }
        public static void SetChoosePrefixClosingProperty(DependencyObject obj, bool value)
        {
            obj.SetValue(ChoosePrefixClosingProperty, value);
        }
        private static void OnChoosePrefixClosingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Window window = d as Window;
            if (window == null) return;

            window.Closing -= ChoosePrefixClosing;
            if ((bool)e.NewValue)
            {
                window.Closing += ChoosePrefixClosing;
            }
        }

        private static void ChoosePrefixClosing(object sender, CancelEventArgs e)
        {
            if (!SingleWPF.Instance.IsPrefixOK) e.Cancel = true;
        }
    }
}
