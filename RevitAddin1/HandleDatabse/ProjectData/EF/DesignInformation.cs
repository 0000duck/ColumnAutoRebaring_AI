using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandleDatabse.ProjectData.EF
{
    public class DesignInformation
    {
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
                return Singleton.Instance.GeneralInfomation.DevelopmentLength.DevelopMultilply * Diameter;
            }
        }
        public int DevelopmentBetweenLength
        {
            get { return DevelopmentLength + Singleton.Instance.GeneralInfomation.DevelopmentLength.DevelopOffset; }
        }
        public int ImplantLength
        {
            get { return Singleton.Instance.GeneralInfomation.DevelopmentLength.ImplantMultilply * Diameter; }
        }
        public bool IsCheckOK(int loc1, int loc2, out bool isFinish)
        {
            isFinish = false;
            int max = Singleton.Instance.GeneralInfomation.LengthInformation.Max;
            int topLim = -1;

            if ((loc1 < Bottom + DevelopmentLength) || (loc1 > Top)) return false;
            if ((loc2 < Bottom + DevelopmentLength) || (loc2 > Top)) return false;
            if (Math.Abs(loc1 - loc2) < DevelopmentBetweenLength) return false;

            if (Singleton.Instance.ShortenOrder.Contains(ID+1))
            {
                if (loc1 - BottomTrue + ImplantLength > Singleton.Instance.GeneralInfomation.LengthInformation.ImplantMax) return false;
                if (loc2 - BottomTrue + ImplantLength > Singleton.Instance.GeneralInfomation.LengthInformation.ImplantMax) return false;
                if (loc1 - BottomTrue + ImplantLength < Singleton.Instance.GeneralInfomation.LengthInformation.Min) return false;
                if (loc2 - BottomTrue + ImplantLength < Singleton.Instance.GeneralInfomation.LengthInformation.Min) return false;

                if (ID < Singleton.Instance.LoopCount - 1)
                {
                    DesignInformation nextDesInfo = Singleton.Instance.DesignInformations[ID + 1];
                    topLim = nextDesInfo.BottomTrue;
                    if (topLim - (loc1 - DevelopmentLength) > max) return false;
                    if (topLim - (loc2 - DevelopmentLength) > max) return false;
                }
            }

            if (ID < Singleton.Instance.LoopCount - 1) return true;

            isFinish = true;
            topLim = Singleton.Instance.GeneralInfomation.TopLimit;
            if (topLim - (loc1 - DevelopmentLength) > max) return false;
            if (topLim - (loc2 - DevelopmentLength) > max) return false;
            return true;
        }
    }
}
