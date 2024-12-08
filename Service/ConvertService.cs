using System;
using System.Linq;
using System.Data;
using System.IO;
using System.Reflection;
using System.Drawing;
using System.Globalization;
using System.Collections.Generic;
using MiMFa.Service;
using MiMFa.Exclusive.DateAndTime;
using MiMFa.General;
using MiMFa.Model.Structure;
using MiMFa.Controls.WinForm.ButtonPack;
using MiMFa.Model;
using System.Collections;
using System.Windows.Forms;
using System.Drawing.Imaging;
using System.Windows.Documents;
using System.Threading;
using MiMFa.Exclusive.ProgramingTechnology.Tools.Pickup;
using System.Text.RegularExpressions;
using System.Data.OleDb;
using System.Xml;
using MiMFa.Model.IO;
using System.Net;
using MiMFa.Intermediate.HotKey;
using System.Drawing.Text;

namespace MiMFa.Service
{
    public class ConvertService
    {
        #region MiMFa Type
        public static Dictionary<string, string> ToDictionary<T>(List<T> lt) where T : StructureBase
        {
            return (
                from v in lt
                select new { Key = v.UID, Value = v.Name }
                ).ToDictionary(x => x.Key, x => x.Value);
        }
        public static Dictionary<string, string> ToDictionary<T>(T[] lt) where T : StructureBase
        {
            return (
                from v in lt
                select new { Key = v.UID, Value = v.Name }
                ).ToDictionary(x => x.Key, x => x.Value);
        }
      
        public static DateTime ToDateTime(string op)
        {
            if (string.IsNullOrWhiteSpace(op)) return default;
            string[] stra = op.Trim().Split(' ');
            if (stra.Length == 2)
            {
                var d = ToSmartDate(stra[0]);
                var t = ToSmartTime(stra[1]);
                if (d.Month > 12 || d.Month < 1 || d.Day > 31 || d.Day < 1)
                    d = ToSmartDate(stra[1]);
                if (t.Hour > 23 || t.Hour < 0 || t.Minute > 59 || t.Minute < 0 || t.Second > 59 || t.Second < 0)
                    t = ToSmartTime(stra[0]);
                if (d.Month > 12 || d.Month < 1 || d.Day > 31 || d.Day < 1)
                    d = ToSmartDate(stra[0]);
                if (t.Hour > 23 || t.Hour < 0 || t.Minute > 59 || t.Minute < 0 || t.Second > 59 || t.Second < 0)
                    t = ToSmartTime(stra[1]);
                return new DateTime(d.Year, d.Month, d.Day, (int)t.Hour, (int)t.Minute, (int)t.Second);
            }
            else
            {
                var d = ToSmartDate(op);
                var t = ToSmartTime(op);
                if (t.Hour > 23 || t.Minute > 59 || t.Second > 59) t = t = new SmartTime();
                if (d.Month > 12 || d.Day > 31) d = new SmartDate();
                return new DateTime(d.Year, d.Month, d.Day, (int)t.Hour, (int)t.Minute, (int)t.Second);
            }
        }
        public static DateTime ToDateTime(SmartDate date)
        {
            return date.GetDateTime();
        }
        public static DateTime ToDateTime(SmartTime time)
        {
            return time.GetDateTime();
        }

        public static SmartDateTime ToSmartDateTime(string op)
        {
            return new SmartDateTime(ToDateTime(op));
        }
        public static SmartDateTime ToSmartDateTime(DateTime op)
        {
            return new SmartDateTime(op);
        }
        public static SmartDateTime ToSmartDateTime(SmartDate date)
        {
            return new SmartDateTime(ToDateTime(date));
        }
        public static SmartDateTime ToSmartDateTime(SmartTime time)
        {
            return new SmartDateTime(ToDateTime(time));
        }

        public static SmartDate ToSmartDate(string op1)
        {
            string[] sta = op1.Replace(" ", "/").Split(new char[]{ '/', '-', '_', ',', ':', '\\', '.', ';' }, StringSplitOptions.RemoveEmptyEntries);
            if (sta.Length > 2)
            {
                int num0 = int.Parse(sta[0]);
                int num1 = int.Parse(sta[1]);
                int num2 = int.Parse(sta[2]);
                if (sta[2].Length >= sta[0].Length)
                {
                   if(num1 < 13) return new SmartDate(num2, num1, num0);
                   else return new SmartDate(num2, num0, num1);
                }
                return new SmartDate(num0, num1, num2);
            }
            if (sta.Length > 1) return new SmartDate(0, int.Parse(sta[1]), int.Parse(sta[0]));
            var v = new SmartDate(1, 1, 0);
            if (sta.Length >0) v.IncrimentDay(int.Parse(sta[0]));
            return v;
        }
        public static SmartDate ToSmartDate(DateTime dateTime)
        {
            return new SmartDate(dateTime.Year, dateTime.Month, dateTime.Day);
        }
        
        public static SmartTime ToSmartTime(string op1)
        {
            string[] sta = op1.Replace(" ", "/").Split(new char[] { ':', '-', '_', ',', '/', '\\', '.', ';' },StringSplitOptions.RemoveEmptyEntries);
            if (sta.Length > 2) return new SmartTime(int.Parse(sta[0]), int.Parse(sta[1]), int.Parse(sta[2]));
            if (sta.Length > 2) return new SmartTime(0, int.Parse(sta[0]), int.Parse(sta[1]));
            var v = new SmartTime(0, 0, 0);
            if (sta.Length > 0) v.Second = int.Parse(sta[0]);
            return v;
        }
        public static SmartTime ToSmartTime(DateTime dateTime)
        {
            return new SmartTime(dateTime.Hour, dateTime.Minute, dateTime.Second);
        }

        public static int ToHijriDayOfWeekNum(DayOfWeek dow)
        {
            switch (dow)
            {
                case DayOfWeek.Saturday:
                    return 0;
                case DayOfWeek.Sunday:
                    return 1;
                case DayOfWeek.Monday:
                    return 2;
                case DayOfWeek.Tuesday:
                    return 3;
                case DayOfWeek.Wednesday:
                    return 4;
                case DayOfWeek.Thursday:
                    return 5;
                case DayOfWeek.Friday:
                    return 6;
                default: return (int)dow;
            }
        }
        public static int ToDayOfWeekNum(SmartDate date, TimeZoneMode timeZone = TimeZoneMode.IranStandard)
        {
            SmartDateTime dt = new SmartDateTime();
            dt.TimeZone = timeZone;
            return ToDayOfWeekNum(date, dt);
        }
        public static int ToDayOfWeekNum(SmartDate date, SmartDateTime dateTime)
        {
            SmartDate now = dateTime.GetSmartDate();
            int dow = dateTime.GetDayOfWeekNumber();
            int toleranse = now.GetLengthDay(date);
            int day = Math.Abs(toleranse % 7);
            if (toleranse > 0)//اگر تاریخ بعد از امروز بود
                for (int i = 0; i < day; i++)
                    if (++dow > 6) dow = 0;
                    else continue;
            else for (int i = 0; i < day; i++)
                    if (--dow < 0) dow = 6;
                    else continue;
            return dow;
        }

