using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;
using System.Reflection;
using MiMFa.Service;
using System.Collections;
using System.Drawing;
using System.Runtime.InteropServices;
using System.IO;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using MiMFa.Network;
using MiMFa.General;
using System.Text.RegularExpressions;

namespace MiMFa.Service
{
    public class InfoService
    {

        #region Is Check
        public static bool IsNumber(object value)
        {
            //if (
            //    value is short
            //    || value is int
            //    || value is long
            //    || value is float
            //    || value is double
            //    || value is decimal
            //    || value is Int16
            //    || value is Int32
            //    || value is Int64
            //    || value is Single
            //    || value is Double
            //    || value is Decimal) return true;
            double d = 0;
            return double.TryParse(value + "", out d);
        }
        public static bool IsText(object value)
        {
            return value is String;
        }
        public static bool IsAddress(string value,bool mostexist = true)
        {
            if(File.Exists(value))return true;
            return Directory.Exists(value) ||
                (!mostexist && !Regex.IsMatch(value.Split('\\').Last(), @"\&|\||\/|\:|\'|\!|\~|\`|"+"\\\"") 
                && (Path.IsPathRooted(value) || Regex.IsMatch(value, @"\w+:([\s\S]*\\?[\s\S]+)+")));
        }
        public static bool IsDirectory(string value, bool mostexist = true)
        {
            if (Directory.Exists(value)) return true;
            return (!mostexist && !Regex.IsMatch(value, @"\&|\||\/|\:|\'|\!|\~|\`|" + "\\\"") && (Path.IsPathRooted(value) || Regex.IsMatch(value, @"((\w\W)*\\?(\w\W)+)+")));
        }
        public static bool IsFile(string value, bool mostexist = true)
        {
            if (File.Exists(value)) return true;
            return (!mostexist && !Regex.IsMatch(value, @"\&|\||\/|\:|\'|\!|\~|\`|" + "\\\"") && (Path.IsPathRooted(value) || Regex.IsMatch(value, @"((\w\W)*\\?(\w\W)+)+")));
        }
        public static bool IsURL(string value)
        {
            return Uri.IsWellFormedUriString(value, UriKind.RelativeOrAbsolute);
        }
        public static bool IsURLFile(string value, string extentions = ".pdf;.doc;.docx;.zip;.zipx;.ppt;.pptx;.txt")
        {
            value = value.ToLower();
            extentions = extentions.ToLower();
            return Uri.IsWellFormedUriString(value, UriKind.RelativeOrAbsolute) &&
               (value.Length > 4 && (
                    (from v in extentions.Split(';',',') where value.EndsWith(v) select v).Any()
               ));
        }
        public static bool IsAbsoluteURL(string value)
        {
            return Uri.IsWellFormedUriString(value, UriKind.Absolute);
        }
        public static bool IsRelativeURL(string value)
        {
            return Uri.IsWellFormedUriString(value, UriKind.Relative);
        }
        public static bool IsHTMLURL(string url)
        {
            var request = HttpWebRequest.Create(url);
            request.Method = "HEAD";
            switch (request.GetResponse().ContentType)
            {
                case "text/html": return true;
                default: return false;
            }
        }
        public static bool IsWord(string value)
        {
            if (IsWordList == null)
            {
                CharBank cb = new CharBank();
                IsWordList = new List<char>();
                IsWordList.AddRange(cb.NumberCharacters);
                IsWordList.AddRange(cb.SymbolCharacter);
                IsWordList.AddRange(cb.SignCharacters);
            }
            foreach (var item in value)
                if (IsWordList.Exists((i)=>i==item)) return false;
            return true;
        }
        private static List<char> IsWordList = null;

