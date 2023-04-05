using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;
using System.Drawing;
using MiMFa.General;
using MiMFa.Service;

namespace MiMFa.Exclusive.DateAndTime
{
    [Serializable]
    public class SmartDateTime
    {
        public Calendar ZoneCalendar = new GregorianCalendar();
        public TimeZoneMode TimeZone
        {
            get { return _TimeZone; }
            set
            {
                switch (value)
                {
                    case TimeZoneMode.IranStandard:
                        ZoneCalendar = new PersianCalendar();
                        break;
                    case TimeZoneMode.IranDaylight:
                        ZoneCalendar = new PersianCalendar();
                        break;
                    case TimeZoneMode.ArabiaStandard:
                        ZoneCalendar = new HijriCalendar();
                        break;
                    case TimeZoneMode.GeorgiaStandard:
                        ZoneCalendar = new GregorianCalendar();
                        break;
                    case TimeZoneMode.JapanStandard:
                        ZoneCalendar = new JapaneseCalendar();
                        break;
                    case TimeZoneMode.IsraelStandard:
                        ZoneCalendar = new HebrewCalendar();
                        break;
                    case TimeZoneMode.IsraelDaylight:
                        ZoneCalendar = new HebrewCalendar();
                        break;
                    case  TimeZoneMode.KoreaStandard:
                        ZoneCalendar = new KoreanCalendar();
                        break;
                    case TimeZoneMode.China:
                        ZoneCalendar = new TaiwanCalendar();
                        break;
                    default:
                        ZoneCalendar = new GregorianCalendar();
                        break;
                }
                _TimeZone = value;
            }
        }
        private TimeZoneMode _TimeZone = TimeZoneMode.Null;
        public DateTime DateTime { get => _DateTime??(_TimeZone == TimeZoneMode.Null? DateTime.Now : DateTime.UtcNow); set=> _DateTime=value; }
        private DateTime? _DateTime = null;

        public SmartDateTime()
        {

        }
        public SmartDateTime(TimeZoneMode timeZone = TimeZoneMode.Null)
        {
            TimeZone = timeZone;
        }
        public SmartDateTime(DateTime dateTime, TimeZoneMode timeZone = TimeZoneMode.Null)
        {
            TimeZone = timeZone;
            DateTime = dateTime;
        }

        public override string ToString()
        {
            return GetDateTime();
        }
        public string ToString(string format)
        {
            return GetDateTime(format);
        }

        public string GetDateTime(string format = "{0:d4}-{1:d2}-{2:d2} {3:d2}:{4:d2}:{5:d2}")
        {
            DateTime dt = DateTime;
            if (_TimeZone == TimeZoneMode.Null)
                return string.Format(format, dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second);
            return string.Format(format, ZoneCalendar.GetYear(dt), ZoneCalendar.GetMonth(dt), ZoneCalendar.GetDayOfMonth(dt), ZoneCalendar.GetHour(dt), ZoneCalendar.GetMinute(dt), ZoneCalendar.GetSecond(dt));
        }


        public SmartDate GetSmartDate()
        {
            DateTime dt = DateTime;
            if (_TimeZone == TimeZoneMode.Null)
                return new SmartDate(dt.Year, dt.Month, dt.Day);
            return new SmartDate(ZoneCalendar.GetYear(dt), ZoneCalendar.GetMonth(dt), ZoneCalendar.GetDayOfMonth(dt));
        }
        public string GetDate(string format = "{0:d4}-{1:d2}-{2:d2}")
        {
            DateTime dt = DateTime;
            if (_TimeZone == TimeZoneMode.Null)
                return string.Format(format, dt.Year, dt.Month, dt.Day);
            return string.Format(format, ZoneCalendar.GetYear(dt), ZoneCalendar.GetMonth(dt), ZoneCalendar.GetDayOfMonth(dt));
        }
        public int GetYear()
        {
            if (_TimeZone == TimeZoneMode.Null)
                return DateTime.Year;
            return ZoneCalendar.GetYear(DateTime);
        }
        public int GetMonth()
        {
            if (_TimeZone == TimeZoneMode.Null)
                return DateTime.Month;
            return ZoneCalendar.GetMonth(DateTime);
        }
        public int GetDay()
        {
            if (_TimeZone == TimeZoneMode.Null)
                return DateTime.Day;
            return ZoneCalendar.GetDayOfMonth(DateTime);
        }
        public DayOfWeek GetDayOfWeek()
        {
            if (_TimeZone == TimeZoneMode.Null)
                return DateTime.DayOfWeek;
            return ZoneCalendar.GetDayOfWeek(DateTime);
        }
        public int GetDayOfWeekNumber()
        {
            return ConvertService.ToHijriDayOfWeekNum( GetDayOfWeek());
        }
        public string GetDayOfWeekName(int dayOfWeek = -1)
        {
            if (dayOfWeek < 0) return ConvertService.ToDayOfWeekName(this.GetSmartDate(), this);
            return ConvertService.ToDayOfWeekName(dayOfWeek, this);
        }
        public string GetMonthName()
        {
            return ConvertService.ToMonthName(GetSmartDate(), this);
        }
        public int GetDayOfYear()
        {
            if (_TimeZone == TimeZoneMode.Null)
                return DateTime.DayOfYear;
            return ZoneCalendar.GetDayOfYear(DateTime);
        }

