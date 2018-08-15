#region Namespaces
using System;
using System.Collections.Generic;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Windows.Media.Imaging;
using System.Reflection;
using System.IO;
#endregion

namespace RebarDetailing
{
    class App : IExternalApplication
    {
        const string Folder = "Library";
        public Result OnStartup(UIControlledApplication a)
        {
            String tabName = "BiMHoaBinh";
            try
            {
                a.CreateRibbonTab(tabName);
            }
            catch { }
            String panelName = "New RebarDetailing";
            Autodesk.Revit.UI.RibbonPanel panel = a.CreateRibbonPanel(tabName, panelName);
            //AddSplitButton(m_projectPanel);
            AddPushButton(panel);
            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication a)
        {
            return Result.Succeeded;
        }
        private void AddPushButton(Autodesk.Revit.UI.RibbonPanel panel)
        {
            string dllPath = System.Reflection.Assembly.GetExecutingAssembly().Location;
            string rdFolderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            PushButtonData pbd = new PushButtonData("RebarDetailing", "RebarDetailing", dllPath, "RebarDetailing.Command");
            pbd.LargeImage = BitmapToImageSource(RebarDetailing.Properties.Resources.RebarDetailing_32x32b);
            panel.AddItem(pbd);

            pbd = new PushButtonData("Settings", "Settings", dllPath, "RebarDetailing.Command1");
            pbd.LargeImage = BitmapToImageSource(RebarDetailing.Properties.Resources.settings_32x32b);
            panel.AddItem(pbd);
        }
        BitmapImage BitmapToImageSource(System.Drawing.Bitmap bitmap)
        {
            using (MemoryStream memory = new MemoryStream())
            {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                BitmapImage bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();
                return bitmapimage;
            }
        }
    }
}
