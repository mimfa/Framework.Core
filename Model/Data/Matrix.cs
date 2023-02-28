using MiMFa.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMFa.Model
{
    [Serializable]
    public class Matrix<T> : SmartList<SmartList<T>>
    {
        public T this[int yIndex,int xIndex]
        {
            get { return this[Index(yIndex)][xIndex]; }
            set { this[Index(yIndex)][xIndex] = value; }
        }
        public override int Index(int yindex)
        {
            if (Count == 0) Add(new SmartList<T>());
            while (yindex >= Count) Add(new SmartList<T>());
            while (yindex < 0) yindex = Count + yindex;
            return yindex;
        }

        public string SplitLineSign { get; set; } = Environment.NewLine;

        public override string ToString()
        {
            return CollectionService.GetAllItems(this, SplitLineSign, 0);
        }
        
        public Matrix()
        {

        }
        public Matrix(int xyNumbers, T defval = default)
        {
            for (int i = 0; i < xyNumbers; i++)
            { 
                AddY();
                for (int j = 0; j < xyNumbers; j++)
                    this[i].Add(defval);
            }
        }
        public Matrix(int yNumbers,int xNumbers, T defval)
        {
            for (int i = 0; i < yNumbers; i++)
            {
                AddY();
                for (int j = 0; j < xNumbers; j++)
                    this[i].Add(defval);
            }
        }

        public override SmartList<T>[] AddArray(params SmartList<T>[] array)
        {
            AddRange(array);
            return array;
        }
        public virtual void AddY(params T[] array)
        {
            var l = new SmartList<T>();
            l.AddRange(array);
            this.Add(l);
        }
        public virtual void AddY() => this.Add(new SmartList<T>());
        public virtual void AddX(params T[] array)
        {
            for (int i = 0; i < array.Length; i++)
                if (this.Count > i) this[i].Add(array[i]);
                else AddY(array[i]);
        }
        public virtual void AddX(T defval)
        {
            for (int i = 0; i < Count; i++)
                this[i].Add(defval);
        }
        public virtual void AddYVal(params T[] values)=>this.Last().AddRange(values);
    }
}
