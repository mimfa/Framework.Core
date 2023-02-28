using MiMFa.Model.IO;
using MiMFa.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace MiMFa.Model.Structure
{
    public interface IHierarchy<TValue>
    {
        TValue Value { get; set; }

        Hierarchy<TValue> this[int index] { get; set; }
        Hierarchy<TValue> this[string uidOrNameOrPath] { get; set; }
        Hierarchy<TValue> this[IEnumerable<string> path] { get; set; }

        string Path { get; }
        Hierarchy<TValue> SuperParent { get; set; }
        IEnumerable<Hierarchy<TValue>> Parents { get; }
        Hierarchy<TValue> Parent { get; set; }
        Hierarchy<TValue> Previous { get; }
        Hierarchy<TValue> Next { get; }
        List<Hierarchy<TValue>> Children { get; }
    }

    [Serializable]
    public class Hierarchy : Hierarchy<object>
    {
        public Hierarchy(object value = null) : base(value)
        {
        }
        public Hierarchy(string name, object value) : base(name, value)
        {
        }
    }
    [Serializable]
    public class Hierarchy<TValue> : StructureBase<TValue>, IHierarchy< TValue>
    {
        public Hierarchy(string name, TValue value) : base(name, value)
        {
        }
        public Hierarchy(TValue value = default) : base(value)
        {
        }

        public virtual Hierarchy<TValue> this[int index]
        {
            get => 
                index < 0? (HasParent ? Statement.Apply(v=> v== null? DefaultChild(index) : v[v.Count + index], Parent[Index - 1]) : DefaultChild(index)) :
                index >= Count ? (HasParent ? Statement.Apply(v => v == null ? DefaultChild(index - Count) : v[index - Count], Parent[Index + 1]) : DefaultChild(index)) :
                Children[index];
            set
            {
                var h = this[index];
                if (h != null) h.Cast(value);
            }
        }
        public virtual Hierarchy<TValue> this[string uidOrNameOrPath]
        {
            get
            {
                if (string.IsNullOrWhiteSpace(uidOrNameOrPath)) return this;
                return
                    Children.FirstOrDefault(v => v.Name+"" == uidOrNameOrPath) ?? 
                    Children.FirstOrDefault(v => v.UID == uidOrNameOrPath) ??
                    (
                        uidOrNameOrPath.Contains(PathSplitter) ?
                            Statement.Apply(v => v == null ? DefaultChild(uidOrNameOrPath.Split(PathSplitter)) : v[uidOrNameOrPath.Split(PathSplitter).Skip(1)], Children.FirstOrDefault(v => uidOrNameOrPath.StartsWith(v.Name + PathSplitter)))
                        : DefaultChild(uidOrNameOrPath)
                    );
            }
            set
            {
                var h = this[uidOrNameOrPath];
                if (h != null) h.Cast(value);
            }
        }
        public Hierarchy<TValue> this[IEnumerable<string> path]
        {
            get
            {
                var f = path.FirstOrDefault();
                return f == null ? this : Statement.Apply(v => v == null ? DefaultChild(f) : v[path.Skip(1)], this[f]);
            }
            set
            {
                var h = this[path];
                if (h != null) h.Cast(value);
            }
        }
        public Hierarchy<TValue> this[params string[] pathParts] { get => this[pathParts.AsEnumerable()]; set => this[pathParts.AsEnumerable()] = value; }

        public virtual string Path => GetPath();
        public virtual Hierarchy<TValue> SuperParent
        {
            get
            {
                return Parents.LastOrDefault();
            }
            set
            {
                PrependToFirst(value);
            }
        }
        public virtual IEnumerable<Hierarchy<TValue>> Parents
        {
            get
            {
                if (HasParent)
                {
                    yield return Parent;
                    foreach (var item in Parent.Parents)
                        yield return item;
                }
                else yield break;
            }
        }
        public virtual Hierarchy<TValue> Parent { get; set; } = null;
        public virtual Hierarchy<TValue> Previous => HasParent ? Parent[Index - 1]: DefaultChild(Index - 1);
        public virtual Hierarchy<TValue> Next => HasParent ? Parent[Index + 1]: DefaultChild(Index + 1);
        public virtual List<Hierarchy<TValue>> Children { get; protected set; } = new List<Hierarchy<TValue>>();
        public virtual IEnumerable<Hierarchy<TValue>> SubChildren
        {
            get
            {
                if (HasChild)
                    foreach (var ch in Children)
                        foreach (var sch in ch.SubChildren)
                            yield return sch;
                else yield return this;
            }
        }

        public virtual int Index => Parent==null? 0 : Parent.Children.IndexOf(this);
        public virtual int Depth => Count == 0? 0 : (from v in Children select v.Depth+1).Max();
        public virtual int Count => Children.Count;
        public virtual int Length => 1 + (from v in Children select v.Count).Sum();

        public virtual bool IsOrphan => !HasParent && !HasChild && !HasNext && !HasPrevious;
        public virtual bool HasParent => Parent != null;
        public virtual bool HasPrevious => HasParent && Index - 1 > 0;
        public virtual bool HasNext => HasParent && Index + 1 < Parent.Children.Count;
        public virtual bool HasChild => Children.Count > 0;


        public virtual char PathSplitter { get; set; } = '\\';




        public bool IsGrandParent(Hierarchy<TValue> dynasty)
        {
            return IsGrandParent(dynasty.ID);
        }
        public bool IsGrandParent(double dynastyID)
        {
            return IsGrandParent(this, dynastyID);
        }
        public static bool IsGrandParent(Hierarchy<TValue> inThisParent, Hierarchy<TValue> thisDynasty)
        {
            return IsGrandParent(inThisParent, thisDynasty.ID);
        }
        public static bool IsGrandParent(Hierarchy<TValue> inThisParent, double thisDynastyID)
        {
            Hierarchy<TValue> dyn = inThisParent;
            while (dyn != null)
                if (dyn.ID == thisDynastyID) return true;
                else dyn = dyn.Parent;
            return false;
        }

        public string GetPath()
        {
            return string.Join(PathSplitter + "", (from v in Parents.Reverse() select v.Name).Concat(new string[] { Name}));
        }
        public string GetPath(params object[] path)
        {
            return string.Join(PathSplitter + "", (from v in Parents.Reverse() select (object)v.Name).Concat(new object[] { Name}).Concat(path));
        }

        public Hierarchy<TValue> PrependToFirst(string child) => PrependToFirst(Create(child));
        public Hierarchy<TValue> PrependToFirst(string name,TValue value) => PrependToFirst(Create(name, value));
        public Hierarchy<TValue> PrependToFirst(Hierarchy<TValue> child)
        {
            if (Parent == null)
            {
                Parent = child;
                child.Parent = null;
            }
            else Parent.PrependToFirst(child);
            return child;
        }
        public Hierarchy<TValue> Prepend(string brother) => Prepend(Create(brother));
        public Hierarchy<TValue> Prepend(string name,TValue value) => Prepend(Create(name, value));
        public Hierarchy<TValue> Prepend(Hierarchy<TValue> brother)
        {
            if (brother == null) return brother;
            return Parent.Append(brother);
        }
        public Hierarchy<TValue> Append(string child) => Append(Create(child));
        public Hierarchy<TValue> Append(string name,TValue value) => Append(Create(name, value));
        public Hierarchy<TValue> Append(Hierarchy<TValue> child)
        {
            if (child == null) return child;
            child.Parent = this;
            Children.Add(child);
            return child;
        }
        public Hierarchy< TValue> AppendToLast(string child) => AppendToLast(Create(child));
        public Hierarchy< TValue> AppendToLast(string name,TValue value) => AppendToLast(Create(name, value));
        public Hierarchy< TValue> AppendToLast(Hierarchy<TValue> child)
        {
            if (child == null) return child;
            if (Count < 1) return Append(child);
            foreach (var item in Children)
                item.AppendToLast(child);
            return child;
        }

        public virtual Hierarchy< TValue> Clear()
        {
            if (Children == null) Children = new List<Hierarchy<TValue>>();
            else Children.Clear();
            return this;
        }

        public virtual Hierarchy< TValue> DefaultChild(double index) => null;
        public virtual Hierarchy< TValue> DefaultChild(string name) => null;
        public Hierarchy< TValue> DefaultChild(IEnumerable<string> path)
        {
            if (!path.Any()) return this;
            var h = DefaultChild(path.First());
            if (h == null) return h;
            return h.DefaultChild(path.Skip(1));
        }

        public Hierarchy< TValue> Create(string name)
        {
            var h = (Hierarchy<TValue>)Create();
            h.Name = name;
            return h;
        }
        public Hierarchy< TValue> Create(string name, TValue value)
        {
            var h = (Hierarchy<TValue>)Create();
            h.Name = name;
            h.Value = value;
            return h;
        }
        public override StructureBase< TValue> Create() => new Hierarchy<TValue>(null,default) { Parent = this };
        public override StructureBase< TValue> Cast(StructureBase< TValue> source) => Cast(source,true);
        public virtual StructureBase< TValue> Cast(StructureBase< TValue> source, bool children)
        {
            if (source == null) return this;
            base.Cast(source);
            if (source is Hierarchy<TValue>)
            {
                Parent = ((Hierarchy<TValue>)source).Parent;
                PathSplitter = ((Hierarchy<TValue>)source).PathSplitter;
                if(children) Children = (from v in ((Hierarchy<TValue>)source).Children select (Hierarchy<TValue>)v.Clone()).ToList();
            }
            return this;
        }

        public Hierarchy<TValue> Update(IEnumerable<IEnumerable<string>> rows)
        {
            Clear();
            foreach (var row in rows)
                Update(row.ToArray());
            return this;
        }
        public Hierarchy<TValue> Update(string[] row)
        {
            if (row.Length < 1) return this;
            if (string.IsNullOrEmpty(row.First()))
                if (Count > 0 && row.Length > 1)
                    return Children.Last().Update(row.Skip(1).ToArray());
                else return this;
            else
            {
                Append(Create(row.First()));
                if (row.Length > 1)
                    return Update(row.Skip(1).ToArray());
                else return this;
            }
        }


        public override string ToString() => ToString("-");
        public string ToString(string tabSign)
        {
            return ToString("○", tabSign);
        }
        public string ToString(string before, string tabSign)
        {
            return string.Join("", before, Name, Environment.NewLine, Count==0?"": string.Join("", from v in Children select v.ToString(before+tabSign,tabSign)));
        }
    }
}
