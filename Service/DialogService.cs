using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MiMFa.General;
using MiMFa.Service.Dialog.MessageDialog;
using MiMFa.Service.Dialog.MessageDialog.FormDialog;
using System.Windows.Media;
using System.IO;
using MiMFa.Network.Web;
using System.Drawing.Imaging;
using MiMFa.Engine.Translate;

namespace MiMFa.Service
{
    public enum DialogMode
    {
        None = 0,
        Console = 1,
        Classic = 2,
        Modern = 3,
        Circle = 5
    }
    public class DialogService 
    {
        public static bool IsShow { get; set; } = false; 
        public static DialogMode Mode { get; set; } = DialogMode.Modern;

        public static OpenFileDialog OFD { get; set; } = new OpenFileDialog();
        public static SaveFileDialog SFD { get; set; } = new SaveFileDialog();
        public static FolderBrowserDialog FBD { get; set; } = new FolderBrowserDialog();


        public static string InitialDirectory = null;
        public static string OpenFile(string fileName = "", string filter = "All Files (*.*) | *.*", bool restoreDirectory = true, string initialDirectory = null)
        {
            SFD.RestoreDirectory = restoreDirectory;
            if (initialDirectory != null) OFD.InitialDirectory = initialDirectory;
            if (fileName != null) OFD.FileName = fileName;
            if (filter != null) OFD.Filter = filter;
            OFD.Multiselect = false;
            if (OFD.ShowDialog() == DialogResult.OK)
                return OFD.FileName;
            return null;
        }
        public static string[] OpenFiles(string fileName = "", string filter = "All Files (*.*) | *.*", bool restoreDirectory = true, string initialDirectory = null)
        {
            SFD.RestoreDirectory = restoreDirectory;
            if (initialDirectory != null) OFD.InitialDirectory = initialDirectory;
            if(fileName != null) OFD.FileName = fileName;
            if (filter != null) OFD.Filter = filter;
            OFD.Multiselect = true;
            try
            {
                if (OFD.ShowDialog() == DialogResult.OK)
                    return OFD.FileNames;
                return new string[0];
            }
            finally
            {
                OFD.Multiselect = false;
            }
        }
        public static string OpenDirectory(string selectedPath = null, bool showNewFolderButton = true, bool restoreDirectory = true)
        {
            FBD.ShowNewFolderButton = showNewFolderButton;
            InitialDirectory = selectedPath ?? InitialDirectory;
            if(showNewFolderButton && !string.IsNullOrEmpty(InitialDirectory)) FBD.SelectedPath = InitialDirectory;
            if (selectedPath != null) FBD.SelectedPath = selectedPath;
            if (FBD.ShowDialog() == DialogResult.OK)
                return InitialDirectory = FBD.SelectedPath +Path.DirectorySeparatorChar;
            return null;
        }
        public static string SaveFile(string fileName = "", string filter = "All Files (*.*) | *.*", bool restoreDirectory = true, string initialDirectory = null)
        {
            SFD.RestoreDirectory = restoreDirectory;
            if (initialDirectory != null) OFD.InitialDirectory = initialDirectory;
            if (fileName != null) SFD.FileName = fileName;
            if (filter != null) SFD.Filter = filter;
            if (SFD.ShowDialog() == DialogResult.OK)
                return SFD.FileName;
            return null;
        }
        public static string SaveImage(string fileName, ref ImageFormat iformat, bool restoreDirectory = true, string initialDirectory = null)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageEncoders();
            string filter = string.Empty;
            string sep = string.Empty;

