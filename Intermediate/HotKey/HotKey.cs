using MiMFa.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiMFa.Intermediate.HotKey
{
    public sealed class HotKey : IDisposable
    {
        // Registers a hot key with Windows.
        [DllImport("user32.dll")]
        public static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);
        // Unregisters the hot key with Windows.
        [DllImport("user32.dll")]
        public static extern bool UnregisterHotKey(IntPtr hWnd, int id);


        private InternalWindow _window = new InternalWindow();
        private int _currentId;

        /// <summary>
        /// A hot key has been pressed.
        /// </summary>
        public event EventHandler<KeyEventArgs> KeyPressed;

        public HotKey()
        {
            // register the event of the inner native window.
            _window.KeyPressed += delegate (object sender, KeyEventArgs args)
            {
                if (KeyPressed != null)
                    KeyPressed(this, args);
            };
        }

        public string ToString(ModifierKeys modifier)
        {
            switch (modifier)
            {
                case ModifierKeys.Alt:
                    return "%";
                case ModifierKeys.Control:
                    return "^";
                case ModifierKeys.Shift:
                    return "+";
                case ModifierKeys.Win:
                    return "{HOME}";
                case ModifierKeys.Control | ModifierKeys.Shift:
                    return "^+";
                case ModifierKeys.Control | ModifierKeys.Shift | ModifierKeys.Alt:
                    return "^+%";
                case ModifierKeys.Control | ModifierKeys.Alt:
                    return "^%";
                case ModifierKeys.Shift | ModifierKeys.Alt:
                    return "+%";
            }
            return "";
        }
        public string ToString(Keys key)
        {
            switch (key)
            {
                case Keys.Back:
                    return "{BACKSPACE}";
                case Keys.Separator:
                    return "{BREAK}";
                case Keys.CapsLock:
                    return "{CAPSLOCK}";
                case Keys.Delete:
                    return "{DELETE}";
                case Keys.Down:
                    return "{DOWN}";
                case Keys.End:
                    return "{END}";
                case Keys.Enter:
                    return "{ENTER}";
                case Keys.Escape:
                    return "{ESC}";
                case Keys.Help:
                    return "{HELP}";
                case Keys.Home:
                    return "{HOME}";
                case Keys.Insert:
                    return "{INSERT}";
                case Keys.Left:
                    return "{LEFT}";
                case Keys.NumLock:
                    return "{NUMLOCK}";
                case Keys.PageDown:
                    return "{PGDN}";
                case Keys.PageUp:
                    return "{PGUP}";
                case Keys.PrintScreen:
                    return "{PRTSC}";
                case Keys.Right:
                    return "{RIGHT}";
                case Keys.Scroll:
                    return "{SCROLLLOCK}";
                case Keys.Tab:
                    return "{TAB}";
                case Keys.Up:
                    return "{UP}";
                case Keys.F1:
                    return "{F1}";
                case Keys.F2:
                    return "{F2}";
                case Keys.F3:
                    return "{F3}";
                case Keys.F4:
                    return "{F4}";
                case Keys.F5:
                    return "{F5}";
                case Keys.F6:
                    return "{F6}";
                case Keys.F7:
                    return "{F7}";
                case Keys.F8:
                    return "{F8}";
                case Keys.F9:
                    return "{F9}";
                case Keys.F10:
                    return "{F10}";
                case Keys.F11:
                    return "{F11}";
                case Keys.F12:
                    return "{F12}";
                case Keys.F13:
                    return "{F13}";
                case Keys.F14:
                    return "{F14}";
                case Keys.F15:
                    return "{F15}";
                case Keys.F16:
                    return "{F16}";
                case Keys.Add:
                    return "{ADD}";
                case Keys.Subtract:
                    return "{SUBTRACT}";
                case Keys.Multiply:
                    return "{MULTIPLY}";
                case Keys.Divide:
                    return "{DIVIDE}";
                case Keys.ShiftKey:
                    return "+";
                case Keys.ControlKey:
                    return "^";
                case Keys.Alt:
                    return "%";

            }
            return key.ToString();
        }
        public string ToString(IEnumerable<Keys> keys)
        {
            return string.Join("", from v in keys select ToString(v));
        }
        public string ToString(ModifierKeys modifier, Keys key)
        {
            return ToString(ToString(modifier), ToString(key));
        }
        public string ToString(ModifierKeys modifier, IEnumerable<Keys> keys)
        {
            return ToString(ToString(modifier), ToString(keys));
        }
        public string ToString(ModifierKeys modifier, string keys)
        {
            return string.Join("", ToString(modifier), "(", keys, ")");
        }
        public string ToString(string modifier, string keys)
        {
            return string.Join("",modifier,"(",keys,")");
        }

        public void Send(params string[] keys)
        {
            foreach (var item in keys)
                SendKeys.Send(item);
        }
        public void Send(ModifierKeys modifier, params string[] keys)
        {
            foreach (var item in keys)
                Send(((char)modifier) + item);
        }
        public void Send(params char[] keys)
        {
            Send(string.Join("", keys));
        }
        public void Send(ModifierKeys modifier, params char[] keys)
        {
            foreach (var item in keys)
                Send(modifier, string.Join("", keys));
        }
        public void Send(params Keys[] keys)
        {
            Send(ToString(keys));
        }
        public void Send(ModifierKeys modifier, params Keys[] keys)
        {
            Send(ToString(modifier, keys));
        }

        public void Send(IntPtr pointer, params string[] keys)
        {
            ProcessService.SetForegroundWindow(pointer);
            foreach (var item in keys)
                SendKeys.Send(item);
        }
        public void Send(IntPtr pointer, ModifierKeys modifier, params string[] keys)
        {
            ProcessService.SetForegroundWindow(pointer);
            foreach (var item in keys)
                SendKeys.Send(((char)modifier)+item);
        }
        public void Send(IntPtr pointer, params char[] keys)
        {
            Send(pointer, string.Join("", keys));
        }
        public void Send(IntPtr pointer, ModifierKeys modifier, params char[] keys)
        {
            Send(pointer, modifier, string.Join("", keys));
        }
        public void Send(IntPtr pointer, params Keys[] keys)
        {
            Send(pointer, ToString(keys));
        }
        public void Send(IntPtr pointer, ModifierKeys modifier, params Keys[] keys)
        {
            Send(pointer, ToString(modifier, keys));
        }

        public void Send(System.Diagnostics.Process process, params string[] keys)
        {
            Send(process.Handle, keys);
        }
        public void Send(System.Diagnostics.Process process, ModifierKeys modifier, params string[] keys)
        {
            Send(process.Handle, modifier, keys);
        }
        public void Send(System.Diagnostics.Process process, params char[] keys)
        {
            Send(process, string.Join("", keys));
        }
        public void Send(System.Diagnostics.Process process, ModifierKeys modifier, params char[] keys)
        {
            Send(process, modifier, string.Join("", keys));
        }
        public void Send(System.Diagnostics.Process process, params Keys[] keys)
        {
            Send(process, ToString(keys));
        }
        public void Send(System.Diagnostics.Process process, ModifierKeys modifier, params Keys[] keys)
        {
            Send(process, ToString(modifier, keys));
        }

        public void SendWait(params string[] keys)
        {
            foreach (var item in keys)
                SendKeys.SendWait(item);
        }
        public void SendWait(ModifierKeys modifier, params string[] keys)
        {
            foreach (var item in keys)
                SendWait(((char)modifier) + item);
        }
        public void SendWait(params char[] keys)
        {
            SendWait(string.Join("", keys));
        }
        public void SendWait(ModifierKeys modifier, params char[] keys)
        {
            foreach (var item in keys)
                SendWait(modifier, string.Join("", keys));
        }
        public void SendWait(params Keys[] keys)
        {
            SendWait(ToString(keys));
        }
        public void SendWait(ModifierKeys modifier, params Keys[] keys)
        {
            SendWait(ToString(modifier, keys));
        }


        public void SendWait(IntPtr pointer, params string[] keys)
        {
            ProcessService.SetForegroundWindow(pointer);
            foreach (var item in keys)
                SendKeys.SendWait(item);
        }
        public void SendWait(IntPtr pointer, ModifierKeys modifier, params string[] keys)
        {
            ProcessService.SetForegroundWindow(pointer);
            foreach (var item in keys)
                SendKeys.SendWait(((char)modifier)+item);
        }
        public void SendWait(IntPtr pointer, params char[] keys)
        {
            SendWait(pointer, string.Join("", keys));
        }
        public void SendWait(IntPtr pointer, ModifierKeys modifier, params char[] keys)
        {
            SendWait(pointer, modifier, string.Join("", keys));
        }
        public void SendWait(IntPtr pointer, params Keys[] keys)
        {
            SendWait(pointer, ToString(keys));
        }
        public void SendWait(IntPtr pointer, ModifierKeys modifier, params Keys[] keys)
        {
            SendWait(pointer, ToString(modifier, keys));
        }


        public void SendWait(System.Diagnostics.Process process, params string[] keys)
        {
            SendWait(process.Handle, keys);
        }
        public void SendWait(System.Diagnostics.Process process, ModifierKeys modifier, params string[] keys)
        {
            SendWait(process.Handle, modifier, keys);
        }
        public void SendWait(System.Diagnostics.Process process, params char[] keys)
        {
            SendWait(process, string.Join("", keys));
        }
        public void SendWait(System.Diagnostics.Process process, ModifierKeys modifier, params char[] keys)
        {
            SendWait(process, string.Join("", modifier, keys));
        }
        public void SendWait(System.Diagnostics.Process process, params Keys[] keys)
        {
            SendWait(process, ToString(keys));
        }
        public void SendWait(System.Diagnostics.Process process, ModifierKeys modifier, params Keys[] keys)
        {
            SendWait(process, ToString(modifier, keys));
        }



        /// <summary>
        /// Registers a hot key in the system.
        /// </summary>
        /// <param name="modifier">The modifiers that are associated with the hot key.</param>
        /// <param name="key">The key itself that is associated with the hot key.</param>
        public void Register(ModifierKeys modifier, Keys key)
        {
            // increment the counter.
            _currentId = _currentId + 1;

            // register the hot key.
            if (!RegisterHotKey(_window.Handle, _currentId, (uint)modifier, (uint)key))
                throw new InvalidOperationException("Couldn’t register the hot key.");
        }
        /// <summary>
        /// Registers a hot key in the system.
        /// </summary>
        /// <param name="key">The key itself that is associated with the hot key.</param>
        /// <param name="modifier">The modifiers that are associated with the hot key.</param>
        public void Register(Keys key, ModifierKeys modifier)
        {
            // increment the counter.
            _currentId = _currentId + 1;

            // register the hot key.
            if (!RegisterHotKey(_window.Handle, _currentId, (uint)modifier, (uint)key))
                throw new InvalidOperationException("Couldn’t register the hot key.");
        }
        /// <summary>
        /// Registers a hot key in the system.
        /// </summary>
        /// <param name="key">The key itself that is associated with the hot key.</param>
        public void Register(Keys key)
        {
            // increment the counter.
            _currentId = _currentId + 1;

            // register the hot key.
            if (!RegisterHotKey(_window.Handle, _currentId, 0, (uint)key))
                throw new InvalidOperationException("Couldn’t register the hot key.");
        }
        public void Dispose()
        {
            // unregister all the registered hot keys.
            for (int i = _currentId; i > 0; i--)
            {
                UnregisterHotKey(_window.Handle, i);
            }

            // dispose the inner native window.
            _window.Dispose();
        }


        /// <summary>
        /// Represents the window that is used internally to get the messages.
        /// </summary>
        private class InternalWindow : NativeWindow, IDisposable
        {
            private static int WM_HOTKEY = 0x0312;
            public event EventHandler<KeyEventArgs> KeyPressed;

            public InternalWindow()
            {
                // create the handle for the window.
                this.CreateHandle(new CreateParams());
            }

            /// <summary>
            /// Overridden to get the notifications.
            /// </summary>
            /// <param name="m"></param>
            protected override void WndProc(ref Message m)
            {
                base.WndProc(ref m);

                // check if we got a hot key pressed.
                if (m.Msg == WM_HOTKEY)
                {
                    // get the keys.
                    Keys key = (Keys)(((int)m.LParam >> 16) & 0xFFFF);
                    ModifierKeys modifier = (ModifierKeys)((int)m.LParam & 0xFFFF);

                    // invoke the event to notify the parent.
                    if (KeyPressed != null)
                        KeyPressed(this, new KeyEventArgs(modifier, key));
                }
            }


            public void Dispose()
            {
                this.DestroyHandle();
            }
        }

    }
}
