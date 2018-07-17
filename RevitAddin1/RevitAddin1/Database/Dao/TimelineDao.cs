using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAddin1
{
    public class TimelineDao
    {
        public static List<int> GetTimelines(RenderDbContext db = null)
        {
            if (db == null) db = new RenderDbContext();
            return db.Timelines.Select(x => x.Date).OrderBy(x=>x).ToList();
        }
    }
}
