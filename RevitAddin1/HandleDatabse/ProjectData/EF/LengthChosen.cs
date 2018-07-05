using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandleDatabse.ProjectData.EF
{
    public class LengthChosen
    {
        public int IDColumn { get; set; }
        public string ID { get; set; }
        public int Count
        {
            get { return ID.Split('-').Length +1; }
        }
        public int L1 { get; set; }
        public int L2 { get; set; }
        public int Start1 { get; set; }
        public int Start2 { get; set; }
        public int End1
        {
            get { return L1 + Start1; }
        }
        public int End2
        {
            get { return L2 + Start2; }
        }
        public bool IsValid { get { return CheckValid(); } }
        public bool IsFinish { get; set; } = false;
        public int Residual { get; set; }
        public LengthChosen(int i, int j)
        {
            IDColumn = 0;
            DesignInformation desInfo = Singleton.Instance.DesignInformations[0];
            Start1 = Singleton.Instance.GeneralInfomation.StartOffset.Offset1 - desInfo.DevelopmentLength;
            Start2 = Singleton.Instance.GeneralInfomation.StartOffset.Offset2 - desInfo.DevelopmentLength;

            L1 = Singleton.Instance.Lengths[i];
            L2 = Singleton.Instance.Lengths[j];

            ID = $"{i},{j}";
        }
        public LengthChosen(LengthChosen befLenChosen, int i, int j)
        {
            IDColumn = befLenChosen.IDColumn + 1;
            DesignInformation desInfo = Singleton.Instance.DesignInformations[IDColumn];
            Start1 = befLenChosen.End1 - desInfo.DevelopmentLength;
            Start2 = befLenChosen.End2 - desInfo.DevelopmentLength;

            L1 = Singleton.Instance.Lengths[i];
            L2 = Singleton.Instance.Lengths[j];

            ID = $"{befLenChosen.ID}-{i},{j}";
        }
        private bool CheckValid()
        {
            DesignInformation aDesInfo = Singleton.Instance.DesignInformations[IDColumn + 1];
            bool isFinish = false;
            bool res = aDesInfo.IsCheckOK(End1, End2, out isFinish);
            if (isFinish) IsFinish = true;
            return res;
        }

    }
}
