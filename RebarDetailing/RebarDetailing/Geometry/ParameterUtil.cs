using System;
using System.Collections.Generic;
using System.IO;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using System.Linq;

namespace Geometry
{
    /// <summary>
    /// Kiểu dữ liệu chưa các phương thức tĩnh là công cụ để hiệu chỉnh và thêm các tham biến các đối tượng
    /// </summary>
    public class ParameterUtil
    {
        /// <summary>
        /// Thêm tham biến cho các đối tượng thuộc tập hợp các category xác định trong document đang xét
        /// </summary>
        /// <param name="doc">Document đang xét</param>
        /// <param name="name">Tên tham biến</param>
        /// <param name="addType">Loại tham biến đang xét (Instance: thể hiện, Type: loại)</param>
        /// <param name="type">Kiểu giá trị của tham biến (integer, text, area ...)</param>
        /// <param name="parameterGroup">Nhóm tham biến sẽ chứa tham biến đang xét</param>
        /// <param name="builtInCategories">Tập hợp các BuiltInCategory xác định tập hợp các category</param>
        public static void AddParameter(Document doc, string name, AddParameterType addType, ParameterType type, BuiltInParameterGroup parameterGroup, List<BuiltInCategory> builtInCategories)
        {
            Application app = doc.Application;
            FilteredElementCollector col = new FilteredElementCollector(doc);
            List<Element> elems = new List<Element>();
            List<int> bics = builtInCategories.Select(x => (int)x).ToList();
            switch (addType)
            {
                case AddParameterType.Instance:
                    {
                        List<Element> elems1 = col.WhereElementIsNotElementType().Where(x => x != null).Where(x => x.Category != null).Where(x => bics.Contains(x.Category.Id.IntegerValue)).ToList();
                        foreach (int bic in bics)
                        {
                            List<Element> elems2 = elems1.Where(x => x.Category.Id.IntegerValue == bic).ToList();
                            if (elems2.Count != 0)
                            {
                                elems.Add(elems2.First());
                            }
                        }
                        break;
                    }
                case AddParameterType.Type:
                    {
                        List<Element> elems1 = col.WhereElementIsElementType().Where(x => x != null).Where(x => x.Category != null).Where(x => bics.Contains(x.Category.Id.IntegerValue)).ToList();
                        foreach (int bic in bics)
                        {
                            List<Element> elems2 = elems1.Where(x => x.Category.Id.IntegerValue == bic).ToList();
                            if (elems2.Count != 0)
                            {
                                elems.Add(elems2.First());
                            }
                        }
                        break;
                    }
            }
            CategorySet cates = null;
            Definition def = null;
            foreach (Element e in elems)
            {
                foreach (Parameter p in e.Parameters)
                {
                    try
                    {
                        if (p.Definition.Name == name)
                        {
                            def = p.Definition; goto L1;
                        }
                    }
                    catch { }
                }
            }
            L1:
            if (def != null)
            {
                switch (addType)
                {
                    case AddParameterType.Instance:
                        InstanceBinding ib = doc.ParameterBindings.get_Item(def) as InstanceBinding;
                        if (ib == null) throw new Exception("Error when retrieve instance binding!");
                        cates = ib.Categories; break;
                    case AddParameterType.Type:
                        TypeBinding tb = doc.ParameterBindings.get_Item(def) as TypeBinding;
                        if (tb == null) throw new Exception("Error when retrieve type binding!");
                        cates = tb.Categories; break;
                }
                if (cates == null) throw new Exception("Error when retrieve categories!");
                foreach (BuiltInCategory bic in builtInCategories)
                {
                    Category c = Category.GetCategory(doc, bic);
                    if (!cates.Contains(c))
                    {
                        cates.Insert(c);
                    }
                }
                switch (addType)
                {
                    case AddParameterType.Instance:
                        doc.ParameterBindings.ReInsert(def, app.Create.NewInstanceBinding(cates), def.ParameterGroup);
                        return;
                    case AddParameterType.Type:
                        doc.ParameterBindings.ReInsert(def, app.Create.NewTypeBinding(cates), def.ParameterGroup);
                        return;
                }
                throw new Exception("Code complier should never be here!");
            }

            string strFileTemp = Path.Combine(Path.GetTempPath(), "temp" + "_AssemblyParameter.txt");
            using (File.Create(strFileTemp)) { }
            app.SharedParametersFilename = strFileTemp;
            DefinitionFile sharedFile = app.OpenSharedParameterFile();
            DefinitionGroups defGroups = sharedFile.Groups;
            DefinitionGroup defGroup = defGroups.Create(name);
            def = defGroup.Definitions.Create(new ExternalDefinitionCreationOptions(name, type) { Visible = true, UserModifiable = true, Description = name });
            cates = app.Create.NewCategorySet();
            foreach (BuiltInCategory bic in builtInCategories)
            {
                Category c = Category.GetCategory(doc, bic);
                cates.Insert(c);
            }
            switch (addType)
            {
                case AddParameterType.Instance:
                    doc.ParameterBindings.Insert(def, app.Create.NewInstanceBinding(cates), parameterGroup);
                    return;
                case AddParameterType.Type:
                    doc.ParameterBindings.Insert(def, app.Create.NewTypeBinding(cates), parameterGroup);
                    return;
            }
            File.Delete(strFileTemp);
        }

