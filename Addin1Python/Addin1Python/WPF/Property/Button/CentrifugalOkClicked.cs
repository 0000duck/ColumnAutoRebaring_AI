using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace Addin1Python
{
    public partial class BaseAttachedProperty
    {
        public static readonly DependencyProperty CentrifugalOkClickedProperty = DependencyProperty.RegisterAttached(
            "CentrifugalOkClicked", typeof(string), typeof(BaseAttachedProperty), new PropertyMetadata(null, new PropertyChangedCallback(OnCentrifugalOkClickedPropertyChanged)));

        public static string GetCentrifugalOkClickedProperty(DependencyObject obj)
        {
            return (string)obj.GetValue(CentrifugalOkClickedProperty);
        }
        public static void SetCentrifugalOkClickedProperty(DependencyObject obj, string value)
        {
            obj.SetValue(CentrifugalOkClickedProperty, value);
        }
        private static void OnCentrifugalOkClickedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Button btn = d as Button;
            if (btn == null) return;

            btn.Click -= CentrifugalOkClicked;
            if ((string)e.NewValue != null)
            {
                btn.Click += CentrifugalOkClicked;
            }
        }

        private static void CentrifugalOkClicked(object sender, RoutedEventArgs e)
        {
            SingleWPF.Instance.IsFormClosedOk = true;
            Singleton.Instance.InputCentrifugalForm.Close();
        }
    }
}
