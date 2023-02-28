using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMFa.Model
{
    [Serializable]
    public struct Balance
    {
        public int Right;
        public int Left;
        public int Top;
        public int Bottom;
        public int Front;
        public int Back;

        public Balance(int right=0, int left = 0, int top = 0, int bottom = 0, int front = 0, int back = 0)
        {
            Right = right;
            Left = left;
            Top = top;
            Bottom = bottom;
            Front = front;
            Back = back;
        }
    }
}