        /// <summary>
        /// Thêm tham biến cho các đối tượng thuộc tập hợp các category xác định trong document đang xét
        /// </summary>
        /// <param name="doc">Document đang xét</param>
        /// <param name="name">Tên tham biến</param>
        /// <param name="addType">Loại tham biến đang xét (Instance: thể hiện, Type: loại)</param>
        /// <param name="type">Kiểu giá trị của tham biến (integer, text, area ...)</param>
        /// <param name="parameterGroup">Nhóm tham biến sẽ chứa tham biến đang xét</param>
        /// <param name="categories">Tập hợp các category đang xét</param>
        public static void AddParameter(Document doc, string name, AddParameterType addType, ParameterType type, BuiltInParameterGroup parameterGroup, List<Category> categories)
        {
            Application app = doc.Application;
            FilteredElementCollector col = new FilteredElementCollector(doc);
            List<Element> elems = new List<Element>();
            List<int> bics = categories.Select(x => x.Id.IntegerValue).ToList();
            switch (addType)
            {
                case AddParameterType.Instance:
                    {
                        List<Element> elems1 = col.WhereElementIsNotElementType().Where(x => x != null).Where(x => x.Category != null).Where(x => bics.Contains(x.Category.Id.IntegerValue)).ToList();
                        foreach (int bic in bics)
                        {
                            List<Element> elems2 = elems1.Where(x => x.Category.Id.IntegerValue == bic).ToList();
                            if (elems2.Count != 0)
                            {
                                elems.Add(elems2.First());
                            }
                        }
                        break;
                    }
                case AddParameterType.Type:
                    {
                        List<Element> elems1 = col.WhereElementIsElementType().Where(x => x != null).Where(x => x.Category != null).Where(x => bics.Contains(x.Category.Id.IntegerValue)).ToList();
                        foreach (int bic in bics)
                        {
                            List<Element> elems2 = elems1.Where(x => x.Category.Id.IntegerValue == bic).ToList();
                            if (elems2.Count != 0)
                            {
                                elems.Add(elems2.First());
                            }
                        }
                        break;
                    }
            }
            CategorySet cates = null;
            Definition def = null;
            foreach (Element e in elems)
            {
                foreach (Parameter p in e.Parameters)
                {
                    try
                    {
                        if (p.Definition.Name == name)
                        {
                            def = p.Definition; goto L1;
                        }
                    }
                    catch { }
                }
            }
            L1:
            if (def != null)
            {
                switch (addType)
                {
                    case AddParameterType.Instance:
                        InstanceBinding ib = doc.ParameterBindings.get_Item(def) as InstanceBinding;
                        if (ib == null) throw new Exception("Error when retrieve instance binding!");
                        cates = ib.Categories; break;
                    case AddParameterType.Type:
                        TypeBinding tb = doc.ParameterBindings.get_Item(def) as TypeBinding;
                        if (tb == null) throw new Exception("Error when retrieve type binding!");
                        cates = tb.Categories; break;
                }
                if (cates == null) throw new Exception("Error when retrieve categories!");
                foreach (Category c in categories)
                {
                    if (!cates.Contains(c))
                    {
                        cates.Insert(c);
                    }
                }
                switch (addType)
                {
                    case AddParameterType.Instance:
                        doc.ParameterBindings.ReInsert(def, app.Create.NewInstanceBinding(cates), def.ParameterGroup);
                        return;
                    case AddParameterType.Type:
                        doc.ParameterBindings.ReInsert(def, app.Create.NewTypeBinding(cates), def.ParameterGroup);
                        return;
                }
                throw new Exception("Code complier should never be here!");
            }

            string strFileTemp = Path.Combine(Path.GetTempPath(), "temp" + "_AssemblyParameter.txt");
            using (File.Create(strFileTemp)) { }
            app.SharedParametersFilename = strFileTemp;
            DefinitionFile sharedFile = app.OpenSharedParameterFile();
            DefinitionGroups defGroups = sharedFile.Groups;
            DefinitionGroup defGroup = defGroups.Create(name);
            def = defGroup.Definitions.Create(new ExternalDefinitionCreationOptions(name, type) { Visible = true, UserModifiable = true, Description = name });
            cates = app.Create.NewCategorySet();
            foreach (Category c in categories)
            {
                cates.Insert(c);
            }
            switch (addType)
            {
                case AddParameterType.Instance:
                    doc.ParameterBindings.Insert(def, app.Create.NewInstanceBinding(cates), parameterGroup);
                    return;
                case AddParameterType.Type:
                    doc.ParameterBindings.Insert(def, app.Create.NewTypeBinding(cates), parameterGroup);
                    return;
            }
            File.Delete(strFileTemp);
        }

