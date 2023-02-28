using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMFa.Model
{
    public class SmartPointCollection : List<Point>
    {
        public string Sign = "";

        public SmartPointCollection(string sign = "")
        {
            Sign = sign;
        }

        public bool Add(int col, int row)
        {
            if (col < 0 && row < 0) Clear();
            else if (col < 0) RemoveAll(p => p.Y == row);
            else if (row < 0) RemoveAll(p => p.X == col);
            RemoveAll(p=> (p.X < 0 && p.Y < 0) || (p.X == col && p.Y < 0) || (p.X < 0 && p.Y == row));
            if (!Contains(col, row))
            {
                Add(new Point(col, row));
                return true;
            }
            return false;
        }
        public int Delete(int col, int row)
        {
            int num = 0;
            if (col < 0 && row < 0) Clear();
            else if (col < 0) num+= RemoveAll(p => p.Y == row);
            else if (row < 0) num += RemoveAll(p => p.X == col);
            num += RemoveAll(p=> (p.X < 0 && p.Y < 0) || (p.X == col && p.Y < 0) || (p.X < 0 && p.Y == row));
            return num += RemoveAll(p=> p.X == col && p.Y == row);
        }
        public bool Contains(int col, int row)
        {
            if (Exists(p => p.X < 0 && p.Y < 0)) return true;
            else if (Exists(p => p.X < 0 && p.Y == row)) return true;
            else if (Exists(p => p.X == col && p.Y < 0)) return true;
            return Exists(p=> p.X == col && p.Y == row);
        }

        public IEnumerable<int> GetXs()
        {
            return from v in this where v.X > -1 orderby v.X select v.X;
        }
        public IEnumerable<int> GetYs()
        {
            return from v in this where v.Y > -1 orderby v.Y select v.Y;
        }
    }
}