            foreach (var c in codecs)
            {
                string codecName = c.CodecName.Substring(8).Replace("Codec", "Files").Trim();
                filter = String.Format("{0}{1}{2} ({3})|{3}", filter, sep, codecName, c.FilenameExtension);
                sep = "|";
            }
            string path = SaveFile(fileName, filter, restoreDirectory, initialDirectory);
            string ext = codecs[SFD.FilterIndex - 1].FilenameExtension.ToUpper();
            if (ext.Contains(".PNG")) iformat = ImageFormat.Png;
            if (ext.Contains(".JPEG")) iformat = ImageFormat.Jpeg;
            if (ext.Contains(".GIF")) iformat = ImageFormat.Gif;
            if (ext.Contains(".TIFF")) iformat = ImageFormat.Tiff;
            if (ext.Contains(".BMP")) iformat = ImageFormat.Bmp;
            if (ext.Contains(".ICO")) iformat = ImageFormat.Icon;
            if (ext.Contains(".EMF")) iformat = ImageFormat.Emf;
            if (ext.Contains(".WMF")) iformat = ImageFormat.Wmf;
            return path;
        }


        public static string Prompt(string message, string defaultValue="")
        {
            return GetMessage(message,defaultValue, true);
        }
        public static string Prompt(string messageFormat, params object[] objs)
        {
            return Prompt(string.Format(messageFormat,objs));
        }
        public static bool Warn(string message)
        {
            return ShowMessage(MessageMode.Warning, true, message) == DialogResult.Yes;
        }
        public static bool Warn(string messageFormat, params object[] objs)
        {
            return Warn(string.Format(messageFormat, objs));
        }
        public static bool Confirm(string message)
        {
            return ShowMessage(MessageMode.Question, true, message) == DialogResult.Yes;
        }
        public static bool Confirm(string messageFormat,params object[] objs)
        {
            return Confirm(string.Format(messageFormat,objs));
        }
        public static bool Alert(string message)
        {
            return ShowMessage(MessageMode.Message, true, message) == DialogResult.OK;
        }
        public static bool Alert(string messageFormat,params object[] objs)
        {
            return Alert(string.Format(messageFormat,objs));
        }
        public static bool Alert(Exception ex)
        {
            return ShowMessage(ex) == DialogResult.OK;
        }


        public static bool? Warning(string message)
        {
            var dr = ShowMessage(MessageMode.Warning, true, message);
            switch (dr)
            {
                case DialogResult.Yes:
                    return true;
                case DialogResult.No:
                    return false;
                default:
                    return null;
            }
        }
        public static bool? Warning(string format, params object[] objs) => Warning(string.Format(format, objs));
        public static bool Question(string message)
        {
            return ShowMessage(MessageMode.Question, true, message) == DialogResult.Yes;
        }
        public static bool Question(string format, params object[] objs) => Question(string.Format(format, objs));
        public static bool Success(string message)
        {
            return ShowMessage(MessageMode.Success, true, message) == DialogResult.OK;
        }
        public static bool Success(string format, params object[] objs) => Success(string.Format(format, objs));
        public static bool Message(string message)
        {
            return ShowMessage(MessageMode.Message, true, message) == DialogResult.OK;
        }
        public static bool Message(string format, params object[] objs) => Message(string.Format(format, objs));
        public static bool Error(string message)
        {
            return ShowMessage(MessageMode.Error, true, message) == DialogResult.OK;
        }
        public static bool Error(string format, params object[] objs) => Error(string.Format(format, objs));
        public static bool Error(Exception ex)
        {
            return ShowMessage(ex) == DialogResult.OK;
        }

