using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Test_CAR
{
    public class OtherData
    {
        #region Variables
        private InputForm inputForm;
        private GeneralInfomation generalInfomation;
        private List<int> lengths;
        private FitChosenInformation fitChosenInformation;
        private List<int> shortenOrder;
        private int idOrderL = -1;
        private int idOrderL2 = -1;
        private int idOrderL3 = -1;
        private int idOrderI = -1;
        private int idOrderI2 = -1;
        private int idFullStandLenOrder = -1;
        private int idLenInfo = -1;
        private int idDevLen = -1;
        private int idStartOff = -1;
        private int idEleOff = -1;
        #endregion

        #region Properties
        public InputForm InputForm
        {
            get
            {
                if (inputForm == null)
                {
                    inputForm = new InputForm() { DataContext = Singleton.Instance.WPFData };
                }
                return inputForm;
            }
        }
        public int LoopCount { get { return DesignInformations.Count; } }
        private List<DesignInformation> designInformations;
        public List<DesignInformation> DesignInformations
        {
            get
            {
                if (designInformations == null) designInformations = DesignInformationDao.GetDesignInformations();
                return designInformations;
            }
        }
        public List<LengthInfoCollection> LengthInfoCollections { get; set; } = new List<LengthInfoCollection>();
        public GeneralInfomation GeneralInfomation
        {
            get
            {
                if (generalInfomation == null) generalInfomation = new GeneralInfomation();
                return generalInfomation;
            }
        }
        public List<int> Lengths
        {
            get
            {
                if (lengths == null) lengths = LengthDao.GetLengths(GeneralInfomation.LengthInformation.Min, GeneralInfomation.LengthInformation.Max);
                return lengths;
            }
        }
        public FitChosenInformation FitChosenInformation
        {
            get
            {
                if (fitChosenInformation == null) fitChosenInformation = new FitChosenInformation();
                return fitChosenInformation;
            }
        }
        public List<int> ShortenOrder
        {
            get
            {
                if (shortenOrder == null) shortenOrder = ShortenOrderDao.GetShortenOrder();
                return shortenOrder;
            }
        }
        public LengthInfoCollection AOLLengthInfoCollection { get; set; }
        public LengthInfoCollection NAOLLengthInfoCollection { get; set; }
        public LengthInfoCollection ChosenLengthInfoCollection { get; set; }
        public bool ChosenAllowOverLevel { get; set; } = true;
        public DataCombine DataCombine { get; set; }
        public int IdOrderL
        {
            get
            {
                if (idOrderL == -1)
                    idOrderL = StandardLengthOrderDao.InsertAndGetId(new List<int> { 7800, 5850, 3900, 2925 }, StandardLengthEnum.L);
                return idOrderL;
            }
        }
        public int IdOrderL2
        {
            get
            {
                if (idOrderL2 == -1)
                    idOrderL2 = StandardLengthOrderDao.InsertAndGetId(new List<int> { 11700, 5850 }, StandardLengthEnum.L2);
                return idOrderL2;
            }
        }
        public int IdOrderL3
        {
            get
            {
                if (idOrderL3 == -1)
                    idOrderL3 = StandardLengthOrderDao.InsertAndGetId(new List<int> { 11700 }, StandardLengthEnum.L3);
                return idOrderL3;
            }
        }
        public int IdOrderI
        {
            get
            {
                if (idOrderI == -1)
                    idOrderI = StandardLengthOrderDao.InsertAndGetId(new List<int> { 3900, 2925, 2340, 1950 }, StandardLengthEnum.I);
                return idOrderI;
            }
        }
        public int IdOrderI2
        {
            get
            {
                if (idOrderI2 == -1)
                    idOrderI2 = StandardLengthOrderDao.InsertAndGetId(new List<int> { 5850, 3900, 2925 }, StandardLengthEnum.I2);
                return idOrderI2;
            }
        }
        public int IdFullStandardLengthOrder
        {
            get
            {
                if (idFullStandLenOrder == -1)
                    idFullStandLenOrder = FullStandardLengthOrderDao.InsertAndGetId(idOrderL, idOrderL2, idOrderL3, idOrderI, idOrderI2);
                return idFullStandLenOrder;
            }
        }
        public int IdLengthInfo
        {
            get
            {
                if (idLenInfo == -1)
                    idLenInfo = LengthInformationDao.InsertAndGetId(1900, 7800, 3900);
                return idLenInfo;
            }
        }
        public int IdElementOffset
        {
            get
            {
                if (idEleOff == -1)
                    idEleOff = ElevationOffsetDao.InsertAndGetId(200, 0, 0, 0);
                return idEleOff;
            }
        }
        public int IdDevelopLength
        {
            get
            {
                if (idDevLen == -1)
                    idDevLen = DevelopmentLengthDao.InsertAndGetId(40, 0, 40);
                return idDevLen;
            }
        }
        public int IdStartOffset
        {
            get
            {
                if (idStartOff == -1)
                    idStartOff = StartOffsetDao.InsertAndGetId(2900, 1500);
                return idStartOff;
            }
        }
        #endregion
    }
}
