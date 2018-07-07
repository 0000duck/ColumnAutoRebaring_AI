using HandleDatabse.ProjectData.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandleDatabse.ProjectData.Dao
{
    public static class LengthInfoCollectionDao
    {
        public static void GetLengthInfoCollection()
        {
            int residual = Singleton.Instance.LengthInfoCollections[0].Residual;
            int residual2Temp = Singleton.Instance.LengthInfoCollections[0].Residual2;
            int residual2 = Singleton.Instance.LengthInfoCollections[0].Residual2;

            int indexI = 0, indexJ = 0;

            for (int i = 0; i < Singleton.Instance.LengthInfoCollections.Count; i++)
            {
                if (residual > Singleton.Instance.LengthInfoCollections[i].Residual)
                {
                    indexI = i;
                    residual = Singleton.Instance.LengthInfoCollections[i].Residual;
                    residual2Temp = Singleton.Instance.LengthInfoCollections[i].Residual2;
                }
                else if (residual == Singleton.Instance.LengthInfoCollections[i].Residual)
                {
                    if (residual2Temp > Singleton.Instance.LengthInfoCollections[i].Residual2)
                    {
                        indexI = i;
                        residual2Temp = Singleton.Instance.LengthInfoCollections[i].Residual2;
                    }
                }

                if (residual2 > Singleton.Instance.LengthInfoCollections[i].Residual2)
                {
                    indexJ = i;
                    residual2 = Singleton.Instance.LengthInfoCollections[i].Residual2;
                }
            }

            Singleton.Instance.ResidualLengthInfoCollection = Singleton.Instance.LengthInfoCollections[indexI];
            Singleton.Instance.Residual2LengthInfoCollection = Singleton.Instance.LengthInfoCollections[indexJ];
        }
    }
}
