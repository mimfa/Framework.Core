using MiMFa.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMFa.Model
{
    [Serializable]
    public class SmartDictionary<T, F> : Dictionary<T, F>
    {
        public F DefaultValue;
        public string MiddleSign { get; set; } = " -> ";
        public string SplitSign { get; set; } = Environment.NewLine;
        public SmartKeyValue<T,F>.DisplayMember Display { get; set; } = SmartKeyValue<T, F>.DisplayMember.Value;

        public SmartDictionary(){}
        public SmartDictionary(params T[] keys) { AddRange(from k in keys select new KeyValuePair<T, F>(k,default)); }
        public SmartDictionary(IEnumerable<T> keys) { AddRange(from k in keys select new KeyValuePair<T,F>(k,default)); }
        public SmartDictionary(Dictionary<T, F> kvps) { AddRange(kvps); }
        public SmartDictionary(IEnumerable<KeyValuePair<T, F>> kvps) { AddRange(kvps); }
        public SmartDictionary(F defaultValue)
        {
            DefaultValue = defaultValue;
        }

        public override string ToString()
        {
            switch (Display)
            {
                case SmartKeyValue < T,F > .DisplayMember.Key:
                    return CollectionService.GetAllKeysItem(this, SplitSign);
                case SmartKeyValue<T, F>.DisplayMember.Value:
                    return CollectionService.GetAllValuesItem(this, SplitSign);
                default:
                    return CollectionService.GetAllItems(this, MiddleSign, SplitSign);
            }
        }
        public virtual Dictionary<T, F> Dictionary => this;
        public virtual bool AddOrSet(T key, F value, Func<F, F, F> valueMaker, F defValue = default(F))
        {
            if (key == null) return false;
            if (ContainsKey(key))
            {
                this[key] = valueMaker(this[key], value);
                return false;
            }
            else
            {
                Add(key, valueMaker(defValue, value));
                return true;
            }
        }
        public virtual bool AddOrSet(T key, Func<F,F> valueMaker, F defValue = default(F))
        {
            if (key == null) return false;
            if (ContainsKey(key))
            { 
                this[key] = valueMaker(this[key]);
                return false;
            }
            else
            {
                Add(key, valueMaker(defValue));
                return true;
            }
        }
        public virtual bool AddOrSet(T key, F value)
        {
            if (key == null) return false;
            if (ContainsKey(key))
            {
                this[key] = value;
                return false;
            }
            else
            {
                Add(key, value);
                return true;
            }
        }
        public virtual bool TryAdd(T key, F value)
        {
            if (key == null || ContainsKey(key)) return false;
            Add(key, value);
            return true;
        }
        public virtual bool TrySet(T key, F value)
        {
            if (key == null || !ContainsKey(key)) return false;
            this[key] = value;
            return true;
        }

        public virtual F GetOrDefault(T key)
        {
            if (key != null && ContainsKey(key))
                return this[key];
            else return DefaultValue;
        }
        public virtual F GetOrDefault(T key, F defultValue)
        {
            if (key != null && ContainsKey(key))
                return this[key];
            else return defultValue;
        }
        public virtual void AddOrSet(KeyValuePair<T, F> kvp)
        {
            if (!TryAdd(kvp)) TrySet(kvp);
        }
        public virtual bool TryAdd(KeyValuePair<T, F> kvp)
        {
           return TryAdd(kvp.Key,kvp.Value);
        }
        public virtual bool TrySet(KeyValuePair<T, F> kvp)
        {
            return TrySet(kvp.Key, kvp.Value);
        }
        public virtual bool TryGet(T key,out F value)
        {
            if (key != null && ContainsKey(key))
            {
                value = this[key];
                return true;
            }
            value = default(F);
            return false;
        }
        public virtual bool TryGet(Func<KeyValuePair<T, F>,bool> func,out F value)
        {
            foreach (var item in this)
                if (func(item))
                {
                    value = item.Value;
                    return true;
                }
            value = default(F);
            return false;
        }
        public virtual bool TryGet(Func<KeyValuePair<T, F>,bool> func,out KeyValuePair<T, F> kvp)
        {
            foreach (var item in this)
                if (func(item))
                {
                    kvp = item;
                    return true;
                }
            kvp = new KeyValuePair<T, F>();
            return false;
        }

        public virtual void AddRange(SmartDictionary<T, F> dic)
        {
            foreach (var item in dic)
                Add(item.Key, item.Value);
        }
        public virtual void AddRange(params SmartKeyValue<T, F>[] kvps)
        {
            foreach (var item in kvps)
                Add(item.Key, item.Value);
        }
        public virtual void AddRange(Dictionary<T, F> dic)
        {
            foreach (var item in dic)
                Add(item.Key, item.Value);
        }
        public virtual void AddRange(params KeyValuePair<T, F>[] kvps)
        {
            foreach (var item in kvps)
                Add(item.Key, item.Value);
        }
        public virtual void AddRange(IEnumerable<SmartKeyValue<T, F>> kvps)
        {
            foreach (var item in kvps)
                Add(item.Key, item.Value);
        }
        public virtual void AddRange(IEnumerable<KeyValuePair<T, F>> kvps)
        {
            foreach (var item in kvps)
                Add(item.Key, item.Value);
        }
        public virtual void AddOrSetRange(SmartDictionary<T, F> dic)
        {
            foreach (var item in dic)
                AddOrSet(item.Key, item.Value);
        }
        public virtual void AddOrSetRange(params SmartKeyValue<T, F>[] kvps) => AddOrSetRange((kvps ?? new SmartKeyValue<T, F>[0]).ToList());
        public virtual void AddOrSetRange(IEnumerable<SmartKeyValue<T, F>> kvps)
        {
            foreach (var item in kvps)
                AddOrSet(item.Key, item.Value);
        }
        public virtual void AddOrSetRange(Dictionary<T, F> dic)
        {
            foreach (var item in dic)
                AddOrSet(item.Key, item.Value);
        }
        public virtual void AddOrSetRange(params KeyValuePair<T, F>[] kvps) => AddOrSetRange((kvps??new KeyValuePair<T, F>[0]).ToList());
        public virtual void AddOrSetRange(IEnumerable<KeyValuePair<T, F>> kvps)
        {
            foreach (var item in kvps)
                AddOrSet(item.Key, item.Value);
        }
        public virtual void TryAddRange(SmartDictionary<T, F> dic)
        {
            foreach (var item in dic)
                TryAdd(item.Key, item.Value);
        }
        public virtual void TryAddRange(params SmartKeyValue<T, F>[] kvps) => TryAddRange((kvps ?? new SmartKeyValue<T, F>[0]).ToList());
        public virtual void TryAddRange(IEnumerable<SmartKeyValue<T, F>> kvps)
        {
            foreach (var item in kvps)
                TryAdd(item.Key, item.Value);
        }
        public virtual void TryAddRange(Dictionary<T, F> dic)
        {
            foreach (var item in dic)
                TryAdd(item.Key, item.Value);
        }
        public virtual void TryAddRange(params KeyValuePair<T, F>[] kvps) => TryAddRange((kvps ?? new KeyValuePair<T, F>[0]).ToList());
        public virtual void TryAddRange(IEnumerable<KeyValuePair<T, F>> kvps)
        {
            foreach (var item in kvps)
                TryAdd(item.Key, item.Value);
        }

        public virtual void AddOrSet(SmartKeyValue<T, F> kvp)
        {
            if (!TryAdd(kvp)) TrySet(kvp);
        }
        public virtual bool TryAdd(SmartKeyValue<T, F> kvp)
        {
            return TryAdd(kvp.Key,kvp.Value);
        }
        public virtual bool TrySet(SmartKeyValue<T, F> kvp)
        {
            return TrySet(kvp.Key, kvp.Value);
        }

        public new void Add(T key, F value)
        {
            base.Add(key,value);
        }

        public virtual void Add(KeyValuePair<T, F> kvp)
        {
            Add(kvp.Key,kvp.Value);
        }

        public virtual void Add(SmartKeyValue<T, F> kvp)
        {
            Add(kvp.Key,kvp.Value);
        }

        public virtual bool ContainsValue(F value)
        {
            return this.Values.Contains(value);
        }

        public virtual bool FindKey(F value, out T key)
        {
            foreach (var item in this)
                if (item.Value.Equals(value))
                {
                    key = item.Key;
                    return true;
                }
            key = default(T);
            return false;
        }
        public virtual bool FindValue(T key, out F value)
        {
            return TryGet(key,out value);
        }

    }
}
