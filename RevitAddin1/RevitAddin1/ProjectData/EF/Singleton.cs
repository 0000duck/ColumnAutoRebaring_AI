using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAddin1
{
    public class Singleton
    {
        #region Variable
        private static Singleton instance;
        private ViewInfoForm viewInfoForm;
        #endregion

        #region Property
        public static Singleton Instance
        {
            get
            {
                if (instance == null) instance = new Singleton();
                return instance;
            }
        }
        public Document Document { get; set; }
        public ViewInfoForm ViewInfoForm
        {
            get
            {
                if (viewInfoForm == null) viewInfoForm = new ViewInfoForm();
                return viewInfoForm;
            }
        }
        #endregion
    }
}