        public static DialogResult ShowMessage(Exception ex, bool translate = true)
        {
            var t = ex.GetType();
            var ps  = t.GetProperty("ErrorDetails");
            if (ps != null && ps.PropertyType == typeof(string))
                return ShowMessage(MessageMode.Error, ps.GetValue(ex) + "", translate);
            ps = t.GetProperty("Data");
            if (ps != null && ps.PropertyType == typeof(string))
                return ShowMessage(MessageMode.Error, ps.GetValue(ex) + "", translate);
            return ShowMessage(MessageMode.Error, ex.Message, translate);
        }
        public static DialogResult ShowMessage(string message, bool translate = true)
        {
            return ShowMessage("", MessageMode.Null, translate, message);
        }
        public static DialogResult ShowMessage(MessageMode messageType, params string[] messages)
        {
            return ShowMessage(messageType, true, messages);
        }
        public static DialogResult ShowMessage(MessageMode messageType, string message, bool translate)
        {
            return ShowMessage(messageType == MessageMode.Null ? "" : messageType + "", messageType, translate, message);
        }
        public static DialogResult ShowMessage(MessageMode messageType, bool translate, params string[] messages)
        {
           return  ShowMessage(messageType== MessageMode.Null?"": messageType + "", messageType, translate, messages);
        }
        public static DialogResult ShowMessage(string caption, MessageMode messageType, bool translate, params string[] messages)
        {
            try
            {
                //while (IsShow) Statement.Delay(1000);
                IsShow = true;
                switch (Mode)
                {
                    case DialogMode.Console:
                        return ShowConsoleMessage(caption, translate, messageType, messages);
                    case DialogMode.Classic:
                        return ShowClassicMessage(caption, translate, messageType, messages);
                    case DialogMode.Circle:
                        return ShowCircleMessage(caption, translate, messageType, messages);
                    default:
                        return ShowModernMessage(caption, translate, messageType, messages);
                }
            }
            finally { IsShow = false; }
        }
        public static string GetMessage(string message, string defaultValue = "", bool translate = true)
        {
            return GetMessage("",defaultValue, MessageMode.Null, translate, message);
        }
        public static string GetMessage(string defaultValue, MessageMode messageType, bool translate, string[] keys)
        {
            return GetMessage(messageType + "", defaultValue, messageType, translate, keys);
        }
        public static string GetMessage(string caption, string defaultValue,MessageMode messageType, bool translate, params string[] keys)
        {
            try
            {
                IsShow = true;
                switch (Mode)
                {
                    case DialogMode.Console:
                        return GetConsoleMessage(caption, defaultValue, translate, messageType, keys);
                    case DialogMode.Classic:
                        return GetClassicMessage(caption, defaultValue, translate, messageType, keys);
                    case DialogMode.Circle:
                        return GetCircleMessage(caption, defaultValue, translate, messageType, keys);
                    default:
                        return GetModernMessage(caption, defaultValue, translate, messageType, keys);
                }
            }
            finally { IsShow = false; }
        }

        public static DialogResult ShowConsoleMessage(string caption, bool translate, MessageMode messageType, params string[] keys)
        {
            string msg = (translate) ? Default.Translator.Get(keys) : CollectionService.GetAllItems(keys, " ");
            Mode = DialogMode.Console;
            string cap = (translate) ? Default.Translator.Get(caption) : caption;
            string msges = cap + " >> " + msg;
            Console.Write(msges);
            try
            {
                ConsoleKey k = ConsoleKey.C;
                switch (messageType)
                {
                    case MessageMode.Success:
                        Console.Write(" (Press any key) ");
                        k = Console.ReadKey().Key;
                        return DialogResult.OK;
                    case MessageMode.Error:
                        Console.Write(" (Press any key) ");
                        k = Console.ReadKey().Key;
                        return DialogResult.OK;
                    case MessageMode.Question:
                        Console.Write(" (Yes:Y, No:N, Cancel:C)? ");
                        k = Console.ReadKey().Key;
                        return (k == ConsoleKey.Y) ? DialogResult.Yes : (k == ConsoleKey.N) ? DialogResult.No : DialogResult.Cancel;
                    case MessageMode.Warning:
                        Console.Write(" (Yes:Y, No:N)? ");
                        k = Console.ReadKey().Key;
                        return (k == ConsoleKey.Y) ? DialogResult.Yes : DialogResult.No;
                    default:
                        return DialogResult.OK;
                }
            }
            finally
            {
                Console.ReadLine();
            }
        }
        public static string GetConsoleMessage(string caption, string defaultValue, bool translate, MessageMode messageType, params string[] keys)
        {
            string msg = (translate) ? Default.Translator.Get(keys) : CollectionService.GetAllItems(keys, " ");
            Mode = DialogMode.Console;
            string cap = (translate) ? Default.Translator.Get(caption) : caption;
            string msges = cap + " >> " + msg;
            Console.Write(msges);
            try
            {
                switch (messageType)
                {
                    case MessageMode.Success:
                        return Console.ReadLine();
                    case MessageMode.Error:
                        return Console.ReadLine();
                    case MessageMode.Question:
                        Console.Write(" (Yes:Y, No:N, Cancel:C)? ");
                        return Console.ReadLine();
                    case MessageMode.Warning:
                        Console.Write(" (Yes:Y, No:N)? ");
                        return Console.ReadLine();
                    default:
                        return Console.ReadLine();
                }
            }
            finally
            {
                Console.ReadLine();
            }
        }

