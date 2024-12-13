using MiMFa.Service;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiMFa.Engine.Web
{
    public class PointerJS : IEnumerable<PointerJS>
    {
        public object Pointer { get; set; } = null;
        public PointerMode Mode { get; set; } = PointerMode.Pure;
        public Func<string, IEnumerable<object>, object> Execute { get; set; } = (s,a) => s;
        public object Evaluate(string code) => Execute(code,new object[] { });
        public PointerJS Sequence { get; set; } = null;
        public PointerJS Source { get; set; } = null;

        public bool _Multiple { get; set; } = false;
        public bool _Returnable { get; set; } = false;
        public bool _IsWindow => Source == null || Source.Pointer != "document";
        public bool _IsDocument => Pointer == null && Source != null && Pointer == "document";

        public virtual PointerJS this[int index] { get => Get(index); set => Set(index, value).Perform(); }
        public virtual PointerJS this[string name] { get => Get(name); set => Set(name, value).Perform(); }


        public PointerJS(Func<string, IEnumerable<object>, object> executer, bool all = false, PointerJS source = null)
        {
            Execute = executer;
            _Multiple = all;
            Source = source;
            Initialize();
        }
        public PointerJS(object pointer, PointerMode mode = PointerMode.Query, bool all = false, PointerJS source = null)
        {
            Mode = mode;
            Pointer = pointer;
            _Multiple = all;
            Source = source;
            Initialize();
        }
        public PointerJS(object pointer, Func<string, IEnumerable<object>, object> executer, PointerMode mode = PointerMode.Query, bool all = false, PointerJS source = null)
        {
            Execute = executer;
            Mode = mode;
            Pointer = pointer;
            _Multiple = all;
            Source = source;
            Initialize();
        }
        public PointerJS(long x, long y, Func<string, IEnumerable<object>, object> executer, bool all = false, PointerJS source = null)
        {
            Pointer = string.Join(", ", x, y);
            Mode = PointerMode.Location;
            Execute = executer;
            _Multiple = all;
            Source = source;
            Initialize();
        }
        public PointerJS(PointerJS pointer, string script, bool? all = null) : this(pointer, all)
        {
            Mode = PointerMode.Pure;
            Pointer = script;
            Initialize();
        }
        public PointerJS(PointerJS pointer, bool? all = null) : this(pointer.Pointer, pointer.Execute, pointer.Mode, all ?? pointer._Multiple, pointer.Source)
        {
            Sequence = pointer.Sequence;
        }

        public PointerJS Clone() => new PointerJS(this);

        public PointerJS Initialize()
        {
            return this;
        }


        public string ElementPointer()
        {
            var source = Source == null?"":Source.ToScript();
            _Multiple = false;
            switch (Mode)
            {
                case PointerMode.Id:
                    return string.Join("", source, ".getElementById(", ToScript(Pointer), ")");
                case PointerMode.Name:
                    return string.Join("", source, ".getElementsByName(", ToScript(Pointer), ")[0]");
                case PointerMode.Tag:
                    return string.Join("", source, ".getElementsByTagName(", ToScript(Pointer), ")[0]");
                case PointerMode.Class:
                    return string.Join("", source, ".getElementsByClassName(", ToScript(Pointer), ")[0]");
                case PointerMode.Location:
                    return string.Join("", source, ".elementFromPoint(", Pointer, ")");
                case PointerMode.Query:
                    return string.Join("", source, ".querySelector(", ToScript(Pointer), ")");
                case PointerMode.XPath:
                    return string.Join("", source, ".evaluate(", ToScript(Pointer), ", document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue");
                case PointerMode.Pure:
                default:
                    return $"{source}{Pointer}";
            }
        }
        public string ElementsPointer()
        {
            var source = Source == null ? "" : Source.ToScript();
            _Multiple = false;
            switch (Mode)
            {
                case PointerMode.Id:
                    return string.Join("", "[", source, ".getElementById(", ToScript(Pointer), ")]");
                case PointerMode.Name:
                    return string.Join("", source, ".getElementsByName(", ToScript(Pointer), ")");
                case PointerMode.Tag:
                    return string.Join("", source, ".getElementsByTagName(", ToScript(Pointer), ")");
                case PointerMode.Class:
                    return string.Join("", source, ".getElementsByClassName(", ToScript(Pointer), ")");
                case PointerMode.Location:
                    return string.Join("", source, ".elementsFromPoint(", Pointer, ")");
                case PointerMode.Query:
                    return string.Join("", source, ".querySelectorAll(", ToScript(Pointer), ")");
                case PointerMode.XPath:
                    return string.Join("", "Array.from((function*(){ let iterator = ", source, ".evaluate(", ToScript(Pointer), ", document, null, XPathResult.UNORDERED_NODE_ITERATOR_TYPE, null); let current = iterator.iterateNext(); while(current){ yield current; current = iterator.iterateNext(); }  })())");
                default:
                    return Pointer+"";
            }
        }

        //public string RootPointerFormat(string format = "{0}", params string[] otherArgs)
        //{
        //    if (Multiple) return string.Join("",
        //        "Array.from((function*(elements) { for(let element of elements) yield (()=>",
        //            string.Format(format, new string[] { string.IsNullOrWhiteSpace(Script) ? "element" : Script }.Concat(otherArgs).ToArray()),
        //        ")()})(", ElementsPointer(), "))"
        //    );
        //    else return string.Format(format, new string[] { string.IsNullOrWhiteSpace(Script) ? ElementPointer() : Script }.Concat(otherArgs).ToArray());
        //}
        //public PointerJS Format(string format = "{0}", params string[] otherArgs) => new PointerJS(this, string.Format(format, new string[] { ToString() }.Concat(otherArgs).ToArray()));

        /// <summary>
        /// Create a JSPointer based on a string format
        /// </summary>
        /// <param name="format">{0} is the current Script</param>
        /// <param name="otherArgs">{0} and next arguments used in 'format'</param>
        /// <returns></returns>
        public PointerJS Format(string format = "{0}", params string[] otherArgs) => new PointerJS(this, string.Format(format, new string[] { ToSnippet() }.Concat(otherArgs).ToArray())) { Source = null };

        public async Task<object> PerformAsync(params object[] args) => await ProcessService.RunTask<object, object>(o => Perform(args));
        public Task PerformTask(params object[] args) => ProcessService.RunTask(() => Perform(args));
        public Thread PerformThread(params object[] args) => ProcessService.Run(() => Perform(args));
        public Form PerformDialog(string message = "Wait until finish the process...", params object[] args) => ProcessService.RunDialog(message, (o, a) => Perform(args));      
        public T TryPerform<T>(T defaultValue = default, params object[] args)
        {
            string s = ToScript();
            if (!_Returnable && !Regex.IsMatch(s, @"^\s*return\b", RegexOptions.Multiline)) s = Return().ToScript();
            var o = Execute(s, args);
            return o is T? (T)o:defaultValue;
        }
        public T Perform<T>(params object[] args) => (T)(Perform(args) ?? default(T));public void Perform(Action<object> process, params object[] args)
        {
            var o = Perform(args);
            if (o is IEnumerable<object>)
                Statement.Loop((IEnumerable<object>)o, process);
            else process(o);
        }
        public void Perform<TIn>(Action<TIn> process, params object[] args)
        {
            var o = Perform(args);
            if (o is IEnumerable<TIn>)
                Statement.Loop((IEnumerable<TIn>)o, process);
            else process((TIn)o);

        }
        public object Perform(Func<object, object> process, params object[] args)
        {
            var o = Perform(args);
            if (o is IEnumerable<object>)
                return Statement.Loop((IEnumerable<object>)o, process);
            else return process(o);
        }
        public object Perform<TIn>(Func<TIn, object> process, params object[] args)
        {
            var o = Perform(args);
            if (o is IEnumerable<TIn>)
                return Statement.Loop((IEnumerable<TIn>)o, process);
            else return process((TIn)o);
        }
        public virtual object Perform(params object[] args) => Execute(ToScript(), args);
        public virtual PointerJS PerformPointer(params object[] args)
        {
            var pName = "pointer_" + DateTime.Now.Ticks;
            var val = Return().As(pName).Perform(args);
            return new PointerJS(pName, Execute, PointerMode.Pure);
        }

        public PointerJS SelectPure(string pointer, bool all = false) => Select(pointer, PointerMode.Pure, all);
        public PointerJS SelectById(string pointer, bool all = false) => Select(pointer, PointerMode.Id, all);
        public PointerJS SelectByTag(string pointer, bool all = false) => Select(pointer, PointerMode.Tag, all);
        public PointerJS SelectByName(string pointer, bool all = false) => Select(pointer, PointerMode.Name, all);
        public PointerJS SelectByClass(string pointer, bool all = false) => Select(pointer, PointerMode.Class, all);
        public PointerJS SelectByXPath(string pointer, bool all = false) => Select(pointer, PointerMode.XPath, all);
        public PointerJS SelectByQuery(string pointer, bool all = false) => Select(pointer, PointerMode.Query, all);
        public PointerJS SelectByLocation(string pointer, bool all = false) => Select(pointer, PointerMode.Location, all);
        public PointerJS SelectByLocation(long x, long y, bool all = false) => Select(x,y, all);
        public PointerJS Select(Func<PointerJS, PointerJS> pointerCreator) => Select(pointerCreator(this));
        public PointerJS Select(string pointer, Func<string, IEnumerable<object>, object> executer, PointerMode pointerMode = PointerMode.Query, bool all = false) => Select(new PointerJS(pointer, executer, pointerMode, all, Source));
        public PointerJS Select(long x, long y, Func<string, IEnumerable<object>, object> executer, bool all = false) => Select(new PointerJS(x,y, executer, all, Source));
        public PointerJS Select(string pointer, PointerMode pointerMode = PointerMode.Query, bool all = false) => Select(new PointerJS(pointer, Execute, pointerMode, all,Source));
        public PointerJS Select(long x, long y, bool all = false) => Select(new PointerJS(x, y, Execute, all, Source));
        public PointerJS Select() => SelectPure(null);
        public virtual PointerJS Select(PointerJS pointer)
        {
            pointer.Execute = Execute ?? pointer.Execute;
            pointer.Sequence = Sequence;
            Sequence = null;
            return new PointerJS(pointer) { Source = this };
        }

        public PointerJS FromPure(string pointer, bool all = false) => From(pointer, PointerMode.Pure, all);
        public PointerJS FromById(string pointer, bool all = false) => From(pointer, PointerMode.Id, all);
        public PointerJS FromByTag(string pointer, bool all = false) => From(pointer, PointerMode.Tag, all);
        public PointerJS FromByName(string pointer, bool all = false) => From(pointer, PointerMode.Name, all);
        public PointerJS FromByClass(string pointer, bool all = false) => From(pointer, PointerMode.Class, all);
        public PointerJS FromByXPath(string pointer, bool all = false) => From(pointer, PointerMode.XPath, all);
        public PointerJS FromByQuery(string pointer, bool all = false) => From(pointer, PointerMode.Query, all);
        public PointerJS FromByLocation(string pointer, bool all = false) => From(pointer, PointerMode.Location, all);
        public PointerJS FromByLocation(long x, long y, bool all = false) => From(x, y, all);
        public PointerJS From(Func<PointerJS, PointerJS> pointerCreator) => From(pointerCreator(this));
        public PointerJS From(string pointer, Func<string, IEnumerable<object>, object> executer, PointerMode pointerMode = PointerMode.Query, bool all = false) => From(new PointerJS(pointer, executer, pointerMode, all));
        public PointerJS From(long x, long y, Func<string, IEnumerable<object>, object> executer, bool all = false) => From(new PointerJS(x, y, executer, all));
        public PointerJS From(string pointer, PointerMode pointerMode = PointerMode.Query, bool all = false) => From(new PointerJS(pointer, Execute, pointerMode, all));
        public PointerJS From(long x, long y, bool all = false) => From(new PointerJS(x, y, Execute, all));
        public PointerJS From() => FromPure(null);
        public virtual PointerJS From(PointerJS pointer)
        {
            pointer.Execute = Execute ?? pointer.Execute;
            Sequence = pointer.Sequence;
            pointer.Sequence = null;
            return new PointerJS(this) { Source = pointer };
        }

        /// <summary>
        /// Add this pointer to sequence and continue with nextPointer
        /// </summary>
        /// <param name="nextPointer">the next pointer</param>
        /// <returns>Updated Pointer</returns>
        public PointerJS Also(PointerJS nextPointer) => Also(nextPointer.ToScript());
        /// <summary>
        /// Add this pointer to sequence and continue with nextPointer
        /// </summary>
        /// <param name="nextCode">the next code to select</param>
        /// <returns>Updated Pointer</returns>
        public PointerJS Also(string nextCode) => Append(new PointerJS(nextCode, Execute, PointerMode.Pure)).Append(Source.Clone());
        /// <summary>
        /// Add this pointer to sequence and continue with a new null pointer
        /// </summary>
        /// <returns>Updated Pointer</returns>
        public PointerJS Also() => Also(";");


        /// <summary>
        /// {0}nextCode
        /// </summary>
        /// <returns>Updated Pointer</returns>
        public virtual PointerJS With(object value) => With(ToScript(value));
        /// <summary>
        /// {0}nextCode
        /// </summary>
        /// <param name="nextCode">a code snippet</param>
        /// <returns>Updated Pointer</returns>
        public virtual PointerJS With(string nextCode) => Format("{0}{1}", nextCode);
        /// <summary>
        /// {0}
        /// </summary>
        /// <param name="nextCode">a code snippet</param>
        /// <returns>Updated Pointer</returns>
        public virtual PointerJS With() => Format("{0}");
        /// <summary>
        /// {0}nextPointer
        /// </summary>
        /// <param name="nextPointer">other pointer</param>
        /// <returns>Updated Pointer</returns>
        public PointerJS With(PointerJS nextPointer) => With(nextPointer == null ? ToScript(null) : nextPointer.ToSnippet());

        /// <summary>
        /// {0}.nextCode
        /// </summary>
        /// <returns>Updated Pointer</returns>
        public virtual PointerJS On(object value) => On(ToScript(value));
        /// <summary>
        /// {0}.nextCode
        /// </summary>
        /// <param name="nextCode">a code snippet</param>
        /// <returns>Updated Pointer</returns>
        public virtual PointerJS On(string nextCode) => Format("{0}.{1}", nextCode);
        /// <summary>
        /// {0}[index[
        /// </summary>
        /// <param name="index">thw item index</param>
        /// <returns>Updated Pointer</returns>
        public virtual PointerJS On(int index) => Format("{0}[{1}]", index.ToString());
        /// <summary>
        /// {0}.
        /// </summary>
        /// <returns>Updated Pointer</returns>
        public virtual PointerJS On() => On("{0}.");
        /// <summary>
        /// {0}.nextPointer
        /// </summary>
        /// <param name="nextPointer">other pointer</param>
        /// <returns>Updated Pointer</returns>
        public PointerJS On(PointerJS nextPointer) => On(nextPointer == null ? ToScript(null) : nextPointer.ToSnippet());

        /// <summary>
        /// {0}.nextCode(args)
        /// </summary>
        /// <param name="nextCode">a code snippet</param>
        /// <param name="args">the function arguments</param>
        /// <returns>Updated Pointer</returns>
        public virtual PointerJS On(string nextCode, params object[] args) => On(nextCode, string.Join(", ", from arg in args select ToScript(arg)));
        /// <summary>
        /// {0}[index](args)
        /// </summary>
        /// <param name="index">thw item index</param>
        /// <param name="args">the function arguments</param>
        /// <returns>Updated Pointer</returns>
        public virtual PointerJS On(int index, params object[] args) => On(index, string.Join(", ", from arg in args select ToScript(arg)));
        /// <summary>
        /// {0}.nextPointer(args)
        /// </summary>
        /// <param name="nextPointer">other pointer</param>
        /// <param name="args">the function arguments</param>
        /// <returns>Updated Pointer</returns>
        public PointerJS On(PointerJS nextPointer, params object[] args) => On(nextPointer, string.Join(", ", from arg in args select ToScript(arg)));

        /// <summary>
        /// {0}.nextCode(args)
        /// </summary>
        /// <param name="nextCode">a code snippet</param>
        /// <param name="args">the function arguments</param>
        /// <returns>Updated Pointer</returns>
        public virtual PointerJS On(string nextCode, string args) => Format("{0}.{1}({2})", nextCode, args);
        /// <summary>
        /// {0}[index](args)
        /// </summary>
        /// <param name="index">thw item index</param>
        /// <param name="args">the function arguments</param>
        /// <returns>Updated Pointer</returns>
        public virtual PointerJS On(int index, string args) => Format("{0}[{1}]({2})", index.ToString(), args);
        /// <summary>
        /// {0}.nextPointer(args)
        /// </summary>
        /// <param name="nextPointer">other pointer</param>
        /// <param name="args">the function arguments</param>
        /// <returns>Updated Pointer</returns>
        public PointerJS On(PointerJS nextPointer, string args) => On(nextPointer == null ? ToScript(null) : nextPointer.ToSnippet(), args);

        /// <summary>
        /// {0}.nextCode(args)
        /// </summary>
        /// <param name="nextCode">a code snippet</param>
        /// <param name="args">the function arguments</param>
        /// <returns>Updated Pointer</returns>
        public virtual PointerJS On(string nextCode, PointerJS args) => On(nextCode, args.ToSnippet());
        /// <summary>
        /// {0}[index](args)
        /// </summary>
        /// <param name="index">thw item index</param>
        /// <param name="args">the function arguments</param>
        /// <returns>Updated Pointer</returns>
        public virtual PointerJS On(int index, PointerJS args) => On(index.ToString(), args.ToSnippet());
        /// <summary>
        /// {0}.nextPointer(args)
        /// </summary>
        /// <param name="nextPointer">other pointer</param>
        /// <param name="args">the function arguments</param>
        /// <returns>Updated Pointer</returns>
        public PointerJS On(PointerJS nextPointer, PointerJS args) => On(nextPointer == null ? ToScript(null) : nextPointer.ToSnippet(), args.ToSnippet());

        /// <summary>
        /// {0}\r\ncode
        /// </summary>
        /// <returns>Updated Pointer</returns>
        public virtual PointerJS Then(object value) => Then(ToScript(value));
        /// <summary>
        /// {0}\r\ncode
        /// </summary>
        /// <param name="code">a code snippet</param>
        /// <returns>Updated Pointer</returns>
        public virtual PointerJS Then(string code) => Format("{0}\r\n(()=>{{{1}}})()", code);
        /// <summary>
        /// {0}\r\n
        /// </summary>
        /// <returns>Updated Pointer</returns>
        public virtual PointerJS Then() => Format("{0}\r\n");
        /// <summary>
        /// {0}\r\npointer
        /// </summary>
        /// <param name="pointer">other pointer</param>
        /// <returns>Updated Pointer</returns>
        public PointerJS Then(PointerJS pointer) => Then(pointer == null ? ToScript(null) : pointer.ToSnippet());

        /// <summary>
        /// {0};\r\nnextCode
        /// </summary>
        /// <returns>Updated Pointer</returns>
        public virtual PointerJS Follows(object value) => Follows(ToScript(value));
        /// <summary>
        /// {0};\r\nnextCode
        /// </summary>
        /// <param name="nextCode">the next code to select</param>
        /// <returns>Updated Pointer</returns>
        public virtual PointerJS Follows(string nextCode) => Format("{0};\r\n{1}", nextCode);
        /// <summary>
        /// {0};\r\n
        /// </summary>
        /// <returns>Updated Pointer</returns>
        public PointerJS Follows() => Follows("");
        /// <summary>
        /// {0};\r\nextPointer
        /// </summary>
        /// <param name="nextPointer">the next pointer</param>
        /// <returns>Updated Pointer</returns>
        public PointerJS Follows(PointerJS nextPointer) => Follows(nextPointer == null ? ToScript(null) : nextPointer.ToSnippet());

        public virtual PointerJS Prepend(object value) => Prepend(ToScript(value));
        public virtual PointerJS Prepend(string code) => Prepend(new PointerJS(code, Execute, PointerMode.Pure));
        public PointerJS Prepend(PointerJS pointer) 
        {
            if (Sequence == null) Sequence = pointer;
            else Sequence.Prepend(pointer);
            return this;
        }

        public virtual PointerJS Append(object value) => Append(ToScript(value));
        public virtual PointerJS Append(string code) => Append(new PointerJS(code, Execute, PointerMode.Pure));
        public PointerJS Append(PointerJS pointer)
        {
            if (pointer.Sequence == null) pointer.Sequence = this;
            else pointer.Sequence.Append(this);
            return pointer;
        }

        public virtual PointerJS A() => new PointerJS(this);
        public virtual PointerJS One() => new PointerJS(this, false);
        public virtual PointerJS All() => new PointerJS(this, true);
        public virtual PointerJS The() => One();
        public virtual PointerJS The(string name) => All().Format("{0}['{1}']", name);
        public virtual PointerJS The(int index) => All().Format("{0}[{1}]", index.ToString());
        public virtual PointerJS First() => All().With("[0]");
        public virtual PointerJS Last() => All().With(".slice(-1).pop()");

        public virtual PointerJS Reverse() => With(".reverse()");
        public virtual PointerJS Slice(int index = 0, int? length = null) => With($".slice({index}" + (length == null ? ")" : $", {length})"));

        public virtual PointerJS Join(object value) => Join(ToScript(value));
        public virtual PointerJS Join(string code) => Format("{0},{1}", code);
        public virtual PointerJS Join() => Format("{0},");
        public PointerJS Join(PointerJS pointer) => Join(pointer == null ? ToScript(null) : pointer.ToSnippet());

        public virtual PointerJS Join(string name,object value) => Join(name, ToScript(value));
        public virtual PointerJS Join(string name,string code) => Format("{0},{1}:{2}", ToScript(name), code);
        public PointerJS Join(string name, PointerJS pointer) => Join(name, pointer == null ? ToScript(null) : pointer.ToSnippet());

        public PointerJS Collect() => Format("{{0}}");
        public PointerJS Array() => Format("[{0}]");

        /// <summary>
        /// There should be a yield code in the Script
        /// </summary>
        /// <returns></returns>
        public virtual PointerJS Iterate() => Format("Array.from((function*(){{{0}}})())");

        /// <summary>
        /// {0}; yield code
        /// </summary>
        /// <returns>Updated Pointer</returns>
        public virtual PointerJS Yield(object value) => Yield(ToScript(value));
        /// <summary>
        /// {0}; yield code
        /// </summary>
        /// <param name="code">a code snippet</param>
        /// <returns>Updated Pointer</returns>
        public virtual PointerJS Yield(string code) => Format("{0}\r\nyield {1}", code);
        /// <summary>
        /// yield {0}
        /// </summary>
        /// <returns>Updated Pointer</returns>
        public virtual PointerJS Yield() => Format("\r\nyield {0}");
        /// <summary>
        /// {0}; yield pointer
        /// </summary>
        /// <param name="pointer">other pointer</param>
        /// <returns>Updated Pointer</returns>
        public PointerJS Yield(PointerJS pointer) => Yield(pointer == null ? ToScript(null) : pointer.ToSnippet());

        /// <summary>
        /// {0}; return code
        /// </summary>
        /// <returns>Updated Pointer</returns>
        public virtual PointerJS Return(object value) => Return(ToScript(value));
        /// <summary>
        /// {0}; return code
        /// </summary>
        /// <param name="code">a code snippet</param>
        /// <returns>Updated Pointer</returns>
        public virtual PointerJS Return(string code) => Format("{0}\r\nreturn {1}", code);
        /// <summary>
        /// return {0}
        /// </summary>
        /// <returns>Updated Pointer</returns>
        public virtual PointerJS Return() => Format("\r\nreturn {0}");
        /// <summary>
        /// {0}; return pointer
        /// </summary>
        /// <param name="pointer">other pointer</param>
        /// <returns>Updated Pointer</returns>
        public PointerJS Return(PointerJS pointer) => Return(pointer == null ? ToScript(null) : pointer.ToSnippet());

        public virtual PointerJS If(object condition) => If(ToScript(condition));
        public virtual PointerJS If(string conditionCode) => Format("\r\nif({1}) ", conditionCode).Then(this);
        public virtual PointerJS If() => Format("\r\nif({0}) ");
        public PointerJS If(PointerJS pointer) => If(pointer == null ? ToScript(null) : pointer.ToSnippet());
        public virtual PointerJS Else(object value) => Else().Then(value);
        public virtual PointerJS Else(string code) => Else().Then(code);
        public virtual PointerJS Else() => Format("{0}\r\nelse ");
        public PointerJS Else(PointerJS pointer) => Else(pointer == null ? ToScript(null) : pointer.ToSnippet());

        public virtual PointerJS Where(object condition) => Where(ToScript(condition));
        public virtual PointerJS Where(string conditionCode) => Format("({1})? ", conditionCode).With(this);
        public virtual PointerJS Where() => Format("({0})? ");
        public PointerJS Where(PointerJS pointer) => Where(pointer == null ? ToScript(null) : pointer.ToSnippet());
        public virtual PointerJS ElseWhere(object value) => ElseWhere(ToScript(value));
        public virtual PointerJS ElseWhere(string code) => ElseWhere().With(code);
        public virtual PointerJS ElseWhere() => Format("{0} : ");
        public PointerJS ElseWhere(PointerJS pointer) => ElseWhere(pointer==null?ToScript(null):pointer.ToSnippet());

        public virtual PointerJS While(object condition) => While(ToScript(condition));
        public virtual PointerJS While(string conditionCode) => Format("\r\nwhile({1}) ", conditionCode).Then(this);
        public virtual PointerJS While() => Format("\r\nwhile({0}) ");
        public PointerJS While(PointerJS pointer) => While(pointer == null ? ToScript(null) : pointer.ToSnippet());

        public virtual PointerJS When(object condition) => When(ToScript(condition));
        public virtual PointerJS When(string conditionCode) => Format("\r\ndo {{0}}\r\nwhile({1});", conditionCode);
        public PointerJS When(PointerJS pointer) => When(pointer == null ? ToScript(null) : pointer.ToSnippet());

        public virtual PointerJS ForEach(string elementName, object collection) => ForEach(elementName, ToScript(collection));
        public virtual PointerJS ForEach(string elementName, string collectionCode) => Format("\r\nfor(let {1} of {2}) ", elementName, collectionCode).Then(this);
        public virtual PointerJS ForEach(string elementName) => Format("\r\nfor(let {1} of {0}) ", elementName);
        public virtual PointerJS ForEach() => Format("\r\nfor(let {1} of {0}) {1}", "element");
        public PointerJS ForEach(string elementName, PointerJS pointer) => ForEach(elementName, pointer == null ? ToScript(null) : pointer.ToSnippet());
        
        public virtual PointerJS ForIn(string elementName, object collection) => ForIn(elementName, ToScript(collection));
        public virtual PointerJS ForIn(string elementName, string collectionCode) => Format("\r\nfor(let {1} in {2}) ", elementName, collectionCode).Then(this);
        public virtual PointerJS ForIn(string elementName) => Format("\r\nfor(let {1} in {0}) ", elementName);
        public virtual PointerJS ForIn() => Format("\r\nfor(let {1} in {0}) {1}", "element");
        public PointerJS ForIn(string elementName, PointerJS pointer) => ForIn(elementName, pointer == null ? ToScript(null) : pointer.ToSnippet());

        public virtual PointerJS As(string elementName,object value) => As(elementName, ToScript(value));
        public virtual PointerJS As(string elementName, string code) => Format("(({1})=>{2})({0})", elementName, code);
        public virtual PointerJS As(string elementName) => Format("{1} = (()=>{{{0}}})()", elementName);
        public PointerJS As(string elementName, PointerJS nextPointer) => As(elementName, nextPointer == null ? ToScript(null) : nextPointer.ToSnippet());
       
        public virtual PointerJS Var(string elementName) => new PointerJS(Format(";\r\nvar {1}", elementName)) { Source = this};
        public virtual PointerJS Let(string elementName) => new PointerJS(Format(";\r\nlet {1}", elementName)) { Source = this };
        public virtual PointerJS Const(string elementName) => new PointerJS(Format(";\r\nconst {1}", elementName)) { Source = this };
        public virtual PointerJS Named(string elementName) => Format("{1}:{0}", elementName);

        public virtual PointerJS Equal(object value) => Format("{0}={1}", ToScript(value));
        public virtual PointerJS Equal(string code) => Format("{0}={1}", code);
        public virtual PointerJS Equal() => Format("{0}=");
        public virtual PointerJS Equal(PointerJS pointer) => Equal(pointer == null ? ToScript(null) : pointer.ToSnippet());

        public virtual PointerJS Minus(object value) => Minus(ToScript(value));
        public virtual PointerJS Minus(string code) => Format("{0}-{1}", code);
        public virtual PointerJS Minus() => Format("{0}-");
        public virtual PointerJS Minus(PointerJS pointer) => Minus(pointer == null ? ToScript(null) : pointer.ToSnippet());

        public virtual PointerJS Plus(object value) => Plus(ToScript(value));
        public virtual PointerJS Plus(string code) => Format("{0}+{1}", code);
        public virtual PointerJS Plus() => Format("{0}+");
        public virtual PointerJS Plus(PointerJS pointer) => Plus(pointer == null ? ToScript(null) : pointer.ToSnippet());

        public virtual PointerJS Multiple(object value) => Multiple(ToScript(value));
        public virtual PointerJS Multiple(string code) => Format("{0}*{1}", code);
        public virtual PointerJS Multiple() => Format("{0}*");
        public virtual PointerJS Multiple(PointerJS pointer) => Multiple(pointer == null ? ToScript(null) : pointer.ToSnippet());

        public virtual PointerJS Divide(object value) => Divide(ToScript(value));
        public virtual PointerJS Divide(string code) => Format("{0}/{1}", code);
        public virtual PointerJS Divide() => Format("{0}/");
        public virtual PointerJS Divide(PointerJS pointer) => Divide(pointer == null ? ToScript(null) : pointer.ToSnippet());

        public virtual PointerJS Power(object value) => Power(ToScript(value));
        public virtual PointerJS Power(string code) => Format("{0}**{1}", code);
        public virtual PointerJS Power() => Format("{0}**");
        public virtual PointerJS Power(PointerJS pointer) => Power(pointer == null ? ToScript(null) : pointer.ToSnippet());

        public virtual PointerJS And(object value) => And(ToScript(value));
        public virtual PointerJS And(string code = "true") => Format("({0} && {1})", code);
        public PointerJS And(PointerJS pointer) => And(pointer == null ? ToScript(null) : pointer.ToSnippet());

        public virtual PointerJS Or(object value) => Or(ToScript(value));
        public virtual PointerJS Or(string code = "true") => Format("({0} || {1})", code);
        public PointerJS Or(PointerJS pointer) => Or(pointer == null ? ToScript(null) : pointer.ToSnippet());

        public virtual PointerJS Null() => Format("{0} null");
        public virtual PointerJS Nothing() => Format("{0} (()=>{{}})()");

        /// <summary>
        /// {0}!=code
        /// </summary>
        /// <returns>Updated Pointer</returns>
        public virtual PointerJS Not(object value) => Not(ToScript(value));
        /// <summary>
        /// {0}!=code
        /// </summary>
        /// <returns>Updated Pointer</returns>
        public virtual PointerJS Not(string code) => Format("({0}!={1})", code);
        /// <summary>
        /// (!{0})
        /// </summary>
        /// <returns>Updated Pointer</returns>
        public virtual PointerJS Not() => Format("(!{0})");
        /// <summary>
        /// {0}!=pointer
        /// </summary>
        /// <returns>Updated Pointer</returns>
        public PointerJS Not(PointerJS pointer) => Not(pointer == null ? ToScript(null) : pointer.ToSnippet());

        /// <summary>
        /// {0} === code
        /// </summary>
        /// <returns>Updated Pointer</returns>
        public virtual PointerJS Is(object value) => Is(ToScript(value));
        /// <summary>
        /// {0} === code
        /// </summary>
        /// <param name="code">a code snippet</param>
        /// <returns>Updated Pointer</returns>
        public virtual PointerJS Is(string code) => Format("{0}=={1}", code);
        /// <summary>
        /// {0}==
        /// </summary>
        /// <returns>Updated Pointer</returns>
        public virtual PointerJS Is() => Format("{0}==");
        /// <summary>
        /// {0} === pointer
        /// </summary>
        /// <param name="pointer">other pointer</param>
        /// <returns>Updated Pointer</returns>
        public PointerJS Is(PointerJS pointer) => Is(pointer == null ? ToScript(null) : pointer.ToSnippet());

        /// <summary>
        /// ({0}===code)
        /// </summary>
        /// <returns>Updated Pointer</returns>
        public virtual PointerJS IsEqual(object value) => IsEqual(ToScript(value));
        /// <summary>
        /// ({0}===code)
        /// </summary>
        /// <param name="code">a code snippet</param>
        /// <returns>Updated Pointer</returns>
        public virtual PointerJS IsEqual(string code) => Format("({0}==={1})", code);
        /// <summary>
        /// {0}===
        /// </summary>
        /// <returns>Updated Pointer</returns>
        public virtual PointerJS IsEqual() => Format("{0}===");
        /// <summary>
        /// ({0}===pointer)
        /// </summary>
        /// <param name="pointer">other pointer</param>
        /// <returns>Updated Pointer</returns>
        public PointerJS IsEqual(PointerJS pointer) => IsEqual(pointer == null ? ToScript(null) : pointer.ToSnippet());

        public virtual PointerJS IsVisible() => IsHidden().Not();
        public virtual PointerJS IsHidden() => As("element", "element === null || element === undefined || element.offsetLeft < 0").Or(GetStyle().As("element","element.visibility === 'hidden' || element.display === 'none'"));
        public virtual PointerJS IsExists() => As("element", "element !== null && element !== undefined");   
        public virtual PointerJS IsUndefined() => IsEqual("undefined");
        public virtual PointerJS IsNull() => IsEqual("null");

        public virtual PointerJS Count() => With("Array.from({0}).length");

        public virtual PointerJS SendKeys(string keys) => Scroll().Follows(InvokeKeyboardEvent(keys, "keydown"));
        public virtual PointerJS SendText(string text) => Scroll().Follows(InvokeKeyboardEvent(ConvertService.ToHotKeys(text), "keydown"));
     
        public virtual PointerJS Scroll() => ScrollingElement().With(".scrollIntoView({ behavior: 'smooth', block: 'end'})");
        public virtual PointerJS ScrollTo(PointerJS pointer) => ScrollX(pointer).Follows(ScrollY(pointer));
        public virtual PointerJS ScrollTo(string codeX, string codeY) => ScrollX(codeX).Follows(ScrollY(codeY));
        public virtual PointerJS ScrollTo(int x, int y) => ScrollX(x).Follows(ScrollY(y));
        public virtual PointerJS ScrollX(PointerJS pointer) => ScrollingElement().With(".scrollLeft").Set(pointer.Clone().PositionX());
        public virtual PointerJS ScrollX(string code) => ScrollingElement().With(".scrollLeft").Set(code);
        public virtual PointerJS ScrollX(int x) => ScrollingElement().With(".scrollLeft").Set(x);
        public virtual PointerJS ScrollY(PointerJS pointer) => ScrollingElement().With(".scrollTop").Set(pointer.Clone().PositionY());
        public virtual PointerJS ScrollY(string code) => ScrollingElement().With(".scrollTop").Set(code);
        public virtual PointerJS ScrollY(int y) => ScrollingElement().With(".scrollTop").Set(y);
        public virtual PointerJS ScrollingElement() => _IsDocument ? Clone().FromPure("document.scrollingElement") : this;
        public virtual PointerJS Position() => PositionX().Join(PositionY()).Array();
        public virtual PointerJS PositionX() => With(".offsetLeft");
        public virtual PointerJS PositionY() => With(".offsetTop");

        public virtual PointerJS Flue() => With(".blur()");
        public virtual PointerJS Focus() => With(".focus()");

        public virtual PointerJS Submit() => Scroll().Follows(With(".submit()"));
        public virtual PointerJS Click() => Scroll().Follows(With(".click()"));// As("element", "element.scrollIntoView(); element.click();");
        public virtual PointerJS DoubleClick() => InvokeMouseEvent("dblclick");
        public virtual PointerJS Hover() => InvokeMouseEvent("mouseenter");
        public virtual PointerJS InvokeMouseEvent(string eventName = "click") => InvokeEvent("MouseEvent", eventName);
        public virtual PointerJS InvokeKeyboardEvent(string keys, string eventName = "keypress")
        {
            //InitializeJQuery();
            //return Prepend(@"
                //var e = jQuery.Event(`"+ eventName + @"`);
                //e.keyCode = char.charCodeAt(0);
                //$(").Append(").trigger(e);").For("char","`" +keys.Replace("`","\\`")+ "`.split('')");
            return InvokeEvent("keyboardEvent",eventName,"null","char").ForEach("char", ToScript(keys)+ ".split('')");
        }
        public virtual PointerJS InvokeEvent(string eventName) => InvokeEvent("Event", eventName);
        public virtual PointerJS InvokeEvent(string eventType, string eventName, params string[] otherArgs)
        {
            return With(".dispatchEvent(evt);").Prepend(string.Join("",
                "var evt  = document.createEvent(`", eventType, "`);",
                "evt.init" + eventType + "(", ToScript(eventName), ", true, true" + (otherArgs.Length>1 ? ", "+string.Join(", ", otherArgs) :"") + ");"));
        }
        public virtual PointerJS InvokeEvents(string eventType, string eventName, IEnumerable<string[]> otherArgsList)
        {
            var p = With(".dispatchEvent(evt);").Prepend(string.Join("", "var evt  = document.createEvent(", ToScript(eventType), "`);"));
            foreach (var otherArgs in otherArgsList)
                p.With(string.Join("", "evt.init" + eventType + "(", ToScript(eventName), ", true, true" + (otherArgs.Length > 1 ? ", " + string.Join(", ", otherArgs) : "") + ");"));
            return p;
        }

        public virtual PointerJS NodeName() => With(".nodeName");
        public virtual PointerJS NodeType() => With(".nodeType");
        public virtual PointerJS NodeValue() => With(".nodeValue");
        public virtual PointerJS NextNode() => With(".nextSibling");
        public virtual PointerJS PreviousNode() => With(".previousSibling");
        public virtual PointerJS ParentNode() => With(".parentNode");
        public virtual PointerJS NormalizeNode() => With(".normalize()");
        public virtual PointerJS CloneNode(bool withChildren = true) => With(".cloneNode(" + (withChildren + "").ToLower() + ")");


        public virtual PointerJS Replace(PointerJS pointer) => Parent().With(".replaceChild(" + (pointer == null ? ToScript(null) : pointer.ToSnippet()) + ","+ToSnippet()+")");
        public virtual PointerJS Remove() => With(".remove()");
        public virtual PointerJS Closest(string query) => With(".closest(" + ToScript(query) + ")");
        public virtual PointerJS Matches(string query) => With(".matches(" + ToScript(query) + ")");
        public virtual PointerJS Next() => With(".nextElementSibling");
        public virtual PointerJS Previous() => With(".previousElementSibling");
        public virtual PointerJS Parent() => With(".parentElement");
        public virtual PointerJS Children() => With(".children");
        public virtual PointerJS Child(int index) => Children().With("[" + index + "]");
        public virtual PointerJS Child(params int[] indeces) => Children().With("[" + string.Join("].children[", indeces) + "]");

        public virtual PointerJS Get(int index) => With("[" + index + "]");
        public virtual PointerJS Get(params int[] indeces) => With("[" + string.Join("][", indeces) + "]");
        public virtual PointerJS Get(string name) => With("[" + ToScript(name) + "]");
        public virtual PointerJS Set(PointerJS pointer) => With("=" + (pointer == null ? ToScript(null) : pointer.ToSnippet()));
        public virtual PointerJS Get() => new PointerJS(this);

        public virtual PointerJS Set(object value) => Set(ToScript(value));
        public virtual PointerJS Set(string code) => With("=" + code);
        public virtual PointerJS Set(int index, PointerJS pointer) => Get(index).Set(pointer);
        public virtual PointerJS Set(int[] indeces, PointerJS pointer) => Get(indeces).Set(pointer);
        public virtual PointerJS Set(string name, PointerJS pointer) => Get(name).Set(pointer);
        public virtual PointerJS Set(int index, object value) => Get(index).Set(value);
        public virtual PointerJS Set(int[] indeces, object value) => Get(indeces).Set(value);
        public virtual PointerJS Set(string name, object value) => Get(name).Set(value);

        public virtual PointerJS GetParent() => With(".parentElement");
        public virtual PointerJS SetParent(PointerJS pointer) => GetParent().Set(pointer);
        public virtual PointerJS GetChild(int index) => Children().Get(index);
        public virtual PointerJS SetChild(int index,PointerJS pointer) => GetChild(index).Set(pointer);
        public virtual PointerJS ReplaceChild(int index,PointerJS pointer) => As("element", "element.replaceChild("+(pointer == null ? ToScript(null) : pointer.ToSnippet())+",element.children[" + index + "])");
        public virtual PointerJS RemoveChild(PointerJS pointer) => With(".removeChild("+ (pointer == null ? ToScript(null) : pointer.ToSnippet()) + ")");
        public virtual PointerJS RemoveChild(int index) => As("element", "element.removeChild(element.children[" + index + "])");
        public virtual PointerJS HasChild() => With(".hasChildNodes()");
        public virtual PointerJS HasChild(PointerJS pointer) => With(".contains(" + (pointer == null ? ToScript(null) : pointer.ToSnippet()) + ")");
        public virtual PointerJS HasChild(int index) => Children().With(".length>"+ index);
        public virtual PointerJS GetAttribute(string name) => With(".getAttribute("+ ToScript(name) +")");
        public virtual PointerJS SetAttribute(string name, object value) => With(".setAttribute(" + ToScript(name) +","+ ToScript(value) + ")");
        public virtual PointerJS RemoveAttribute(string name) => With(".removeAttribute(" + ToScript(name) +")");
        public virtual PointerJS HasAttribute(string name) => With(".hasAttribute(" + ToScript(name) + ")");
        public virtual PointerJS HasAttribute() => With(".hasAttributes()");
        public virtual PointerJS GetId() => With(".id");
        public virtual PointerJS SetId(string value) => GetId().Set(value);
        public virtual PointerJS GetName() => GetAttribute("name");
        public virtual PointerJS SetName(object value) => SetAttribute("name", value);
        public virtual PointerJS GetTitle() => With(".title");
        public virtual PointerJS SetTitle(object value) => SetAttribute("title", value);
        public virtual PointerJS GetContent() => With(".textContent");
        public virtual PointerJS SetContent(object value) => GetContent().Set(value);
        public virtual PointerJS GetText() => With(".innerText");
        public virtual PointerJS SetText(object value) => GetText().Set(value);
        public virtual PointerJS GetValue() => As("elem","elem.value??elem.innerText");
        public virtual PointerJS SetValue(object value) => As("elem", "{try{elem.value = " + ToScript(value) + ";}catch{elem.innerText = "+ ToScript(value) +";}}");
        public virtual PointerJS GetInnerHTML() => With(".innerHTML");
        public virtual PointerJS SetInnerHTML(object html) => GetInnerHTML().Set(html);
        public virtual PointerJS GetOuterHTML() => With(".outerHTML");
        public virtual PointerJS SetOuterHTML(object html) => GetOuterHTML().Set(html);
        public virtual PointerJS GetStyle() => Format("window.getComputedStyle({0})");
        public virtual PointerJS SetStyle(object style) => With(".style").Set(style);
        public virtual PointerJS GetStyle(string property) => With(".style."+ ConvertService.ToConcatedName(property.ToLower()));
        public virtual PointerJS SetStyle(string property, object value) => GetStyle(property).Set(value);
        public virtual PointerJS GetShadowRoot() => With(".shadowRoot");
        public virtual PointerJS SetShadowRoot(string mode="closed") => Format(".attachShadow({{mode:{1}}})", ToScript(mode));


        public virtual string ToSnippet()
        {
            if (_Multiple) return $"Array.from((function*(elements) {{ for(let element of elements) yield (()=>element)()}})({ElementsPointer()}))";
            else return ElementPointer();
        }
        public virtual string ToScript() =>
            (Sequence == null ? "" : Sequence.ToScript()) + ToSnippet();
        public static string ToScript(object value) =>
            value == null ? "null" :
            value is string ? string.Join("", "`", (value + "").Replace("`", "\\`"), "`") :
            value is bool ? value.ToString().ToLower() :
            value is IEnumerable ? "[" + string.Join(",", Statement.Loop((IEnumerable)value, (v) => ToScript(v))) + "]" :
            value is PointerJS ? ((PointerJS)value).ToScript() :
            value + "";


        public virtual bool Wait(long milisecond = 1000)
        {
            //return Not()
            //    .And("(delay -= 1000)>0")
            //    .While()
            //    .Then("new Promise(resolve => setTimeout(resolve, 1000));")
            //    .Prepend("let delay =" + milisecond + ";")
            //    .Return(this)
            //    .TryPerform(false);
            var tick = DateTime.Now.Ticks + milisecond * 10000;
            do
            {
                if (TryPerform(false)) return true;
                Task.Delay(3000);
            } while (tick > DateTime.Now.Ticks);
            return false;
        }

        public virtual string Parse() => ConvertService.ToString(TryPerform(default(object)));
        public virtual bool Parse(bool defaultValue = default) => ConvertService.TryToBoolean(TryPerform(default(object)), defaultValue);
        public virtual short Parse(short defaultValue = default) => ConvertService.TryToShort(TryPerform(default(object)), defaultValue);
        public virtual int Parse(int defaultValue = default) => ConvertService.TryToInt(TryPerform(default(object)), defaultValue);
        public virtual long Parse(long defaultValue = default) => ConvertService.TryToLong(TryPerform(default(object)), defaultValue);
        public virtual float Parse(float defaultValue = default) => ConvertService.TryToSingle(TryPerform(default(object)), defaultValue);
        public virtual double Parse(double defaultValue = default) => ConvertService.TryToDouble(TryPerform(default(object)), defaultValue);
        public virtual decimal Parse(decimal defaultValue = default) => ConvertService.TryToDecimal(TryPerform(default(object)), defaultValue);
        public virtual T Parse<T>(T defaultValue = default(T)) => TryPerform(defaultValue);

        public override string ToString() => TryPerform("");

        public static implicit operator string(PointerJS pointer) => pointer.Parse(string.Empty);
        public static implicit operator bool(PointerJS pointer) => pointer.Parse(false);
        public static implicit operator short(PointerJS pointer) => pointer.Parse((short)0);
        public static implicit operator int(PointerJS pointer) => pointer.Parse(0);
        public static implicit operator long(PointerJS pointer) => pointer.Parse(0L);
        public static implicit operator float(PointerJS pointer) => pointer.Parse(0F);
        public static implicit operator double(PointerJS pointer) => pointer.Parse(0D);
        public static implicit operator decimal(PointerJS pointer) => pointer.Parse(0M);

        public IEnumerator<PointerJS> GetEnumerator()
        {
            var pointer = PerformPointer();
            int index = 0;
            while (pointer.Get(index).IsExists().TryPerform(false))
                yield return pointer.Get(index++);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            var pointer = PerformPointer();
            int index = 0;
            while (pointer.Get(index).IsExists().TryPerform(false))
                yield return pointer.Get(index++);
        }


        public static explicit operator PointerJS(string value) => new PointerJS(ToScript(value), PointerMode.Pure);
        public static explicit operator PointerJS(bool value) => new PointerJS(ToScript(value), PointerMode.Pure);
        public static explicit operator PointerJS(short value) => new PointerJS(ToScript(value), PointerMode.Pure);
        public static explicit operator PointerJS(int value) => new PointerJS(ToScript(value), PointerMode.Pure);
        public static explicit operator PointerJS(long value) => new PointerJS(ToScript(value), PointerMode.Pure);
        public static explicit operator PointerJS(float value) => new PointerJS(ToScript(value), PointerMode.Pure);
        public static explicit operator PointerJS(double value) => new PointerJS(ToScript(value), PointerMode.Pure);
        public static explicit operator PointerJS(decimal value) => new PointerJS(ToScript(value), PointerMode.Pure);

    }
}
