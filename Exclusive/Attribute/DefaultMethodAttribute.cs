using System;
using System.Reflection;

namespace MiMFa.Exclusive.Attribute
{
    /// <summary>
    /// This attribute is ways for detect main method
    /// </summary>
    public class DefaultMethodAttribute : System.Attribute
    {
        public string DefaultMethodName { get; set; }
        public MethodInfo DefaultMethod { get; set; }

        public DefaultMethodAttribute(string name)
        {
            DefaultMethodName = name;
            object obj = base.MemberwiseClone();
            Type t = obj.GetType();
            MethodInfo mi = null;
            try { mi = t.GetMethod(name); } catch { }
            if (mi != null) DefaultMethod = mi;
        }
    }
}