        public static DialogResult ShowClassicMessage(string caption,bool translate,MessageMode messageType, params string[] keys)
        {
            string msg = (Default.HasTranslator && translate) ? Default.Translator.Get(keys) : CollectionService.GetAllItems(keys, " ",0,-1);
            Mode = DialogMode.Classic;
            string cap = (Default.HasTranslator && translate) ? Default.Translator.Get(caption) : caption;
            if (Default.RightToLeft)
                switch (messageType)
                {
                    case MessageMode.Success:
                        return MessageBox.Show(
                            msg,
                           cap,
                             MessageBoxButtons.OK,
                             MessageBoxIcon.Asterisk,
                             MessageBoxDefaultButton.Button1,
                             MessageBoxOptions.RtlReading
                             );
                    case MessageMode.Error:
                        return MessageBox.Show(
                            msg,
                           cap,
                             MessageBoxButtons.OK,
                             MessageBoxIcon.Error,
                             MessageBoxDefaultButton.Button1,
                             MessageBoxOptions.RtlReading
                             );
                    case MessageMode.Question:
                        return MessageBox.Show(
                            msg,
                           cap,
                            MessageBoxButtons.YesNoCancel,
                            MessageBoxIcon.Warning,
                            MessageBoxDefaultButton.Button2,
                            MessageBoxOptions.RtlReading
                            );
                    case MessageMode.Warning:
                        return MessageBox.Show(
                            msg,
                           cap,
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question,
                            MessageBoxDefaultButton.Button1,
                            MessageBoxOptions.RtlReading
                            );
                    default:
                        return MessageBox.Show(
                            msg,
                           cap,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information,
                            MessageBoxDefaultButton.Button1,
                            MessageBoxOptions.RtlReading
                            );
                }
            else
                switch (messageType)
                {
                    case MessageMode.Success:
                        return MessageBox.Show(
                            msg,
                           cap,
                             MessageBoxButtons.OK,
                             MessageBoxIcon.Asterisk,
                             MessageBoxDefaultButton.Button1
                             );
                    case MessageMode.Error:
                        return MessageBox.Show(
                            msg,
                           cap,
                             MessageBoxButtons.OK,
                             MessageBoxIcon.Error,
                             MessageBoxDefaultButton.Button1
                             );
                    case MessageMode.Question:
                        return MessageBox.Show(
                            msg,
                           cap,
                            MessageBoxButtons.YesNoCancel,
                            MessageBoxIcon.Warning,
                            MessageBoxDefaultButton.Button2
                            );
                    case MessageMode.Warning:
                        return MessageBox.Show(
                            msg,
                           cap,
                            MessageBoxButtons.YesNo,
                            MessageBoxIcon.Question,
                            MessageBoxDefaultButton.Button1
                            );
                    default:
                        return MessageBox.Show(
                            msg,
                           cap,
                            MessageBoxButtons.OK,
                            MessageBoxIcon.Information,
                            MessageBoxDefaultButton.Button1
                            );
                }
        }
        public static string GetClassicMessage(string caption, string defaultValue, bool translate, MessageMode messageType, params string[] keys)
        {
           var LastDialog = new ModernDialog();
            Mode = DialogMode.Classic;
            string msg = (Default.HasTranslator && translate) ? Default.Translator.Get(keys) : CollectionService.GetAllItems(keys, " ");
            string cap = (Default.HasTranslator && translate) ? Default.Translator.Get(caption) : caption;
            if (Default.RightToLeft)
                return LastDialog.GetDialog(
                    msg,
                   cap,
                     MessageBoxButtons.OK,
                     messageType,
                     MessageBoxDefaultButton.Button1,
                     MessageBoxOptions.RtlReading,
                     defaultValue
                     );
            else
                return LastDialog.GetDialog(
                     msg,
                     cap,
                     MessageBoxButtons.OK,
                     messageType,
                     MessageBoxDefaultButton.Button1,
                     MessageBoxOptions.DefaultDesktopOnly,
                     defaultValue
                     );
        }

