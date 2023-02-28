using MiMFa.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMFa.Model.Comparer
{
    public class StartsOrEndsWithComparer : IEqualityComparer<string>
    {
        public bool Equals(string x, string y)
        {
            x = x + "";
            y = y + "";
            return x == y || x.StartsWith(y) || x.EndsWith(y);
        }

        public int GetHashCode(string obj)
        {
            return (obj + "").GetHashCode();
        }
    }
}
