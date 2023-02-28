using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMFa.Model
{
    [Serializable]
    public struct LongPoint
    {
        public long X;
        public long Y;

        public bool HasNegative => X < 0 || Y < 0;
        public bool IsNegative => X < 0 && Y < 0;

        public LongPoint(long x = 0, long y = 0)
        {
            X = x;
            Y = y;
        }
    }
}
