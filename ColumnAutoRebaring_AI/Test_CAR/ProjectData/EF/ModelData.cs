using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Test_CAR
{
    public class ModelData
    {
        #region Variables
        private int idOrderL = -1;
        private int idOrderL2 = -1;
        private int idOrderL3 = -1;
        private int idOrderI = -1;
        private int idOrderI2 = -1;
        private int idFullStandLenOrder = -1;
        private int idLengthInfo = -1;
        private int idEleOff = -1;
        private int idDevLength = -1;
        private Model generalModel;
        private List<Model> models;
        private GeneralInfomation generalInfomation;
        private List<DesignInformation> designInformations;
        #endregion

        #region Variables
        public Model GeneralModel
        {
            get
            {
                if (generalModel == null)
                    generalModel = new Model()
                    {
                        RebarNames = InputRebarNames,
                        RebarOrders = InputRebarLocations,
                        ElevationOrders = InputElevationOrders,
                        ShortenOrders = InputShortenOrders,
                        BeamElevationOrders = InputBeamElevationOrders,
                        StartOffsets = InputStartOffsets
                    };
                return generalModel;
            }
        }
        public List<Model> Models
        {
            get
            {
                if (models == null) models = new List<Model>();
                return models;
            }
        }
        public GeneralInfomation GeneralInfomation
        {
            get
            {
                if (generalInfomation == null) generalInfomation = new GeneralInfomation();
                return generalInfomation;
            }
        }
        public List<DesignInformation> DesignInformations
        {
            get
            {
                if (designInformations == null) designInformations = DesignInformationDao.GetDesignInformations();
                return designInformations;
            }
        }
        public List<string> InputRebarNames { get; set; }
        public List<int> InputRebarLocations { get; set; }
        public List<int> InputElevationOrders { get; set; }
        public List<int> InputShortenOrders { get; set; }
        public List<int> InputBeamElevationOrders { get; set; }
        public List<int> InputOrderLs { get; set; }
        public List<int> InputOrderL2s { get; set; }
        public List<int> InputOrderL3s { get; set; }
        public List<int> InputOrderIs { get; set; }
        public List<int> InputOrderI2s { get; set; }
        public List<int> InputLengthInfo { get; set; }
        public List<int> InputElevationOffsets { get; set; }
        public List<int> InputDevelopLength { get; set; }
        public List<int> InputStartOffsets { get; set; }
        public int IdOrderL
        {
            get
            {
                if (idOrderL == -1)
                    idOrderL = StandardLengthOrderDao.InsertAndGetId(InputOrderLs, StandardLengthEnum.L);
                return idOrderL;
            }
        }
        public int IdOrderL2
        {
            get
            {
                if (idOrderL2 == -1)
                    idOrderL2 = StandardLengthOrderDao.InsertAndGetId(InputOrderL2s, StandardLengthEnum.L2);
                return idOrderL2;
            }
        }
        public int IdOrderL3
        {
            get
            {
                if (idOrderL3 == -1)
                    idOrderL3 = StandardLengthOrderDao.InsertAndGetId(InputOrderL3s, StandardLengthEnum.L3);
                return idOrderL3;
            }
        }
        public int IdOrderI
        {
            get
            {
                if (idOrderI == -1)
                    idOrderI = StandardLengthOrderDao.InsertAndGetId(InputOrderIs, StandardLengthEnum.I);
                return idOrderI;
            }
        }
        public int IdOrderI2
        {
            get
            {
                if (idOrderI2 == -1)
                    idOrderI2 = StandardLengthOrderDao.InsertAndGetId(InputOrderI2s, StandardLengthEnum.I2);
                return idOrderI2;
            }
        }
        public int IdFullStandardLengthOrder
        {
            get
            {
                if (idFullStandLenOrder == -1)
                    idFullStandLenOrder = FullStandardLengthOrderDao.InsertAndGetId(IdOrderL, IdOrderL2, IdOrderL3, IdOrderI, IdOrderI2);
                return idFullStandLenOrder;
            }
        }
        public int IdLengthInfo
        {
            get
            {
                if (idLengthInfo == -1)
                    idLengthInfo = LengthInformationDao.InsertAndGetId(InputLengthInfo);
                return idLengthInfo;
            }
        }
        public int IdElevationOffset
        {
            get
            {
                if (idEleOff == -1)
                    idEleOff = ElevationOffsetDao.InsertAndGetId(InputElevationOffsets);
                return idEleOff;
            }
        }
        public int IdDevelopLength
        {
            get
            {
                if (idDevLength == -1)
                    idDevLength = DevelopmentLengthDao.InsertAndGetId(InputDevelopLength);
                return idDevLength;
            }
        }
        #endregion
    }
}