        public static DialogResult ShowModernMessage(string caption, bool translate, MessageMode messageType, params string[] keys)
        {
            var LastDialog = new ModernDialog();
            if (LastDialog is Form && ((Form)LastDialog).Visible)
            {
                ((Form)LastDialog).Close();
                LastDialog = new ModernDialog();
            }
            Mode = DialogMode.Modern;
            string msg = (Default.HasTranslator && translate) ? Default.Translator.Get(keys) : CollectionService.GetAllItems(keys, " ");
            string cap = (Default.HasTranslator && translate) ? Default.Translator.Get(caption) : caption;
            if (Default.RightToLeft)
                switch (messageType)
                {
                    case MessageMode.Success:
                        return LastDialog.ShowDialog(
                            msg,
                           cap,
                             MessageBoxButtons.OK,
                             messageType,
                             MessageBoxDefaultButton.Button1,
                             MessageBoxOptions.RtlReading
                             );
                    case MessageMode.Error:
                        return LastDialog.ShowDialog(
                             msg,
                             cap,
                             MessageBoxButtons.OK,
                             messageType,
                             MessageBoxDefaultButton.Button1,
                             MessageBoxOptions.RtlReading
                             );
                    case MessageMode.Warning:
                        return LastDialog.ShowDialog(
                            msg,
                            cap,
                            MessageBoxButtons.YesNoCancel,
                             messageType,
                            MessageBoxDefaultButton.Button2,
                            MessageBoxOptions.RtlReading
                            );
                    case MessageMode.Question:
                        return LastDialog.ShowDialog(
                            msg,
                            cap,
                            MessageBoxButtons.YesNo,
                             messageType,
                            MessageBoxDefaultButton.Button1,
                            MessageBoxOptions.RtlReading
                            );
                    default:
                        return LastDialog.ShowDialog(
                            msg,
                            cap,
                            MessageBoxButtons.OK,
                             messageType,
                            MessageBoxDefaultButton.Button1,
                            MessageBoxOptions.RtlReading
                            );
                }
            else
                switch (messageType)
                {
                    case MessageMode.Success:
                        return LastDialog.ShowDialog(
                             msg,
                             cap,
                             MessageBoxButtons.OK,
                             messageType,
                             MessageBoxDefaultButton.Button1
                             );
                    case MessageMode.Error:
                        return LastDialog.ShowDialog(
                             msg,
                             cap,
                             MessageBoxButtons.OK,
                             messageType,
                             MessageBoxDefaultButton.Button1
                             );
                    case MessageMode.Warning:
                        return LastDialog.ShowDialog(
                            msg,
                            cap,
                            MessageBoxButtons.YesNoCancel,
                             messageType,
                            MessageBoxDefaultButton.Button2
                            );
                    case MessageMode.Question:
                        return LastDialog.ShowDialog(
                            msg,
                            cap,
                            MessageBoxButtons.YesNo,
                             messageType,
                            MessageBoxDefaultButton.Button1
                            );
                    default:
                        return LastDialog.ShowDialog(
                            msg,
                            cap,
                            MessageBoxButtons.OK,
                             messageType,
                            MessageBoxDefaultButton.Button1
                            );
                }
        }
        public static string GetModernMessage(string caption, string defaultValue, bool translate, MessageMode messageType, params string[] keys)
        {
            var LastDialog = new ModernDialog();
            if (LastDialog is Form && ((Form)LastDialog).Visible)
            {
                ((Form)LastDialog).Close();
                LastDialog = new ModernDialog();
            }
            Mode = DialogMode.Modern;
            string msg = (Default.HasTranslator && translate) ? Default.Translator.Get(keys) : CollectionService.GetAllItems(keys, " ");
            string cap = (Default.HasTranslator && translate) ? Default.Translator.Get(caption) : caption;
            if (Default.RightToLeft)
                return LastDialog.GetDialog(
                    msg,
                   cap,
                     MessageBoxButtons.OKCancel,
                     messageType,
                     MessageBoxDefaultButton.Button1,
                     MessageBoxOptions.RtlReading,
                     defaultValue
                     );
            else
                return LastDialog.GetDialog(
                     msg,
                     cap,
                     MessageBoxButtons.OKCancel,
                     messageType,
                     MessageBoxDefaultButton.Button1,
                     MessageBoxOptions.DefaultDesktopOnly,
                     defaultValue
                     );
        }

