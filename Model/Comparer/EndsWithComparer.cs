﻿using MiMFa.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMFa.Model.Comparer
{
    public class EndsWithComparer : IEqualityComparer<string>
    {
        public bool Equals(string x, string y)
        {
            return (x + "").EndsWith(y + "");
        }

        public int GetHashCode(string obj)
        {
            return (obj + "").GetHashCode();
        }
    }
}
