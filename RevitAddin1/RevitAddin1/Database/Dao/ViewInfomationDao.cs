using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAddin1
{
    public class ViewInfomationDao
    {
        public static List<ViewInformation> GetViewInfomations(RenderDbContext db = null)
        {
            if (db == null) db = new RenderDbContext();
            var obj = db.ViewInformations.ToList();
            obj.Sort(new ViewInformationSorter());
            return obj;
        }
    }

    public class ViewInformationSorter : IComparer<ViewInformation>
    {
        public int Compare(ViewInformation x, ViewInformation y)
        {
            return x.Name.CompareTo(y.Name);
        }
    }
}
