using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pyRevitTest
{
    public class Singleton
    {
        #region Variables
        private ScriptEngine scriptEngine;
        private ScriptScope scriptScope;
        private ScriptSource scriptSource;
        private string[] sourceString;
        #endregion

        #region Properties
        public static Singleton Instance { get; set; }
        public ScriptEngine ScriptEngine
        {
            get
            {
                if (scriptEngine == null) scriptEngine = Python.CreateEngine();
                return scriptEngine;
            }
        }
        public ScriptScope ScriptScope
        {
            get
            {
                if (scriptScope == null)
                {
                    scriptScope = ScriptEngine.CreateScope();
                    Python.CreateRuntime();
                }
                return scriptScope;
            }
        }
        public ScriptSource ScriptSource
        {
            get
            {
                if (scriptSource == null)
                {
                    string pySrc;
                    using (var stream = System.Reflection.Assembly.GetExecutingAssembly().
                        GetManifestResourceStream($"{ConstantValue.NameSpace}.{ConstantValue.PythonTextName}.{ConstantValue.PythonSuffix}"))
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            pySrc = reader.ReadToEnd();
                        }
                    }
                    scriptSource = ScriptEngine.CreateScriptSourceFromString(pySrc);
                }
                return scriptSource;
            }
        }
        public string[] SourceString
        {
            get
            {
                if (sourceString == null)
                {
                    string pySrc;
                    using (var stream = System.Reflection.Assembly.GetExecutingAssembly().
                        GetManifestResourceStream($"{ConstantValue.NameSpace}.{ConstantValue.PythonTextName}.{ConstantValue.PythonSuffix}"))
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            pySrc = reader.ReadToEnd();
                            sourceString = pySrc.Split('\n');
                        }
                    }
                }
                return sourceString;
            }
        }
        #endregion
    }
}
