using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAddin1
{
    public class WPFViewInfomationDao
    {
        public static void GetViewInfomations()
        {
            SingleWPF.Instance.ViewInformations.Clear();
            ViewInfomationDao.GetViewInfomations().ForEach(x => SingleWPF.Instance.ViewInformations.Add(x));
        }
    }
}
