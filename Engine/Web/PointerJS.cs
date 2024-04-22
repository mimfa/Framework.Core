using MiMFa.Service;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MiMFa.Engine.Web
{
    public class PointerJS : IEnumerable<PointerJS>
    {
        public string Pointer { get; set; } = null;
        public PointerMode Mode { get; set; } = PointerMode.Pure;
        public Func<string, IEnumerable<object>, object> Execute { get; set; } = (s,a) => s;
        public object Evaluate(string code) => Execute(code,new object[] { });
        public bool Multiple { get; set; } = false;
        public PointerJS Source { get; set; } = null;
        public string Script { get; set; } = null;
        public bool AccessToJQuery { get; set; } = false;

        public PointerJS(Func<string, IEnumerable<object>, object> executer, bool all = false, PointerJS source = null)
        {
            Execute = executer;
            Multiple = all;
            Source = source;
            Initialize();
        }
        public PointerJS(string pointer, PointerMode mode = PointerMode.Query, bool all = false, PointerJS source = null)
        {
            Mode = mode;
            Pointer = pointer;
            Multiple = all;
            Source = source;
            Initialize();
        }
        public PointerJS(string pointer, Func<string, IEnumerable<object>, object> executer, PointerMode mode = PointerMode.Query, bool all = false, PointerJS source = null)
        {
            Execute = executer;
            Mode = mode;
            Pointer = pointer;
            Multiple = all;
            Source = source;
            Initialize();
        }
        public PointerJS(long x, long y, Func<string, IEnumerable<object>, object> executer, bool all = false, PointerJS source = null)
        {
            Pointer = string.Join(", ", x, y);
            Mode = PointerMode.Location;
            Execute = executer;
            Multiple = all;
            Source = source;
            Initialize();
        }
        public PointerJS(PointerJS pointer, bool? all = null) : this(pointer, pointer.Script, all)
        {
        }
        public PointerJS(PointerJS pointer, string script, bool? all = null) : this(pointer.Pointer, pointer.Execute, pointer.Mode, all ?? pointer.Multiple, pointer.Source)
        {
            Script = script;
            AccessToJQuery = pointer.AccessToJQuery;
            Initialize();
        }

        public PointerJS Clone() => new PointerJS(this);

        public PointerJS Initialize()
        {
            return this;
        }


        public string ElementPointer()
        {
            var source = Source == null?"document": Source.ToString();
            Multiple = false;
            switch (Mode)
            {
                case PointerMode.Id:
                    return source + ".getElementById(" + CreateString(Pointer) + ")";
                case PointerMode.Name:
                    return source + ".getElementsByName(" + CreateString(Pointer) + ")[0]";
                case PointerMode.Tag:
                    return source + ".getElementsByTagName(" + CreateString(Pointer) + ")[0]";
                case PointerMode.Class:
                    return source + ".getElementsByClassName(" + CreateString(Pointer) + ")[0]";
                case PointerMode.Location:
                    return source + ".elementFromPoint(" + CreateString(Pointer) + ")";
                case PointerMode.Query:
                    return source + ".querySelector(" + CreateString(Pointer) + ")";
                case PointerMode.XPath:
                    return source + ".evaluate(" + CreateString(Pointer) + ", document, null, XPathResult.FIRST_ORDERED_NODE_TYPE, null).singleNodeValue";
                case PointerMode.Pure:
                default:
                    return Pointer;
            }
        }
        public string ElementsPointer()
        {
            var source = Source ==null?"document": Source.ToString();
            Multiple = false;
            switch (Mode)
            {
                case PointerMode.Id:
                    return "["+source + ".getElementById(" + CreateString(Pointer) + ")]";
                case PointerMode.Name:
                    return source + ".getElementsByName(" + CreateString(Pointer) + ")";
                case PointerMode.Tag:
                    return source + ".getElementsByTagName(" + CreateString(Pointer) + ")";
                case PointerMode.Class:
                    return source + ".getElementsByClassName(" + CreateString(Pointer) + ")";
                case PointerMode.Location:
                    return source + ".elementsFromPoint(" + CreateString(Pointer) + ")";
                case PointerMode.Query:
                    return source + ".querySelectorAll(" + CreateString(Pointer) + ")";
                case PointerMode.XPath:
                    return "Array.from((function*(){ let iterator = "+source + ".evaluate(" + CreateString(Pointer) + ", document, null, XPathResult.UNORDERED_NODE_ITERATOR_TYPE, null); let current = iterator.iterateNext(); while(current){ yield current; current = iterator.iterateNext(); }  })())";
                default:
                    return Pointer;
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
        public PointerJS Format(string format = "{0}", params string[] otherArgs) => new PointerJS(this, string.Format(format, new string[] { ToString() }.Concat(otherArgs).ToArray()));

        public async Task<object> PerformAsync(params object[] args) => await ProcessService.RunTask<object, object>(o => Perform(args));
        public Task PerformTask(params object[] args) => ProcessService.RunTask(() => Perform(args));
        public Thread PerformThread(params object[] args) => ProcessService.Run(() => Perform(args));
        public Form PerformDialog(string message = "Wait until finish the process...", params object[] args) => ProcessService.RunDialog(message, (o, a) => Perform(args));      
        public T TryPerform<T>(T defaultValue = default, params object[] args)
        {
            var o = Perform(args);
            if (o is T)
                return (T)o;
            else return defaultValue;
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
        public virtual object Perform(params object[] args) => Execute(ToString(), args);
        public virtual PointerJS PerformPointer(params object[] args)
        {
            var pName = "pointer_" + DateTime.Now.Ticks;
            var val = As(pName).Perform(args);
            return new PointerJS(pName,Execute, PointerMode.Pure);
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
        public PointerJS Select() => Select(this);
        public virtual PointerJS Select(PointerJS pointer)
        {
            Pointer = pointer.Pointer;
            Mode = pointer.Mode;
            Execute = pointer.Execute;
            Multiple = pointer.Multiple;
            Source = pointer.Source??Source;
            Script = null;
            return this;
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
        public PointerJS From() => From(this);
        public virtual PointerJS From(PointerJS pointer)
        {
            Source = pointer;
            return this;
        }


        public virtual PointerJS All()=> new PointerJS(this, true);
        public virtual PointerJS One()=> new PointerJS(this, false);
        public virtual PointerJS The(int index = 0)=> All().On("["+ index + "]");
        public virtual PointerJS First()=> All().On("[0]");
        public virtual PointerJS Last()=> All().On(".slice(-1).pop()");
        public virtual PointerJS Reverse() => On(".reverse()");
        public virtual PointerJS Slice(int index = 0, int? length = null) => On(".slice(" + index + (length == null ? ")" : $", {length})"));

        public PointerJS On(PointerJS nextPointer) => On(nextPointer.ToString());
        public virtual PointerJS On(string nextCode) => Format("{0}{1}", nextCode);

        public PointerJS Follows(PointerJS nextPointer) => Follows(nextPointer.ToString());
        public virtual PointerJS Follows(string nextCode) => Format("{0};{1}", nextCode);
        public PointerJS Follows() => Format("{0};");

        public PointerJS Prepend(PointerJS pointer) => Prepend(pointer.ToString());
        public virtual PointerJS Prepend(string code) => new PointerJS(this, code + ToString());
        public PointerJS Append(PointerJS pointer) => Append(pointer.ToString());
        public virtual PointerJS Append(string code) => new PointerJS(this, ToString() + code);

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

        public PointerJS Join(PointerJS pointer) => Join(pointer.ToString());
        public virtual PointerJS Join(string code) => Format("{0},{1}", code);
        public virtual PointerJS Join() => Format("{0},");
        public PointerJS Join(string name, PointerJS pointer) => Join(name, pointer.ToString());
        public virtual PointerJS Join(string name,string code) => Format("{0},{1}:{2}", CreateString(name), code);
        public PointerJS Collect() => Format("{{0}}");
        public PointerJS Array() => Format("[{0}]");
        public PointerJS Then(PointerJS pointer) => Then(pointer.ToString());
        public virtual PointerJS Then(string code) => Format("{0}(()=>{{{1}}})()", code);
        public virtual PointerJS Then() => Format("(()=>{{{0}}})()");

        /// <summary>
        /// There should be a yield code in the Script
        /// </summary>
        /// <returns></returns>
        public virtual PointerJS Iterate() => Format("Array.from((function*(){{{0}}})())");
        public PointerJS Yield(PointerJS pointer) => Yield(pointer.ToString());
        public virtual PointerJS Yield(string code) => Format("{0}; yield {1}",code);
        public virtual PointerJS Yield() => Format(" yield {0}");
        public PointerJS Return(PointerJS pointer) => Return(pointer.ToString());
        public virtual PointerJS Return(string code) => Format("{0}; return {1}", code);
        public virtual PointerJS Return() => Format(" return {0}");

        public PointerJS If(PointerJS pointer) => If(pointer.ToString());
        public virtual PointerJS If(string conditionCode) => Format("if({1}) ", conditionCode).Then(this);
        public virtual PointerJS If() => Format("if({0}) ");
        public PointerJS Else(PointerJS pointer) => Else(pointer.ToString());
        public virtual PointerJS Else(string conditionCode) => Else().Then(conditionCode);
        public virtual PointerJS Else() => Format("{0}; else ");
        public PointerJS Where(PointerJS pointer) => Where(pointer.ToString());
        public virtual PointerJS Where(string conditionCode) => Format("({1})? ", conditionCode).On(this);
        public virtual PointerJS Where() => Format("({0})? ");
        public PointerJS ElseWhere(PointerJS pointer) => ElseWhere(pointer.ToString());
        public virtual PointerJS ElseWhere(string code) => ElseWhere().On(code);
        public virtual PointerJS ElseWhere() => Format("{0} : ");
        public PointerJS While(PointerJS pointer) => While(pointer.ToString());
        public virtual PointerJS While(string conditionCode) => Format("while({1}) ", conditionCode).Then(this);
        public virtual PointerJS While() => Format("while({0}) ");
        public PointerJS ForEach(string elementName, PointerJS pointer) => ForEach(elementName, pointer.ToString());
        public virtual PointerJS ForEach(string elementName,string collectionCode) => Format("for(let {1} of {2}) ", elementName, collectionCode).Then(this);
        public virtual PointerJS ForEach(string elementName) => Format("for(let {1} of {0}) ", elementName);
        public virtual PointerJS ForEach() => Format("for(let {1} of {0}) {1}", "element");
        public PointerJS ForIn(string elementName, PointerJS pointer) => ForIn(elementName, pointer.ToString());
        public virtual PointerJS ForIn(string elementName, string collectionCode) => Format("for(let {1} in {2}) ", elementName, collectionCode).Then(this);
        public virtual PointerJS ForIn(string elementName) => Format("for(let {1} in {0}) ", elementName);
        public virtual PointerJS ForIn() => Format("for(let {1} in {0}) {1}", "element");

        public PointerJS As(string elementName, PointerJS nextPointer) => As(elementName, nextPointer.ToString());
        public virtual PointerJS As(string elementName,string code) => Format("(({1})=>{2})({0})", elementName, code);
        public virtual PointerJS As(string elementName) => Format("{1} = (()=>{{{0}}})()", elementName);
        public virtual PointerJS Var(string elementName) => Format("var {1} = {0};", elementName);
        public virtual PointerJS Let(string elementName) => Format("let {1} = {0};", elementName);
        public virtual PointerJS Const(string elementName) => Format("const {1} = {0};", elementName);
        public virtual PointerJS Named(string elementName) => Format("{1}:{0}", elementName);

        public PointerJS And(PointerJS pointer) => And(pointer.ToString());
        public virtual PointerJS And(string code = "true") => Format("({0} && {1})", code);
        public PointerJS Or(PointerJS pointer) => Or(pointer.ToString());
        public virtual PointerJS Or(string code = "true") => Format("({0} || {1})", code);

        public virtual PointerJS Null() => Format("{0} null");
        public virtual PointerJS Nothing() => Format("{0} (()=>{{}})()");

        public PointerJS Not(PointerJS pointer) => Not(pointer.ToString());
        public virtual PointerJS Not(string code) => Format("({0} !== {1})", code);
        public virtual PointerJS Not() => Format("(!{0})");

        public PointerJS Is(PointerJS pointer) => Is(pointer.ToString());
        public virtual PointerJS Is(string code) => Format(" === {1}", code);
        public PointerJS IsEquals(PointerJS pointer) => IsEquals(pointer.ToString());
        public virtual PointerJS IsEquals(string code) => Format("({0} === {1})", code);

        public virtual PointerJS IsVisible() => IsHidden().Not();
        public virtual PointerJS IsHidden() => As("element", "element === null || element === undefined || element.offsetLeft < 0").Or(GetStyle().As("element","element.visibility === 'hidden' || element.display === 'none'"));
        public virtual PointerJS IsExists() => As("element", "element !== null && element !== undefined");   
        public virtual PointerJS IsUndefined() => Is("undefined");
        public virtual PointerJS IsNull() => Is("null");

        public virtual PointerJS Count() => On("Array.from({0}).length");

        public virtual PointerJS SendKeys(string keys) => Scroll().Follows(InvokeKeyboardEvent(keys, "keydown"));
        public virtual PointerJS SendText(string text) => Scroll().Follows(InvokeKeyboardEvent(ConvertService.ToHotKeys(text), "keydown"));
        public virtual PointerJS Scroll() => On(".scrollIntoView({ behavior: 'smooth', block: 'end'})");
        public virtual PointerJS ScrollTo(PointerJS pointer) => ScrollX(pointer).Follows(ScrollY(pointer));
        public virtual PointerJS ScrollTo(string codeX, string codeY) => ScrollX(codeX).Follows(ScrollY(codeY));
        public virtual PointerJS ScrollTo(int x, int y) => ScrollX(x).Follows(ScrollY(y));
        public virtual PointerJS ScrollX(PointerJS pointer) => On(".scrollLeft").Set(pointer.Clone().PositionX());
        public virtual PointerJS ScrollX(string code) => On(".scrollLeft").Set(code);
        public virtual PointerJS ScrollX(int x) => On(".scrollLeft").Set(x);
        public virtual PointerJS ScrollY(PointerJS pointer) => On(".scrollTop").Set(pointer.Clone().PositionY());
        public virtual PointerJS ScrollY(string code) => On(".scrollTop").Set(code);
        public virtual PointerJS ScrollY(int y) => On(".scrollTop").Set(y);
        public virtual PointerJS Position() => PositionX().Join(PositionY()).Array();
        public virtual PointerJS PositionX() => On(".offsetLeft");
        public virtual PointerJS PositionY() => On(".offsetTop");
        public virtual PointerJS Flow() => On(".blur()");
        public virtual PointerJS Focus() => On(".focus()");
        public virtual PointerJS Submit() => Scroll().Follows(On(".submit()"));
        public virtual PointerJS Click() => Scroll().Follows(On(".click()"));// As("element", "element.scrollIntoView(); element.click();");
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
            return InvokeEvent("keyboardEvent",eventName,"null","char").ForEach("char", CreateString(keys)+ ".split('')");
        }
        public virtual PointerJS InvokeEvent(string eventName) => InvokeEvent("Event", eventName);
        public virtual PointerJS InvokeEvent(string eventType, string eventName, params string[] otherArgs)
        {
            return On(".dispatchEvent(evt);").Prepend(string.Join("",
                "var evt  = document.createEvent(`", eventType, "`);",
                "evt.init" + eventType + "(", CreateString(eventName), ", true, true" + (otherArgs.Length>1 ? ", "+string.Join(", ", otherArgs) :"") + ");"));
        }
        public virtual PointerJS InvokeEvents(string eventType, string eventName, IEnumerable<string[]> otherArgsList)
        {
            var p = On(".dispatchEvent(evt);").Prepend(string.Join("", "var evt  = document.createEvent(", CreateString(eventType), "`);"));
            foreach (var otherArgs in otherArgsList)
                p.On(string.Join("", "evt.init" + eventType + "(", CreateString(eventName), ", true, true" + (otherArgs.Length > 1 ? ", " + string.Join(", ", otherArgs) : "") + ");"));
            return p;
        }


        public virtual PointerJS NodeName() => On(".nodeName");
        public virtual PointerJS NodeType() => On(".nodeType");
        public virtual PointerJS NodeValue() => On(".nodeValue");
        public virtual PointerJS NextNode() => On(".nextSibling");
        public virtual PointerJS PreviousNode() => On(".previousSibling");
        public virtual PointerJS ParentNode() => On(".parentNode");
        public virtual PointerJS NormalizeNode() => On(".normalize()");
        public virtual PointerJS CloneNode(bool withChildren = true) => On(".cloneNode(" + (withChildren + "").ToLower() + ")");


        public virtual PointerJS Replace(PointerJS pointer) => Parent().On(".replaceChild(" + pointer.ToString() + ","+ToString()+")");
        public virtual PointerJS Remove() => On(".remove()");
        public virtual PointerJS Closest(string query) => On(".closest(" + CreateString(query) + ")");
        public virtual PointerJS Matches(string query) => On(".matches(" + CreateString(query) + ")");
        public virtual PointerJS Next() => On(".nextElementSibling");
        public virtual PointerJS Previous() => On(".previousElementSibling");
        public virtual PointerJS Parent() => On(".parentElement");
        public virtual PointerJS Children() => On(".children");
        public virtual PointerJS Child(int index) => Children().On("[" + index + "]");

        public virtual PointerJS this[int index] { get => Get(index); set => Set(index,value); }
        public virtual PointerJS this[string name] { get => Get(name); set => Set(name, value); }

        public virtual PointerJS Get() => new PointerJS(this);
        public virtual PointerJS Get(int index) => On("[" + index + "]");
        public virtual PointerJS Get(string name) => On("[" + CreateString(name) + "]");
        public virtual PointerJS Set(PointerJS pointer) => On(" = " + pointer.ToString());
        public virtual PointerJS Set(string value) => On(" = " + CreateString(value));
        public virtual PointerJS Set(object value) => On(" = " + (value is string ? CreateString(value) : value + ""));
        public PointerJS Set(int index, PointerJS pointer) => Get(index).Set(pointer);
        public PointerJS Set(string name, PointerJS pointer) => Get(name).Set(pointer);
        public PointerJS Set(int index, string value) => Get(index).Set(value);
        public PointerJS Set(string name, string value) => Get(name).Set(value);
        public PointerJS Set(int index, object value) => Get(index).Set(value);
        public PointerJS Set(string name, object value) => Get(name).Set(value);

        public virtual PointerJS GetParent() => On(".parentElement");
        public virtual PointerJS SetParent(PointerJS pointer) => GetParent().Set(pointer);
        public virtual PointerJS GetChild(int index) => Children().Get(index);
        public virtual PointerJS SetChild(int index,PointerJS pointer) => GetChild(index).Set(pointer);
        public virtual PointerJS ReplaceChild(int index,PointerJS pointer) => As("element", "element.replaceChild("+pointer.ToString()+",element.children[" + index + "])");
        public virtual PointerJS RemoveChild(PointerJS pointer) => On(".removeChild("+ pointer.ToString() + ")");
        public virtual PointerJS RemoveChild(int index) => As("element", "element.removeChild(element.children[" + index + "])");
        public virtual PointerJS HasChild() => On(".hasChildNodes()");
        public virtual PointerJS HasChild(PointerJS pointer) => On(".contains(" + pointer.ToString() + ")");
        public virtual PointerJS HasChild(int index) => Children().On(".length>"+ index);
        public virtual PointerJS GetAttribute(string name) => On(".getAttribute("+ CreateString(name) +")");
        public virtual PointerJS SetAttribute(string name, string value) => On(".setAttribute(" + CreateString(name) +","+ CreateString(value) +")");
        public virtual PointerJS RemoveAttribute(string name) => On(".removeAttribute(" + CreateString(name) +")");
        public virtual PointerJS HasAttribute(string attributeName) => On(".hasAttribute(" + CreateString(attributeName) + ")");
        public virtual PointerJS HasAttribute() => On(".hasAttributes()");
        public virtual PointerJS GetId() => On(".id");
        public virtual PointerJS SetId(string value) => GetId().Set(value);
        public virtual PointerJS GetName() => GetAttribute("name");
        public virtual PointerJS SetName(string value) => SetAttribute("name", value);
        public virtual PointerJS GetTitle() => On(".title");
        public virtual PointerJS SetTitle(string value) => GetTitle().Set(value);
        public virtual PointerJS GetContent() => On(".textContent");
        public virtual PointerJS SetContent(string text) => GetContent().Set(text);
        public virtual PointerJS GetText() => On(".innerText");
        public virtual PointerJS SetText(string text) => GetText().Set(text);
        public virtual PointerJS GetValue() => As("elem","elem.value??elem.innerText");
        public virtual PointerJS SetValue(string value) => As("elem", "{try{elem.value = " + CreateString(value) + ";}catch{elem.innerText = "+ CreateString(value) +";}}");
        public virtual PointerJS GetInnerHTML() => On(".innerHTML");
        public virtual PointerJS SetInnerHTML(string html) => GetInnerHTML().Set(html);
        public virtual PointerJS GetOuterHTML() => On(".outerHTML");
        public virtual PointerJS SetOuterHTML(string html) => GetOuterHTML().Set(html);
        public virtual PointerJS GetStyle() => Format("window.getComputedStyle({0})");
        public virtual PointerJS SetStyle(string style) => On(".style = " + style);
        public virtual PointerJS GetStyle(string property) => On(".style."+ ConvertService.ToConcatedName(property.ToLower()));
        public virtual PointerJS SetStyle(string property, object value) => GetStyle(property).Set(value);
        public virtual PointerJS GetShadowRoot() => On(".shadowRoot");
        public virtual PointerJS SetShadowRoot(string mode="closed") => Format(".attachShadow({{mode:{1}}})", CreateString(mode));


        public virtual string CreateString(object value = null) => value == null? "null": string.Join("","`", (value+"").Replace("`","\\`"), "`");

        public override string ToString()
        {
            if (Multiple) return string.Join("",
                "Array.from((function*(elements) { for(let element of elements) yield (()=>",
                    string.IsNullOrWhiteSpace(Script)? "element": Script,
                ")()})(", ElementsPointer(), "))"
            );
            else return string.IsNullOrWhiteSpace(Script) ?ElementPointer() : Script;
        }

        public IEnumerator<PointerJS> GetEnumerator()
        {
            var pointer = PerformPointer();
            int index = 0;
            while (pointer.Get(index).IsExists().Return().TryPerform(false))
                yield return pointer.Get(index++);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            var pointer = PerformPointer();
            int index = 0;
            while (pointer.Get(index).IsExists().Return().TryPerform(false))
                yield return pointer.Get(index++);
        }

        public static implicit operator bool(PointerJS pointer) => pointer.TryPerform(false);
        public static implicit operator string(PointerJS pointer) => pointer.TryPerform("");
        public static implicit operator short(PointerJS pointer) => pointer.TryPerform<short>(0);
        public static implicit operator int(PointerJS pointer) => pointer.TryPerform(0);
        public static implicit operator long(PointerJS pointer) => pointer.TryPerform(0l);
        public static implicit operator float(PointerJS pointer) => pointer.TryPerform(0F);
        public static implicit operator double(PointerJS pointer) => pointer.TryPerform(0d);
        public static implicit operator decimal(PointerJS pointer) => pointer.TryPerform(0m);
    }
}