        public static bool IsHTMLContent(object value)
        {
            if (value == null) return false;
            if (StringService.FirstWordBetween(value + "","<",">") != null) return true;
            return false;
        }
        public static bool IsConvertable(Type type,object obj)
        {
            try
            {
               return type.IsAssignableFrom(obj.GetType());
            }
            catch { return false; }
        }
        public static bool IsDictionary(object obj)
        {
            if (obj == null) return false;
            Type t = obj.GetType();
            string tn = t.Name.ToLower();
            return (t is IDictionary || tn.StartsWith("dictionary") || tn.StartsWith("mimfa_dictionary"));
        }
        public static bool IsMatrix(object obj)
        {
            if (obj == null) return false;
            string tn = obj.GetType().Name.ToLower();
            return (tn.StartsWith("mimfa_matrix"));
        }
        public static bool IsKeyValuePair(object obj)
        {
            if (obj == null) return false;
            string tn = obj.GetType().Name.ToLower();
            return (tn.StartsWith("keyvaluepair"));
        }
        public static bool IsList(object obj)
        {
            if (obj == null) return false;
            Type t = obj.GetType();
            string tn = t.Name.ToLower();
            return (t is IList || tn.StartsWith("list") || tn.StartsWith("mimfa_list"));
        }
        public static bool IsArray(object obj)
        {
            if (obj == null) return false;
            string tn = obj.GetType().Name.ToLower();
            return (tn.EndsWith("[]"));
        }
        public static bool IsCollection(object obj)
        {
            if (obj == null) return false;
            Type t = obj.GetType();
            return (t is ICollection);
        }
        public static bool IsRegexPattern(string pattern)
        {
            Regex re = new Regex(@"\^|\$|\+|(\W|\w)\-(\W|\w)|\*|\?|\\b|\\e", RegexOptions.IgnoreCase, new TimeSpan(0, 0, 20));
            return re.IsMatch(pattern);
        }
        public static bool IsValidRegexPattern(string pattern, string testText = "", int maxSecondTimeOut = 20)
        {
            if (string.IsNullOrEmpty(pattern)) return false;
            try
            {
                Regex re = new Regex(pattern, RegexOptions.None, new TimeSpan(0, 0, 20));
                re.IsMatch(testText); 
            }
            //ArgumentException
            //RegexMatchTimeoutException
            catch{ return false; }
            return true;
        }

        public static string GetFloatSign()
        {
            return (1.1F).ToString().Replace("1", "");
        }
        public static char GetFloatChar()
        {
            return (1.1F).ToString().Replace("1", "")[0];
        }

        public static bool IsStack(object obj)
        {
            if (obj == null) return false;
            string tn = obj.GetType().Name.ToLower();
            return (tn.StartsWith("stack"));
        }
        public static bool IsDataTable(object obj)
        {
            if (obj == null) return false;
            string tn = obj.GetType().Name.ToLower();
            return (tn.StartsWith("datatable"));
        }
        public static bool IsDataRow(object obj)
        {
            if (obj == null) return false;
            string tn = obj.GetType().Name.ToLower();
            return (tn.StartsWith("datarow"));
        }
        public static bool IsDataColumn(object obj)
        {
            if (obj == null) return false;
            string tn = obj.GetType().Name.ToLower();
            return (tn.StartsWith("datacolumn"));
        }
        public static bool IsBitmap(object obj)
        {
            if (obj == null) return false;
            Type t = obj.GetType();
            return (typeof(Bitmap).IsAssignableFrom(t));
        }
        public static bool IsBitmap(byte[] obj)
        {
            if (obj == null) return false;
            try
            {
                ConvertService.ToImage(obj);
                return false;
            }
            catch { return false; }
        }
        public static readonly string[] AllSignElseEvaluatable = { "->", "-<", "=>", "=<", "&&", "||", "!==", "~==", "===", "==", "!=", "~=", ">=", "<=", ">", "<" };
        public static bool IsEvaluatable(string str)
        {
            if (str.Split(AllSignElseEvaluatable, StringSplitOptions.None).Length > 1) return false;
            string[] ca = new string[] { "^", "√", "%", "×", "*", "÷", "\\", "+", "-" };
            string[] sar = str.Split(ca,StringSplitOptions.None);
            return sar.Length > 1;
        }
        public static readonly string[] AllSignElseComparable = { "->", "-<", "=>", "=<" };
        public static bool IsComparable(string str)
        {
            if (str.Split(AllSignElseComparable, StringSplitOptions.None).Length>1) return false;
            string[] ca = { "&&", "||", "!==", "~==", "===", "==", "!=", "~=", ">=", "<=", ">", "<" };
            str = str.Trim();
            ca = str.Split(ca, StringSplitOptions.None);
            if (ca.Length > 1) return true;
            if ((ca[0] = ca[0].Trim().Replace("(", "").Replace(")", "")) == "true" || ca[0] == "false") return true;
            return false;
        }
        #endregion

