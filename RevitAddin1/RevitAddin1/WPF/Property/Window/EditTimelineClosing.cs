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
        public static readonly DependencyProperty EditTimelineClosingProperty = DependencyProperty.RegisterAttached(
            "EditTimelineClosing", typeof(bool), typeof(BaseAttachedProperty), new PropertyMetadata(false, new PropertyChangedCallback(OnEditTimelineClosingPropertyChanged)));

        public static bool GetEditTimelineClosingProperty(DependencyObject obj)
        {
            return (bool)obj.GetValue(EditTimelineClosingProperty);
        }
        public static void SetEditTimelineClosingProperty(DependencyObject obj, bool value)
        {
            obj.SetValue(EditTimelineClosingProperty, value);
        }
        private static void OnEditTimelineClosingPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Window window = d as Window;
            if (window == null) return;

            window.Closing -= EditTimelineClosing;
            if ((bool)e.NewValue)
            {
                window.Closing += EditTimelineClosing;
            }
        }

        private static void EditTimelineClosing(object sender, CancelEventArgs e)
        {
            e.Cancel = true;
            SingleWPF.Instance.EditTimelineForm.Hide();
        }
    }
}
