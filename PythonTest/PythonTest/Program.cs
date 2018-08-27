using IronPython.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PythonTest
{
    class Program
    {
        static void Main(string[] args)
        {

            var py = Python.CreateEngine();
            var s = py.CreateScope();
            var ironPythonRunetime = Python.CreateRuntime();

            s.SetVariable("x", 3);

            var ss = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceNames();

            string pySrc;
            using (var steam = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("PythonTest.TextFile1.py"))
            {
                using (var reader = new System.IO.StreamReader(steam))
                {
                    pySrc = reader.ReadToEnd();
                }
            }

            var x = py.CreateScriptSourceFromString(pySrc);
            x.Execute(s);

            var myFoo = s.GetVariable("x");
            Console.WriteLine(myFoo);
            Console.Read();
        }
    }
}
