using MiMFa.Model;
using MiMFa.Model.Structure;
using MiMFa.General;
using MiMFa.Model;
using MiMFa.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;

namespace MiMFa
{
    [Serializable]
    public class Config
    {
        public static string ApplicationName { get; set; } = AppDomain.CurrentDomain.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar).Split(Path.DirectorySeparatorChar).Last();
        public static string ApplicationDirectory { get; set; } = AppDomain.CurrentDomain.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar)+ Path.DirectorySeparatorChar.ToString();
        public static string ApplicationDataDirectory => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar.ToString() + "MiMFa" + Path.DirectorySeparatorChar.ToString();


        public static string TradeMark = "MiMFa Software ©";
        public static string Contact = "09014841090";
        public static string URL = "https://mimfa.net";
        public static string ProductSignature = "MiMFa";
        public static string ProductURL { get => _ProductURL ?? URL + "/software/" + ProductSignature; set => _ProductURL = value; }
        static string _ProductURL = null;
        public static string ProductHelpURL { get => _ProductHelpURL ?? ProductURL + "/help"; set => _ProductHelpURL = value; }
        static string _ProductHelpURL = null;
        public static string ProductSupportURL { get => _ProductSupportURL ?? ProductURL + "/support"; set => _ProductSupportURL = value; }
        static string _ProductSupportURL = null;
        public static string ProductSponsorURL { get => _ProductSponsorURL ?? ProductURL + "/sponsor"; set => _ProductSponsorURL = value; }
        static string _ProductSponsorURL = null;
        public static string ProductSponsor0URL { get => _ProductSponsor0URL ?? ProductSponsorURL + "/sponsor.png"; set => _ProductSponsor0URL = value; }
        static string _ProductSponsor0URL = null;
        public static string ProductSponsor1URL { get => _ProductSponsor1URL ?? ProductSponsorURL + "/sponsor1.png"; set => _ProductSponsor1URL = value; }
        static string _ProductSponsor1URL = null;
        public static string ProductSponsor2URL { get => _ProductSponsor2URL ?? ProductSponsorURL + "/sponsor2.png"; set => _ProductSponsor2URL = value; }
        static string _ProductSponsor2URL = null;
        public static string ProductActivateURL{ get => _ProductActivateURL ?? URL + "/application/activate.php"; set => _ProductActivateURL = value; }
        static string _ProductActivateURL = null;
        public static string ProductUpdateURL { get => _ProductUpdateURL ?? URL + "/application/update.php"; set => _ProductUpdateURL = value; }
        static string _ProductUpdateURL = null;
        public static string ProductLogURL { get => _ProductLogURL ?? URL + "/application/log.php"; set => _ProductLogURL = value; }
        static string _ProductLogURL = null;


        public static string FileExtension { get; set; } = ".mft";
        public static string LibraryExtension { get; set; } = ".dll";
        public static string ConfigurationExtension { get; set; } = ".cnf";
        public static string ExecutableExtension { get; set; } = ".exe";
        public static string DataBaseExtension { get; set; } = ".data";
        public static string LanguageExtension { get; set; } = ".lng";
        public static string TemplateExtension { get; set; } = ".thm";
        public static string PlugInExtension { get; set; } = ".plg";
        public static string ScriptExtension { get; set; } = ".js";
        public static string AttachExtension { get; set; } = ".attach";
        public static string HTMLExtension { get; set; } = ".htm";
        public static string CssExtension { get; set; } = ".css";
        public static string LogExtension { get; set; } = ".log";
        public static string TempExtension { get; set; } = ".tmp";

        public static string LogFileDialogFilter => "Log Files (*"+ LogExtension + ") | *"+ LogExtension;
        public static string CssFileDialogFilter => "Cascading Style Sheets Files (*" + CssExtension + ") | *"+ CssExtension;
        public static string HTMLFileDialogFilter => "Hyper Text Murkup Languages Files (*" + HTMLExtension + ") | *"+ HTMLExtension;
        public static string ScriptFileDialogFilter => "Script Files (*" + ScriptExtension + ") | *"+ ScriptExtension;

        public static string PhotoFileDialogFilter = "All Photo Files (*.jpg, *.jpeg, *.png, *.gif) | *.jpg;*.jpeg;*.png;*.gif;";
        public static string ImageFileDialogFilter = "All Image Files (*.jpg, *.jpeg, *.png, *.bmp, *.gif, *.ico)|*.jpg; *.jpeg; *.png; *.bmp; *.gif; *.ico";

        public string FileName = "My File";
        public string DefaultName = "Default";

        public static string ConfigurationSeparator { get; set; } = "-->";
        public static string ConfigurationPath { get; set; } = ConfigurationDirectory + "Config" + ConfigurationExtension;
        public static SmartDictionary<string, string> Configurations
        {
            get { return File.Exists(ConfigurationPath)? IOService.ReadSmartDictionary(ConfigurationPath, ConfigurationSeparator) : new SmartDictionary<string, string>(); }
            set { IOService.WriteDictionary(ConfigurationPath,CollectionService.Sort(value), ConfigurationSeparator); }
        }
        public static SmartDictionary<string, string> AddConfiguration(string key, object value)
        {
            var dic = Configurations;
            dic.AddOrSet(key, value + "");
            return Configurations = dic;
        }
        public static SmartDictionary<string, string> AddConfiguration(params KeyValuePair<string, string>[] keyValues)
        {
            var dic = Configurations;
            foreach (var kvp in keyValues) dic.AddOrSet(kvp.Key, kvp.Value);
            return Configurations = dic;
        }
        public static SmartDictionary<string, string> RemoveConfiguration(params string[] keys)
        {
            var dic = Configurations;
            foreach (var key in keys) dic.Remove(key);
            return Configurations = dic;
        }

        public static string Language
        {
            get
            {
                try
                {
                    return Configurations["Language"].Trim();
                }
                catch { return "Default"; }
            }
            set
            {
                var cd = Configurations;
                try
                {
                    cd["Language"] = Path.GetFileNameWithoutExtension(value);
                }
                catch { cd.Add("Language", Path.GetFileNameWithoutExtension(value)); }
                finally { Configurations = cd; }
            }
        }
        public static IEnumerable<string> Languages => from v in LanguagesPaths select Path.GetFileNameWithoutExtension(v);
        public static IEnumerable<string> LanguagesPaths =>  Directory.GetFiles(LanguageDirectory, "*" + LanguageExtension, SearchOption.AllDirectories);

        public static TimeZoneMode TimeZone
        {
            get
            {
                try
                {
                    return (TimeZoneMode)Enum.Parse(typeof(TimeZoneMode), Configurations["TimeZone"].Trim());
                }
                catch { return Default.DateTime.TimeZone; }
            }
            set
            {
                var cd = Configurations;
                try
                {
                    cd["TimeZone"] = value.ToString();
                }
                catch { cd.Add("TimeZone", value.ToString()); }
                finally { Configurations = cd; }
            }
        }
        public static string Template
        {
            get
            {
                try
                {
                    return Configurations["Template"];
                }
                catch { return string.Empty; }
            }
            set
            {
                var cd = Configurations;
                try
                {
                    cd["Template"] = value;
                }
                catch { cd.Add("Template", value); }
                finally { Configurations = cd; }
            }
        }
        public static int ServerPort
        {
            get
            {
                try
                {
                    return Convert.ToInt32(Configurations["ServerPort"]);
                }
                catch { return 7911; }
            }
            set
            {
                var cd = Configurations;
                try
                {
                    cd["ServerPort"] = value + "";
                }
                catch { cd.Add("ServerPort", value + ""); }
                finally { Configurations = cd; }
            }
        }
        public static int ClientPort
        {
            get
            {
                try
                {
                    return Convert.ToInt32(Configurations["ClientPort"]);
                }
                catch { return 7912; }
            }
            set
            {
                var cd = Configurations;
                try
                {
                    cd["ClientPort"] = value + "";
                }
                catch { cd.Add("ClientPort", value + ""); }
                finally { Configurations = cd; }
            }
        }
        public static int BufferSize
        {
            get
            {
                try
                {
                    return Convert.ToInt32(Configurations["BufferSize"]);
                }
                catch { return 200 * 1024; }
            }
            set
            {
                var cd = Configurations;
                try
                {
                    cd["BufferSize"] = value + "";
                }
                catch { cd.Add("BufferSize", value + ""); }
                finally { Configurations = cd; }
            }
        }
        public static string DataBaseDirectory
        {
            get
            {
                try
                {
                    return Configurations["DataBaseDirectory"].Replace("|DataDirectory|\\", ApplicationDirectory); //Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)));
                }
                catch { return string.Empty; }
            }
            set
            {
                var cd = Configurations;
                try
                {
                    cd["DataBaseDirectory"] = value;
                }
                catch { cd.Add("DataBaseDirectory", value); }
                finally { Configurations = cd; }
            }
        }
        public static string MainDataBaseAdderss
        {
            get
            {
                try
                {
                    return Configurations["MainDataBaseAdderss"].Replace("|DataDirectory|\\", ApplicationDirectory);
                }
                catch { return string.Empty; }
            }
            set
            {
                var cd = Configurations;
                try
                {
                    cd["MainDataBaseAdderss"] = value;
                }
                catch { cd.Add("MainDataBaseAdderss", value); }
                finally { Configurations = cd; }
            }
        }

        public static string FrameworkAddress
        {
            get
            {
                try
                {
                    string str = Configurations["MiMFa Framework"].Replace("|DataDirectory|\\", ApplicationDirectory);
                    if(string.IsNullOrEmpty(str)) return ApplicationDirectory;
                    return str;
                }
                catch { return ApplicationDirectory; }
            }
            set
            {
                var cd = Configurations;
                try
                {
                    cd["MiMFa Framework"] = value;
                }
                catch { cd.Add("MiMFa Framework", value); }
                finally { Configurations = cd; }
            }
        }

        public static string UpdatePath = ApplicationDirectory + "UpdatePackage" + ExecutableExtension;

        public static string ThisDirectory { get;  set; }
        public static string ConfigurationDirectory { get;  set; }
        public static string LanguageDirectory { get;  set; }
        public static string PackageDirectory { get;  set; }
        public static string LibraryDirectory { get;  set; }
        public static string TemporaryDirectory { get;  set; }
        public static string LogDirectory { get;  set; }
        public static string HelpDirectory { get; set; }

        public static bool Initialized { get; set; } = false;

        public static void Initial()
        {
            if (Initialized) Final();
            string key = @"HKEY_CURRENT_USER\SOFTWARE\Microsoft\Internet Explorer\MAIN\FeatureControl\FEATURE_BROWSER_EMULATION";
            var value = 0x22B8;

            Microsoft.Win32.Registry.SetValue(key, System.AppDomain.CurrentDomain.FriendlyName, value,Microsoft.Win32.RegistryValueKind.DWord);
            #if DEBUG 
                Microsoft.Win32.Registry.SetValue(key, System.AppDomain.CurrentDomain.FriendlyName.Replace(".exe", ".vshost.exe"), value, Microsoft.Win32.RegistryValueKind.DWord);
            #endif
            DefaultValues();
            CreateAllDirectories();
            Initialized = true;
        }
        public static void Final()
        {
            try
            {
                if (Directory.Exists(TemporaryDirectory)) Directory.Delete(TemporaryDirectory, true);
                Initialized = false;
            }
            catch { }
        }
        public static void DefaultValues()
        {
            string separator = Path.DirectorySeparatorChar.ToString();
            ThisDirectory = FrameworkAddress + (new Config()).GetType().Namespace.Replace(".", separator ).Replace("_"," ") +separator;
            ConfigurationDirectory = ApplicationDirectory +  "Configuration"+ separator;
            LanguageDirectory = ApplicationDirectory + "Language" + separator;
            HelpDirectory = ApplicationDirectory + "Help" + separator;
            PackageDirectory = ApplicationDirectory + "Package" + separator;
            LibraryDirectory = ApplicationDirectory + "Library" + separator;
            TemporaryDirectory = ApplicationDataDirectory + "Temporary" + separator;
            LogDirectory = ApplicationDataDirectory + "Log" + separator;
            UpdatePath = ApplicationDataDirectory + "UpdatePackage" + ExecutableExtension;
            ConfigurationPath= ConfigurationDirectory + "Configurations" + ConfigurationExtension;
        }
        public static void CreateAllDirectories()
        {
            var th = new Config();
            PropertyInfo[] pi = th.GetType().GetProperties();
            for (int i = 0; i < pi.Length; i++)
                if(pi[i].Name.EndsWith("Directory"))
                    try { PathService.CreateAllDirectories(pi[i].GetValue(th).ToString()); }
                catch { }
            PathService.DeleteAllFilesInAllDirectoriesInPath(TemporaryDirectory);
        }

        public static void RefreshTempDirectory()
        {
            try
            {
                if (Directory.Exists(TemporaryDirectory)) Directory.Delete(TemporaryDirectory, true);
                Directory.CreateDirectory(TemporaryDirectory);
            }
            catch { }
        }

        public static void EnforceAdminPrivileges()
        {
            RegistryKey rk;
            string registryPath = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon\";

            try
            {
                if (Environment.Is64BitOperatingSystem)
                {
                    rk = RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, RegistryView.Registry64);
                }
                else
                {
                    rk = RegistryKey.OpenBaseKey(Microsoft.Win32.RegistryHive.LocalMachine, RegistryView.Registry32);
                }

                rk = rk.OpenSubKey(registryPath, true);
            }
            catch (System.Security.SecurityException ex)
            {
                throw new Exception("Please run as administrator");
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
