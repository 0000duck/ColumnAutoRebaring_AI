using HandleDatabse.Database.Dao;
using HandleDatabse.Database.EF;
using HandleDatabse.ProjectData.EF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HandleDatabse.ProjectData.Dao
{
    public static class LengthChosenDao
    {
        public static void InsertLengthChosens(int i, LengthChosen befLenChosen = null)
        {
            int numLens = Singleton.Instance.Lengths.Count;
            if (i == 0)
            {
                for (int i1 = 0; i1 < numLens; i1++)
                {
                    for (int i2 = 0; i2 < numLens; i2++)
                    {
                        LengthChosen lenChosen = new LengthChosen(i1, i2);
                        if (lenChosen.IsValid)
                        {
                            if (lenChosen.IsFinish)
                                Singleton.Instance.LengthChosens.Add(lenChosen);
                            //Console.WriteLine($"Tầng thứ {i}: Sử dụng: {lenChosen.L1} và {lenChosen.L2}");
                            InsertLengthChosens(i + 1, lenChosen);
                        }
                    }
                }
            }
            else if (i < Singleton.Instance.LoopCount - 1)
            {
                for (int i1 = 0; i1 < numLens; i1++)
                {
                    for (int i2 = 0; i2 < numLens; i2++)
                    {
                        LengthChosen lenChosen = new LengthChosen(befLenChosen, i1, i2);
                        if (lenChosen.IsValid)
                        {
                            if (lenChosen.IsFinish)
                                Singleton.Instance.LengthChosens.Add(lenChosen);
                            //for (int j = 0; j < i; j++)
                            //{
                            //    Console.Write("\t");
                            //}
                            //Console.WriteLine($"Tầng thứ {i}: Sử dụng: {lenChosen.L1} và {lenChosen.L2}");
                            InsertLengthChosens(i + 1, lenChosen);
                        }
                    }
                }
            }
            else
            {
                return;
            }
        }
    }
}
