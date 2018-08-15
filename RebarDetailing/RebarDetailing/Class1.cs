using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace RebarDetailing
{
    class Class1
    {
        public string Folder { get; private set; }

        void A()
        {
            string rdFolderPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string rftFilePath = Path.Combine(rdFolderPath, Folder, "Metric Detail Item.rft");
            Document famDoc = app.NewFamilyDocument(rftFilePath);
            if (famDoc == null)
            {
                throw new Exception("Cannot create family document");
            }

            Autodesk.Revit.Creation.FamilyItemFactory factory = famDoc.FamilyCreate;
            View famView = new FilteredElementCollector(famDoc).OfClass(typeof(View)).Cast<View>()
                .First(x => x.Name == "Ref. Level");
        }
    }
}
