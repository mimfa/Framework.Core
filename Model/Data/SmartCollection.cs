using MiMFa.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace MiMFa.Model
{
    [Serializable]
    public class SmartCollection<T> : ICollection<T>
    {
        public string SplitSign { get; set; } = Environment.NewLine;

        public List<T> Items { get; set; } = new List<T>();

        public T this[int index]
        {
            get { return Items[index]; }
            set { Items[index] = value; }
        }

        public int Count => Items.Count;
        public bool IsReadOnly { get; set; } = false;

        public void Add(T item)
        {
            if (IsReadOnly) { throw new Exception("This Collection is readonly"); }
            Items.Add(item);
        }
        public void Clear()
        {
            if (IsReadOnly) { throw new Exception("This Collection is readonly"); }
            Items.Clear();
        }
        public bool Contains(T item)
        {
            return Items.Contains(item);
        }
        public void CopyTo(T[] array, int arrayIndex)
        {
            if (IsReadOnly) { throw new Exception("This Collection is readonly"); }
            Items.CopyTo(array, arrayIndex);
        }
        public IEnumerator<T> GetEnumerator()
        {
            return Items.GetEnumerator();
        }
        public bool Remove(T item)
        {
            if (IsReadOnly) { throw new Exception("This Collection is readonly"); }
            return Items.Remove(item);
        }
        public override string ToString()
        {
            return CollectionService.GetAllItems(Items, SplitSign, 0);
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return Items.GetEnumerator();
        }
    }
}
