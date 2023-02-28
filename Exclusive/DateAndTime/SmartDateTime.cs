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
        public Calendar ZoneCalendar = new PersianCalendar();
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
        private TimeZoneMode _TimeZone = TimeZoneMode.IranStandard;

        public SmartDateTime()
        { }
        public SmartDateTime(TimeZoneMode timeZone = TimeZoneMode.IranStandard)
        {
            TimeZone = timeZone;
        }

        public SmartDate GetDatePAC()
        {
            DateTime dt = DateTime.Now;
            if (ZoneCalendar == null)
                return new SmartDate(dt.Year, dt.Month, dt.Day);
            else
                return new SmartDate(ZoneCalendar.GetYear(dt), ZoneCalendar.GetMonth(dt), ZoneCalendar.GetDayOfMonth(dt));
        }
        public string GetDate()
        {
            DateTime dt = DateTime.Now;
            if (ZoneCalendar == null)
                return string.Format("{0:d2}\\{1:d2}\\{2:d4}", dt.Day, dt.Month, dt.Year);
            else
                return string.Format("{0:d2}\\{1:d2}\\{2:d4}", ZoneCalendar.GetDayOfMonth(dt), ZoneCalendar.GetMonth(dt), ZoneCalendar.GetYear(dt));
        }
        public int GetYear()
        {
            DateTime dt = DateTime.Now;
            if (ZoneCalendar == null)
                return dt.Year;
            else
                return ZoneCalendar.GetYear(dt);
        }
        public int GetMonth()
        {
            DateTime dt = DateTime.Now;
            if (ZoneCalendar == null)
                return dt.Month;
            else
                return ZoneCalendar.GetMonth(dt);
        }
        public int GetDay()
        {
            DateTime dt = DateTime.Now;
            if (ZoneCalendar == null)
                return dt.Day;
            else
                return ZoneCalendar.GetDayOfMonth(dt);
        }
        public DayOfWeek GetDayOfWeek()
        {
            DateTime dt = DateTime.Now;
            if (ZoneCalendar == null)
                return dt.DayOfWeek;
            else
                return ZoneCalendar.GetDayOfWeek(dt);
        }
        public int GetDayOfWeekNumber()
        {
            return ConvertService.ToHijriDayOfWeekNum( GetDayOfWeek());
        }
        public string GetDayOfWeekName(int dayOfWeek = -1)
        {
            if (dayOfWeek < 0) return ConvertService.ToDayOfWeekName(this.GetDatePAC(), this);
            return ConvertService.ToDayOfWeekName(dayOfWeek, this);
        }
        public string GetMonthName()
        {
            return ConvertService.ToMonthName(GetDatePAC(), this);
        }
        public int GetDayOfYear()
        {
            DateTime dt = DateTime.Now;
            if (ZoneCalendar == null)
                return dt.DayOfYear;
            else
                return ZoneCalendar.GetDayOfYear(dt);
        }

        public SmartTime GetTimePAC()
        {
            DateTime dt = DateTime.Now;
            return new SmartTime( dt.Hour, dt.Minute, dt.Second);
        }
        public string GetTime()
        {
            DateTime dt = DateTime.Now;
            return string.Format("{0:d2}:{1:d2}:{2:d2}", dt.Hour, dt.Minute, dt.Second);
        }
        public int GetHour()
        {
            DateTime dt = DateTime.Now;
            if (ZoneCalendar == null)
                return dt.Hour;
            else
                return ZoneCalendar.GetHour(dt);
        }
        public int GetMinute()
        {
            DateTime dt = DateTime.Now;
            if (ZoneCalendar == null)
                return dt.Minute;
            else
                return ZoneCalendar.GetMinute(dt);
        }
        public int GetSecond()
        {
            DateTime dt = DateTime.Now;
            if (ZoneCalendar == null)
                return dt.Second;
            else
                return ZoneCalendar.GetSecond(dt);
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

        public static SmartTime GetThisTimePAC()
        {
            DateTime dt = DateTime.Now;
            return new SmartTime(dt.Hour, dt.Minute, dt.Second);
        }
        public static string GetThisTime()
        {
            DateTime dt = DateTime.Now;
            return string.Format("{0:d2}:{1:d2}:{2:d2}", dt.Hour, dt.Minute, dt.Second);
        }

        public static SmartDate GetThisDatePAC(Calendar zoneCalendar)
        {
            DateTime dt = DateTime.Now;
            if (zoneCalendar == null)
                return new SmartDate(dt.Year, dt.Month, dt.Day);
            else
                return new SmartDate(zoneCalendar.GetYear(dt), zoneCalendar.GetMonth(dt), zoneCalendar.GetDayOfMonth(dt));
        }
        public static string GetThisDate(Calendar zoneCalendar)
        {
            DateTime dt = DateTime.Now;
            if (zoneCalendar == null)
                return string.Format("{0:d2}\\{1:d2}\\{2:d4}", dt.Day, dt.Month, dt.Year);
            else
                return string.Format("{0:d2}\\{1:d2}\\{2:d4}", zoneCalendar.GetDayOfMonth(dt), zoneCalendar.GetMonth(dt), zoneCalendar.GetYear(dt));
        }
    }
}
