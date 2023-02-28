using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiMFa.Intermediate.HotKey
{
    /// <summary>
    /// The enumeration of possible modifiers.
    /// </summary>
    [Flags]
    public enum ModifierKeys : uint
    {
        Alt = 1,
        Control = 2,
        Shift = 4,
        Win = 8
    }

    /// <summary>
    /// Event Args for the event that is fired after the hot key has been pressed.
    /// </summary>
    public class KeyEventArgs : EventArgs
    {
        private ModifierKeys _modifier;
        private Keys _key;
        public ModifierKeys Modifier
        {
            get { return _modifier; }
        }
        public Keys Key
        {
            get { return _key; }
        }

        internal KeyEventArgs(ModifierKeys modifier, Keys key)
        {
            _modifier = modifier;
            _key = key;
        }
    }

}
