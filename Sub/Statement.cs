using MiMFa.Model;
using MiMFa.Model.Structure;
using MiMFa.General;
using MiMFa.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MiMFa
{
    public static class Statement
    {
        public static bool IsDesignTime => (LicenseManager.UsageMode == LicenseUsageMode.Designtime);

        public static bool Interrupt
        {
            get
            {
                bool interrupt = _Interrupt;
                _Interrupt = false;
                return interrupt;
            }
            set
            {
                _Interrupt = value;
            }
        }

        public static bool _Interrupt = false;
        public static T Interrupting<T>(Func<long,T> action, long when = -1)
        {
            long i = 0;
            do try
                {
                    Statement.Interrupt = true;
                    return action(i++);
                }
                catch { Wait(1); }
            while (i != when);
            return default(T);
        }
        public static void Wait(int timeout = 1000)
        {
            Thread.Sleep(timeout);
        }
      
        public static IEnumerable<TOut> Loop<TIn, TOut>(IEnumerable<TIn> collection, Func<TIn,TOut> action)
        {
            foreach (var item in collection)
                yield return action(item);
        }
        public static void Loop<T>(IEnumerable<T> collection, Action<T> action)
        {
            foreach (var item in collection)
                action(item);
        }
        public static IEnumerable<TOut> Loop<TOut>(long length, Func<TOut> action)
        {
            long index = 0;
            while (index++ < length)
               yield return action();
        }
        public static void Loop(long length, Action action)
        {
            long index = 0;
            while (index++ < length)
                action();
        }
        public static void LimitedLoop(Func<bool> condition,long length,Action action)
        {
            long index = 0;
            while (index++ < length && condition())
                action();
        }
        public static IEnumerable<T> Loop<T>(int index , int length, Func<int, T> action)
        {
            while (index < length) yield return action(index++);
        }
        public static IEnumerable<T> Loop<T>(long index, long length, Func<long, T> action)
        {
            while (index < length) yield return action(index++);
        }
        public static IEnumerable<T> LimitedLoop<T>(Func<int, bool> condition, int index, int length, Func<int, T> action)
        {
            while (index < length && condition(index)) yield return action(index++);
        }
        public static IEnumerable<T> LimitedLoop<T>(Func<long, bool> condition, long index, long length, Func<long, T> action)
        {
            while (index < length && condition(index)) yield return action(index++);
        }
        public static T Recursive<T>(int indent, Func<T,T> action, T input = default)
        {
            return Recursive(v=> indent-- > 0, action,input);
        }
        public static T Recursive<T>(Func<T, bool> continueCondition, Func<T,T> action, T input = default)
        {
            if (continueCondition(input)) return Recursive(continueCondition,action, action(input));
            return input;
        }

        public static void Apply(Action action)
        {
            action();
        }
        public static void Apply<T>(Action<T> action, T arg = default(T))
        {
            action(arg);
        }
        public static F Apply<T,F>(Func<T,F> action, T arg = default(T))
        {
            return action(arg);
        }
      
        public static bool And(params bool[] args)
        {
            foreach (var item in args)
                if (!item) return false;
            return true;
        }
        public static bool Or(params bool[] args)
        {
            foreach (var item in args)
                if (item) return true;
            return false;
        }
    }
}
