using System;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using MiMFa;
using MiMFa.General;
using MiMFa.Exclusive.DateAndTime;
using MiMFa.Exclusive.Accessibility;
using MiMFa.Exclusive.ProgramingTechnology.DataBase;
using MiMFa.Service;
using System.IO;


namespace MiMFa.Exclusive.Language
{
    
    public static class Reader
    {
        #region Property
        private static string SignInternalParameter = "¶";
        private static string SignTryTranslate = "§";
        private static string SignNoTranslate = "▬";
        private static string SignFullTranslate = "↨";
        private static string SplitSign = " | ";
        private static LanguageMode _Language = LanguageMode.English;
        private static LanguageMode MainLanguage = LanguageMode.Null;
        private static Properties.Settings DIC = Properties.Settings.Default;
        public static PropertyToContentInjection PTCI = new PropertyToContentInjection('¶', Application.CompanyName);
        public static MessageMode MessageType = MessageMode.Message;
        public static RightToLeft RTL = RightToLeft.No;
        public static Image Flag = Properties.Resources.Flag_Iran;
        public static string Name = "English";
        public static bool IsRun = false;
        public static bool WithNormalization = true;
        public static bool WithWordSpace = true;
        public static string Conditions = " COLLATE NOCASE";
        private static bool _CaseSensitive = false;
        public static string ConfigurationTableName = "Configuration";
        public static string TextTableName = MessageMode.Message.ToString();
        public static string SuccessTableName = MessageMode.Success.ToString();
        public static string AlertTableName = MessageMode.Warning.ToString();
        public static string ErrorTableName = MessageMode.Error.ToString();
        public static string[] ResultSpliter = new string[] { SplitSign };
        public static SQLiteDataBase db = null;// new SQLiteDataBase(LanguagePath);

        public static string LanguageDirectory
        {
            get { return Config.ThisDirectory; }
            set
            {
                Config.ThisDirectory = value;
                Restart();
            }
        }
        public static LanguageMode Language
        {
            get { return _Language; }
            set
            {
                _Language = value;
                Restart();
            }
        }
        public static string LanguagePath { get; set; } = Config.ThisDirectory + Language + Config.LanguageExtension;
        public static bool CaseSensitive
        {
            get { return _CaseSensitive; }
            set
            {
                if (!(_CaseSensitive = value))
                {
                    if (!StringService.ExistAny(Conditions,false , "COLLATE NOCASE")) Conditions += " " + "COLLATE NOCASE";
                }
                else Conditions = Conditions.Replace("COLLATE NOCASE", "");
            }
        }
        #endregion

        #region Translate        
        
        /// <summary>
        /// if key start with:
        /// ¶ (Alt + 20): is Internal Parameter & Converted to than Parameter
        /// § (Alt + 21): if cant translated, no changed
        /// ▬ (Alt + 22): No Translate, No Change
        /// 
        /// Example: ¶FName => MyNameParameter
        /// Example: §FName => FirstName
        /// Example: ▬FName => FName
        /// </summary>
        /// <param name="Keys"></param>
        /// <returns></returns>  
        public static string GetReverseText(params string[] Keys)
        {
            if (!IsRun) Restart();
            string result = "";
            for (int i = 0; i < Keys.Length; i++)
            {
                string word = "";
                if (Keys[i].StartsWith(SignInternalParameter)) word = GetInternalParameter(Keys[i]);
                else if (Language == MainLanguage) word += Keys[i]
                    .Replace(SignInternalParameter, "")
                    .Replace(SignNoTranslate, "")
                    .Replace(SignFullTranslate, "")
                    .Replace(SignTryTranslate, "");
                else if (Keys[i].StartsWith(SignNoTranslate)) word = GetNoTranslate(Keys[i]);
                else if (Keys[i].StartsWith(SignFullTranslate)) word = GetReverseFullTranslate(Keys[i]);
                else if (DIC == null || string.IsNullOrEmpty(word = GetFromDIC(Keys[i]))) word = GetReverseTranslate(Keys[i]);
                result += word;
                if (i < Keys.Length - 1 && !string.IsNullOrEmpty(word.Trim()) && WithWordSpace) result += " ";
            }
            return GetInternalParameter(result);
        }

