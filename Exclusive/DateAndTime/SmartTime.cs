using MiMFa.General;
using MiMFa.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MiMFa.Exclusive.DateAndTime
{
    [Serializable]
    public class SmartTime
    {
        public long Hour
        {
            get { return _Hour; }
            set
            {
                if (value > 23)
                {
                    _Hour = value % 24;
                }
                else if (value < 0)
                {
                    _Hour = 24 + value % 24;
                }
                else _Hour = value;
            }
        }
        public long Minute
        {
            get { return _Minute; }
            set
            {
                if (value > 59)
                {
                    Hour += value/60;
                    _Minute = value % 60;
                }
                else if (value < 0)
                {
                    Hour -= ((value / 60) + 1);
                    _Minute = 60 + value % 60;
                }
                else _Minute = value;
            }
        }
        public long Second
        {
            get { return _Second; }
            set
            {
                if (value > 59)
                {
                    Minute += value/60;
                    _Second = value % 60;
                }
                else if (value < 0)
                {
                    Minute -= ((value/60) +1);
                    _Second = 60 + value % 60;
                }
                else _Second = value;
            }
        }

        /// <summary>
        /// Unlimited Hour
        /// </summary>
        public long ULHour
        {
            get { return _Hour; }
            set
            {_Hour = value;
            }
        }
        /// <summary>
        /// Unlimited Minute
        /// </summary>
        public long ULMinute
        {
            get { return _Minute; }
            set
            {
                if (value > 59)
                {
                    ULHour += value / 60;
                    _Minute = value % 60;
                }
                else if (value < 0)
                {
                    ULHour -= ((value / 60) + 1);
                    _Minute = 60 + value % 60;
                }
                else _Minute = value;
            }
        }
        /// <summary>
        /// Unlimited ULSecond
        /// </summary>
        public long ULSecond
        {
            get { return _Second; }
            set
            {
                if (value > 59)
                {
                    ULMinute += value / 60;
                    _Second = value % 60;
                }
                else if (value < 0)
                {
                    ULMinute -= ((value / 60) + 1);
                    _Second = 60 + value % 60;
                }
                else _Second = value;
            }
        }
        public double LengthHour
        {
           get{ return(this != new SmartTime(0, 0, 0))? 
                    GetLengthHour(new SmartTime(0, 0, 0), this) :
                    0;}
        }
        public double LengthMinute
        {
           get{ return (this != new SmartTime(0, 0, 0)) ?
                    GetLengthMinute(new SmartTime(0, 0, 0), this) :0;}
        }
        public double LengthSecond
        {
            get { return (this != new SmartTime(0, 0, 0)) ?
                    GetLengthSecond(new SmartTime(0, 0, 0), this) :0; }
        }

        #region override
        public override bool Equals(object op1)
        {
            if (op1 == null)
            {
                return false;
            }
            try
            {
                return (this == (SmartTime)op1);
            }
            catch
            {
                return false;
            }
        }
        public override string ToString()
        {
            return this.GetTime();
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
        public static implicit operator string(SmartTime op1)
        {
            return op1.GetTime();
        }
        public static implicit operator long(SmartTime op1)
        {
            return Convert.ToInt64(string.Format("{0}{1:d2}{2:d2}", op1.Hour ,op1.Minute ,op1.Second));
        }
        public static implicit operator double(SmartTime op1)
        {
            return Convert.ToDouble(string.Format("{0}{1:d2}{2:d2}", op1.Hour, op1.Minute, op1.Second));
        }
        public static implicit operator decimal(SmartTime op1)
        {
            return Convert.ToDecimal(string.Format("{0}{1:d2}{2:d2}", op1.Hour, op1.Minute, op1.Second));
        }
        public static implicit operator float(SmartTime op1)
        {
            return Convert.ToSingle(string.Format("{0}{1:d2}{2:d2}", op1.Hour , op1.Minute , op1.Second));
        }
        public static implicit operator int(SmartTime op1)
        {
            return Convert.ToInt32(string.Format("{0}{1:d2}{2:d2}", op1.Hour , op1.Minute , op1.Second));
        }
        public static implicit operator byte[] (SmartTime op1)
        {
            return IOService.Serialize(op1);
        }
        public static implicit operator DateTime(SmartTime op1)
        {
            return GetDateTime(op1);
        }

        public static explicit operator SmartTime(DateTime op1)
        {
            return ConvertService.ToMiMFaTime(op1);
        }
        public static explicit operator SmartTime(SmartDateTime dateTime)
        {
            SmartTime d = new SmartTime();
            d.SetTime(dateTime.GetYear(), dateTime.GetMonth(), dateTime.GetDay());
            return d;
        }
        public static explicit operator SmartTime(string op1)
        {
            return ConvertService.ToMiMFaTime(op1);
        }
        public static explicit operator SmartTime(double op1)
        {
            var t = new SmartTime();
            t.SetTime(op1);
            return t;
        }

        public static SmartTime operator !(SmartTime op1)
        {
            if (op1.Hour > 12) op1.Hour = op1.Hour - 12;
            else op1.Hour = op1.Hour + 12;
            return op1;
        }
        public static SmartTime operator ++(SmartTime op1)
        {
            op1.Second++;
            return op1;
        }
        public static SmartTime operator --(SmartTime op1)
        {
            op1.Second--;
            return op1;
        }
        public static SmartTime operator +(SmartTime op1, SmartTime op2)
        {
           var op = new SmartTime( op1);
            op.IncreaseWith(op2);
            return op;
        }
        public static SmartTime operator -(SmartTime op1, SmartTime op2)
        {
            var op = new SmartTime(op1);
            op.DecreaseWith(op2);
            return op;
        }
        public static SmartTime operator *(SmartTime op1, SmartTime op2)
        {
            var op = new SmartTime(op1);
            op.Hour *= op1.Hour;
            op.Minute *= op1.Minute;
            op.Second *= op1.Second;
            return op;
        }
        public static SmartTime operator /(SmartTime op1, SmartTime op2)
        {
            var op = new SmartTime(op1);
            op.Hour /= op1.Hour;
            op.Minute /= op1.Minute;
            op.Second /= op1.Second;
            return op;
        }
        public static bool operator ==(SmartTime op1, SmartTime op2)
        {
            try
            {
                return op1.IsSame(op2);
            }
            catch { return ReferenceEquals(op1, op2); }
        }
        public static bool operator !=(SmartTime op1, SmartTime op2)
        {
            return !(op1 == op2);
        }
        public static bool operator >(SmartTime op1, SmartTime op2)
        {
            return op1.IsNext(op2);
        }
        public static bool operator <(SmartTime op1, SmartTime op2)
        {
            return op1.IsBack(op2);
        }
        public static bool operator >=(SmartTime op1, SmartTime op2)
        {
            return op1 == op2 || op1 > op2;
        }
        public static bool operator <=(SmartTime op1, SmartTime op2)
        {
            return op1 == op2 || op1 < op2;
        }
        public static SmartTime operator ~(SmartTime op1)
        {
            return new SmartTime(0, 0, 0);
        }
        #endregion

        public SmartTime(long hour = 0, long min = 0, long sec = 0)
        {
            SetTime(hour, min, sec);
        }
        public SmartTime(DateTime dt)
        {
            SetTime(dt.TimeOfDay.Hours, dt.TimeOfDay.Minutes, dt.TimeOfDay.Seconds);
        }
        public SmartTime(SmartDateTime mdt)
        {
            SetTime(mdt);
        }

        public void IncrementSecond()
        {
            Second++;
        }
        public void DecrementSecond()
        {
            Second--;
        }

        public void SetTime(long hour = 0, long min = 0, long sec = 0)
        {
            _Hour = hour;
            _Minute = min;
            _Second = sec;
        }
        public void SetTime(SmartDateTime dateTime)
        {
            SetTime( dateTime.GetHour(),dateTime.GetMinute(), dateTime.GetSecond());
        }
        public void SetTime(DateTime dateTime)
        {
            SetTime(dateTime.Hour, dateTime.Minute, dateTime.Second);
        }
        public void SetTime(string time)
        {
            SetTime(ConvertService.ToMiMFaTime(time));
        }
        public void SetTime(double date)
        {
            string str = date + "";
            if (str.Length > 6) { _Second = Convert.ToInt32(str.Substring(6)); str = str.Substring(0, 6); }
            if (str.Length > 4) { _Minute = Convert.ToInt32(str.Substring(4)); str = str.Substring(0, 4); }
            if (str.Length > 0) _Hour = Convert.ToInt32(str.Substring(0));
        }
        public void SetTime(SmartTime time)
        {
            SetTime(time.Hour, time.Minute, time.Second);
        }
        public string GetTime()
        {
            return GetTime(this) ;
        } 

        public static string GetTime(SmartTime time)
        {
            return string.Format("{0:d2}:{1:d2}:{2:d2}", time.Hour, time.Minute, time.Second);
        }
        public DateTime GetDateTime()
        {
            return GetDateTime(this);
        }
        public static DateTime GetDateTime(SmartTime time)
        {
            DateTime dt = new DateTime();
            dt = new DateTime(dt.Year, dt.Month, dt.Day, (int)time.Hour, (int)time.Minute, (int)time.Second);
            return dt;
        }
        public string GetSpecialTime()
        {
            string ti = "";
            if (_Hour != 0) ti = string.Format("{0:d2}:{1:d2}:{2:d2}", Hour, Minute, Second);
            else if (_Minute != 0) ti = string.Format("{0:d2}:{1:d2}", Minute, Second);
            else ti = string.Format("{0:d2}", Second);
            return ti;
        }
     
        #region Service

        public void CopyTo(SmartTime thisTime)
        {
            CopyTo(this, thisTime);
        }
        public static void CopyTo(SmartTime ofThisTime, SmartTime toThisTime)
        {
            toThisTime._Hour = ofThisTime._Hour;
            toThisTime._Minute = ofThisTime._Minute;
            toThisTime._Second = ofThisTime._Second;
        }

        public void IncreaseWith(params SmartTime[] withThisTimes)
        {
            Increase(this, withThisTimes);
        }
        public static void Increase(SmartTime thisTime,params SmartTime[] withThisTimes)
        {
            foreach (var item in withThisTimes)
            {
                thisTime.ULHour += item._Hour;
                thisTime.ULMinute += item._Minute;
                thisTime.ULSecond += item._Second;
            }
        }
        public void DecreaseWith(params SmartTime[] withThisTimes)
        {
            Decrease(this, withThisTimes);
        }
        public static void Decrease(SmartTime thisTime, params SmartTime[] withThisTimes)
        {
            foreach (var item in withThisTimes)
            {
                thisTime.ULHour -= item._Hour;
                thisTime.ULMinute -= item._Minute;
                thisTime.ULSecond -= item._Second;
            }
        }

        public bool Compare(SmartTime thisTime)
        {
            return Compare(this, thisTime);
        }
        public static bool Compare(SmartTime thisTime, SmartTime rhitThisTime)
        {
            if (rhitThisTime._Hour == thisTime._Hour &&
            rhitThisTime._Minute == thisTime._Minute &&
            rhitThisTime._Second == thisTime._Second)
                return true;
            return false;
        }

        public bool IsSame(SmartTime thisTime)
        {
            return IsSame(this, thisTime);
        }
        public static bool IsSame(SmartTime thisTime, SmartTime rhitthisTime)
        {
            return (rhitthisTime._Hour == thisTime._Hour &&
            rhitthisTime._Minute == thisTime._Minute &&
            rhitthisTime._Second == thisTime._Second) ;
        }
        public bool IsBetween(SmartTime fromthisTime, SmartTime tothisTime)
        {
            return IsBetween(this, fromthisTime, tothisTime);
        }
        public static bool IsBetween(SmartTime thisTime, SmartTime fromthisTime, SmartTime tothisTime)
        {
            if (IsSame(thisTime, fromthisTime)
                || IsSame(thisTime, tothisTime)
                || (IsNext(thisTime, fromthisTime)
                && IsBack(thisTime, tothisTime)))
                return true;
            return false;
        }
        public bool IsBack(SmartTime fromthisTime)
        {
            return IsBack(this, fromthisTime);
        }
        public static bool IsBack(SmartTime thisTime, SmartTime fromthisTime)
        {
            if (ReferenceEquals(thisTime, null) || ReferenceEquals(fromthisTime, null)) return false;
            if (thisTime.Hour < fromthisTime.Hour) return true;
            if (thisTime.Hour == fromthisTime.Hour && thisTime.Minute < fromthisTime.Minute) return true;
            if (thisTime.Hour == fromthisTime.Hour && thisTime.Minute == fromthisTime.Minute && thisTime._Second < fromthisTime._Second) return true;
            return false;
        }
        public bool IsNext(SmartTime fromthisTime)
        {
            return IsNext(this, fromthisTime);
        }
        public static bool IsNext(SmartTime thisTime, SmartTime fromthisTime)
        {
            if (ReferenceEquals(thisTime, null) || ReferenceEquals(fromthisTime, null)) return false;
            if (thisTime.Hour > fromthisTime.Hour) return true;
            if (thisTime.Hour == fromthisTime.Hour && thisTime.Minute > fromthisTime.Minute) return true;
            if (thisTime.Hour == fromthisTime.Hour && thisTime.Minute == fromthisTime.Minute && thisTime.Second > fromthisTime.Second) return true;
            return false;
        }


        public SmartTime GetLengthTime(SmartTime toThisTime)
        {
            return GetLengthTime(this, toThisTime);
        }
        public static SmartTime GetLengthTime(SmartTime ofThisTime, SmartTime toThisTime)
        {
            double Len = GetLengthSecond(ofThisTime, toThisTime);
            int h = (int)Len / 3600;
            int m = (int)(Len % 3600) / 60;
            int s = (int)(Len % 3600) % 60;
            SmartTime time = new SmartTime();
            time._Hour = h;
            time._Minute = m;
            time._Second = s;
            return time;
        }
        public double GetLengthHour(SmartTime toThisTime)
        {
            return GetLengthHour(this, toThisTime);
        }
        public static double GetLengthHour(SmartTime ofThisTime, SmartTime toThisTime)
        {
            return GetLengthSecond(ofThisTime, toThisTime) / 3600;
        }
        public double GetLengthMinute(SmartTime toThisTime)
        {
            return GetLengthMinute(this, toThisTime);
        }
        public static double GetLengthMinute(SmartTime ofThisTime, SmartTime toThisTime)
        {
            return GetLengthSecond(ofThisTime, toThisTime) / 60;
        }
        public double GetLengthSecond(SmartTime toThisTime)
        {
            return GetLengthSecond(this, toThisTime);
        }
        public static double GetLengthSecond(SmartTime ofThisTime, SmartTime toThisTime)
        {
            long sec = 0;
            if (ofThisTime.Hour >= toThisTime.Hour && ofThisTime.Minute >= toThisTime.Minute && ofThisTime.Second >= toThisTime.Second)
                sec += (24 - (ofThisTime.Hour - toThisTime.Hour)) * 3600;
            else sec += ((toThisTime.Hour - ofThisTime.Hour) * 3600);
            sec += ((toThisTime.Minute - ofThisTime.Minute) * 60);
            sec += toThisTime.Second - ofThisTime.Second;

            return sec;
        }

        #endregion

        #region Private Region

        public long _Hour;
        public long _Minute;
        public long _Second;

        #endregion
    }
}
