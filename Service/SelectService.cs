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
    public class SelectService
    {
        public static string First(params string[] values)
        {
            return First(v1 => !string.IsNullOrWhiteSpace(v1), values);
        }
        public static string Last(params string[] values)
        {
            return Last(v1 => !string.IsNullOrWhiteSpace(v1), values);
        }
        public static string Between(params string[] values)
        {
            return Between((v1, v2) => !string.IsNullOrWhiteSpace(v1) && v1.GetHashCode() > (v2 ?? "").GetHashCode(), values);
        }
        public static string Best(params string[] values)
        {
            return Between((v1, v2) => !string.IsNullOrWhiteSpace(v1) && v1.GetHashCode() > (v2 ?? "").GetHashCode(), values);
        }

        public static short First(params short[] values)
        {
            return First(v1 => MathService.IsSigned(v1), values);
        }
        public static short Last(params short[] values)
        {
            return Last(v1 => MathService.IsSigned(v1), values);
        }
        public static short Between(params short[] values)
        {
            return Between((v1, v2) => MathService.IsSigned(v1) && MathService.IsSigned(v2) && v1.GetHashCode() > v2.GetHashCode(), values);
        }
        public static short Best(params short[] values)
        {
            return Best((v1, v2) => MathService.IsSigned(v1) && MathService.IsSigned(v2) && v1.GetHashCode() > v2.GetHashCode(), values);
        }

        public static int First(params int[] values)
        {
            return First(v1 => MathService.IsSigned(v1), values);
        }
        public static int Last(params int[] values)
        {
            return Last(v1 => MathService.IsSigned(v1), values);
        }
        public static int Between(params int[] values)
        {
            return Between((v1, v2) => MathService.IsSigned(v1) && MathService.IsSigned(v2) && v1.GetHashCode() > v2.GetHashCode(), values);
        }
        public static int Best(params int[] values)
        {
            return Best((v1, v2) => MathService.IsSigned(v1) && MathService.IsSigned(v2) && v1.GetHashCode() > v2.GetHashCode(), values);
        }

        public static long First(params long[] values)
        {
            return First(v1 => MathService.IsSigned(v1), values);
        }
        public static long Last(params long[] values)
        {
            return Last(v1 => MathService.IsSigned(v1), values);
        }
        public static long Between(params long[] values)
        {
            return Between((v1, v2) => MathService.IsSigned(v1) && MathService.IsSigned(v2) && v1.GetHashCode() > v2.GetHashCode(), values);
        }
        public static long Best(params long[] values)
        {
            return Best((v1, v2) => MathService.IsSigned(v1) && MathService.IsSigned(v2) && v1.GetHashCode() > v2.GetHashCode(), values);
        }

        public static float First(params float[] values)
        {
            return First(v1 => MathService.IsSigned(v1), values);
        }
        public static float Last(params float[] values)
        {
            return Last(v1 => MathService.IsSigned(v1), values);
        }
        public static float Between(params float[] values)
        {
            return Between((v1, v2) => MathService.IsSigned(v1) && MathService.IsSigned(v2) && v1.GetHashCode() > v2.GetHashCode(), values);
        }
        public static float Best(params float[] values)
        {
            return Best((v1, v2) => MathService.IsSigned(v1) && MathService.IsSigned(v2) && v1.GetHashCode() > v2.GetHashCode(), values);
        }

        public static double First(params double[] values)
        {
            return First(v1 => MathService.IsSigned(v1), values);
        }
        public static double Last(params double[] values)
        {
            return Last(v1 => MathService.IsSigned(v1), values);
        }
        public static double Between(params double[] values)
        {
            return Between((v1, v2) => MathService.IsSigned(v1) && MathService.IsSigned(v2) && v1.GetHashCode() > v2.GetHashCode(), values);
        }
        public static double Best(params double[] values)
        {
            return Best((v1, v2) => MathService.IsSigned(v1) && MathService.IsSigned(v2) && v1.GetHashCode() > v2.GetHashCode(), values);
        }

        public static decimal First(params decimal[] values)
        {
            return First(v1 => MathService.IsSigned(v1), values);
        }
        public static decimal Last(params decimal[] values)
        {
            return Last(v1 => MathService.IsSigned(v1), values);
        }
        public static decimal Between(params decimal[] values)
        {
            return Between((v1, v2) => MathService.IsSigned(v1) && MathService.IsSigned(v2) && v1.GetHashCode() > v2.GetHashCode(), values);
        }
        public static decimal Best(params decimal[] values)
        {
            return Best((v1, v2) => MathService.IsSigned(v1) && MathService.IsSigned(v2) && v1.GetHashCode() > v2.GetHashCode(), values);
        }

        public static Color First(params Color[] values)
        {
            return First(v1 => v1 != Color.Empty, values);
        }
        public static Color Last(params Color[] values)
        {
            return Last(v1 => v1 != Color.Empty, values);
        }
        public static Color Between(params Color[] values)
        {
            return Between((v1, v2) => v1 != Color.Empty && v2 != Color.Transparent && v1.GetHashCode() > v2.GetHashCode(), values);
        }
        public static Color Best(params Color[] values)
        {
            return Best((v1, v2) => v1 != Color.Empty && v2 != Color.Transparent && v1.GetHashCode() > v2.GetHashCode(), values);
        }

        public static object First(params object[] values)
        {
            return First(v1 => v1 != null, values);
        }
        public static object Last(params object[] values)
        {
            return Last(v1 => v1 != null, values);
        }
        public static object Between(params object[] values)
        {
            return Between((v1, v2) => v1 != null && v2 != null && v1.GetHashCode() > v2.GetHashCode(), values);
        }
        public static object Best(params object[] values)
        {
            return Best((v1, v2) => v1 != null && v2 != null && v1.GetHashCode() > v2.GetHashCode(), values);
        }


        public static T First<T>(Func<T, bool> condition, params T[] values)
        {
            T res = values.FirstOrDefault(condition);
            return res == null || (res.Equals(default(T)) && !condition(default(T))) ? values.Last() : res;
        }
        public static T Last<T>(Func<T, bool> condition, params T[] values)
        {
            T res = values.LastOrDefault(condition);
            return res == null || (res.Equals(default(T)) && !condition(default(T))) ? values.First() : res;
        }
        public static T Between<T>(Func<T, T, bool> condition, params T[] values)
        {
            for (int i = 1; i < values.Length; i++)
                if (condition(values[i - 1], values[i])) return values[i - 1];
            return values.Last();
        }
        public static T Best<T>(Func<T, T, bool> condition, params T[] values)
        {
            for (int i = 1; i < values.Length; i++)
                if (condition(values[i - 1], values[i]))
                    return Best(condition, new T[] { values[i - 1] }.Concat(values.Skip(i + 1)).ToArray());
            return values.Last();
        }
    }
}
