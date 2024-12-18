﻿using MiMFa.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMFa.Model
{
    [Serializable]
    public class SmartList<T> : List<T>
    {
        public SmartList()
        {
        }
        public SmartList(int capacity) : base(capacity)
        {
        }
        public SmartList(params T[] collection) : base(collection)
        {
        }
        public SmartList(IEnumerable<T> collection) : base(collection)
        {
        }

        public new T this[int index] { get => base[Index(index)]; set => base[Index(index)] = value; }
        public virtual int Index(int index)
        {
            if (Count == 0) Add(default);
            while (index >= Count) Add(default);
            while (index < 0) index = Count + index;
            return index;
        }

        public string SplitSign { get; set; } = "\t";

        public override string ToString()
        {
            return CollectionService.GetAllItems(this, SplitSign, 0);
        }


        public virtual T[] AddRange(params T[] array)
        {
            AddRange((IEnumerable<T>)array);
            return array;
        }

        public virtual int DistinctAddRange(params T[] array)
        {
            int b = 0;
            foreach (var value in array) 
                if (DistinctAdd(value)) b++;
            return b;
        }
        public virtual bool DistinctAdd(T value)
        {
            bool b = false;
            if (b = !Contains(value)) Add(value);
            return b;
        }
    }
}
