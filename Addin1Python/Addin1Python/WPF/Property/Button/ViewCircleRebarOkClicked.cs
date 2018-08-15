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
        public static readonly DependencyProperty ViewCircleRebarOkClickedProperty = DependencyProperty.RegisterAttached(
            "ViewCircleRebarOkClicked", typeof(string), typeof(BaseAttachedProperty), new PropertyMetadata(null, new PropertyChangedCallback(OnViewCircleRebarOkClickedPropertyChanged)));

        public static string GetViewCircleRebarOkClickedProperty(DependencyObject obj)
        {
            return (string)obj.GetValue(ViewCircleRebarOkClickedProperty);
        }
        public static void SetViewCircleRebarOkClickedProperty(DependencyObject obj, string value)
        {
            obj.SetValue(ViewCircleRebarOkClickedProperty, value);
        }
        private static void OnViewCircleRebarOkClickedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Button btn = d as Button;
            if (btn == null) return;

            btn.Click -= ViewCircleRebarOkClicked;
            if ((string)e.NewValue != null)
            {
                btn.Click += ViewCircleRebarOkClicked;
            }
        }

        private static void ViewCircleRebarOkClicked(object sender, RoutedEventArgs e)
        {
            SingleWPF.Instance.IsFormClosedOk = true;
            Singleton.Instance.InputViewCircleRebar.Close();
        }
    }
}
