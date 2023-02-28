using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMFa.Model
{
    public class SmartLongPointCollection : List<LongPoint>
    {
        public string Sign = "";

        public bool IsFull => this.Any(v => v.Y < 0 && v.X < 0);
        public bool IsEmpty => this.Count < 0;
        public bool IsMultiCells => IsFull || this.Any(v=>this.Any(w => w.Y != v.Y || w.X != v.X));
        public bool IsMultiLines => this.Any(v => v.Y < 0) || this.Any(v=>this.Any(w => w.Y != v.Y));
        public bool IsMultiWarps => this.Any(v => v.X < 0) || this.Any(v=>this.Any(w => w.X != v.X));

        public SmartLongPointCollection(string sign = "")
        {
            Sign = sign;
        }

        public bool Set(long col, long row, bool scope, bool multiple, bool alt = false)
        {
            if (!multiple && !scope && !Contains(col, row)) Clear();
            
            //if (alt && col > -1 && row > -1) col = -1;

            if (scope && Count > 0)
                if (row < 0)
                    try
                    {
                        var fp = Find(p => p.X > -1);
                        var lp = FindLast(p => p.X > -1);
                        var len = Math.Max(Math.Max(lp.X, fp.X), col);
                        var i = Math.Min(Math.Min(lp.X, fp.X), col);
                        for (; i <= len; i++)
                            Add(i, -1);
                        return true;
                    }
                    catch { }
                else if (col < 0)
                    try
                    {
                        var fp = Find(p => p.Y > -1);
                        var lp = FindLast(p => p.Y > -1);
                        var len = Math.Max(Math.Max(lp.Y, fp.Y), row);
                        var i = Math.Min(Math.Min(lp.Y, fp.Y), row);
                        for (; i <= len; i++)
                            Add(-1, i);
                        return true;
                    }
                    catch { }
                else RemoveAll(v=> v.X < 0 || v.Y < 0);
            else if(multiple && col > -1 && row > -1) RemoveAll(v=> v.X < 0 || v.Y < 0);

            if (this.Delete(col, row) < 1) 
                return Add(col, row); 

            return false;
        }
        public bool Add(long col, long row)
        {
            if (col < 0 && row < 0) Clear();
            else if (col < 0) RemoveAll(p => p.Y < 0 || p.X > -1);
            else if (row < 0) RemoveAll(p => p.X < 0 || p.Y > -1);
            RemoveAll(p=> (p.X < 0 && p.Y < 0) || (p.X == col && p.Y < 0) || (p.X < 0 && p.Y == row));
            if (!Contains(col, row))
            {
                Add(new LongPoint(col, row));
                return true;
            }
            return false;
        }
        public long Delete(long col, long row)
        {
            int num = 0;
            if (col < 0 && row < 0) Clear();
            else if (col < 0) num+= RemoveAll(p => p.Y == row);
            else if (row < 0) num += RemoveAll(p => p.X == col);
            num += RemoveAll(p=> (p.X < 0 && p.Y < 0) || (p.X == col && p.Y < 0) || (p.X < 0 && p.Y == row));
            return num += RemoveAll(p=> p.X == col && p.Y == row);
        }
        public bool Contains(long col, long row)
        {
            if (Exists(p => p.X < 0 && p.Y < 0)) return true;
            else if (Exists(p => p.X < 0 && p.Y == row)) return true;
            else if (Exists(p => p.X == col && p.Y < 0)) return true;
            return Exists(p=> p.X == col && p.Y == row);
        }

        public IEnumerable<LongPoint> GetPoints()
        {
            return from v in this where v.Y > -1 && v.X > -1 select v;
        }
        public IEnumerable<long> GetXs()
        {
            return from v in this where v.X > -1 orderby v.X select v.X;
        }
        public IEnumerable<long> GetYs()
        {
            return from v in this where v.Y > -1 orderby v.Y select v.Y;
        }
    }
}
