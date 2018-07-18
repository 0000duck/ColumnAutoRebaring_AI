using Autodesk.Revit.UI.Selection;
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
        public static readonly DependencyProperty PickCreateDateClickedProperty = DependencyProperty.RegisterAttached(
            "PickCreateDateClicked", typeof(string), typeof(BaseAttachedProperty), new PropertyMetadata(null, new PropertyChangedCallback(OnPickCreateDateClickedPropertyChanged)));

        public static string GetPickCreateDateClickedProperty(DependencyObject obj)
        {
            return (string)obj.GetValue(PickCreateDateClickedProperty);
        }
        public static void SetPickCreateDateClickedProperty(DependencyObject obj, string value)
        {
            obj.SetValue(PickCreateDateClickedProperty, value);
        }
        private static void OnPickCreateDateClickedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Button btn = d as Button;
            if (btn == null) return;

            btn.Click -= PickCreateDateClicked;
            if ((string)e.NewValue != null)
            {
                btn.Click += PickCreateDateClicked;
            }
        }

        private static void PickCreateDateClicked(object sender, RoutedEventArgs e)
        {
            SingleWPF.Instance.EditTimelineForm.Hide();
            Singleton.Instance.ViewInfoForm.Hide();

            SingleWPF.Instance.EditTimelineText = Singleton.Instance.Document.GetElement(Singleton.Instance.Selection.PickObject(ObjectType.Element)).LookupParameter("CreateDate").AsInteger().ToString();
            SingleWPF.Instance.EditTimelineForm.ShowDialog();
        }
    }
}
