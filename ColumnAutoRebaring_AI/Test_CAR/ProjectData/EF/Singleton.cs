using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_CAR
{
    public class Singleton
    {     
        private RevitData revitData;
        private WPFData wpfData;
        private OtherData otherData;
        private ModelData modelData;

        public static Singleton Instance { get; set; }
        
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
        public RevitData RevitData
        {
            get
            {
                if (revitData == null) revitData = new RevitData();
                return revitData;
            }
        }
        public ModelData ModelData
        {
            get
            {
                if (modelData == null) modelData = new ModelData();
                return modelData;
            }
        }
    }
}
