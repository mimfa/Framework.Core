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
        #region MAIN
        public object Pointer { get; set; } = null;
        public PointerMode Mode { get; set; } = PointerMode.Pure;
        public Func<string, IEnumerable<object>, object> Execute { get; set; } = (s,a) => s;
        public object Evaluate(string code) => Execute(code,new object[] { });
        public PointerJS Sequence { get; set; } = null;
        public PointerJS Source { get; set; } = null;

        public bool _Multiple { get; set; } = false;
        public bool _Returnable { get; set; } = false;
        public bool _IsWindow => Source == null || Source.Pointer != (object)"document";
        public bool _IsDocument => Pointer == null && Source != null && Source.Pointer == (object)"document";

        public virtual PointerJS this[int index] { get => Get(index); set => Set(index, value); }
        public virtual PointerJS this[string name] { get => Get(name); set => Set(name, value); }


        public PointerJS(Func<string, IEnumerable<object>, object> executer, bool all = false, PointerJS source = null)
        {
            Execute = executer;
            _Multiple = all;
            Source = source;
            Initialize();
        }
        public PointerJS(object pointer, PointerMode mode = PointerMode.Undefined, bool all = false, PointerJS source = null)
        {
            Pointer = pointer;
            Mode = mode == PointerMode.Undefined ? DetectPointerMode(Pointer) : mode;
            _Multiple = all;
            Source = source;
            Initialize();
        }
        public PointerJS(object pointer, Func<string, IEnumerable<object>, object> executer, PointerMode mode = PointerMode.Undefined, bool all = false, PointerJS source = null)
        {
            Execute = executer;
            Pointer = pointer;
            Mode = mode == PointerMode.Undefined ? DetectPointerMode(Pointer) : mode;
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
        #endregion


        #region EXECUTIONS PART    
        public async Task<object> PerformAsync(params object[] args) => await ProcessService.RunTask<object, object>(o => Perform(args));
        public Task PerformTask(params object[] args) => ProcessService.RunTask(() => Perform(args));
        public Thread PerformThread(params object[] args) => ProcessService.Run(() => Perform(args));
        public Form PerformDialog(string message = "Wait until finish the process...", params object[] args) => ProcessService.RunDialog(message, (o, a) => Perform(args));      
        public T TryPerform<T>(string script, T defaultValue, params object[] args) => new PointerJS(script,Execute,PointerMode.Pure).TryPerform(defaultValue, args);
        public T TryPerform<T>(T defaultValue = default, params object[] args)
        {
            var script = ToScript();
            if (!_Returnable && !Regex.IsMatch(script, @"^\s*return\b", RegexOptions.Multiline))
                script = Return().ToScript();
            var o = Execute(script, args);
            return o is T ? (T)o : defaultValue;
        }
        public T Perform<T>(params object[] args) => (T)(Perform(args) ?? default(T));
        public void Perform(Action<object> process, params object[] args)
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
            var pName = UniqueName();
            var val = _Returnable? As(pName).Perform(args):Return().As(pName).Perform(args);
            return new PointerJS(pName, Execute, PointerMode.Pure);
        }

        public static bool MacroMode { get; set; } = false;
        
        /// <summary>
        /// Start macro coding mode (You can write multiple lines of procedures)
        /// </summary>
        /// <returns></returns>
        public PointerJS Begin()
        {
            MacroMode = true;
            return this;
        }
        /// <summary>
        /// Perform all macro codes and
        /// Finish macro coding mode (You should write single line of procedure)
        /// </summary>
        /// <param name="args">Send these arguments to the execute function of the browser</param>
        /// <returns></returns>
        public object End(params object[] args)
        {
            object res = Perform(args);
            MacroMode = false;
            return res;
        }
        /// <summary>
        /// Perform all macro codes and
        /// Finish macro coding mode (You should write single line of procedure)
        /// </summary>
        /// <param name="args">Send these arguments to the execute function of the browser</param>
        /// <param name="defaultValue">Default type and value expected to results</param>
        /// <returns></returns>
        public T End<T>(T defaultValue = default, params object[] args)
        {
            T res = TryPerform(defaultValue, args);
            MacroMode = false;
            return res;
        }


        /// <summary>
        /// {0};\r\nnextCode
        /// </summary>
        /// <returns>Updated Pointer</returns>
        public PointerJS Follows(object value) => Follows(ToScript(value));
        /// <summary>
        /// {0};\r\nnextCode
        /// </summary>
        /// <param name="nextCode">the next code to select</param>
        /// <returns>Updated Pointer</returns>
        public virtual PointerJS Follows(string nextCode) //=> Format("{0};\r\n{1}", nextCode);
        {
            if (MacroMode) return Format("{0};\r\n{1}", nextCode);
            Perform();
            return From(nextCode);
        }
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

        #endregion


        #region SELECTIONS PART
        public PointerJS SelectPure(string pointer, bool all = false) => Select(pointer, PointerMode.Pure, all);
        public PointerJS SelectById(string pointer, bool all = false) => Select(pointer, PointerMode.Id, all);
        public PointerJS SelectByTag(string pointer, bool all = false) => Select(pointer, PointerMode.Tag, all);
        public PointerJS SelectByName(string pointer, bool all = false) => Select(pointer, PointerMode.Name, all);
        public PointerJS SelectByClass(string pointer, bool all = false) => Select(pointer, PointerMode.Class, all);
        public PointerJS SelectByRegex(string pointer, bool all = false) => Select(pointer, PointerMode.Regex, all);
        public PointerJS SelectByXPath(string pointer, bool all = false) => Select(pointer, PointerMode.XPath, all);
        public PointerJS SelectByQuery(string pointer, bool all = false) => Select(pointer, PointerMode.Query, all);
        public PointerJS SelectByLocation(string pointer, bool all = false) => Select(pointer, PointerMode.Location, all);
        public PointerJS SelectByLocation(long x, long y, bool all = false) => Select(x,y, all);
        public PointerJS Select(Func<PointerJS, PointerJS> pointerCreator) => Select(pointerCreator(this));
        public PointerJS Select(string pointer, Func<string, IEnumerable<object>, object> executer, PointerMode pointerMode = PointerMode.Undefined, bool all = false) => Select(new PointerJS(pointer, executer, pointerMode, all, Source));
        public PointerJS Select(long x, long y, Func<string, IEnumerable<object>, object> executer, bool all = false) => Select(new PointerJS(x,y, executer, all, Source));
        public PointerJS Select(string pointer, PointerMode pointerMode = PointerMode.Undefined, bool all = false) => Select(new PointerJS(pointer, Execute, pointerMode, all,Source));
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
        public PointerJS FromByRegex(string pointer, bool all = false) => From(pointer, PointerMode.Regex, all);
        public PointerJS FromByXPath(string pointer, bool all = false) => From(pointer, PointerMode.XPath, all);
        public PointerJS FromByQuery(string pointer, bool all = false) => From(pointer, PointerMode.Query, all);
        public PointerJS FromByLocation(string pointer, bool all = false) => From(pointer, PointerMode.Location, all);
        public PointerJS FromByLocation(long x, long y, bool all = false) => From(x, y, all);
        public PointerJS From(Func<PointerJS, PointerJS> pointerCreator) => From(pointerCreator(this));
        public PointerJS From(string pointer, Func<string, IEnumerable<object>, object> executer, PointerMode pointerMode = PointerMode.Undefined, bool all = false) => From(new PointerJS(pointer, executer, pointerMode, all));
        public PointerJS From(long x, long y, Func<string, IEnumerable<object>, object> executer, bool all = false) => From(new PointerJS(x, y, executer, all));
        public PointerJS From(string pointer, PointerMode pointerMode = PointerMode.Undefined, bool all = false) => From(new PointerJS(pointer, Execute, pointerMode, all));
        public PointerJS From(long x, long y, bool all = false) => From(new PointerJS(x, y, Execute, all));
        public PointerJS From() => FromPure(null);
        public virtual PointerJS From(PointerJS pointer)
        {
            pointer.Execute = Execute ?? pointer.Execute;
            Sequence = pointer.Sequence;
            pointer.Sequence = null;
            return new PointerJS(this) { Source = pointer };
        }
        #endregion


        #region COMBINATIONS PART
        /// <summary>
        /// Create a JSPointer based on a string format
        /// </summary>
        /// <param name="format">{0} is the current Script</param>
        /// <param name="otherArgs">{0} and next arguments used in 'format'</param>
        /// <returns></returns>
        public PointerJS Format(string format = "{0}", params string[] otherArgs) => new PointerJS(this, string.Format(format, new string[] { ToSnippet() }.Concat(otherArgs).ToArray())) { Source = null };

        public PointerJS Prepend(object value) => Prepend(ToScript(value));
        public PointerJS Prepend(string code) => Prepend(new PointerJS(code, Execute, PointerMode.Pure));
        public virtual PointerJS Prepend(PointerJS pointer)
        {
            if (Sequence == null) Sequence = pointer;
            else Sequence.Prepend(pointer);
            return this;
        }

        public PointerJS Append(object value) => Append(ToScript(value));
        public PointerJS Append(string code) => Append(new PointerJS(code, Execute, PointerMode.Pure));
        public virtual PointerJS Append(PointerJS pointer)
        {
            if (pointer.Sequence == null) pointer.Sequence = this;
            else pointer.Sequence.Append(this);
            return pointer;
        }


        /// <summary>
        /// Add this pointer to sequence and continue with nextPointer
        /// </summary>
        /// <param name="nextCode">the next code to select</param>
        /// <returns>Updated Pointer</returns>
        public virtual PointerJS Also(string nextCode) => Source==null? Append(new PointerJS(nextCode, Execute, PointerMode.Pure)) : Append(new PointerJS(nextCode, Execute, PointerMode.Pure)).Append(Source.Clone());
        /// <summary>
        /// Add this pointer to sequence and continue with a new null pointer
        /// </summary>
        /// <returns>Updated Pointer</returns>
        public PointerJS Also() => Also(";");
        /// <summary>
        /// Add this pointer to sequence and continue with nextPointer
        /// </summary>
        /// <param name="nextPointer">the next pointer</param>
        /// <returns>Updated Pointer</returns>
        public PointerJS Also(PointerJS nextPointer) => Also(nextPointer == null ? ToScript(null) : nextPointer.ToScript());


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
        /// {0}\r\n(value)
        /// </summary>
        /// <returns>Updated Pointer</returns>
        public virtual PointerJS There(object value) => There(ToScript(value));
        /// <summary>
        /// {0}\r\n(code)
        /// </summary>
        /// <param name="code">a code snippet</param>
        /// <returns>Updated Pointer</returns>
        public virtual PointerJS There(string code) => Format("{0}\r\n((()=>{{{1}}})())", code);
        /// <summary>
        /// ({0})
        /// </summary>
        /// <returns>Updated Pointer</returns>
        public virtual PointerJS There() => Format("({0})");
        /// <summary>
        /// {0}\r\n(pointer)
        /// </summary>
        /// <param name="pointer">other pointer</param>
        /// <returns>Updated Pointer</returns>
        public PointerJS There(PointerJS pointer) => There(pointer == null ? ToScript(null) : pointer.ToSnippet());

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
        #endregion


        #region COLLECTIOINS PART
        public virtual PointerJS A() => new PointerJS(this);
        public virtual PointerJS One() => new PointerJS(this, false);
        public virtual PointerJS All() => new PointerJS(this, true);
        public virtual PointerJS The() => One();
        public virtual PointerJS The(string name) => All().Format("{0}['{1}']", name);
        public virtual PointerJS The(int index) => All().Format("{0}[{1}]", index.ToString());
        public virtual PointerJS First() => All().With("[0]");
        public virtual PointerJS Last() => All().With(".slice(-1).pop()");
        public virtual PointerJS Count() => Format("Array.from({0}).length");

        public virtual PointerJS Reverse() => With(".reverse()");
        public virtual PointerJS Slice(int index = 0, int? length = null) => With($".slice({index}" + (length == null ? ")" : $", {length})"));

        public virtual PointerJS Join(object value) => Join(ToScript(value));
        public virtual PointerJS Join(string code) => Format("{0},{1}", code);
        public virtual PointerJS Join() => Format("{0},");
        public PointerJS Join(PointerJS pointer) => Join(pointer == null ? ToScript(null) : pointer.ToSnippet());

        public virtual PointerJS Join(string name,object value) => Join(name, ToScript(value));
        public virtual PointerJS Join(string name,string code) => Format("{0},{1}:{2}", ToScript(name), code);
        public PointerJS Join(string name, PointerJS pointer) => Join(name, pointer == null ? ToScript(null) : pointer.ToSnippet());

        public virtual PointerJS Collect() => Format("{{0}}");
        public virtual PointerJS Array() => Format("[{0}]");
        #endregion


        #region OPERATIONS PART
        public virtual PointerJS Result(object value) => Result(ToScript(value));
        public virtual PointerJS Result(string code) => Format("{0}\r\nfunction(){{{0}}})()", code).Then(this);
        public virtual PointerJS Result() => Format("function(){{{0}}})()");
        public PointerJS Result(PointerJS pointer) => Result(pointer == null ? ToScript(null) : pointer.ToSnippet());

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
        public virtual PointerJS Return()
        {
            string name = UniqueName();
            return Format($"\r\nconst {name} = {{0}}; return {name};");
        }
        /// <summary>
        /// {0}; return pointer
        /// </summary>
        /// <param name="pointer">other pointer</param>
        /// <returns>Updated Pointer</returns>
        public PointerJS Return(PointerJS pointer) => Return(pointer == null ? ToScript(null) : pointer.ToSnippet());

        public virtual PointerJS If(object condition) => If(ToScript(condition));
        public virtual PointerJS If(string conditionCode) => Format("\r\nif({1}) ", conditionCode).Then(this);
        public virtual PointerJS If() => Format("{0}\r\nif ");
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

        public virtual PointerJS Do(object condition) => Do(ToScript(condition));
        public virtual PointerJS Do(string conditionCode) => Format("{0}\r\ndo {{{1}}} ", conditionCode).Then(this);
        public virtual PointerJS Do() => Format("do {{{0}}}\r\nwhile(true);");
        public PointerJS Do(PointerJS pointer) => Do(pointer == null ? ToScript(null) : pointer.ToSnippet());

        public virtual PointerJS While(object condition) => While(ToScript(condition));
        public virtual PointerJS While(string conditionCode) => Format("{0}\r\nwhile({1})", conditionCode).Then(this);
        public virtual PointerJS While() => Format("{0}\r\nwhile ");
        public PointerJS While(PointerJS pointer) => While(pointer == null ? ToScript(null) : pointer.ToSnippet());

        public virtual PointerJS Until(object condition) => Until(ToScript(condition));
        public virtual PointerJS Until(string conditionCode) => Format("\r\ndo {{{0}}}\r\nwhile({1});", conditionCode);
        public PointerJS Until(PointerJS pointer) => Until(pointer == null ? ToScript(null) : pointer.ToSnippet());
        public virtual PointerJS Until() => Format("{0}\r\nwhile ");

        public virtual PointerJS ForEach(string elementName, object collection) => ForEach(elementName, ToScript(collection));
        public virtual PointerJS ForEach(string elementName, string collectionCode) => Format("\r\nfor(let {1} of {2}) ", elementName, collectionCode).Then(this);
        public virtual PointerJS ForEach(string elementName) => Format("\r\nfor(let {1} of {0}) ", elementName);
        public virtual PointerJS ForEach() => Format("\r\nfor(let {1} of {0}) ", "element");
        public PointerJS ForEach(string elementName, PointerJS pointer) => ForEach(elementName, pointer == null ? ToScript(null) : pointer.ToSnippet());

        public virtual PointerJS For(object defination, object condition, object iteration) => For(ToScript(defination), ToScript(condition), ToScript(iteration));
        public virtual PointerJS For(string definationCode = "", string conditionCode = "", string iterationCode = "") => Format("\r\nfor(let {1}; {2}; {1}) ", definationCode, conditionCode, iterationCode).Then(this);
        public virtual PointerJS For(string counterName, int length) => For(counterName+"=0", counterName + (0 < length ? "<" : ">") + length, counterName + (0 < length?"++":"--"));
        public virtual PointerJS For(int index, int length, string counterName = null)
        {
            counterName = counterName??UniqueName();
            return For(counterName + "=" + index, counterName + (index < length ? "<" : "--") + length, counterName + (index < length ? "++" : "--"));
        }
        public PointerJS For(PointerJS defination, PointerJS condition, PointerJS iteration) => For(defination == null ? null : defination.ToSnippet(), condition == null ? null : condition.ToSnippet(), iteration == null ? null : iteration.ToSnippet());

        public virtual PointerJS Each(string elementName, object collection) => Each(elementName, ToScript(collection));
        public virtual PointerJS Each(string elementName, string collectionCode) => Format("\r\nfor(let {1} in {2}) ", elementName, collectionCode).Then(this);
        public virtual PointerJS Each(string elementName) => Format("\r\nfor(let {1} in {0}) ", elementName);
        public virtual PointerJS Each() => Format("\r\nfor(let {1} in {0}) ", "element");
        public PointerJS Each(string elementName, PointerJS pointer) => Each(elementName, pointer == null ? ToScript(null) : pointer.ToSnippet());

        public virtual PointerJS As(string elementName,object value) => As(elementName, ToScript(value));
        public virtual PointerJS As(string elementName, string code) => Format("(({1})=>{2})({0})", elementName, code);
        public virtual PointerJS As(string elementName) => Format("{1} = (()=>{{{0}}})()", elementName);
        public PointerJS As(string elementName, PointerJS nextPointer) => As(elementName, nextPointer == null ? ToScript(null) : nextPointer.ToSnippet());
       
        public virtual PointerJS Var(string elementName) => new PointerJS(Format(";\r\nvar {1}", elementName)) { Source = this};
        public virtual PointerJS Let(string elementName) => new PointerJS(Format(";\r\nlet {1}", elementName)) { Source = this };
        public virtual PointerJS Const(string elementName) => new PointerJS(Format(";\r\nconst {1}", elementName)) { Source = this };
        public virtual PointerJS Named(string elementName) => Format("{1}:{0}", elementName);
        #endregion


        #region COMPUTATIONS PART
        public virtual PointerJS Equal(object value) => Equal(ToScript(value));
        public virtual PointerJS Equal(string code) => Format("{0}={1}", code);
        public virtual PointerJS Equal() => Equal("");
        public virtual PointerJS Equal(PointerJS pointer) => Equal(pointer == null ? ToScript(null) : pointer.ToSnippet());

        public virtual PointerJS Minus(object value) => Minus(ToScript(value));
        public virtual PointerJS Minus(string code) => Format("{0}-{1}", code);
        public virtual PointerJS Minus() => Minus("");
        public virtual PointerJS Minus(PointerJS pointer) => Minus(pointer == null ? ToScript(null) : pointer.ToSnippet());

        public virtual PointerJS Plus(object value) => Plus(ToScript(value));
        public virtual PointerJS Plus(string code) => Format("{0}+{1}", code);
        public virtual PointerJS Plus() => Plus("");
        public virtual PointerJS Plus(PointerJS pointer) => Plus(pointer == null ? ToScript(null) : pointer.ToSnippet());

        public virtual PointerJS Multiple(object value) => Multiple(ToScript(value));
        public virtual PointerJS Multiple(string code) => Format("{0}*{1}", code);
        public virtual PointerJS Multiple() => Multiple("");
        public virtual PointerJS Multiple(PointerJS pointer) => Multiple(pointer == null ? ToScript(null) : pointer.ToSnippet());

        public virtual PointerJS Divide(object value) => Divide(ToScript(value));
        public virtual PointerJS Divide(string code) => Format("{0}/{1}", code);
        public virtual PointerJS Divide() => Divide("");
        public virtual PointerJS Divide(PointerJS pointer) => Divide(pointer == null ? ToScript(null) : pointer.ToSnippet());

        public virtual PointerJS Power(object value) => Power(ToScript(value));
        public virtual PointerJS Power(string code) => Format("{0}**{1}", code);
        public virtual PointerJS Power() => Power("");
        public virtual PointerJS Power(PointerJS pointer) => Power(pointer == null ? ToScript(null) : pointer.ToSnippet());
    
        public virtual PointerJS Square(object value) => Square(ToScript(value));
        public virtual PointerJS Square(string code) => Format("{0}**(1/{1})", code);
        public virtual PointerJS Square() => Square("");
        public virtual PointerJS Square(PointerJS pointer) => Square(pointer == null ? ToScript(null) : pointer.ToSnippet());
        #endregion


        #region CONDITIONS PART
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
        #endregion


        #region ACTIONS PART
        public PointerJS Load(PointerJS pointer) => Load(pointer == null ? ToScript(null) : pointer.ToSnippet());
        public virtual PointerJS Load(object value) => Format("{0}.location.href={1}", ToScript(value)).Follows();
        public virtual PointerJS ReLoad() => Format("{0}.location.href={1}.location.href").Follows();

        public virtual PointerJS Scroll() => GetScrollBar().With(".scrollIntoView({ behavior: 'smooth', block: 'end'})").Follows();
        public virtual PointerJS ScrollTo(PointerJS pointer) => ScrollX(pointer).Follows(ScrollY(pointer));
        public virtual PointerJS ScrollTo(string codeX, string codeY) => ScrollX(codeX).Follows(ScrollY(codeY));
        public virtual PointerJS ScrollTo(int x, int y) => ScrollX(x).Follows(ScrollY(y));
        public virtual PointerJS ScrollX(PointerJS pointer) => GetScrollBar().With(".scrollLeft").Set(pointer.Clone().GetScrollPositionX());
        public virtual PointerJS ScrollX(string code) => GetScrollBar().With(".scrollLeft").Set(code);
        public virtual PointerJS ScrollX(int x) => GetScrollBar().With(".scrollLeft").Set(x);
        public virtual PointerJS ScrollY(PointerJS pointer) => GetScrollBar().With(".scrollTop").Set(pointer.Clone().GetScrollPositionY());
        public virtual PointerJS ScrollY(string code) => GetScrollBar().With(".scrollTop").Set(code);
        public virtual PointerJS ScrollY(int y) => GetScrollBar().With(".scrollTop").Set(y);

        public virtual PointerJS Flue() => With(".blur()").Follows();
        public virtual PointerJS Focus() => With(".focus()").Follows();

        public virtual PointerJS SendKeys(string keys) => Scroll().Follows(InvokeKeyboardEvent(keys, "keydown"));
        public virtual PointerJS SendText(string text) => Scroll().Follows(InvokeKeyboardEvent(ConvertService.ToHotKeys(text), "keydown"));

        public virtual PointerJS Submit() => Scroll().Also(With(".submit()").Follows());
        public virtual PointerJS Click() => Scroll().Also(With(".click()").Follows());
        public virtual PointerJS DoubleClick() => InvokeMouseEvent("dblclick");
        public virtual PointerJS Hover() => InvokeMouseEvent("mouseenter");
        public virtual PointerJS KeyPress(string keys) => InvokeKeyboardEvent(keys, "keypress");
        public virtual PointerJS KeyUp(string keys) => InvokeKeyboardEvent(keys, "keyup");
        public virtual PointerJS KeyDown(string keys) => InvokeKeyboardEvent(keys, "keyup");
        public virtual PointerJS InvokeMouseEvent(string eventName = "click") => InvokeEvent("MouseEvent", eventName);
        public virtual PointerJS InvokeKeyboardEvent(string keys, string eventName = "keypress") => InvokeEvent("keyboardEvent",eventName,"null","char").ForEach("char", ToScript(keys)+ ".split('')");
        public virtual PointerJS InvokeEvent(string eventName) => InvokeEvent("Event", eventName);
        public virtual PointerJS InvokeEvent(string eventType, string eventName, params string[] otherArgs) => With(".dispatchEvent(evt);").Prepend(string.Join("",
                "var evt  = document.createEvent(`", eventType, "`);",
                "evt.init" + eventType + "(", ToScript(eventName), ", true, true" + (otherArgs.Length>1 ? ", "+string.Join(", ", otherArgs) :"") + ");")).Follows();
        public virtual PointerJS InvokeEvents(string eventType, string eventName, IEnumerable<string[]> otherArgsList)
        {
            var p = With(".dispatchEvent(evt);").Prepend(string.Join("", "var evt  = document.createEvent(", ToScript(eventType), "`);")).Follows();
            foreach (var otherArgs in otherArgsList)
                p.With(string.Join("", "evt.init" + eventType + "(", ToScript(eventName), ", true, true" + (otherArgs.Length > 1 ? ", " + string.Join(", ", otherArgs) : "") + ");")).Follows();
            return p;
        }
        #endregion


        #region ELEMENTATIONS PART
        public virtual PointerJS NodeName() => With(".nodeName");
        public virtual PointerJS NodeType() => With(".nodeType");
        public virtual PointerJS NodeValue() => With(".nodeValue");
        public virtual PointerJS NextNode() => With(".nextSibling");
        public virtual PointerJS PreviousNode() => With(".previousSibling");
        public virtual PointerJS ParentNode() => With(".parentNode");
        public virtual PointerJS NormalizeNode() => With(".normalize()");
        public virtual PointerJS CloneNode(bool withChildren = true) => With(".cloneNode(" + (withChildren + "").ToLower() + ")");

        public virtual PointerJS Replace(PointerJS pointer) => Parent().With(".replaceChild(" + (pointer == null ? ToScript(null) : pointer.ToSnippet()) + ","+ToSnippet()+")").Follows();
        public virtual PointerJS Remove() => With(".remove()").Follows();
        public virtual PointerJS Closest(string query) => With(".closest(" + ToScript(query) + ")");
        public virtual PointerJS Matches(string query) => With(".matches(" + ToScript(query) + ")");
        public virtual PointerJS Next() => With(".nextElementSibling");
        public virtual PointerJS Previous() => With(".previousElementSibling");
        public virtual PointerJS Parent() => With(".parentElement");
        public virtual PointerJS Children() => With(".children");
        public virtual PointerJS Child(int index) => Children().With("[" + index + "]");
        public virtual PointerJS Child(params int[] indeces) => Children().With("[" + string.Join("].children[", indeces) + "]");
        #endregion


        #region NORMALIZATIONS PART

        public virtual PointerJS Length() => With(".length");

        /// <summary>
        /// Get all search results in your source
        /// </summary>
        /// <param name="valueOrPattern">A simple string or Regex pattern to search</param>
        /// <returns>An array of all search results</returns>
        public virtual PointerJS Search(object valueOrPattern) => With($".match({ToScript(valueOrPattern)})").As("element").Then().Return("Array.isArray(element)?item:element===null?[]:[element]").Result();
        /// <summary>
        /// Find the first match of the pattern in the source
        /// </summary>
        /// <param name="valueOrPattern">A simple string or Regex pattern to search</param>
        /// <returns>The first match found</returns>
        public virtual PointerJS Find(object valueOrPattern) => With($".match({ToScript(valueOrPattern)})").Format("({0}??[null])[0]");
        /// <summary>
        /// Find the match at the specified index
        /// </summary>
        /// <param name="valueOrPattern">A simple string or Regex pattern to search</param>
        /// <param name="index">Index of the match to find</param>
        /// <returns>The match found at the specified index</returns>
        public virtual PointerJS Find(object valueOrPattern, int index) => With($".match({ToScript(valueOrPattern)})").Format("{0}[index]");
        /// <summary>
        /// Replace occurrences of a value or pattern with a new value
        /// </summary>
        /// <param name="oldValueOrPattern">A simple string or Regex pattern to be replaced</param>
        /// <param name="newValue">A new value to replace the old value or pattern (default is null)</param>
        /// <returns>A new string with the replacements</returns>
        public virtual PointerJS Replace(object oldValueOrPattern, object newValue = null) => With($".replace({ToScript(oldValueOrPattern)}, {ToScript(newValue)})");
        /// <summary>
        /// Remove occurrences of a value or pattern from the source
        /// </summary>
        /// <param name="valueOrPattern">A simple string or Regex pattern to be removed</param>
        /// <returns>A new string without the specified value or pattern</returns>
        public virtual PointerJS Remove(object valueOrPattern) => Replace(valueOrPattern, "");
        /// <summary>
        /// Get the position of the first occurrence of a value or pattern
        /// </summary>
        /// <param name="valueOrPattern">A simple string or Regex pattern to search</param>
        /// <returns>The position of the first occurrence</returns>
        public virtual PointerJS Position(object valueOrPattern) => With($".search({ToScript(valueOrPattern)})");
        /// <summary>
        /// Get the position of the first occurrence of a value starting from the start index
        /// </summary>
        /// <param name="value">The value to search</param>
        /// <param name="startIndex">The start index</param>
        /// <returns>The position of the first occurrence starting from the start index</returns>
        public virtual PointerJS Position(int startIndex, string value) => With($".indexOf({ToScript(value)}, {startIndex})");
        /// <summary>
        /// Get the position of the last occurrence of a value
        /// </summary>
        /// <param name="value">The value to search</param>
        /// <returns>The position of the last occurrence</returns>
        public virtual PointerJS LastPosition(string value) => With($".lastIndexOf({ToScript(value)})");

        /// <summary>
        /// Split the source into an array of substrings
        /// </summary>
        /// <param name="valueOrPattern">A simple string or Regex pattern to split the source</param>
        /// <returns>An array of substrings</returns>
        public virtual PointerJS Split(object valueOrPattern) => With($".split({ToScript(valueOrPattern)})").As("element").Then().Return("Array.isArray(element)?item:element===null?[]:[element]").Result();
        /// <summary>
        /// Split the source into an array of substrings based on whitespace
        /// </summary>
        /// <param name="valueOrPattern">A simple string or Regex pattern to split the source (default is whitespace)</param>
        /// <returns>An array of substrings</returns>
        public virtual PointerJS Split(string valueOrPattern = @"/\s/") => Split((object)valueOrPattern);
        /// <summary>
        /// Get the substring from the start index to the end of the string
        /// </summary>
        /// <param name="startIndex">The start index</param>
        /// <returns>The substring from the start index to the end of the string</returns>
        public virtual PointerJS Segment(int startIndex) => With($".substring({startIndex})");
        /// <summary>
        /// Get the substring from the start index to the specified length
        /// </summary>
        /// <param name="startIndex">The start index</param>
        /// <param name="length">The length of the substring</param>
        /// <returns>The substring from the start index with the specified length</returns>
        public virtual PointerJS Segment(int startIndex, int length) => With($".substring({startIndex}, {length})");
        /// <summary>
        /// Get the slice from the start index to the end of the string
        /// </summary>
        /// <param name="startIndex">The start index</param>
        /// <returns>The slice from the start index to the end of the string</returns>
        public virtual PointerJS Slice(int startIndex) => With($".slice({startIndex})");
        /// <summary>
        /// Get the slice from the start index to the end index
        /// </summary>
        /// <param name="startIndex">The start index</param>
        /// <param name="endIndex">The end index</param>
        /// <returns>The slice from the start index to the end index</returns>
        public virtual PointerJS Slice(int startIndex, int endIndex) => With($".slice({startIndex}, {endIndex})");
        /// <summary>
        /// Concatenate multiple pointers into a single string
        /// </summary>
        /// <param name="pointers">An array of pointers to concatenate</param>
        /// <returns>The concatenated string</returns>
        public virtual PointerJS Concat(params PointerJS[] pointers) => With($".concat({string.Join(", ", from pointer in pointers select pointer == null ? ToScript(null) : pointer.ToSnippet())})");
        /// <summary>
        /// Concatenate multiple strings into a single string
        /// </summary>
        /// <param name="values">An array of strings to concatenate</param>
        /// <returns>The concatenated string</returns>
        public virtual PointerJS Concat(params string[] values) => With($".concat({string.Join(", ", from v in values select ToScript(v))})");
        /// <summary>
        /// Repeat the source string a specified number of times
        /// </summary>
        /// <param name="count">The number of times to repeat</param>
        /// <returns>The repeated string</returns>
        public virtual PointerJS Repeat(int count) => With($".repeat({count})");

        /// <summary>
        /// Check if the source includes the specified value
        /// </summary>
        /// <param name="value">The value to search</param>
        /// <returns>True if the source includes the value, otherwise false</returns>
        public virtual PointerJS Includes(string value) => With($".includes({ToScript(value)})");
        /// <summary>
        /// Check if the source starts with the specified value
        /// </summary>
        /// <param name="value">The value to search</param>
        /// <returns>True if the source starts with the value, otherwise false</returns>
        public virtual PointerJS StartsWith(string value) => With($".startsWith({ToScript(value)})");
        /// <summary>
        /// Check if the source ends with the specified value
        /// </summary>
        /// <param name="value">The value to search</param>
        /// <returns>True if the source ends with the value, otherwise false</returns>
        public virtual PointerJS EndsWith(string value) => With($".endsWith({ToScript(value)})");
        /// <summary>
        /// Remove extra whitespace and replace multiple spaces with a single space
        /// </summary>
        /// <returns>The cleaned string</returns>
        public virtual PointerJS Clean() => Trim().Replace(@"/(\s)+/gm", "$1");
        /// <summary>
        /// Trim whitespace from both ends of the source string
        /// </summary>
        /// <returns>The trimmed string</returns>
        public virtual PointerJS Trim() => With(".trim()");
        /// <summary>
        /// Trim whitespace from the start of the source string
        /// </summary>
        /// <returns>The trimmed string</returns>
        public virtual PointerJS TrimStart() => With(".trimStart()");
        /// <summary>
        /// Trim whitespace from the end of the source string
        /// </summary>
        /// <returns>The trimmed string</returns>
        public virtual PointerJS TrimEnd() => With(".trimEnd()");
        /// <summary>
        /// Pad the source string to the specified length with the specified character pattern
        /// </summary>
        /// <param name="length">The total length to pad to</param>
        /// <param name="charPattern">The character pattern to use for padding (default is a space)</param>
        /// <returns>The padded string</returns>
        public virtual PointerJS Pad(int length, string charPattern = " ") => PadStart(length / 2, charPattern).PadEnd(length, charPattern);
        /// <summary>
        /// Pad the start of the source string to the specified length with the specified character pattern
        /// </summary>
        /// <param name="length">The total length to pad to</param>
        /// <param name="charPattern">The character pattern to use for padding (default is a space)</param>
        /// <returns>The padded string</returns>
        public virtual PointerJS PadStart(int length, string charPattern = " ") => With($".padStart({length}, {ToScript(charPattern)})");
        /// <summary>
        /// Pad the end of the source string to the specified length with the specified character pattern
        /// </summary>
        /// <param name="length">The total length to pad to</param>
        /// <param name="charPattern">The character pattern to use for padding (default is a space)</param>
        /// <returns>The padded string</returns>
        public virtual PointerJS PadEnd(int length, string charPattern = " ") => With($".padEnd({length}, {ToScript(charPattern)})");
        /// <summary>
        /// Reverse the source string
        /// </summary>
        /// <returns>The reversed string</returns>
        public virtual PointerJS ReverseChars() => With(@".split('').reverse().join('')");

        /// <summary>
        /// Convert the source string to uppercase
        /// </summary>
        /// <returns>The uppercase string</returns>
        public virtual PointerJS ToUpperCase() => With(".toUpperCase()");
        /// <summary>
        /// Convert the source string to lowercase
        /// </summary>
        /// <returns>The lowercase string</returns>
        public virtual PointerJS ToLowerCase() => With(".toLowerCase()");
        /// <summary>
        /// Convert the source string to proper case (first letter of each word is capitalized, others are lowercase)
        /// </summary>
        /// <returns>The proper case string</returns>
        public virtual PointerJS ToProperCase() => With(@".replace(/\w\S*/g, function(txt)=> txt.charAt(0).toUpperCase() + txt.substr(1).toLowerCase())");
        /// <summary>
        /// Convert the source string to name case (first letter of each word is capitalized, others remain unchanged)
        /// </summary>
        /// <returns>The name case string</returns>
        public virtual PointerJS ToNameCase() => With(@".replace(/\w\S*/g, function(txt)=> txt.charAt(0).toUpperCase() + txt.substr(1))");
        /// <summary>
        /// Convert the source string to inverse case (first letter of each word is lowercase, others are uppercase)
        /// </summary>
        /// <returns>The inverse case string</returns>
        public virtual PointerJS ToInverseCase() => With(@".replace(/\w\S*/g, function(txt)=> txt.charAt(0).toLowerCase() + txt.substr(1).toUpperCase())");
        /// <summary>
        /// Toggle the case of each character in the source string
        /// </summary>
        /// <returns>The toggle case string</returns>
        public virtual PointerJS ToToggleCase() => With(@".replace(/./g, function(char) => char.toLowerCase() == char ? char.toUpperCase() : char.toLowerCase())");

        #endregion


        #region OPTIONS PART
        public virtual PointerJS Get(int index) => With("[" + index + "]");
        public virtual PointerJS Get(params int[] indeces) => With("[" + string.Join("][", indeces) + "]");
        public virtual PointerJS Get(string name) => With("[" + ToScript(name) + "]");
        public virtual PointerJS Get() => new PointerJS(this);

        public virtual PointerJS Set(string code) => With("=" + code).Follows();
        public PointerJS Set(object value) => Set(ToScript(value));
        public PointerJS Set(PointerJS pointer) => Set(pointer == null ? ToScript(null) : pointer.ToSnippet());
        public PointerJS Set(int index, PointerJS pointer) => Get(index).Set(pointer);
        public PointerJS Set(int[] indeces, PointerJS pointer) => Get(indeces).Set(pointer);
        public PointerJS Set(string name, PointerJS pointer) => Get(name).Set(pointer);
        public PointerJS Set(int index, object value) => Get(index).Set(value);
        public PointerJS Set(int[] indeces, object value) => Get(indeces).Set(value);
        public PointerJS Set(string name, object value) => Get(name).Set(value);

        public virtual PointerJS GetParent() => With(".parentElement");
        public virtual PointerJS SetParent(PointerJS pointer) => GetParent().Set(pointer);
        public virtual PointerJS GetChild(int index) => Children().Get(index);
        public virtual PointerJS SetChild(int index,PointerJS pointer) => GetChild(index).Set(pointer);
        public virtual PointerJS ReplaceChild(int index,PointerJS pointer) => As("element", "element.replaceChild("+(pointer == null ? ToScript(null) : pointer.ToSnippet())+",element.children[" + index + "])").Follows();
        public virtual PointerJS RemoveChild(PointerJS pointer) => With(".removeChild("+ (pointer == null ? ToScript(null) : pointer.ToSnippet()) + ")").Follows();
        public virtual PointerJS RemoveChild(int index) => As("element", "element.removeChild(element.children[" + index + "])").Follows();
        public virtual PointerJS HasChild() => With(".hasChildNodes()");
        public virtual PointerJS HasChild(PointerJS pointer) => With(".contains(" + (pointer == null ? ToScript(null) : pointer.ToSnippet()) + ")");
        public virtual PointerJS HasChild(int index) => Children().With(".length>"+ index);
        public virtual PointerJS GetAttribute(string name) => With(".getAttribute("+ ToScript(name) +")");
        public virtual PointerJS SetAttribute(string name, object value) => With(".setAttribute(" + ToScript(name) +","+ ToScript(value) + ")").Follows();
        public virtual PointerJS RemoveAttribute(string name) => With(".removeAttribute(" + ToScript(name) +")").Follows();
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
        public virtual PointerJS SetValue(object value) => As("elem", "{try{elem.value = " + ToScript(value) + ";}catch{elem.innerText = "+ ToScript(value) +";}}").Follows();
        public virtual PointerJS GetInnerHTML() => With(".innerHTML");
        public virtual PointerJS SetInnerHTML(object html) => GetInnerHTML().Set(html);
        public virtual PointerJS GetOuterHTML() => With(".outerHTML");
        public virtual PointerJS SetOuterHTML(object html) => GetOuterHTML().Set(html);
        public virtual PointerJS GetScrollPosition() => GetScrollPositionX().Join(GetScrollPositionY()).Array();
        public virtual PointerJS SetScrollPosition(int xOffset = 0, int yOffset = 0) => SetScrollPositionX(xOffset).Also().SetScrollPositionY();
        public virtual PointerJS GetScrollPositionX() => With(".offsetLeft");
        public virtual PointerJS SetScrollPositionX(int offset = 0) => GetScrollPositionX().Set(offset);
        public virtual PointerJS GetScrollPositionY() => With(".offsetTop");
        public virtual PointerJS SetScrollPositionY(int offset = 0) => GetScrollPositionY().Set(offset);
        public virtual PointerJS GetScrollBar() => _IsDocument ? Clone().FromPure("document.scrollingElement") : this;
        public virtual PointerJS GetStyle() => Format("window.getComputedStyle({0})");
        public virtual PointerJS SetStyle(object style) => With(".style").Set(style);
        public virtual PointerJS GetStyle(string property) => With(".style."+ ConvertService.ToConcatedName(property.ToLower()));
        public virtual PointerJS SetStyle(string property, object value) => GetStyle(property).Set(value);
        public virtual PointerJS GetShadowRoot() => With(".shadowRoot");
        public virtual PointerJS SetShadowRoot(string mode="closed") => Format(".attachShadow({{mode:{1}}})", ToScript(mode)).Follows();
        #endregion


        #region CONVERSIONS PART 
        public string ElementPointer()
        {
            var source = Source == null ? "" : Source.ToScript();
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
                case PointerMode.Regex:
                    return string.Join("", "(()=>{", "for (let el of ", source, ".querySelectorAll('*')) { if (regex.test(el.textContent)) return el; else return null;})()");
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
                case PointerMode.Regex:
                    return string.Join("", "Array.from(", source, ".querySelectorAll('*')).filter(el => regex.test(el.textContent))");
                case PointerMode.Query:
                    return string.Join("", source, ".querySelectorAll(", ToScript(Pointer), ")");
                case PointerMode.XPath:
                    return string.Join("", "Array.from((function*(){ let iterator = ", source, ".evaluate(", ToScript(Pointer), ", document, null, XPathResult.UNORDERED_NODE_ITERATOR_TYPE, null); let current = iterator.iterateNext(); while(current){ yield current; current = iterator.iterateNext(); }  })())");
                default:
                    //return string.Join("", "Array.from((function*(){ let iterator = ", source, Pointer, "; let current = iterator.iterateNext(); while(current){ yield current; current = iterator.iterateNext(); }  })())");
                    return $"{source}{Pointer}";
            }
        }

        public virtual string ToSnippet()
        {
            if (_Multiple) return $"Array.from((function*(elements) {{ for(let element of elements) yield (()=>element)()}})({ElementsPointer()}))";
            else return ElementPointer();
        }
        public virtual string ToScript() =>
            (Sequence == null ? "" : Sequence.ToScript()) + ToSnippet();

        public static string ToScript(object value) =>
            value == null ? "null" :
            value is string ? Regex.IsMatch(value + "", @"^\s*\/.*\/[gimsuy]{0,6}\s*$") ? (value + "") : string.Join("", "`", (value + "").Replace("`", "\\`"), "`") :
            value is bool ? value.ToString().ToLower() :
            value is IEnumerable ? "[" + string.Join(",", Statement.Loop((IEnumerable)value, (v) => ToScript(v))) + "]" :
            value is PointerJS ? ((PointerJS)value).ToScript() :
            value + "";
        public static string UniqueName(string starts = "P_", string ends = "_") => $"{starts}{DateTime.Now.Ticks}{ends}";
        public static PointerMode DetectPointerMode(object pointer)
        {
            if (pointer == null) return PointerMode.Undefined;
            if (pointer is PointerJS) return ((PointerJS)pointer).Mode;
            if (pointer is int || pointer is Enumerable) return PointerMode.Location;
            string spointer = (pointer + "").Trim();
            if (string.IsNullOrWhiteSpace(spointer)) return PointerMode.Undefined;
            if (Regex.IsMatch(spointer, @"^\s*\/.*\/[gimsuy]{0,6}\s*$")) return PointerMode.Regex;
            if (Regex.IsMatch(spointer, @"^\d+\s*[\,\;]\s*\d+$")) return PointerMode.Location;
            if (Regex.IsMatch(spointer, @"^[A-Za-z][A-Za-z\-_]+$")) return PointerMode.Class;
            if (Regex.IsMatch(spointer, @"^[A-Za-z][A-Za-z\d_]+$")) return PointerMode.Id;
            if (Regex.IsMatch(spointer, @"^\/") && !Regex.IsMatch(spointer, @"[\r\n\t\f\v]")) return PointerMode.XPath;
            if (Regex.IsMatch(spointer, @"^(document|window)\s*\.\s*[A-Za-z]+")) return PointerMode.Pure;
            if (Regex.IsMatch(spointer, @"(\s*\w+\s*\>)|(^\s*[\#\.\:])|(\s*\w+\s*\[\#\.])")) return PointerMode.Query;
            if (Regex.IsMatch(spointer, @"(\s*(document|window)\s*\.\s*[A-Za-z]+)|([\=\+\-\*\/]\=\=?)")) return PointerMode.Pure;
            if (Regex.IsMatch(spointer, @"(^\/)|((\w+|\*)\/(\w+|\*))") && !Regex.IsMatch(spointer, @"[\r\n\t\f\v]")) return PointerMode.XPath;
            return PointerMode.Query;
        }


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
        #endregion
    }
}