        public static DialogResult ShowCircleMessage(string caption, bool translate, MessageMode messageType, params string[] keys)
        {
            var LastDialog = new CircleDialog();
            if (LastDialog is Form && ((Form)LastDialog).Visible)
            {
                ((Form)LastDialog).Close();
                LastDialog = new CircleDialog();
            }
            Mode = DialogMode.Circle;
            string msg = (Default.HasTranslator && translate) ? Default.Translator.Get(keys) : CollectionService.GetAllItems(keys, " ");
            string cap = (Default.HasTranslator && translate) ? Default.Translator.Get(caption) : caption;
            if (Default.RightToLeft)
                switch (messageType)
                {
                    case MessageMode.Success:
                        return LastDialog.ShowDialog(
                            msg,
                           cap,
                             MessageBoxButtons.OK,
                             messageType,
                             MessageBoxDefaultButton.Button1,
                             MessageBoxOptions.RtlReading
                             );
                    case MessageMode.Error:
                        return LastDialog.ShowDialog(
                             msg,
                             cap,
                             MessageBoxButtons.OK,
                             messageType,
                             MessageBoxDefaultButton.Button1,
                             MessageBoxOptions.RtlReading
                             );
                    case MessageMode.Warning:
                        return LastDialog.ShowDialog(
                            msg,
                            cap,
                            MessageBoxButtons.YesNoCancel,
                             messageType,
                            MessageBoxDefaultButton.Button2,
                            MessageBoxOptions.RtlReading
                            );
                    case MessageMode.Question:
                        return LastDialog.ShowDialog(
                            msg,
                            cap,
                            MessageBoxButtons.YesNo,
                             messageType,
                            MessageBoxDefaultButton.Button1,
                            MessageBoxOptions.RtlReading
                            );
                    default:
                        return LastDialog.ShowDialog(
                            msg,
                            cap,
                            MessageBoxButtons.OK,
                             messageType,
                            MessageBoxDefaultButton.Button1,
                            MessageBoxOptions.RtlReading
                            );
                }
            else
                switch (messageType)
                {
                    case MessageMode.Success:
                        return LastDialog.ShowDialog(
                             msg,
                             cap,
                             MessageBoxButtons.OK,
                             messageType,
                             MessageBoxDefaultButton.Button1
                             );
                    case MessageMode.Error:
                        return LastDialog.ShowDialog(
                             msg,
                             cap,
                             MessageBoxButtons.OK,
                             messageType,
                             MessageBoxDefaultButton.Button1
                             );
                    case MessageMode.Warning:
                        return LastDialog.ShowDialog(
                            msg,
                            cap,
                            MessageBoxButtons.YesNoCancel,
                             messageType,
                            MessageBoxDefaultButton.Button2
                            );
                    case MessageMode.Question:
                        return LastDialog.ShowDialog(
                            msg,
                            cap,
                            MessageBoxButtons.YesNo,
                             messageType,
                            MessageBoxDefaultButton.Button1
                            );
                    default:
                        return LastDialog.ShowDialog(
                            msg,
                            cap,
                            MessageBoxButtons.OK,
                             messageType,
                            MessageBoxDefaultButton.Button1
                            );
                }
        }
        public static string GetCircleMessage(string caption, string defaultValue, bool translate, MessageMode messageType, params string[] keys)
        {
            var LastDialog = new CircleDialog();
            if (LastDialog is Form && ((Form)LastDialog).Visible)
            {
                ((Form)LastDialog).Close();
                LastDialog = new CircleDialog();
            }
            Mode = DialogMode.Circle;
            string msg = (Default.HasTranslator && translate) ? Default.Translator.Get(keys) : CollectionService.GetAllItems(keys, " ");
            string cap = (Default.HasTranslator && translate) ? Default.Translator.Get(caption) : caption;
            if (Default.RightToLeft)
                return LastDialog.GetDialog(
                    msg,
                   cap,
                     MessageBoxButtons.OK,
                     messageType,
                     MessageBoxDefaultButton.Button1,
                     MessageBoxOptions.RtlReading,
                     defaultValue
                     );
            else
                return LastDialog.GetDialog(
                     msg,
                     cap,
                     MessageBoxButtons.OK,
                     messageType,
                     MessageBoxDefaultButton.Button1,
                     MessageBoxOptions.DefaultDesktopOnly,
                     defaultValue
                     );
        }
   