        /// <summary>
        /// if key start with:
        /// ¶ (Alt + 20): is Internal Parameter & Converted to than Parameter
        /// § (Alt + 21): if cant translated, no changed
        /// ▬ (Alt + 22): No Translate, No Change
        /// ↨ (Alt + 23): Full Translate
        /// 
        /// Example: ¶FName => MyNameParameter
        /// Example: §FName => FirstName
        /// Example: ▬FName => FName
        /// </summary> 
        public static string GetText(params string[] Keys)
        {
            return string.Join(" ", Keys);
            if (!IsRun) Restart();
            string result = "";
            for (int i = 0; i < Keys.Length; i++)
            {
                string word = "";
                if (string.IsNullOrEmpty(Keys[i])) continue;
                else if (Keys[i].StartsWith(SignInternalParameter)) word = GetInternalParameter(Keys[i]);
                else if (Language == MainLanguage) word += Keys[i]
                        .Replace(SignInternalParameter, "")
                        .Replace(SignNoTranslate, "")
                        .Replace(SignFullTranslate, "")
                        .Replace(SignTryTranslate, "");
                else if (Keys[i].StartsWith(SignNoTranslate)) word = GetNoTranslate(Keys[i]);
                else if (Keys[i].StartsWith(SignFullTranslate)) word = GetFullTranslate(Keys[i]);
                else if (DIC == null || string.IsNullOrEmpty(word = GetFromDIC(Keys[i]))) word = GetTranslate(Keys[i]);
                result += (WithNormalization) ? StringService.ReplaceWordsBetween(StringService.ReplaceWordsBetween(word, "(", ")", ""), "[", "]", "") : word;
                if (i < Keys.Length - 1 && !string.IsNullOrEmpty(word.Trim()) && WithWordSpace) result += " ";
            }
            return GetInternalParameter(result);
        }

        /// <summary>
        /// if key start with:
        /// ¶ (Alt + 20): is Internal Parameter & Converted to than Parameter
        /// § (Alt + 21): if cant translated, no changed
        /// ▬ (Alt + 22): No Translate, No Change
        /// ↨ (Alt + 23): Full Translate
        /// 
        /// Example: ¶FName => MyNameParameter
        /// Example: §FName => FirstName
        /// Example: ▬FName => FName
        /// </summary>
        public static string[] GetTextArray(params string[] Keys)
        {
            string[] stra = new string[Keys.Length];
            for (int i = 0; i < Keys.Length; i++)
                stra[i] = GetText(Keys[i]);
            return PTCI.CheckStringArray(stra);
        }

        /// <summary>
        /// if key start with:
        /// ¶ (Alt + 20): is Internal Parameter & Converted to than Parameter
        /// § (Alt + 21): if cant translated, no changed
        /// ▬ (Alt + 22): No Translate, No Change
        /// ↨ (Alt + 23): Full Translate
        /// 
        /// Example: ¶FName => MyNameParameter
        /// Example: §FName => FirstName
        /// Example: ▬FName => FName
        /// </summary>
        public static string GetText(MessageMode messageType, params string[] Keys)
        {
            MessageType = messageType;
            return GetText(Keys);
        }

        /// <summary>
        /// if key start with:
        /// ¶ (Alt + 20): is Internal Parameter & Converted to than Parameter
        /// § (Alt + 21): if cant translated, no changed
        /// ▬ (Alt + 22): No Translate, No Change
        /// ↨ (Alt + 23): Full Translate
        /// 
        /// Example: ¶FName => MyNameParameter
        /// Example: §FName => FirstName
        /// Example: ▬FName => FName
        /// </summary>
        public static string[] GetTextArray(MessageMode messageType, params string[] Keys)
        {
            MessageType = messageType;
            return GetTextArray(Keys);
        }

        public static string GetInternalParameter(string str)
        {
            return PTCI.CheckString(str);
        }
        public static string GetNoTranslate(string str)
        {
            return str.Replace(SignNoTranslate, "");
        }
        public static string GetFromDIC(string str)
        {
            foreach (var item in DIC.Properties)
                if ((item.ToString().ToLower() == str.Replace(SignInternalParameter, "").Replace(SignTryTranslate, "").Replace(SignNoTranslate, "").ToLower()))
                   return DIC[item.ToString()].ToString();
            return null;
        }
        public static string GetFullTranslate(string str)
        {
            str = str.Replace(SignFullTranslate, "");
            string[] sa = str.Split(new string[] { " " }, StringSplitOptions.None);
            List<KeyValuePair<bool, string>> dic = new List<KeyValuePair<bool, string>>();
            if (sa.Length > 1)
                for (int i = 0; i < sa.Length; i++)
                {
                    string s = GetText(sa[i]);
                    dic.Add(new KeyValuePair<bool, string>(s != sa[i], s));
                }
            else dic.Add(new KeyValuePair<bool, string>(false, str));
            for (int i = 0; i < dic.Count; i++)
                if (!dic[i].Key)
                {
                    sa = ConvertService.ToSeparatedWords(dic[i].Value).Split(new string[] { " " }, StringSplitOptions.None);
                    if (sa.Length > 1)
                    {
                        dic[i] = new KeyValuePair<bool, string>(true,"");
                        foreach (var itm in sa)
                            dic[i] = new KeyValuePair<bool, string>(dic[i].Key, dic[i].Value + " " + GetText(itm));
                    }
                }
            string word = "";
            foreach (var item in dic)
            {
                word += item.Value;
                if (!string.IsNullOrEmpty(item.Value.Trim()))
                    word += " ";
            }
            if (word.Length > 0) word = word.Substring(0, word.Length - 1);
            return word;
        }
        public static string GetTranslate(string str)
        {
            string tablename;
            switch (MessageType)
            {
                case MessageMode.Success:
                    tablename = SuccessTableName;
                    break;
                case MessageMode.Warning:
                    tablename = AlertTableName;
                    break;
                case MessageMode.Error:
                    tablename = ErrorTableName;
                    break;
                default:
                    tablename = TextTableName;
                    break;
            }
            try
            {
                string ss = "";
                object obj = null;
                obj = db.GetValue(tablename, str, Conditions);
                if (obj != null)
                    return obj.ToString().Split(ResultSpliter, StringSplitOptions.None)[0];
                else if (str.StartsWith(SignTryTranslate))
                    if ((obj = db.GetValue(tablename, str = str.Replace(SignTryTranslate, ""), Conditions)) != null)
                        return obj.ToString().Split(ResultSpliter, StringSplitOptions.None)[0];
                    else return str;
                else if ((obj = db.GetValue(tablename, ss = ConvertService.ToSeparatedWords(str), Conditions)) != null)
                    return obj.ToString().Split(ResultSpliter, StringSplitOptions.None)[0];
                if (ss.Contains(" ")) return GetFullTranslate(str);
            }
            catch { }
            return str;
        }

