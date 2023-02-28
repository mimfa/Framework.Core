using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using System.Text;
using MiMFa.Model.Structure;
using MiMFa.Exclusive.DateAndTime;
using MiMFa.Model;
using MiMFa;
using MiMFa.General;

namespace MiMFa.Model
{
    [Serializable]
    public struct Percent
    {
        public double Positive => Total==0?0:Math.Abs(RealPositive * 100 / Total);
        public double None => Total == 0 ? 0 : RealNone * 100 / Total;
        public double Negative => Total == 0 ? 0 : RealNegative * 100 / Total;
        public double Both => None + Positive + -Math.Abs(Negative);
        public double Total => Math.Abs(RealPositive) + Math.Abs(RealNone) + Math.Abs(RealNegative);
        public ScoreStateMode FinalScore { get; set; }
        public double RealNone { get; private set; }
        public double RealBoth => RealNone + RealPositive + -Math.Abs(RealNegative);
        public double RealPositive { get; private set; }
        public double RealNegative { get; private set; }

        public Percent(double negative, double none, double positive, ScoreStateMode finalScore = ScoreStateMode.Positive)
        {
            RealPositive =  positive;
            RealNone = none;
            RealNegative =  negative;
            FinalScore = finalScore;
        }
        public Percent(double negative = 0, double positive = 0) : this(negative,0, positive,ScoreStateMode.Null)
        {
        }
        public Percent(Percent mp) : this(mp.Negative,mp.None,mp.Positive, mp.FinalScore)
        {
        }

        public Percent AddValue(double dec, bool isNone = false)
        {
            if (isNone) RealNone += dec;
            else if (dec > 0) RealPositive += dec;
            else if (dec < 0) RealNegative += dec;
            return this;
        }
        public Percent AddValue(Percent percent)
        {
            RealNone += percent.RealNone;
            RealPositive += percent.RealPositive;
            RealNegative += percent.RealNegative;
            return this;
        }
        public Percent RemoveValue(double dec, bool isNone = false)
        {
            if (isNone) RealNone -= dec;
            else if (dec > 0) RealPositive -= dec;
            else if (dec < 0) RealNegative -= dec;
            return this;
        }
        public Percent RemoveValue(Percent percent)
        {
            RealNone -= percent.RealNone;
            RealPositive -= percent.RealPositive;
            RealNegative -= percent.RealNegative;
            return this;
        }
        public Percent SetValue(double dec, bool isNone = false)
        {
            if (isNone) RealNone = dec;
            else if (dec > 0) RealPositive = dec;
            else if (dec < 0) RealNegative = dec;
            return this;
        }
        public Percent SetValue(Percent percent)
        {
            RealNone = percent.RealNone;
            RealPositive = percent.RealPositive;
            RealNegative = percent.RealNegative;
            return this;
        }
        public double GetValue()
        {
            return GetValue(this);
        }

        public static double GetValue(Percent th)
        {
            switch (th.FinalScore)
            {
                case ScoreStateMode.Null:
                    return th.None;
                case ScoreStateMode.Positive:
                    return th.Positive;
                case ScoreStateMode.Negative:
                    return th.Negative;
                case ScoreStateMode.Both:
                    return th.Both;
                default:
                    return th.Positive;
            }
        }
        public static Percent Max(Percent op1, Percent op2)
        {
            if(op1 > op2)  return op1;
            return op2;
        }
        public static Percent Min(Percent op1 , Percent op2)
        {
            if (op1 < op2) return op1;
            return op2;
        }

