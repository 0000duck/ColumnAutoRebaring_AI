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
        public static readonly DependencyProperty ChoosePrefixOKClickedProperty = DependencyProperty.RegisterAttached(
            "ChoosePrefixOKClicked", typeof(string), typeof(BaseAttachedProperty), new PropertyMetadata(null, new PropertyChangedCallback(OnChoosePrefixOKClickedPropertyChanged)));

        public static string GetChoosePrefixOKClickedProperty(DependencyObject obj)
        {
            return (string)obj.GetValue(ChoosePrefixOKClickedProperty);
        }
        public static void SetChoosePrefixOKClickedProperty(DependencyObject obj, string value)
        {
            obj.SetValue(ChoosePrefixOKClickedProperty, value);
        }
        private static void OnChoosePrefixOKClickedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Button btn = d as Button;
            if (btn == null) return;

            btn.Click -= ChoosePrefixOKClicked;
            if ((string)e.NewValue != null)
            {
                btn.Click += ChoosePrefixOKClicked;
            }
        }

        private static void ChoosePrefixOKClicked(object sender, RoutedEventArgs e)
        {
            if (SingleWPF.Instance.Prefix == "")
            {
                MessageBox.Show("Bạn phải nhập thông tin đầu vào!");
                return;
            }
            SingleWPF.Instance.IsPrefixOK = true;
            Singleton.Instance.ChoosePrefixForm.Close();
        }
    }
}
