using MiMFa.Model;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace MiMFa.Service
{
    public class MathService
    {
        public static bool IsEven(double x)
        {
            return (x % 2 == 0);
        }
        public static bool IsEven(short x)
        {
            return (x % 2 == 0);
        }
        public static bool IsEven(int x)
        {
            return (x % 2 == 0);
        }
        public static bool IsEven(long x)
        {
            return (x % 2 == 0);
        }
        public static bool IsEven(float x)
        {
            return (x % 2 == 0);
        }
        public static bool IsEven(decimal x)
        {
            return (x % 2 == 0);
        }
        public static bool IsOdd(short x)
        {
            return (x % 2 > 0);
        }
        public static bool IsOdd(int x)
        {
            return (x % 2 > 0);
        }
        public static bool IsOdd(long x)
        {
            return (x % 2 > 0);
        }
        public static bool IsOdd(float x)
        {
            return (x % 2 > 0);
        }
        public static bool IsOdd(double x)
        {
            return (x % 2 > 0);
        }
        public static bool IsOdd(decimal x)
        {
            return (x % 2 > 0);
        }

        public static bool IsNaN(short val)
        {
            return !(val > 0 || val < 0) && val != 0;
        }
        public static bool IsNaN(int val)
        {
            return !(val > 0 || val < 0) && val != 0;
        }
        public static bool IsNaN(long val)
        {
            return !(val > 0 || val < 0) && val != 0;
        }
        public static bool IsNaN(float val)
        {
            return !(val > 0 || val < 0) && val != 0;
        }
        public static bool IsNaN(double val)
        {
            return !(val > 0 || val < 0) && val != 0;
        }
        public static bool IsNaN(decimal val)
        {
            return !(val > 0 || val < 0) && val != 0;
        }

        public static bool IsNumber(short val)
        {
            return !IsNaN(val);
        }
        public static bool IsNumber(int val)
        {
            return !IsNaN(val);
        }
        public static bool IsNumber(long val)
        {
            return !IsNaN(val);
        }
        public static bool IsNumber(float val)
        {
            return !IsNaN(val);
        }
        public static bool IsNumber(double val)
        {
            return !IsNaN(val);
        }
        public static bool IsNumber(decimal val)
        {
            return !IsNaN(val);
        }

        public static bool IsZero(short val)
        {
            return !IsNaN(val) && val == 0;
        }
        public static bool IsZero(int val)
        {
            return !IsNaN(val) && val == 0;
        }
        public static bool IsZero(long val)
        {
            return !IsNaN(val) && val == 0;
        }
        public static bool IsZero(float val)
        {
            return !IsNaN(val) && val == 0;
        }
        public static bool IsZero(double val)
        {
            return !IsNaN(val) && val == 0;
        }
        public static bool IsZero(decimal val)
        {
            return !IsNaN(val) && val == 0;
        }

        public static bool IsSigned(short val)
        {
            return !IsNaN(val) && val != 0;
        }
        public static bool IsSigned(int val)
        {
            return !IsNaN(val) && val != 0;
        }
        public static bool IsSigned(long val)
        {
            return !IsNaN(val) && val != 0;
        }
        public static bool IsSigned(float val)
        {
            return !IsNaN(val) && val != 0;
        }
        public static bool IsSigned(double val)
        {
            return !IsNaN(val) && val != 0;
        }
        public static bool IsSigned(decimal val)
        {
            return !IsNaN(val) && val != 0;
        }

        public static bool IsUnSigned(short val)
        {
            return IsNaN(val) || val == 0;
        }
        public static bool IsUnSigned(int val)
        {
            return IsNaN(val) || val == 0;
        }
        public static bool IsUnSigned(long val)
        {
            return IsNaN(val) || val == 0;
        }
        public static bool IsUnSigned(float val)
        {
            return IsNaN(val) || val == 0;
        }
        public static bool IsUnSigned(double val)
        {
            return IsNaN(val) || val == 0;
        }
        public static bool IsUnSigned(decimal val)
        {
            return IsNaN(val) || val == 0;
        }

        public static bool IsAround(short val, short baseval, double tolerancePercent = 1d)
        {
            return Math.Abs((val - baseval) / baseval * 1d) <= Math.Abs(tolerancePercent);
        }
        public static bool IsAround(int val, int baseval, double tolerancePercent = 1d)
        {
            return Math.Abs((val - baseval) / baseval * 1d) <= Math.Abs(tolerancePercent);
        }
        public static bool IsAround(long val, long baseval, double tolerancePercent = 1d)
        {
            return Math.Abs((val - baseval) / baseval*1d) <= Math.Abs(tolerancePercent);
        }
        public static bool IsAround(float val, float baseval, double tolerancePercent = 0.01d)
        {
            return Math.Abs((val - baseval) / baseval) <= Math.Abs(tolerancePercent);
        }
        public static bool IsAround(double val, double baseval, double tolerancePercent = 0.01d)
        {
            return Math.Abs((val - baseval) / baseval) <= Math.Abs(tolerancePercent);
        }
        public static bool IsAround(decimal val, decimal baseval, decimal tolerancePercent=0.01m)
        {
            return Math.Abs((val - baseval) / baseval) <= Math.Abs(tolerancePercent);
        }

        public static bool IsBetween(short value, short min, short max)
        {
            return value > min && value < max;
        }
        public static bool IsBetween(int value, int min, int max)
        {
            return value > min && value < max;
        }
        public static bool IsBetween(long value, long min, long max)
        {
            return value > min && value < max;
        }
        public static bool IsBetween(float value, float min, float max)
        {
            return value > min && value < max;
        }
        public static bool IsBetween(double value, double min, double max)
        {
            return value > min && value < max;
        }
        public static bool IsBetween(decimal value, decimal min, decimal max)
        {
            return value > min && value < max;
        }

        public static double[][] Frequencies(params double[] arguments) => Frequencies(arguments.AsEnumerable());
        public static double[][] Frequencies(IEnumerable<double> arguments)
        {
            List<double[]> nums = new List<double[]>();
            foreach (var val in arguments)
            {
                var ind = nums.FindIndex(v => v[0] == val);
                if (ind < 0)
                    nums.Add(new double[] { val, 1 });
                else nums[ind][1] += 1;
            }
            return nums.ToArray();
        }

        public static IEnumerable<double> Sort(params double[] arguments) => Sort(arguments.AsEnumerable());
        public static IEnumerable<double> Sort(IEnumerable<double> arguments)
        {
            var res = arguments.ToList();
            res.Sort();
            return res;
        }
        public static IEnumerable<double> Distinct(params double[] arguments) => Distinct(arguments.AsEnumerable());
        public static IEnumerable<double> Distinct(IEnumerable<double> arguments)
        {
            return arguments.Distinct();
        }


        public static double Sigma(int i, int k, Func<int, double> func)
        {
            double d = 0;
            for (; i < k; i++)
                d += func(i);
            return d;
        }
        public static double Sum(params double[] arguments) => Sum(arguments.AsEnumerable());
        public static double Sum(IEnumerable<double> arguments)
        {
            return arguments.Sum();
        }
        public static double Multiple(IEnumerable<double> arguments) => Multiple(arguments.ToArray());
        public static double Multiple(params double[] arguments)
        {
            double m = arguments.Length==0?0: 1;
            foreach (var item in arguments)
                m *= item;
            return m;
        }
        public static double SafeMultiple(IEnumerable<double> arguments) => SafeMultiple(arguments.ToArray());
        public static double SafeMultiple(params double[] arguments)
        {
            var nn = arguments.Count(v => v < 0);
            var res = Multiple(arguments);
            if (nn == 0) return res;
            else return -Math.Abs(res);
        }
        public static double Mean(params double[] arguments)=> Mean(arguments.AsEnumerable());
        public static double Mean(IEnumerable<double> arguments) => arguments.Average(); 
        public static double Median(params double[] arguments) => Median(arguments.AsEnumerable());
        public static double Median(IEnumerable<double> arguments)
        {
            var arr = Sort(arguments).ToArray();
            var c = arr.Length;
            if (IsOdd(c)) return arr[c / 2];
            var cd = c / 2;
            return Mean(arr[cd - 1], arr[cd]);
        }
        public static double Mode(params double[] arguments) => Mode(arguments.AsEnumerable());
        public static double Mode(IEnumerable<double> arguments)
        {
            var modes = Modes(arguments).ToList();
            return modes.Count>0? modes.Average():0;
        }
        public static IEnumerable<double> Modes(params double[] arguments) => Distinct(arguments.AsEnumerable());
        public static IEnumerable<double> Modes(IEnumerable<double> arguments)
        {
            var arr = Frequencies(arguments);
            if (arr.Length == 0) yield break;
            var max = Maximum((from v in arr select v.Last()).ToArray());
            for (int i = 0; i < arr.Length; i++)
                if (arr[i][1] == max)
                    yield return arr[i][0];
        }
        public static IEnumerable<double> SimpleMovingAverages(int period, params double[] arguments)
        {
            var len = arguments.Length;
            var minlen = arguments.Length - period;
            for (int i = 0; i < len; i++)
                if (i < minlen)
                    yield return arguments.Skip(i).Take(period).Sum() / period;
        }
        public static IEnumerable<double> ExponentialMovingAverages(int period, double smoothing, params double[] arguments)
        {
            if (arguments.Length < 2) yield break;
            var b = smoothing / (period + 1);
            double ema = arguments[1] * b + arguments[0]*(1 - b);
            yield return ema;
            foreach (var item in ExponentialMovingAverages(period, smoothing, (new double[]{ ema }).Concat(arguments.Skip(2)).ToArray()))
                yield return item;
        }
        public static IEnumerable<double> SmoothedMovingAverages(int period, params double[] arguments)
        {
            var len = arguments.Length;
            var minlen = arguments.Length - period;
            double alpha = 1D / period;
            var sigma = Sigma(0, period, (i) => Growth(1, i * alpha));
            for (int i = 0; i < len; i++)
                if (i < minlen)
                {
                    int j = 0;
                    yield return arguments.Skip(i).Take(period).Select(v => Growth(v, j++ * alpha)).Sum() / sigma;
                }
        }
        public static IEnumerable<double> LinearWeightedMovingAverages(int period, params double[] arguments)
        {
            var len = arguments.Length;
            var minlen = arguments.Length - period;
            for (int i = 0; i < len; i += period)
                if (i < minlen)
                    yield return arguments.Skip(i).Take(period).Sum() / period;
        }
        public static IEnumerable<double> RelativeStrengthIndices(int length,params double[] arguments)
        {
            if (arguments.Length <= length) yield break;
            double gain = 0d;
            double lost = 0d;
            double preGain = 0d;
            double preLost = 0d;
            for (int i = 1; i < arguments.Length; i++)
            {
                var difference = arguments[i] - arguments[i - 1];
                if (difference > 0)
                    gain += difference;
                else
                    lost -= difference;

                if (i > length)
                {
                    preGain = (preGain * (length - 1) + gain) / length;
                    preLost = (preLost * (length - 1) + lost) / length;
                    yield return preLost == 0 ? 100D : 100.0D - (100.0D / (1D + (preGain / preLost)));
                    gain = 0d;
                    lost = 0d;
                }
            }
        }
        public static IEnumerable<double> Convergences(params double[][] arguments)
        {
            var last = 1D;
            foreach (var arg in arguments)
            {
                foreach (var item in arg)
                    last = (last + ((item-last) / item))/2; 
                yield return last;
            }
        }
        public static IEnumerable<double> Peaks(params double[] arguments)
        {
            for (int i = 1; i < arguments.Length - 1; i++)
                if (Math.Abs(arguments[i - 1]) < Math.Abs(arguments[i]) && Math.Abs(arguments[i + 1]) < Math.Abs(arguments[i]))
                    yield return arguments[i];
        }
        public static IEnumerable<double> Valleys(params double[] arguments)
        {
            for (int i = 1; i < arguments.Length - 1; i++)
                if (Math.Abs(arguments[i - 1]) > Math.Abs(arguments[i]) && Math.Abs(arguments[i + 1]) > Math.Abs(arguments[i]))
                    yield return arguments[i];
        }
        public static IEnumerable<IEnumerable<double>> CompareMatrix(double[] source, params double[] vs)
        {
            if (source.Length < vs.Length) yield break;
            for (int i = 0; i <= source.Length-vs.Length; i++)
                yield return CompareCollection(source.Skip(i).ToArray(), vs);
        }
        public static IEnumerable<double> CompareCollection(double[] source, params double[] vs)
        {
            if (source.Length < vs.Length) yield break;
            for (int i = 0; i < vs.Length; i++)
                yield return vs[i] ==0? 0 : source[i] / vs[i];
        }
        public static IEnumerable<IEnumerable<double>> DistancesClassification (params double[] collection)
        {
            List<SmartKeyValue<double, List<double>>> means = new List<SmartKeyValue<double, List<double>>>();
            if (collection.Length < 3 || collection.Min() == collection.Max()) yield return collection;
            else
            {
                collection = collection.OrderBy(v => v).ToArray();
                means.Add(new SmartKeyValue<double, List<double>>(collection.First(), new List<double>() { collection.First() }));
                means.Add(new SmartKeyValue<double, List<double>>(collection.Last(), new List<double>() { collection.Last() }));
                collection = collection.Skip(1).Take(collection.Length - 2).ToArray();
                //Separating
                foreach (var item in collection)
                {
                    int ind = means.FindIndex(v => v.Key == item);
                    if (ind < 0)
                        for (int i = 0; i < means.Count; i++)
                        {
                            if (i == means.Count - 1)
                            {
                                means[i].Value.Add(item);
                                means[i].Key = means[i].Value.Average();
                                break;
                            }
                            var dis = means[i + 1].Key - means[i].Key;
                            var th = dis / (3+Math.Abs(means[i + 1].Key + means[i].Key));
                            var nsl = means[i].Key + th;
                            var nsh = means[i + 1].Key - th;
                            if (item <= nsl)
                            {
                                means[i].Value.Add(item);
                                means[i].Key = means[i].Value.Average();
                                break;
                            }
                            else if (item <= nsh)
                            {
                                means.Insert(i + 1, new SmartKeyValue<double, List<double>>(item, new List<double>() { item }));
                                break;
                            }
                        }
                    else means[ind].Value.Add(item);
                }
                //Traction
                for (int i = 0; i < means.Count; i++)
                {
                    if (i == means.Count - 1) yield return means[i].Value;
                    else
                    {
                        List<double> disl = new List<double>();
                        for (int j = 0; j < means[i].Value.Count - 1; j++)
                            disl.Add(Math.Abs(means[i].Value[j] - means[i].Value[j + 1]));
                        List<double> dish = new List<double>();
                        for (int j = 0; j < means[i + 1].Value.Count - 1; j++)
                            dish.Add(Math.Abs(means[i + 1].Value[j] - means[i + 1].Value[j + 1]));
                        var dis = means[i + 1].Value.First() - means[i].Value.Last();

                        if ((disl.Count > 0 && disl.Max() >= dis) || (dish.Count > 0 && dish.Max() >= dis))
                        {
                            means[i].Value = means[i].Value.Concat(means[i+1].Value).ToList();
                            means[i].Key = means[i].Value.Average();
                            i++;
                        }
                        else yield return means[i].Value;
                    }
                }
            }
        }
        public static IEnumerable<double> DistanceBasedModes(params double[] collection)
        {
            var arr = (from v in DistancesClassification(collection) select v.ToList()).ToList();
            if (arr.Count < 1) yield break;
            var max = arr.Max(v => v.Count);
            foreach (var item in arr.Where(v => v.Count == max))
                yield return item.Average();
        }
        public static double DistanceBasedMode(params double[] collection) => Statement.Apply(v=>v.Count <1? collection.First() : v.Average(), DistanceBasedModes(collection).ToList());

        public static Point FrameSlice(Point frame, int length, Point minFrame = default(Point))
        {
            var x = frame.X;
            var y = frame.Y;

            if (minFrame == default(Point)) minFrame = new Point(x/10,y/10);
            
            var xLength = 1;
            var yLength = 1;
            var xSize = x / xLength;
            var ySize = y / yLength;

            while (xLength * yLength < length)
            {
                if (xSize < minFrame.X && ySize < minFrame.Y) return minFrame;
                else if (ySize < minFrame.Y)
                {
                    yLength = Math.Max(1, yLength - 1);
                    xLength = Math.Min(length, xLength + 1);
                }
                else if (xSize < minFrame.X)
                {
                    xLength = Math.Max(1, xLength - 1);
                    yLength = Math.Min(length, yLength + 1);
                }
                else if (xLength > yLength)
                {
                    yLength = Math.Min(length, yLength + 1);
                }
                else
                {
                    xLength = Math.Min(length, xLength + 1);
                }
                xSize = x / xLength;
                ySize = y / yLength;
            }
            return new Point(Math.Min(xSize, x), Math.Min(ySize, y));
        }

        public static double Percent(double value, double full)
        {
            return value/full;
        }
        public static short Range(params short[] arguments)
        {
            return (short)(Maximum(arguments) - Minimum(arguments));
        }
        public static int Range(params int[] arguments)
        {
            return Maximum(arguments) - Minimum(arguments);
        }
        public static long Range(params long[] arguments)
        {
            return Maximum(arguments) - Minimum(arguments);
        }
        public static float Range(params float[] arguments)
        {
            return Maximum(arguments) - Minimum(arguments);
        }
        public static double Range(params double[] arguments)
        {
            return Maximum(arguments) - Minimum(arguments);
        }
        public static decimal Range(params decimal[] arguments)
        {
            return Maximum(arguments) - Minimum(arguments);
        }
        public static T Minimum<T>(IEnumerable<T> arguments) => arguments.Min();
        public static short Minimum(params short[] arguments) => arguments.Min();
        public static int Minimum(params int[] arguments) => arguments.Min();
        public static long Minimum(params long[] arguments) => arguments.Min();
        public static float Minimum(params float[] arguments) => arguments.Min();
        public static double Minimum(params double[] arguments) => arguments.Min();
        public static decimal Minimum(params decimal[] arguments) => arguments.Min();
        public static T Maximum<T>(IEnumerable<T> arguments) => arguments.Max();
        public static short Maximum(params short[] arguments) => arguments.Max();
        public static int Maximum(params int[] arguments) => arguments.Max();
        public static long Maximum(params long[] arguments) => arguments.Max();
        public static float Maximum(params float[] arguments) => arguments.Max();
        public static double Maximum(params double[] arguments) => arguments.Max();
        public static decimal Maximum(params decimal[] arguments) => arguments.Max();

        public static short Factorial(short x, short decrement = 1)
        {
            if (x == 0) return 1;
            short dec = Convert.ToInt16(x -Math.Sign(x) * decrement);
            if (Math.Sign(x) == Math.Sign(dec)) 
                return Convert.ToInt16(Math.Sign(x) * Math.Abs(x * Factorial(dec, decrement)));
            else return x;
        }
        public static int Factorial(int x, int decrement = 1)
        {
            if (x == 0) return 1;
            var dec = x - Math.Sign(x) * decrement;
            if (Math.Sign(x) == Math.Sign(dec)) return Math.Sign(x) * Math.Abs(x * Factorial(dec, decrement));
            else return x;
        }
        public static long Factorial(long x, long decrement = 1)
        {
            if (x == 0) return 1;
            var dec = x - Math.Sign(x) * decrement;
            if (Math.Sign(x) == Math.Sign(dec)) return Math.Sign(x) * Math.Abs(x * Factorial(dec, decrement));
            else return x;
        }
        public static float Factorial(float x, float decrement = 1)
        {
            if (x == 0) return 1;
            var dec = x - Math.Sign(x) * decrement;
            if (Math.Sign(x) == Math.Sign(dec)) return Math.Sign(x) * Math.Abs(x * Factorial(dec, decrement));
            else return x;
        }
        public static double Factorial(double x, double decrement = 1)
        {
            if (x == 0) return 1;
            var dec = x - Math.Sign(x) * decrement;
            if (Math.Sign(x) == Math.Sign(dec)) return Math.Sign(x) * Math.Abs(x * Factorial(dec, decrement));
            else return x;
        }
        public static decimal Factorial(decimal x, decimal decrement = 1)
        {
            if (x == 0) return 1;
            var dec = x - Math.Sign(x) * decrement;
            if (Math.Sign(x) == Math.Sign(dec)) return Math.Sign(x) * Math.Abs(x * Factorial(dec, decrement));
            else return x;
        }
        public static double Fibonacci(double x)
        {
            if (x <= 1)
                return x;
            return Fibonacci(x - 1) + Fibonacci(x - 2);
        }
        public static double ModularPower(double a, int b, double n)
        {
            int c = 0;
            double d = 1;
            int k = 31;
            while ((b >> k & 1) == 0) k--;
            for (int i = k; i >= 0; i--)
            {
                c = 2 * c;
                d = (d * d) % n;
                if ((b >> i & 1) == 1)
                {
                    c = c + 1;
                    d = (d * a) % n;
                }
            }
            return d;
        }
        public static int FibonacciIteration(double fibonachiValue)
        {
            int iteration = 0;
            while (Fibonacci(iteration++) < fibonachiValue) ;
            return iteration - 1;
        }

        public static double Variance(params double[] arguments)
        {
            double mean = arguments.Average();
            double res = 0;
            foreach (double val in arguments)
                res += Math.Pow(val - mean, 2);
            return res / arguments.Length;
        }
        public static double STD(params double[] arguments)
        {
            return Math.Sqrt(Variance(arguments));
        }

        public static double Log(double x, double b = 10)
        {
            return Math.Log(x, b);
        }

        public static double Sin(double value)
        {
            return Math.Sin(value);
        }
        public static double Cos(double value)
        {
            return Math.Cos(value);
        }
        public static double Tan(double value)
        {
            return Math.Tan(value);
        }
        public static double Cot(double value)
        {
            return 1 / Tan(value);
        }
        public static double Power(double x, double y = 2)
        {
            return Math.Pow(x, y);
        }
        public static double Radical(double x, double b = 2)
        {
            return Math.Pow(x, 1 / b);
        }

        public static int Decimals(float num) => Decimals((decimal)num);
        public static int Decimals(double num) => Decimals((decimal)num);
        public static int Decimals(decimal num)
        {
            var sa = (num + "").Split(InfoService.GetFloatChar());
            return sa.Length < 2? 0:sa.Last().Length;
        }

        public static float Growth(float x, float rate = 0.01f)
        {
            return x + (x * rate);
        }
        public static double Growth(double x, double rate = 0.01)
        {
            return x + (x * rate);
        }
        public static decimal Growth(decimal x, decimal rate = 0.01m)
        {
            return x + (x * rate);
        }
        public static double Limit(double limit, double current)
        {
            return Math.Sign(current)*(limit * Math.Pow(current, 2) / (Math.Pow(current, 2)+ limit));
        }

        public static float Round(float x, int decimals)
        {
            return (float)Math.Round(x, decimals);
        }
        public static double Round(double x, int decimals)
        {
            return Math.Round(x, decimals);
        }
        public static decimal Round(decimal x, int decimals)
        {
            return (decimal)Math.Round(x, decimals);
        }
        public static float Round(float x)
        {
            string v = x + "";
            char ch = InfoService.GetFloatChar();
            if (v.Contains(ch))
                return Convert.ToSingle(v.Substring(0, v.Length - 1).Trim(ch));
            else return x;
        }
        public static double Round(double x)
        {
            string v = x + "";
            char ch = InfoService.GetFloatChar();
            if (v.Contains(ch))
                return Math.Round(x,v.Split(ch).Last().Length-1);
            //return Convert.ToDouble(v.Substring(0, v.Length - 1).Trim(ch));
            else return x;
        }
        public static decimal Round(decimal x)
        {
            string v = x + "";
            char ch = InfoService.GetFloatChar();
            if (v.Contains(ch))
                return Convert.ToDecimal(v.Substring(0, v.Length - 1).Trim(ch));
            else return x;
        }

        public static short TimeEffect(short number, DateTime fromTime, DateTime toTime, int miliseconds = 1000, short target = 0)
        {
            var d = Math.Min(1, (toTime.Ticks - fromTime.Ticks) / (10000 * miliseconds));
            var v = number - target;
            return Convert.ToInt16(target + (v - v * d));
        }
        public static int TimeEffect(int number, DateTime fromTime, DateTime toTime, int miliseconds = 1000, int target = 0)
        {
            var d = Math.Min(1, (toTime.Ticks - fromTime.Ticks) / (10000 * miliseconds));
            var v = number - target;
            return target +  Convert.ToInt32(v - v * d);
        }
        public static long TimeEffect(long number, DateTime fromTime, DateTime toTime, int miliseconds = 1000, long target = 0)
        {
            var d = Math.Min(1, (toTime.Ticks - fromTime.Ticks) / (10000l * miliseconds));
            var v = number - target;
            return target + (v - v * d);
        }
        public static float TimeEffect(float number, DateTime fromTime, DateTime toTime, int miliseconds = 1000, float target = 0)
        {
            var d = Math.Min(1, (toTime.Ticks - fromTime.Ticks) / (10000f * miliseconds));
            var v = number - target;
            return target + (v - v * d);
        }
        public static double TimeEffect(double number, DateTime fromTime, DateTime toTime, int miliseconds = 1000, double target = 0)
        {
            var d = Math.Min(1,(toTime.Ticks - fromTime.Ticks) / (10000d * miliseconds));
            var v = number - target;
            return target + (v - v * d);
        }
        public static decimal TimeEffect(decimal number, DateTime fromTime, DateTime toTime, int miliseconds = 1000, decimal target = 0)
        {
            var d = Math.Min(1, (toTime.Ticks - fromTime.Ticks) / (10000m * miliseconds));
            var v = number - target;
            return target + (v - v * d);
        }

        public static short Between(short value, short min, short max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
        public static int Between(int value, int min, int max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
        public static long Between(long value, long min, long max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
        public static float Between(float value, float min, float max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
        public static double Between(double value, double min, double max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }
        public static decimal Between(decimal value, decimal min, decimal max)
        {
            if (value < min) return min;
            if (value > max) return max;
            return value;
        }

        public static short FuzzyValue(short previousValue, short middleValue, short nextValue, double currentState = 0.5, double startState = 0, double endState = 1)
        {
            return Convert.ToInt16(FuzzyValue((double)previousValue, (double)middleValue, (double)nextValue, (double)currentState, (double)startState, (double)endState));
        }
        public static int FuzzyValue(int previousValue, int middleValue, int nextValue, double currentState = 0.5, double startState = 0, double endState = 1)
        {
            return Convert.ToInt32(FuzzyValue((double)previousValue, (double)middleValue, (double)nextValue, (double)currentState, (double)startState, (double)endState));
        }
        public static long FuzzyValue(long previousValue, long middleValue, long nextValue, double currentState = 0.5, double startState = 0, double endState = 1)
        {
            return Convert.ToInt64(FuzzyValue((double)previousValue, (double)middleValue, (double)nextValue, (double)currentState, (double)startState, (double)endState));
        }
        public static float FuzzyValue(float previousValue, float middleValue, float nextValue, double currentState = 0.5, double startState = 0, double endState = 1)
        {
            return Convert.ToSingle(FuzzyValue((double)previousValue, (double)middleValue, (double)nextValue, (double)currentState, (double)startState, (double)endState));
        }
        public static double FuzzyValue(double previousValue, double middleValue, double nextValue, double currentState = 0.5, double startState = 0, double endState=1)
        {
            var m = Mean(startState, endState);
            var min = Minimum(startState, endState);
            var max = Maximum(startState, endState);
            var minE = min + ((m - min) / 2);
            var maxE = max - ((max - m) / 2);
            var Min = Minimum(previousValue, nextValue);
            var Max = Maximum(previousValue, nextValue);

            if (currentState > maxE)
                return middleValue + ((currentState - maxE) * (Max - middleValue) / (max - maxE));
            if (currentState < minE)
                return middleValue + ((currentState - minE) * (Min - middleValue) / (min - minE));

            return middleValue;
        }
        public static decimal FuzzyValue(decimal previousValue, decimal middleValue, decimal nextValue, decimal currentState = 0.5m, decimal startState = 0, decimal endState = 1)
        {
            return Convert.ToDecimal(FuzzyValue((double)previousValue, (double)middleValue, (double)nextValue,  (double)currentState,(double)startState, (double)endState));
        }

    }
}
