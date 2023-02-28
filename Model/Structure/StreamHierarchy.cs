using MiMFa.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace MiMFa.Model.Structure
{
    [Serializable]
    public class StreamHierarchy : StreamHierarchy<object>
    {
        public StreamHierarchy(string name, object value, string extension) : base(name, value, extension)
        {
        }
        public StreamHierarchy(object value = null, string extension = ".mh") : base(value, extension)
        {
        }
        public StreamHierarchy(string address) : base(address) { }
    }
    [Serializable]
    public class StreamHierarchy<T> : Hierarchy<T>
    {
        public StreamHierarchy(string name, T value, string extension) : base(name, value)
        {
            Extension = extension;
        }
        public StreamHierarchy(T value = default, string extension = ".mh") : base(value)
        {
            Extension = extension;
        }
        public StreamHierarchy(string address) : base(System.IO.Path.GetFileNameWithoutExtension(address),default)
        {
            Extension = System.IO.Path.GetExtension(address);
            Load(address);
        }

        public override Hierarchy<T> this[string uidOrNameOrPath]
        {
            get
            {
                if (string.IsNullOrWhiteSpace(uidOrNameOrPath)) return this;
                return IsExpand ?
                    (_Children.FirstOrDefault(v => v.Name == uidOrNameOrPath) ??
                    _Children.FirstOrDefault(v => v.UID == uidOrNameOrPath) ??
                        (
                            uidOrNameOrPath.Contains(PathSplitter) ?
                                Statement.Apply(v => v == null ? DefaultChild(uidOrNameOrPath.Split(PathSplitter)) : v[uidOrNameOrPath.Split(PathSplitter).Skip(1)],
                                    _Children.FirstOrDefault(v => uidOrNameOrPath.StartsWith(v.Name + PathSplitter))
                                )
                            : DefaultChild(uidOrNameOrPath)
                        )
                    ) : base[uidOrNameOrPath];
            }
            set
            {
                var h = this[uidOrNameOrPath];
                if (h != null) h.Cast(value);
            }
        }

        public virtual bool IsExpand => _Children != null;
        public virtual bool IsCollapse => _Children == null;

        public virtual string Address => string.Join("",Path, Extension);
        public virtual string Extension { get; set; } = ".mh";
        public override int Depth => Count == 0 || IsCollapse? 0 : (from v in _Children select v.Depth+1).Max();
        public override int Length => IsCollapse ? 1:(1 + (from v in _Children select ((StreamHierarchy<T>)v).Length).Sum());
        public override int Count => IsCollapse? 0 : _Children.Count;

        public override List<Hierarchy<T>> Children
        {
            get
            {
                if (IsCollapse) Expand();
                return _Children;
            }
            protected set
            {
                _Children = value;
                for (int i = 0; i < _Children.Count; i++)
                    _Children[i].Parent = this;
            }
        }
        [NonSerialized] protected List<Hierarchy<T>> _Children =null;
        public List<Hierarchy<T>> CachedChildren =>_Children?? new List<Hierarchy<T>>();

        public override Hierarchy<T> Clear()
        {
            if (_Children == null) _Children = new List<Hierarchy<T>>();
            else _Children.Clear();
            return this;
        }

        public virtual StreamHierarchy<T> Save() => Store().Collapse();
        public virtual StreamHierarchy<T> Store()
        {
            return Store(Address);
        }
        public virtual StreamHierarchy<T> Store(string path)
        {
            IOService.SaveSerializeFile(path, this);
            return this;
        }
        public virtual StreamHierarchy<T> Open() => Load().Expand();
        public virtual StreamHierarchy<T> Load()
        {
            return Load(Address);
        }
        public virtual StreamHierarchy<T> Load(string path)
        {
            if (File.Exists(path))
                try
                {
                    object h = null;
                    IOService.OpenDeserializeFile(path, ref h);
                    if (h != null && h is Hierarchy<T>) Cast((Hierarchy<T>)h, false);
                }
                catch { }
            return this;
        }

        public virtual StreamHierarchy<T> Collapse()
        {
            if (Count > 0)
            {
                PathService.CreateDirectory(Path);
                foreach (var item in _Children)
                    ((StreamHierarchy<T>)item).Store().Collapse();
            }
            Clear();
            _Children = null;
            return this;
        }
        public virtual StreamHierarchy<T> Expand(bool allchild = false)
        {
            Clear();
            string dir = Path;
            if (Directory.Exists(dir))
            {
                foreach (var p in Directory.GetFiles(dir, "*" + Extension, SearchOption.TopDirectoryOnly))
                    try
                    {
                        object h = null;
                        IOService.OpenDeserializeFile(p, ref h);
                        if (h != null && h is Hierarchy<T>) Append((Hierarchy<T>)h);
                    }
                    catch (Exception ex) { }
                if (allchild)
                    foreach (StreamHierarchy<T> item in Children)
                        if(item.IsCollapse) ((StreamHierarchy<T>)item).Expand(allchild);
            }
            return this;
        }
    }
}
