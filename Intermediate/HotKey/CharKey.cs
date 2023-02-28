using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiMFa.Intermediate.HotKey
{
    /// <summary>
    /// Event Args for the event that is fired after the hot key has been pressed.
    /// </summary>
    public class CharKey : EventArgs
    {
        public char @Char { get; internal set; }
        public bool @Control { get; internal set; } = false;
        public bool Alt { get; internal set; } = false;
        public bool Shift { get; internal set; } = false;

        public CharKey(char @char, bool control = false, bool alt = false, bool shift = false)
        {
            Char = @char;
            @Control = control;
            Alt = alt;
            Shift = shift;
        }
    }

}