        /// <summary>
        /// Thêm tham biến cho các đối tượng thuộc tập hợp các category xác định trong document đang xét
        /// </summary>
        /// <param name="doc">Document đang xét</param>
        /// <param name="name">Tên tham biến</param>
        /// <param name="addType">Loại tham biến đang xét (Instance: thể hiện, Type: loại)</param>
        /// <param name="type">Kiểu giá trị của tham biến (integer, text, area ...)</param>
        /// <param name="parameterGroup">Nhóm tham biến sẽ chứa tham biến đang xét</param>
        /// <param name="categories">CategorySet chứa tập hợp các category đang xé</param>
        public static void AddParameter(Document doc, string name, AddParameterType addType, ParameterType type, BuiltInParameterGroup parameterGroup, CategorySet categories)
        {
            Application app = doc.Application;
            FilteredElementCollector col = new FilteredElementCollector(doc);
            List<Element> elems = new List<Element>();
            switch (addType)
            {
                case AddParameterType.Instance:
                    {
                        List<Element> elems1 = col.WhereElementIsNotElementType().Where(x => x != null).Where(x => x.Category != null).Where(x => categories.Contains(x.Category)).ToList();
                        foreach (Category c in categories)
                        {
                            List<Element> elems2 = elems1.Where(x => x.Category.Id.IntegerValue == c.Id.IntegerValue).ToList();
                            if (elems2.Count != 0)
                            {
                                elems.Add(elems2.First());
                            }
                        }
                        break;
                    }
                case AddParameterType.Type:
                    {
                        List<Element> elems1 = col.WhereElementIsElementType().Where(x => x != null).Where(x => x.Category != null).Where(x => categories.Contains(x.Category)).ToList();
                        foreach (Category c in categories)
                        {
                            List<Element> elems2 = elems1.Where(x => x.Category.Id.IntegerValue == c.Id.IntegerValue).ToList();
                            if (elems2.Count != 0)
                            {
                                elems.Add(elems2.First());
                            }
                        }
                        break;
                    }
            }
            CategorySet cates = null;
            Definition def = null;
            foreach (Element e in elems)
            {
                foreach (Parameter p in e.Parameters)
                {
                    try
                    {
                        if (p.Definition.Name == name)
                        {
                            def = p.Definition; goto L1;
                        }
                    }
                    catch { }
                }
            }
            L1:
            if (def != null)
            {
                switch (addType)
                {
                    case AddParameterType.Instance:
                        InstanceBinding ib = doc.ParameterBindings.get_Item(def) as InstanceBinding;
                        if (ib == null) throw new Exception("Error when retrieve instance binding!");
                        cates = ib.Categories; break;
                    case AddParameterType.Type:
                        TypeBinding tb = doc.ParameterBindings.get_Item(def) as TypeBinding;
                        if (tb == null) throw new Exception("Error when retrieve type binding!");
                        cates = tb.Categories; break;
                }
                if (cates == null) throw new Exception("Error when retrieve categories!");
                foreach (Category c in categories)
                {
                    if (!cates.Contains(c))
                    {
                        cates.Insert(c);
                    }
                }
                switch (addType)
                {
                    case AddParameterType.Instance:
                        doc.ParameterBindings.ReInsert(def, app.Create.NewInstanceBinding(cates), def.ParameterGroup);
                        return;
                    case AddParameterType.Type:
                        doc.ParameterBindings.ReInsert(def, app.Create.NewTypeBinding(cates), def.ParameterGroup);
                        return;
                }
                throw new Exception("Code complier should never be here!");
            }

            string strFileTemp = Path.Combine(Path.GetTempPath(), "temp" + "_AssemblyParameter.txt");
            using (File.Create(strFileTemp)) { }
            app.SharedParametersFilename = strFileTemp;
            DefinitionFile sharedFile = app.OpenSharedParameterFile();
            DefinitionGroups defGroups = sharedFile.Groups;
            DefinitionGroup defGroup = defGroups.Create(name);
            def = defGroup.Definitions.Create(new ExternalDefinitionCreationOptions(name, type) { Visible = true, UserModifiable = true, Description = name });
            cates = app.Create.NewCategorySet();
            foreach (Category c in categories)
            {
                cates.Insert(c);
            }
            switch (addType)
            {
                case AddParameterType.Instance:
                    doc.ParameterBindings.Insert(def, app.Create.NewInstanceBinding(cates), parameterGroup);
                    return;
                case AddParameterType.Type:
                    doc.ParameterBindings.Insert(def, app.Create.NewTypeBinding(cates), parameterGroup);
                    return;
            }
            File.Delete(strFileTemp);
        }
    }

    /// <summary>
    /// Kiểu dữ liệu enum chứa thông tin loại tham biến đang xét (Instance: thể hiện, Type: loại)
    /// </summary>
    public enum AddParameterType
    {
        Instance, Type
    }
}
