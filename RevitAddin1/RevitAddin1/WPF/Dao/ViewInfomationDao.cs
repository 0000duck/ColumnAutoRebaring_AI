using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAddin1
{
    public class ViewInfomationDao
    {
        public static void GetViewInfomations()
        {
            SingleWPF.Instance.ViewInformations.Clear();
            for (int i = 0; i < SingleWPF.Instance.Timelines.Count - 1; i++)
            {
                SingleWPF.Instance.ViewInformations.Add(new ViewInformation()
                {
                    Name = $"V{SingleWPF.Instance.Prefix}_{i + 1}",
                    FromDate = SingleWPF.Instance.Timelines[i],
                    ToDate = SingleWPF.Instance.Timelines[i + 1] - (i + 1 == SingleWPF.Instance.Timelines.Count - 1 ? 0 : 1)
                });
            }
        }
    }
}
