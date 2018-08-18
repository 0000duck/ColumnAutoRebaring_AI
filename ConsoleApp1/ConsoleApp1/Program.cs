using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ConsoleApp1
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            if (Clipboard.ContainsText())
            {
                Console.Write(Clipboard.GetDataObject().ToString());
            }
            Console.Read();
        }
    }
}
