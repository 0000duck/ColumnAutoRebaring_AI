using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Test_CAR
{
    public class RevitData
    {
        // Variables
        private Application application;
        private UIDocument uiDocument;
        private Document document;
        private Transaction transaction;
        private List<RebarBarType> rebarBarTypes;
        private Element element;

        // Properties
        public UIApplication UIApplication { get; set; }
        public Application Application
        {
            get
            {
                if (application == null) application = UIApplication.Application;
                return application;
            }
        }
        public UIDocument UIDocument
        {
            get
            {
                if (uiDocument == null) uiDocument = UIApplication.ActiveUIDocument;
                return uiDocument;
            }
        }
        public Document Document
        {
            get
            {
                if (document == null) document = UIDocument.Document;
                return document;
            }
        }
        public Transaction Transaction
        {
            get
            {
                if (transaction== null) transaction = new Transaction(Document, "Addin");
                return transaction;
            }
        }
        public Element Element
        {
            get
            {
                if (element == null) element = Document.GetElement(new ElementId(298701));
                return element;
            }
        }
        public List<RebarBarType> RebarBarTypes
        {
            get
            {
                if (rebarBarTypes == null) rebarBarTypes = new FilteredElementCollector(Document).OfClass(typeof(RebarBarType)).Cast<RebarBarType>().ToList();
                return rebarBarTypes;
            }
        }
        public List<UV> UVs { get; set; } = new List<UV>() { new UV(-40.3, 25.5), new UV(-39.7, 25.5), new UV(-34.8, 25.5), new UV(-35.4, 25.5) };
    }
}