        #region Objective
        public static MethodInfo[] GetMethods(object obj, string methodName, bool caseSensivity = true)
        {
            Type t = obj.GetType();
            MethodInfo[] methods = CollectionService.Concat(t.GetMethods(), t.GetRuntimeMethods().ToArray());
            if (caseSensivity) return (from v in methods where v.Name == methodName select v).ToArray();
            return (from v in methods where v.Name.ToUpper() == methodName.ToUpper() select v).ToArray();
        }
        public static MethodInfo[] GetMethods( string methodName, Type type, bool caseSensivity = true)
        {
            MethodInfo[] methods = CollectionService.Concat(type.GetMethods(), type.GetRuntimeMethods().ToArray());
            if (caseSensivity) return (from v in methods where v.Name == methodName select v).ToArray();
            return (from v in methods where v.Name.ToUpper() == methodName.ToUpper() select v).ToArray();
        }
        public static EventInfo[] GetEvents(object obj, string eventName, bool caseSensivity = true)
        {
            EventInfo[] events = obj.GetType().GetEvents();
            if (caseSensivity) return (from v in events where v.Name == eventName select v).ToArray();
            return (from v in events where v.Name.ToUpper() == eventName.ToUpper() select v).ToArray();
        }
        public static EventInfo[] GetEvents( string eventName, Type type, bool caseSensivity = true)
        {
            EventInfo[] events = type.GetEvents();
            if (caseSensivity) return (from v in events where v.Name == eventName select v).ToArray();
            return (from v in events where v.Name.ToUpper() == eventName.ToUpper() select v).ToArray();
        }
        public static PropertyInfo[] GetProperties(object obj, string propertyName, bool caseSensivity = true)
        {
            PropertyInfo[] properties = obj.GetType().GetProperties();
            if (caseSensivity) return (from v in properties where v.Name == propertyName select v).ToArray();
            return (from v in properties where v.Name.ToUpper() == propertyName.ToUpper() select v).ToArray();
        }
        public static PropertyInfo[] GetProperties( string propertyName, Type type, bool caseSensivity = true)
        {
            PropertyInfo[] properties = type.GetProperties();
            if (caseSensivity) return (from v in properties where v.Name == propertyName select v).ToArray();
            return (from v in properties where v.Name.ToUpper() == propertyName.ToUpper() select v).ToArray();
        }
        public static FieldInfo[] GetFields(object obj, string fieldName, bool caseSensivity = true)
        {
            FieldInfo[] fields = obj.GetType().GetFields();
            if (caseSensivity) return (from v in fields where v.Name == fieldName select v).ToArray();
            return (from v in fields where v.Name.ToUpper() == fieldName.ToUpper() select v).ToArray();
        }
        public static FieldInfo[] GetFields(string fieldName,Type type, bool caseSensivity = true)
        {
            FieldInfo[] fields = type.GetFields();
            if (caseSensivity) return (from v in fields where v.Name == fieldName select v).ToArray();
            return (from v in fields where v.Name.ToUpper() == fieldName.ToUpper() select v).ToArray();
        }
        public static string GetMemberName<T>(Expression<Func<T>> memberExpression)
        {
            MemberExpression expressionBody = (MemberExpression)memberExpression.Body;
            return expressionBody.Member.Name;
        }
        #endregion

        #region System

        #region Is Check
        public static bool Is64BitProcess()
        {
            return Environment.Is64BitProcess;// IntPtr.Size == 8;
        }
        public static bool Is64BitOperatingSystem()
        {
            return Environment.Is64BitOperatingSystem;
        }
        public static bool Is32BitProcess()
        {
            return !Environment.Is64BitProcess;// IntPtr.Size != 8;
        }
        public static bool Is32BitOperatingSystem()
        {
            return !Environment.Is64BitOperatingSystem;
        }

