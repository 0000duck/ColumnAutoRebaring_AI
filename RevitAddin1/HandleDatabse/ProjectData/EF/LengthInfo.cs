using HandleDatabse.Constant;
using HandleDatabse.Database.Dao;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandleDatabse.ProjectData.EF
{
    public class LengthInfo
    {
        public int Value { get; set; }
        public int ImplantValue
        {
            get
            {
                return End - Singleton.Instance.DesignInformations[IDColumn + 1].BottomTrue + Singleton.Instance.DesignInformations[IDColumn + 1].ImplantLength;
            }
        }

        public int IDColumn { get; set; }
        public int IDLocation { get; set; }
        public int Start { get; set; }
        public int End { get { return Start + Value; } }
        public string DiameterName { get { return Singleton.Instance.DesignInformations[IDColumn].DiameterName; } }
        public LengthType LengthType { get; set; } = LengthType.Residual;
        public LengthType LengthType2 { get; set; } = LengthType.Residual;
        public LengthPosition LengthPosition { get; set; } = LengthPosition.First;
        public LengthPosition LengthPosition2 { get; set; } = LengthPosition.First;
    }
    public enum LengthType
    {
        Residual, SingleFit, DoubleFit, TrippleFit, ResidualImplant, SingleImplant, DoubleImplant, DoubleImplantFit, Tripple2ImplantFit, TrippleImplant2Fit
    }
    public enum LengthPosition
    {
        First, Second, Third
    }
    public class LengthInfoCollection : IEnumerable<LengthInfo>
    {
        private List<int> Location1s = new List<int>() { Singleton.Instance.GeneralInfomation.StartOffset.Offset1 - Singleton.Instance.DesignInformations[0].DevelopmentLength };
        private List<int> Location2s = new List<int>() { Singleton.Instance.GeneralInfomation.StartOffset.Offset2 - Singleton.Instance.DesignInformations[0].DevelopmentLength };
        private List<LengthInfo> lenInfos = new List<LengthInfo>();
        public LengthInfo GetIndex(int idColumn, int idLocation)
        {
            return lenInfos.Where(x => x.IDColumn == idColumn && x.IDLocation == idLocation).First();
        }
        public int Residual { get; set; } = 0;
        public int Residual2 { get; set; } = 0;
        public LengthInfoCollection(string id, bool isFromDatabase = false)
        {
            string[] sFull = id.Split('-');
            for (int i = 0; i < sFull.Length; i++)
            {
                string[] sCol = sFull[i].Split(',');
                for (int j = 0; j < sCol.Length; j++)
                {
                    int value = isFromDatabase ? LengthDao.GetLength(int.Parse(sCol[j])) : Singleton.Instance.Lengths[int.Parse(sCol[j])];
                    lenInfos.Add(new LengthInfo()
                    {
                        Value = value,
                        IDColumn = i,
                        IDLocation = j,
                        Start = GetStart(i, j),
                        LengthType = Singleton.Instance.ShortenOrder.Contains(i + 2) ? LengthType.ResidualImplant : LengthType.Residual,
                        LengthType2 = Singleton.Instance.ShortenOrder.Contains(i + 2) ? LengthType.ResidualImplant : LengthType.Residual
                    });
                    if (j == 0) Location1s.Add(Location1s[Location1s.Count - 1] + value - Singleton.Instance.DesignInformations[i + 1].DevelopmentLength);
                    else Location2s.Add(Location2s[Location2s.Count - 1] + value - Singleton.Instance.DesignInformations[i + 1].DevelopmentLength);
                }
            }

            CheckResidual();
        }
        private int GetStart(int i, int j)
        {
            if (i == 0)
            {
                return (j == 0 ? Singleton.Instance.GeneralInfomation.StartOffset.Offset1 : Singleton.Instance.GeneralInfomation.StartOffset.Offset2) - Singleton.Instance.DesignInformations[0].DevelopmentLength;
            }
            return GetIndex(i - 1, j).End - Singleton.Instance.DesignInformations[i].DevelopmentLength;
        }
        private void CheckResidual(int i=0)
        {
            LengthInfo lenInfo1 = lenInfos.Where(x => x.IDColumn == i && x.IDLocation == 0).First();
            LengthInfo lenInfo2 = lenInfos.Where(x => x.IDColumn == i && x.IDLocation == 1).First();

            if (i == Singleton.Instance.LoopCount - 2)
            {
                SetResidual(lenInfo1, lenInfo2);
                return;
            }

            LengthInfo lenInfo3 = lenInfos.Where(x => x.IDColumn == i + 1 && x.IDLocation == 0).First();
            LengthInfo lenInfo4 = lenInfos.Where(x => x.IDColumn == i + 1 && x.IDLocation == 1).First();

            // Column 0 Implanted
            if (Singleton.Instance.ShortenOrder.Contains(i + 2))
            {
                SetSingleImplant(lenInfo1); SetSingleImplant(lenInfo2);
                SetDoubleImplant(lenInfo1, lenInfo2);

                // Column 1 Implanted
                if (Singleton.Instance.ShortenOrder.Contains(i + 3))
                {
                    SetSingleImplant(lenInfo3); SetSingleImplant(lenInfo4);
                    SetDoubleImplant(lenInfo3, lenInfo4);
                }
                // Column 1 Not Implanted
                else
                {
                    SetSingleFit(lenInfo3); SetSingleFit(lenInfo4);
                    SetDoubleFit(lenInfo3, lenInfo4);

                    // Different Diameter
                    if (Singleton.Instance.DesignInformations[i].Diameter != Singleton.Instance.DesignInformations[i + 1].Diameter)
                    {
                        
                    }
                    // Same Diameter
                    else
                    {
                        SetDoubleImplantFit(lenInfo1, lenInfo3); SetDoubleImplantFit(lenInfo1, lenInfo4); SetDoubleImplantFit(lenInfo2, lenInfo3); SetDoubleImplantFit(lenInfo2, lenInfo4);
                        SetTripple2ImplantFit(lenInfo1, lenInfo2, lenInfo3); SetTripple2ImplantFit(lenInfo1, lenInfo2, lenInfo4);
                        SetTrippleImplant2Fit(lenInfo1, lenInfo3, lenInfo4); SetTrippleImplant2Fit(lenInfo2, lenInfo3, lenInfo4);
                    }
                }
            }
            // Column 0 Not Implanted
            else
            {
                SetSingleFit(lenInfo1); SetSingleFit(lenInfo2);
                SetDoubleFit(lenInfo1, lenInfo2);

                // Column 1 Implanted
                if (Singleton.Instance.ShortenOrder.Contains(i + 3))
                {
                    SetSingleImplant(lenInfo3); SetSingleImplant(lenInfo4);
                    SetDoubleImplant(lenInfo3, lenInfo4);
                }
                // Column 1 Not Implanted
                else
                {
                    SetSingleFit(lenInfo3); SetSingleFit(lenInfo4);
                    SetDoubleFit(lenInfo3, lenInfo4);

                    // Different Diameter
                    if (Singleton.Instance.DesignInformations[i].Diameter != Singleton.Instance.DesignInformations[i + 1].Diameter)
                    {

                    }
                    // Same Diameter
                    else
                    {
                        SetDoubleFit(lenInfo1, lenInfo3); SetDoubleFit(lenInfo1, lenInfo4); SetDoubleFit(lenInfo2, lenInfo3); SetDoubleFit(lenInfo2, lenInfo4);
                        SetTrippleFit(lenInfo1, lenInfo2, lenInfo3); SetTrippleFit(lenInfo1, lenInfo2, lenInfo4);
                        SetTrippleFit(lenInfo1, lenInfo3, lenInfo4); SetTrippleFit(lenInfo2, lenInfo3, lenInfo4);
                    }
                }
            }

            SetResidual(lenInfo1, lenInfo2);
            CheckResidual(i + 1);
        }
        private void SetSingleFit(LengthInfo lenInfo)
        {
            if (lenInfo.LengthType == LengthType.Residual)
            {
                if (Singleton.Instance.FitChosenInformation.SingleFits.Contains(lenInfo.Value))
                {
                    lenInfo.LengthType = LengthType.SingleFit;
                    lenInfo.LengthPosition = LengthPosition.First;
                }
            }
        }
        private void SetSingleFit2(LengthInfo lenInfo)
        {
            if (lenInfo.LengthType2 == LengthType.Residual)
            {
                if (Singleton.Instance.FitChosenInformation.SingleFits.Contains(lenInfo.Value))
                {
                    lenInfo.LengthType2 = LengthType.SingleFit;
                    lenInfo.LengthPosition2 = LengthPosition.First;
                }
            }
        }
        private void SetDoubleFit(LengthInfo lenInfo1, LengthInfo lenInfo2)
        {
            if (lenInfo1.LengthType == LengthType.Residual && lenInfo2.LengthType == LengthType.Residual)
            {
                if (Singleton.Instance.FitChosenInformation.DoubleFits.Contains(lenInfo1.Value + lenInfo2.Value))
                {
                    lenInfo1.LengthType = LengthType.DoubleFit; lenInfo2.LengthType = LengthType.DoubleFit;
                    lenInfo1.LengthPosition = LengthPosition.First; lenInfo2.LengthPosition = LengthPosition.Second;
                }
            }
        }
        private void SetDoubleFit2(LengthInfo lenInfo1, LengthInfo lenInfo2)
        {
            if (lenInfo1.LengthType2 == LengthType.Residual && lenInfo2.LengthType2 == LengthType.Residual)
            {
                if (Singleton.Instance.FitChosenInformation.DoubleFits.Contains(lenInfo1.Value + lenInfo2.Value))
                {
                    lenInfo1.LengthType2 = LengthType.DoubleFit; lenInfo2.LengthType2 = LengthType.DoubleFit;
                    lenInfo1.LengthPosition2 = LengthPosition.First; lenInfo2.LengthPosition2 = LengthPosition.Second;
                }
            }
        }
        private void SetTrippleFit(LengthInfo lenInfo1, LengthInfo lenInfo2, LengthInfo lenInfo3)
        {
            if (lenInfo1.LengthType == LengthType.Residual && lenInfo2.LengthType == LengthType.Residual && lenInfo3.LengthType == LengthType.Residual)
            {
                if (Singleton.Instance.FitChosenInformation.TrippleFits.Contains(lenInfo1.Value + lenInfo2.Value + lenInfo3.Value))
                {
                    lenInfo1.LengthType = LengthType.TrippleFit; lenInfo2.LengthType = LengthType.TrippleFit; lenInfo3.LengthType = LengthType.TrippleFit;
                    lenInfo1.LengthPosition = LengthPosition.First; lenInfo2.LengthPosition = LengthPosition.Second; lenInfo3.LengthPosition = LengthPosition.Third;
                }
            }
        }
        private void SetSingleImplant(LengthInfo lenInfo1)
        {
            if (lenInfo1.LengthType == LengthType.ResidualImplant)
            {
                if (Singleton.Instance.FitChosenInformation.SingleImplants.Contains(lenInfo1.ImplantValue))
                {
                    lenInfo1.LengthType = LengthType.SingleImplant;
                    lenInfo1.LengthPosition = LengthPosition.First;
                }
            }
        }
        private void SetSingleImplant2(LengthInfo lenInfo1)
        {
            if (lenInfo1.LengthType2 == LengthType.ResidualImplant)
            {
                if (Singleton.Instance.FitChosenInformation.SingleImplants.Contains(lenInfo1.ImplantValue))
                {
                    lenInfo1.LengthType2 = LengthType.SingleImplant;
                    lenInfo1.LengthPosition2 = LengthPosition.First;
                }
            }
        }
        private void SetDoubleImplant(LengthInfo lenInfo1, LengthInfo lenInfo2)
        {
            if (lenInfo1.LengthType == LengthType.ResidualImplant && lenInfo2.LengthType == LengthType.ResidualImplant)
            {
                if (Singleton.Instance.FitChosenInformation.DoubleImplants.Contains(lenInfo1.ImplantValue + lenInfo2.ImplantValue))
                {
                    lenInfo1.LengthType = LengthType.DoubleImplant; lenInfo2.LengthType = LengthType.DoubleImplant;
                    lenInfo1.LengthPosition = LengthPosition.First; lenInfo2.LengthPosition = LengthPosition.Second;
                }
            }
        }
        private void SetDoubleImplant2(LengthInfo lenInfo1, LengthInfo lenInfo2)
        {
            if (lenInfo1.LengthType2 == LengthType.ResidualImplant && lenInfo2.LengthType2 == LengthType.ResidualImplant)
            {
                if (Singleton.Instance.FitChosenInformation.DoubleImplants.Contains(lenInfo1.ImplantValue + lenInfo2.ImplantValue))
                {
                    lenInfo1.LengthType2 = LengthType.DoubleImplant; lenInfo2.LengthType2 = LengthType.DoubleImplant;
                    lenInfo1.LengthPosition2 = LengthPosition.First; lenInfo2.LengthPosition2 = LengthPosition.Second;
                }
            }
        }
        private void SetDoubleImplantFit(LengthInfo lenInfo1, LengthInfo lenInfo2)
        {
            if (lenInfo1.LengthType == LengthType.ResidualImplant && lenInfo2.LengthType == LengthType.Residual)
            {
                if (Singleton.Instance.FitChosenInformation.DoubleFits.Contains(lenInfo1.ImplantValue + lenInfo2.Value))
                {
                    lenInfo1.LengthType = LengthType.DoubleImplantFit; lenInfo2.LengthType = LengthType.DoubleImplantFit;
                    lenInfo1.LengthPosition = LengthPosition.First; lenInfo2.LengthPosition = LengthPosition.Second;
                }
            }
        }
        private void SetTripple2ImplantFit(LengthInfo lenInfo1, LengthInfo lenInfo2, LengthInfo lenInfo3)
        {
            if (lenInfo1.LengthType == LengthType.ResidualImplant && lenInfo2.LengthType == LengthType.ResidualImplant && lenInfo3.LengthType == LengthType.Residual)
            {
                if (Singleton.Instance.FitChosenInformation.TrippleFits.Contains(lenInfo1.ImplantValue + lenInfo2.ImplantValue + lenInfo3.Value))
                {
                    lenInfo1.LengthType = LengthType.Tripple2ImplantFit; lenInfo2.LengthType = LengthType.Tripple2ImplantFit; lenInfo3.LengthType = LengthType.Tripple2ImplantFit;
                    lenInfo1.LengthPosition = LengthPosition.First; lenInfo2.LengthPosition = LengthPosition.Second; lenInfo3.LengthPosition = LengthPosition.Third;
                }
            }
        }
        private void SetTrippleImplant2Fit(LengthInfo lenInfo1, LengthInfo lenInfo2, LengthInfo lenInfo3)
        {
            if (lenInfo1.LengthType == LengthType.ResidualImplant && lenInfo2.LengthType == LengthType.Residual && lenInfo3.LengthType == LengthType.Residual)
            {
                if (Singleton.Instance.FitChosenInformation.TrippleFits.Contains(lenInfo1.ImplantValue + lenInfo2.Value + lenInfo3.Value))
                {
                    lenInfo1.LengthType = LengthType.Tripple2ImplantFit; lenInfo2.LengthType = LengthType.Tripple2ImplantFit; lenInfo3.LengthType = LengthType.Tripple2ImplantFit;
                    lenInfo1.LengthPosition = LengthPosition.First; lenInfo2.LengthPosition = LengthPosition.Second; lenInfo3.LengthPosition = LengthPosition.Third;
                }
            }
        }
        private void SetResidual(LengthInfo lenInfo1, LengthInfo lenInfo2)
        {
            if (lenInfo1.LengthType == LengthType.Residual)
            {
                Residual += ConstantValue.MaxLength - lenInfo1.Value;
            }
            else if (lenInfo1.LengthType == LengthType.ResidualImplant)
            {
                Residual += ConstantValue.MaxLength - lenInfo1.ImplantValue;
            }

            if (lenInfo2.LengthType == LengthType.Residual)
            {
                Residual += ConstantValue.MaxLength - lenInfo2.Value;
            }
            else if (lenInfo2.LengthType == LengthType.ResidualImplant)
            {
                Residual += ConstantValue.MaxLength - lenInfo2.ImplantValue;
            }
        }
        private void SetResidual2(LengthInfo lenInfo1, LengthInfo lenInfo2)
        {
            if (lenInfo1.LengthType2 == LengthType.Residual)
            {
                Residual2 += ConstantValue.MaxLength - lenInfo1.Value;
            }
            else if (lenInfo1.LengthType2 == LengthType.ResidualImplant)
            {
                Residual2 += ConstantValue.MaxLength - lenInfo1.ImplantValue;
            }

            if (lenInfo2.LengthType2 == LengthType.Residual)
            {
                Residual2 += ConstantValue.MaxLength - lenInfo2.Value;
            }
            else if (lenInfo2.LengthType2 == LengthType.ResidualImplant)
            {
                Residual2 += ConstantValue.MaxLength - lenInfo2.ImplantValue;
            }
        }
        public IEnumerator<LengthInfo> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return lenInfos.GetEnumerator();
        }
    }
}
