using Autodesk.Revit.DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RevitAddin1
{
    static class CategoryDao
    {
        public static List<Category> GetCategories()
        {
            List<Category> cates = new List<Category>();
            Categories categories = Singleton.Instance.Document.Settings.Categories;
            foreach (Category cate in categories)
            {
                cates.Add(cate);
            }
            return cates;
        }
    }
}
