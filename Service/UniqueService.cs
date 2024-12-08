using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiMFa.Exclusive.Security;

namespace MiMFa.Service
{
    public class UniqueService
    {
        private static long _RandNumber = DateTime.Now.Second;
        public static long RandNumber => _RandNumber+= 1 + _RandNumber / 7;
        public static long CurrentIncrement { get; set; } = 0;
        public static long GetNextIncrement(long? increment = null)
        {
            if (increment.HasValue) CurrentIncrement = increment.Value;
            return ++CurrentIncrement;
        }
        public static string CreateNewString(int length)
        {
            Cryptography cr = new Cryptography();
            string s = (DateTime.Now.Ticks.ToString() + DateTime.Now.Millisecond)
                .Replace("10", "a")
                .Replace("11", "b")
                .Replace("12", "c")
                .Replace("13", "d")
                .Replace("14", "e")
                .Replace("15", "f");
            return (s+s+s+s).Substring((4*s.Length-1)-length);
        }
        public static string CreateNewString(string name)
        {
            name = ((name+"").GetHashCode() + DateTime.Now.Ticks.ToString() + DateTime.Now.Millisecond)
                .Replace("10", "a")
                .Replace("11", "b")
                .Replace("12", "c")
                .Replace("13", "d")
                .Replace("14", "e")
                .Replace("15", "f");
            return name;
        }
        public static double CreateNewDouble(Random random = null)
        {
            random = random??new Random();
            return Convert.ToDouble(DateTime.Now.Ticks.ToString().Substring(12) + DateTime.Now.Millisecond + "" + random.Next(1111, 9999));
        }
        public static long CreateNewLong(Random random = null)
        {
            random = random??new Random();
            string s = DateTime.Now.Ticks.ToString().Substring(8) + DateTime.Now.Millisecond;
            s = s.Substring(s.Length - 4) + random.Next(1111, 9999);
            return Convert.ToInt32(s);
        }
        public static int CreateRandom(int min = 11,int max = 99, Random random = null)
        {
            random = random??new Random();
            return random.Next(min,max);
        }
        public static bool CreateBoolean()
        {
            return RandNumber%2==0;
        }
    }
}
