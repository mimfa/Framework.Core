using MiMFa.Exclusive.ProgramingTechnology.CommandLanguage;
using MiMFa.General;
using MiMFa.Model;
using MiMFa.Model.IO;
using MiMFa.Service;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MiMFa.Exclusive.View
{
    public abstract class ToStringBase
    {
        public virtual string SignTranslate { get; set; } = "";
        public virtual bool Translate { get; set; } = false;
        public virtual bool InternalParameterTranslate
        {
            get { return SignTranslate.Contains("¶"); }
            set
            {
                if (Translate = value) SignTranslate = "¶"+ SignTranslate;
                else SignTranslate.Replace("¶", "");
            }
        }
        public virtual bool TryTranslate
        {
            get { return SignTranslate.Contains("§"); }
            set
            {
                if (Translate = value) SignTranslate = "§"+ SignTranslate;
                else SignTranslate.Replace("§", "");
            }
        }
        public virtual bool NoTranslate
        {
            get { return SignTranslate.Contains("▬"); }
            set
            {
                if (Translate = value) SignTranslate = "▬"+ SignTranslate;
                else SignTranslate.Replace("▬", "");
            }
        }
        public virtual bool FullTranslate
        {
            get { return SignTranslate.Contains("↨"); }
            set
            {
                if (Translate = value) SignTranslate = "↨"+ SignTranslate;
                else SignTranslate.Replace("↨", "");
            }
        }

        public bool AllowMime = false;
        public bool AllowChainedFile = true;
        public bool AllowDictionary = true;
        public bool AllowKeyValuePair = true;
        public bool AllowCollection = true;
        public bool AllowDataTable = true;
        public bool AllowDataRow = true;
        public bool AllowEnum = true;
        public bool AllowBitmap = true;
        public bool AllowByte = true;
        public bool AllowEventPack = true;
        public bool AllowString = true;
        public bool AllowUri = true;

        public virtual string TemporaryDirectory => MiMFa.Config.TemporaryDirectory;
        public virtual string StartSign { get; set; } = "";
        public virtual string MiddleSign { get; set; } = "=";
        public virtual string PointerSign { get; set; } = "->";
        public virtual string EndSign { get; set; } = "";
        public virtual string SplitSign { get; set; } = ", ";
        public virtual string TabSign { get; set; } = "\t ";
        public virtual string BreakSign { get; set; } = Environment.NewLine;
        public virtual string BreakLineSign { get; set; } = "----------------------------------" + Environment.NewLine;
        public virtual string Table_ { get; set; } = "----------------------------------" + Environment.NewLine;
        public virtual string TableRow_ { get; set; } = "_________________________________" + Environment.NewLine;
        public virtual string TableCell_ { get; set; } = "";
        public virtual string _TableCell { get; set; } = "\t\t";
        public virtual string _TableRow { get; set; } = Environment.NewLine + "_________________________________";
        public virtual string _Table { get; set; } = Environment.NewLine + "----------------------------------";
        public virtual Func<string, string> Highlight { get; set; } = (s) => " «" + s + "» ";

        public virtual object TryDone(object obj)
        {
            if (obj == null) return "";
            Type t = obj.GetType();
            string tn = t.Name.ToLower();

            if (AllowDictionary &&( t is IDictionary || tn.StartsWith("dictionary") || tn.StartsWith("mimfa_dictionary")))
                return Done(ConvertService.ToDictionary(obj));
            if (AllowDictionary && (tn.StartsWith("mimfa_matrix")))
                return Done(ConvertService.ToMiMFaMatrix(obj));
            if (AllowKeyValuePair && tn.StartsWith("keyvaluepair"))
                return Done(ConvertService.ToKeyValuePair(obj));
            if (AllowDataTable && tn.StartsWith("datatable"))
                return Done((DataTable)obj);
            if (AllowDataRow && tn.StartsWith("datarow"))
                return Done((DataRow)obj);
            if (AllowEnum && obj is Enum)
                return Done(obj + "");
            if (AllowBitmap && obj is Bitmap)
                return Done((Bitmap)obj);
            if (AllowUri && obj is Uri)
                return Done((Uri)obj);
            if (AllowEventPack && obj is EventPack)
                return Done((EventPack)obj);
            if (obj is DateTime)
                return Done((DateTime)obj);
            if (AllowByte && obj is byte)
                return Done((byte)obj);
            if (AllowString && obj is String)
                return Done(obj + "");
            if (AllowCollection && obj is MiMFa.Model.IO.ChainedFile)
                return Done((MiMFa.Model.IO.ChainedFile)obj);
            if (AllowCollection && obj is IEnumerable<string>)
                return Done((IEnumerable<string>)obj);
            if (AllowCollection && obj is IEnumerable<IEnumerable<string>>)
                return Done((IEnumerable<IEnumerable<string>>)obj);
            if (AllowCollection && obj is IEnumerable<IEnumerable<IEnumerable>>)
                return Done((IEnumerable<IEnumerable<IEnumerable>>)obj);
            if (AllowCollection && obj is IEnumerable<IEnumerable>)
                return Done((IEnumerable<IEnumerable>)obj);
            if (AllowCollection && obj is IEnumerable)
            {
                List<object> lo = new List<object>();
                foreach (var v in (dynamic)obj) lo.Add(v);
                return Done(lo);
            }
            if (double.NaN.Equals(obj))
                return "NaN";
            if (obj is Int64 || obj is long)
                return Done(Convert.ToInt64(obj));
            if (obj is UInt64 || obj is double)
                return Done(Convert.ToDouble(obj));
            if (obj is decimal)
                return Done(Convert.ToDecimal(obj));
            return Normalization(obj);
        }
        public virtual String Done(object obj, bool withStartAndEndSign = false)
        {
            return withStartAndEndSign? (StartSign + TryDone(obj)  + EndSign) : TryDone(obj) + "";
        }
        public virtual object Normalization(object obj)
        {
            return (AllowMime)?  DoneByMime(obj):obj;
        }
        private object DoneByMime(object arg)
        {
            string ext = InfoService.GetMimeObject(arg).Split('/').First().Trim().ToLower();
            try
            {
                switch (ext)
                {
                    case "image":
                        return Done(Image.FromStream(new System.IO.MemoryStream(IOService.Serialize(arg))));
                    default:
                        return arg;
                }
            }
            catch { return arg; }
        }
        public virtual String Done(string arg)
        {
            if (Translate) arg = Default.Translate(SignTranslate + (arg));
            return arg;
        }
        public virtual String Done(ChainedFile arg)
        {
            if (arg == null) return "";
            return string.Join(BreakSign, from item in arg.ReadRows() select Done(item));
        }
        public virtual String Done(IEnumerable<IEnumerable<IEnumerable>> arg)
        {
            if (arg == null) return "";
            return string.Join(BreakLineSign, from item in arg select Done(item));
        }
        public virtual String Done(IEnumerable<IEnumerable> arg)
        {
            if (arg == null) return "";
            return string.Join(BreakSign, from item in arg select Done(item));
        }
        public virtual String Done(IEnumerable<IEnumerable<string>> arg)
        {
            if (arg == null) return "";
            return string.Join(BreakSign, from item in arg select Done(item));
        }
        public virtual String Done(IEnumerable<string> arg)
        {
            if (arg == null) return "";
            return string.Join(SplitSign, from item in arg select item);
        }
        public virtual String Done(IEnumerable<object> arg)
        {
            if (arg == null) return "";
            return string.Join(TabSign, from item in arg select Done(item));
        }
        public virtual String Done<T>(Matrix<T> arg)
        {
            if (arg == null) return "";
            return string.Join(BreakSign, from item in arg select Done(item));
        }
        public virtual String Done<T, F>(IDictionary<T, F> arg)
        {
            if (arg == null) return "";
            return string.Join(BreakSign, from item in arg select Done(item));
        }
        public virtual String Done<T, F>(KeyValuePair<T, F> arg)
        {
            return string.Join(PointerSign, Done(arg.Key), Done(arg.Value));
        }
        //public virtual String Done<T>(Stack<T> arg)
        //{
        //    if (arg == null) return "";
        //    return string.Join(BreakSign, from item in arg select Done(item));
        //}
        //public virtual String Done<T>(Queue<T> arg)
        //{
        //    if (arg == null) return "";
        //    return string.Join(BreakSign, from item in arg select Done(item));
        //}
        public virtual String Done(DataTable arg)
        {
            if (arg == null) return "";
            string str = "";
            str += TableRow_;
            foreach (DataColumn item in arg.Columns)
                str += TableCell_ + Done(item.ColumnName) + _TableCell;
            str += _TableRow;
            foreach (DataRow item in arg.Rows)
                str += Done(item);
            return Table_ + str + _Table;
        }
        public virtual String Done(DataRow arg)
        {
            if (arg == null) return "";
            return TableRow_ + TableCell_ +
                string.Join(_TableCell + TableCell_, from item in arg.ItemArray select Done(item))
                + _TableCell + _TableRow;
        }
        public virtual String Done(DateTime arg)
        {
            if (arg == null) return "";
            return arg.ToString("yyyy/MM/dd HH:mm:ss:FFFFFFF");
        }
        public virtual String Done(EventPack arg)
        {
            return arg.Target;
        }
        public virtual String Done(Bitmap arg)
        {
            string address = TemporaryDirectory + DateTime.Now.Ticks + ".jpg";
            arg.Save(address);
            System.Diagnostics.Process.Start(address);
            return null;
        }
        public virtual String Done(Uri arg)
        {
            return arg.OriginalString;
        }
        public virtual String Done(byte[] arg)
        {
            if (arg == null) return "";
            string ext = "data";
            try { ext = InfoService.GetMimeObject(arg).Split('/').Last().Trim().Split(' ').First(); } catch { }
            string address = TemporaryDirectory + DateTime.Now.Ticks + "." + ext; System.IO.File.WriteAllBytes(address,arg);
            System.Diagnostics.Process.Start(address);
            return null;
        }
        public virtual String Done(byte arg)
        {
            return arg + "";
        }
        public virtual String Done(long arg)
        {
            return arg + "";
        }
        public virtual String Done(double arg)
        {
            if (Double.IsPositiveInfinity(arg)) return "∞";
            if (Double.IsNegativeInfinity(arg)) return "-∞";
            return arg + "";
        }
        public virtual String Done(decimal arg)
        {
            return arg + "";
        }

        public virtual bool CaseSensitiveSearch { get; set; } = false;
        public virtual object TrySearch(object inObj, bool showAll, bool highlight, params object[] theseObjects)
        {
            if (showAll && !highlight) return inObj;
            if (inObj == null) return "";
            string tn = inObj.GetType().Name.ToLower();
            try
            {
                if (AllowDictionary && inObj is IDictionary)
                    return Search(ConvertService.ToDictionary(inObj), showAll, highlight, theseObjects);
                if (AllowKeyValuePair && tn.StartsWith("keyvaluepair"))
                    return Search(ConvertService.ToKeyValuePair(inObj), showAll, highlight, theseObjects);
                if (AllowString && inObj is String)
                    return Search(inObj + "", showAll, highlight, theseObjects); 
                if (AllowCollection && inObj is IEnumerable)
                    return Search((IEnumerable)inObj, showAll, highlight, theseObjects);
                if (AllowDataTable && inObj is  DataTable)
                    return Search((DataTable)inObj, showAll, highlight, theseObjects);
                if (AllowDataRow && inObj is DataRow)
                    return Search((DataRow)inObj, showAll, highlight, theseObjects);
                if (AllowEnum && inObj is Enum)
                    return Search(inObj + "", showAll, highlight, theseObjects);
                if (AllowUri && inObj is Uri)
                    return Search((Uri)inObj, showAll, highlight, theseObjects);
                object o = null;
                foreach (var item in theseObjects)
                    if (o == null) o = Find(inObj, highlight, theseObjects);
                    else return o;
            }
            catch
            {
            }
            return (showAll) ? inObj : null;
        }
        private object Find(object arg, bool highlight, object thisobj)
        {
            if (arg == null) return null;
            if (!(arg is string))
                return (arg == thisobj)? arg:null;
            string narg = (CaseSensitiveSearch) ? (arg + "") : (arg + "").ToLower();
            string nthisobj = (CaseSensitiveSearch) ? (thisobj + "") : (thisobj + "").ToLower();
            if (!highlight) return (narg.Contains(nthisobj))?arg:null;
            return (narg.Contains(nthisobj))? (arg+"").Replace(thisobj + "",Highlight(thisobj + "")):null;
        }
        public virtual string Search(string arg, bool showAll, bool highlight, object[] theseObjects)
        {
            foreach (var item in theseObjects)
            {
                object a = Find(arg, highlight, item + "");
                if (a != null) return a+"";
            }
            return (showAll)?arg:null;
        }
        public virtual IEnumerable Search(IEnumerable arg, bool showAll, bool highlight, object[] theseObjects)
        {
            if (arg == null) yield break;
            foreach (var item in arg)
            {
                object a = TrySearch(item, showAll, highlight, theseObjects);
                if (a != null) yield return a;
            }
        }
        public virtual IDictionary<T, F> Search<T, F>(IDictionary<T, F> arg, bool showAll, bool highlight, object[] theseObjects)
        {
            if (arg == null) return null;
            IDictionary<T, F> ta = new Dictionary<T, F>();
            foreach (var item in arg)
            {
                object a = TrySearch(item.Key, showAll, highlight, theseObjects);
                if (a != null) ta.Add((T)a, item.Value);
                else
                {
                    a = TrySearch(item.Value, showAll, highlight, theseObjects);
                    if (a != null) ta.Add(item.Key, (F)a);
                }
            }
            return ta;
        }
        public virtual KeyValuePair<T, F> Search<T, F>(KeyValuePair<T, F> arg, bool showAll, bool highlight, object[] theseObjects)
        {
            object a = TrySearch(arg.Key, showAll, highlight, theseObjects);
            if (a != null) return new KeyValuePair<T, F>((T)a, arg.Value);
            else
            {
                a = TrySearch(arg.Value, showAll, highlight, theseObjects);
                if (a != null) return new KeyValuePair<T, F>(arg.Key, (F)a);
            }
            return arg;
        }
        public virtual DataTable Search(DataTable arg, bool showAll, bool highlight, object[] theseObjects)
        {
            if (arg == null) return null;
            DataTable ta = arg.Clone();
            for (int i = 0; i < arg.Rows.Count; i++)
            {
                object a = Search(arg.Rows[i], showAll, highlight, theseObjects);
                if (a != null) ta.Rows.Add((a as DataRow).ItemArray);
            }
            return ta;
        }
        public virtual DataRow Search(DataRow arg, bool showAll, bool highlight, object[] theseObjects)
        {
            if (arg == null) return null;
            for (int i = 0; i < arg.ItemArray.Length; i++)
                if ((arg.ItemArray[i] = TrySearch(arg.ItemArray[i], showAll, highlight, theseObjects)) != null)
                    return arg;
            return null;
        }
        public virtual Uri Search(Uri arg, bool showAll, bool highlight, object[] theseObjects)
        {
            string a = null;
            foreach (var item in theseObjects)
                if (a == null) a = Find(arg.OriginalString, highlight, theseObjects) + "";
                else return arg;
            return null;
        }
    }
}