        public static string GetReverseFullTranslate(string str)
        {
            string word = "";
            foreach (var item in (ConvertService.ToSeparatedWords(str.Replace(SignFullTranslate, ""))).Split(' '))
            {
                var it = GetReverseText(item);
                word += it;
                if (!string.IsNullOrEmpty(it.Trim()))
                    word += " ";
            }
            return word;
        }
        public static string GetReverseTranslate(string str)
        {
            string tablename;
            switch (MessageType)
            {
                case MessageMode.Success:
                    tablename = SuccessTableName;
                    break;
                case MessageMode.Warning:
                    tablename = AlertTableName;
                    break;
                case MessageMode.Error:
                    tablename = ErrorTableName;
                    break;
                default:
                    tablename = TextTableName;
                    break;
            }
            object obj = null;
                obj = db.GetKeys(tablename, str, Conditions)[0];
            if (obj != null)
                return obj.ToString().Split(new string[] { SplitSign }, StringSplitOptions.None)[0];
            else if (str.StartsWith(SignTryTranslate) && (obj = db.GetKeys(tablename,str = str.Replace(SignTryTranslate, ""), Conditions)) != null)
                return ((object[])obj)[0].ToString().Split(ResultSpliter, StringSplitOptions.None)[0];
            else if ((obj = db.GetKeys(tablename, ConvertService.ToSeparatedWords(str), Conditions)[0]) != null)
                return obj.ToString().Split(new string[] { SplitSign }, StringSplitOptions.None)[0];
            else return GetReverseFullTranslate(str);
        }
        #endregion

        #region Config
        public static void Restart()
        {
            try
            {
                IsRun = true;
                //db.ShowException = false;
                db.Start(LanguagePath);
                db.CreateDic(ConfigurationTableName, "BLOB");
                db.CreateDic(SuccessTableName, "TEXT");
                db.CreateDic(ErrorTableName, "TEXT");
                db.CreateDic(AlertTableName, "TEXT");
                db.CreateDic(TextTableName, "TEXT");
                //SaveConfiguration();
                UpdateConfiguration();
            }
            catch { }
        }
        public static void ChangeLanguage(LanguageMode language)
        {
            LanguagePath = Config.ThisDirectory + language + Config.LanguageExtension;
            Language = language;
            Restart();
        }
        public static void ChangeLanguage(string language)
        {
            LanguagePath = Config.ThisDirectory + language + Config.LanguageExtension;
            Restart();
        }
        public static void ChangeLanguagePath(string path)
        {
            LanguagePath = path;
            Restart();
        }
        public static bool SetValue(string key, object value, MessageMode messageType = MessageMode.Message)
        {
            if (!IsRun) Restart();
            string mt;
            switch (MessageType)
            {
                case MessageMode.Success:
                    mt = SuccessTableName;
                    break;
                case MessageMode.Warning:
                    mt = AlertTableName;
                    break;
                case MessageMode.Error:
                    mt = ErrorTableName;
                    break;
                default:
                    mt = TextTableName;
                    break;
            }
            try { db.SetValue(mt, key, value); return true; } catch { return false; }
        }

        public static void SaveConfiguration()
        {
            try
            {
                db.SetValue(ConfigurationTableName, "Name", Name, true);
                db.SetValue(ConfigurationTableName, "Language", Language, true);
                db.SetValue(ConfigurationTableName, "RightToLeft", RTL, true);
                db.SetValue(ConfigurationTableName, "Flag", ConvertService.ToByteArray(Flag));
            }
            catch { }
        }
        public static void UpdateConfiguration()
        {
            try
            {
                Name = db.GetValue(ConfigurationTableName, "Name", true).ToString();
                _Language = (LanguageMode)db.GetValue(ConfigurationTableName, "Language", true);
                RTL = (RightToLeft)db.GetValue(ConfigurationTableName, "RightToLeft", true);
                Flag = (Bitmap)db.GetValue(ConfigurationTableName, "Flag");
            }
            catch{ }
        }

        #endregion
    }
}