        public static string ToDayOfWeekName(SmartDate date, TimeZoneMode timeZone = TimeZoneMode.IranStandard)
        {
            SmartDateTime dt = new SmartDateTime();
            dt.TimeZone = timeZone;
            return ToDayOfWeekName(date, dt);
        }
        public static string ToDayOfWeekName(int DayNum, SmartDateTime dateTime)
        {
            if (dateTime.ZoneCalendar is PersianCalendar)
                return ToPersianDayOfWeek(DayNum);
            else if (dateTime.ZoneCalendar is HijriCalendar)
                return ToArabicDayOfWeek(DayNum);
            else
                return ToEnglishDayOfWeek(DayNum);
        }
        public static string ToDayOfWeekName(SmartDate date, SmartDateTime dateTime)
        {
            int dayOfWeek = ToDayOfWeekNum(date, dateTime);
            if (dateTime.ZoneCalendar is PersianCalendar)
                return ToPersianDayOfWeek(dayOfWeek);
            else if (dateTime.ZoneCalendar is HijriCalendar)
                return ToArabicDayOfWeek(dayOfWeek);
            else
                return ToEnglishDayOfWeek(dayOfWeek);
        }
        public static string ToPersianDayOfWeek(int dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case 0:
                    return "شنبه";
                case 1:
                    return "یکشنبه";
                case 2:
                    return "دوشنبه";
                case 3:
                    return "سه شنبه";
                case 4:
                    return "چهارشنبه";
                case 5:
                    return "پنجشنبه";
                case 6:
                    return "آدینه";
            }
            return null;
        }
        public static string ToArabicDayOfWeek(int dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case 0:
                    return "السبت";
                case 1:
                    return "الأحد";
                case 2:
                    return "الاثنين";
                case 3:
                    return "الثلاثاء";
                case 4:
                    return "الاربعاء";
                case 5:
                    return "الخميس";
                case 6:
                    return "الجمعة";
            }
            return null;
        }

        public static string ToEnglishDayOfWeek(int dayOfWeek)
        {
            switch (dayOfWeek)
            {
                case 0:
                    return "Saturday";
                case 1:
                    return "Sunday";
                case 2:
                    return "Monday";
                case 3:
                    return "Tuesday";
                case 4:
                    return "Wednesday";
                case 5:
                    return "Thursday";
                case 6:
                    return "Friday";
            }
            return null;
        }
        public static string ToMonthName(SmartDate date, SmartDateTime dateTime)
        {
            if (dateTime.ZoneCalendar is PersianCalendar)
                return ToPersianMonth(date.Month);
            else if (dateTime.ZoneCalendar is HijriCalendar)
                return ToArabicMonth(date.Month);
            else
                return ToEnglishMonth(date.Month);
        }
        public static string ToPersianMonth(int month)
        {
            switch (month)
            {
                case 1:
                    return "فروردین";
                case 2:
                    return "اردیبهشت";
                case 3:
                    return "خرداد";
                case 4:
                    return "تیر";
                case 5:
                    return "مرداد";
                case 6:
                    return "شهریور";
                case 7:
                    return "مهر";
                case 8:
                    return "آبان";
                case 9:
                    return "آذر";
                case 10:
                    return "دی";
                case 11:
                    return "بهمن";
                case 12:
                    return "اسفند";
            }
            return null;
        }
        public static string ToArabicMonth(int month)
        {
            switch (month)
            {
                case 1:
                    return "محرم";
                case 2:
                    return "صفر";
                case 3:
                    return "ربیع‌الاول";
                case 4:
                    return "ربیع‌الثانی";
                case 5:
                    return "جمادی‌الاول";
                case 6:
                    return "جمادی‌الثانی";
                case 7:
                    return "رجب";
                case 8:
                    return "شعبان";
                case 9:
                    return "رمضان";
                case 10:
                    return "شوال";
                case 11:
                    return "ذیقعده";
                case 12:
                    return "ذیحجه";
            }
            return null;
        }
        public static string ToEnglishMonth(int month)
        {
            switch (month)
            {
                case 1:
                    return "January";
                case 2:
                    return "February";
                case 3:
                    return "March";
                case 4:
                    return "April";
                case 5:
                    return "May";
                case 6:
                    return "June";
                case 7:
                    return "July";
                case 8:
                    return "August";
                case 9:
                    return "September";
                case 10:
                    return "October";
                case 11:
                    return "November";
                case 12:
                    return "December";
            }
            return null;
        }

        public static Controls.WinForm.ButtonPack.ButtonAction ToButtonAction(object obj)
        {
            Controls.WinForm.ButtonPack.ButtonAction ba = new Controls.WinForm.ButtonPack.ButtonAction();
            if (obj != null)
                try { ba = (Controls.WinForm.ButtonPack.ButtonAction)obj; }
                catch { }
            return ba;
        }

        #endregion

        #region *Image*
        public static Graphics ToGraphics(Bitmap bitmap)
        {
            Size s = bitmap.Size;
            Graphics memoryGraphics = Graphics.FromImage(bitmap);
            return memoryGraphics;
        }
        public static Icon ToIcon(Image image)
        {
            var thumb = (Bitmap)image.GetThumbnailImage(64, 64, null, IntPtr.Zero);
            thumb.MakeTransparent();
            return Icon.FromHandle(thumb.GetHicon());
        }
        public static Image ToImage(Icon icon)
        {
            try { return icon.ToBitmap(); } catch { }
            return null;
        }
        public static Image ToImage(Bitmap bm)
        {
            return Image.FromHbitmap(bm.GetHbitmap());
        }
        #endregion

        #region *byte*

        public static byte[] ToByteArray(Image image)
        {
            if (image == null) return null;
            ImageConverter converter = new ImageConverter();
            return (byte[])converter.ConvertTo(image, typeof(byte[]));
        }
        public static Image ToImage(byte[] byteimage)
        {
            if (byteimage == null) return null;
            MemoryStream ms = new MemoryStream(byteimage);
            return Image.FromStream(ms);
        }
        public static byte[] ToByteArray(string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            System.Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }
        public static string ToString(byte[] bytes)
        {
            char[] chars = new char[(bytes.Length+1) / sizeof(char)];
            System.Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }
        public static byte[] ToByteArrayFile(string fileAddress)
        {
            if (string.IsNullOrEmpty(fileAddress)) return null;
            return File.ReadAllBytes(fileAddress);
        }
        public static void ToFile(string exportToaAddress, byte[] bytes)
        {
            if (string.IsNullOrEmpty(exportToaAddress)) return;
            File.WriteAllBytes(exportToaAddress, bytes);
        }
        public static Color HexaDecimalToColor(string val)
        {
            if (string.IsNullOrWhiteSpace(val)) return Color.Empty;
            string newvalue = val.Trim().Trim('#');
            double countd = newvalue.Length / 3d;
            if (countd < 1) return Color.Empty;
            bool hasAlpha = countd % 1 != 0;
            int count = newvalue.Length / (hasAlpha?4:3);
            int r = MathService.Between(ConvertService.HexaDecimalToInt(newvalue.Substring(0, count)),0,255);
            int g = MathService.Between(ConvertService.HexaDecimalToInt(newvalue.Substring(count, count)), 0, 255);
            int b = MathService.Between(ConvertService.HexaDecimalToInt(newvalue.Substring(2 * count, count)), 0, 255);
            int a = hasAlpha? MathService.Between(ConvertService.HexaDecimalToInt(newvalue.Substring(3 * count)), 0, 255) :255;
            return Color.FromArgb(a,r,g,b);
        }
        public static string ToHexaDecimal(Color val, int num = 8)
        {
            var hasAlpha = num % 3d != 0;
            int count = num / (hasAlpha?4:3);
            string a="",r,g,b;
            r = ConvertService.ToHexaDecimal(val.R);
            while (r.Length < count) r = "0" + r;
            g = ConvertService.ToHexaDecimal(val.G);
            while (g.Length < count) g = "0" + g;
            b = ConvertService.ToHexaDecimal(val.B);
            while (b.Length < count) b = "0" + b;
            if (hasAlpha)
            {
                a = ConvertService.ToHexaDecimal(val.A);
                while (a.Length < count) a = "0" + a;
            }
            return r+g+b+a;
        }
        public static string ToHexaDecimal(params short[] val)
        {
            return string.Join("",from v in val select v.ToString("X"));
        }
        public static string ToHexaDecimal(params int[] val)
        {
            return string.Join("",from v in val select v.ToString("X"));
        }
        public static string ToHexaDecimal(params long[] val)
        {
            return string.Join("",from v in val select v.ToString("X"));
        }
        public static string ToHexaDecimal(params float[] val)
        {
            return string.Join("",from v in val select v.ToString("X"));
        }
        public static string ToHexaDecimal(params double[] val)
        {
            return string.Join("",from v in val select v.ToString("X"));
        }
        public static string ToHexaDecimal(params decimal[] val)
        {
            return string.Join("",from v in val select v.ToString("X"));
        }
        public static short HexaDecimalToShort(string hexString)
        {
            uint num = uint.Parse(hexString, System.Globalization.NumberStyles.AllowHexSpecifier);
            return BitConverter.ToInt16(BitConverter.GetBytes(num), 0);
        }
        public static int HexaDecimalToInt(string hexString)
        {
            uint num = uint.Parse(hexString, System.Globalization.NumberStyles.AllowHexSpecifier);
            return BitConverter.ToInt32(BitConverter.GetBytes(num), 0);
        }
        public static long HexaDecimalToLong(string hexString)
        {
            uint num = uint.Parse(hexString, System.Globalization.NumberStyles.AllowHexSpecifier);
            return BitConverter.ToInt64(BitConverter.GetBytes(num), 0);
        }
        public static float HexaDecimalToFloat(string hexString)
        {
            uint num = uint.Parse(hexString, System.Globalization.NumberStyles.AllowHexSpecifier);
            return BitConverter.ToSingle(BitConverter.GetBytes(num), 0);
        }
        public static double HexaDecimalToDouble(string hexString)
        {
            uint num = uint.Parse(hexString, System.Globalization.NumberStyles.AllowHexSpecifier);
            return BitConverter.ToDouble(BitConverter.GetBytes(num), 0);
        }
        #endregion

        #region *string*
        private static CharBank CharBank = null;
        public static Point ToPoint(string val, Point defaultVal = default)
        {
            if (val == null) return defaultVal;
            var matchs = Regex.Matches(val,"\\b\\d+\\b");
            return matchs.Count > 1 ? new Point(Convert.ToInt32(matchs[0].Value), Convert.ToInt32(matchs[1].Value)) : defaultVal;
        }
        public static string ToString(Point Obj)
        {
            return $"{Obj.GetType().FullName}({Obj.X},{Obj.Y})";
        }
        public static Location ToLocation(string val, Location defaultVal = default)
        {
            if (val == null) return defaultVal;
            var matchs = Regex.Matches(val, "\\b\\d+\\b");
            return matchs.Count > 1 ? new Location(Convert.ToInt32(matchs[0].Value), Convert.ToInt32(matchs[1].Value)) : defaultVal;
        }
        public static string ToString(Location Obj)
        {
            return $"{Obj.GetType().FullName}({Obj.X},{Obj.Y})";
        }
        public static Size ToSize(string val, Size defaultVal = default)
        {
            if (val == null) return defaultVal;
            var matchs = Regex.Matches(val, "\\b\\d+\\b");
            return matchs.Count > 1 ? new Size(Convert.ToInt32(matchs[0].Value), Convert.ToInt32(matchs[1].Value)) : defaultVal;
        }
        public static string ToString(Size Obj)
        {
            return $"{Obj.GetType().FullName}({Obj.Width},{Obj.Height})";
        }
        public static Rectangle ToRectangle(string val, Rectangle defaultVal = default)
        {
            if (val == null) return defaultVal;
            var matchs = Regex.Matches(val, "\\b\\d+\\b");
            return matchs.Count > 3 ? new Rectangle(Convert.ToInt32(matchs[0].Value), Convert.ToInt32(matchs[1].Value), Convert.ToInt32(matchs[2].Value), Convert.ToInt32(matchs[3].Value)) : defaultVal;
        }
        public static string ToString(Rectangle Obj)
        {
            return $"{Obj.GetType().FullName}({Obj.X},{Obj.Y},{Obj.Width},{Obj.Height})";
        }
        public static object ToObject(string val, object defaultVal = default)
        {
            if (val == null) return defaultVal;
            var match = Regex.Match(val, "^(\\w+\\.?)+\\b");
            if (!match.Success)
            {
                match = Regex.Match(val, "^[+-]?\\d*\\.?\\d+$");
                if (!match.Success) return val;
                if (match.Value.Contains("."))
                    if (match.Value.Length > 20)
                        return ToDecimal(val);
                    else if (match.Value.Length > 10)
                        return ToDouble(val);
                    else
                        return ToSingle(val);
                else if (match.Value.Length > 25)
                    return ToDecimal(val);
                else if (match.Value.Length > 10)
                    return ToLong(val);
                else
                    return ToInt(val);
            }
            var key = match.Value.ToLower().Trim();
            switch (key)
            {
                case "point":
                case "system.drawing.point":
                    return ToPoint(val);
                case "size":
                case "system.drawing.size":
                    return ToSize(val);
                case "rectangle":
                case "system.drawing.rectangle":
                    return ToRectangle(val);
                case "location":
                case "mimfa.model.location":
                    return ToLocation(val);
            }
            return val;
        }
        public static string ToString(Font font)
        {
            if (font == null) return null;
            return $@"{font.Style} normal {(font.Bold ? "bold" : "normal")} {font.Size}{ToUniversalUnit(font.Unit)} '{font.FontFamily}'";
        }
        public static string ToString(object Obj)
        {
            if (Obj == null) return string.Empty;
            var t = Obj.GetType();
            if (Obj is Point) return ToString((Point)Obj);
            if (Obj is Size) return ToString((Size)Obj);
            if (Obj is Location) return ToString((Location)Obj);
            if (Obj is Rectangle) return ToString((Rectangle)Obj);
            return Obj + "";
        }
        public static string ToString(System.Windows.Forms.HtmlDocument Obj)
        {
            if (Obj == null || Obj.Body == null || Obj.Body.Parent == null) return null;
            return Obj.Body.Parent.OuterHtml;
        }
        public static string ToString<T>(List<T> list)
        {
            string str = "";
            for (int i = 0; i < list.Count; i++)
                str += list[i] + Environment.NewLine;
            return str;
        }
        public static string ToString<T>(T[] array)
        {
            string str = "";
            for (int i = 0; i < array.Length; i++)
                str += array[i] + Environment.NewLine;
            return str;
        }
        public static string ToString<T, F>(Dictionary<T, F> dic, string splitChar, string lineSplitor = "\r\n")
        {
            return string.Join(lineSplitor, ToStrings(dic, splitChar));
        }
        public static IEnumerable<string> ToStrings<T, F>(Dictionary<T, F> dic, string SplitChar)
        {
            foreach (var item in dic)
                yield return string.Join(SplitChar, item.Key, item.Value);
        }
        public static IEnumerable<string> ToStrings<T,F>(IEnumerable<KeyValuePair<T, F>> dic, string SplitChar)
        {
            foreach (var item in dic)
                yield return string.Join(SplitChar, item.Key, item.Value);
        }
        public static string ToSeparatedWords(string cancatedString)
        {
            string str = "";
            bool uperfind = false;
            if (cancatedString == null) return cancatedString;
            for (int i = 0; i < cancatedString.Length; i++)
            {
                string ch = cancatedString[i] + "";
                if (ch == ch.ToUpper())
                {
                    if (i == 0) uperfind = true;
                    else if (!uperfind)
                    {
                        str += " ";
                        uperfind = true;
                    }
                }
                else uperfind = false;
                str += ch;
            }
            return str;
        }
        public static string ToSeparatedWordsFast(string cancatedString)
        {
            string str = "";
            if (cancatedString == null || (cancatedString.Contains(" ")&& !cancatedString.Split(' ').ToList().Exists(v=>v.ToLower() == v))) return cancatedString;
            for (int i = 0; i < cancatedString.Length; i++)
            {
                string ch = cancatedString[i] + "";
                if (ch == ch.ToUpper() && i > 0)
                    str += " ";
                str += ch;
            }
            return str;
        }
        public static string ToAbbreviation(string name, bool justASCIIAlfabet = true)
        {
            if (name == null) return "";
            string sp = "[\\s,.=+\\-\\/\\\\'\"?<>:;~`!@#$%^&*(){}\\{\\}]+";
            if (name == name.ToUpper())
                if (!Regex.IsMatch(name, sp)) return name;
                else if(justASCIIAlfabet) name = name.ToLower();
            List<char> stra = new List<char>();
            foreach (var word in Regex.Split(name, sp))
                if (!string.IsNullOrWhiteSpace(word))
                {
                    bool b = true;
                    foreach (char ch in word)
                        if (b || char.IsUpper(ch))
                        {
                            stra.Add(ch);
                            b = false;
                        }
                }
            return string.Join("", stra);
        }
        public static string ToConcatedName(string name, bool justASCIIAlfabet = true, string splitter = "")
        {
            if (name == null) return "";
            if (justASCIIAlfabet)
            {
                string nn = "";
                foreach (var item in name)
                    if (char.IsLetterOrDigit(item))
                        nn += item.ToString();
                    else nn += " ";
                name = nn;
            }
            return ToConcatedName(splitter, Regex.Split(name, "\\s+"));
        }        
        public static string ToConcatedName(string splitter, string[] parts)=>parts.FirstOrDefault() + string.Join(splitter, from v in parts.Skip(1) select StringService.CapitalFirstLetter(v));
        public static string ToConcatedName(params string[] parts)=> ToConcatedName("", parts);
        public static string ToAlphabetCharacters(string name, string alter = " ")
        {
            if (name == null) return "";
            string nn = "";
            foreach (var item in name)
            {
                byte b = (byte)item;
                if ((b < 91 && b > 64) || (b < 123 && b > 96))
                    nn += item;
                else nn += alter;
            }
            return nn;
        }
        public static string ToAbsoluteURL(string url)
        {
            url = (url+"").Trim();
            string lurl = url.ToLower();
            if (!lurl.StartsWith("http://") && !lurl.StartsWith("https://"))
            {
                while (lurl.StartsWith("/")) url = url.Substring(1);
                url = "http://" + url;
            }
            return url;
        }
        public static string ToAbsoluteURL(string url, string repativePath) => ToAbsoluteURL(new Uri(url), repativePath);
        public static string ToAbsoluteURL(Uri uri,string repativePath)
        {
            if (string.IsNullOrWhiteSpace(repativePath)) return uri.OriginalString;
            if (Uri.IsWellFormedUriString(repativePath, UriKind.Absolute) &&
                           (repativePath.StartsWith("https://")
                           || repativePath.StartsWith("http://"))
                           )
                return repativePath;
            if (repativePath.StartsWith("/"))
                return uri.GetLeftPart(UriPartial.Authority) + repativePath;
            return uri.OriginalString.TrimEnd('/') + "/" + repativePath;
        }
        public static string ToUnSigned(string name)
        {
            if(CharBank == null) CharBank = new CharBank();
            if (string.IsNullOrEmpty(name)) return "";
            foreach (var item in CharBank.SignCharacters)
                name = name.Replace(item.ToString(), "");
            foreach (var item in CharBank.SymbolCharacter)
                name = name.Replace(item.ToString(), "");
            return name;
        }
        public static string[] ToSignSplitted(string text)
        {
            if (CharBank == null) CharBank = new CharBank();
            return text.Split(CharBank.SignCharacters, StringSplitOptions.None);
        }
        public static string FromRegexPattern(string pattern)
        {
            return pattern.Replace("\\`", "`").Replace("\\\"", "\"").Replace("\\'", "\'").Replace("\\r", "\r").Replace("\\n", "\n").Replace("\\t", "\t").Replace(@"\\", "\\");
        }
        public static string ToRegexPattern(string text)
        {
            return text.Replace("`", "\\`").Replace("\"", "\\\"").Replace("\'", "\\\'").Replace("\\", @"\\").Replace("\r", @"\r").Replace("\n", @"\n").Replace("\t", @"\t");
        }
        public static IEnumerable<CharKey> ToCharKeys(string keys)
        {
            List<string[]> ls = new List<string[]>();
            bool ctrl = false, alt = false, shift = false;
            string codechar = null;
            List<char> codes = new List<char>();
            foreach (var c in keys)
            {
                var normal = string.IsNullOrEmpty(codechar);
                if (!normal)
                    if (codechar[0] == c && codes.Count > 0)
                    {
                        switch (c)
                        {
                            case ')':
                                foreach (var cm in codes)
                                    yield return new CharKey(cm, ctrl,alt,shift);
                                break;
                            case ']':
                            case '}':
                                switch (string.Join("",codes))
                                {
                                    case "SPACE":
                                        yield return new CharKey(' ', ctrl, alt, shift);
                                        break;
                                    case "TAB":
                                        yield return new CharKey('\t', ctrl, alt, shift);
                                        break;
                                    default:
                                        if (codes.Count == 1) yield return new CharKey(codes.First(), ctrl, alt, shift);
                                        else
                                        {
                                            if (c == ']') yield return new CharKey('[', ctrl, alt, shift);
                                            else yield return new CharKey('{', ctrl, alt, shift);
                                            foreach (var cm in codes)
                                                yield return new CharKey(cm, ctrl, alt, shift);
                                            yield return new CharKey(c, ctrl, alt, shift);
                                        }
                                        break;
                                }
                                break;
                            default:
                                break;
                        }
                        codechar = string.Join("", codechar.Skip(1));
                        codes.Clear();
                        ctrl = alt = shift = false;
                    }
                    else codes.Add(c);
                switch (c)
                {
                    case '+':
                        shift = true;
                        break;
                    case '^':
                        ctrl = true;
                        break;
                    case '%':
                        alt = true;
                        break;
                    case '~':
                        yield return new CharKey('\n', ctrl, alt, shift);
                        break;
                    case '(':
                        codechar += ")";
                        break;
                    case '[':
                        codechar += "]";
                        break;
                    case '{':
                        codechar += "}";
                        break;
                    default:
                        yield return new CharKey(c, ctrl, alt, shift);
                        ctrl = alt = shift = false;
                        break;
                }
            }
        }
        public static string FromHotKeys(string keys)
        {
            return string.Join("",from v in ToCharKeys(keys) select v);
        }
        public static string ToHotKeys(string text)
        {
            var sb = new System.Text.StringBuilder();
            foreach (var c in text)
            {
                switch (c)
                {
                    case '+':
                    case '^':
                    case '%':
                    case '~':
                    case '(':
                    case ')':
                    case '[':
                    case ']':
                    case '{':
                    case '}':
                        sb.Append('{');
                        sb.Append(c);
                        sb.Append('}');
                        break;
                    default:
                        sb.Append(c);
                        break;
                }
            }
            return sb.ToString();
        }

        public static string ToVariableName(string name, string splitter = "")
        {
            if (string.IsNullOrEmpty(name)) return "";
            return ToConcatedName(Regex.Replace(name, "(^[\\W\\d]+)|\\W+", " ").Trim(), true, splitter);
        }
        public static string ToAlphabeticName(string name)
        {
            if (string.IsNullOrEmpty(name)) return "";
            name = Regex.Replace(name
                .Replace("&", "And")
                .Replace("|", "Or")
                .Replace("^", "Power")
                .Replace("!", "Not")
                .Replace("@", "At")
                .Replace("#", "Num")
                .Replace("$", "Dollar")
                .Replace("%", "Percent")
                .Replace("*", "Product")
                .Replace("=", "Equals")
                .Replace("-", "Minus")
                .Replace("+", "Plus")
                .Replace(">", "GT")
                .Replace("<", "LT")
                .Replace(".", "Dot"),"\\W+"," ")
                ;
            return ToConcatedName(name, true);
        }
        public static string ToURLCharacters(string name)
        {
            return System.Web.HttpUtility.UrlEncode(name);
        }
        public static string FromURLCharacters(string name)
        {
            return System.Web.HttpUtility.UrlDecode(name);
        }
        public static string ToTimeString(long number, string viewFormat = ":d2", string splitorSign = " and ", string multipleSign = "s", string secondSign = " second", string minuteSign = " minute", string hourSign = " hour", string daySign = " day", string yearSign = " year")
        {
            long val = number;
            long y = val / 31536000;
            val %= 31536000;
            long d = val / 86400;
            val %= 86400;
            long h = val / 3600;
            val %= 3600;
            long m = val / 60;
            val %= 60;
            long s = val;
            string result = "";
            if (y > 0) result += string.Format("{0}{1" + viewFormat + "}{2}", (string.IsNullOrWhiteSpace(result) ? "" : splitorSign), y,  yearSign + (y > 1?multipleSign:""));
            if (d > 0) result += string.Format("{0}{1" + viewFormat + "}{2}", (string.IsNullOrWhiteSpace(result) ? "" : splitorSign), d, daySign + (d > 1?multipleSign:""));
            if (h > 0) result += string.Format("{0}{1" + viewFormat + "}{2}", (string.IsNullOrWhiteSpace(result) ? "" : splitorSign), h,  hourSign + (h > 1?multipleSign:""));
            if (m > 0) result += string.Format("{0}{1" + viewFormat + "}{2}", (string.IsNullOrWhiteSpace(result) ? "" : splitorSign), m,  minuteSign + (m > 1?multipleSign:""));
            if (s > 0) result += string.Format("{0}{1" + viewFormat + "}{2}", (string.IsNullOrWhiteSpace(result) ? "" : splitorSign), s,  secondSign + (s > 1?multipleSign:""));
            return result;
        }
        public static string ToOrdinalNumber(long number)
        {
            long l = number % 10;
            return l == 1 ? number + "st" :
                l == 2 ? number + "nd" :
                l == 3 ? number + "rd" :
                number + "th";
        }
        public static string ToAlphabet(long number,bool signed = true)
        {
            if(CharBank == null) CharBank = new CharBank();
            if (number < 0 && !signed) return "";
            int len = CharBank.EnglishUpperCharacters.Length;
            return number < 0? "-" + string.Join("", ToAlphabet(((number = -number)-1 / len), false), CharBank.EnglishUpperCharacters[number  % len]):
                string.Join("", ToAlphabet((number / len)-1,false), CharBank.EnglishUpperCharacters[number % len]);
        }
        public static List<long> ToAreaNumbers(string areaText)
        {
            List<long> area = new List<long>();
            string lines = Regex.Match(areaText.ToLower(), @"(\d*(\-|\;|\,)\d*)+|\d+").Value;
            area.Clear();
            if (lines.Contains(";") || lines.Contains(","))
                area.AddRange(from v in lines.Split(';', ',') let n = ConvertService.TryToLong(v, -1) where n > -1 select n);
            else if (lines.Contains("-"))
            {
                string[] sta = lines.Split('-');
                long i = ConvertService.ToNumber(sta.First());
                long len = ConvertService.ToNumber(sta.Last());
                if (i < 0) i = 0;
                if (len < 0) len = 0;

                if (i < len) for (; i <= len; i++) area.Add(i);
                else for (; i >= len; i--) area.Add(i);
            }
            else if (ConvertService.TryToInt(lines, -1) > -1)
                area.Add(ConvertService.TryToInt(lines, -1));
            return area;
        }
        public static long ToNumber(object number)
        {
            return ToNumber(number+"");
        }
        public static long ToNumber(IEnumerable<char> number)
        {
            if (number==null || !number.Any()) return 0;
            if (InfoService.IsNumber(number)) return Convert.ToInt64(number);
            if (CharBank == null) CharBank = new CharBank();
            char ch = char.ToUpper(number.First());
            number = number.Skip(1);
            if (char.IsDigit(ch))
                return Convert.ToInt16(ch) + ToNumber(number);
            else
            {
                long num = CharBank.EnglishUpperCharacters.ToList().FindIndex(c => c == ch);
                return num < 0 ? (ch=='-'?-ToNumber(number) : ToNumber(number)) : number.Any()? ((num+1) * CharBank.EnglishUpperCharacters.Length * number.Count()) + ToNumber(number) : num;
            }
        }
        public static T ToEnum<T>(string value, bool ignoreCase = true) where T : Enum
        {
            var v = Enum.Parse(typeof(T), value, ignoreCase);
            return v == null ? default(T) : (T)v;
        }
        public static T TryToEnum<T>(string value, T defaultEnum, bool ignoreCase = true) where T : Enum
        {
            var v = Enum.Parse(typeof(T), value, ignoreCase);
            return v == null ? defaultEnum : (T)v;
        }

        public static IPAddress ToIPAddress(string address)
        {
            if (string.IsNullOrWhiteSpace(address)) return null;
           return new IPAddress((from v in address.Split('.') select ((byte)TryToInt(v))).ToArray());
        }
        #endregion

        #region *file*
        public static string ToTrimedString(string fileAddress)
        {
            return IOService.ReadTrimedText(fileAddress);
        }
        public static string ToString(string fileAddress)
        {
            return IOService.ReadText(fileAddress);
        }
        public static void ToFile<T, F>(string fileAddress, Dictionary<T, F> Dic, string SplitChar = "|")
        {
            IOService.WriteDictionary(fileAddress, Dic, SplitChar);
        }
        public static Image ToImage(string fileAddress)
        {
            Image image = null;
            try { image = Image.FromFile(fileAddress); } catch { }
            return image;
        }

        #endregion

        #region *html*
        public static string ToHTMLTag(Image img, string imageFileAddress, string cssClassName = "Image", int id = 0)
        {
            string str = "";
            if (!File.Exists(imageFileAddress)) img.Save(imageFileAddress);
            str += @"<img class='" + cssClassName + "' name='" + cssClassName + "" + id + "' src='" + imageFileAddress + "'/>";
            return str;
        }
        public static void ToImage(string html, string imageFileAddress, Size size, bool forceOpen = true)
        {
            var bm = ToImage(html, size);
            var ext = Path.GetExtension(imageFileAddress).ToLower();
            switch (ext)
            {
                case ".png":
                    bm.Save(imageFileAddress, ImageFormat.Png);
                    break;
                case ".bmp":
                    bm.Save(imageFileAddress, ImageFormat.Bmp);
                    break;
                case ".tif":
                case ".tiff":
                    bm.Save(imageFileAddress, ImageFormat.Tiff);
                    break;
                case ".gif":
                    bm.Save(imageFileAddress, ImageFormat.Gif);
                    break;
                case ".icon":
                    bm.Save(imageFileAddress, ImageFormat.Icon);
                    break;
                case ".wmf":
                    bm.Save(imageFileAddress, ImageFormat.Wmf);
                    break;
                case ".emf":
                    bm.Save(imageFileAddress, ImageFormat.Emf);
                    break;
                case ".exif":
                    bm.Save(imageFileAddress, ImageFormat.Exif);
                    break;
                default:
                    bm.Save(imageFileAddress, ImageFormat.Jpeg);
                    break;
            }
            if (forceOpen) System.Diagnostics.Process.Start(imageFileAddress);
        }
        public static Bitmap ToImage(string html, Size size)
        {
            WebBrowser wb = new WebBrowser();
            wb.ScriptErrorsSuppressed = true;
            ControlService.WebBrowserDocument(ref wb, html);
            wb.ScrollBarsEnabled = false;
            wb.Size = size;
            Bitmap bm = new Bitmap(size.Width, size.Height);
            wb.DrawToBitmap(bm, new Rectangle(0, 0, size.Width, size.Height));
            return bm;
            //Point loc = new Point(0, 0);
            //MiMFa_ControlService.WebBrowserDocument(wb, html);
            //while (true)
            //{
            //    wb.DrawToBitmap(bm, new Rectangle(loc.X, loc.Y, size.Width, size.Height));
            //    loc.Y += size.Height;
            //    yield return bm;
            //    if (loc.Y > wb.Document.Body.ScrollRectangle.Height) yield break;
            //    else wb.Document.Body.ScrollTop = loc.Y;
            //}
        }
        public static string ToXHTMLElements(string text)
        {
            return "<html><head></head><body>" + StringService.ToHTML(text) + "</body></html>";
        }
        public static string ToHTML(string html)
        {
            return StringService.ToHTML(html);
        }
        public static HtmlAgilityPack.HtmlDocument ToHtmlDocument(System.Windows.Forms.HtmlDocument doc)
        {
            return ToHtmlDocument(ToString(doc));
        }
        public static HtmlAgilityPack.HtmlDocument ToHtmlDocument(string html)
        {
            if (string.IsNullOrEmpty(html)) return null;
            HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();
            doc.LoadHtml(html);
            return doc;
        }
        public static HtmlDocument ToHTMLDocument(string html)
        {
            WebBrowser browser = new WebBrowser();
            browser.ScriptErrorsSuppressed = true;
            browser.DocumentText = html;
            browser.Document.OpenNew(true);
            browser.Document.Write(html);
            browser.Refresh();
            return browser.Document;
        }
        public static XMLElement ToHTMLElement(HtmlElement html,int childindex = 0, XMLElement parent = null)
        {
            string[] stra = new string[] { "<" + html.TagName + ">" };
            string s = "", e = "";
            if (html.OuterHtml != null)
                if (html.InnerHtml == null)
                {
                    stra = html.OuterHtml.Split(new string[] { "></" }, StringSplitOptions.None);
                    if (stra.Length > 1)
                    {
                        stra[0] += ">";
                        e = "</" + stra.Last();
                    }
                }
                else if ((stra = html.OuterHtml.Split(new string[] { html.InnerHtml }, StringSplitOptions.None)).Length > 1)
                    e = stra.Last().Trim();
            else if (stra.Length > 1) e = stra.Last().Trim();
            s = stra.First().Trim();
            XMLElement p = new XMLElement(childindex, html.TagName,s,e,null, parent);
            for (int i = 0; i < html.Children.Count; i++)
                p.Children.Add(ToHTMLElement(html.Children[i], i, p));
            return p;
        }
        public static List<XMLElement> ToHTMLElements(WebBrowser wb, bool quick = false)
        {
            //string html =
            //@"
            //<doctype type='html'/>
            //<!--[if ex] <html> <head>< <comment1 [end if]-->
            //    <head>
            //        <script type='text'>
            //        func
            //        </script>
            //        <style>
            //            a{}b{}
            //        </style>
            //    </head>
            //    <body>
            //        <hr/><!-- comment1--><hr>
            //        <br>
            //        <div ho='<aa> ali<br> </aa>'>
            //            div 1
            //            <br>
            //            <h1>aaaaa</h1>
            //            <div>
            //                div 1-2
            //            </div>
            //            <div>
            //                div 1-3
            //            </div
            //        </div>
            //        <div style='back:black'>
            //            <div>
            //                div 2-1
            //            </div>
            //            <a href='http://nazkd.com'>
            //                <!-- comment2-->nazkd link
            //            </a>
            //        </div><!-- comment3-->
            //    </body>
            //</html>
            //";
            //
            //HtmlElement he = wb.Document.Body;
            //while (he.Parent != null) he = he.Parent;
            //return ToMiMFaHTMLElements(he);
            //
            //string html = "";
            //if (quick) html = wb.DocumentText;
            //else
            //{
            //    int num = 0;
            //    while (num++ < 15)
            //        if (!wb.IsBusy) { html = wb.DocumentText; break; }
            //}
            ////
            string html = wb.DocumentText;
            return ToHTMLElements(html, quick);
        }
        public static List<XMLElement> ToHTMLElements( HtmlElementCollection elems)
        {
            List<XMLElement> res = new List<XMLElement>();
            for (int i = 0; i < elems.Count; i++)
                res.Add(ToHTMLElement(elems[i], i, null));
            return res;
        }
        public static List<XMLElement> ToHTMLElements(params HtmlElement[] elems)
        {
            List<XMLElement> res = new List<XMLElement>();
            for (int i = 0; i < elems.Length; i++)
                res.Add(ToHTMLElement(elems[i], i, null));
            return res;
        }
        public static List<XMLElement> ToHTMLElements(string html, bool quick = false)
        {
            Pickup mscriptp = new Pickup("" + "[|SCRIPT|]" + "", "<SCRIPT", "</SCRIPT>", false);
            Pickup mxmpp = new Pickup("" + "[|XMP|]" + "", "<XMP", "</XMP>", false);
            Pickup mcodep = new Pickup("" + "[|CODE|]" + "", "<CODE", "</CODE>", false);
            html = mscriptp.Pick(html);
            html = mxmpp.Pick(html);
            html = mcodep.Pick(html);
            if (quick) html = mscriptp.ParseTo(html, "");
            var res = ToXMLElements(html);
            for (int i = 0; i < res.Count; i++)
                res[i] = parse1(res[i],
                    mcodep,
                    mxmpp,
                    mscriptp);
            return res;
        }
        private static XMLElement parse1(XMLElement xmle,params Pickup[] picks)
        {
            foreach (var pick in picks)
                xmle.StartTag = pick.Parse(xmle.StartTag);
            for (int i = 0; i < xmle.Children.Count; i++)
                xmle.Children[i] = parse1(xmle.Children[i], picks);
            return xmle;
        }
        #endregion

        #region object
        public static bool ToBoolean(object obj)
        {
            if (obj is int || obj is long || obj is double || obj is float || obj is decimal)
                return ToDouble(obj) > 0;
            if (obj is IEnumerable<object>)
                return ((IEnumerable<object>)obj).Any();
            return Convert.ToBoolean(obj);
        }
        public static bool TryToBoolean(object obj, bool defaultVal = false)
        {
            if (obj == null) return defaultVal;
            try { return ToBoolean(obj); } catch { return defaultVal; }
        }
        public static double ToDouble(object obj)
        {
            return obj == null?0:Convert.ToDouble(obj);
        }
        public static double ForceToDouble(object obj)
        {
            if (obj == null) return 0;
            if (obj is string) return ForceToDouble((string)obj);
            return IOService.Serialize(obj).Length;
        }
        public static double ForceToDouble(string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return 0;
            double d = 0;
            if (double.TryParse(str, out d)) return d;
            var fs = InfoService.GetFloatSign();
            str = Regex.Replace(str, "[^\\-\\d\\"+fs+"]","").Trim(fs.First());
            if (str.Length < 1) return 0;
            if (str.IndexOf(fs) < 0) return Convert.ToDouble(str);
            string[] stra = str.Split(fs.First());
            return Convert.ToDouble(stra.First() + fs + string.Join("", stra.Skip(1)));
        }
        public static float ToSingle(object obj)
        {
            return obj == null ? 0 : Convert.ToSingle(obj);
        }
        public static int ToInt(object obj)
        {
            return obj == null ? 0 : Convert.ToInt32(obj);
        }
        public static int TryToInt(object obj,int defaultVal = 0)
        {
            if (obj == null) return defaultVal;
            try { return Convert.ToInt32(obj); } catch { return defaultVal; }
        }
        public static uint TryToUInt(object obj,uint defaultVal = 0)
        {
            if (obj == null) return defaultVal;
            try { return Convert.ToUInt32(obj); } catch { return defaultVal; }
        }
        public static long ToLong(object obj)
        {
            return obj == null ? 0 : Convert.ToInt64(obj);
        }
        public static long TryToLong(object obj, long defaultVal = 0)
        {
            if (obj == null) return defaultVal;
            try { return Convert.ToInt64(obj); } catch { return defaultVal; }
        }
        public static ulong TryToULong(object obj, ulong defaultVal = 0)
        {
            if (obj == null) return defaultVal;
            try { return Convert.ToUInt64(obj); } catch { return defaultVal; }
        }
        public static float TryToSingle(object obj, float defaultVal = 0)
        {
            if (obj == null) return defaultVal;
            try { return Convert.ToSingle(obj); } catch { return defaultVal; }
        }
        public static double TryToDouble(object obj, double defaultVal = 0)
        {
            if (obj == null) return defaultVal;
            try { return Convert.ToDouble(obj); } catch { return defaultVal; }
        }
        public static decimal ToDecimal(object obj)
        {
            if (obj == null) return 0;
            return Convert.ToDecimal(obj);
        }
        public static decimal ForceToDecimal(string str)
        {
            str = Regex.Replace(str,"\\D+",".").Trim('.');
            return str.Length>0? Convert.ToDecimal(str) : Decimal.Zero;
        }
        public static double TryToNumber(object obj, double defaultVal = 0)
        {
            if (obj == null) return defaultVal;
            try { return ToNumber((obj + "").Trim()); } catch { return defaultVal; }
        }
        #endregion

        #region datatable
        public static List<List<string>> ToCells(string text, char columnsDelimiter = '\t', char rowsDelimiter = '\n', char enclosure = '"')
        {
            if (string.IsNullOrEmpty(text)) return new List<List<string>>();
            var rows = new List<List<string>>();
            var length = text.Length;
            var index = 0;
            while (index < length) {
                var row = new List<string>();
                var column = "";
                var inEnclosure = false;
                do {
                    var mchar = text[index++];
                    if (inEnclosure) {
                        if (mchar == enclosure) {
                            if (index < length) {
                                mchar = text[index];
                                if (mchar == enclosure) {
                                    column += mchar;
                                    index++;
                                } else inEnclosure = false;
                            } else
                            {
                                row.Add(column);
                                break;
                            }
                        } else column += mchar;
                    } else if(mchar == enclosure) {
                        if (index < length) {
                            mchar = text[index++];
                            if (mchar == enclosure) column += mchar;
                            else
                            {
                                inEnclosure = true;
                                column += mchar;
                            }
                        } else
                        {
                            row.Add(column);
                            break;
                        }
                    }
                    else if(mchar == columnsDelimiter) {
                        row.Add(column);
                        column = "";
                    }
                    else if(mchar == '\r') {
                        if (index < length) {
                            mchar = text[index];
                            if (mchar == rowsDelimiter) index++;
                        }
                        row.Add(column);
                        break;
                    }
                    else if(mchar == rowsDelimiter) {
                        row.Add(column);
                        break;
                    } else column += mchar;

                    if (index == length) {
                        row.Add(column);
                        break;
                    }
                } while (index < length);
                rows.Add(row);
            }
            return rows;
        }
        public static DataTable ToTable(string textfilePath, string columnsDelimiter="\t", string rowsDelimiter="\r\n",int maxrow = 999999999)
        {
            var document = new ChainedFile(textfilePath);
            document.WarpsSplitter = columnsDelimiter;
            document.LinesSplitter = rowsDelimiter;
            document.ColumnsLabelsIndex = 0;
            document.Count();
            DataTable table = new DataTable();
            foreach (var item in document.ForceColumnsLabels)
                table.Columns.Add(item);
            var len = table.Columns.Count;
            foreach (var row in document.ReadRows(1, maxrow))
                table.Rows.Add((from v in row select (object)v).Take(len).ToArray());
            return table;
        }
        public static string ToString(SmartTable miMFa_Table, string columnsDelimiter = "\t", string rowsDelimiter = "\r\n")
        {
            return string.Join(rowsDelimiter,ToStrings(miMFa_Table, columnsDelimiter));
        }
        public static string ToString(DataTable dt, string columnsDelimiter = "\t", string rowsDelimiter = "\r\n")
        {
            return string.Join(rowsDelimiter, ToStrings(dt, columnsDelimiter));
        }
        public static IEnumerable<string> ToStrings(DataTable dt, string columnsDelimiter = "\t")
        {
            string s = "";
            for (int i = 0; i < dt.Columns.Count; i++)
                s += dt.Columns[i].ColumnName + columnsDelimiter;
            yield return s.Length > columnsDelimiter.Length ? s.Substring(0, s.Length - columnsDelimiter.Length) : s;
            foreach (DataRow item in dt.Rows)
                yield return string.Join(columnsDelimiter, item.ItemArray);
        }
        public static IEnumerable<string> ToStrings(SmartTable dt, string columnsDelimiter = "\t")
        {
            string s = "";
            yield return s.Length > columnsDelimiter.Length ? s.Substring(0, s.Length - columnsDelimiter.Length) : s;
            foreach (DataRow item in dt.Rows)
                yield return string.Join(columnsDelimiter, item.ItemArray);
        }
        public static List<T> ToList<T>(DataTable dataTable, params object[] constructorParams)
        {
            List<T> result = new List<T>();
            Type type = typeof(T);
            FieldInfo[] fi = type.GetFields();
            PropertyInfo[] pi = type.GetProperties();
            if (dataTable != null)
                if (dataTable.Rows.Count > 0)
                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {
                        T tm = (T)Activator.CreateInstance(type, constructorParams);
                        foreach (DataColumn item in dataTable.Columns)
                            tm = IOService.ValueToObject(tm, item.ColumnName, dataTable.Rows[i][item.ColumnName]);
                        result.Add(tm);
                    }
            return result;
        }
        public static T ToObject<T>(DataTable dt, int row, ref T obj)
        {
            try
            {
                if (dt.Rows.Count <= 0) throw new NullReferenceException("This table is empty!");
                foreach (DataColumn item in dt.Columns)
                    if (dt.Rows[row][item.ColumnName].GetType() != typeof(DBNull))
                        obj = IOService.ValueToObject(obj, item.ColumnName, dt.Rows[row][item.ColumnName]);
            }
            catch { }
            return obj;
        }
        public static DataTable ToPropertiesDataTable<T>(List<T> lt)
        {
            DataTable obj = new DataTable();
            if(lt.Count>0)try
            {
                Type t = lt.First().GetType();
                var pi = t.GetProperties();
                foreach (var item in pi)
                    obj.Columns.Add(item.Name,item.PropertyType);
                foreach (var item in lt)
                    obj.Rows.Add(ToPropertiesValueArray(item,t));
            }
            catch { }
            return obj;
        }
        public static DataTable ToPropertiesDataTable<T>(List<T> lt, string[] properties)
        {
            DataTable obj = new DataTable();
            if (lt.Count > 0) try
                {
                    Type t = lt.First().GetType();
                    foreach (var item in properties)
                    {
                        var pi = t.GetProperty(item);
                        obj.Columns.Add(pi.Name, pi.PropertyType);
                    }
                    foreach (var item in lt)
                        obj.Rows.Add(ToPropertiesValueArray(item, t));
                }
                catch { }
            return obj;
        }
        public static object[] ToPropertiesValueArray<T>(T o)
        {
            List<object> obj = new List<object>();
            try
            {
                Type t = o.GetType();
                var pi = t.GetProperties();
                foreach (var item in pi)
                    obj.Add(item.GetValue(o));
            }
            catch { }
            return obj.ToArray();
        }
        public static object[] ToPropertiesValueArray<T>(T o, Type t, string[] properties)
        {
            List<object> obj = new List<object>();
            try
            {
                foreach (var item in properties)
                    try { obj.Add(t.GetProperty(item).GetValue(o)); } catch { }
            }
            catch { }
            return obj.ToArray();
        }
        public static object[] ToPropertiesValueArray<T>(T o, Type t)
        {
            List<object> obj = new List<object>();
            try
            {
                var pi = t.GetProperties();
                foreach (var item in pi)
                    obj.Add(item.GetValue(o));
            }
            catch { }
            return obj.ToArray();
        }
        public static object[] ToPropertiesValueArray<T>(T o, string[] properties)
        {
            List<object> obj = new List<object>();
            try
            {
                Type t = o.GetType();
                foreach (var item in properties)
                    try { obj.Add(t.GetProperty(item).GetValue(o)); } catch { }
            }
            catch { }
            return obj.ToArray();
        }
        #endregion

        #region Objects
        public static SmartDictionary<T,F> ToSmartDictionary<T,F>(params KeyValuePair<T,F>[] args)
        {
            SmartDictionary<T, F> dic = new SmartDictionary<T, F>();
            for (int i = 0; i < args.Length; i++)
                dic.Add(args[i].Key,args[i].Value);
            return dic;
        }
        public static Dictionary<T,F> ToDictionary<T,F>(params KeyValuePair<T,F>[] args)
        {
            Dictionary<T, F> dic = new Dictionary<T, F>();
            for (int i = 0; i < args.Length; i++)
                dic.Add(args[i].Key,args[i].Value);
            return dic;
        }
        public static string ToUniversalUnit(double value,int decimals = 2, string startDelimited = "", string endDelimited = "")
        {
            string[] universalUnits = { "", "K", "M", "G", "T", "P", "E", "Z", "Y", "S"};
            int ui = 0;
            while (value > 999)
            {
                value /= 1000;
                ui++;
            }
            return string.Join("", Math.Round(value, decimals), startDelimited, universalUnits[ui],endDelimited);
        }
        public static object ToUniversalUnit(GraphicsUnit unit)
        {
            switch (unit)
            {
                case GraphicsUnit.World:
                    return "ex";
                case GraphicsUnit.Display:
                    return "px";
                case GraphicsUnit.Point:
                    return "pt";
                case GraphicsUnit.Inch:
                    return "in";
                case GraphicsUnit.Document:
                    return "rem";
                case GraphicsUnit.Millimeter:
                    return "mm";
                case GraphicsUnit.Pixel:
                default:
                    return "px";
            }
        }

        public static object[] ToArray(object obj)
        {
            if (obj == null) return null;
            var Inewobj = obj as ICollection;
            if (Inewobj == null) return null;
            return Inewobj.Cast<object>()
                .ToArray();
        }
        public static List<object> ToList(object obj)
        {
            if (obj == null) return null;
            var Inewobj = obj as IList;
            if (Inewobj == null) return null;
            List<object> lo = new List<object>();
            for (int i = 0; i < Inewobj.Count; i++)
                lo.Add(Inewobj[i]);
            return lo;
        }
        public static Dictionary<string, string> ToDictionary(string text, string keyValueSplitor = "\t", string lineSplitor = "'r'n", bool keysToLower = false)
        {
            return ToDictionary(text.Split(new string[] { lineSplitor }, StringSplitOptions.None), keyValueSplitor, keysToLower);
        }
        public static Dictionary<string, string> ToDictionary(IEnumerable<string> lines, string SplitChar, bool keysToLower = false)
        {
            Dictionary<string, string> Dic = new Dictionary<string, string>();
            foreach (var item in lines)
            {
                var ma = StringService.FirstSplit(item, SplitChar);
                string key = keysToLower ? ma.First() : ma.First();
                if (Dic.ContainsKey(key)) Dic[key] += Environment.NewLine + ma.First();
                else if (ma.Length > 1) Dic.Add(ma.First(), ma.Last());
                else if (Dic.Count > 0) Dic[Dic.Last().Key] += Environment.NewLine + ma.First();
                else Dic.Add(key, "");
            }
            return Dic;
        }
        public static SmartDictionary<string, string> ToSmartDictionary(string text, string keyValueSplitor="\t", string lineSplitor="'r'n", bool keysToLower = false)
        {
            return ToSmartDictionary(text.Split(new string[] { lineSplitor },StringSplitOptions.None), keyValueSplitor,  keysToLower);
        }
        public static SmartDictionary<string, string> ToSmartDictionary(IEnumerable<string> lines, string SplitChar, bool keysToLower = false)
        {
            SmartDictionary<string, string> Dic = new SmartDictionary<string, string>();
            foreach (var item in lines)
            {
                var ma = StringService.FirstSplit(item, SplitChar);
                string key = keysToLower ? ma.First() : ma.First();
                if (Dic.ContainsKey(key)) Dic[key] += Environment.NewLine + ma.First();
                else if (ma.Length > 1) Dic.Add(ma.First(), ma.Last());
                else if (Dic.Count > 0) Dic[Dic.Last().Key] += Environment.NewLine + ma.First();
                else Dic.Add(key, "");
            }
            return Dic;
        }
        public static Dictionary<object, object> ToDictionary(dynamic obj)
        {
            if (!InfoService.IsDictionary(obj)) return null;
            Dictionary<object, object> dic = new Dictionary<object, object>();
            foreach (var item in obj)
                dic.Add(item.Key, item.Value);
            return dic;
        }
        public static IEnumerable<KeyValuePair<string, string>> ToKeyValuePairs(string text, string keyValueSplitor="\t", string lineSplitor="'r'n", bool keysToLower = false)
        {
            return ToKeyValuePairs(text.Split(new string[] { lineSplitor }, StringSplitOptions.None), keyValueSplitor, keysToLower);
        }
        public static IEnumerable<KeyValuePair<string, string>> ToKeyValuePairs(IEnumerable<string> lines, string SplitChar, bool keysToLower = false)
        {
            string key = "";
            string value = "";
            foreach (var item in lines)
            {
                var ma = StringService.FirstSplit(item, SplitChar);
                if (ma.Length > 1)
                {
                    if (!string.IsNullOrEmpty(key))
                    {
                        yield return new KeyValuePair<string, string>(key, value);
                        key = "";
                        value = "";
                    }
                    key = keysToLower? ma.First().ToLower():ma.First();
                    value = ma.Last();
                }
                else value += ma.First();
            }

            if (!string.IsNullOrEmpty(key)|| !string.IsNullOrEmpty(value))
                yield return new KeyValuePair<string, string>(key, value);
        }
        public static Matrix<object> ToMatrix(dynamic obj)
        {
            if (!InfoService.IsMatrix(obj)) return null;
            Matrix<object> mat = new Matrix<object>();
            foreach (var item in obj)
                mat.AddY(item);
            return mat;
        }
        public static IEnumerable<T> ToEnumerable<T>(IEnumerator<T> enumerator)
        {
            while (enumerator.MoveNext())
                yield return enumerator.Current;
        }
        public static IEnumerable<T> ToEnumerable<T>(ICollection<T> collection)
        {
            foreach (var item in collection)
                yield return item;
        }
        public static IEnumerable<T> ToEnumerable<T>(ICollection collection)
        {
            foreach (var item in collection)
                yield return (T)item;
        }
        public static IEnumerable<T> ToEnumerable<T>(IEnumerator enumerator)
        {
            while (enumerator.MoveNext())
                yield return (T)enumerator.Current;
        }
        public static KeyValuePair<object, object> ToKeyValuePair(dynamic obj)
        {
            return new KeyValuePair<object, object>(obj.Key, obj.Value);
        }

        public static Stack<object> ToStack(object obj)
        {
            if (obj == null) return null;
            var Inewobj = obj as Stack;
            if (Inewobj == null) return null;
            Stack<object> lo = new Stack<object>();
            object[] arr = Inewobj.ToArray();
            for (int i = 0; i < Inewobj.Count; i++)
                lo.Push(arr[i]);
            return lo;
        }
        public static Queue<object> ToQueue(object obj)
        {
            if (obj == null) return null;
            var Inewobj = obj as Queue;
            if (Inewobj == null) return null;
            Queue<object> lo = new Queue<object>();
            object[] arr = Inewobj.ToArray();
            for (int i = 0; i < Inewobj.Count; i++)
                lo.Enqueue(arr[i]);
            return lo;
        }
        public static object[] ToParents<T>(T[] arg)
        {
            if (arg == null) return null;
            object[] lo = new object[arg.Length];
            for (int i = 0; i < arg.Length; i++)
                lo[i] = arg[i];
            return lo.ToArray();
        }
        public static List<object> ToParents<T>(IList<T> arg)
        {
            if (arg == null) return null;
            List<object> lo = new List<object>();
            for (int i = 0; i < arg.Count; i++)
                lo.Add(arg[i]);
            return lo;
        }
        public static Dictionary<object,object> ToDictionary<T,F>(IDictionary<T,F> arg)
        {
            if (arg == null) return null;
            Dictionary<object, object> lo = new Dictionary<object, object>();
            foreach (var item in arg.Keys)
                lo.Add(item,arg[item]);
            return lo;
        }
        public static Stack<object> ToStack<T>(Stack<T> arg)
        {
            if (arg == null) return null;
            Stack<object> lo = new Stack<object>();
            List<T> lt = arg.ToList();
            for (int i = 0; i < lt.Count; i++)
                lo.Push(lt[i]);
            return lo;
        }
        public static SmartStack<object> ToSmartStack<T>(SmartStack<T> arg)
        {
            if (arg == null) return null;
            SmartStack<object> lo = new SmartStack<object>();
            List<T> lt = arg.ToList();
            for (int i = 0; i < lt.Count; i++)
                lo.Push(lt[i]);
            return lo;
        }
        #endregion

        #region Excel

        public static void ToExcelFile(DataGridView dgv, string exceladdress, string sheetName = "sheet1", bool openAfter = true)
        {
            ToExcelFile((new IEnumerable<object>[]{from v in dgv.Columns.Cast<DataGridViewColumn>() select v.HeaderText}).Concat(from v in dgv.Rows.Cast<DataGridViewRow>() select from c in v.Cells.Cast<DataGridViewCell>() select c.Value), exceladdress, sheetName, openAfter);
        }
        public static void ToExcelFile(DataSet ds, string exceladdress, bool openAfter = true, bool forceComputeStyles = false)
        {
            ToExcelFile(ds.Tables.Cast<DataTable>(), exceladdress, openAfter, forceComputeStyles);
        }
        public static void ToExcelFile(IEnumerable<DataTable> dts, string exceladdress, bool openAfter = true, bool forceComputeStyles = false)
        {
            // creating Excel Application
            Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
            // creating new WorkBook within Excel application
            Microsoft.Office.Interop.Excel.Workbook workbook = app.Workbooks.Add(Type.Missing);
            // creating new Excelsheet in workbook
            Microsoft.Office.Interop.Excel.Worksheet worksheet = null;
            // see the excel sheet behind the program
            //app.Visible = true;

            // get the reference of first sheet. By default its name is Sheet1.
            // store its reference to worksheet
            worksheet = workbook.Sheets["Sheet1"];
            worksheet = workbook.ActiveSheet;
            int si = 1;
            foreach (DataTable dt in dts)
            {
                if (si > 1) worksheet = workbook.Sheets.Add();
                ToExcelWorksheet(dt, workbook, worksheet, SelectService.First(dt.TableName, "sheet" + si++), forceComputeStyles);
            }
            // save the application
            workbook.SaveAs(exceladdress, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

            // Exit from the application
            if (openAfter) app.Visible = true;
            else app.Quit();
        }
        public static void ToExcelFile(DataTable dt, string exceladdress, string sheetName = "sheet1", bool openAfter = true, bool forceComputeStyles = false)
        {
            // creating Excel Application
            Microsoft.Office.Interop.Excel.Application app = new Microsoft.Office.Interop.Excel.Application();
            // creating new WorkBook within Excel application
            Microsoft.Office.Interop.Excel.Workbook workbook = app.Workbooks.Add(Type.Missing);
            // creating new Excelsheet in workbook
            Microsoft.Office.Interop.Excel.Worksheet worksheet = null;
            // see the excel sheet behind the program
            //app.Visible = true;

            // get the reference of first sheet. By default its name is Sheet1.
            // store its reference to worksheet
            worksheet = workbook.Sheets["Sheet1"];
            worksheet = workbook.ActiveSheet;
            ToExcelWorksheet(dt,workbook, worksheet, sheetName, forceComputeStyles);
            // save the application
            workbook.SaveAs(exceladdress, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

            // Exit from the application
            if (openAfter) app.Visible = true;
            else app.Quit();
        }
        public static void ToExcelFile(SmartTable dt, string exceladdress, string sheetName = "sheet1", bool openAfter = true, bool forceComputeStyles = false)
        {
            ToExcelFile(dt.MainTable, exceladdress, sheetName, openAfter, forceComputeStyles);
        }
        public static void ToExcelFile<T, F>(Dictionary<T, F> dic, string exceladdress, string sheetName = "sheet1", bool openAfter = true)
        {
            ToExcelFile(from v in dic select new object[] { v.Key, v.Value }.AsEnumerable(), exceladdress, sheetName, openAfter);
        }
        public static void ToExcelFile(string textFileAddress, string exceladdress, string[] delimiteds, char[] trimchars, string sheetName = "sheet1", bool openAfter = true)
        {
            ToExcelFile(from v in new ChainedFile(textFileAddress) {WarpsSplitters = delimiteds } select from c in v select c.Trim(trimchars), exceladdress, sheetName, openAfter);
        }
        public static void ToExcelFile(ChainedFile doc, string exceladdress, string sheetName = "sheet", bool openAfter = true)
        {
            int i = 0;
            if(doc.IsBig) 
                foreach (var item in doc.Chain)
                    ToExcelFile(item.ReadPieceRows(), exceladdress, sheetName + ++i, openAfter);
            else ToExcelFile(doc.ReadRows(), exceladdress, sheetName + ++i, openAfter);
        }
        public static void ToExcelFile<T, F>(IEnumerable<KeyValuePair<T, F>> dic, string exceladdress, string sheetName = "sheet1", bool openAfter = true)
        {
            ToExcelFile(from v in dic select new object[] { v.Key,v.Value }.AsEnumerable(), exceladdress, sheetName, openAfter);
        }
        public static void ToExcelFile<T>(IEnumerable<T> lt, string exceladdress, string sheetName = "sheet1", bool openAfter = true)
        {
            ToExcelFile(from v in lt select new object[] { v }.AsEnumerable(),exceladdress,sheetName, openAfter);
        }
        public static void ToExcelFile(IEnumerable<IEnumerable<string>> cells, string exceladdress, string sheetName = "sheet1", bool openAfter = true)
        {
            // creating Excel Application
            Microsoft.Office.Interop.Excel._Application app = new Microsoft.Office.Interop.Excel.Application();
            // creating new WorkBook within Excel application
            Microsoft.Office.Interop.Excel._Workbook workbook = app.Workbooks.Add(Type.Missing);
            // creating new Excelsheet in workbook
            Microsoft.Office.Interop.Excel._Worksheet worksheet = null;
            // see the excel sheet behind the program

            // get the reference of first sheet. By default its name is Sheet1.
            // store its reference to worksheet
            worksheet = workbook.Sheets["Sheet1"];
            worksheet = workbook.ActiveSheet;
            ToExcelWorksheet(cells,worksheet,sheetName);
            // save the application
            workbook.SaveAs(exceladdress, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

            // Exit from the application
            if (openAfter) app.Visible = true;
            else app.Quit();
        }
        public static void ToExcelFile(IEnumerable<IEnumerable<object>> cells, string exceladdress, string sheetName = "sheet1", bool openAfter = true)
        {
            // creating Excel Application
            Microsoft.Office.Interop.Excel._Application app = new Microsoft.Office.Interop.Excel.Application();
            // creating new WorkBook within Excel application
            Microsoft.Office.Interop.Excel._Workbook workbook = app.Workbooks.Add(Type.Missing);
            // creating new Excelsheet in workbook
            Microsoft.Office.Interop.Excel._Worksheet worksheet = null;
            // see the excel sheet behind the program

            // get the reference of first sheet. By default its name is Sheet1.
            // store its reference to worksheet
            worksheet = workbook.Sheets["Sheet1"];
            worksheet = workbook.ActiveSheet;
            ToExcelWorksheet(cells,worksheet,sheetName);
            // save the application
            workbook.SaveAs(exceladdress, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Microsoft.Office.Interop.Excel.XlSaveAsAccessMode.xlExclusive, Type.Missing, Type.Missing, Type.Missing, Type.Missing);

            // Exit from the application
            if (openAfter) app.Visible = true;
            else app.Quit();
        }
        public static void ToExcelWorksheet(IEnumerable<IEnumerable<string>> cells, Microsoft.Office.Interop.Excel._Worksheet worksheet, string sheetName = "sheet1")
        {
            worksheet.Activate();
            // changing the name of active sheet
            worksheet.Name = PathService.NormalizeForFileAndDirectoryName(sheetName,30);
            worksheet.EnableFormatConditionsCalculation = false;

            // storing Each row and column value to excel sheet
            int r = 1;
            foreach (var row in cells)
            {
                int c = 1;
                foreach (var cell in row)
                    worksheet.Cells[r, c++] = cell;
                r++;
            }
        }
        public static void ToExcelWorksheet(IEnumerable<IEnumerable<object>> cells, Microsoft.Office.Interop.Excel._Worksheet worksheet, string sheetName = "sheet1")
        {
            worksheet.Activate();
            // changing the name of active sheet
            worksheet.Name = PathService.NormalizeForFileAndDirectoryName(sheetName,30);
            worksheet.EnableFormatConditionsCalculation = false;

            // storing Each row and column value to excel sheet
            int r = 1;
            foreach (var row in cells)
            {
                int c = 1;
                foreach (var cell in row)
                    worksheet.Cells[r, c++] = cell;
                r++;
            }
        }
        public static void ToExcelWorksheet(DataTable dt, Microsoft.Office.Interop.Excel._Workbook workbook, Microsoft.Office.Interop.Excel._Worksheet worksheet, string sheetName = "sheet1", bool forceComputeStyles = false)
        {
            worksheet.Activate();

            // changing the name of active sheet
            worksheet.Name = PathService.NormalizeForFileAndDirectoryName(SelectService.First(sheetName,dt.TableName,"sheet1"), 30);
            worksheet.EnableFormatConditionsCalculation = false;

            bool hascf = forceComputeStyles;
            int addtorow = 1;
            // storing header part in Excel
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                var val = SelectService.First(dt.Columns[i].Caption, dt.Columns[i].ColumnName);
                worksheet.Cells[1, i + 1] = val;
                if (addtorow == 1 && !Regex.IsMatch(val, "^Column\\d*$")) addtorow = 2;
                if (dt.Columns[i].ExtendedProperties.Count > 0)
                {
                    hascf = true;
                    if (dt.Columns[i].ExtendedProperties.ContainsKey("Formula")) 
                        try { ((dynamic)worksheet.Cells[1, i + 1]).FormulaR1C1 = dt.Columns[i].ExtendedProperties["Formula"].ToString().Replace("R[0]C[0]", val); } catch { }
                    if (dt.Columns[i].ExtendedProperties.ContainsKey("Font"))
                        try { ((dynamic)worksheet.Cells[1, i + 1]).Font = dt.Columns[i].ExtendedProperties["Font"]; } catch { }
                    if (dt.Columns[i].ExtendedProperties.ContainsKey("Borders"))
                        try { ((dynamic)worksheet.Cells[1, i + 1]).Borders = dt.Columns[i].ExtendedProperties["Borders"]; } catch { }
                    if (dt.Columns[i].ExtendedProperties.ContainsKey("ForeColor"))
                        try { ((dynamic)worksheet.Cells[1, i + 1]).ForeColor = dt.Columns[i].ExtendedProperties["ForeColor"]; } catch { }
                    if (dt.Columns[i].ExtendedProperties.ContainsKey("BackColor"))
                        try { ((dynamic)worksheet.Cells[1, i + 1]).BackColor = dt.Columns[i].ExtendedProperties["BackColor"]; } catch { }
                }
            }

            // storing Each row and column value to excel sheet
            if (hascf)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                    for (int j = 0; j < dt.Columns.Count; j++)
                    {
                        var cell = dt.Rows[i][j] + "";
                        worksheet.Cells[i + addtorow, j + 1] = cell;
                        if (dt.Columns[j].ExtendedProperties.ContainsKey("Formula"+i))
                            try { ((dynamic)worksheet.Cells[i + addtorow, j + 1]).FormulaR1C1 = dt.Columns[j].ExtendedProperties["Formula" + i].ToString().Replace("R[0]C[0]", cell); } catch { }
                        else if (dt.Columns[j].ExtendedProperties.ContainsKey("Formula"))
                            try { ((dynamic)worksheet.Cells[i + addtorow, j + 1]).FormulaR1C1 = dt.Columns[j].ExtendedProperties["Formula"].ToString().Replace("R[0]C[0]", cell); } catch (Exception ex) { }
                        if (dt.Columns[j].ExtendedProperties.ContainsKey("Font" + i))
                            try { ((dynamic)worksheet.Cells[i + addtorow, j + 1]).Font = dt.Columns[j].ExtendedProperties["Font" + i]; } catch { }
                        else if (dt.Columns[j].ExtendedProperties.ContainsKey("Font"))
                            try { ((dynamic)worksheet.Cells[i + addtorow, j + 1]).Font = dt.Columns[j].ExtendedProperties["Font"]; } catch { }
                        if (dt.Columns[j].ExtendedProperties.ContainsKey("Borders" + i))
                            try { ((dynamic)worksheet.Cells[i + addtorow, j + 1]).Borders = dt.Columns[j].ExtendedProperties["Borders" + i]; } catch { }
                        else if (dt.Columns[j].ExtendedProperties.ContainsKey("Borders"))
                            try { ((dynamic)worksheet.Cells[i + addtorow, j + 1]).Borders = dt.Columns[j].ExtendedProperties["Borders"]; } catch { }
                        if (dt.Columns[j].ExtendedProperties.ContainsKey("ForeColor" + i))
                            try { ((dynamic)worksheet.Cells[i + addtorow, j + 1]).ForeColor = dt.Columns[j].ExtendedProperties["ForeColor" + i]; } catch { }
                        else if (dt.Columns[j].ExtendedProperties.ContainsKey("ForeColor"))
                            try { ((dynamic)worksheet.Cells[i + addtorow, j + 1]).ForeColor = dt.Columns[j].ExtendedProperties["ForeColor"]; } catch { }
                        if (dt.Columns[j].ExtendedProperties.ContainsKey("BackColor" + i))
                            try { ((dynamic)worksheet.Cells[i + addtorow, j + 1]).BackColor = dt.Columns[j].ExtendedProperties["BackColor" + i]; } catch { }
                        else if (dt.Columns[j].ExtendedProperties.ContainsKey("BackColor"))
                            try { ((dynamic)worksheet.Cells[i + addtorow, j + 1]).BackColor = dt.Columns[j].ExtendedProperties["BackColor"]; } catch { }
                    }
            }
            else for (int i = 0; i < dt.Rows.Count; i++)
                for (int j = 0; j < dt.Columns.Count; j++)
                    worksheet.Cells[i + addtorow, j + 1] = dt.Rows[i][j] + "";

            if (hascf)
                try
                {
                    workbook.ForceFullCalculation = true;
                    worksheet.EnableCalculation = true;
                    worksheet.Calculate();
                }
                catch { }
        }
        public static DataTable ToDataTable(string exceladdress, string sheetName, string condition = "")
        {
            DataSet ds = ToDataSet(exceladdress, new List<string> { sheetName }, condition);
            if (ds != null && ds.Tables.Count > 0) return ds.Tables[0];
            return null;
        }
        public static DataTable ToDataTable(string exceladdress, string condition = "")
        {
            var dts = ToDataTables(exceladdress, condition);
            if (dts.Any()) return dts.First();
            return null;
        }
        public static string ToString(string exceladdress, string sheetName, string condition = "")
        {
            return ConvertService.ToString(ToDataTable(exceladdress, sheetName, condition));
        }
        public static string ToString(string exceladdress, string condition = "")
        {
            return ConvertService.ToString(ToDataTable(exceladdress, condition));
        }
        public static IEnumerable<DataTable> ToDataTables(string exceladdress, string condition = "")
        {
            using (OleDbConnection connection = new OleDbConnection((exceladdress.TrimEnd().ToLower().EndsWith("x")) ? "Provider=Microsoft.ACE.OLEDB.12.0;Data Source='" + exceladdress + "';" + "Extended Properties='Excel 12.0 Xml;HDR=YES;'"
                : "provider=Microsoft.Jet.OLEDB.4.0;Data Source='" + exceladdress + "';Extended Properties=Excel 8.0;"))
            {
                connection.Open();
                DataTable schema = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                foreach (DataRow drSheet in schema.Rows)
                {
                    var s = drSheet["TABLE_NAME"].ToString();
                    if (s.EndsWith("$") || s.EndsWith("$'") || s.EndsWith("$`"))
                    {
                        //if (s.StartsWith("'")) s = s.Substring(1, s.Length - 2);
                        System.Data.OleDb.OleDbDataAdapter command =
                            new System.Data.OleDb.OleDbDataAdapter(string.Join("", "SELECT * FROM [", s, "] ", condition), connection);
                        DataTable dt = new DataTable();
                        command.Fill(dt);
                        yield return dt;
                    }
                }
                connection.Close();
            }
        }
        public static IEnumerable<IEnumerable<object>> ToSquareMatrix(string exceladdress, string condition = "")
        {
            using (OleDbConnection connection = new OleDbConnection(
                (exceladdress.TrimEnd().ToLower().EndsWith("x")) ? "Provider=Microsoft.ACE.OLEDB.12.0;Data Source='" + exceladdress + "';" + "Extended Properties='Excel 12.0 Xml;HDR=YES;'"
                : "provider=Microsoft.Jet.OLEDB.4.0;Data Source='" + exceladdress + "';Extended Properties=Excel 8.0;"))
            {
                connection.Open();
                DataTable schema = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                foreach (DataRow drSheet in schema.Rows)
                {
                    string s = drSheet["TABLE_NAME"].ToString();
                    if (s.EndsWith("$") || s.EndsWith("$'") || s.EndsWith("$`"))
                    {
                        //if (s.StartsWith("'")) s = s.Substring(1, s.Length - 2);
                        System.Data.OleDb.OleDbDataAdapter command =
                            new System.Data.OleDb.OleDbDataAdapter(string.Join("", "SELECT * FROM [", s, "] ", condition), connection);
                        int start = 0;
                        int number = 1;
                        DataTable dt = new DataTable();
                        command.Fill(start, 10, dt);
                        List<object> lo = new List<object>();
                        foreach (DataColumn item in dt.Columns)
                            lo.Add(item.ColumnName);
                        if (lo.Count > 0) yield return lo;
                        while (number > 0)
                        {
                            dt = new DataTable();
                            number = command.Fill(start, 10000, dt);
                            start += number;
                            foreach (DataRow item in dt.Rows)
                                yield return item.ItemArray;
                        }
                    }
                }
                connection.Close();
            }
        }
        public static IEnumerable<IEnumerable<string>> ToStringSquareMatrix(string exceladdress, string condition = "")
        {
            return from item in ToSquareMatrix(exceladdress, condition)
                select from v in item select v +"";
        }
        public static IEnumerable<string> ToStrings(string exceladdress, string condition = "", string separator = "\t", string quote = "\"")
        {
            foreach (var item in ToSquareMatrix(exceladdress, condition))
                yield return string.Join("\t", from v in item let cell = v+"" select cell.Contains(separator) ? string.Join(cell, quote, quote) :cell);
        }
        public static IEnumerable<string> ToStrings(XmlNode node)
        {
            if (node.HasChildNodes)
                foreach (XmlElement ch in node.ChildNodes)
                    yield return ch.InnerText;
            else yield return node.InnerText;
        }
        public static IEnumerable<string> ToStrings(XmlNodeList childNodes)
        {
            foreach (XmlElement ch in childNodes)
                 yield return ch.InnerText;
        }
        public static DataSet ToDataSet(string exceladdress, string sheetName, string condition = "")
        {
            return ToDataSet(exceladdress, new List<string> { sheetName }, condition);
        }
        public static DataSet ToDataSet(string exceladdress, string condition = "")
        {
            return ToDataSet(exceladdress, 0, -1, condition);
        }
        public static DataSet ToDataSet(string exceladdress, int startRecord = 0, int maxRecord = -1, string condition = "")
        {
            DataSet result = new DataSet();
            using (OleDbConnection connection = new OleDbConnection((exceladdress.TrimEnd().ToLower().EndsWith("x")) ? "Provider=Microsoft.ACE.OLEDB.12.0;Data Source='" + exceladdress + "';" + "Extended Properties='Excel 12.0 Xml;HDR=YES;'"
                : "provider=Microsoft.Jet.OLEDB.4.0;Data Source='" + exceladdress + "';Extended Properties=Excel 8.0;"))
                try
                {
                    connection.Open();
                    DataTable schema = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                    foreach (DataRow drSheet in schema.Rows)
                    {
                        var s = drSheet["TABLE_NAME"].ToString();
                        if (s.EndsWith("$") || s.EndsWith("$'") || s.EndsWith("$`"))
                        {
                            //if (s.StartsWith("'")) s = s.Substring(1, s.Length - 2);
                            System.Data.OleDb.OleDbDataAdapter command =
                                new System.Data.OleDb.OleDbDataAdapter(string.Join("", "SELECT * FROM [", s, "] ", condition), connection);
                            DataTable dt = new DataTable();
                            if (maxRecord > -1 && startRecord > -1) command.Fill(startRecord, maxRecord, dt);
                            else command.Fill(dt);
                            result.Tables.Add(dt);
                        }
                    }
                    return result;
                }
                catch { return null; }
                finally { connection.Close(); }
        }
        public static DataSet ToDataSet(string exceladdress, IEnumerable<string> sheetNames, string condition = "")
        {
            OleDbConnection MyConnection = new OleDbConnection((exceladdress.TrimEnd().ToLower().EndsWith("x")) ? "Provider=Microsoft.ACE.OLEDB.12.0;Data Source='" + exceladdress + "';" + "Extended Properties='Excel 12.0 Xml;HDR=YES;'"
                : "provider=Microsoft.Jet.OLEDB.4.0;Data Source='" + exceladdress + "';Extended Properties=Excel 8.0;");
            try
            {
                System.Data.DataSet result = new System.Data.DataSet();
                foreach (var item in sheetNames)
                {
                    System.Data.OleDb.OleDbDataAdapter command = new System.Data.OleDb.OleDbDataAdapter(string.Join("", "SELECT * FROM [", item, "$] ", condition), MyConnection);
                    DataTable dt = new DataTable();
                    command.Fill(dt);
                    result.Tables.Add(dt);
                }
                return result;
            }
            catch { return null; }
            finally { MyConnection.Close(); }
        }
        public static SmartTable ToSmartTable(string exceladdress, string sheetName, string condition = "")
        {
            DataTable dt = ToDataTable(exceladdress, sheetName, condition);
            if (dt == null) return null;
            return new SmartTable(dt);
        }
        public static IEnumerable<SmartTable> ToSmartTables(string exceladdress, int startRecord = 0, int maxRecord = -1, string condition = "")
        {
            DataSet ds = ToDataSet(exceladdress, startRecord, maxRecord, condition);
            if (ds == null) yield break;
            foreach (DataTable item in ds.Tables)
                yield return new SmartTable(item);
        }
        public static IEnumerable<string> ToSheets(string exceladdress)
        {
            using (OleDbConnection connection = new OleDbConnection((exceladdress.TrimEnd().ToLower().EndsWith("x")) ? "Provider=Microsoft.ACE.OLEDB.12.0;Data Source='" + exceladdress + "';" + "Extended Properties='Excel 12.0 Xml;HDR=YES;'"
                : "provider=Microsoft.Jet.OLEDB.4.0;Data Source='" + exceladdress + "';Extended Properties=Excel 8.0;"))
            {
                connection.Open();
                try
                {
                    DataTable dt = connection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                    foreach (DataRow drSheet in dt.Rows)
                    {
                        string s = drSheet["TABLE_NAME"].ToString();
                        yield return s.StartsWith("'") ? s.Substring(1, s.Length - 3) : s.Substring(0, s.Length - 1);
                    }
                }
                finally { connection.Close(); }
            }
        }
        #endregion

        #region XAML
        public static string ToXAML(string rtfText)
        {
            var richTextBox = new System.Windows.Controls.RichTextBox();
            if (string.IsNullOrEmpty(rtfText)) return "";
            var textRange = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
            using (var rtfMemoryStream = new MemoryStream())
            {
                using (var rtfStreamWriter = new StreamWriter(rtfMemoryStream))
                {
                    rtfStreamWriter.Write(rtfText);
                    rtfStreamWriter.Flush();
                    rtfMemoryStream.Seek(0, SeekOrigin.Begin);
                    textRange.Load(rtfMemoryStream, System.Windows.DataFormats.Rtf);
                }
            }
            using (var rtfMemoryStream = new MemoryStream())
            {
                textRange = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
                textRange.Save(rtfMemoryStream, System.Windows.DataFormats.Xaml);
                rtfMemoryStream.Seek(0, SeekOrigin.Begin);
                using (var rtfStreamReader = new StreamReader(rtfMemoryStream))
                {
                    return rtfStreamReader.ReadToEnd();
                }
            }
        }
        public static string ToRTF(string xamlText)
        {
            var richTextBox = new System.Windows.Controls.RichTextBox();
            if (string.IsNullOrEmpty(xamlText)) return "";
            var textRange = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
            using (var xamlMemoryStream = new MemoryStream())
            {
                using (var xamlStreamWriter = new StreamWriter(xamlMemoryStream))
                {
                    xamlStreamWriter.Write(xamlText);
                    xamlStreamWriter.Flush();
                    xamlMemoryStream.Seek(0, SeekOrigin.Begin);
                    textRange.Load(xamlMemoryStream, System.Windows.DataFormats.Xaml);
                }
            }
            using (var rtfMemoryStream = new MemoryStream())
            {
                textRange = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);
                textRange.Save(rtfMemoryStream, System.Windows.DataFormats.Rtf);
                rtfMemoryStream.Seek(0, SeekOrigin.Begin);
                using (var rtfStreamReader = new StreamReader(rtfMemoryStream))
                {
                    return rtfStreamReader.ReadToEnd();
                }
            }
        }
        #endregion

        #region XML
        public static List<XMLElement> ToXMLElements_Quick1(string xml)
        {
            List<KeyValuePair<int, XMLElement>> nested = new List<KeyValuePair<int, XMLElement>>();
            Stack<XMLElement> child = new Stack<XMLElement>();
            Stack<string> stack = new Stack<string>();
            string startTag = "", endTag = "";
            string mhtml = "";
            RouteMode route = RouteMode.Null;

            Func<bool> textcheck = () =>
           {
               bool t = false;
               if (t = !string.IsNullOrWhiteSpace(startTag))
                   stack.Push(startTag.Trim());
               startTag = "";
               return t;
           };
            Func<int, bool> helcheck = (count) =>
            {
                for (int i = nested.Count - 1; i >= 0; i--)
                    if (nested[i].Key > count)
                    {
                        child.Push(nested[i].Value);
                        nested.RemoveAt(i);
                    }
                return true;
            };

            for (int i = 0; i < xml.Length; i++)
            {
                mhtml = xml.Substring(i);
                switch (route)
                {
                    case RouteMode.Start:
                        if (mhtml.StartsWith(">"))
                        {
                            startTag += ">";
                            stack.Push(startTag);
                            startTag = "";
                            route = RouteMode.Middle;
                        }
                        else startTag += xml[i];
                        break;
                    case RouteMode.Null:
                    case RouteMode.Middle:
                        if (mhtml.StartsWith("</"))
                        {
                            textcheck();
                            endTag = "</";
                            i++;
                            route = RouteMode.End;
                        }
                        else if (mhtml.StartsWith("<"))
                        {
                            textcheck();
                            startTag = "<";
                            route = RouteMode.Start;
                        }
                        else startTag += xml[i];
                        break;
                    case RouteMode.End:
                        if (mhtml.StartsWith(">"))
                        {
                            textcheck();
                            endTag += ">";
                            string tne = endTag.Replace("</", "").Replace("<", "").Replace(">", "").Trim().ToUpper();
                            int num = -1;
                            if ((num = stack.ToList().FindIndex((x) =>
                            {
                                string tns = x.StartsWith("<") ? StringService.FirstSplit(x + " ", " ", 1).First().Replace("<", "").Replace("/>", "").Replace(">", "").ToUpper() : "";
                                return tns == tne;
                            })) > -1)
                            {
                                int ni = 0;
                                string ss, tns;
                                while (ni++ < num)
                                {
                                    ss = stack.Pop();
                                    bool isTag = ss.StartsWith("<");
                                    tns = isTag ? StringService.FirstSplit(ss + " ", " ", 1).First().Replace("<", "").Replace("/>", "").Replace(">", "").ToUpper() : "";
                                    helcheck(stack.Count);
                                    child.Push(new XMLElement(-1, tns, ss, ""));
                                }
                                ss = stack.Pop();
                                tns = StringService.FirstSplit(ss + " ", " ", 1).First().Replace("<", "").Replace("/>", "").Replace(">", "").ToUpper();
                                helcheck(stack.Count);
                                XMLElement elem = new XMLElement(-1, tns, ss, endTag);
                                elem.Children = child.ToList();
                                child = new Stack<XMLElement>();
                                nested.Add(new KeyValuePair<int, XMLElement>(stack.Count, elem));
                            }
                            endTag = "";
                            route = RouteMode.Null;
                        }
                        else endTag += xml[i];
                        break;
                }
            }
            while (stack.Count > 0)
            {
                helcheck(stack.Count);
                child.Push(new XMLElement(child.Count, "", stack.Pop(), ""));
            }
            helcheck(0);

            List<XMLElement> res = child.ToList();
            for (int i = 0; i < res.Count; i++)
            {
                res[i].Index = i;
                res[i].Parent = null;
                res[i].Refresh();
            }
            return res;
        }
        public static List<XMLElement> ToXMLElements_Quick2(string xml)
        {
            List<KeyValuePair<int, XMLElement>> nested = new List<KeyValuePair<int, XMLElement>>();
            Stack<XMLElement> child = new Stack<XMLElement>();
            Stack<string> stack = new Stack<string>();
            string startTag = "", endTag = "";
            string mhtml = "";
            RouteMode route = RouteMode.Null;

            Func<bool> textcheck = () =>
           {
               bool t = false;
               if (t = !string.IsNullOrWhiteSpace(startTag))
                   stack.Push(startTag.Trim());
               startTag = "";
               return t;
           };
            Func<int, bool> nestedcheck = (tothisindex) =>
            {
                for (int i = nested.Count - 1; i >= 0; i--)
                    if (nested[i].Key > tothisindex)
                    {
                        child.Push(nested[i].Value);
                        nested.RemoveAt(i);
                    }
                return true;
            };

            for (int i = 0; i < xml.Length; i++)
            {
                mhtml = xml.Substring(i);
                switch (route)
                {
                    case RouteMode.Start:
                        if (mhtml.StartsWith("/>"))
                        {
                            startTag += "/>";
                            stack.Push(startTag);
                            startTag = "";
                            i++;
                            route = RouteMode.Null;
                        }
                        else if (mhtml.StartsWith(">"))
                        {
                            startTag += ">";
                            stack.Push(startTag);
                            startTag = "";
                            route = RouteMode.Middle;
                        }
                        else startTag += xml[i];
                        break;
                    case RouteMode.Null:
                    case RouteMode.Middle:
                        if (mhtml.StartsWith("</"))
                        {
                            textcheck();
                            endTag = "</";
                            i++;
                            route = RouteMode.End;
                        }
                        else if (mhtml.StartsWith("<"))
                        {
                            textcheck();
                            startTag = "<";
                            route = RouteMode.Start;
                        }
                        else startTag += xml[i];
                        break;
                    case RouteMode.End:
                        if (mhtml.StartsWith(">"))
                        {
                            textcheck();
                            endTag += ">";
                            string endtagname = endTag.Replace("</", "").Replace("<", "").Replace(">", "").Trim().ToUpper();
                            int starttagindex = -1;
                            if ((starttagindex = stack.ToList().FindIndex((x) =>
                            {
                                if (x.StartsWith("<"))
                                {
                                    string tns = StringService.FirstSplit(x + " ", " ", 1).First().Replace("<", "").Replace(">", "").ToUpper();
                                    return tns == endtagname;
                                }
                                return false;
                            })) > -1)
                            {
                                int ni = 0;
                                string starttag, starttagname;
                                while (ni++ < starttagindex)
                                {
                                    starttag = stack.Pop();
                                    bool isTag = starttag.StartsWith("<");
                                    nestedcheck(stack.Count);
                                    starttagname = isTag? StringService.FirstSplit(starttag + " ", " ", 1).First().Replace("<", "").Replace(">", "").ToUpper():"";
                                    child.Push(new XMLElement(-1, starttagname, starttag, ""));
                                }
                                if (stack.Count > 0)
                                {
                                    starttag = stack.Pop();
                                    starttagname = StringService.FirstSplit(starttag + " ", " ", 1).First().Replace("<", "").Replace(">", "").ToUpper();
                                }
                                else
                                {
                                    starttag = "";
                                    starttagname = endtagname;
                                }
                                nestedcheck(stack.Count);
                                XMLElement elem = new XMLElement(-1, starttagname, starttag, endTag);
                                elem.Children = child.ToList();
                                child = new Stack<XMLElement>();
                                nested.Add(new KeyValuePair<int, XMLElement>(stack.Count, elem));
                            }
                            endTag = "";
                            route = RouteMode.Null;
                        }
                        else endTag += xml[i];
                        break;
                }
            }
            while (stack.Count > 0)
            {
                nestedcheck(stack.Count);
                child.Push(new XMLElement(child.Count, "", stack.Pop(), ""));
            }
            nestedcheck(0);

            List<XMLElement> result = child.ToList();
            for (int i = 0; i < result.Count; i++)
            {
                result[i].Index = i;
                result[i].Parent = null;
                result[i].Refresh();
            }
            return result;
        }
        public static List<XMLElement> ToXMLElements(string xml)
        {
            Stack<KeyValuePair<bool, XMLElement>> resultStack = new Stack<KeyValuePair<bool, XMLElement>>();
            string startTag = "", endTag = "";
            string mhtml = "";
            RouteMode route = RouteMode.Null;
            string quote = "'", dblquote = "\"", cmnt = "<!-", sign = "";
            Func<string, bool> Append = (value) =>
             {
                 if (route == RouteMode.End)
                     endTag += value;
                 else startTag += value;
                 return true;
             };
            Func<bool> StartTag = () =>
            {
                startTag = startTag.Trim();
                if (string.IsNullOrEmpty(startTag)) return false;
                if (startTag.StartsWith("<"))// if is tag
                {
                    string[] tag = StringService.FirstSplit(startTag + " ", " ", 1);
                    string tagname = tag.First().Replace("<", "").Replace("/>", "").Replace(">", "").ToLower();
                    resultStack.Push(new KeyValuePair<bool, XMLElement>(
                        !startTag.EndsWith("/>"),
                        new XMLElement(resultStack.Count, tagname, startTag, null, null)));
                }
                else // if is text
                {
                    resultStack.Push(new KeyValuePair<bool, XMLElement>(
                        false,
                        new XMLElement(resultStack.Count, "", startTag, "", null, null)));
                }
                startTag = "";
               return true;
            };
            Func<bool> EndTag = () =>
            {
                string tagname = endTag.Replace("</", "").Replace("<", "").Replace(">", "").Trim().ToLower();
                if (string.IsNullOrEmpty(tagname)) return false;
                bool find = false;
                foreach (var item in resultStack)
                    if (find = item.Key && item.Value.TagName == tagname)
                        break;
                if (find)
                {
                    Stack<XMLElement> childStack = new Stack<XMLElement>();
                    find = false;
                    while (!find)
                    {
                        KeyValuePair<bool, XMLElement> kvp = resultStack.Pop();
                        if (find = kvp.Key && kvp.Value.TagName == tagname)
                        {
                            kvp.Value.Index = resultStack.Count;
                            kvp.Value.Children = childStack.ToList();
                            kvp.Value.EndTag = endTag;
                            resultStack.Push(new KeyValuePair<bool, XMLElement>(false, kvp.Value));
                        }
                        else childStack.Push(kvp.Value);
                    }
                }
                endTag = "";
                return true;
            };
            for (int i = 0; i < xml.Length; i++)
            {
                mhtml = xml.Substring(i);
                if (sign == "")
                    if (mhtml.StartsWith(quote))
                    { Append(sign = quote); i += quote.Length - 1; }
                    else if (mhtml.StartsWith(dblquote))
                    { Append(sign = dblquote); i += dblquote.Length - 1; }
                    else if (mhtml.StartsWith(cmnt))
                    { sign = "-->"; Append(cmnt); i += cmnt.Length - 1; }
                    else switch (route)
                        {
                            case RouteMode.Start:
                                if (mhtml.StartsWith("/>"))
                                {
                                    startTag += "/>";
                                    StartTag();
                                    i++;
                                    route = RouteMode.Null;
                                }
                                else if (mhtml.StartsWith(">"))
                                {
                                    startTag += ">";
                                    StartTag();
                                    route = RouteMode.Middle;
                                }
                                else startTag += xml[i];
                                break;
                            case RouteMode.End:
                                if (mhtml.StartsWith(">"))
                                {
                                    endTag += ">";
                                    EndTag();
                                    route = RouteMode.Null;
                                }
                                else endTag += xml[i];
                                break;
                            //case MiMFa_Route.Null:
                            //case MiMFa_Route.Middle:
                            default:
                                if (mhtml.StartsWith("</"))
                                {
                                    StartTag();
                                    endTag = "</";
                                    i++;
                                    route = RouteMode.End;
                                }
                                else if (mhtml.StartsWith("<"))
                                {
                                    StartTag();
                                    startTag = "<";
                                    route = RouteMode.Start;
                                }
                                else startTag += xml[i];
                                break;
                        }
                else if (mhtml.StartsWith(sign))
                { Append(sign); i += sign.Length - 1; sign = ""; }
                else Append(xml[i] + "");
            }
            List<XMLElement> result = (resultStack.Count > 0)? (from kvp in resultStack select kvp.Value).Reverse().ToList():new List<XMLElement>();
            for (int i = 0; i < result.Count; i++)
                result[i].Refresh();
            return result;
        }

        public static Dictionary<string,string> ToDictionaryRecord(List<XMLElement> xmls,string attrName)
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            for (int i = 0; i < xmls.Count; i++)
                if (string.IsNullOrWhiteSpace(xmls[i].TagName))
                {
                    string p = xmls[i].Parent == null ? "" : xmls[i].Parent.TagName + ":"+xmls[i].Parent.GetAttribute(attrName);
                    try { dic.Add(p, xmls[i].StartTag); }
                    catch { dic.Add(p + "_" + dic.Count, xmls[i].StartTag); }
                }
                else
                {
                    Dictionary<string, string> d = ToDictionaryRecord(xmls[i].Children, attrName);
                    foreach (var item in d)
                        try { dic.Add(item.Key, item.Value); }
                        catch { dic.Add(item.Key+ "_" + dic.Count, item.Value); }
                }
            return dic;
        }
        public static List<KeyValuePair<string, string>> ToListRecord(List<XMLElement> xmls,string attrName)
        {
            List<KeyValuePair<string, string>> dic = new List<KeyValuePair<string, string>>();
            for (int i = 0; i < xmls.Count; i++)
                if (string.IsNullOrWhiteSpace(xmls[i].TagName))
                {
                    string p = xmls[i].Parent == null ? "" : xmls[i].Parent.TagName + ":"+xmls[i].Parent.GetAttribute(attrName);
                    dic.Add(new KeyValuePair<string, string>(p, xmls[i].StartTag));
                }
                else
                {
                    List<KeyValuePair<string, string>> d = ToListRecord(xmls[i].Children, attrName);
                    dic.AddRange(d);
                }
            return dic;
        }

        #endregion
    }
}

