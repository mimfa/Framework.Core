using MiMFa.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MiMFa.Model.IO.Connector
{
    public abstract class ConnectorBase
    {
        public virtual string Path { get; set; }
        public virtual Encoding Encoding { get; set; } = Encoding.UTF8;

        public bool DefaultConfig = true;
        public char QuoteChar
        {
            get => QuoteChars.Length > 0 ? QuoteChars.First() : '\"';
            set => QuoteChars = new char[] { value };
        }
        public char[] QuoteChars
        {
            get { return _QuoteChars; }
            set
            {
                _QuoteChars = value;
                UpdateDetectors();
            }
        }
        private char[] _QuoteChars = new char[] { '\"' };
        public char MetaChar
        {
            get => _MetaChar; set
            {
                _MetaChar = value;
                UpdateDetectors();
            }
        }
        private char _MetaChar = '\\';
        public string MetaLineFeedChar { get;  set; }
        public string MetaCarriageReturnChar { get;  set; }
        public string MetaLineBreakChar { get;  set; }
        public string WarpsSplitter
        {
            get => WarpsSplitters.Length > 0 ? WarpsSplitters.First() : "\t";
            set => WarpsSplitters = new string[] { value };
        }
        public string[] WarpsSplitters
        {
            get => _WarpsSplitters;
            set
            {
                _WarpsSplitters = value;
                UpdateDetectors();
            }
        }
        private string[] _WarpsSplitters = { "\t" };
        public string LinesSplitter
        {
            get => LinesSplitters.Length > 0 ? LinesSplitters.First() : Environment.NewLine;
            set => LinesSplitters = new string[] { value };
        }
        public string[] LinesSplitters
        {
            get => _LinesSplitters;
            set
            {
                _LinesSplitters = value;
                UpdateDetectors();
            }
        }
        private string[] _LinesSplitters = { Environment.NewLine, "\n" };

        public Regex QuoteDetector { get; set; }
        public Regex StartQuoteCharDetector { get; set; }
        public Regex EndQuoteCharDetector { get; set; }
        public Regex LineDetector { get; set; }
        public Regex WarpDetector { get; set; }
   
        public void UpdateDetectors()
        {
            MetaLineFeedChar = string.Join("", _MetaChar, "n");
            MetaCarriageReturnChar = string.Join("", _MetaChar, "r");
            MetaLineBreakChar = string.Join("", _MetaChar, "r", _MetaChar, "n");

            QuoteDetector = new Regex(string.Join("|", from v in _QuoteChars let q = Regex.Escape(v.ToString()) select string.Format("(?<=([^{0}]{1})|^{1})([^{1}]|({0}{1}))*[^{0}](?={1})", Regex.Escape(_MetaChar.ToString()), q)));
            StartQuoteCharDetector = new Regex(string.Join("|", from v in _QuoteChars let q = Regex.Escape(v.ToString()) select string.Format(@"((?<=({2})){1})|(^{1})", Regex.Escape(_MetaChar.ToString()), q,string.Join(")|(",from v in WarpsSplitters select Regex.Escape(v)))));
            EndQuoteCharDetector = new Regex(string.Join("|", from v in _QuoteChars let q = Regex.Escape(v.ToString()) select string.Format(@"((?<![^{0}]{0}){1}(?=({2})))|((?<![^{0}]{0}){1}$)", Regex.Escape(_MetaChar.ToString()), q,string.Join(")|(",from v in WarpsSplitters select Regex.Escape(v)))));

            LineDetector = new Regex(string.Join("|", from v in _LinesSplitters let q = Regex.Escape(v) select string.Format("([^{0}]{1})|^{1}", Regex.Escape(_MetaChar.ToString()), q)));
            WarpDetector = new Regex(string.Join("|", from v in _WarpsSplitters let q = Regex.Escape(v) select string.Format("([^{0}]{1})|^{1}", Regex.Escape(_MetaChar.ToString()), q)));
        }
        public string EscapeChars(string text)
        {
            text = text.Replace("\r", MetaCarriageReturnChar).Replace("\n", MetaLineFeedChar);
            foreach (var spl in _QuoteChars) text = text.Replace(spl.ToString(), string.Join("", _MetaChar, spl));
            return text;
        }
        public string UnescapeChars(string text)
        {
            foreach (var spl in QuoteChars) text = text.Replace(string.Join("", MetaChar, spl), spl.ToString());
            return text.Replace(MetaLineFeedChar, "\n").Replace(MetaCarriageReturnChar, "\r");
        }



        public ConnectorBase(string path, Encoding encoding = null)
        {
            Path = string.IsNullOrWhiteSpace(path)? CreatePath() : path;
            Encoding = encoding ?? Encoding;
            UpdateDetectors();
        }

        public virtual bool CreateNew()
        {
            UpdateDetectors();
            return true;
        }
        public virtual string CreatePath() => PathService.CreateValidPathName(Config.TemporaryDirectory ?? Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), DateTime.Now.Ticks + "", ".tsv", false);

        public virtual string[][] ReadAllColumns() => (from v in ReadColumns() select v.ToArray()).ToArray();
        public virtual string[] ReadAllWarps() => ReadWarps().ToArray();
        public virtual string[][] ReadAllRows() =>( from v in ReadRows() select v.ToArray()).ToArray();
        public virtual string[] ReadAllLines() => ReadLines().ToArray();
        public virtual IEnumerable<IEnumerable<string>> ReadColumns()
        {
            return RowsToColumns(ReadRows());
        }
        public virtual IEnumerable<string> ReadWarps()
        {
            return ColumnsToWarps(ReadColumns());
        }
        public virtual IEnumerable<IEnumerable<string>> ReadRows()
        {
            return LinesToRows(ReadLines());
        }
        public virtual IEnumerable<string> ReadLines()
        {
            return RowsToLines(ReadRows());
        }
        public virtual string ReadText()
        {
            return LinesToText(ReadLines(), LinesSplitter);
        }

        public virtual int WriteAllColumns(string[][] cols) => WriteColumns(cols.AsEnumerable());
        public virtual int WriteAllWarps(string[] warps) => WriteWarps(warps.AsEnumerable());
        public virtual int WriteAllRows(string[][] rows) => WriteRows(rows.AsEnumerable());
        public virtual int WriteAllLines(string[] lines) => WriteLines(lines.AsEnumerable());
        public int WriteColumns(params IEnumerable<string>[] cols) => WriteColumns(cols.AsEnumerable());
        public virtual int WriteColumns(IEnumerable<IEnumerable<string>> cols)
        {
            return WriteRows(ColumnsToRows(cols));
        }
        public int WriteWarps(params string[] warps) => WriteWarps(warps.AsEnumerable());
        public virtual int WriteWarps(IEnumerable<string> warps)
        {
            return WriteColumns(WarpsToColumns(warps, WarpsSplitter));
        }
        public int WriteRows(params IEnumerable<string>[] rows)=>WriteRows(rows.AsEnumerable());
        public virtual int WriteRows(IEnumerable<IEnumerable<string>> rows)
        {
            return WriteLines(RowsToLines(rows,WarpsSplitter));
        }
        public int WriteLines(params string[] lines) => WriteLines(lines.AsEnumerable());
        public virtual int WriteLines(IEnumerable<string> lines)
        {
            return WriteRows(LinesToRows(lines,WarpsSplitters));
        }
        public virtual int WriteText(string text)
        {
            return WriteLines(TextToLines(text));
        }
        public int PrependLines(params string[] lines) => PrependLines(lines.AsEnumerable());
        public virtual int PrependLines(IEnumerable<string> lines)
        {
            int i = 0;
            WriteLines((from v in lines where ++i > 0 select v).Concat(ReadLines()));
            return i;
        }
        public virtual int PrependText(string text)
        {
            return WriteText(LinesToText(new string[]{ text, ReadText()}));
        }
        public int AppendLines(params string[] lines) => AppendLines(lines.AsEnumerable());
        public virtual int AppendLines(IEnumerable<string> lines)
        {
            int i = 0;
            WriteLines(ReadLines().Concat(from v in lines where ++i > 0 select v));
            return i;
        }
        public virtual int AppendText(string text)
        {
            return WriteText(LinesToText(new string[]{ReadText(), text}));
        }



        #region CONVERT
        public IEnumerable<string> LineToRow(string line)
        {
            return LineToRow(line, WarpsSplitters);
        }
        public virtual IEnumerable<string> LineToRow(string line,params string[] warpsSplitor)
        {
            return SplitText(line, warpsSplitor);
        }
        public  IEnumerable<IEnumerable<string>> LinesToRows(params string[] lines)
        {
            return LinesToRows(lines, WarpsSplitters);
        }
        public  IEnumerable<IEnumerable<string>> LinesToRows(IEnumerable<string> lines)
        {
            return LinesToRows(lines, WarpsSplitters);
        }
        public virtual IEnumerable<IEnumerable<string>> LinesToRows(IEnumerable<string> lines, params string[] warpsSplitters)
            => from v in lines select LineToRow(v, warpsSplitters);
        public virtual IEnumerable<string> TextToLines(string text)
        {
            return SplitText(text, LinesSplitters);
        }
        public virtual IEnumerable<string> TextToWarps(string text) => from v in TextToColumns(text) select ColumnToWarp(v);
        public virtual IEnumerable<IEnumerable<string>> TextToRows(string text) => from v in TextToLines(text) select LineToRow(v);
        public virtual IEnumerable<IEnumerable<string>> TextToColumns(string text)
        {
            var rows = TextToRows(text).Select(v => v.ToList()).ToList();
            int j = -1;
            int con = rows.Max(r => r.Count);
            while (++j < con)
                yield return from v in rows select v.ElementAtOrDefault(j);
        }

        public virtual string JoinToLine(IEnumerable<string> cells)
        {
            return string.Join(WarpsSplitter, cells);
        }
        public  string RowToLine(params string[] cells)
        {
            return RowToLine(cells, WarpsSplitter);
        }
        public  string RowToLine(IEnumerable<string> cells)
        {
            return RowToLine(cells, WarpsSplitter);
        }
        public virtual string RowToLine(IEnumerable<string> cells, string delimited)
        {
            return string.Join(delimited, from cel in cells select PlainToStandard(cel, delimited));
        }
        public IEnumerable<string> RowsToLines(params IEnumerable<string>[] linesCells)
        {
            return RowsToLines(linesCells, WarpsSplitter);
        }
        public IEnumerable<string> RowsToLines(IEnumerable<IEnumerable<string>> linesCells)
        {
            return RowsToLines(linesCells, WarpsSplitter);
        }
        public virtual IEnumerable<string> RowsToLines(IEnumerable<IEnumerable<string>> linesCells, string warpSep) => from row in linesCells select RowToLine(row, warpSep);
        public string LinesToText(params string[] lines)
        {
            return LinesToText(lines, LinesSplitter);
        }
        public string LinesToText(IEnumerable<string> lines)
        {
            return LinesToText(lines, LinesSplitter);
        }
        public virtual string LinesToText(IEnumerable<string> lines, string sep)
        {
            return string.Join(sep, from line in lines select line.Contains(sep) ? QuoteChar + line + QuoteChar : line);
        }
        public string RowsToText(params IEnumerable<string>[] linesCells)
        {
            return RowsToText(linesCells, WarpsSplitter, LinesSplitter);
        }
        public string RowsToText(IEnumerable<IEnumerable<string>> linesCells)
        {
            return RowsToText(linesCells, WarpsSplitter, LinesSplitter);
        }
        public virtual string RowsToText(IEnumerable<IEnumerable<string>> linesCells, string warpSep, string lineSep)
            => LinesToText(from v in linesCells select RowToLine(v, warpSep), lineSep);

        public  string ColumnToWarp(params string[] cells)
        {
            return ColumnToWarp(cells, LinesSplitter);
        }
        public  string ColumnToWarp(IEnumerable<string> cells)
        {
            return ColumnToWarp(cells, LinesSplitter);
        }
        public virtual string ColumnToWarp(IEnumerable<string> cells, string delimited)
        {
            return string.Join(delimited, from cell in cells select cell.Contains(delimited) ? QuoteChar + cell + QuoteChar : cell);
        }
        public  IEnumerable<string> ColumnsToWarps(IEnumerable<string>[] cols)
        {
            return ColumnsToWarps(cols, LinesSplitter);
        }
        public  IEnumerable<string> ColumnsToWarps(IEnumerable<IEnumerable<string>> cols)
        {
            return ColumnsToWarps(cols, LinesSplitter);
        }
        public virtual IEnumerable<string> ColumnsToWarps(IEnumerable<IEnumerable<string>> cols, string delimited)
        {
            return  from col in cols select ColumnToWarp(col, delimited);
        }
        public  IEnumerable<string> WarpToColumn(string warp)
        {
            return WarpToColumn(warp, LinesSplitters);
        }
        public virtual IEnumerable<string> WarpToColumn(string warp, params string[] linesSplitor)
        {
            return SplitText(warp, linesSplitor);
        }
        public  IEnumerable<IEnumerable<string>> WarpsToColumns(params string[] warps)
        {
            return WarpsToColumns(warps, LinesSplitters);
        }
        public  IEnumerable<IEnumerable<string>> WarpsToColumns(IEnumerable<string> warps)
        {
            return WarpsToColumns(warps, LinesSplitters);
        }
        public virtual IEnumerable<IEnumerable<string>> WarpsToColumns(IEnumerable<string> warps, params string[] linesSplitor)
            => from v in warps select WarpToColumn(v, linesSplitor);
        public  string WarpsToText(params string[] warps) => WarpsToText(warps, WarpsSplitter);
        public  string WarpsToText(IEnumerable<string> warps) => WarpsToText(warps, WarpsSplitter);
        public virtual string WarpsToText(IEnumerable<string> warps, string warpsSplitor) => ColumnsToText(from v in warps select WarpToColumn(v, LinesSplitters), warpsSplitor, LinesSplitter);
        public  string ColumnsToText(params IEnumerable<string>[] colsCells)
        {
            return ColumnsToText(colsCells, WarpsSplitter, LinesSplitter);
        }
        public  string ColumnsToText(IEnumerable<IEnumerable<string>> colsCells)
        {
            return ColumnsToText(colsCells, WarpsSplitter, LinesSplitter);
        }
        public virtual string ColumnsToText(IEnumerable<IEnumerable<string>> colsCells, string warpSep, string lineSep)
        {
            return RowsToText(ColumnsToRows(colsCells), warpSep, lineSep);
        }

        public virtual IEnumerable<IEnumerable<string>> ColumnsToRows(IEnumerable<IEnumerable<string>> cols)
        {
            int i = 0;
            bool hasrow = true;
            IEnumerator<IEnumerable<string>> iecliplinescells = cols.GetEnumerator();
            while (hasrow)
            {
                hasrow = false;
                List<string> row = new List<string>();
                while (iecliplinescells.MoveNext())
                {
                    var celld = iecliplinescells.Current.Skip(i);
                    hasrow = hasrow || celld.Any();
                    string cell = celld.ElementAtOrDefault(0);
                    row.Add(cell);
                }
                iecliplinescells = cols.GetEnumerator();
                i++;
                if (hasrow) yield return row;
            }
        }
        public virtual IEnumerable<IEnumerable<string>> RowsToColumns(IEnumerable<IEnumerable<string>> rows)
        {
            int i = 0;
            bool hasrow = true;
            IEnumerator<IEnumerable<string>> iecliplinescells = rows.GetEnumerator();
            while (hasrow)
            {
                hasrow = false;
                List<string> row = new List<string>();
                while (iecliplinescells.MoveNext())
                {
                    var celld = iecliplinescells.Current.Skip(i);
                    hasrow = hasrow || celld.Any();
                    string cell = celld.ElementAtOrDefault(0);
                    row.Add(cell);
                }
                iecliplinescells = rows.GetEnumerator();
                i++;
                if (hasrow) yield return row;
            }
        }


        public virtual string PlainToStandard(string text)
        {
            return PlainToStandard(text, WarpsSplitter);
        }
        public virtual string PlainToStandard(string text, string delimited)
        {
            if (text == null) return string.Empty;
            string ncel = EscapeChars(text);
            return ncel.Contains(delimited) ? QuoteChar + ncel + QuoteChar : ncel;
        }
        public virtual string StandardToPlain(string cell)
        {
            foreach (var item in QuoteChars)
                if (cell.FirstOrDefault() == item)
                    if (cell.LastOrDefault() == item && !cell.EndsWith(string.Join("", MetaChar, item)))
                        cell = cell.Substring(1, cell.Length - 2);
            return UnescapeChars(cell);
        }


        public virtual IEnumerable<string> QuickSplitText(string text, params string[] splitor)
        {
            if (text == null) yield break;
            if (splitor.Length < 1)
            {
                yield return text;
                yield break;
            }
            string cel = null;
            var ql = QuoteChars.Select(c => c + "").ToList();
            var endQF = @"((?<!(?<!\{0})\{0})\{1}$)";
            int ind = -1;
            string q = null;
            foreach (var item in text.Split(splitor, StringSplitOptions.None))
                if (q != null)
                    if (Regex.IsMatch(item, string.Format(endQF, MetaChar, q)))
                    {
                        yield return StandardToPlain(string.Join(splitor.First(), cel, item));
                        cel = null;
                        q = null;
                    }
                    else cel = string.Join(splitor.First(), cel, item);
                else if ((ind = ql.FindIndex(c => item.StartsWith(c))) >= 0)
                    if (Regex.IsMatch(item, string.Format(endQF, MetaChar, ql[ind])))
                        yield return StandardToPlain(item);
                    else
                    {
                        cel = item;
                        q = ql[ind];
                    }
                else yield return item;

            if (cel != null)
                foreach (var item in cel.Split(splitor, StringSplitOptions.None))
                    yield return item;
        }
        //public IEnumerable<string> SplitText(string text, string[] splitor)
        //{
        //    if (text == null) yield break;
        //    if (splitor.Length < 1)
        //    {
        //        yield return text;
        //        yield break;
        //    }
        //    string cel = null;
        //    var ql = QuoteChars.Select(c => c + "").ToList();
        //    int ind = -1;
        //    foreach (var item in text.Split(splitor, StringSplitOptions.None))
        //        if (cel != null)
        //            if ((ind = ql.FindIndex(c => item.StartsWith(c))) >= 0)
        //            {
        //                yield return StandardToPlain(cel);
        //                if (item.EndsWith(ql[ind]))
        //                {
        //                    yield return StandardToPlain(item);
        //                    cel = null;
        //                }
        //                else cel = item;
        //            }
        //            else if ((ind = ql.FindIndex(c => item.EndsWith(c))) >= 0)
        //            {
        //                yield return StandardToPlain(string.Join(splitor.First(), cel, item));
        //                cel = null;
        //            }
        //            else cel += splitor.First() + item;
        //        else if ((ind = ql.FindIndex(c => item.StartsWith(c))) >= 0)
        //            if (item.EndsWith(ql[ind]))
        //                yield return StandardToPlain(item);
        //            else cel = item;
        //        else yield return item;

        //    if (cel != null)
        //        foreach (var item in cel.Split(splitor, StringSplitOptions.None))
        //            yield return item;
        //}
        public virtual IEnumerable<string> SplitText(string text, params string[] splitors) => from v in StringService.Split(text, splitors, QuoteChars, MetaChar) select UnescapeChars(v);
        #endregion
    }
}