        public static bool ExceptionHandler(Exception ex, string message = null,bool translate = true, string logDirectory = null, string logUrl = null)
        {
            if (string.IsNullOrWhiteSpace(message)) message = ex.Message;
            else message = string.Format(message, ex.Message, ex.StackTrace);
            string stacktrace = ex.StackTrace;
            if (!string.IsNullOrWhiteSpace(logDirectory))
                try { File.WriteAllText(logDirectory + DateTime.Now.Ticks + ".txt", stacktrace); } catch { }
            if (!string.IsNullOrWhiteSpace(logUrl))
            {
                if (ShowMessage(MessageMode.Warning, translate, message) == DialogResult.Yes)
                {
                    string parameters = string.Join("&",
                        "product=" + System.Web.HttpUtility.UrlEncode(Application.CompanyName + " " + Application.ProductName),
                        "version=" + Application.ProductVersion,
                        "ip=" +  NetService.GetInternalIPv4(),
                        "mac=" +  NetService.GetMAC(),
                        "date=" + System.Web.HttpUtility.UrlEncode(DateTime.UtcNow.Date.ToString()),
                        "time=" + System.Web.HttpUtility.UrlEncode(DateTime.UtcNow.TimeOfDay.ToString()),
                        "directory=" + System.Web.HttpUtility.UrlEncode(MiMFa.Config.ApplicationDirectory.Replace("\\", "/")),
                        "stacktrace=" + System.Web.HttpUtility.UrlEncode(stacktrace));
                    try
                    {
                        using (MiMFa.Network.Web.WebClient wc = new MiMFa.Network.Web.WebClient())
                        {
                            wc.Headers[System.Net.HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                            return string.IsNullOrWhiteSpace(System.Web.HttpUtility.UrlDecode(wc.UploadString(logUrl, parameters)));
                        }
                    }
                    catch { return false; }
                }
                return true;
            }
            else return ShowMessage(MessageMode.Error, translate, message) == DialogResult.OK;
        }

    }
}
