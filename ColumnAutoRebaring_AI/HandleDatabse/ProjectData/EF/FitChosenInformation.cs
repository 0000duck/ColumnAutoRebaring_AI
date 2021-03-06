﻿using HandleDatabse.Database.Dao;
using HandleDatabse.Database.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandleDatabse.ProjectData.EF
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
                if (singleFits == null) singleFits = FullStandardLengthOrderDao.GetStandardLengths(Singleton.Instance.DataCombine.IDFullStandardLengthOrder, Database.Others.StandardLengthEnum.L);
                return singleFits;
            }
        }
        public List<int> DoubleFits
        {
            get
            {
                if (doubleFits == null) doubleFits = FullStandardLengthOrderDao.GetStandardLengths(Singleton.Instance.DataCombine.IDFullStandardLengthOrder, Database.Others.StandardLengthEnum.L2);
                return doubleFits;
            }
        }
        public List<int> TrippleFits
        {
            get
            {
                if (trippleFits == null) trippleFits = FullStandardLengthOrderDao.GetStandardLengths(Singleton.Instance.DataCombine.IDFullStandardLengthOrder, Database.Others.StandardLengthEnum.L3);
                return trippleFits;
            }
        }
        public List<int> SingleImplants
        {
            get
            {
                if (singleImplants == null) singleImplants = FullStandardLengthOrderDao.GetStandardLengths(Singleton.Instance.DataCombine.IDFullStandardLengthOrder, Database.Others.StandardLengthEnum.I);
                return singleImplants;
            }
        }
        public List<int> DoubleImplants
        {
            get
            {
                if (doubleImplants == null) doubleImplants = FullStandardLengthOrderDao.GetStandardLengths(Singleton.Instance.DataCombine.IDFullStandardLengthOrder, Database.Others.StandardLengthEnum.I2);
                return doubleImplants;
            }
        }
    }
}
