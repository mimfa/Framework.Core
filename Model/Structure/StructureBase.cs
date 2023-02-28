using MiMFa.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMFa.Model.Structure
{
    [Serializable]
    public class StructureBase : StructureBase<object>
    {
        public StructureBase(object value = null) : base(value)
        {
        }
        public StructureBase(string name, object value) : base(name, value)
        {
        }
    }
    [Serializable]
    public class StructureBase<TValue>
    {
        private static long _ID = DateTime.Now.Ticks;
        public virtual string UID { get; set; }
        public virtual long ID { get; set; }
        public virtual string Name { get; set; }
        public virtual string Details { get; set; }
        public virtual TValue Value { get; set; }

        public StructureBase(string name, TValue value) : this(value)
        {
            Name = name;
            UID = CreateID(Name);
        }
        public StructureBase(TValue value = default)
        {
            ID = _ID++;
            Value = value;
            UID = ID.ToString();
        }

        public virtual StructureBase< TValue> Create() => new StructureBase< TValue>();
        public StructureBase< TValue> Clone() => Create().Cast(this);
        public virtual StructureBase<TValue> Cast(StructureBase< TValue> source) 
        {
            ID = source.ID;
            UID = source.UID;
            Name = source.Name;
            Details = source.Details;
            Value = source.Value;
            return this;
        }

        public string CreateID(string name)
        {
            return GetType().Name + "-" + (name+"").Replace(" ", "").Trim() + "-" + UID;
        }
        public override string ToString()
        {
            return string.Join(": ", Name,Value);
        }
    }
}