        public static bool IsConnectedToInternet()
        {
            return GetPublicIP() != null;
        }

        public static bool IsBinaryFile(string path)
        {
            using (StreamReader stream = new StreamReader(path,true))
            {
                int num = 0;
                int ch = 0;
                try
                {
                    while ((ch = stream.Read()) != -1 && num++ < 999999)
                        if (IsControlCharNumber(ch))
                            return true;
                }
                finally { stream.Close(); }
            }
            return false;
        }

        public static bool IsControlCharNumber(int ch)
        {
            return (ch > 0 && ch < 8)
                || (ch > 13 && ch < 26);
        }
        public static bool IsBinary(string content)
        {
            return content.Any(ch => char.IsControl(ch) && ch != '\r' && ch != '\n');
        }
        #endregion

        #region OS
        public static string OSInformation(string separator = "; ") => string.Join(separator, from v in OSFeatures() select string.Join(separator,v.Key, v.Value));
        public static Dictionary<string,string> OSFeatures()
        {
            Dictionary<string,string> r = new Dictionary<string,string>();
            try
            {
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT * FROM Win32_OperatingSystem"))
                {
                    ManagementObjectCollection information = searcher.Get();
                    if (information != null)
                        foreach (ManagementObject obj in information)
                            r.Add(obj["Caption"].ToString(),obj["OSArchitecture"].ToString());
                    foreach (var item in r.Keys.ToList())
                        r[item] = r[item].Replace("NT 5.1.2600", "XP")
                            .Replace("NT 5.2.3790", "Server 2003");
                }
            }
            catch { }
            return r;
        }
        #endregion

        #region Mime
        [DllImport(@"urlmon.dll", CharSet = CharSet.Auto)]
        public extern static System.UInt32 FindMimeFromData(
        System.UInt32 pBC,
        [MarshalAs(UnmanagedType.LPStr)] System.String pwzUrl,
        [MarshalAs(UnmanagedType.LPArray)] byte[] pBuffer,
        System.UInt32 cbSize,
        [MarshalAs(UnmanagedType.LPStr)] System.String pwzMimeProposed,
        System.UInt32 dwMimeFlags,
        out System.UInt32 ppwzMimeOut,
        System.UInt32 dwReserverd );
        public static string GetMimeFile(string filename)
        {
            if (!File.Exists(filename))
                throw new FileNotFoundException(filename + " not found");
            try
            {
                byte[] buffer = new byte[256];
                using (FileStream fs = new FileStream(filename, FileMode.Open))
                {
                    if (fs.Length >= 256)
                        fs.Read(buffer, 0, 256);
                    else
                        fs.Read(buffer, 0, (int)fs.Length);
                }
                System.UInt32 mimetype;
                FindMimeFromData(0, null, buffer, 256, null, 0, out mimetype, 0);
                System.IntPtr mimeTypePtr = new IntPtr(mimetype);
                string mime = Marshal.PtrToStringUni(mimeTypePtr);
                Marshal.FreeCoTaskMem(mimeTypePtr);
                return mime;
            }
            catch
            {
                return "unknown/unknown";
            }
        }
        public static string GetMimeObject(object obj)
        {
            if (obj == null) return "null";

            try
            {
                if (obj is byte[])
                {
                    var o = IOService.TryDeserialize((byte[])obj);
                    if (o is byte[]) throw new Exception("unknown");
                    else return GetMimeObject(o);
                }
                else try
                    {
                        string typ = obj.GetType().Name.ToLower();
                        switch (typ)
                        {
                            case "string":
                            case "int":
                            case "uint":
                            case "long":
                            case "float":
                            case "double":
                            case "intptr":
                            case "decimal":
                            case "single":
                                return "text/txt";
                            case "bitmap":
                                return "image/png";
                            case "image":
                                return "image/jpg";
                            case "icon":
                                return "image/ico";
                            default:
                                return typ + "/data";
                        }
                    }
                    catch
                    {
                        string filename = Config.TemporaryDirectory + "\\mimecheck";
                        IOService.SaveSerializeFile(filename, obj);
                        return GetMimeFile(filename);
                    }
            }
            catch
            {
                return "unknown/unknown";
            }
        }
        #endregion

