using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using HandleDatabse.ProjectData.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project
{
    public class Singleton
    {
        // Variables
        private static Singleton instance;
        private List<RebarBarType> rebarBarTypes;
        private Element element;
        private WPFData wpfData;
        private OtherData otherData;

        // Properties
        public static Singleton Instance
        {
            get
            {
                if (instance == null) instance = new Singleton();
                return instance;
            }
        }
        public Document Document { get; set; }
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
        public WPFData WPFData
        {
            get
            {
                if (wpfData == null) wpfData = new WPFData();
                return wpfData;
            }
        }
        public OtherData OtherData
        {
            get
            {
                if (otherData == null) otherData = new OtherData();
                return otherData;
            }
        }
    }
}
