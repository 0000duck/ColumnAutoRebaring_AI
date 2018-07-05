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
        public int IDColumn { get; set; }
        public int IDLocation { get; set; }
        public LengthType LengthType { get; set; } = LengthType.Residual;
        public LengthPosition LengthPosition { get; set; } = LengthPosition.First;
    }
    public enum LengthType
    {
        Residual, SingleFit, DoubleFit, TrippleFit
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
                        IDLocation = j
                    });
                    if (j == 0) Location1s.Add(Location1s[Location1s.Count - 1] + value - Singleton.Instance.DesignInformations[i + 1].DevelopmentLength);
                    else Location2s.Add(Location2s[Location2s.Count - 1] + value - Singleton.Instance.DesignInformations[i + 1].DevelopmentLength);
                }
            }

            CheckResidual(0);
        }
        private void CheckResidual(int i)
        {
            LengthInfo lenInfo1 = lenInfos.Where(x => x.IDColumn == i && x.IDLocation == 0).First();
            LengthInfo lenInfo2 = lenInfos.Where(x => x.IDColumn == i && x.IDLocation == 1).First();

            if (i == Singleton.Instance.LoopCount - 2)
            {
                // Calculate Residual
                if (lenInfo1.LengthType == LengthType.Residual)
                {
                    Residual += ConstantValue.MaxLength - lenInfo1.Value;
                }
                if (lenInfo2.LengthType == LengthType.Residual)
                {
                    Residual += ConstantValue.MaxLength - lenInfo2.Value;
                }
                return;
            }

            List<int> singleFits = Singleton.Instance.FitChosenInformation.SingleFits;
            List<int> doubleFits = Singleton.Instance.FitChosenInformation.DoubleFits;
            List<int> trippleFits = Singleton.Instance.FitChosenInformation.TrippleFits;

            LengthInfo lenInfo3 = lenInfos.Where(x => x.IDColumn == i + 1 && x.IDLocation == 0).First();
            LengthInfo lenInfo4 = lenInfos.Where(x => x.IDColumn == i + 1 && x.IDLocation == 1).First();

            //#region Column 0 and 1 have same diameter and not implanted
            //if (Singleton.Instance.DesignInformations[i].Diameter == Singleton.Instance.DesignInformations[i + 1].Diameter)
            //{

            //}
            //#endregion

            SetSingleFit(lenInfo1); SetSingleFit(lenInfo2); SetSingleFit(lenInfo3); SetSingleFit(lenInfo4);
            SetDoubleFit(lenInfo1, lenInfo2); SetDoubleFit(lenInfo1, lenInfo3); SetDoubleFit(lenInfo1, lenInfo4); SetDoubleFit(lenInfo2, lenInfo3); SetDoubleFit(lenInfo2, lenInfo4); SetDoubleFit(lenInfo3, lenInfo4);
            SetTrippleFit(lenInfo1, lenInfo2, lenInfo3); SetTrippleFit(lenInfo1, lenInfo2, lenInfo4); SetTrippleFit(lenInfo2, lenInfo3, lenInfo4);

            //// Check SingleFit for Length1
            //if (lenInfo1.LengthType == LengthType.Residual)
            //{
            //    if (singleFits.Contains(lenInfo1.Value))
            //    {
            //        lenInfo1.LengthType = LengthType.SingleFit;
            //        lenInfo1.LengthPosition = LengthPosition.First;
            //    }
            //}

            //// Check SingleFit for Length2
            //if (lenInfo2.LengthType == LengthType.Residual)
            //{
            //    if (singleFits.Contains(lenInfo2.Value))
            //    {
            //        lenInfo2.LengthType = LengthType.SingleFit;
            //        lenInfo2.LengthPosition = LengthPosition.First;
            //    }
            //}

            //// Check SingleFit for Length3
            //if (lenInfo3.LengthType == LengthType.Residual)
            //{
            //    if (singleFits.Contains(lenInfo3.Value))
            //    {
            //        lenInfo3.LengthType = LengthType.SingleFit;
            //        lenInfo3.LengthPosition = LengthPosition.First;
            //    }
            //}

            //// Check SingleFit for Length4
            //if (lenInfo4.LengthType == LengthType.Residual)
            //{
            //    if (singleFits.Contains(lenInfo4.Value))
            //    {
            //        lenInfo4.LengthType = LengthType.SingleFit;
            //        lenInfo4.LengthPosition = LengthPosition.First;
            //    }
            //}

            //// Check DoubleFit for Length1, Length2
            //if (lenInfo1.LengthType == LengthType.Residual && lenInfo2.LengthType == LengthType.Residual)
            //{
            //    if (doubleFits.Contains(lenInfo1.Value + lenInfo2.Value))
            //    {
            //        lenInfo1.LengthType = LengthType.DoubleFit; lenInfo2.LengthType = LengthType.DoubleFit;
            //        lenInfo1.LengthPosition = LengthPosition.First; lenInfo2.LengthPosition = LengthPosition.Second;
            //    }
            //}

            //// Check DoubleFit for Length1, Length3
            //if (lenInfo1.LengthType == LengthType.Residual && lenInfo3.LengthType == LengthType.Residual)
            //{
            //    if (doubleFits.Contains(lenInfo1.Value + lenInfo3.Value))
            //    {
            //        lenInfo1.LengthType = LengthType.DoubleFit; lenInfo3.LengthType = LengthType.DoubleFit;
            //        lenInfo1.LengthPosition = LengthPosition.First; lenInfo3.LengthPosition = LengthPosition.Second;
            //    }
            //}

            //// Check DoubleFit for Length1, Length4
            //if (lenInfo1.LengthType == LengthType.Residual && lenInfo4.LengthType == LengthType.Residual)
            //{
            //    if (doubleFits.Contains(lenInfo1.Value + lenInfo4.Value))
            //    {
            //        lenInfo1.LengthType = LengthType.DoubleFit; lenInfo4.LengthType = LengthType.DoubleFit;
            //        lenInfo1.LengthPosition = LengthPosition.First; lenInfo4.LengthPosition = LengthPosition.Second;
            //    }
            //}

            //// Check DoubleFit for Length2, Length3
            //if (lenInfo2.LengthType == LengthType.Residual && lenInfo3.LengthType == LengthType.Residual)
            //{
            //    if (doubleFits.Contains(lenInfo2.Value + lenInfo3.Value))
            //    {
            //        lenInfo2.LengthType = LengthType.DoubleFit; lenInfo3.LengthType = LengthType.DoubleFit;
            //        lenInfo2.LengthPosition = LengthPosition.First; lenInfo3.LengthPosition = LengthPosition.Second;
            //    }
            //}

            //// Check DoubleFit for Length2, Length4
            //if (lenInfo2.LengthType == LengthType.Residual && lenInfo4.LengthType == LengthType.Residual)
            //{
            //    if (doubleFits.Contains(lenInfo2.Value + lenInfo4.Value))
            //    {
            //        lenInfo2.LengthType = LengthType.DoubleFit; lenInfo4.LengthType = LengthType.DoubleFit;
            //        lenInfo2.LengthPosition = LengthPosition.First; lenInfo4.LengthPosition = LengthPosition.Second;
            //    }
            //}

            //// Check TrippleFit for Length1, Length2, Length3
            //if (lenInfo1.LengthType == LengthType.Residual && lenInfo2.LengthType == LengthType.Residual && lenInfo3.LengthType == LengthType.Residual)
            //{
            //    if (trippleFits.Contains(lenInfo1.Value + lenInfo2.Value + lenInfo3.Value))
            //    {
            //        lenInfo1.LengthType = LengthType.TrippleFit; lenInfo2.LengthType = LengthType.TrippleFit; lenInfo3.LengthType = LengthType.TrippleFit;
            //        lenInfo1.LengthPosition = LengthPosition.First; lenInfo2.LengthPosition = LengthPosition.Second; lenInfo3.LengthPosition = LengthPosition.Third;
            //    }
            //}

            //// Check TrippleFit for Length1, Length2, Length4
            //if (lenInfo1.LengthType == LengthType.Residual && lenInfo2.LengthType == LengthType.Residual && lenInfo4.LengthType == LengthType.Residual)
            //{
            //    if (trippleFits.Contains(lenInfo1.Value + lenInfo2.Value + lenInfo4.Value))
            //    {
            //        lenInfo1.LengthType = LengthType.TrippleFit; lenInfo2.LengthType = LengthType.TrippleFit; lenInfo4.LengthType = LengthType.TrippleFit;
            //        lenInfo1.LengthPosition = LengthPosition.First; lenInfo2.LengthPosition = LengthPosition.Second; lenInfo4.LengthPosition = LengthPosition.Third;
            //    }
            //}

            //// Check TrippleFit for Length1, Length3, Length4
            //if (lenInfo1.LengthType == LengthType.Residual && lenInfo3.LengthType == LengthType.Residual && lenInfo4.LengthType == LengthType.Residual)
            //{
            //    if (trippleFits.Contains(lenInfo1.Value + lenInfo3.Value + lenInfo4.Value))
            //    {
            //        lenInfo1.LengthType = LengthType.TrippleFit; lenInfo3.LengthType = LengthType.TrippleFit; lenInfo4.LengthType = LengthType.TrippleFit;
            //        lenInfo1.LengthPosition = LengthPosition.First; lenInfo3.LengthPosition = LengthPosition.Second; lenInfo4.LengthPosition = LengthPosition.Third;
            //    }
            //}

            //// Check TrippleFit for Length2, Length3, Length4
            //if (lenInfo2.LengthType == LengthType.Residual && lenInfo3.LengthType == LengthType.Residual && lenInfo4.LengthType == LengthType.Residual)
            //{
            //    if (trippleFits.Contains(lenInfo2.Value + lenInfo3.Value + lenInfo4.Value))
            //    {
            //        lenInfo2.LengthType = LengthType.TrippleFit; lenInfo3.LengthType = LengthType.TrippleFit; lenInfo4.LengthType = LengthType.TrippleFit;
            //        lenInfo2.LengthPosition = LengthPosition.First; lenInfo3.LengthPosition = LengthPosition.Second; lenInfo4.LengthPosition = LengthPosition.Third;
            //    }
            //}

            // Calculate Residual
            if (lenInfo1.LengthType == LengthType.Residual)
            {
                Residual += ConstantValue.MaxLength - lenInfo1.Value;
            }
            if (lenInfo2.LengthType == LengthType.Residual)
            {
                Residual += ConstantValue.MaxLength - lenInfo2.Value;
            }

            // Next Level Column
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