        #region Processor Id
        [DllImport("user32", EntryPoint = "CallWindowProcW", CharSet = CharSet.Unicode, SetLastError = true, ExactSpelling = true)]
        private static extern IntPtr CallWindowProcW([In] byte[] bytes, IntPtr hWnd, int msg, [In, Out] byte[] wParam, IntPtr lParam);
        [return: MarshalAs(UnmanagedType.Bool)]
        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool VirtualProtect([In] byte[] bytes, IntPtr size, int newProtect, out int oldProtect);
        const int PAGE_EXECUTE_READWRITE = 0x40;
        private static bool ExecuteCode(ref byte[] result)
        {
            int num;

            /* The opcodes below implement a C function with the signature:
             * __stdcall CpuIdWindowProc(hWnd, Msg, wParam, lParam);
             * with wParam interpreted as an 8 byte unsigned character buffer.
             * */

            byte[] code_x86 = new byte[] {
            0x55,                      /* push ebp */
            0x89, 0xe5,                /* mov  ebp, esp */
            0x57,                      /* push edi */
            0x8b, 0x7d, 0x10,          /* mov  edi, [ebp+0x10] */
            0x6a, 0x01,                /* push 0x1 */
            0x58,                      /* pop  eax */
            0x53,                      /* push ebx */
            0x0f, 0xa2,                /* cpuid    */
            0x89, 0x07,                /* mov  [edi], eax */
            0x89, 0x57, 0x04,          /* mov  [edi+0x4], edx */
            0x5b,                      /* pop  ebx */
            0x5f,                      /* pop  edi */
            0x89, 0xec,                /* mov  esp, ebp */
            0x5d,                      /* pop  ebp */
            0xc2, 0x10, 0x00,          /* ret  0x10 */
        };
            byte[] code_x64 = new byte[] {
            0x53,                                     /* push rbx */
            0x48, 0xc7, 0xc0, 0x01, 0x00, 0x00, 0x00, /* mov rax, 0x1 */
            0x0f, 0xa2,                               /* cpuid */
            0x41, 0x89, 0x00,                         /* mov [r8], eax */
            0x41, 0x89, 0x50, 0x04,                   /* mov [r8+0x4], edx */
            0x5b,                                     /* pop rbx */
            0xc3,                                     /* ret */
        };

            byte[] code;

            if (Is64BitProcess())
                code = code_x64;
            else
                code = code_x86;

            IntPtr ptr = new IntPtr(code.Length);

            if (!VirtualProtect(code, ptr, PAGE_EXECUTE_READWRITE, out num))
                Marshal.ThrowExceptionForHR(Marshal.GetHRForLastWin32Error());

            ptr = new IntPtr(result.Length);

            try
            {
                return (CallWindowProcW(code, IntPtr.Zero, 0, result, ptr) != IntPtr.Zero);
            }
            catch {  return false; }
        }
        public static string ProcessorSerial()
        {
            byte[] sn = new byte[8];
            if (!ExecuteCode(ref sn))
                return "ND";
            return string.Format("{0}{1}", BitConverter.ToUInt32(sn, 4).ToString("X8"), BitConverter.ToUInt32(sn, 0).ToString("X8"));
        }
        public static List<string> ProcessorsSerial()
        {
            ManagementObjectCollection mbsList = null;
            ManagementObjectSearcher mbs = new ManagementObjectSearcher("Select * From Win32_processor");
            mbsList = mbs.Get();
            List<string> id = new List<string>();
            foreach (ManagementObject mo in mbsList)
            {
                id.Add( mo["ProcessorID"] + "");
            }
            return id;
        }
        #endregion

