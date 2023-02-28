using MiMFa.Model;
using MiMFa.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMFa.Model
{
    [Serializable]
    public class SmartMatrix<T>
    {
        public SmartDictionary<long, SmartDictionary<long, T>> Items;
        public T this[long xIndex, long  yIndex]
        {
            get { return Items.GetOrDefault(yIndex).GetOrDefault(xIndex); }
            set
            {
                SmartDictionary<long, T> Y;
                if (Items.TryGetValue(yIndex, out Y))
                    if (DefaultValue.Equals(value))
                    {
                        Y.Remove(xIndex);
                        if (Items.DefaultValue == Y)
                            Items.Remove(yIndex);
                        else Items.AddOrSet(yIndex, Y);
                    }
                    else
                    {
                        Y.AddOrSet(xIndex, value);
                        Items.AddOrSet(yIndex, Y);
                    }
                else if (!DefaultValue.Equals(value))
                {
                    Y = new SmartDictionary<long, T>(DefaultValue);
                    Y.AddOrSet(xIndex, value);
                    Items.AddOrSet(yIndex, Y);
                }
                YCount = Math.Max(YCount, yIndex + 1);
                XCount = Math.Max(XCount, xIndex + 1);
            }
        }


        public T DefaultValue { get; private set; }
        public long YCount { get; private set; } = 0;
        public long XCount { get; private set; } = 0;
        public char MetaSign { get; set; } = '¶';
        public string SplitSign { get; set; } = "\t";
        public char[] TrimChars { get; set; } = { '\"','\'' };

        public SmartMatrix(T defaultValue) { DefaultValue = defaultValue; Clear(); }

        public override string ToString()
        {
            return string.Join(Environment.NewLine, ToLines() );
        }
        public virtual IEnumerable<string> ToLines()
        {
            for (long y = 0; y < YCount; y++)
                yield return string.Join(SplitSign,GetYValues(y));
        }
        public virtual IEnumerable<string> Get()
        {
            long x = -1;
            string ts = TrimChars.First().ToString();
            return (new string[]{ string.Join(MetaSign+"", YCount,XCount,DefaultValue,SplitSign,string.Join("",TrimChars)) }).Concat(
                    from Y in Items
                    select string.Join(SplitSign, Y.Key, string.Join(SplitSign,
                    from X in Y.Value
                    select X.Key == ++x? string.Join("", ts, X.Value, ts):
                    string.Join(SplitSign, x = X.Key, string.Join("", ts, X.Value, ts)))));
        }
        public virtual void Set(IEnumerable<string> codes, Func<string,T> converter)
        {
            bool hasHead = false;
            Clear();
            if(hasHead = codes.First().Contains(MetaSign))
            {
                string[] metas = codes.First().Split(MetaSign);
                if (metas.Length > 2) DefaultValue = converter(metas[2]);
                if (metas.Length > 3) SplitSign = metas[3];
                if (metas.Length > 4) TrimChars = metas[4].ToCharArray();
                codes = codes.Skip(1);
            }
            string[] spl = { SplitSign };
            long y = 0;
            long x = 0;
            if (hasHead)
                foreach (var item in codes)
                {
                    string[] cells = item.Split(spl, StringSplitOptions.None);
                    if (long.TryParse(cells.First(), out y))
                    {
                        bool val = false;
                        for (int i = 1; i < cells.Length; i++)
                            if (val) this[x, y] = converter(cells[i].Trim(TrimChars));
                            else if (!(val = long.TryParse(cells[i], out x)))
                                this[x = XCount, y] = converter(cells[i].Trim(TrimChars));
                    }
                    else
                    {
                        y = YCount;
                        for (int i = 0; i < cells.Length; i++)
                            this[i, y] = converter(cells[i]);
                    }
                }
            else foreach (var item in codes)
                {
                    string[] cells = item.Split(spl, StringSplitOptions.None);
                    y = YCount;
                    for (int i = 0; i < cells.Length; i++)
                        this[i, y] = converter(cells[i]);
                }
        }
        public virtual void Set(IEnumerable<string> codes, Func<string, T> converter, long yn, long xn)
        {
            bool hasHead = false;
            Clear();
            if (hasHead = codes.First().Contains(MetaSign))
            {
                string[] metas = codes.First().Split(MetaSign);
                if (metas.Length > 0) YCount = Convert.ToInt64(metas[0]);
                if (metas.Length > 1) XCount = Convert.ToInt64(metas[1]);
                if (metas.Length > 2) DefaultValue = converter(metas[2]);
                if (metas.Length > 3) SplitSign = metas[3];
                if (metas.Length > 4) TrimChars = metas[4].ToCharArray();
                codes = codes.Skip(1);
            }
            string[] spl = { SplitSign };
            long y = 0;
            long x = 0;
            long ly = -1;
            long lx = -1;
            if (hasHead)
                foreach (var item in codes)
                {
                    if (y == yn) break;
                    string[] cells = item.Split(spl, StringSplitOptions.None);
                    if (long.TryParse(cells.First(), out y))
                    {
                        ly = y;
                        bool val = false;
                        for (int i = 1; i < cells.Length; i++)
                            if (x == xn) break;
                            else if (val) this[x, y] = converter(cells[i].Trim(TrimChars));
                            else if (val = long.TryParse(cells[i], out x)) lx = x;
                            else this[lx = (x = lx + 1), y] = converter(cells[i].Trim(TrimChars));
                    }
                    else
                    {
                        ly = (y = ly + 1);
                        for (int i = 0; i < cells.Length; i++)
                            if (i == xn) break;
                            else this[i, y] = converter(cells[i]);
                    }
                }
            else
                foreach (var item in codes)
                {
                    if (y == yn) break;
                    string[] cells = item.Split(spl, StringSplitOptions.None);
                    ly = (y = ly + 1);
                    for (int i = 0; i < cells.Length; i++)
                        if (i == xn) break;
                        else this[i, y] = converter(cells[i]);
                }
        }
        private void Clear()
        {
            Items = new SmartDictionary<long, SmartDictionary<long, T>>(new SmartDictionary<long, T>(DefaultValue));
            XCount = YCount = 0;
        }


        public SmartDictionary<long, T> GetX(long xIndex)
        {
            var dic = new SmartDictionary<long, T>(DefaultValue);
            foreach (var item in Items)
                if(item.Value.ContainsKey(xIndex))
                    dic.Add(item.Key, item.Value[xIndex]);
            return dic;
        }
        public SmartDictionary<long, T> GetY(long yIndex)
        {
            return Items[yIndex];
        }
        public IEnumerable<T> GetXValues(long xIndex)
        {
            for (long i = 0; i < YCount; i++)
                yield return this[xIndex,i];
        }
        public IEnumerable<T> GetYValues(long yIndex)
        {
            for (long i = 0; i < XCount; i++)
                yield return this[i,yIndex];
        }

        public void SetX(long xIndex, SmartDictionary<long, T> X)
        {
            foreach (var item in X)
                this[xIndex,item.Key] = item.Value;
        }
        public void SetY(long yIndex, SmartDictionary<long, T> Y)
        {
            foreach (var item in Y)
                this[ item.Key,yIndex] = item.Value;
        }
        public void SetXValues(long xIndex, IEnumerable<T> vals)
        {
            long yi = 0;
            foreach (var val in vals)
                this[xIndex,yi++] = val;
        }
        public void SetYValues(long yIndex, IEnumerable<T> vals)
        {
            long xi = 0;
            foreach (var item in vals)
                this[xi++,yIndex] = item;
        }
    }
}
