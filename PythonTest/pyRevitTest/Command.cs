using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pyRevitTest
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Singleton.Instance = new Singleton();

            Form frm = new Form();
            Singleton.Instance.ScriptScope.SetVariable("commandData", commandData);
            Singleton.Instance.ScriptScope.SetVariable("frm", frm);

            //for (int i = 0; i < Singleton.Instance.SourceString.Length; i++)
            //{
            //    try
            //    {
            //        Singleton.Instance.ScriptEngine.Execute(Singleton.Instance.SourceString[i], Singleton.Instance.ScriptScope);
            //    }
            //    catch (Exception ex)
            //    {
            //        string mess = $"Error at python code line: {i + 1}\n{ex.Message}";
            //        throw new Exception(mess);
            //    }
            //}

            Singleton.Instance.ScriptSource.Execute(Singleton.Instance.ScriptScope);


            return Result.Succeeded;
        }
    }
}