        #region Volume Serial
        public static string VolumeSerial()
        {
            string volumeSerial = "";
            try
            {
                ManagementObject dsk = new ManagementObject(@"win32_logicaldisk.deviceid=""C:""");
                dsk.Get();
                volumeSerial = dsk["VolumeSerialNumber"] + "";
            }
            catch
            {
                try
                {
                    ManagementObject dsk = new ManagementObject(@"win32_logicaldisk.deviceid=""D:""");
                    dsk.Get();
                    volumeSerial = dsk["VolumeSerialNumber"] + "";
                }
                catch { File.WriteAllText("disk.mising", "need C or D"); Environment.Exit(0); }
            }
            return volumeSerial;
        }
        public static List<string> VolumesSerial()
        {
            String query = "SELECT * FROM Win32_DiskDrive";
            ManagementObjectSearcher mos = new ManagementObjectSearcher(query);
            ManagementObjectCollection moc = mos.Get();
            List<string> list = new List<string>();
            foreach (ManagementObject item in moc)
                list.Add(item["DeviceId"] + "");
            return list;
        }
        public static List<string> VolumesModel()
        {
            String query = "SELECT * FROM Win32_DiskDrive";
            ManagementObjectSearcher mos = new ManagementObjectSearcher(query);
            ManagementObjectCollection moc = mos.Get();
            List<string> list = new List<string>();
            foreach (ManagementObject item in moc)
                list.Add(item["Model"] + "");
            return list;
        }
        public static List<uint> VolumesPartitions()
        {
            String query = "SELECT * FROM Win32_DiskDrive";
            ManagementObjectSearcher mos = new ManagementObjectSearcher(query);
            ManagementObjectCollection moc = mos.Get();
            List<uint> list = new List<uint>();
            foreach (ManagementObject item in moc)
                list.Add(Convert.ToUInt32(item["Partitions"]));
            return list;
        }
        #endregion

        #region  Motherboard ID
        public static string MotherBoardSerial()
            {
                ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard");
                ManagementObjectCollection moc = mos.Get();
                string motherBoard = "";
                foreach (ManagementObject mo in moc)
                {
                    motherBoard = mo["SerialNumber"] + "";
                    if (!string.IsNullOrEmpty(motherBoard)) return motherBoard;
                }
                return motherBoard;
            }
        public static List<string> MotherBoardsSerial()
        {
            ManagementObjectSearcher mos = new ManagementObjectSearcher("SELECT * FROM Win32_BaseBoard");
            ManagementObjectCollection moc = mos.Get();
            List<string> motherBoard = new List<string>();
            foreach (ManagementObject mo in moc)
            {
                motherBoard.Add(mo["SerialNumber"] + "");
            }
            return motherBoard;
        }
        #endregion

        #region  Network
        public static IPAddress GetPublicIP()
        {
            string address = null;
            WebRequest request = WebRequest.Create("http://checkip.dyndns.org/");
            var t = request.GetResponseAsync();
            try
            {
                t.Wait(new TimeSpan(0, 0, 2));
                using (WebResponse response = t.Result)
                    if (response != null)
                        using (StreamReader stream = new StreamReader(response.GetResponseStream()))
                        {
                            address = stream.ReadToEnd();
                        }
            }
            catch { }
            if (string.IsNullOrWhiteSpace(address)) return null;
            int first = address.IndexOf("Address: ") + 9;
            int last = address.LastIndexOf("</body>");
            address = address.Substring(first, last - first);
            return ConvertService.ToIPAddress(address);
        }
        public static IPAddress GetExternalIP(string url)
        {
            return NetService.GetExternalIP(url);
        }
        public static string GetHostName()
        {
            return NetService.GetHostName();
        }
        public static List<IPAddress> GetInternalIPs(string hostName = null)
        {
            return NetService.GetInternalIPs(hostName);
        }
        public static IPAddress GetInternalIPv4(string hostName = null)
        {
            return NetService.GetInternalIPv4(hostName);
        }
        public static IPAddress GetInternalIPv6(string hostName = null)
        {
            return NetService.GetInternalIPv6(hostName);

        }
        public static PhysicalAddress GetMAC()
        {
            return NetService.GetMAC();

        }
        public static List<PhysicalAddress> GetMACs()
        {
            return NetService.GetMACs();

        }

        #endregion

        #endregion
    }
}
