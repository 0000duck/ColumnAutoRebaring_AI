using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_CAR
{
    public class DesignInformation
    {
        #region Variables
        private int developmentLength=-1;
        private int developmentBetweenLength = -1;
        private int implantLength=-1;
        #endregion

        #region Properties
        public bool IsShorten { get; set; } = false;
        public int ID { get; set; }
        public int Bottom { get; set; }
        public int BottomTrue { get; set; }
        public int Top { get; set; }
        public int Diameter { get; set; }
        public string DiameterName { get { return $"T{Diameter}"; } }
        public int DevelopmentLength
        {
            get
            {
                if (developmentLength == -1)
                    developmentLength = Singleton.Instance.ModelData.GeneralInfomation.DevelopmentLength.DevelopMultilply * Diameter;
                return developmentLength;
            }
        }
        public int DevelopmentBetweenLength
        {
            get
            {
                if (developmentBetweenLength == -1)
                    developmentBetweenLength = DevelopmentLength + Singleton.Instance.ModelData.GeneralInfomation.DevelopmentLength.DevelopOffset;
                return developmentBetweenLength;
            }
        }
        public int ImplantLength
        {
            get
            {
                if (implantLength == -1)
                    implantLength = Singleton.Instance.ModelData.GeneralInfomation.DevelopmentLength.ImplantMultilply * Diameter;
                return implantLength;
            }
        }
        #endregion

        public bool IsCheckOK(int loc1, int loc2, out bool isFinish)
        {
            isFinish = false;
            int max = Singleton.Instance.OtherData.GeneralInfomation.LengthInformation.Max;
            int topLim = -1;

            if ((loc1 < Bottom + DevelopmentLength) || (loc1 > Top)) return false;
            if ((loc2 < Bottom + DevelopmentLength) || (loc2 > Top)) return false;
            if (Math.Abs(loc1 - loc2) < DevelopmentBetweenLength) return false;

            if (Singleton.Instance.OtherData.ShortenOrder.Contains(ID+1))
            {
                if (loc1 - BottomTrue + ImplantLength > Singleton.Instance.OtherData.GeneralInfomation.LengthInformation.ImplantMax) return false;
                if (loc2 - BottomTrue + ImplantLength > Singleton.Instance.OtherData.GeneralInfomation.LengthInformation.ImplantMax) return false;
                if (loc1 - BottomTrue + ImplantLength < Singleton.Instance.OtherData.GeneralInfomation.LengthInformation.Min) return false;
                if (loc2 - BottomTrue + ImplantLength < Singleton.Instance.OtherData.GeneralInfomation.LengthInformation.Min) return false;

                if (ID < Singleton.Instance.OtherData.LoopCount - 1)
                {
                    DesignInformation nextDesInfo = Singleton.Instance.OtherData.DesignInformations[ID + 1];
                    topLim = nextDesInfo.BottomTrue;
                    if (topLim - (loc1 - DevelopmentLength) > max) return false;
                    if (topLim - (loc2 - DevelopmentLength) > max) return false;
                }
            }

            if (ID < Singleton.Instance.OtherData.LoopCount - 1) return true;

            isFinish = true;
            topLim = Singleton.Instance.OtherData.GeneralInfomation.TopLimit;
            if (topLim - (loc1 - DevelopmentLength) > max) return false;
            if (topLim - (loc2 - DevelopmentLength) > max) return false;
            return true;
        }
        
    }
}
