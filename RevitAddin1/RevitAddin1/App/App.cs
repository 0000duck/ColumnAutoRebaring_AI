using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace RevitAddin1
{
    class App : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication a)
        {
            String tabName = "BiMHoaBinh";
            try
            {
                a.CreateRibbonTab(tabName);
            }
            catch { }
            String panelName = "Auto Viewer";
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
            PushButtonData pbd = new PushButtonData("Auto Viewer", "Auto Viewer", dllPath, "RevitAddin1.ShowForm");
            pbd.LargeImage = BitmapToImageSource(RevitAddin1.Properties.Resources.AutoRebaring_32x32b);
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
