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
        public static readonly DependencyProperty PickRemoveDateClickedProperty = DependencyProperty.RegisterAttached(
            "PickRemoveDateClicked", typeof(string), typeof(BaseAttachedProperty), new PropertyMetadata(null, new PropertyChangedCallback(OnPickRemoveDateClickedPropertyChanged)));

        public static string GetPickRemoveDateClickedProperty(DependencyObject obj)
        {
            return (string)obj.GetValue(PickRemoveDateClickedProperty);
        }
        public static void SetPickRemoveDateClickedProperty(DependencyObject obj, string value)
        {
            obj.SetValue(PickRemoveDateClickedProperty, value);
        }
        private static void OnPickRemoveDateClickedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Button btn = d as Button;
            if (btn == null) return;

            btn.Click -= PickRemoveDateClicked;
            if ((string)e.NewValue != null)
            {
                btn.Click += PickRemoveDateClicked;
            }
        }

        private static void PickRemoveDateClicked(object sender, RoutedEventArgs e)
        {
            SingleWPF.Instance.EditTimelineForm.Hide();
            Singleton.Instance.ViewInfoForm.Hide();

            SingleWPF.Instance.EditTimelineText = (Singleton.Instance.Document.GetElement(Singleton.Instance.Selection.PickObject(ObjectType.Element)).LookupParameter("RemoveDate").AsInteger()-1).ToString();
            SingleWPF.Instance.EditTimelineForm.ShowDialog();
        }
    }
}
