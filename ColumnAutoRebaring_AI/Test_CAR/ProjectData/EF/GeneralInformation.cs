using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_CAR
{
    public class GeneralInfomation
    {
        private DevelopmentLength developmentLength;
        private LengthInformation lengthInformation;

        public StartOffset StartOffset { get; set; }
        public int TopLimit = -1;
        public DevelopmentLength DevelopmentLength
        {
            get
            {
                if (developmentLength == null) developmentLength = DevelopmentLengthDao.GetDevelopmentLength(Singleton.Instance.ModelData.IdDevelopLength);
                return developmentLength;
            }
        }
        public LengthInformation LengthInformation
        {
            get
            {
                if (lengthInformation == null) lengthInformation = LengthInformationDao.GetLengthInformation(Singleton.Instance.ModelData.IdLengthInfo);
                return lengthInformation;
            }
        }
    }
}