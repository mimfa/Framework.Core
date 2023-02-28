using MiMFa.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMFa.Model
{
    [Serializable]
    public class SmartKeyValueList<T, F>: List<SmartKeyValue<T,F>>
    {
        public F this[T key]
        {
            get { return this.FindLast((kw) => kw.Key.Equals(key)).Value; }
            set { this[this.FindLastIndex((kw) => kw.Key.Equals(key))] = new SmartKeyValue<T, F>(key , value); }
        }
        public T[] Keys => KeyList.ToArray();
        public F[] Values => ValueList.ToArray();
        public List<T> KeyList
        {
            get
            {
                List<T> arr = new List<T>();
                foreach (var item in this)
                    arr.Add(item.Key);
                return arr;
            }
        }
        public List<F> ValueList
        {
            get
            {
                List<F> arr = new List<F>();
                foreach (var item in this)
                    arr.Add(item.Value);
                return arr;
            }
        }

        public string MiddleSign { get; set; } = " -> ";
        public string SplitSignSign { get; set; } = Environment.NewLine;
        public SmartKeyValue<T, F>.DisplayMember Display { get; set; } = SmartKeyValue<T, F>.DisplayMember.Value;

        public override string ToString()
        {
            switch (Display)
            {
                case SmartKeyValue<T, F>.DisplayMember.Key:
                    return CollectionService.GetAllKeysItem(this, SplitSignSign);
                case SmartKeyValue<T, F>.DisplayMember.Value:
                    return CollectionService.GetAllValuesItem(this, SplitSignSign);
                default:
                    return CollectionService.GetAllItems(this, MiddleSign, SplitSignSign);
            }
        }

        public virtual void AddOrSet(T key, F value)
        {
            if (!TryAdd(key, value)) TrySet(key, value);
        }
        public virtual bool TryAdd(T key, F value)
        {
            try { Add(key, value); return true; } catch { return false; }
        }
        public virtual bool TrySet(T key, F value)
        {
            try { this[key] = value; return true; } catch { return false; }
        }

        public virtual void AddOrSet(KeyValuePair<T, F> kvp)
        {
            if (!TryAdd(kvp)) TrySet(kvp);
        }
        public virtual bool TryAdd(KeyValuePair<T, F> kvp)
        {
            try { Add(kvp); return true; } catch { return false; }
        }
        public virtual bool TrySet(KeyValuePair<T, F> kvp)
        {
            try { this[kvp.Key] = kvp.Value; return true; } catch { return false; }
        }

        public virtual void AddOrSet(SmartKeyValue<T, F> kvp)
        {
            if (!TryAdd(kvp)) TrySet(kvp);
        }
        public virtual bool TryAdd(SmartKeyValue<T, F> kvp)
        {
            try { Add(kvp); return true; } catch { return false; }
        }
        public virtual bool TrySet(SmartKeyValue<T, F> kvp)
        {
            try { this[kvp.Key] = kvp.Value; return true; } catch { return false; }
        }

        public virtual void Add(T key, F value)
        {
            base.Add(new SmartKeyValue<T, F>(key, value));
        }
        public virtual void Remove(T key, F value)
        {
            base.Remove(new SmartKeyValue<T, F>(key, value));
        }
        public virtual void Remove(T key)
        {
            Remove(key, this[key]);
        }
        public virtual void Add(KeyValuePair<T, F> kvp)
        {
            Add(kvp.Key, kvp.Value);
        }

        public virtual bool Exist( T key)
        {
            return Find((a)=> a.Key.Equals(key)) != null;
        }
        public virtual bool Exist( F value)
        {
            return Find((a)=> a.Value.Equals(value)) != null;
        }
        public virtual SmartKeyValue<T, F> Find(Func<SmartKeyValue<T, F>, bool> func)
        {
            foreach (var item in this)
                if (func(item)) return item;
            return null;
        }

    }
}