        public SmartTime GetSmartTime()
        {
            DateTime dt = DateTime;
            if (_TimeZone == TimeZoneMode.Null)
                return new SmartTime(dt.Hour, dt.Minute, dt.Second);
            return new SmartTime(ZoneCalendar.GetHour(dt), ZoneCalendar.GetMinute(dt), ZoneCalendar.GetSecond(dt));
        }
        public string GetTime(string format = "{0:d2}:{1:d2}:{2:d2}")
        {
            DateTime dt = DateTime;
            if (_TimeZone == TimeZoneMode.Null)
                return string.Format(format, dt.Hour, dt.Minute, dt.Second);
            return string.Format(format, ZoneCalendar.GetHour(dt), ZoneCalendar.GetMinute(dt), ZoneCalendar.GetSecond(dt));
        }
        public int GetHour()
        {
            if (_TimeZone == TimeZoneMode.Null)
                return DateTime.Hour;
            return ZoneCalendar.GetHour(DateTime);
        }
        public int GetMinute()
        {
            if (_TimeZone == TimeZoneMode.Null)
                return DateTime.Minute;
            return ZoneCalendar.GetMinute(DateTime);
        }
        public int GetSecond()
        {
            if (_TimeZone == TimeZoneMode.Null)
                return DateTime.Second;
            return ZoneCalendar.GetSecond(DateTime);
        }
        public Color GetSkyColor()
        {
            switch (GetHour())
            {
                case 0: return Color.MidnightBlue;
                case 1: return Color.MidnightBlue;
                case 2: return Color.Navy;
                case 3: return Color.DarkBlue;
                case 4: return Color.MediumBlue;
                case 5: return Color.RoyalBlue;
                case 6: return Color.CornflowerBlue;
                case 7: return Color.Lavender;
                case 8: return Color.AliceBlue;
                case 9: return Color.GhostWhite;
                case 10: return Color.MintCream;
                case 11: return Color.Honeydew;
                case 12: return Color.Ivory;
                case 13: return Color.LightYellow;
                case 14: return Color.Cornsilk;
                case 15: return Color.OldLace;
                case 16: return Color.PapayaWhip;
                case 17: return Color.NavajoWhite;
                case 18: return Color.Tan;
                case 19: return Color.SlateGray;
                case 20: return Color.DarkSlateGray;
                case 21: return Color.DarkBlue;
                case 22: return Color.Navy;
                case 23: return Color.MidnightBlue;
                default: return Color.DeepSkyBlue;
            }
        }

        public static SmartDate GetCurrentSmartDate(Calendar zoneCalendar = null)
        {
            DateTime dt = DateTime.Now;
            if (zoneCalendar == null)
                return new SmartDate(dt.Year, dt.Month, dt.Day);
            dt = DateTime.UtcNow;
            return new SmartDate(zoneCalendar.GetYear(dt), zoneCalendar.GetMonth(dt), zoneCalendar.GetDayOfMonth(dt));
        }
        public static string GetCurrentDate(string format = "{0:d4}-{1:d2}-{2:d2}", Calendar zoneCalendar = null)
        {
            DateTime dt = DateTime.Now;
            if (zoneCalendar == null)
                return string.Format(format, dt.Year, dt.Month, dt.Day);
            dt = DateTime.UtcNow;
            return string.Format(format, zoneCalendar.GetYear(dt), zoneCalendar.GetMonth(dt), zoneCalendar.GetDayOfMonth(dt));
        }

        public static SmartTime GetCurrentSmartTime(Calendar zoneCalendar = null)
        {
            DateTime dt = DateTime.Now;
            if (zoneCalendar == null)
            return new SmartTime(dt.Hour, dt.Minute, dt.Second);
            dt = DateTime.UtcNow;
            return new SmartTime(zoneCalendar.GetHour(dt), zoneCalendar.GetMinute(dt), zoneCalendar.GetSecond(dt));
        }
        public static string GetCurrentTime(string format = "{0:d2}:{1:d2}:{2:d2}", Calendar zoneCalendar = null)
        {
            DateTime dt = DateTime.Now;
            if (zoneCalendar == null)
                return string.Format(format, dt.Hour, dt.Minute, dt.Second);
            dt = DateTime.UtcNow;
            return string.Format(format, zoneCalendar.GetHour(dt), zoneCalendar.GetMinute(dt), zoneCalendar.GetSecond(dt));
        }
}
}
