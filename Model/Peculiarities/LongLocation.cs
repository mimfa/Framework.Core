using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMFa.Model
{
    [Serializable]
    public struct LongLocation
    {
        public long X;
        public long Y;
        public long Z;

        public LongLocation(long x = 0, long y = 0, long z = 0)
        {
            X = x;
            Y = y;
            Z = z;
        }
    }
}
