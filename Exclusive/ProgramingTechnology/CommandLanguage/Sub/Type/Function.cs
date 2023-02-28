using mshtml;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiMFa.Exclusive.ProgramingTechnology.CommandLanguage
{
    [Serializable]
    public class Function
    {
        public string Name = Default.Time.LengthSecond.ToString();
        public string[] Inputs = new string[0];
        public string Commands = "";
        public Accessibility Access;
        public Function()
        {
        }
        public override string ToString()
        {
            return "";
        }

    }
}
