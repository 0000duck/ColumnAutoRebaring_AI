using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAddin1
{
    public class TimelineDao
    {
        public static List<int> GetTimelines(string prefix, RenderDbContext db = null)
        {
            if (db == null) db = new RenderDbContext();
            return db.Timelines.Where(x=> x.Prefix == prefix).Select(x => x.Date).OrderBy(x => x).ToList();
        }
        public static void Update(RenderDbContext db = null)
        {
            if (db == null) db = new RenderDbContext();
            var newTimelines = SingleWPF.Instance.Timelines;

            db.Timelines.RemoveRange(db.Timelines);

            for (int i = 0; i < newTimelines.Count; i++)
            {
                //if (i <= oldTimelines.Count - 1)
                //{
                //    var timeline = db.Timelines.Where(x=> x.ID ==indexes[i]).First();
                //    timeline = new Timeline()
                //    {
                //        IDProject=1,
                //        Date = newTimelines[i]
                //    };
                //}
                //else
                //{
                db.Timelines.Add(new Timeline()
                {
                    CreateDate = DateTime.Now,
                    IDProject = 1,
                    Prefix = SingleWPF.Instance.Prefix,
                    Date = newTimelines[i]
                });
                //}
            }

            db.SaveChanges();
        }
    }
}
