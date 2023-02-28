using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MiMFa.General;
using MiMFa.Service;
using System.Reflection;

namespace MiMFa.Exclusive.Collection.Instance
{
    public class ApplicationDirectories : Config
    {
        public string SecuritySetTableName = "Security";
        public string SystemSetTableName = "System";
        public string ViewSetTableName = "View";

        public string dbPath;
        public string dbDataPath;
        public string dbSettingPath;
        public string dbTemporaryPath;
        public string dbBackupPath;

        public string TableName = "MFT";
        public string HeavyTableName = "HeavyData";
        public string TempTableName = "Temp";
        public string LogTableName = "Log";

        public string LogRecoveryFileAddress;
        public string UILConfigurationAddress;
        public string CLLConfigurationAddress;
        public string MFLConfigurationAddress;
        public string SHIConfigurationAddress;
        public string FrameWorksConfigurationAddress;
        public string ArchiveReportStyleAddress;

        public string DefaultAddress;
        public string FullFileAddress;

        public new string ThisDirectory { get; set; }
        public new string ConfigurationDirectory { get; set; }
        public new string TempDirectory { get; set; }
        public new string LogDirectory { get; set; }
        public string DataTempDirectory { get; set; }
        public virtual string DataDirectory { get; set; }
        public virtual string BaseDirectory { get; set; }
        public virtual string LanguageDirectory { get; set; }
        public virtual string ViewDirectory { get; set; }
        public virtual string FileDirectory { get; set; }
        public virtual string TemplateDirectory { get; set; }
        public virtual string ComponentDirectory { get; set; }
        public virtual string PluginDirectory { get; set; }

        public virtual string DefaultDirectory { get; set; }
        public virtual string ThemeDirectory { get; set; }

        public ApplicationDirectories() : base()
        {
            DefaultValues();
            CreateAllPath();
        }
        public override void DefaultValues()
        {
            string sep = System.IO.Path.DirectorySeparatorChar.ToString();
            ThisDirectory = ApplicationDirectory;
            base.DefaultValues();
            ThisDirectory = ApplicationDirectory;
            ConfigurationDirectory = ThisDirectory + @"Configuration"+sep;
            TempDirectory = ThisDirectory + @"Temp"+sep;
            LogDirectory = ThisDirectory + @"Log"+sep;
            DataDirectory = ThisDirectory + @"Data"+sep;
            BaseDirectory = ThisDirectory + @"Base"+sep;
            LanguageDirectory = ThisDirectory + @"Language"+sep;
            ViewDirectory = ThisDirectory + @"View"+sep;
            FileDirectory = ThisDirectory + @"My Files"+sep;
            TemplateDirectory = ThisDirectory + @"Template"+sep;
            ComponentDirectory = ThisDirectory + @"Component"+sep;
            PluginDirectory = ThisDirectory + @"Plugin"+sep;

            DataTempDirectory = TempDirectory + @"Data"+sep;
            DefaultDirectory = ConfigurationDirectory + @"Default"+sep;
            ThemeDirectory = ViewDirectory + @"Theme"+sep;

            dbPath = DataDirectory + @"Main" + DataBaseExtension;
            dbDataPath = DataDirectory + @"Data" + DataBaseExtension;
            dbSettingPath = DataDirectory + @"Setting" + DataBaseExtension;
            dbTemporaryPath = DataDirectory + @"Temporary" + DataBaseExtension;
            dbBackupPath = DataDirectory + @"Backup" + DataBaseExtension;


            LogRecoveryFileAddress = LogDirectory + @"RecoveryFileAddress" + LogExtension;
            UILConfigurationAddress = ConfigurationDirectory + @"UILC" + ConfigurationExtension;
            CLLConfigurationAddress = ConfigurationDirectory + @"CLLC" + ConfigurationExtension;
            MFLConfigurationAddress = ConfigurationDirectory + @"MFLC" + ConfigurationExtension;
            SHIConfigurationAddress = ConfigurationDirectory + @"SHIC" + ConfigurationExtension;
            FrameWorksConfigurationAddress = ConfigurationDirectory + @"MFWC" + ConfigurationExtension;
            ArchiveReportStyleAddress = DefaultDirectory + @"Archive" + ProgramingTechnology.ReportLanguage.ReportStyle.Extension;

            DefaultAddress = FileDirectory + FileName + FileExtension;
            FullFileAddress = null;
        }


        public virtual  string[] GetOpenFileLogs() => MiMFa.Service.IOService.FileToStringArray(LogRecoveryFileAddress);
        public virtual  void SetOpenFileLogs(string[] files) => MiMFa.Service.IOService.StringArrayToFile(LogRecoveryFileAddress, files);
        public  virtual void AppendOpenFileLogs(string file) => MiMFa.Service.IOService.StringNewLineAppendFile(LogRecoveryFileAddress, file);

        public virtual Dictionary<string, string> GetDicOfRecoveryData()
        {
            string[] files = System.IO.Directory.GetFiles(DataTempDirectory);
            Dictionary<string, string> dic = new Dictionary<string, string>();
            if (files.Length > 1)
            {
                for (int i = 0; i < files.Length; i++)
                {
                    var fi = new System.IO.FileInfo(files[i]);
                    if (!PathService.IsUsingByProccess(files[i]))
                        dic.Add(files[i], (dic.Count + 1) + "- " + Exclusive.Language.Reader.GetText("Date") + " " + ConvertService.ToMiMFaDate(fi.LastAccessTime).GetDate() + " " + Exclusive.Language.Reader.GetText("On Clock") + " " + ConvertService.ToMiMFaTime(fi.LastAccessTime).GetTime() + " ---> (" + fi.Length + " byte)");
                }
            }
            return dic;
        }
    }
}
