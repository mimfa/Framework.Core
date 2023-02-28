using MiMFa.Model.IO;
using MiMFa.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace MiMFa.Model.Structure
{
    public class FileHierarchy : FileHierarchy<object>
    {
        public FileHierarchy(string name, object value, string extension = ".mh") : base(name, value, extension)
        {
        }
        public FileHierarchy(string address) : base(address) { }
    }
    [Serializable]
    public class FileHierarchy<T> : StreamHierarchy<T>
    {
        public FileHierarchy(string name, T value, string extension = ".mh") : base(name, value, extension)
        {
        }
        public FileHierarchy(string address) : base(System.IO.Path.GetFileNameWithoutExtension(address), default, System.IO.Path.GetExtension(address))
        {
            Load(address);
        }

        public virtual Encoding Encoding { get; set; } = Encoding.UTF8;

        public override StreamHierarchy<T> Store(string path)
        {
            var sp= PathSplitter + "";
            IOService.WriteLines(path, from v in Children select string.Join(sp,v.Name, Encoding.GetString(IOService.Serialize(v.Value))), Encoding);
            return this;
        }
        public override StreamHierarchy<T> Load(string path)
        {
            if (File.Exists(path))
                try
                {
                    FileHierarchy<T> h = new FileHierarchy<T>(System.IO.Path.GetFileNameWithoutExtension(path),Value, System.IO.Path.GetExtension(path));
                    foreach (KeyValuePair<string, string> item in IOService.ReadKeyValues(path, PathSplitter + "", Encoding))
                        h.Append(item.Key,IOService.TryDeserialize<T>(Encoding.GetBytes(item.Value),default));
                    if (h != null) Cast(h, false);
                }
                catch { }
            return this;
        }

        public override StreamHierarchy<T> Collapse()
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
        public override StreamHierarchy<T> Expand(bool allchild = false)
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

        public Hierarchy<T> Update(string path)
        {
            return Update(new ChainedFile(path));
        }
    }
}
