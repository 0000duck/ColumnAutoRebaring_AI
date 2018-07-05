using HandleDatabse.Database.Dao;
using HandleDatabse.Database.EF;
using HandleDatabse.ProjectData.Dao;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandleDatabse.ProjectData.EF
{
    public class Singleton
    {
        private static Singleton instance;
        private GeneralInfomation generalInfomation;
        private List<int> lengths;
        private FitChosenInformation fitChosenInformation;
        private List<int> shortenOrder;

        public static Singleton Instance
        {
            get
            {
                if (instance == null) instance = new Singleton();
                return instance;
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
        public List<LengthInfoCollection> LengthInfoCollections { get; set; }
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
        public List<LengthChosen> LengthChosens { get; set; } = new List<LengthChosen>();
        public LengthInfoCollection OptimizeLengthInfoCollection
        {
            get
            {
                return LengthInfoCollections.OrderBy(x => x.Residual).First();
            }
        }
        public DataCombine DataCombine { get; set; }
    }
}
