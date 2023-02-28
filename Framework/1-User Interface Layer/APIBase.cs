using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MiMFa.Framework.User_Interface_Layer
{
    public abstract class APIBase<T>
    {
        public static T MAIN;
        public static OpenFileDialog OFD { get; set; } = new OpenFileDialog();
        public static SaveFileDialog SFD { get; set; } = new SaveFileDialog();
        public static FolderBrowserDialog FBD { get; set; } = new FolderBrowserDialog();
    }
}
