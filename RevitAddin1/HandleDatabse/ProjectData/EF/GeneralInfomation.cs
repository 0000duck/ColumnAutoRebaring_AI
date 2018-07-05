using HandleDatabse.Database.Dao;
using HandleDatabse.Database.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandleDatabse.ProjectData.EF
{
    public class GeneralInfomation
    {
        private DevelopmentLength developmentLength;
        private StartOffset startOffset;
        private LengthInformation lengthInformation;
        public int TopLimit { get; set; }
        public DevelopmentLength DevelopmentLength
        {
            get
            {
                if (developmentLength == null) developmentLength = DevelopmentLengthDao.GetDevelopmentLength(Singleton.Instance.DataCombine.IDDevelopmentLength);
                return developmentLength;
            }
        }
        public StartOffset StartOffset
        {
            get
            {
                if (startOffset == null) startOffset = StartOffsetDao.GetStartOffset(Singleton.Instance.DataCombine.IDStartOffset);
                return startOffset;
            }
        }
        public LengthInformation LengthInformation
        {
            get
            {
                if (lengthInformation == null) lengthInformation = LengthInformationDao.GetLengthInformation(Singleton.Instance.DataCombine.IDLengthInformation);
                return lengthInformation;
            }
        }
    }
}