        public static implicit operator double(Percent op1)
        {
           return (GetValue(op1));
        }
        public static explicit operator Percent(double op1)
        {
            return (new Percent(0, 0, 0)).AddValue(op1, false);
        }
        public static implicit operator float(Percent op1)
        {
            return Convert.ToSingle(GetValue(op1));
        }
        public static explicit operator Percent(float op1)
        {
            return (new Percent(0, 0, 0)).AddValue(op1, false);
        }
        public static implicit operator int(Percent op1)
        {
            return Convert.ToInt32(GetValue(op1));
        }
        public static explicit operator Percent(int op1)
        {
            return (new Percent(0, 0, 0)).AddValue(op1, false);
        }
        public static implicit operator Color(Percent op1)
        {
            double d = GetValue(op1);
            if (d > 0) return Color.Honeydew;
            if (d < 0) return Color.MistyRose;
            return Color.Ivory;
        }
        public static bool operator ==(Percent op1, Percent op2)
        {
            return op1.Both == op2.Both;
        }
        public static bool operator !=(Percent op1, Percent op2)
        {
            return op1.Both != op2.Both;
        }
        public static bool operator >=(Percent op1, Percent op2)
        {
            return op1.Both >= op2.Both;
        }
        public static bool operator <=(Percent op1, Percent op2)
        {
            return op1.Both <= op2.Both;
        }
        public static bool operator >(Percent op1, Percent op2)
        {
            return op1.Both > op2.Both;
        }
        public static bool operator <(Percent op1, Percent op2)
        {
            return op1.Both < op2.Both;
        }
        public static Percent operator +(Percent op1, Percent op2)
        {
            var op = new Percent(0,0,0);
            op.AddValue(op1.RealNegative);
            op.AddValue(op1.RealPositive);
            op.AddValue(op1.RealNone, true);
            op.AddValue(op2.RealNegative);
            op.AddValue(op2.RealPositive);
            op.AddValue(op2.RealNone, true);
            return op;
        }
        public static Percent operator -(Percent op1, Percent op2)
        {
            var op = new Percent(0, 0, 0);
            op.AddValue(op1.RealNegative);
            op.AddValue(op1.RealPositive);
            op.AddValue(op1.RealNone, true);
            op.AddValue(-op2.RealNegative);
            op.AddValue(-op2.RealPositive);
            op.AddValue(-op2.RealNone, true);
            return op;
        }
        public static Percent operator /(Percent op1, Percent op2)
        {
            var op = new Percent(0, 0, 0);
            op.SetValue(op1.RealNegative / op2.RealNegative);
            op.SetValue(op1.RealPositive / op2.RealPositive);
            op.SetValue(op1.RealNone / op2.RealNone, true);
            return op;
        }
        public static Percent operator *(Percent op1, Percent op2)
        {
            var op = new Percent(0, 0, 0);
            op.SetValue(op1.RealNegative * op2.RealNegative);
            op.SetValue(op1.RealPositive * op2.RealPositive);
            op.SetValue(op1.RealNone * op2.RealNone, true);
            return op;
        }
        public static Percent operator +(Percent op1, double op2)
        {
            var op = new Percent(0, 0, 0);
            op.AddValue(op1.RealNegative);
            op.AddValue(op1.RealPositive);
            op.AddValue(op1.RealNone, true);
            op.AddValue(op2);
            return op;
        }
        public static Percent operator -(Percent op1, double op2)
        {
            var op = new Percent(0, 0, 0);
            op.AddValue(op1.RealNegative);
            op.AddValue(op1.RealPositive);
            op.AddValue(op1.RealNone, true);
            op.AddValue(-op2);
            return op;
        }
        public static Percent operator /(Percent op1, double op2)
        {
            var op = new Percent(0, 0, 0);
            op.SetValue(op1.RealNegative / op2);
            op.SetValue(op1.RealPositive / op2);
            op.SetValue(op1.RealNone / op2, true);
            return op;
        }
        public static Percent operator *(Percent op1, double op2)
        {
            var op = new Percent(0, 0, 0);
            op.SetValue(op1.RealNegative * op2);
            op.SetValue(op1.RealPositive * op2);
            op.SetValue(op1.RealNone * op2, true);
            return op;
        }

        public override string ToString()
        {
            string str = "";
            str += "Negative: " + Negative;
            str += " , None: " + None;
            str += " , Positive: " + Positive;
            return str;
        }
        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj is double) return this.Both == (double)obj;
            if (obj is Percent) return this.Negative == ((Percent)obj).Negative && this.Positive == ((Percent)obj).Positive && this.None == ((Percent)obj).None;
           return false;
        }
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }
    }
}
