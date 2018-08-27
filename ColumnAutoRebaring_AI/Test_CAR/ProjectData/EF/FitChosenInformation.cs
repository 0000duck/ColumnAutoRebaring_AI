using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_CAR
{
    public class FitChosenInformation
    {
        private List<int> singleFits;
        private List<int> doubleFits;
        private List<int> trippleFits;
        private List<int> singleImplants;
        private List<int> doubleImplants;
        public List<int> SingleFits
        {
            get
            {
                if (singleFits == null) singleFits = FullStandardLengthOrderDao.GetStandardLengths(Singleton.Instance.OtherData.DataCombine.IDFullStandardLengthOrder, StandardLengthEnum.L);
                return singleFits;
            }
        }
        public List<int> DoubleFits
        {
            get
            {
                if (doubleFits == null) doubleFits = FullStandardLengthOrderDao.GetStandardLengths(Singleton.Instance.OtherData.DataCombine.IDFullStandardLengthOrder, StandardLengthEnum.L2);
                return doubleFits;
            }
        }
        public List<int> TrippleFits
        {
            get
            {
                if (trippleFits == null) trippleFits = FullStandardLengthOrderDao.GetStandardLengths(Singleton.Instance.OtherData.DataCombine.IDFullStandardLengthOrder, StandardLengthEnum.L3);
                return trippleFits;
            }
        }
        public List<int> SingleImplants
        {
            get
            {
                if (singleImplants == null) singleImplants = FullStandardLengthOrderDao.GetStandardLengths(Singleton.Instance.OtherData.DataCombine.IDFullStandardLengthOrder, StandardLengthEnum.I);
                return singleImplants;
            }
        }
        public List<int> DoubleImplants
        {
            get
            {
                if (doubleImplants == null) doubleImplants = FullStandardLengthOrderDao.GetStandardLengths(Singleton.Instance.OtherData.DataCombine.IDFullStandardLengthOrder, StandardLengthEnum.I2);
                return doubleImplants;
            }
        }
    }
}
