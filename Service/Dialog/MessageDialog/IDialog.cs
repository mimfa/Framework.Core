using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MiMFa.General;

namespace MiMFa.Service.Dialog.MessageDialog
{
    public interface IDialog
    {
        DialogResult ShowDialog(string text, string caption = "", MessageBoxButtons buttuns = MessageBoxButtons.OK, MessageMode icon = MessageMode.Message, MessageBoxDefaultButton dbtn = MessageBoxDefaultButton.Button1, MessageBoxOptions rtlReading = MessageBoxOptions.DefaultDesktopOnly, string defaultValue = "", params string[] options);
        string GetDialog(string text, string caption = "", MessageBoxButtons buttuns = MessageBoxButtons.OK, MessageMode icon = MessageMode.Message, MessageBoxDefaultButton dbtn = MessageBoxDefaultButton.Button1, MessageBoxOptions rtlReading = MessageBoxOptions.DefaultDesktopOnly, string defaultValue = "", params string[] options);
        void Set(string text = "", string caption = "", MessageBoxButtons buttuns = MessageBoxButtons.OK, MessageMode icon = MessageMode.Message, MessageBoxDefaultButton dbtn = MessageBoxDefaultButton.Button1, MessageBoxOptions rtlReading = MessageBoxOptions.DefaultDesktopOnly, string defaultValue = "", params string[] options);

        void Close();
    }
}
