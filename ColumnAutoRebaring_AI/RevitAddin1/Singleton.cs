using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace RevitAddin1
{
    public class Singleton : INotifyPropertyChanged
    {
        private static Singleton instance;
        public static Singleton Instance
        {
            get
            {
                if (instance == null) instance = new Singleton();
                return instance;
            }
        }
        private Input input;
        public Input Input
        {
            get
            {
                if (input == null)
                {
                    input = new Input();
                    input.Left = 250;
                    input.Top = 220;
                }
                return input;
            }
        }
        public string InputParameter { get; set; } = "T";

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
