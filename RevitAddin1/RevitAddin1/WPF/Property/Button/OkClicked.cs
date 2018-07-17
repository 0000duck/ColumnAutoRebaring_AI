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
        public static readonly DependencyProperty OKClickedProperty = DependencyProperty.RegisterAttached(
            "OKClicked", typeof(string), typeof(BaseAttachedProperty), new PropertyMetadata(null, new PropertyChangedCallback(OnOKClickedPropertyChanged)));

        public static string GetOKClickedProperty(DependencyObject obj)
        {
            return (string)obj.GetValue(OKClickedProperty);
        }
        public static void SetOKClickedProperty(DependencyObject obj, string value)
        {
            obj.SetValue(OKClickedProperty, value);
        }
        private static void OnOKClickedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Button btn = d as Button;
            if (btn == null) return;

            btn.Click -= OKClicked;
            if ((string)e.NewValue != null)
            {
                btn.Click += OKClicked;
            }
        }

        private static void OKClicked(object sender, RoutedEventArgs e)
        {
            Singleton.Instance.ViewInfoForm.Close();
        }
    }
}
