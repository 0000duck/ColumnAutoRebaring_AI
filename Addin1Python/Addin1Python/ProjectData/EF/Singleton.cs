using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Addin1Python
{
    public class Singleton
    {
        #region Variables
        private RevitData revitData;
        private WPFData wpfData;
        private ModelData modelData;

        
        private View rebarPlanTemplateView;

        private View selectedPlanView;




        private Element selectedElement;
        private RebarHookType selectedRebarHookType;
        #endregion

        #region Properties
        public static Singleton Instance { get; set; }
        public RevitData RevitData
        {
            get
            {
                if (revitData == null) revitData = new RevitData(); return revitData;
            }
        }
        public WPFData WPFData
        {
            get
            {
                if (wpfData == null) wpfData = new WPFData(); return wpfData;
            }
        }
        public ModelData ModelData
        {
            get
            {
                if (modelData == null) modelData = new ModelData(); return modelData;
            }
        }
        public string Level { get; set; }
        
        public List<RebarInfo> CircleRebarInfos { get; set; } = new List<RebarInfo>();
        public List<List<RebarInfo>> CircleRebarInfosList { get; set; } = new List<List<RebarInfo>>();
        public List<List<Rebar>> CentrifugalRebarsList { get; set; } = new List<List<Rebar>>();
        public List<AssemblyInstanceCentrifugalInfo> AssemblyInstanceCentrifugalInfos { get; set; } = new List<AssemblyInstanceCentrifugalInfo>();
        public List<AssemblyInstanceInfo> AssemblyInstanceInfos { get; set; } = new List<AssemblyInstanceInfo>();
        public List<GroupInfo> GroupInfos { get; set; } = new List<GroupInfo>();
        public List<ArcInfo> ArcInfos { get; set; } = new List<ArcInfo>();
        #endregion

    }
}
