using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Project
{
    public class OtherData
    {
        #region Variables
        private InputForm inputForm;
        #endregion

        #region Properties
        public InputForm InputForm
        {
            get
            {
                if (inputForm == null)
                {
                    inputForm = new InputForm() { DataContext = Singleton.Instance.WPFData };
                }
                return inputForm;
            }
        }
        #endregion
    }
}
