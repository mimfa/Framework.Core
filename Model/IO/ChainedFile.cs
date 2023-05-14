using MiMFa.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;
using System.Windows;
using System.Text.RegularExpressions;
using Microsoft.VisualBasic.Devices;
using MiMFa.General;
using System.Xml;
using MiMFa.Model.IO.ChainedFiles;
using Microsoft.Office.Interop.Excel;
using System.Windows.Shapes;
using MiMFa.Model.IO.Matrices;
using System.Data;
using MiMFa.Model.IO.Connector;

namespace MiMFa.Model.IO
{
    public delegate void ChainedFileHandler(ChainedFile file, object arg);
    public delegate bool ChainedFileCountHandler(ChainedFile file, long len);
    [Serializable]
    public class ChainedFile : IEnumerable<IEnumerable<string>>
    {
        #region IDENTIFIERS
        public long ID { get; protected set; } = DateTime.Now.Ticks;
        public string Token { get; set; } = null;
        public bool Freeze
        {
            get { return _Freeze; }
            set
            {
                _Freeze = value;
                if (HasForePiece) ForePiece.Freeze = value;
            }
        }
        private bool _Freeze = false;

        private Dictionary<string, object> Attachs = new Dictionary<string, object>();
        public bool HasAttachs => Attachs.Count > 0;
        public bool SetAttached(string key, object obj)
        {
            if (Attachs.ContainsKey(key))
            {
                Attachs[key] = obj;
                return false;
            }
            Attachs[key] = obj;
            return true;
        }
        public object GetAttached(string key)
        {
            if (Attachs.ContainsKey(key)) return Attachs[key];
            return null;
        }
        public T GetAttached<T>(string key, T def)
        {
            var val = GetAttached(key);
            return val != null ? (T)val : def;
        }

        public Encoding GetEncoding()
        {
            if (Freeze) return Encoding;
            StreamReader reader = null;
            try
            {
                reader = new StreamReader(Path, Encoding ?? Encoding.UTF8, true);
                reader.Peek();
                return reader.CurrentEncoding;
            }
            catch { return Encoding; }
            finally
            {
                if (reader != null) reader.Close();
            }
        }

        public IEnumerable<ChainedFile> GetChain()
        {
            yield return this;
            foreach (var item in GetSequences())
                yield return item;
        }
        public IEnumerable<ChainedFile> GetSequences()
        {
            if (HasForePiece)
                foreach (var item in ForePiece.GetChain())
                    yield return item;
        }

        IEnumerator<IEnumerable<string>> IEnumerable<IEnumerable<string>>.GetEnumerator() => ReadRows().GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ReadRows().GetEnumerator();

        public override string ToString() => Path;
        #endregion


        #region EVENTS
        public event ChainedFileHandler Acted = (e, a) => { };
        public event ChainedFileCountHandler Changed = (e, a) => true;
        public event ChainedFileCountHandler LinesChanged = (e, a) => true;
        public event ChainedFileCountHandler WarpsChanged = (e, a) => true;
        public event ChainedFileCountHandler PieceLinesCounting = (e, a) => true;
        public event ChainedFileCountHandler PieceLinesCounted = (e, a) => true;
        public event ChainedFileCountHandler WarpsCounting = (e, a) => true;
        public event ChainedFileCountHandler WarpsCounted = (e, a) => true;
        public event ChainedFileCountHandler LinesCounting = (e, a) => true;
        public event ChainedFileCountHandler LinesCounted = (e, a) => true;
        public event ChainedFileHandler PathChanged = (e, a) => { };

        public bool IsAutoSave { get; set; } = false;

        public void OnActed(object act = null)
        {
            Acted(this, act);
        }
        public bool OnChanged(long changeNum)
        {
            if (changeNum != 0)
            {
                LastPieceColumnsLabels = null;
                LastPieceRowsLabels = null;
            }
            IsPieceChanged = true;//IsChanged || ChangesBuffer.Count > 0 || changeNum != 0;
            IsBuffered = IsBuffered && !IsPieceChanged;
            if (changeNum >= int.MaxValue || changeNum <= -int.MaxValue)
            {
                changeNum = 0;
                PieceCount(BackSequenceLinesCount);
            }
            if (IsAutoSave) Save();
            if (UndoBufferCount >= BufferCountLimit) Stick();
            OnActed("Changed");
            return Changed(this, changeNum);
        }
        public void SetLinesChanged(ChainedFileCountHandler eventHandler)
        {
            LinesChanged = eventHandler;
        }
        public bool OnLinesChanged(long changeNum)
        {
            if (changeNum >= int.MaxValue || changeNum <= -int.MaxValue)
            {
                changeNum = 0;
                PieceCount(BackSequenceLinesCount);
            }
            LinesChanged(this, changeNum);
            OnActed("LinesChanged");
            OnChanged(changeNum);
            return LinesCounted(this, PieceLinesCount = LastPieceLinesCount += changeNum);
        }
        public void SetWarpsChanged(ChainedFileCountHandler eventHandler)
        {
            WarpsChanged = eventHandler;
        }
        public bool OnWarpsChanged(long changeNum)
        {
            if (changeNum >= int.MaxValue || changeNum <= -int.MaxValue)
            {
                changeNum = 0;
                PieceCount(BackSequenceLinesCount);
            }
            WarpsChanged(this, changeNum);
            OnActed("WarpsChanged");
            OnChanged(changeNum);
            return WarpsCounted(this, PieceWarpsCount = LastPieceWarpsCount += changeNum);
        }
        public void SetPieceLinesCounting(ChainedFileCountHandler eventHandler)
        {
            PieceLinesCounting = eventHandler;
        }
        public bool OnPieceLinesCounting(long count) => IsCountingPiece = PieceLinesCounting(this, LastPieceLinesCount = count);
        public void SetPieceLinesCounted(ChainedFileCountHandler eventHandler)
        {
            PieceLinesCounted = eventHandler;
        }
        public bool OnPieceLinesCounted(long count)
        {
            IsCountingPiece = false;
            return IsCountedPiece = PieceLinesCounted(this, PieceLinesCount = LastPieceLinesCount = count);
        }
        public void SetLinesCounting(ChainedFileCountHandler eventHandler)
        {
            LinesCounting = eventHandler;
        }
        public bool OnLinesCounting(long count) => LinesCounting(this, count);
        public void SetLinesCounted(ChainedFileCountHandler eventHandler)
        {
            LinesCounted = eventHandler;
        }
        public bool OnLinesCounted(long count) => LinesCounted(this, count);
        public void SetWarpsCounted(ChainedFileCountHandler eventHandler)
        {
            WarpsCounted = eventHandler;
        }
        public bool OnWarpsCounted(long count) => WarpsCounted(this, count);
        public void SetWarpsCounting(ChainedFileCountHandler eventHandler)
        {
            WarpsCounting = eventHandler;
        }
        public bool OnWarpsCounting(long count)
        {
            PieceWarpsCount = LastPieceWarpsCount = Math.Max(LastPieceWarpsCount, count);
            return WarpsCounting(this, count);
        }
        public void SetPathChanged(ChainedFileHandler eventHandler)
        {
            PathChanged = eventHandler;
        }
        public void OnPathChanged(string oldPath)
        {
            Encoding = GetEncoding();
            PathChanged(this, oldPath);
        }
        #endregion


        #region PATHS
        public bool IsExist => File.Exists(Path);
        public bool IsPieceExist => File.Exists(Path);
        public bool IsValid => (!string.IsNullOrWhiteSpace(Path) && IsExist);
        public bool IsPieceValid => (!string.IsNullOrWhiteSpace(Path) && IsPieceExist);
        public bool IsEmpty => !(IsExist && ReadLines().Any());
        public bool IsPieceEmpty => !(IsExist && ReadPieceLines().Any());

        public ConnectorBase Connector { get; private set; } = new PlainTextFile("");
        public string Path => Connector.Path;
        public string RelativePath { get; private set; } = null;
        public Encoding Encoding
        {
            get { return Connector.Encoding; }
            set
            {
                Connector.Encoding = value;
                if (HasForePiece) ForePiece.Encoding = value;
            }
        }
        public string Name => System.IO.Path.GetFileName(Path);
        public string NameWithoutExtension => System.IO.Path.GetFileNameWithoutExtension(Path);
        public string Extension => System.IO.Path.GetExtension(Path);
        public string Directory => string.IsNullOrWhiteSpace(Path) ? "" : System.IO.Path.GetDirectoryName(Path) + "\\";
        public string Statement => ConvertService.ToAlphabeticName(NameWithoutExtension);
        #endregion


        #region SEQUENCES
        public bool HasForePiece => ForePiece != null;
        public bool HasBackPiece => BackPiece != null;

        public bool IsFirstPiece => !HasBackPiece;
        public bool IsMiddlePiece => HasBackPiece && HasForePiece;
        public bool IsChainedPiece => HasForePiece || HasBackPiece;
        public bool IsLastPiece => !HasForePiece;

        public bool IsAutoSequence
        {
            get { return _IsAutoSequence; }
            set
            {
                _IsAutoSequence = value;
                if (HasForePiece) ForePiece.IsAutoSequence = value;
            }
        }
        private bool _IsAutoSequence = true;
        public bool IsSequenceTurn => IsAutoSequence && IsBigPiece;

        public ChainedFile MockPiece => new ChainedFile(this);
        public ChainedFile SinglePiece => new ChainedFile(this) { ForePiece = null, BackPiece = null };
        public ChainedFile BackPiece { get; private set; } = null;
        public ChainedFile ForePiece { get; private set; } = null;
        public ChainedFile FirstPiece => HasBackPiece ? BackPiece.FirstPiece : this;
        public ChainedFile LastPiece => HasForePiece ? ForePiece.LastPiece : this;

        public IEnumerable<ChainedFile> Chain => BackChain.Concat(ForeSequence);
        public IEnumerable<ChainedFile> ForeChain
        {
            get
            {
                yield return this;
                if (HasForePiece)
                    foreach (var item in ForePiece.ForeChain)
                        yield return item;
            }
        }
        public IEnumerable<ChainedFile> BackChain
        {
            get
            {
                yield return this;
                if (HasBackPiece)
                    foreach (var item in BackPiece.BackChain)
                        yield return item;
            }
        }
        public IEnumerable<ChainedFile> ForeSequence => HasForePiece ? ForePiece.ForeChain : new ChainedFile[0];
        public IEnumerable<ChainedFile> BackSequence => HasBackPiece ? BackPiece.BackChain : new ChainedFile[0];
        public int Length => BackSequenceLength + 1 + ForeSequenceLength;
        public int ForeLength => 1 + ForeSequenceLength;
        public int BackLength => BackSequenceLength + 1;
        public int ForeSequenceLength => HasForePiece ? ForePiece.ForeSequenceLength + 1 : 0;
        public int BackSequenceLength => HasBackPiece ? BackPiece.BackSequenceLength + 1 : 0;
        public int PieceIndex => BackSequenceLength;


        public void BreakSequence()
        {
            BackPiece = null;
            ForePiece = null;
            OnActed("BreakSequence");
        }

        public ChainedFile SetForePiece()
        {
            return SetForePiece(CreateNewSequencePath());
        }
        public ChainedFile SetForePiece(string path)
        {
            return SetForePiece(new ChainedFile(ConvertPathFromRelative(path)));
        }
        public ChainedFile SetForePiece(ChainedFile file)
        {
            if (file != null)
            {
                SetFeatures(file);
                SetEvents(file);
                file.BackPiece = null;
                file.BackPiece = this;
            }
            ForePiece = null;
            ForePiece = file;
            OnActed("SetForePiece");
            return file;
        }
        public ChainedFile AppendForePiece()
        {
            return AppendForePiece(CreateNewSequencePath());
        }
        public ChainedFile AppendForePiece(string path)
        {
            if (ForePiece == null) return AppendForePiece(new ChainedFile(ConvertPathFromRelative(path)));
            else if (Path == path) return null;
            else return ForePiece.AppendForePiece(path);
        }
        public ChainedFile AppendForePiece(ChainedFile appendedFile)
        {
            if (Path == appendedFile.Path) return null;
            if (HasForePiece) return ForePiece.AppendForePiece(appendedFile);
            else return SetForePiece(appendedFile);
        }

        public ChainedFile SetBackPiece()
        {
            return SetBackPiece(CreateNewSequencePath());
        }
        public ChainedFile SetBackPiece(string path)
        {
            return SetBackPiece(new ChainedFile(ConvertPathFromRelative(path)));
        }
        public ChainedFile SetBackPiece(ChainedFile file)
        {
            if (file != null)
            {
                SetFeatures(file);
                SetEvents(file);
                file.ForePiece = null;
                file.ForePiece = this;
            }
            BackPiece = null;
            BackPiece = file;
            OnActed("SetBackPiece");
            return file;
        }
        public ChainedFile PrependBackPiece()
        {
            return PrependBackPiece(CreateNewSequencePath());
        }
        public ChainedFile PrependBackPiece(string path)
        {
            if (BackPiece == null) return PrependBackPiece(new ChainedFile(ConvertPathFromRelative(path)));
            else if (Path == path) return null;
            else return BackPiece.PrependBackPiece(path);
        }
        public ChainedFile PrependBackPiece(ChainedFile prependedFile)
        {
            if (Path == prependedFile.Path) return null;
            if (HasBackPiece) BackPiece.PrependBackPiece(prependedFile);
            else SetBackPiece(prependedFile);
            OnActed("PrependBackPiece");
            return prependedFile;
        }
        #endregion


        #region LINES
        public long PieceLinesCount { get; private set; } = 0;
        public long LastPieceLinesCount { get; private set; } = 0;
        public long LinesCount => BackSequenceLinesCount + PieceLinesCount + ForeSequenceLinesCount;
        public long LastLinesCount => BackSequenceLastLinesCount + LastPieceLinesCount + ForeSequenceLastLinesCount;
        public long ForeLinesCount => PieceLinesCount + ForeSequenceLinesCount;
        public long ForeLastLinesCount => LastPieceLinesCount + ForeSequenceLastLinesCount;
        public long BackLinesCount => BackSequenceLinesCount + PieceLinesCount;
        public long BackLastLinesCount => BackSequenceLastLinesCount + LastPieceLinesCount;
        public long ForeMaxPieceLinesCount => Math.Max(PieceLinesCount, HasForePiece ? ForePiece.ForeMaxPieceLinesCount : 0);
        public long BackMaxPieceLinesCount => Math.Max(PieceLinesCount, HasBackPiece ? BackPiece.BackMaxPieceLinesCount : 0);
        public long ForeMinPieceLinesCount => Math.Min(PieceLinesCount, HasForePiece ? ForePiece.ForeMinPieceLinesCount : 0);
        public long BackMinPieceLinesCount => Math.Min(PieceLinesCount, HasBackPiece ? BackPiece.BackMinPieceLinesCount : 0);

        public long ForeSequenceLinesCount => HasForePiece ? ForePiece.PieceLinesCount + ForePiece.ForeSequenceLinesCount : 0;
        public long ForeSequenceLastLinesCount => HasForePiece ? ForePiece.LastPieceLinesCount + ForePiece.ForeSequenceLastLinesCount : 0;
        public long BackSequenceLinesCount => HasBackPiece ? BackPiece.PieceLinesCount + BackPiece.BackSequenceLinesCount : 0;
        public long BackSequenceLastLinesCount => HasBackPiece ? BackPiece.LastPieceLinesCount + BackPiece.BackSequenceLastLinesCount : 0;
        public long ForeSequenceMaxPieceLinesCount => HasForePiece ? ForePiece.ForeMaxPieceLinesCount : 0;
        public long BackSequenceMaxPieceLinesCount => HasBackPiece ? BackPiece.BackMaxPieceLinesCount : 0;
        public long ForeSequenceMinPieceLinesCount => HasForePiece ? ForePiece.ForeMinPieceLinesCount : 0;
        public long BackSequenceMinPieceLinesCount => HasBackPiece ? BackPiece.BackMinPieceLinesCount : 0;

        public long PieceWarpsCount { get; private set; } = 0;
        public long LastPieceWarpsCount { get; private set; } = 0;
        public long WarpsCount => (from v in GetChain() select v.PieceWarpsCount).Max();
        public long LastWarpsCount => (from v in GetChain() select v.LastPieceWarpsCount).Max();

        public bool IsCountingPiece { get; private set; } = false;
        public bool IsCountedPiece { get; set; } = false;
        public bool IsCounting => IsCountingBackSequence || IsCountingPiece || IsCountingForeSequence;
        public bool IsCounted => IsCountedBackSequence && IsCountedPiece && IsCountedForeSequence;
        public bool IsCountingForeSequence => HasForePiece ? ForePiece.IsCountingPiece || ForePiece.IsCountingForeSequence : false;
        public bool IsCountedForeSequence => HasForePiece ? ForePiece.IsCountedPiece && ForePiece.IsCountedForeSequence : true;
        public bool IsCountingBackSequence => HasBackPiece ? BackPiece.IsCountingPiece || BackPiece.IsCountingBackSequence : false;
        public bool IsCountedBackSequence => HasBackPiece ? BackPiece.IsCountedPiece && BackPiece.IsCountedBackSequence : true;

        public long PieceCount(long baselinescount = 0)
        {
            if (Freeze) return LastPieceLinesCount;
            OnActed("Counting");
            long count = 0;
            foreach (var item in ReadSampleLines(5))
                OnWarpsCounting(Connector.LineToRow(item).LongCount());
            if (PieceLinesSplitters.Length < 1 || PieceLinesSplitters[0] == Environment.NewLine)
            {
                count += ReadPieceLines().LongCount();
                OnPieceLinesCounting(count);
                OnLinesCounting(baselinescount + count);
            }
            //{
            //    int num = 9;
            //    IEnumerable<string> lines = ReadPieceLines();
            //    IEnumerable<string> nlines = lines.Skip(num);
            //    do
            //    {
            //        if (nlines.Any())
            //        {
            //            count += num;
            //            if (num < 99999) num = Convert.ToInt32(num * 10 + num);
            //            lines = nlines;
            //            OnCellsCounting(LineToCells(lines.First()).LongCount());
            //            nlines = nlines.Skip(num);
            //        }
            //        else if (num > 0)
            //        {
            //            num = Convert.ToInt32(num / 10);
            //            nlines = lines.Skip(num);
            //        }
            //        else
            //        {
            //            OnCellsCounting(LineToCells(lines.First()).LongCount());
            //            count += lines.Count();
            //            break;
            //        }
            //    }
            //    while (OnLinesCounting(count) && OnChainLinesCounting(baselinescount + count));
            //}
            else
            {
                count = IsEmpty ? 0 : Regex.Matches(ReadPieceText(), string.Join("|", from v in PieceLinesSplitters select Regex.Escape(v))).Count + 1;
                OnPieceLinesCounting(count);
                OnLinesCounting(baselinescount + count);
            }
            OnPieceLinesCounted(count);
            OnActed("Count");
            return baselinescount + count;
        }
        public long Count(bool quick = true)
        {
            long count = quick ? QuickCount(0) : Count(0);
            OnActed("ChainCount");
            OnLinesCounted(count);
            OnWarpsCounted(WarpsCount);
            return count;
        }
        public long QuickCount(long baselinescount)
        {
            PieceWarpsCount = 0;
            LastPieceWarpsCount = 0;
            if (Freeze) return LastLinesCount;
            long count = (!IsCountedPiece || PieceLinesCount < 1 ? PieceCount(baselinescount) : baselinescount + PieceLinesCount);
            if (HasForePiece) count = ForePiece.QuickCount(count);
            return count;
        }
        public long Count(long baselinescount)
        {
            PieceWarpsCount = 0;
            LastPieceWarpsCount = 0;
            if (Freeze) return LastLinesCount;
            long count = PieceCount(baselinescount);
            if (HasForePiece) count = ForePiece.Count(count);
            return count;
        }
        #endregion


        #region FORMATS
        public virtual string CreateColumnsPattern(string pattern, int[] cols, bool isplainText = true)
        {
            string sep = Regex.Escape(PieceWarpsSplitter);
            //string es = Regex.Escape(pattern);
            if (isplainText && InfoService.IsRegexPattern(pattern)) pattern = ConvertService.ToRegexPattern(pattern);
            string word = @"([\S\s](?<!" + sep + @"))*" + pattern;
            sep = @"^(([\S\s](?<!" + sep + @"))*" + sep + ")";
            pattern = "";
            foreach (var item in cols)
                if (item < 1) pattern = string.Join("", @"^", word, string.IsNullOrEmpty(pattern) ? "" : "|", pattern);
                else pattern = string.Join("", sep, "{", item, "}", word, string.IsNullOrEmpty(pattern) ? "" : "|", pattern);
            return pattern;
        }

        public bool IsForceColumnsLabels { get; private set; } = false;
        public bool HasPieceColumnsLabels => PieceColumnsLabelsIndex > -1;
        private long _PieceColumnsLabelsIndex = -1;
        public long PieceColumnsLabelsIndex
        {
            get => _PieceColumnsLabelsIndex;
            set
            {
                if (_PieceColumnsLabelsIndex != value)
                    LastPieceColumnsLabels = null;
                _PieceColumnsLabelsIndex = value;
            }
        }
        public IEnumerable<string> LastPieceColumnsLabels { get; set; } = null;
        public IEnumerable<string> PieceColumnsLabels
        {
            get
            {
                if (!IsForceColumnsLabels && LastPieceColumnsLabels != null) return LastPieceColumnsLabels;
                IsForceColumnsLabels = false;
                if (HasPieceColumnsLabels)
                     LastPieceColumnsLabels = ReadRow(PieceColumnsLabelsIndex).ToList();
                else LastPieceColumnsLabels = new List<string>();
                return LastPieceColumnsLabels = LastPieceColumnsLabels.Concat(MiMFa.Statement.Loop(LastPieceColumnsLabels.LongCount(), PieceWarpsCount, i => default(string))).ToList();
            }
            set
            {
                if (LinesCount == PieceColumnsLabelsIndex) WriteRow(LastPieceColumnsLabels = value);
                else ChangeRow(ColumnsLabelsIndex, LastPieceColumnsLabels = value);
            }
        }
        public string GetPieceColumnLabel(long colIndex) => PieceColumnsLabels.ElementAtOrDefault((int)colIndex);
        public IEnumerable<string> ForcePieceColumnsLabels
        {
            get
            {
                if (IsForceColumnsLabels && LastPieceColumnsLabels != null) return LastPieceColumnsLabels;
                IsForceColumnsLabels = true;
                if (HasPieceColumnsLabels)
                    LastPieceColumnsLabels = ReadRow(PieceColumnsLabelsIndex).ToList();
                else LastPieceColumnsLabels = new List<string>();
                return LastPieceColumnsLabels = LastPieceColumnsLabels.Concat(MiMFa.Statement.Loop(LastPieceColumnsLabels.LongCount(), PieceWarpsCount, i => ConvertService.ToAlphabet(i, false))).ToList();
            }
            set => PieceColumnsLabels = value;
        }
        public string GetForcePieceColumnLabel(long colIndex) => ForcePieceColumnsLabels.ElementAtOrDefault((int)colIndex);

        public bool HasColumnsLabels => ColumnsLabelsIndex > -1;
        public bool FreeColumnsLabels { get; set; } = true;
        public long ColumnsLabelsIndex
        {
            get => HasPieceColumnsLabels ? PieceColumnsLabelsIndex : HasForePiece ? ForePiece.ColumnsLabelsIndex : -1;
            set
            {
                PieceColumnsLabelsIndex = value;
                if (HasForePiece) ForePiece.ColumnsLabelsIndex = value; ;
            }
        }
        public IEnumerable<string> ColumnsLabels
        {
            get => HasPieceColumnsLabels ? PieceColumnsLabels : HasForePiece ? ForePiece.ColumnsLabels : new string[] { };
            set
            {
                if (HasPieceColumnsLabels) PieceColumnsLabels = value;
                else if (HasForePiece) ForePiece.ColumnsLabels = value;
            }
        }
        public string GetColumnLabel(long colIndex) => HasPieceColumnsLabels ? GetColumnLabel(colIndex) : HasForePiece ? ForePiece.GetColumnLabel(colIndex) : null;
        public IEnumerable<string> ForceColumnsLabels
        {
            get => HasPieceColumnsLabels ? ForcePieceColumnsLabels : HasForePiece ? ForePiece.ForceColumnsLabels : MiMFa.Statement.Loop(0, PieceWarpsCount, i => ConvertService.ToAlphabet(i, false));
            set => ColumnsLabels = value;
        }
        public string GetForceColumnLabel(long colIndex) => HasPieceColumnsLabels ? GetForcePieceColumnLabel(colIndex) : HasForePiece ? ForePiece.GetForceColumnLabel(colIndex) : ConvertService.ToAlphabet(colIndex);

        public long GetForcePieceColumnIndex(string label, bool sense = false)
        {
            if (label == null) return -1;
            long l = -1;
            if (sense)
                foreach (var item in ForcePieceColumnsLabels)
                    if (label == item) return ++l;
                    else ++l;
            else
            {
                label = label.ToLower().Trim();
                foreach (var item in ForcePieceColumnsLabels)
                    if (label == item.ToLower().Trim()) return ++l;
                    else ++l;
            }
            return ConvertService.ToNumber(label);
        }
        public long GetPieceColumnIndex(string label, bool sense = false)
        {
            if (label == null) return -1;
            long l = -1;
            if (sense)
                foreach (var item in PieceColumnsLabels)
                    if (label == item) return ++l;
                    else ++l;
            else
            {
                label = label.ToLower().Trim();
                foreach (var item in PieceColumnsLabels)
                    if (label == item.ToLower().Trim()) return ++l;
                    else ++l;
            }
            return -1;
        }
        public long GetForceColumnIndex(string label, bool sense = false)
        {
            if (label == null) return -1;
            long l = -1;
            if (sense)
                foreach (var item in ForceColumnsLabels)
                    if (label == item) return ++l;
                    else ++l;
            else
            {
                label = label.ToLower().Trim();
                foreach (var item in ForceColumnsLabels)
                    if (label == item.ToLower().Trim()) return ++l;
                    else ++l;
            }
            return ConvertService.ToNumber(label);
        }
        public long GetColumnIndex(string label, bool sense = false)
        {
            if (label == null) return -1;
            long l = -1;
            if (sense)
                foreach (var item in ColumnsLabels)
                    if (label == item) return ++l;
                    else ++l;
            else {
                label = label.ToLower().Trim();
                foreach (var item in ColumnsLabels)
                    if (label == item.ToLower().Trim()) return ++l;
                    else ++l;
            }
            return -1;
        }


        public bool IsForceRowsLabels { get; set; } = false;
        public bool HasPieceRowsLabels => PieceRowsLabelsIndex > -1;
        private long _PieceRowsLabelsIndex = -1;
        public long PieceRowsLabelsIndex
        {
            get => _PieceRowsLabelsIndex;
            set
            {
                if (_PieceRowsLabelsIndex != value)
                    LastPieceRowsLabels = null;
                _PieceRowsLabelsIndex = value;
            }
        }
        public IEnumerable<string> LastPieceRowsLabels { get; set; } = null;
        public IEnumerable<string> PieceRowsLabels
        {
            get
            {
                if (!IsForceRowsLabels && LastPieceRowsLabels != null) return LastPieceRowsLabels;
                IsForceRowsLabels = false;
                if (HasPieceRowsLabels)
                    LastPieceRowsLabels = ReadColumn(PieceRowsLabelsIndex);
                else LastPieceRowsLabels = new List<string>();
                return LastPieceRowsLabels = LastPieceRowsLabels.Concat(MiMFa.Statement.Loop(LastPieceRowsLabels.LongCount(), PieceLinesCount, i => string.Empty)).ToList();
            }
            set => ChangeColumn(RowsLabelsIndex, LastPieceRowsLabels = value);
        }
        public string GetPieceRowLabel(long rowIndex) => PieceRowsLabels.ElementAtOrDefault((int) rowIndex);
        public IEnumerable<string> ForcePieceRowsLabels
        {
            get
            {
                if (IsForceRowsLabels && LastPieceRowsLabels != null) return LastPieceRowsLabels;
                IsForceRowsLabels = true;
                if (HasPieceRowsLabels)
                    LastPieceRowsLabels = ReadColumn(PieceRowsLabelsIndex);
                else LastPieceRowsLabels = new List<string>();
                return LastPieceRowsLabels = LastPieceRowsLabels.Concat(MiMFa.Statement.Loop(LastPieceRowsLabels.LongCount(), PieceLinesCount, i => i.ToString())).ToList();
            }
            set => PieceRowsLabels= value;
        }
        public string GetForcePieceRowLabel(long rowIndex) => ForcePieceRowsLabels.ElementAtOrDefault((int)rowIndex);

        public bool HasRowsLabels => RowsLabelsIndex > -1;
        public bool FreeRowsLabels { get; set; } = true;
        public long RowsLabelsIndex
        {
            get => HasPieceRowsLabels ? PieceRowsLabelsIndex : HasForePiece ? ForePiece.RowsLabelsIndex : -1; set
            {
                PieceRowsLabelsIndex = value;
                if (HasForePiece) ForePiece.RowsLabelsIndex = value; ;
            }
        }
        public IEnumerable<string> RowsLabels
        {
            get => HasPieceRowsLabels ? PieceRowsLabels : HasForePiece ? ForePiece.RowsLabels : new string[] { };
            set
            {
                if (HasPieceRowsLabels) PieceRowsLabels = value;
                else if (HasForePiece) ForePiece.RowsLabels = value;
            }
        }
        public IEnumerable<string> ForceRowsLabels
        {
            get => HasPieceRowsLabels ? PieceRowsLabels : HasForePiece ? ForePiece.RowsLabels : new string[] { };
            set => RowsLabels = value;
        }

        public string GetRowLabel(long rowIndex) => HasPieceRowsLabels ? GetRowLabel(rowIndex) : HasForePiece ? ForePiece.GetRowLabel(rowIndex) : null;
        public string GetForceRowLabel(long rowIndex) => HasPieceRowsLabels ? GetForcePieceRowLabel(rowIndex) : HasForePiece ? ForePiece.GetForceRowLabel(rowIndex) : rowIndex + "";

        public long GetForcePieceRowIndex(string label, bool sense = false)
        {
            if (label == null) return -1;
            long l = -1;
            if (sense)
                foreach (var item in ForcePieceRowsLabels)
                    if (label == item) return ++l;
                    else ++l;
            else
            {
                label = label.ToLower().Trim();
                foreach (var item in ForcePieceRowsLabels)
                    if (label == item.ToLower().Trim()) return ++l;
                    else ++l;
            }
            return ConvertService.ToNumber(label);
        }
        public long GetPieceRowIndex(string label, bool sense = false)
        {
            if (label == null) return -1;
            long l = -1;
            if (sense)
                foreach (var item in PieceRowsLabels)
                    if (label == item) return ++l;
                    else ++l;
            else
            {
                label = label.ToLower().Trim();
                foreach (var item in PieceRowsLabels)
                    if (label == item.ToLower().Trim()) return ++l;
                    else ++l;
            }
            return -1;
        }
        public long GetForceRowIndex(string label, bool sense = false)
        {
            if (label == null) return -1;
            long l = -1;
            if (sense)
                foreach (var item in ForceRowsLabels)
                    if (label == item) return ++l;
                    else ++l;
            else
            {
                label = label.ToLower().Trim();
                foreach (var item in ForceRowsLabels)
                    if (label == item.ToLower().Trim()) return ++l;
                    else ++l;
            }
            return ConvertService.ToNumber(label);
        }
        public long GetRowIndex(string label, bool sense = false)
        {
            if (label == null) return -1;
            long l = -1;
            if (sense)
                foreach (var item in RowsLabels)
                    if (label == item) return ++l;
                    else ++l;
            else
            {
                label = label.ToLower().Trim();
                foreach (var item in RowsLabels)
                    if (label == (item ?? "").ToLower().Trim()) return ++l;
                    else ++l;
            }
            return -1;
        }


        public string WarpsLabels => Connector.RowToLine(ColumnsLabels);
        public string ForceWarpsLabels => Connector.RowToLine(ForceColumnsLabels);
        public string PieceWarpsLabels => Connector.RowToLine(PieceColumnsLabels);
        public string PieceForceWarpsLabels => Connector.RowToLine(ForcePieceColumnsLabels);

        public string LinesLabels => Connector.ColumnToWarp(RowsLabels);
        public string ForceLinesLabels => Connector.ColumnToWarp(ForceRowsLabels);
        public string PieceLinesLabels => Connector.ColumnToWarp(PieceRowsLabels);
        public string PieceForceLinesLabels => Connector.ColumnToWarp(ForcePieceRowsLabels);

        public bool DefaultConfig { get => Connector.DefaultConfig; set => Connector.DefaultConfig = value; }
        public string WarpsSplitter
        {
            get => Connector.WarpsSplitter;
            set => WarpsSplitters = new string[] { value };
        }
        public string[] WarpsSplitters
        {
            get { return Connector.WarpsSplitters; }
            set
            {
                Connector.WarpsSplitters = value;
                if (HasForePiece) ForePiece.WarpsSplitters = value;
            }
        }
        public string PieceWarpsSplitter
        {
            get => Connector.WarpsSplitter ;
            set => Connector.WarpsSplitter = value  ;
        }
        public string[] PieceWarpsSplitters
        {
            get => Connector.WarpsSplitters;
            set
            {
                Connector.WarpsSplitters = value;
            }
        }
        public string LinesSplitter
        {
            get => Connector.LinesSplitter;
            set => LinesSplitters = new string[] { value };
        }
        public string[] LinesSplitters
        {
            get { return Connector.LinesSplitters; }
            set
            {
                Connector.LinesSplitters = value;
                if (HasForePiece) ForePiece.LinesSplitters = value;
            }
        }
        public string PieceLinesSplitter
        {
            get => Connector.LinesSplitter;
            set => Connector.LinesSplitter =  value;
        }
        public string[] PieceLinesSplitters
        {
            get => Connector.LinesSplitters;
            set
            {
                Connector.LinesSplitters = value;
            }
        }

        public  char QuoteChar
        {
            get => Connector.QuoteChar;
            set => Connector.QuoteChar =value ;
        }
        public  char[] QuoteChars
        {
            get { return Connector.QuoteChars; }
            set  { Connector.QuoteChars = value;}
        }
        public  char MetaChar
        {
            get => Connector.MetaChar; set
            {
                Connector.MetaChar = value;
            }
        }
        public  string MetaLineFeedChar { get => Connector.MetaLineFeedChar; set => Connector.MetaLineFeedChar = value; }
        public string MetaCarriageReturnChar { get => Connector.MetaCarriageReturnChar; set => Connector.MetaCarriageReturnChar = value; }
        public string MetaLineBreakChar { get => Connector.MetaLineBreakChar; set => Connector.MetaLineBreakChar = value; }



        public Dictionary<int, double> WarpsLabelsIndexSuggests()
        {
            return LabelsIndexSuggests((from v in ReadRows().Take(5) select v.Take(10).ToArray()).ToArray());
        }
        public Dictionary<int, double> LinesLabelsIndexSuggests()
        {
            return LabelsIndexSuggests((from v in ReadColumns().Take(5) select v.Take(10).ToArray()).ToArray());
        }
        public Dictionary<int, double> LabelsIndexSuggests(string[][] sampleCells, double minPercent = 50)
        {
            Dictionary<int, double> dic = new Dictionary<int, double>();
            double[][][] list = new double[sampleCells.Length][][];
            for (int i = 0; i < sampleCells.Length; i++)
            {
                var cells = sampleCells[i];
                list[i] = new double[cells.Length + 1][];
                for (int j = 0; j < cells.Length; j++)
                {
                    list[i][j] = new double[3];
                    //Number of Characters
                    list[i][j][0] = cells[j].Length;
                    List<int> lasci = new List<int>();
                    foreach (char item in cells[j])
                        lasci.Add(item);
                    //ASCI Codes Average
                    list[i][j][1] = lasci.Count < 1 ? 0 : lasci.Average();
                    //ASCI Codes STD
                    list[i][j][2] = lasci.Count < 1 ? 0 : Math.Sqrt((from v in lasci select Math.Pow(v - list[i][j][1], 2)).Average());
                }
                list[i][cells.Length] = new double[3];
                //Number of Characters
                list[i][cells.Length][0] = (from v in list[i].Take(cells.Length) where v != null select v[0]).Average();
                //ASCIs Codes Average
                list[i][cells.Length][1] = (from v in list[i].Take(cells.Length) where v != null select v[1]).Average();
                //ASCIs Codes STD
                list[i][cells.Length][2] = Math.Sqrt((from v in list[i].Take(cells.Length) where v != null select Math.Pow(v[1] - list[i][cells.Length][1], 2)).Average());
            }
            var lengthAVG = (from v in list where v != null select v.Last()[0]).Average();
            var asciAVG = (from v in list where v != null select v.Last()[1]).Average();
            var stdAVG = (from v in list where v != null select v.Last()[2]).Average();
            var sum = lengthAVG + asciAVG + 10 * stdAVG;
            dic.Add(-1, minPercent);
            if (sum > 0)
                for (int i = 0; i < list.Length; i++)
                    if (list[i] != null)
                    {
                        var l = lengthAVG - list[i].Last()[0];
                        var a = Math.Abs(asciAVG - list[i].Last()[1]);
                        var s = Math.Abs(stdAVG - list[i].Last()[2]);
                        dic.Add(i, ((l + a + 10 * s) / sum) * 100);
                    }
            return dic.OrderByDescending(kvp => kvp.Value).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }
        public Dictionary<string, double> WarpsSplittersSuggests()
        {
            Regex spat = new Regex(@"\W");
            Regex pat = new Regex(@"\W+");
            Dictionary<string, List<int>> candid = new Dictionary<string, List<int>>();
            List<string> lines = ReadSampleLines(9).ToList();
            for (int i = 0; i < lines.Count; i++)
            {
                string nline = Connector.QuoteDetector.Replace(lines[i], "A");
                foreach (Match item in pat.Matches(nline))
                {
                    if (!candid.ContainsKey(item.Value)) candid.Add(item.Value, new List<int>());
                    while (candid[item.Value].Count <= i) candid[item.Value].Add(0);
                    string[] arr = lines[i].Split(new string[] { item.Value }, StringSplitOptions.None);
                    candid[item.Value][i] = arr.Length;
                }
                foreach (Match item in spat.Matches(nline))
                {
                    if (!candid.ContainsKey(item.Value)) candid.Add(item.Value, new List<int>());
                    while (candid[item.Value].Count <= i) candid[item.Value].Add(0);
                    string[] arr = lines[i].Split(new string[] { item.Value }, StringSplitOptions.None);
                    candid[item.Value][i] = arr.Length;
                }
            }
            double max = 0;
            double min = double.MaxValue;
            Dictionary<string, double> result = new Dictionary<string, double>();
            foreach (var item in candid.OrderByDescending(v => v.Value.Count))
            {
                double avg = item.Value.Average();
                double m = item.Value.Max() - avg;
                if (m <= min && item.Value.Count >= max)
                {
                    min = m;
                    max = item.Value.Count;
                    result.Add(item.Key, avg);
                }
            }
            switch (Extension.ToLower())
            {
                case ".csv":
                    if (result.ContainsKey(",")) result[","] *= 999;
                    else result.Add(",", 9999);
                    break;
                case ".tsv":
                    if (result.ContainsKey("\t")) result["\t"] *= 999;
                    else result.Add("\t", 9999);
                    break;

                default:
                    break;
            }
            return result.OrderByDescending(kvp => kvp.Value).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }
        public Dictionary<string, double> LinesSplittersSuggests()
        {
            Dictionary<string, double> result = new Dictionary<string, double>();
            result.Add(Environment.NewLine, 3);
            result.Add("\n", 2);
            result.Add("\r", 2);
            return result.OrderByDescending(kvp => kvp.Value).ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        }
        #endregion


        #region CONTROLS
        public static ulong RAMMinUnuseableCapacity { get; set; } = 102400000;
        public static ulong RAMMinUseableCapacity { get; set; } = 10000000;
        public static ulong RAMMaximumCapacity => RAMFreeCapacity - RAMMinUnuseableCapacity;
        public static bool RAMAvailable => RAMFreeCapacity > RAMMinUnuseableCapacity;
        public static ulong RAMFreeCapacity => new ComputerInfo().AvailablePhysicalMemory;
        #endregion


        #region INFO
        public bool IsSmall => Size < RAMMaximumCapacity / 4;
        public bool IsSmallPiece => PieceSize < RAMMaximumCapacity / 4;
        public bool HasSmall => IsSmallPiece || HasSmallInSequence;
        public bool IsSmallSequence => SequencesSize < RAMMaximumCapacity / 4;
        public bool HasSmallInSequence => HasSmallInForeSequence || HasSmallInBackSequence;
        public bool IsSmallBackSequence => BackSequenceSize < RAMMaximumCapacity / 4;
        public bool HasSmallInBackSequence => HasBackPiece ? BackPiece.IsSmallPiece || BackPiece.HasSmallInBackSequence : false;
        public bool IsSmallForeSequence => ForeSequenceSize < RAMMaximumCapacity / 4;
        public bool HasSmallInForeSequence => HasForePiece ? ForePiece.IsSmallPiece || ForePiece.HasSmallInForeSequence : false;


        public bool IsBig => Size > RAMMaximumCapacity / 2;
        public bool IsBigPiece => PieceSize > RAMMaximumCapacity / 2;
        public bool HasBig => IsBigPiece || HasBigInSequence;
        public bool IsBigSequence => IsBigForeSequence || IsBigBackSequence;
        public bool HasBigInSequence => HasBigInForeSequence || HasBigInBackSequence;
        public bool IsBigBackSequence => BackSequenceSize > RAMMaximumCapacity / 2;
        public bool HasBigInBackSequence => HasBackPiece ? BackPiece.IsBigPiece || BackPiece.HasBigInBackSequence : false;
        public bool IsBigForeSequence => ForeSequenceSize > RAMMaximumCapacity / 2;
        public bool HasBigInForeSequence => HasForePiece ? ForePiece.IsBigPiece || ForePiece.HasBigInForeSequence : false;

        public FileInfo Info => IsValid ? (new FileInfo(Path)) : null;
        public ulong Size => SequencesSize + PieceSize;
        public ulong PieceSize => IsValid ? (ulong)Info.Length : 0;
        public ulong AverageSize => Size / (ulong)Length;
        public ulong MaximumSize => Math.Max(Math.Max(BackSequenceMaximumSize, PieceSize), ForeSequenceMaximumSize);
        public ulong MinimumSize => Math.Min(Math.Min(BackSequenceMinimumSize, PieceSize), ForeSequenceMinimumSize);
        public ulong SequencesSize => BackSequenceSize + ForeSequenceSize;
        public ulong BackSequenceSize => HasBackPiece ? BackPiece.PieceSize + BackPiece.BackSequenceSize : 0;
        public ulong ForeSequenceSize => HasForePiece ? ForePiece.PieceSize + ForePiece.ForeSequenceSize : 0;
        public ulong BackSequenceMaximumSize => HasBackPiece ? Math.Max(PieceSize, BackPiece.BackSequenceMaximumSize) : PieceSize;
        public ulong ForeSequenceMaximumSize => HasForePiece ? Math.Max(PieceSize, BackPiece.ForeSequenceMaximumSize) : PieceSize;
        public ulong BackSequenceMinimumSize => HasBackPiece ? Math.Min(PieceSize, BackPiece.BackSequenceMinimumSize) : PieceSize;
        public ulong ForeSequenceMinimumSize => HasForePiece ? Math.Min(PieceSize, BackPiece.ForeSequenceMinimumSize) : PieceSize;

        public string RegularSize => ConvertService.ToUniversalUnit(Size, 2, "", "B");
        public string RegularPieceSize => ConvertService.ToUniversalUnit(PieceSize, 2, "", "B");
        public string RegularForeSequenceSize => ConvertService.ToUniversalUnit(ForeSequenceSize, 2, "", "B");
        public string RegularBackSequenceSize => ConvertService.ToUniversalUnit(BackSequenceSize, 2, "", "B");

        /// <summary>
        /// ⚑: Has MetaData
        /// ⛓: Has Link
        /// ⚡: Is Changed
        /// ⚠: Is Not Valid
        /// ⛣: Is Counting
        /// </summary>
        public string DiagnosticsSigns => GetDiagnostics(true, false, "", "");
        public string DiagnosticsLabels => GetDiagnostics(false, true, "", ", ");
        public string Description => string.Format("{0}; {1}C; {2}L; {3}W; {4}", Name, Length, LinesCount, WarpsCount, DiagnosticsSigns);
        public string GetDiagnostics(bool signs = true, bool labels = true, string betweenSeparator = ": ", string separator = "\r\n")
        {
            List<string> ls = new List<string>();
            if (IsPieceChanged)
                ls.Add((signs ? "⚡" : "") + betweenSeparator + (labels ? "Is Changed" : ""));
            if (HasMetaData)
                ls.Add((signs ? "⚑" : "") + betweenSeparator + (labels ? "Has Metadata" : ""));
            if (IsChainedPiece)
                ls.Add((signs ? "⛓" : "") + betweenSeparator + (labels ? "Is Chained" : ""));
            if (IsEmpty)
                ls.Add((signs ? "Φ" : "") + betweenSeparator + (labels ? "Is Empty" : ""));
            else if (IsBigPiece)
                ls.Add((signs ? "⚫" : "") + betweenSeparator + (labels ? "Is Big" : ""));
            if (IsCounting)
                ls.Add((signs ? "⏳" : "") + betweenSeparator + (labels ? "In Processing" : ""));
            if (!IsValid)
                ls.Add((signs ? "⚠" : "") + betweenSeparator + (labels ? "Not Valid" : ""));
            return string.Join(separator, ls);
        }
        #endregion


        #region CONSTRUCTORS

        [Obsolete("This constractor is obsoleted! Please Use another constractors!", false)]
        public ChainedFile() : this(PathService.CreateValidPathName(Config.TemporaryDirectory ?? Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), DateTime.Now.Ticks + "", ".tsv", false), true)
        {
        }
        public ChainedFile(string[] paths, string newPath)
        {
            if (paths.Length > 0)
            {
                Open(newPath, true);
                foreach (var item in paths)
                    AppendForePiece(item);
                Save();
                Sleep();
            } else Open(newPath, false);
            Init();
        }
        public ChainedFile(ChainedFile file, string path = null)
        {
            Open(file, path);
            Init();
        }
        public ChainedFile(string path, bool createifnotExist = true)
        {
            Open(path, createifnotExist);
            Init();
        }
        public ChainedFile(string path, Encoding encoding, bool createifnotExist = true)
        {
            Encoding = encoding;
            Open(path, createifnotExist);
            Encoding = encoding;
            Init();
        }
        public ChainedFile(string path, ChainedFile cf, string warpsSeparator = null, string linesSeparator = null)
        {
            Open(path);
            WarpsSplitter = warpsSeparator ?? WarpsSplitter;
            LinesSplitter = linesSeparator ?? LinesSplitter;
            Clear();
            WriteLines(cf.ReadLines());
            Save();
            Sleep();
            Init();
        }
        public ChainedFile(string path, IEnumerable<string> lines, string warpsSeparator = null, string linesSeparator = null)
        {
            Open(path);
            WarpsSplitter = warpsSeparator ?? WarpsSplitter;
            LinesSplitter = linesSeparator ?? LinesSplitter;
            Clear();
            WriteLines(lines);
            Save();
            Sleep();
            Init();
        }
        public ChainedFile(string path, IEnumerable<IEnumerable<string>> cells, string warpsSeparator = null, string linesSeparator = null)
        {
            Open(path);
            WarpsSplitter = warpsSeparator ?? WarpsSplitter;
            LinesSplitter = linesSeparator ?? LinesSplitter;
            Clear();
            WriteRows(cells);
            Save();
            Sleep();
            Init();
        }
        public ChainedFile(string path, System.Data.DataSet ds, string warpsSeparator = null, string linesSeparator = null) : this(path, ds.Tables.Cast<System.Data.DataTable>(), warpsSeparator, linesSeparator)
        {
        }
        public ChainedFile(string path, IEnumerable<System.Data.DataTable> dts, string warpsSeparator = null, string linesSeparator = null) : this(path, dts.First(), warpsSeparator, linesSeparator)
        {
            foreach (var item in dts.Skip(1)) AppendForePiece(new ChainedFile(item));
        }
        public ChainedFile(string path, System.Data.DataTable dt, string warpsSeparator = null, string linesSeparator = null) : this(path, (new IEnumerable<string>[] { dt.Columns.Cast<DataColumn>().Select(v => v.ColumnName) }).Concat(dt.Rows.Cast<DataRow>().Select(v => from i in v.ItemArray select i + "")), warpsSeparator, linesSeparator)
        {
        }
        public ChainedFile(System.Data.DataTable dt, string warpsSeparator = null, string linesSeparator = null) : this((new IEnumerable<string>[] { dt.Columns.Cast<DataColumn>().Select(v => v.ColumnName) }).Concat(dt.Rows.Cast<DataRow>().Select(v => from i in v.ItemArray select i + "")), warpsSeparator , linesSeparator)
        {
        }
        public ChainedFile(IEnumerable<string> lines, string warpsSeparator = null, string linesSeparator = null) : this()
        {
            WarpsSplitter = warpsSeparator ?? WarpsSplitter;
            LinesSplitter = linesSeparator ?? LinesSplitter;
            Clear();
            WriteLines(lines);
            Save();
            Sleep();
            Init();
        }
        public ChainedFile(IEnumerable<IEnumerable<string>> cells, string warpsSeparator = null, string linesSeparator = null) : this()
        {
            WarpsSplitter = warpsSeparator?? WarpsSplitter;
            LinesSplitter = linesSeparator?? LinesSplitter;
            Clear();
            WriteRows(cells);
            Save();
            Sleep();
            Init();
        }
        public ChainedFile(XmlDocument doc, string warpsSeparator = null, string linesSeparator = null)
        {
            WarpsSplitter = warpsSeparator ?? WarpsSplitter;
            LinesSplitter = linesSeparator ?? LinesSplitter;
            Open(doc);
            Init();
        }
        public ChainedFile(XmlElement doc, string warpsSeparator = null, string linesSeparator = null)
        {
            WarpsSplitter = warpsSeparator ?? WarpsSplitter;
            LinesSplitter = linesSeparator ?? LinesSplitter;
            Open(doc);
            Init();
        }
        private void Init()
        {
            QuoteChars = QuoteChars;
            TrackPiece = () => this;
            switch (Extension.ToLower())
            {
                case ".tsv":
                    WarpsSplitter = "\t";
                    break;
                case ".csv":
                    WarpsSplitter = ",";
                    break;
                case ".ssv":
                    WarpsSplitter = ";";
                    break;
                default:
                    break;
            }
            Book = new Matrices.TextMatrixFile(this, string.Empty, string.Empty);
            Matrix = new Matrices.MathMatrixFile(this, 0, string.Empty);
            ForceMatrix = new Matrices.ForceMathMatrixFile(this, 0, string.Empty);
        }
        #endregion


        #region Access     
        public TextMatrixFile Book;
        public MathMatrixFile Matrix;
        public ForceMathMatrixFile ForceMatrix;
        public Func<ChainedFile> TrackPiece { get; private set; } = () => null;
        //public long TrackBackLinesCount { get; private set; } = 0;
        public long TrackBackLinesCount => TrackPiece().BackSequenceLinesCount;
        public long LastPieceIndex { get; private set; } = 0;
        public long LastLineIndex { get; private set; } = 0;
        public long LastWarpIndex { get; private set; } = 0;

        public RegexOptions DefaultRegexOption { get; set; } = RegexOptions.IgnoreCase;

        public IEnumerable<KeyValuePair<long, IEnumerable<string>>> this[string pattern, long columnInd, string fromRowInd, long limit = -1]
        {
            get => this[pattern, DefaultRegexOption, new long[] { columnInd }, new long[] { GetRowIndex(fromRowInd, DefaultRegexOption != RegexOptions.IgnoreCase) }, limit];
            set => this[pattern, DefaultRegexOption, new long[] { columnInd }, new long[] { GetRowIndex(fromRowInd, DefaultRegexOption != RegexOptions.IgnoreCase) }, limit] = value;
        }
        public IEnumerable<KeyValuePair<long, IEnumerable<string>>> this[string pattern, long[] columnInds, string fromRowInd, long limit = -1]
        {
            get => this[pattern, DefaultRegexOption, columnInds, new long[] { GetRowIndex(fromRowInd, DefaultRegexOption != RegexOptions.IgnoreCase) }, limit];
            set => this[pattern, DefaultRegexOption, columnInds, new long[] { GetRowIndex(fromRowInd, DefaultRegexOption != RegexOptions.IgnoreCase) }, limit] = value;
        }
        public IEnumerable<KeyValuePair<long, IEnumerable<string>>> this[string pattern, long[] columnInds, string[] fromRowInds, long limit = -1]
        {
            get => this[pattern, DefaultRegexOption, columnInds, (from v in fromRowInds select GetRowIndex(v, DefaultRegexOption != RegexOptions.IgnoreCase)).ToArray(), limit];
            set => this[pattern, DefaultRegexOption, columnInds, (from v in fromRowInds select GetRowIndex(v, DefaultRegexOption != RegexOptions.IgnoreCase)).ToArray(), limit] = value;
        }
        public IEnumerable<KeyValuePair<long, IEnumerable<string>>> this[string pattern, string columnInd, long fromRowInd, long limit = -1]
        {
            get => this[pattern, DefaultRegexOption, new long[] { GetColumnIndex(columnInd, DefaultRegexOption != RegexOptions.IgnoreCase) }, new long[] { fromRowInd }, limit];
            set => this[pattern, DefaultRegexOption, new long[] { GetColumnIndex(columnInd, DefaultRegexOption != RegexOptions.IgnoreCase) }, new long[] { fromRowInd }, limit] = value;
        }
        public IEnumerable<KeyValuePair<long, IEnumerable<string>>> this[string pattern, string columnInd, long[] fromRowInds, long limit = -1]
        {
            get => this[pattern, DefaultRegexOption, new long[] { GetColumnIndex(columnInd, DefaultRegexOption != RegexOptions.IgnoreCase) }, fromRowInds, limit];
            set => this[pattern, DefaultRegexOption, new long[] { GetColumnIndex(columnInd, DefaultRegexOption != RegexOptions.IgnoreCase) }, fromRowInds, limit] = value;
        }
        public IEnumerable<KeyValuePair<long, IEnumerable<string>>> this[string pattern, string[] columnInds, long[] fromRowInds, long limit = -1]
        {
            get => this[pattern, DefaultRegexOption, (from v in columnInds select GetColumnIndex(v, DefaultRegexOption != RegexOptions.IgnoreCase)).ToArray(), fromRowInds, limit];
            set => this[pattern, DefaultRegexOption, (from v in columnInds select GetColumnIndex(v, DefaultRegexOption != RegexOptions.IgnoreCase)).ToArray(), fromRowInds, limit] = value;
        }
        public IEnumerable<KeyValuePair<long, IEnumerable<string>>> this[string pattern, string columnInd, string fromRowInd, long limit = -1]
        { get => this[pattern, DefaultRegexOption, new long[] { GetColumnIndex(columnInd, DefaultRegexOption != RegexOptions.IgnoreCase) }, new long[] { GetRowIndex(fromRowInd, DefaultRegexOption != RegexOptions.IgnoreCase) }, limit];
            set => this[pattern, DefaultRegexOption, new long[] { GetColumnIndex(columnInd, DefaultRegexOption != RegexOptions.IgnoreCase) }, new long[] { GetRowIndex(fromRowInd, DefaultRegexOption != RegexOptions.IgnoreCase) }, limit] = value; }
        public IEnumerable<KeyValuePair<long, IEnumerable<string>>> this[string pattern, string[] columnInds, string fromRowInd, long limit = -1]
        { get => this[pattern, DefaultRegexOption, (from v in columnInds select GetColumnIndex(v, DefaultRegexOption != RegexOptions.IgnoreCase)).ToArray(), new long[] { GetRowIndex(fromRowInd, DefaultRegexOption != RegexOptions.IgnoreCase) }, limit];
            set => this[pattern, DefaultRegexOption, (from v in columnInds select GetColumnIndex(v, DefaultRegexOption != RegexOptions.IgnoreCase)).ToArray(), new long[] { GetRowIndex(fromRowInd, DefaultRegexOption != RegexOptions.IgnoreCase) }, limit] = value; }
        public IEnumerable<KeyValuePair<long, IEnumerable<string>>> this[string pattern, string columnInd, string[] fromRowInds, long limit = -1]
        { get => this[pattern, DefaultRegexOption, new long[] { GetColumnIndex(columnInd, DefaultRegexOption != RegexOptions.IgnoreCase) }, (from v in fromRowInds select GetRowIndex(v, DefaultRegexOption != RegexOptions.IgnoreCase)).ToArray(), limit]; 
            set => this[pattern, DefaultRegexOption, new long[] { GetColumnIndex(columnInd, DefaultRegexOption != RegexOptions.IgnoreCase) }, (from v in fromRowInds select GetRowIndex(v, DefaultRegexOption != RegexOptions.IgnoreCase)).ToArray(), limit] = value; }
        public IEnumerable<KeyValuePair<long, IEnumerable<string>>> this[string pattern, string[] columnInds, string[] fromRowInds, long limit = -1]
        { get => this[pattern, DefaultRegexOption, (from v in columnInds select GetColumnIndex(v, DefaultRegexOption != RegexOptions.IgnoreCase)).ToArray(), (from v in fromRowInds select GetRowIndex(v, DefaultRegexOption != RegexOptions.IgnoreCase)).ToArray(), limit]; 
            set => this[pattern, DefaultRegexOption, (from v in columnInds select GetColumnIndex(v, DefaultRegexOption != RegexOptions.IgnoreCase)).ToArray(), (from v in fromRowInds select GetRowIndex(v, DefaultRegexOption != RegexOptions.IgnoreCase)).ToArray(), limit] = value; }
        public IEnumerable<KeyValuePair<long, IEnumerable<string>>> this[string pattern, long columnInd, long fromRowInd, long limit = -1]
        { get => this[pattern, DefaultRegexOption, new long[] { columnInd }, new long[] { fromRowInd }, limit];
            set => this[pattern, DefaultRegexOption, new long[] { columnInd }, new long[] { fromRowInd }, limit] = value; }
        public IEnumerable<KeyValuePair<long, IEnumerable<string>>> this[string pattern, long[] columnInds, long fromRowInd, long limit = -1]
        { get => this[pattern, DefaultRegexOption, columnInds, new long[] { fromRowInd }, limit];
            set => this[pattern, DefaultRegexOption, columnInds, new long[] { fromRowInd }, limit] = value; }
        public IEnumerable<KeyValuePair<long, IEnumerable<string>>> this[string pattern, long columnInd, long[] fromRowInds, long limit = -1]
        { get => this[pattern, DefaultRegexOption, new long[]{columnInd}, fromRowInds, limit];
            set => this[pattern, DefaultRegexOption, new long[] { columnInd }, fromRowInds, limit] = value; }
        public IEnumerable<KeyValuePair<long, IEnumerable<string>>> this[string pattern, long[] columnInds = null, long[] fromRowInds = null, long limit = -1]
        { get => this[pattern, DefaultRegexOption, columnInds, fromRowInds, limit]; 
            set => this[pattern, DefaultRegexOption, columnInds, fromRowInds, limit] = value; }
        public IEnumerable<KeyValuePair<long,IEnumerable<string>>> this[string pattern, RegexOptions option, long[] columnInds = null, long[] fromRowInds = null, long limit = -1]
        {
            get {
                if(limit<0) return FindRows(pattern, option, 5, RowsLabelsIndex + 1, columnInds, fromRowInds);
                return FindRows(pattern, option, 5, RowsLabelsIndex + 1, columnInds, fromRowInds).Take(ConvertService.TryToInt(limit,int.MaxValue)); }
            set
            {
                var rows = this[pattern, option, columnInds, fromRowInds].GetEnumerator();
                var nrows = value.GetEnumerator();
                bool finished = false;
                var f = new Func<bool>(() => { nrows.Reset(); finished = true; return nrows.MoveNext(); });
                var cols = columnInds == null || columnInds.Length == 0 ? new int[0] : (from v in columnInds select ConvertService.TryToInt(v)).ToArray();
                if (cols.Length < 1)
                {
                    while (rows.MoveNext())
                        if (limit-- == 0) return;
                        else if (nrows.MoveNext() || f())
                            ChangeRow(rows.Current.Key, nrows.Current.Value);
                    if (!finished)
                        while (nrows.MoveNext())
                            if (limit-- == 0) return;
                            else if (nrows.Current.Key > -1) ChangeRow(nrows.Current.Key, nrows.Current.Value);
                }
                else
                {
                    var colsE = cols.AsEnumerable().GetEnumerator();
                    var nrowE = (new string[0]).AsEnumerable().GetEnumerator();
                    var nrowfunc = new Func<bool>(()=> { nrowE.Reset(); return nrowE.MoveNext(); });
                    while (rows.MoveNext())
                        if (limit-- == 0) return;
                        else if (nrows.MoveNext() || f())
                        {
                            var row = rows.Current.Value.ToList();
                            nrowE = nrows.Current.Value.GetEnumerator();
                            while (colsE.MoveNext())
                            {
                                if (!nrowE.MoveNext() || !nrowfunc()) break;
                                while (colsE.Current < row.Count) row.Add(null);
                                row[colsE.Current] = nrowE.Current;
                            }
                            colsE.Reset();
                            ChangeRow(rows.Current.Key, row);
                        }
                    if (!finished)
                        while (nrows.MoveNext())
                            if (limit-- == 0) return;
                            else if (nrows.Current.Key > -1)
                            {
                                var row = ReadRow(nrows.Current.Key).ToList();
                                nrowE = nrows.Current.Value.GetEnumerator();
                                while (colsE.MoveNext())
                                {
                                    if (!nrowE.MoveNext() || !nrowfunc()) break;
                                    while (colsE.Current < row.Count) row.Add(null);
                                    row[colsE.Current] = nrowE.Current;
                                }
                                colsE.Reset();
                                ChangeRow(nrows.Current.Key, row);
                            }
                }
            }
        }
        public IEnumerable<string> this[long row]
        {
            get { return Row(row); }
            set
            {
                ChangeRow(row, value);
            }
        }
        public string this[long col, long row]
        {
            get { return Cell(col, row); }
            set
            {
                ChangeCell(col, row, value);
            }
        }
        public string this[long doc, long col, long row]
        {
            get { return Cell(col, row, doc); }
            set
            {
                ChangeCell(col, row, doc, value);
            }
        }

        public ChainedFile GetPieceByIndex(long index)
        {
            int v = Convert.ToInt32(index);
            var ch = index < 0 ? BackChain.Skip(-v) : ForeChain.Skip(v);
            if (ch.Any()) return ch.First();
            else return null;
        }
        public ChainedFile GetPieceByLine(long fromIndex)
        {
            if (fromIndex < 0)
            {
                //TrackBackLinesCount = LastPiece.BackSequenceLinesCount;
                var tp = LastPiece;
                TrackPiece().ReadsFlush();
                TrackPiece = () => tp;
                LastPieceIndex = LastPiece.PieceIndex;
                fromIndex += LastPiece.LinesCount;
            }
            LastLineIndex = fromIndex;

            if ((fromIndex < 0 || fromIndex >= PieceLinesCount) && IsCountedPiece && HasForePiece)
            {
                ReadsFlush();
                while (fromIndex < TrackBackLinesCount && TrackPiece().HasBackPiece)
                {
                    //TrackBackLinesCount -= TrackPiece().BackPiece.PieceLinesCount;
                    var tp = TrackPiece().BackPiece;
                    TrackPiece().ReadsFlush();
                    TrackPiece = () => tp;
                    LastPieceIndex--;
                }
                while (fromIndex >= TrackBackLinesCount + TrackPiece().PieceLinesCount && TrackPiece().HasForePiece)
                {
                    //TrackBackLinesCount += TrackPiece().PieceLinesCount;
                    var tp = TrackPiece().ForePiece;
                    TrackPiece().ReadsFlush();
                    TrackPiece = () => tp;
                    LastPieceIndex++;
                }
                return TrackPiece().GetPieceByLine(fromIndex - TrackBackLinesCount);
            }
            else
            {
                if (TrackPiece() != this)
                {
                    TrackPiece().ReadsFlushIfNeeds();
                    TrackPiece = () => this;
                    //TrackBackLinesCount = BackSequenceLinesCount;
                }
                LastPieceIndex = 0;
                return this;
            }
        }

        public ChainedFile Piece(long index, ChainedFile d = null)
        {
            return GetPieceByIndex(index) ??d;
        }
        public string Warp(long index) => ReadWarp(index);
        public string Line(long index) => ReadLine(index);
        public string Cell(long col, long row) => ReadCell(col, row);
        public string Cell(long col, long row, long documentIndex) => ReadCell(col, row, documentIndex);
        public IEnumerable<string> Column(string label, bool caseSense = false) => ReadColumn(GetColumnIndex(label, caseSense));
        public IEnumerable<string> Column(long index,long fromRowIndex) => ReadColumn(index, fromRowIndex);
        public IEnumerable<string> Column(long index) => ReadColumn(index);
        public IEnumerable<string> Row(long index, long fromColIndex) => ReadRow(index, fromColIndex);
        public IEnumerable<string> Row(string label, bool caseSense = false) => ReadRow(GetRowIndex(label, caseSense));
        public IEnumerable<string> Row(long index) => ReadRow(index);
        public IEnumerable<KeyValuePair<string, string>> Record(long index,long fromColIndex) => ReadRecord(index, fromColIndex);
        public IEnumerable<KeyValuePair<string, string>> Record(string label, bool caseSense = false) => ReadRecord(GetRowIndex(label, caseSense));
        public IEnumerable<KeyValuePair<string, string>> Record(long index) => ReadRecord(index);
        public string Text { get => ReadText(); set { Clear(); WriteText(value); Save(); } }
        public IEnumerable<string> Warps { get => ReadWarps(); set { Clear(); WriteWarps(value); Save(); } }
        public IEnumerable<string> Lines { get => ReadLines(); set { Clear(); WriteLines(value); Save(); } }
        public IEnumerable<IEnumerable<string>> Columns { get => ReadColumns(FreeColumnsLabels ? 0 : ColumnsLabelsIndex); set { Clear(); WriteColumns(FreeColumnsLabels ? value : new IEnumerable<string>[] { ColumnsLabels }.Concat(value)); Save(); } }
        public IEnumerable<IEnumerable<string>> Rows { get => ReadRows(FreeRowsLabels?0:RowsLabelsIndex); set { Clear(); WriteRows(FreeRowsLabels ? value : new IEnumerable<string>[] {RowsLabels }.Concat(value)); Save(); } }
        public IEnumerable<string> Cells { get => ReadCells(); set { Clear(); WriteCells(value); Save(); } }
        #endregion


        #region MAIN
        public virtual ConnectorBase CreateConnector(string path=null, Encoding encoding = null)
        {
            return new PlainTextFile(path??Path,encoding??Encoding);
        }
        public bool Create()
        {
            if (!File.Exists(Path))
                Connector.CreateNew();
            else return false;
            return true;
        }
        public void New()
        {
            Connector = CreateConnector("",Encoding);
            Restart();
            OnActed("New");
        }
        public void Open(ChainedFile file, string path = null)
        {
            if (path == null)
            {
                Open(file.Path);
                if (file.HasForePiece) ForePiece = new ChainedFile(file.ForePiece);
                LastPieceLinesCount = file.LastPieceLinesCount;
                LastLineIndex = file.LastLineIndex;
                BufferStartLine = file.BufferStartLine;
                IsPieceChanged = file.IsPieceChanged;
                IsCountingPiece = file.IsCountingPiece;
                IsCountedPiece = file.IsCountedPiece;
                PieceReadsBuffer.Clear();
                PieceReadsBuffer.AddRange( file.PieceReadsBuffer);
                PieceWritesBuffer.Clear();
                PieceWritesBuffer.AddRange(file.PieceWritesBuffer);
                PieceUndoBuffer.Clear();
                PieceUndoBuffer.AddRange(file.PieceUndoBuffer);
                PieceRedoBuffer.Clear();
                PieceRedoBuffer.AddRange(file.PieceRedoBuffer);
            }
            else Open(path);
            Encoding = file.Encoding;
            PieceWarpsSplitters = file.PieceWarpsSplitters;
            PieceLinesSplitters = file.PieceLinesSplitters;
        }
        public void Open()
        {
            Open(Path);
        }
        public void Open(XmlDocument doc)
        {
            LoadMetaData(doc);
            try
            {
                Open(Path);
            }
            catch { }
        }
        public void Open(XmlElement doc)
        {
            MetaData = new XmlDocument();
            MetaData.AppendChild(MetaData.CreateElement(doc.Name));
            MetaData.InnerXml = doc.InnerXml;
            Open(MetaData);
        }
        public void Open(string path, bool createIfNotExist = true)
        {
            try
            {
                if (Path != path)
                {
                    string oldpath = Path;
                    Sleep();
                    New();
                    if (System.IO.Path.GetExtension(path).ToLower() == MetaDataExtension)
                        path = path.Substring(0, path.Length - MetaDataExtension.Length);
                    RelativePath = string.IsNullOrWhiteSpace(path) ? "" : System.IO.Path.GetFullPath(path);
                    Connector = CreateConnector(RelativePath, Encoding);
                    if (createIfNotExist) Create();
                    OpenMetaData();
                    OnPathChanged(oldpath);
                }
                else
                {
                    if (createIfNotExist) Create();
                }
                OnActed("Open");
            }
            catch { }
        }
        public void Open(string path, string appendedPath)
        {
            Open(path, new ChainedFile(ConvertPathFromRelative(appendedPath)));
        }
        public void Open(string path, ChainedFile appendedFile)
        {
            Open(path);
            SetForePiece(appendedFile);
        }
        public bool Save() => Save(NeedsMetaData);
        public bool Save(bool withMetadata)
        {
            bool b = true;
            if (PieceWritesBuffer.Count > 0) b = WritesFlush();
            if (!b) return false;
            IsPieceChanged = false;
            if (HasForePiece)
            {
                ForePiece.RelativePath = ConvertPathToRelative(ForePiece.Path);
                b = b && ForePiece.Save(false);
            }
            if (!b) return false;
            if (withMetadata) b = SaveMetaData();
            else
            {
                if (b)
                    if ((HasMetaData || PieceUndoBuffer.Count > 0) && IsFirstPiece)
                        b = ChangesFlush();
                    else b = SetMetaData();
                try { if(b) PathService.DeleteFile(MetaDataPath); } catch { }
            }
            OnActed("Save");
            if (b) IsCountingPiece = false;
            return b;
        }
        public bool Save(string path) => Save(path, NeedsMetaData);
        public bool Save(string path, bool withMetadata)
        {
            if (Path != path && withMetadata)
            {
                string dir = System.IO.Path.GetDirectoryName(path) + System.IO.Path.DirectorySeparatorChar;
                foreach (var item in ForeSequence.Reverse())
                {
                    item.RelativePath = ConvertPathToRelative(item.Path, dir);
                    item.SetMetaData();
                }
                SaveMetaData(path + MetaDataExtension);
                PathService.DeleteFile(MetaDataPath);
            }
            PathService.MoveFile(Path, path, true);
            Open(path);
            if (!Save(withMetadata)) return false;
            OnActed("Save");
            return true;
        }
        public bool SaveAs(string path) => SaveAs(path,NeedsMetaData);
        public bool SaveAs(string path, bool withMetadata)
        {
            if (Path != path && (NeedsMetaData || IsChanged))
            {
                foreach (var item in ForeSequence.Reverse())
                {
                    item.RelativePath = item.Path;
                    item.SetMetaData();
                }
                if(withMetadata) SaveMetaData(path + MetaDataExtension);
            }
            PathService.CopyFile(Path, path, true);
            var op = Path;
            Connector = CreateConnector(path, Encoding);
            OnPathChanged(op);
            if (!Save(withMetadata)) return false;
            OnActed("SaveAs");
            return true;
        }
        public string Send(string dir, bool overrideIfExists)
        {
            if (!Save()) return null;
            return Move(dir, overrideIfExists);
        }
        public bool Send(string path)
        {
            if (!Save()) return false;
            Move(path);
            return true;
        }
        public string Export(string dir, bool overrideIfExists)
        {
            //if (!Save()) return null;
            //return Copy(dir, overrideIfExists);
            string path = overrideIfExists? dir.TrimEnd(System.IO.Path.DirectorySeparatorChar)+ System.IO.Path.DirectorySeparatorChar.ToString() + Name
                : PathService.CreateValidPathName(dir, NameWithoutExtension, Extension, false);
            if(Export(path)) return path;
            else return null;
        }
        public bool Export(string path)
        {
            //if (!Save()) return false;
            //Copy(path);
            switch (System.IO.Path.GetExtension(path))
            {
                case ".xls":
                case ".xlsx":
                    ConvertService.ToExcelFile(this, path);
                    return true;
                default:
                    return CreateConnector(path, Encoding).WriteLines(ReadLines()) >= LinesCount;
            }
        }
        public bool Stick()
        {
            //bool b = WritesFlush() && ChangesFlush();
            bool b = Save(false);
            //Restore();
            //if (HasForePiece) b &= ForePiece.Stick();
            OnActed("Stick");
            return b;
        }
        public void Sleep()
        {
            IsCountingPiece = false;
            if (HasForePiece) ForePiece.Sleep();
            OnActed("Sleep");
        }
        public void Hibernate()
        {
            IsCountingPiece = false;
            ReadsFlush();
            if (HasForePiece) ForePiece.Hibernate();
            OnActed("Hibernate");
        }
        public void Restore()
        {
            IsPieceChanged = false;
            IsCountingPiece = false;
            ReadsFlush();
            PieceWritesBuffer.Clear();
            PieceUndoBuffer.Clear();
            PieceRedoBuffer.Clear();
            LastUndoBufferID = 0;
            LastRedoBufferID = 0;
            if (HasForePiece) ForePiece.Restore();
            OnActed("Restore");
        }
        public void Restart()
        {
            IsPieceChanged = false;
            IsCountingPiece = false;
            IsCountedPiece = false;
            LastPieceWarpsCount = 0;
            LastPieceLinesCount = 0;
            PieceWarpsCount = 0;
            PieceLinesCount = 0;
            ReadsFlush();
            PieceWritesBuffer.Clear();
            PieceUndoBuffer.Clear();
            PieceRedoBuffer.Clear();
            LastUndoBufferID = 0;
            LastRedoBufferID = 0;
            if (HasForePiece) ForePiece.Restart();
            OnActed("Restart");
        }
        public bool DeletePiece()
        {
            if (Freeze) return false;
            try
            {
                ProcessService.ReduceMemory(true);
                PathService.DeleteFile(Path);
                if (HasMetaData) PathService.DeleteFile(MetaDataPath);
                IsPieceChanged = false;
                IsCountingPiece = false;
                IsCountedPiece = false;
                LastPieceWarpsCount = 0;
                LastPieceLinesCount = 0;
                PieceLinesCount = 0;
                ReadsFlush();
                PieceWritesBuffer.Clear();
                PieceUndoBuffer.Clear();
                PieceRedoBuffer.Clear();
                LastUndoBufferID = 0;
                LastRedoBufferID = 0;
            }
            catch(Exception ex)
            {
                if (TryAgainJob()) return DeletePiece();
                else return false;
            }
            OnActed("DeletePiece");
            return true;
        }
        public bool Delete()
        {
            if (Freeze) return false;
            DeletePiece();
            if (HasForePiece) ForePiece.Delete();
            OnActed("Delete");
            return true;
        }
        public void ClearPiece()
        {
            if (Freeze) return;
            try
            {
                ProcessService.ReduceMemory(true);
                PathService.DeleteFile(Path);
                if (HasMetaData) PathService.DeleteFile(MetaDataPath);
                IsPieceChanged = false;
                IsCountingPiece = false;
                IsCountedPiece = false;
                LastPieceWarpsCount = 0;
                LastPieceLinesCount = 0;
                PieceWarpsCount = 0;
                PieceLinesCount = 0;
                ReadsFlush();
                PieceWritesBuffer.Clear();
                PieceUndoBuffer.Clear();
                PieceRedoBuffer.Clear();
                LastUndoBufferID = 0;
                LastRedoBufferID = 0;
            }
            catch
            {
                if (TryAgainJob()) ClearPiece();
            }
            Create();
            OnActed("ClearPiece");
        }
        public void Clear()
        {
            ClearPiece();
            if (HasForePiece) ForePiece.Clear();
            OnActed("Clear");
        }
        public void Copy(string newPath)
        {
            if (HasForePiece) ForePiece.Copy(System.IO.Path.GetDirectoryName(newPath) + System.IO.Path.DirectorySeparatorChar.ToString() + ForePiece.Name);
            if (Path != newPath)
            {
                PathService.CopyFile(Path, newPath, true);
                RelativePath = System.IO.Path.GetFileName(newPath);
                if (HasMetaData) SaveMetaData(newPath + MetaDataExtension);
                else SetMetaDataProperties();
                OnActed("Copy");
            }
        }
        public string Copy(string dir, bool overrideIfExists)
        {
            dir = System.IO.Path.GetFullPath(dir).TrimEnd('\\') + "\\";
            if (HasForePiece) ForePiece.Copy(dir, overrideIfExists);
            string newPath = dir + Name;
            if (!overrideIfExists)
            {
                int i = 1;
                while (File.Exists(newPath))
                    newPath = dir + NameWithoutExtension + i++ + Extension;
            }
            PathService.CopyFile(Path, newPath, overrideIfExists);
            RelativePath = System.IO.Path.GetFileName(newPath);
            if (HasMetaData) SaveMetaData(newPath + MetaDataExtension);
            else SetMetaDataProperties();
            OnActed("Copy");
            return newPath;
        }
        public void Move(string newPath)
        {
            if (Freeze) return;
            if (HasForePiece) ForePiece.Move(System.IO.Path.GetDirectoryName(newPath) + System.IO.Path.DirectorySeparatorChar.ToString() + ForePiece.Name);
            if (Path != newPath)
            {
                PathService.MoveFile(Path, newPath, true);
                SetMetaData(MetaDataProperties(), "RelativePath", Name);
                if (HasMetaData)
                {
                    RelativePath = System.IO.Path.GetFileName(newPath);
                    if (HasMetaData) SaveMetaData(newPath + MetaDataExtension);
                    else SetMetaDataProperties();
                    PathService.DeleteFile(MetaDataPath);
                }
                Open(newPath);
                OnActed("Move");
            }
        }
        public string Move(string dir, bool overrideIfExists)
        {
            if (Freeze) return Path;
            dir = System.IO.Path.GetFullPath(dir).TrimEnd(System.IO.Path.DirectorySeparatorChar) + System.IO.Path.DirectorySeparatorChar.ToString();
            if (HasForePiece) ForePiece.Move(dir, overrideIfExists);
            string newPath = dir + Name;
            if (!overrideIfExists)
            {
                int i = 1;
                while (File.Exists(newPath))
                    newPath = dir + NameWithoutExtension + i++ + Extension;
            }
            if (Path != newPath)
            {
                PathService.MoveFile(Path, newPath, overrideIfExists);
                SetMetaData(MetaDataProperties(), "RelativePath", Name);
                if (HasMetaData)
                {
                    RelativePath = System.IO.Path.GetFileName(newPath);
                    if (HasMetaData) SaveMetaData(newPath + MetaDataExtension);
                    else SetMetaDataProperties();
                    PathService.DeleteFile(MetaDataPath);
                }
                Open(newPath);
            }
            OnActed("Move");
            return Path;
        }
        public void Rename(string newName)
        {
            if (Freeze) return;
            Save();
            string newPath = Directory + newName;
            if (Path != newPath)
            {
                PathService.MoveFile(Path, newPath, true);
                SetMetaData(MetaDataProperties(), "RelativePath", Name);
                if (HasMetaData)
                {
                    RelativePath = System.IO.Path.GetFileName(newPath);
                    if (HasMetaData) SaveMetaData(newPath + MetaDataExtension);
                    else SetMetaDataProperties();
                    PathService.DeleteFile(MetaDataPath);
                }
                Open(newPath);
                OnActed("Rename");
            }
        }
        public void ToClipBoard()
        {
            Clipboard.SetText(ReadText());
            OnActed("ToClipBoard");
        }
        public long FromClipBoard()
        {
            OnActed("FromClipBoard");
            return WriteText(Clipboard.GetText());
        }
        public void Update()
        {
            try
            {
                WarpsSplitters = new string[] { WarpsSplittersSuggests().First().Key };
                LinesSplitters = new string[] { LinesSplittersSuggests().First().Key };
                ColumnsLabelsIndex = WarpsLabelsIndexSuggests().First().Key;
                //RowsLabelsIndex = LinesLabelsIndexSuggests().First().Key;
                Connector.UpdateDetectors();
                if (HasForePiece) ForePiece.Update();
            }
            catch { }
        }
        public bool Update(string warpsSeparator, string linesSeparator = "\r\n")
        {
            try
            {
                if (WarpsSplitter != warpsSeparator || LinesSplitter != linesSeparator)
                {
                    var c = new ChainedFile(PieceTemporaryPath);
                    c.Clear();
                    c.WarpsSplitters = new string[] { warpsSeparator };
                    c.LinesSplitters = new string[] { linesSeparator };
                    c.WriteRows(ReadPieceRows());
                    c.Save();
                    c.Move(Path);
                }
                if (HasForePiece) return ForePiece.Update(warpsSeparator, linesSeparator);
                return true;
            }
            catch { }
            return false;
        }
        #endregion


        #region BUFFERS
        public bool IsBuffered { get; private set; } = false;

        public static int BufferMaxLinesCapasity { get; set; } = 100000;
        public static int BufferMinLinesCapasity { get; set; } = 20;
        public int BufferMaxLinesNumber => !RAMAvailable && _BufferMaxLinesNumber > BufferMinLinesCapasity ? _BufferMaxLinesNumber /= 2 :
           _BufferMaxLinesNumber <= BufferMaxLinesCapasity ? _BufferMaxLinesNumber += BufferMinLinesCapasity : _BufferMaxLinesNumber;
        private int _BufferMaxLinesNumber = BufferMaxLinesCapasity;
        public List<string> PieceWritesBuffer { get; private set; } = new List<string>();
        public int LastUndoBufferID { get => FirstPiece._LastUndoBufferID; private set => FirstPiece._LastUndoBufferID = value; }
        private int _LastUndoBufferID = 0;
        public long LastUndoFromLineIndex =>
            IsPieceUndoAble && PieceUndoBuffer.Last().ID >= LastUndoBufferID ?
                (TrackBackLinesCount + PieceUndoBuffer.Last().From) :
                HasForePiece ?
                    ForePiece.LastUndoFromLineIndex :
                    HasForePiece ?
                        ForePiece.LastUndoFromLineIndex :
                        -1;
        public long LastUndoToLineIndex =>
            IsPieceUndoAble && PieceUndoBuffer.Last().ID >= LastUndoBufferID ?
                (/*BackSequenceLinesCount*/ TrackBackLinesCount + PieceUndoBuffer.Last().To) :
                HasForePiece ?
                    ForePiece.LastUndoToLineIndex :
                    HasForePiece ?
                        ForePiece.LastUndoToLineIndex :
                        -1;
        public FilterItem? LastUndoFilter => 
            IsPieceUndoAble && PieceUndoBuffer.Last().ID >= LastUndoBufferID?
                PieceUndoBuffer.Last(): 
                HasForePiece? 
                    ForePiece.LastUndoFilter :
                    HasForePiece?
                        ForePiece.LastUndoFilter : null;
        public FilterCollection PieceUndoBuffer { get; private set; } = new FilterCollection();
        public int UndoBufferCount => PieceUndoBuffer.Count + (HasForePiece ? ForePiece.UndoBufferCount : 0);
        public long LastRedoFromLineIndex =>
            IsPieceRedoAble && PieceRedoBuffer.Last().ID >= LastRedoBufferID ?
                (TrackBackLinesCount + PieceRedoBuffer.Last().From) :
                HasForePiece ?
                    ForePiece.LastRedoFromLineIndex :
                    HasForePiece ?
                        ForePiece.LastRedoFromLineIndex :
                        -1; 
        public long LastRedoToLineIndex =>
            IsPieceRedoAble && PieceRedoBuffer.Last().ID >= LastRedoBufferID ?
                (TrackBackLinesCount + PieceRedoBuffer.Last().To) :
                HasForePiece ?
                    ForePiece.LastRedoToLineIndex :
                    HasForePiece ?
                        ForePiece.LastRedoToLineIndex :
                        -1; 
        public int LastRedoBufferID { get => FirstPiece._LastRedoBufferID; private set => FirstPiece._LastRedoBufferID = value; }
        private int _LastRedoBufferID = 0; 
        public FilterItem? LastRedoFilter => 
            IsPieceRedoAble && PieceRedoBuffer.Last().ID >= LastRedoBufferID?
                PieceRedoBuffer.Last(): 
                HasForePiece? 
                    ForePiece.LastRedoFilter:
                    HasForePiece?
                        ForePiece.LastRedoFilter: null;
        public FilterCollection PieceRedoBuffer { get; private set; } = new FilterCollection();
        public int RedoBufferCount => PieceRedoBuffer.Count + (HasForePiece ? ForePiece.RedoBufferCount : 0);
        public long BufferStartLine { get; private set; } = 0;
        public long BufferEndLine => BufferStartLine + PieceReadsBuffer.Count;
        public List<string> PieceReadsBuffer { get; private set; } = new List<string>();
        public static int BufferCountLimit { get; set; } = 100;


        public void RestartReadsBuffer()
        {
            IsCountingPiece = false;
            IsCountedPiece = false;
            LastPieceLinesCount = 0;
            ReadsFlush();
            if (HasForePiece) ForePiece.RestartReadsBuffer();
            OnActed("RestartReadsBuffer");
        }
        public void RestartChangesBuffer()
        {
            IsCountingPiece = false;
            IsCountedPiece = false;
            LastPieceWarpsCount = 0;
            LastPieceLinesCount = 0;
            PieceUndoBuffer.Clear();
            PieceRedoBuffer.Clear();
            LastUndoBufferID = 0;
            LastRedoBufferID = 0;
            if (HasForePiece) ForePiece.RestartChangesBuffer();
            OnActed("RestartChangesBuffer");
        }
        public void RestartWritesBuffer()
        {
            IsCountingPiece = false;
            IsCountedPiece = false;
            LastPieceLinesCount = 0;
            PieceWritesBuffer.Clear();
            if (HasForePiece) ForePiece.RestartWritesBuffer();
            OnActed("RestartWritesBuffer");
        }
        public bool ChangesFlush()
        {
            if (Freeze) return false;
            if (Path == null)
            {
                PieceUndoBuffer.Clear();
                PieceRedoBuffer.Clear();
                LastUndoBufferID = 0;
                LastRedoBufferID = 0;
            }
            else
            {
                string newPath = Directory + "" + Name + "";
                try
                {
                    //IOService.ToFile(newPath, ReadPieceLines(), Encoding);
                    CreateConnector(newPath, Encoding).WriteLines(ReadPieceLines());
                    if (DeletePiece())
                    {
                        PathService.MoveFile(newPath, Path, true);
                        if (HasForePiece) ForePiece.ChangesFlush();
                        OnActed("ChangesFlush");
                        PieceUndoBuffer.Clear();
                        PieceRedoBuffer.Clear();
                        LastUndoBufferID = 0;
                        LastRedoBufferID = 0;
                        return true;
                    }
                    return false;
                }
                catch(Exception ex)
                {
                    if (PieceUndoBuffer.Count > 0 && TryAgainJob()) return ChangesFlush();
                }
                finally { PathService.DeleteFile(newPath); }
            }
            return false;
        }
        public bool ChangesFlushIfNeeds()
        {
            if (PieceUndoBuffer.Count > BufferMaxLinesNumber) return ChangesFlush();
            return false;
        }
        public bool WritesFlush()
        {
            if (Freeze) return false;
            if (Path == null)
                PieceWritesBuffer.Clear();
            else
                try
                {
                    if (!IsPieceExist || IsPieceEmpty)
                    {
                        PathService.CreateAllDirectories(Directory);
                        if (PieceLinesSplitters.Length < 1 || PieceLinesSplitter == Environment.NewLine)
                            Connector.WriteLines(PieceWritesBuffer);
                        else Connector.WriteText(Connector.LinesToText(PieceWritesBuffer));
                    }
                    else
                    {
                        ChainedFile last;
                        if (IsSequenceTurn) last = AppendForePiece();
                        else last = this;
                        if (PieceLinesSplitters.Length < 1 || PieceLinesSplitter == Environment.NewLine)
                            last.CreateConnector().AppendLines(PieceWritesBuffer);
                        else Connector.AppendText(PieceLinesSplitter + Connector.LinesToText(PieceWritesBuffer));
                    }
                    PieceWritesBuffer.Clear();
                    OnActed("WritesFlush");
                    return true;
                }
                catch (Exception ex)
                {
                    if (PieceWritesBuffer.Count > 0 && TryAgainJob()) return WritesFlush();
                }
                return false;
        }
        public bool WritesFlushIfNeeds()
        {
            if (PieceWritesBuffer.Count > BufferMaxLinesNumber) return WritesFlush();
            return false;
        }
        public bool ReadsFlush()
        {
            if (Freeze) return false;
            PieceReadsBuffer.Clear();
            //TrackPiece = () => this;
            //LastPieceIndex = 0;
            return true;
        }
        public bool ReadsFlushIfNeeds()
        {
            if (Freeze) return false;
            PieceReadsBuffer.Clear();
            //TrackPiece = () => this;
            //LastPieceIndex = 0; 
            if (HasBackPiece) BackPiece.ReadsFlushIfNeeds();
            return true;
        }

        public bool ResetReadBuffer(long fromIndex) => ResetReadBuffer(ReadLines(), fromIndex);
        public bool ResetReadBuffer(IEnumerable<string> lines, long fromIndex)
        {
            IsBuffered = true;
            int buffcap = BufferMaxLinesNumber;
            BufferStartLine = fromIndex - (buffcap / 2);
            if (BufferStartLine < 0) BufferStartLine = 0;
            ReadsFlushIfNeeds();
            PieceReadsBuffer = new List<string>(lines.Skip((int)BufferStartLine).Take(buffcap));
            return PieceReadsBuffer.Count > 0;
        }
        public bool PrependReadBuffer(long fromIndex) => PrependReadBuffer(ReadLines(), fromIndex);
        public bool PrependReadBuffer(IEnumerable<string> lines, long fromIndex)
        {
            long bind = fromIndex - (BufferMaxLinesNumber / 2);
            if (bind < 0) bind = 0;
            int count = PieceReadsBuffer.Count;
            PieceReadsBuffer.InsertRange(0, lines.Skip((int)bind).Take((int)(BufferStartLine - bind)));
            PieceReadsBuffer.RemoveRange(count, PieceReadsBuffer.Count - count);
            bool b = BufferStartLine > bind;
            BufferStartLine = bind;
            return b;

            //long bind = fromIndex - (BufferMaxLinesNumber / 2);
            //if (bind < 0) bind = 0;
            //long si = bind;
            //int i = 0;
            //foreach (var item in lines.Skip((int)bind))
            //{
            //    if (si++ < BufferStartLine)
            //    {
            //        ReadsBuffer.Insert(i++, item);
            //        if (ReadsBuffer.Count > BufferMaxLinesNumber)
            //            ReadsBuffer.RemoveAt(ReadsBuffer.Count - 1);
            //    }
            //    else break;
            //}
            //bool b = BufferStartLine > bind;
            //BufferStartLine = bind;
            //return b;
        }
        public bool AppendReadBuffer(long fromIndex) => AppendReadBuffer(ReadLines(), fromIndex);
        public bool AppendReadBuffer(IEnumerable<string> lines, long fromIndex)
        {
            int count = PieceReadsBuffer.Count;
            PieceReadsBuffer.AddRange(lines.Skip((int)BufferStartLine + count).Take(BufferMaxLinesNumber / 2));
            count = PieceReadsBuffer.Count - count;
            PieceReadsBuffer.RemoveRange(0, count);
            BufferStartLine += count;
            return count > 0;

            //bool b = false;
            //foreach (var line in lines.Skip((int)BufferStartLine + ReadsBuffer.Count).Take(BufferMaxLinesNumber / 2))
            //{
            //    ReadsBuffer.Add(line);
            //    if (ReadsBuffer.Count > BufferMaxLinesNumber)
            //    {
            //        ReadsBuffer.RemoveAt(0);
            //        BufferStartLine++;
            //    }
            //    b = true;
            //}
            //return b;
        }
        public IEnumerable<string> ReadSampleLines(int linesNum = 5)
        {
            IEnumerable<string> lines = ReadLines();
            int num = 0;
            int ln = linesNum;
            while (ln > num)
            {
                int ind = num * ln;
                while (ind > -1)
                {
                    IEnumerable<string> ss = lines.Skip(ind);
                    if (ss.Any())
                        if (string.IsNullOrEmpty(ss.First()))
                            ind++;
                        else
                        {
                            yield return ss.First();
                            num++;
                            break;
                        }
                    else
                    {
                        ln--;
                        num = 1;
                        break;
                    }
                }
            }
        }
        #endregion


        #region FUNCTIONS
        private long LastTick = 0;
        private bool TryAgainJob()
        {
            ProcessService.ReduceMemory(false);
            long newTick = DateTime.Now.Ticks;
            if (newTick - LastTick > 6000000) LastTick = newTick;
            bool b = newTick - LastTick < 5000000 && RAMFreeCapacity > RAMMinUseableCapacity;
            if (!b) LastTick = 0;
            return b;
        }

        public void SetFeatures(ChainedFile piece)
        {
            piece.Encoding = Encoding;
            piece.FreeColumnsLabels = FreeColumnsLabels;
            piece.FreeRowsLabels = FreeRowsLabels;
            piece.PieceColumnsLabelsIndex = PieceColumnsLabelsIndex;
            piece.PieceLinesSplitters = PieceLinesSplitters;
            piece.PieceWarpsSplitters = PieceWarpsSplitters;
            if (piece.HasForePiece) piece.SetFeatures(piece.ForePiece);
        }
        public void SetEvents(ChainedFile piece)
        {
            piece.RelativePath = ConvertPathToRelative(piece.Path);
            piece.LinesCounting = (a, e) => LinesCounting(a, e);
            piece.LinesCounted = (a, e) => LinesCounted(a, e);
            piece.WarpsCounted = (a, e) => WarpsCounted(a, e);
            piece.WarpsCounting = (a, e) => WarpsCounting(a, e);
            piece.LinesChanged = (a, e) => LinesChanged(a, e);
            piece.WarpsChanged = (a, e) => WarpsChanged(a, e);
            piece.Changed = (a, e) => Changed(a,e);
            if (piece.HasForePiece) piece.SetEvents(piece.ForePiece);
        }

        public string CreateNewSequencePath()
        {
            long num = 1;
            string path;
            while (File.Exists(path = Directory + NameWithoutExtension.Split(new string[]{"→"},StringSplitOptions.None).First() + "→" + num + Extension))
                num++;
            return path;
        }
        public string ConvertPathFromRelative(string path)
        {
            if (System.IO.Path.IsPathRooted(path)) return path;
            else return Directory + (path??"").TrimStart('\\');
        }
        public string ConvertPathToRelative(string path)
        {
            return ConvertPathToRelative(path, Directory);
        }
        public static string ConvertPathToRelative(string path, string dir)
        {
            return path.StartsWith(dir) ? path.Substring(dir.Length) : path;
        }
        #endregion


        #region WRITE
        public int WritePieceLine(string line)
        {
            PieceWritesBuffer.Add(line);
            WritesFlushIfNeeds();
            OnLinesChanged(1);
            return 1;
        }
        public int WritePieceLine(params string[] cells)
        {
            return WritePieceLine(Connector.JoinToLine(cells));
        }
        public int WritePieceRow(IEnumerable<string> cells)
        {
            return WritePieceLine(Connector.RowToLine(cells));
        }
        public long WritePieceLines(IEnumerable<string> lines)
        {
            int counter = 0;
            foreach (var line in lines)
                counter += WritePieceLine(line);
            return counter;
        }
        public long WritePieceRows(IEnumerable<IEnumerable<string>> lineCells)
        {
            return WritePieceLines(from v in lineCells select Connector.RowToLine(v));
        }

        public int WriteLine(string line)
        {
            if (HasForePiece) return ForePiece.WriteLine(line);
            return WritePieceLine(line);
        }
        public int WriteLine(params string[] cells)
        {
            return WriteLine(Connector.JoinToLine(cells));
        }
        public long WriteLines(IEnumerable<string> lines)
        {
            long counter = 0;
            foreach (var line in lines)
                counter += WriteLine(line);
            return counter;
        }

        public int WriteRow(params string[] cells) => WriteRow((IEnumerable<string>)cells);
        public int WriteRow(params KeyValuePair<string, string>[] cells)
        {
            var labels = ColumnsLabels.ToList();
            List<string> row = new List<string>(labels.Count);
            foreach (var item in cells)
            {
                int ind = labels.FindIndex(v=>v == item.Key);
                if (ind < 0)
                {
                    labels.Add(item.Key);
                    ColumnsLabels = labels;
                    ind = labels.Count - 1;
                }
                while (ind >= row.Count) row.Add("");
                row[ind] = item.Value;
            }
            return WriteRow(row);
        }
        public int WriteRow(params KeyValuePair<int, string>[] cells)
        {
            List<string> row = new List<string>();
            foreach (var item in cells)
            {
                int ind = item.Key;
                while (ind >= row.Count) row.Add("");
                row[ind] = item.Value;
            }
            return WriteRow(row);
        }
        public int WriteRow(IEnumerable<string> cells)
        {
            return WriteLine(Connector.RowToLine(cells));
        }
        public long WriteRows(IEnumerable<IEnumerable<string>> lineCells)
        {
            return WriteLines(from v in lineCells select Connector.RowToLine(v));
        }

        public long WriteWarp(string warp)
        {
            return WriteColumn(Connector.WarpToColumn(warp));
        }
        public long WriteWarps(IEnumerable<string> warps)
        {
            long counter = 0;
            foreach (var warp in warps)
                counter += WriteColumn(Connector.WarpToColumn(warp));
            return counter;
        }

        public long WriteColumn(params string[] cells) => WriteColumn((IEnumerable<string>)cells);
        public long WriteColumn(IEnumerable<string> warpCells)
        {
            long l = -1;
            foreach (var item in warpCells)
                ChangeLine(++l, Connector.RowToLine(ReadRow(l).Concat(new string[] { item })));
            return l;
        }
        public long WriteColumn(params KeyValuePair<string, string>[] cells)
        {
            var labels = RowsLabels.ToList();
            List<string> col = new List<string>(labels.Count);
            foreach (var item in cells)
            {
                int ind = labels.FindIndex(v => v == item.Key);
                if (ind < 0)
                {
                    labels.Add(item.Key);
                    RowsLabels = labels;
                    ind = labels.Count - 1;
                }
                while (ind >= col.Count) col.Add("");
                col[ind] = item.Value;
            }
            return WriteColumn(col);
        }
        public long WriteColumn(params KeyValuePair<int, string>[] cells)
        {
            List<string> col = new List<string>();
            foreach (var item in cells)
            {
                int ind = item.Key;
                while (ind >= col.Count) col.Add("");
                col[ind] = item.Value;
            }
            return WriteColumn(col);
        }
        public long WritePieceColumn(IEnumerable<string> warpCells)
        {
            long l = -1;
            foreach (var item in warpCells)
                ChangePieceLine(++l, Connector.RowToLine(ReadPieceRow(l).Concat(new string[] { item })));
            return l;
        }
        public long WriteColumns(IEnumerable<IEnumerable<string>> warpsCells)
        {
            int i = 0;
            long num = 0;
            if (warpsCells.Any())
                while (warpsCells.First().Skip(i).Any())
                {
                    List<string> ls = new List<string>();
                    foreach (var item in warpsCells)
                    {
                        var it = item.Skip(i);
                        if (it.Any()) ls.Add(it.First());
                        else break;
                    }
                    num += ChangeRow(i, ReadRow(i).Concat(ls));
                    i++;
                }
            return num;
        }
        public long WritePieceColumns(IEnumerable<IEnumerable<string>> warpsCells)
        {
            int i = 0;
            long num = 0;
            while (warpsCells.Any() && warpsCells.First().Skip(i).Any())
            {
                List<string> ls = new List<string>();
                foreach (var item in warpsCells)
                {
                    var it = item.Skip(i);
                    if (it.Any()) ls.Add(it.First());
                    else break;
                }
                num += WritePieceLine(Connector.RowToLine(ls));
                i++;
            }
            return num;
        }

        public long WriteCells(params string[] cells) => WriteCells((IEnumerable<string>)cells);
        public long WriteCells(IEnumerable<string> cells)
        {
            return  WriteLine(Connector.RowToLine(cells));
        }
        public long WritePieceCells(IEnumerable<string> cells)
        {
            return WritePieceLine(Connector.RowToLine(cells));
        }

        public long WriteFrom(string path)
        {
            if (File.Exists(path))
                return WriteLines((new ChainedFile(path)).ReadLines());
            return 0;
        }

        public long WriteText(string text)
        {
            if (Freeze) return 0;
            BreakSequence();
            Restore();
            if (Path != null) Connector.WriteText(text);
            else return 0;
            OnChanged(long.MaxValue);
            IsCountedPiece = false;
            return 1;
        }
        #endregion


        #region UPDATE
        public long UpdateLines(Func<long,string,string> func, int fromIndex=0, int toIndex=-1)
        {
            long changes = 0;
            ChainedFile temp = new ChainedFile(PieceTemporaryPath);
            IEnumerable<string> file = ReadLines();
            foreach (var item in file.Take(fromIndex))
                temp.WriteLine(item);
            foreach (var item in file.Skip(fromIndex))
            {
                temp.WriteLine(func(fromIndex, item));
                changes++;
                if (++fromIndex == toIndex) break;
            }
            if (toIndex > -1)
                foreach (var item in file.Skip(toIndex))
                    temp.WriteLine(item);
            Delete();
            temp.Save();
            temp.Move(Path);
            OnLinesChanged(changes);
            return changes;
        }
        public long UpdateRows(Func<long, IEnumerable<string>, IEnumerable<string>> func, int fromIndex = 0, int toIndex = -1)
        {
            return UpdateLines((l,line)=> Connector.RowToLine(func(l, Connector.LineToRow(line))), fromIndex, toIndex);
        }
        #endregion


        #region INSERT
        public long InsertLines(long fromIndex, IEnumerable<string> lines)
        {
            long l = 0;
            foreach (var line in lines)
                l += InsertLine(fromIndex++, line);
            return l;
        }
        public long InsertLines(long fromIndex, int toIndex, string line)
        {
            List<string> ls = new List<string>();
            for (long i = fromIndex; i < toIndex; i++)
                ls.Add(line);
            return InsertLines(fromIndex,ls);
        }
        public long InsertLine(long index,string line = "")
        {
            var lp = GetPieceByLine(index);
            return lp.InsertPieceLine(lp.LastLineIndex, line);
        }
        public long InsertRows(long index, IEnumerable<IEnumerable<string>> rows)
        {
            return InsertLines(index, from cells in rows select Connector.RowToLine(cells));
        }
        public long InsertRow(long index, IEnumerable<string> cells)
        {
            return InsertLine(index, Connector.RowToLine(cells));
        }

        public long InsertPieceLines(long fromIndex, IEnumerable<string> lines)
        {
            long l = 0;
            foreach (var line in lines)
                l += InsertPieceLine(fromIndex++, line);
            return l;
            //List<string> ls = lines.ToList();
            //AddFilterItem(new FilterItem(TableChangeMode.Insert, fromIndex, fromIndex + ls.Count, ls));
            //OnChanged(ls.Count);
            //return ls.Count;
        }
        public long InsertPieceLines(long fromIndex, int toIndex, string line)
        {
            long l = 0;
            for (long i = fromIndex; i < toIndex; i++)
                l += InsertPieceLine(i,line);
            return l;
            //AddFilterItem(new FilterItem(TableChangeMode.Insert, fromIndex, toIndex, line));
            //OnChanged(counter);
            //return counter;
        }
        public long InsertPieceLine(long index, IEnumerable<string> cells)
        {
            return InsertPieceLine(index, Connector.RowToLine(cells));
        }
        public long InsertPieceLine(long index, string line)
        {
            ReadsFlushIfNeeds();
            //return lp.ChangePieceLine(index,line + LinesSplitter + ReadLine(index));
            return WritePieceLine(line);
            //AddFilterItem(new FilterItem(TableChangeMode.Insert, index, line));
            //OnChanged(1);
            //return 1;
        }


        public long InsertWarps(long fromIndex, IEnumerable<string> warps)
        {
            return InsertColumns(fromIndex, from warp in warps select Connector.WarpToColumn(warp));
        }
        public long InsertWarps(long fromIndex, int toIndex, string warp)
        {
            var col = Connector.WarpToColumn(warp);
            List<IEnumerable<string>> ls = new List<IEnumerable<string>>();
            for (long i = fromIndex; i < toIndex; i++)
                ls.Add(col);
            return InsertColumns(fromIndex, ls);
        }
        public long InsertWarp(long index, string warp = "")
        {
            return InsertColumn(index, Connector.WarpToColumn(warp));
        }
        public long InsertColumns(long index, IEnumerable<IEnumerable<string>> warpsCells)
        {
            //if (!IsSmall) return 0;
            ReadsFlushIfNeeds();
            if(LinesCount > 0) ChangeRow(0, CollectionService.TakeOrDefault(ReadRow(0),WarpsCount+1));
            //int i = 0;
            //int ind = Convert.ToInt32(index);
            //while (warpsCells.Any() && warpsCells.First().Skip(i).Any())
            //{
            //    List<string> ls = ReadRow(i).ToList();
            //    int cind = ind;
            //    foreach (var item in warpsCells)
            //    {
            //        var it = item.Skip(i);
            //        while (cind >= ls.Count) ls.Add("");
            //        ls.Insert(cind, it.Any() ? it.First():"");
            //        cind++;
            //    }
            //    ChangeRow(i,ls);
            //    i++;
            //}
            //var length = warpsCells.Count();
            //string[] sa = new string[length];
            //for (; i < LinesCount; i++)
            //{
            //    List<string> ls = ReadRow(i).ToList();
            //    while (ind >= ls.Count) ls.Add("");
            //    ls.InsertRange(ind, sa);
            //    ChangeRow(i, ls);
            //}
            return LinesCount;
        }
        public long InsertColumn(long index, IEnumerable<string> cells)
        {
            return InsertColumns(index,new IEnumerable<string>[] { cells });
        }
        #endregion


        #region CHANGE
        public void AddFilterItem(FilterItem filterItem)
        {
            PieceUndoBuffer.AddItem(filterItem);
            PieceRedoBuffer.Clear();
            LastRedoBufferID = 0;
            ReadsFlushIfNeeds();
            ChangesFlushIfNeeds();
        }

        public long ChangeLines(long fromIndex, string line)
        {
            var lp = GetPieceByLine(fromIndex);
            var ls = new string[] { line };
            return lp.HasForePiece ?
                lp.ChangePieceLines(fromIndex, fromIndex, ls) + lp.ForePiece.ChangeLines(0, line) :
                lp.ChangePieceLines(fromIndex, fromIndex, ls);
        }
        public long ChangeLines(long fromIndex, long toIndex, IEnumerable<string> lines)
        {
            var lp = GetPieceByLine(fromIndex);
            long tl = toIndex - PieceLinesCount;
            if (tl > 0)
                return lp.HasForePiece ?
                    lp.ChangePieceLines(fromIndex, PieceLinesCount, lines.Take((int)PieceLinesCount)) 
                        + lp.ForePiece.ChangeLines(0, tl, lines.Skip((int)PieceLinesCount)) :
                    lp.ChangePieceLines(fromIndex, toIndex, lines);
            else return lp.ChangePieceLines(fromIndex, toIndex, lines);
        }
        public long ChangeLine(long index, string line)
        {
            var lp = GetPieceByLine(index);
            return lp.ChangePieceLine(lp.LastLineIndex, line);
        }
        public long ChangeRows(long fromIndex, IEnumerable<string> cells)
        {
            var lp = GetPieceByLine(fromIndex);
            var ls = new IEnumerable<string>[] { cells };
            return lp.HasForePiece ?
                lp.ChangePieceRows(fromIndex, fromIndex, ls) 
                    + lp.ForePiece.ChangeRows(0, cells) :
                lp.ChangePieceRows(fromIndex, fromIndex, ls);
        }
        public long ChangeRows(long fromIndex, long toIndex, IEnumerable<IEnumerable<string>> lines)
        {
            var lp = GetPieceByLine(fromIndex);
            long tl = toIndex - PieceLinesCount;
            if (tl > 0)
                return lp.HasForePiece ?
                    lp.ChangePieceRows(fromIndex, PieceLinesCount, lines.Take((int)PieceLinesCount))
                        + lp.ForePiece.ChangeRows(0, tl, lines.Skip((int)PieceLinesCount)) :
                    lp.ChangePieceRows(fromIndex, toIndex, lines);
            else return lp.ChangePieceRows(fromIndex, toIndex, lines);
        }
        public long ChangeRows(IEnumerable<IEnumerable<string>> rows, params long[] indexes)
        {
            if (!rows.Any()) return 0;
            long l = 0;
            IEnumerator<IEnumerable<string>> iecliplinescells = rows.GetEnumerator();
            if (indexes == null || indexes.Length == 0)
                for (int i = 0; i < LinesCount; i++)
                    if (iecliplinescells.MoveNext())
                        l+= ChangeRow(i, iecliplinescells.Current);
                    else break;
            else
                for (int i = 0; i < indexes.Length; i++)
                    if (iecliplinescells.MoveNext())
                        l += ChangeRow(indexes[i], iecliplinescells.Current);
                    else break;
            return l;
        }
        public long ChangeRow(long index, IEnumerable<string> cells)
        {
            var lp = GetPieceByLine(index);
            return lp.ChangePieceRow(lp.LastLineIndex, cells);
        }
        public long ChangeWarp(long index, string warp)
        {
            return ChangeColumn(index, Connector.WarpToColumn(warp));
        }
        public long ChangeColumn(long index, IEnumerable<string> cells)
        {
            if (!IsSmall) return 0;
            long l = 0;
            int i = Convert.ToInt32(index);
            var ce = cells.GetEnumerator();
            string n = "";
            foreach (var row in ReadRows())
            {
                string p = row.ElementAtOrDefault(i);
                if (ce.MoveNext()) n = ce.Current;
                else n = "";
                if (p != n) ChangeCell(i, l, n);
                l++;
            }
            return l;
        }
        public long ChangeColumns(IEnumerable<IEnumerable<string>> cols, params long[] indexes)
        {
            if (!IsSmall) return 0;
            if (!cols.Any()) return 0;
            long l = 0;
            int i = 0;
            bool hasrow = false;
            bool hasInd = indexes != null && indexes.Length > 0;
            IEnumerator<IEnumerable<string>> iecliplinescells = cols.GetEnumerator();
            IEnumerator<long> ieindex = hasInd? indexes.AsEnumerable().GetEnumerator() : MiMFa.Statement.Loop(0,WarpsCount,v=>v).GetEnumerator();
            foreach (var item in ReadRows())
            {
                var row = item.ToList();
                hasrow = false;
                while (iecliplinescells.MoveNext())
                    if (ieindex.MoveNext())
                    {
                        int c = Convert.ToInt32(ieindex.Current);
                        string cell = iecliplinescells.Current.ElementAtOrDefault(i);
                        while (c >= row.Count) row.Add(null);
                        row[c] = cell;
                    }
                    else break;
                ieindex = hasInd? indexes.AsEnumerable().GetEnumerator() : MiMFa.Statement.Loop(0,WarpsCount,v=>v).GetEnumerator();
                iecliplinescells = cols.GetEnumerator();
                l += ChangeRow(i, row);
                i++;
            }
            return l;
        }
        public long ChangeCell(LongPoint position, string value)
        {
            return ChangeCell(position.X, position.Y, value);
        }
        public long ChangeCell(int colIndex, long rowIndex, string value)
        {
            var row = ReadRow(rowIndex);
            return ChangeRow(rowIndex, CollectionService.TakeOrDefault(row,colIndex).Concat(new string[] { value }).Concat(row.Skip(colIndex + 1)));
        }
        public long ChangeCell(long colIndex, long rowIndex, string value)
        {
            return ChangeCell(Convert.ToInt32(colIndex),rowIndex, value);
        }
        public long ChangeCell(long colIndex, long rowIndex, long documentIndex, string value)
        {
            if (documentIndex > 0)
            {
                if (!HasForePiece) AppendForePiece();
                return ForePiece.ChangeCell(colIndex, rowIndex, documentIndex - 1, value);
            }
            return ChangeCell(colIndex, rowIndex, value);
        }
        public long ChangeCells(IEnumerable<string> cells, params LongPoint[] indexes)
        {
            if (!cells.Any()) return 0;
            long l = 0;
            IEnumerator<string> iecliplinescells = cells.GetEnumerator();
            if (indexes == null || indexes.Length == 0)
                ChangeRows(CollectionService.Split(cells, Convert.ToInt32(WarpsCount)), null);
            else
                for (int i = 0; i < indexes.Length; i++)
                    if (iecliplinescells.MoveNext())
                        l += ChangeCell(indexes[i], iecliplinescells.Current);
                    else
                    {
                        iecliplinescells = cells.GetEnumerator();
                        i--;
                    }
            return l;
        }


        public long ChangePieceLines(long fromIndex, string line)
        {
            return ChangePieceLines(fromIndex, fromIndex, new string[] { line });
        }
        public long ChangePieceLines(long fromIndex, long toIndex, IEnumerable<string> lines)
        {
            long l = toIndex - fromIndex;
            if (l < 0) l = LastPieceLinesCount - fromIndex;
            AddFilterItem(new FilterItem(TableChangeMode.Modify, fromIndex, fromIndex + l, lines.ToList(), ++LastUndoBufferID));
            l = PieceUndoBuffer.Last().ChangeNumber;
            OnLinesChanged(l);
            return l;
        }
        public long ChangePieceLine(long index, string line)
        {
            AddFilterItem(new FilterItem(TableChangeMode.Modify, index, ++LastUndoBufferID, line));
            long l = PieceUndoBuffer.Last().ChangeNumber;
            OnLinesChanged(l);
            return l;
        }
        public long ChangePieceRow(long index, IEnumerable<string> cells)
        {
            return ChangePieceLine(index, Connector.RowToLine(cells));
        }
        public long ChangePieceRows(long fromIndex, IEnumerable<string> cells)
        {
            return ChangePieceRows(fromIndex, fromIndex, new string[1][] { cells.ToArray() });
        }
        public long ChangePieceRows(long fromIndex, long toIndex, IEnumerable<IEnumerable<string>> lines)
        {
            return ChangePieceLines(fromIndex, toIndex, from v in lines select Connector.RowToLine(v));
        }
        public long ChangePieceWarp(long index, string warp)
        {
            return ChangePieceColumn(index, Connector.WarpToColumn(warp));
        }
        public long ChangePieceColumn(long index, IEnumerable<string> cells)
        {
            if (!IsSmallPiece) return 0;
            long l = 0;
            int i = Convert.ToInt32(index);
            var ce = cells.GetEnumerator();
            string n = "";
            foreach (var row in ReadPieceRows())
            {
                string p = row.ElementAtOrDefault(i);
                if (ce.MoveNext()) n = ce.Current;
                else n = "";
                if (p != n) ChangePieceCell(i, l, n);
                l++;
            }
            return l;
        }
        public long ChangePieceCell(int colIndex, long rowIndex, string value)
        {
            var row = ReadPieceRow(rowIndex);
            return ChangePieceRow(rowIndex, row.Take(colIndex).Concat(new string[] { value }).Concat(row.Skip(colIndex + 1)));
        }
        public long ChangePieceCell(long colIndex, long rowIndex, string value)
        {
            var row = ReadPieceRow(rowIndex);
            int ci = Convert.ToInt32(colIndex);
            return ChangePieceRow(rowIndex, row.Take(ci).Concat(new string[] { value }).Concat(row.Skip(ci + 1)));
        }
        #endregion


        #region DELETE
        public long DeleteLines(long fromIndex, long toIndex)
        {
            var lp = GetPieceByLine(fromIndex);
            long tl = toIndex - PieceLinesCount;
            if (tl > 0)
                return lp.HasForePiece ?
                    lp.DeletePieceLines(fromIndex, PieceLinesCount) + lp.ForePiece.DeleteLines(0,tl) :
                    lp.DeletePieceLines(fromIndex, toIndex);
            else return lp.DeletePieceLines(fromIndex, toIndex);
        }
        public long DeleteLine(long index)
        {
            var lp = GetPieceByLine(index);
            return lp.DeletePieceLine(lp.LastLineIndex);
        }

        public long DeletePieceLines(long fromIndex, long toIndex)
        {
            long l = toIndex - fromIndex;
            if (l < 0) l = LastPieceLinesCount - fromIndex;
            AddFilterItem(new FilterItem(TableChangeMode.Delete, fromIndex, fromIndex + l, ++LastUndoBufferID));
            l = PieceUndoBuffer.Last().ChangeNumber;
            OnLinesChanged(l);
            return l;
        }
        public long DeletePieceLine(long index)
        {
            AddFilterItem(new FilterItem(TableChangeMode.Delete, index, ++LastUndoBufferID));
            long l = PieceUndoBuffer.Last().ChangeNumber;
            OnLinesChanged(l);
            return l;
        }

        //public long DeleteWarps(long fromIndex, long toIndex)
        //{
        //    DeletePieceWarps(fromIndex, toIndex);
        //    if (HasForePiece) return (fromIndex - toIndex) + ForePiece.DeleteWarps(fromIndex, toIndex);
        //    return fromIndex-toIndex;
        //}
        //public long DeleteWarp(long index)
        //{
        //    DeletePieceWarp(index);
        //    if(HasForePiece) return -1 + ForePiece.DeleteWarp(index);
        //    return -1;
        //}
        //public long DeletePieceWarps(long fromIndex, long toIndex)
        //{
        //    try
        //    {
        //        int find = Convert.ToInt32(fromIndex);
        //        int tind = Convert.ToInt32(toIndex) + 1;
        //        CreateConnector(PieceTemporaryPath, Encoding).WriteLines(from row in ReadPieceRows() let v = row.ToList() select RowToLine(v.Take(find).Concat(v.Skip(tind))));
        //        if (DeletePiece())
        //            if (PathService.CopyFile(PieceTemporaryPath, Path, true))
        //                PathService.DeleteFile(PieceTemporaryPath);
        //        OnWarpsChanged(find - tind);
        //        return find - tind;
        //    }
        //    catch { }
        //    return 0;
        //}
        //public long DeletePieceWarp(long index)
        //{
        //    return DeletePieceWarps(index, index);
        //}


        public long DeleteWarps(IEnumerable<long> indices)
        {
            return DeleteColumns(from i in indices select Convert.ToInt32(i));
        }
        public long DeleteWarps(IEnumerable<int> indices)
        {
            return DeleteColumns(indices);
        }
        public long DeleteWarps(long fromIndex, long toIndex)
        {
            return DeleteColumns(fromIndex, toIndex);
        }
        public long DeleteWarp(long index)
        {
            return DeleteColumn(index);
        }
        public long DeleteColumns(IEnumerable<long> indices)
        {
            return DeleteColumns(from i in indices select Convert.ToInt32(i));
        }
        public long DeleteColumns(IEnumerable<int> indices)
        {
            if (!IsSmall) return 0;
            ReadsFlushIfNeeds();
            var cols = indices.Distinct().ToList();
            int i = 0;
            foreach (var row in ReadRows(0))
            {
                int c = 0;
                ChangeRow(i++, from v in row where !cols.Contains(c++) select v);
            }
            OnWarpsChanged(-cols.Count());
            return LinesCount;
        }
        public long DeleteColumns(long fromIndex, long toIndex)
        {
            if (!IsSmall) return 0;
            ReadsFlushIfNeeds();
            int from = Convert.ToInt32(Math.Min(fromIndex, toIndex));
            int to = Convert.ToInt32(Math.Max(fromIndex, toIndex))+1;
            int i = 0;
            foreach (var row in ReadRows(0))
            {
                var ls = row.ToList();
                ChangeRow(i++, ls.Take(from).Concat(ls.Skip(to)));
            }
            OnWarpsChanged(from - to);
            return LinesCount;
        }
        public long DeleteColumn(long index)
        {
            return DeleteColumns(index,index);
        }


        public long DeletePieceWarps(IEnumerable<long> indices)
        {
            return DeletePieceColumns(from i in indices select Convert.ToInt32(i));
        }
        public long DeletePieceWarps(IEnumerable<int> indices)
        {
            return DeletePieceColumns(indices);
        }
        public long DeletePieceWarps(long fromIndex, long toIndex)
        {
            return DeletePieceColumns(fromIndex, toIndex);
        }
        public long DeletePieceWarp(long index)
        {
            return DeletePieceColumn(index);
        }
        public long DeletePieceColumns(IEnumerable<long> indices)
        {
            return DeletePieceColumns(from i in indices select Convert.ToInt32(i));
        }
        public long DeletePieceColumns(IEnumerable<int> indices)
        {
            if (!IsSmallPiece) return 0;
            ReadsFlushIfNeeds();
            var cols = indices.Distinct().ToList();
            int i = 0;
            foreach (var row in ReadPieceRows())
            {
                int c = 0;
                ChangePieceRow(i++, from v in row where !cols.Contains(c++) select v);
            }
            OnWarpsChanged(-cols.Count());
            return LinesCount;
        }
        public long DeletePieceColumns(long fromIndex, long toIndex)
        {
            if (!IsSmallPiece) return 0;
            ReadsFlushIfNeeds();
            int from = Convert.ToInt32(Math.Min(fromIndex, toIndex));
            int to = Convert.ToInt32(Math.Max(fromIndex, toIndex)) + 1;
            int i = 0;
            foreach (var row in ReadPieceRows())
            {
                var ls = row.ToList();
                ChangePieceRow(i++, ls.Take(from).Concat(ls.Skip(to)));
            }
            OnWarpsChanged(from - to);
            return LinesCount;
        }
        public long DeletePieceColumn(long index)
        {
            return DeletePieceColumns(index, index);
        }

        #endregion



        #region HISTORY
        //public List<int> Colls { get; set; } = null;
        //public long RestructLines(params int[] colls)
        //{
        //    if (Colls == null) Colls = colls.ToList();
        //    else
        //    {
        //        List<int> list = new List<int>();
        //        foreach (var item in colls)
        //            if (item < Colls.Count) list.Add(Colls[item]);
        //            else list.Add(item);
        //        Colls = list;
        //    }
        //    OnChanged(LastCount);
        //    return LastCount;
        //}
        public bool IsPieceChanged { get; private set; } = false;
        public bool IsChanged => IsPieceChanged || (HasForePiece? ForePiece.IsPieceChanged : false);

        public bool IsPieceUndoAble => PieceUndoBuffer.Count > 0;
        public bool IsUndoAble => IsPieceUndoAble || (HasForePiece ? ForePiece.IsUndoAble : false);
        public bool IsPieceRedoAble => PieceRedoBuffer.Count > 0;
        public bool IsRedoAble => IsPieceRedoAble || (HasForePiece? ForePiece.IsRedoAble: false);

        public bool Undo(long fromIndex) => GetPieceByLine(fromIndex).Undo();
        public bool Undo()
        {
            if (IsPieceUndoAble && PieceUndoBuffer.Last().ID >= LastUndoBufferID)
            {
                PieceRedoBuffer.Add(PieceUndoBuffer.Last());
                PieceUndoBuffer.RemoveAt(PieceUndoBuffer.Count - 1);
                LastRedoBufferID = PieceRedoBuffer.Last().ID;
                LastUndoBufferID--;
                ReadsFlushIfNeeds();
                OnLinesChanged(-PieceRedoBuffer.Last().ChangeNumber);
                return IsUndoAble;
            }
            if (HasForePiece)
                if (ForePiece.Undo())
                    return true;
                else if (--LastUndoBufferID > 0)
                    return Undo();
                else LastUndoBufferID = 0;
            else if (--LastUndoBufferID > 0)
                return Undo();
            else LastUndoBufferID = 0;
            return false; 
        }
        public bool Redo(long fromIndex) => GetPieceByLine(fromIndex).Redo();
        public bool Redo()
        {
            if (IsPieceRedoAble && PieceRedoBuffer.Last().ID >= LastRedoBufferID)
            {
                PieceUndoBuffer.Add(PieceRedoBuffer.Last());
                PieceRedoBuffer.RemoveAt(PieceRedoBuffer.Count - 1);
                LastUndoBufferID = PieceUndoBuffer.Last().ID;
                LastRedoBufferID--;
                ReadsFlushIfNeeds();
                OnLinesChanged(PieceUndoBuffer.Last().ChangeNumber);
                return IsRedoAble;
            }
            if (HasForePiece)
                if (ForePiece.Redo())
                    return true;
                else if (--LastRedoBufferID > 0)
                    return Redo();
                else LastRedoBufferID = 0;
            else if (--LastRedoBufferID > 0)
                return Redo();
            else LastRedoBufferID = 0;
            return false;
        }
        #endregion


        #region READ
        public IEnumerable<IEnumerable<string>> Fetch(params LongPoint[] positions)
        {
            if (positions == null || positions.Length == 0) yield break;

            long minX = WarpsCount;
            long minY = LinesCount;
            long maxX = -1;
            long maxY = -1;
            foreach (var cell in positions)
            {
                if (cell.X > -1)
                {
                    minX = Math.Min(minX, cell.X);
                    maxX = Math.Max(maxX, cell.X);
                }
                if (cell.Y > -1)
                {
                    minY = Math.Min(minY, cell.Y);
                    maxY = Math.Max(maxY, cell.Y);
                }
            }
            // All Rows
            if ((minX >= WarpsCount && minY >= LinesCount) || (minX < 0 && minY >= LinesCount) || (minX >= WarpsCount && minY < 0))
            {
                foreach (var row in ReadRows()) yield return row;
                yield break;
            }
            // Selected Rows
            else if (minX < 0 && minY < LinesCount)
            {
                foreach (var row in ReadRows((from v in positions select v.Y).ToList()))
                    yield return row;
                yield break;
            }
            // Selected Columns
            else if (minX >= 0 && minY >= LinesCount)
            {
                foreach (var col in ReadColumns((from v in positions where v.X > -1 select Convert.ToInt32(v.X)).ToList()))
                    yield return col;
                yield break;
            }
            // selected Cells
            List<long> blkl = new List<long>();
            foreach (var c in positions)
                if (!blkl.Contains(c.Y))
                {
                    var mcells = (from mc in positions where c.Y == mc.Y && mc.X > -1 select Convert.ToInt32(mc.X)).ToList();
                    if (mcells.Any()) yield return ReadRow(c.Y, mcells);
                    else yield return ReadRow(c.Y);
                    blkl.Add(c.Y);
                }
            yield break;
        }

        public string ReadPieceRawText()
        {
            if (PieceWritesBuffer.Count > 0)
                return Connector.LinesToText(new string[]{Connector.ReadText(), Connector.LinesToText(PieceWritesBuffer)});
            else return Connector.ReadText();
        }
        public IEnumerable<string> ReadPieceBlockRawLines()
        {
            if (Freeze) return PieceReadsBuffer;
            try
            {
                if (PieceLinesSplitters.Length < 1 || PieceLinesSplitter == Environment.NewLine)
                    if (IsBigPiece) return Connector.ReadLines().Concat(PieceWritesBuffer);
                    else return Connector.ReadAllLines().Concat(PieceWritesBuffer);
                else
                    return Connector.ReadText().Split(PieceLinesSplitters, StringSplitOptions.None).Concat(PieceWritesBuffer);
            }
            catch (DirectoryNotFoundException) { return new string[] { }; }
            catch (FileNotFoundException) { return new string[] { }; }
            catch (Exception ex)
            {
                if (TryAgainJob()) return ReadPieceBlockRawLines();
                return new string[] { };
            }
        }
        public IEnumerable<string> ReadPieceRawLines()
        {
            if (Freeze) return PieceReadsBuffer;
            try
            {
                if (PieceLinesSplitters.Length < 1 || PieceLinesSplitter == Environment.NewLine)
                    return Connector.ReadLines().Concat(PieceWritesBuffer);
                else
                    return Connector.ReadText().Split(PieceLinesSplitters, StringSplitOptions.None).Concat(PieceWritesBuffer);
            }
            catch (DirectoryNotFoundException) { return new string[] { }; }
            catch (FileNotFoundException) { return new string[] { }; }
            catch (Exception)
            {
                if (TryAgainJob()) return ReadPieceRawLines();
                return new string[] { };
            }
        }
        public IEnumerable<string> ReadRawLines()
        {
            //if (Freeze) return ReadsBuffer;
            if (HasForePiece) return ReadPieceRawLines().Concat(ForePiece.ReadRawLines());
            else return ReadPieceRawLines();
        }

        public string ReadPieceText()
        {
            if (IsPieceChanged) return Connector.LinesToText(ReadPieceLines());
            else return ReadPieceRawText();
        }
        public string ReadText()
        {
            if (HasForePiece) return MiMFa.Statement.Apply(v=>string.IsNullOrEmpty(v) ? ForePiece.ReadText() : Connector.LinesToText(v, ForePiece.ReadText()), ReadPieceText());
            else return ReadPieceText();
        }

        public IEnumerable<string> ReadPieceLines()
        {
            return PieceUndoBuffer.Count < 1? ReadPieceRawLines() : PieceUndoBuffer.Filter(ReadPieceRawLines());
        }
        public string ReadPieceLine(long index)
        {
            var lines = ReadPieceLines().Skip(Convert.ToInt32(index));
            if (lines.Any()) return lines.First();
            //foreach (var item in ReadLines()) if (index-- == 0) return item;
            return null;
        }
        public string ReadLine(long index)
        {
            var lines = ReadLines(index);
            if (lines.Any()) return lines.First();
            //foreach (var item in ReadLines()) if (index-- == 0) return item;
            return null;
        }
        public IEnumerable<string> ReadLines()
        {
            if (HasForePiece)
                return ReadPieceLines().Concat(ForePiece.ReadLines());
            return ReadPieceLines();
        }
        public IEnumerable<string> ReadLines(long fromIndex)
        {
            var lp = GetPieceByLine(fromIndex);
            if (lp.LastLineIndex < BufferMinLinesCapasity)
                return lp.ReadLines().Skip((int)lp.LastLineIndex);
            return lp.ReadLines(lp.ReadLines(), lp.LastLineIndex);
        }
        public IEnumerable<string> ReadLines(long fromIndex, int count)
        {
            if (count > 0) return ReadLines(fromIndex).Take(count);
            else return ReadLines(fromIndex);
        }
        public IEnumerable<string> ReadLines(List<int> indexes)
        {
            int len = indexes.Count;
            List<string> stra = new List<string>(new string[len]);
            int findex = -1;
            int index = 0;
            foreach (var item in ReadLines())
            {
                while ((findex = indexes.FindIndex(i => i == index)) > -1)
                {
                    if(findex == 0)
                    {
                        stra.RemoveAt(0);
                        indexes.RemoveAt(0);
                        yield return item;
                    }
                    else stra[findex] = item;
                    if (--len < 0) break;
                }
                index++;
            }
            foreach (var item in stra)
                yield return item;
        }
        public IEnumerable<string> ReadLines(List<long> indexes)
        {
            return ReadLines(indexes.Select(v=> (int)v).ToList());
        }
        public IEnumerable<string> ReadLines(IEnumerable<string> lines,long fromIndex)
        {
            if (LastLinesCount > 0 && fromIndex >= LastLinesCount) yield break;
            LastLineIndex = fromIndex;

            if (!IsBuffered) ResetReadBuffer(lines, fromIndex);
            else if (fromIndex < BufferStartLine)
                if (BufferStartLine - fromIndex > (PieceReadsBuffer.Count / 2))
                    ResetReadBuffer(lines, fromIndex);
                else if (!PrependReadBuffer(lines, fromIndex)) yield break;

            foreach (var line in PieceReadsBuffer.Skip((int)(fromIndex - BufferStartLine)))
                yield return line;
            if(PieceReadsBuffer.Count > fromIndex - BufferStartLine)
                fromIndex += PieceReadsBuffer.Count - (fromIndex - BufferStartLine);

            if (fromIndex >= BufferStartLine)
                if (fromIndex - BufferStartLine > PieceReadsBuffer.Count + (PieceReadsBuffer.Count / 2))
                { 
                    if (!ResetReadBuffer(lines, fromIndex)) yield break;
                    else if(fromIndex - BufferStartLine > PieceReadsBuffer.Count + (PieceReadsBuffer.Count / 2))
                        yield break;
                }
                else if (!AppendReadBuffer(lines, fromIndex)) yield break;

            foreach (var line in ReadLines(lines,fromIndex))
                yield return line;
        }
        public IEnumerable<string> ReadLines(string pattern)
        {
            Regex re =  new Regex(Regex.Escape(pattern));
            try
            {
                re = new Regex(pattern);
            }
            catch { }
            string word = pattern.ToLower();
            return from line in ReadLines() where re.IsMatch(line) || line.ToLower().Contains(word) select line;
        }
        public IEnumerable<string> ReadLines(string pattern, long fromIndex)
        {
            Regex re = new Regex(Regex.Escape(pattern));
            try
            {
                re = new Regex(pattern);
            }
            catch { }
            string word = pattern.ToLower();
            return from line in ReadLines(fromIndex) where re.IsMatch(line) || line.ToLower().Contains(word) select line;
        }


        public IEnumerable<KeyValuePair<string, string>> ReadRecord(string rowLabel, bool caseSensitive = false, bool forceLabeled = true)
        {
            return ReadRecord(GetRowIndex(rowLabel, caseSensitive), forceLabeled);
        }
        public IEnumerable<KeyValuePair<string, string>> ReadRecord(long index, long fromColIndex, bool forceLabeled = true)
        {
            return ReadRecord(index, forceLabeled).Skip(Convert.ToInt32(fromColIndex));
        }
        public IEnumerable<KeyValuePair<string, string>> ReadRecord(long index = 0,bool forceLabeled = true)
        {
            return ReadRecords(index, forceLabeled).First();
        }
        public IEnumerable<IEnumerable<KeyValuePair<string, string>>> ReadRecords(long fromIndex = 0,bool forceLabeled = true)
        {
            List<string> labels;
            if (forceLabeled) labels = ForcePieceColumnsLabels.ToList();
            else labels = PieceColumnsLabels.ToList();
            foreach (var row in ReadRows(fromIndex))
            {
                int i = 0;
                yield return from v in row select new KeyValuePair<string, string>(labels.ElementAtOrDefault(i++), v);
            }
        }

        public IEnumerable<string> ReadRow(long index)
        {
            return Connector.LineToRow(ReadLine(index));
        }
        public IEnumerable<string> ReadRow(long index, long fromColIndex)
        {
            return ReadRow(index).Skip(Convert.ToInt32(fromColIndex));
        }
        public IEnumerable<string> ReadRow(long fromIndex, List<int> colIndexes)
        {
            return  CollectionService.GetItems(ReadRow(fromIndex), colIndexes);
        }
        public IEnumerable<string> ReadRow(long fromIndex, List<long> colIndexes)
        {
            return  CollectionService.GetItems(ReadRow(fromIndex), (from v in colIndexes select Convert.ToInt32(v)).ToList());
        }
        public IEnumerable<string> ReadPieceRow(long index)
        {
            return Connector.LineToRow(ReadPieceLine(index));
        }
        public IEnumerable<IEnumerable<string>> ReadRows()
        {
            return from line in ReadLines() select Connector.LineToRow(line);
        }
        public IEnumerable<IEnumerable<string>> ReadRows(long fromIndex)
        {
            return from line in ReadLines(fromIndex) select Connector.LineToRow(line);
        }
        public IEnumerable<IEnumerable<string>> ReadRows(int fromIndex, int count)
        {
            return from line in ReadLines(fromIndex, count) select Connector.LineToRow(line);
        }
        public IEnumerable<IEnumerable<string>> ReadRows(List<int> indexes)
        {
            return from line in ReadLines(indexes) select Connector.LineToRow(line);
        }
        public IEnumerable<IEnumerable<string>> ReadRows(long fromIndex, List<int> colIndexes)
        {
            return from row in ReadRows(fromIndex) select CollectionService.GetItems(row, colIndexes);
        }
        public IEnumerable<IEnumerable<string>> ReadRows(long fromIndex, List<long> colIndexes)
        {
            return from row in ReadRows(fromIndex) select CollectionService.GetItems(row, (from v in colIndexes select Convert.ToInt32(v)).ToList());
        }
        public IEnumerable<IEnumerable<string>> ReadRows(List<int> rowIndexes, List<int> colIndexes)
        {
            return from row in ReadRows(rowIndexes) select CollectionService.GetItems(row, colIndexes);
        }
        public IEnumerable<IEnumerable<string>> ReadRows(List<long> rowIndexes,List<long> colIndexes)
        {
            return ReadRows((from v in rowIndexes select Convert.ToInt32(v)).ToList(), (from v in colIndexes select Convert.ToInt32(v)).ToList());
        }
        public IEnumerable<IEnumerable<string>> ReadRows(List<long> indexes)
        {
            return from line in ReadLines(indexes) select Connector.LineToRow(line);
        }
        public IEnumerable<IEnumerable<string>> ReadRows(IEnumerable<string> lines, long fromIndex)
        {
            return from line in ReadLines(lines, fromIndex) select Connector.LineToRow(line);
        }
        public IEnumerable<IEnumerable<string>> ReadRows(string pattern)
        {
            return from line in ReadLines(pattern) select Connector.LineToRow(line);
        }
        public IEnumerable<IEnumerable<string>> ReadRows(string pattern, long fromIndex)
        {
            return from line in ReadLines( pattern, fromIndex) select Connector.LineToRow(line);
        }
        public IEnumerable<IEnumerable<string>> ReadPieceRows()
        {
            return from line in ReadPieceLines() select Connector.LineToRow(line);
        }

        public string ReadWarp(long index, long fromrowIndex = 0)
        {
            var cells = ReadColumn(index, fromrowIndex);
            if (cells.Any())
                return Connector.ColumnToWarp(cells);
            return null;
        }
        public string ReadPieceWarp(long index)
        {
            var cells = ReadPieceColumn(index);
            if (cells.Any())
                return Connector.ColumnToWarp(cells);
            return null;
        }
        public IEnumerable<string> ReadWarps(long fromColIndex = 0, long fromRowIndex = 0)
        {
            for (long i = fromColIndex; i < LastPieceWarpsCount; i++)
                yield return ReadWarp(i, fromRowIndex);
        }
        public IEnumerable<string> ReadPieceWarps(long fromColIndex = 0)
        {
            for (long i = fromColIndex; i < LastPieceWarpsCount; i++)
                yield return ReadPieceWarp(i);
        }
        public IEnumerable<string> ReadWarps(List<int> indexes)
        {
            return from line in ReadColumns(indexes) select Connector.ColumnToWarp(line);
        }
        public IEnumerable<string> ReadWarps(List<long> indexes)
        {
            return ReadWarps(indexes.Select(v => (int)v).ToList());
        }

        public IEnumerable<string> ReadColumn(long index, long fromrowIndex = 0)
        {
            int i = Convert.ToInt32(index);
            return from v in ReadRows(fromrowIndex) select v.ElementAtOrDefault(i);
        }
        public IEnumerable<string> ReadColumn(int index, List<int> rowIndexes)
        {
            return CollectionService.GetItems(ReadColumn(index), rowIndexes);
        }
        public IEnumerable<string> ReadColumn(long index, List<long> rowIndexes)
        {
            return CollectionService.GetItems(ReadColumn(index), (from v in rowIndexes select Convert.ToInt32(v)).ToList());
        }
        public IEnumerable<string> ReadPieceColumn(long index)
        {
            int i = Convert.ToInt32(index);
            return from v in ReadPieceRows() select v.ElementAtOrDefault(i);
        }
        public IEnumerable<IEnumerable<string>> ReadPieceColumns(long fromColIndex = 0)
        {
            for (long i = fromColIndex; i < LastPieceWarpsCount; i++)
                yield return ReadPieceColumn(i);
        }
        public IEnumerable<IEnumerable<string>> ReadColumns(long fromColIndex = 0, long fromRowIndex = 0)
        {
            for (long i = fromColIndex; i < LastPieceWarpsCount; i++)
                yield return ReadColumn(i, fromRowIndex);
        }
        public IEnumerable<IEnumerable<string>> ReadColumns(List<int> indexes, long fromRowIndex = 0)
        {
            int len = indexes.Count;
            List<IEnumerable<string>> stra = new IEnumerable<string>[indexes.Count].ToList();
            int findex = -1;
            int index = 0;
            foreach (var item in ReadColumns(0,fromRowIndex))
            {
                while ((findex = indexes.FindIndex(i => i == index)) > -1)
                {
                    if (findex == 0)
                    {
                        stra.RemoveAt(0);
                        indexes.RemoveAt(0);
                        yield return item;
                    }
                    else stra[findex] = item;
                    if (--len < 0) break;
                }
                index++;
            }
            foreach (var item in stra)
                yield return item;
        }
        public IEnumerable<IEnumerable<string>> ReadColumns(List<long> indexes, long fromRowIndex = 0)
        {
            return ReadColumns(indexes.Select(v => (int)v).ToList(), fromRowIndex);
        }
        public IEnumerable<IEnumerable<string>> ReadColumns(long fromIndex, List<int> rowIndexes)
        {
            return from row in ReadColumns(fromIndex) select CollectionService.GetItems(row, rowIndexes);
        }
        public IEnumerable<IEnumerable<string>> ReadColumns(long fromIndex, List<long> rowIndexes)
        {
            return from row in ReadColumns(fromIndex) select CollectionService.GetItems(row, from v in rowIndexes select Convert.ToInt32(v));
        }
        public IEnumerable<IEnumerable<string>> ReadColumns(List<int> colIndexes, List<int> rowIndexes)
        {
            return from row in ReadColumns(colIndexes) select CollectionService.GetItems(row, rowIndexes);
        }
        public IEnumerable<IEnumerable<string>> ReadColumns(List<long> colIndexes, List<long> rowIndexes)
        {
            return ReadColumns((from v in colIndexes select Convert.ToInt32(v)).ToList(), (from v in rowIndexes select Convert.ToInt32(v)).ToList());
        }

        public string ReadCell(long col, long row)
        {
           return ReadRow(row).ElementAtOrDefault(Convert.ToInt32(col));
        }
        public string ReadCell(long col, long row, long documentIndex)
        {
            var ch = Piece(documentIndex, null);
            if (ch != null) return ch.ReadCell(col, row);
            else return null;
        }
        public string ReadPieceCell(long col, long row)
        {
            return ReadPieceRow(row).ElementAtOrDefault(Convert.ToInt32(col));
        }
        public string ReadPieceCell(long col, long row, long documentIndex)
        {
            var ch = Piece(documentIndex, null);
            if (ch != null) return ch.ReadPieceCell(col, row);
            else return null;
        }
        public IEnumerable<string> ReadCells()
        {
            foreach (var row in ReadRows())
                foreach (var cell in row)
                    yield return cell;
        }
        public IEnumerable<string> ReadPieceCells()
        {
            foreach (var row in ReadPieceRows())
                foreach (var cell in row)
                    yield return cell;
        }
        #endregion


        #region FIND
        public IEnumerable<KeyValuePair<long, IEnumerable<string>>> FindRows(string pattern, long fromIndex, RegexOptions option = RegexOptions.IgnoreCase, int timeoutSeconds = 20)
            => from v in FindLines(pattern, fromIndex, option, timeoutSeconds) select new KeyValuePair<long, IEnumerable<string>>(v.Key, Connector.LineToRow(v.Value));
        public IEnumerable<KeyValuePair<long, string>> FindLines(string pattern, long fromIndex, RegexOptions option = RegexOptions.IgnoreCase, int timeoutSeconds = 20)
        {
            if (string.IsNullOrWhiteSpace(pattern)) return new KeyValuePair<long, string>[0];
            Regex re = new Regex(Connector.EscapeChars(pattern), option,new TimeSpan(0,0,timeoutSeconds));
            return FindLines(re, fromIndex);
        }
        public IEnumerable<KeyValuePair<long, IEnumerable<string>>> FindRows(Regex re, long fromIndex = 0)
             => from v in FindLines(re, fromIndex) select new KeyValuePair<long, IEnumerable<string>>(v.Key, Connector.LineToRow(v.Value));
        public IEnumerable<KeyValuePair<long, string>> FindLines(Regex re, long fromIndex = 0)
        {
            long l = fromIndex-1;
            return from line in ReadLines(fromIndex) where ++l<0 || re.IsMatch(line) select new KeyValuePair<long,string>(l, line);
        }
        public IEnumerable<KeyValuePair<long, IEnumerable<string>>> FindRows(string pattern, RegexOptions option, int timeoutSeconds, long fromIndex, long[] cols, long[] rows, bool isplainText = true)
            => from v in FindLines(pattern, option, timeoutSeconds, fromIndex, cols, rows, isplainText) select new KeyValuePair<long, IEnumerable<string>>(v.Key, Connector.LineToRow(v.Value)); 
        public IEnumerable<KeyValuePair<long, string>> FindLines(string pattern, RegexOptions option, int timeoutSeconds, long fromIndex, long[] cols, long[] rows, bool isplainText = true)
        {
            return FindLines(pattern,  option, timeoutSeconds, fromIndex, cols==null?new int[0]:(from v in cols select Convert.ToInt32(v)).ToArray(), rows == null ? new int[0] : (from v in rows select Convert.ToInt32(v)).ToArray(), isplainText);
        }
        public IEnumerable<KeyValuePair<long, IEnumerable<string>>> FindRows(string pattern, RegexOptions option, int timeoutSeconds, long fromIndex, int[] cols, int[] rows, bool isplainText = true)
            => from v in FindLines(pattern, option, timeoutSeconds, fromIndex, cols, rows, isplainText) select new KeyValuePair<long, IEnumerable<string>>(v.Key, Connector.LineToRow(v.Value)); 
        public IEnumerable<KeyValuePair<long, string>> FindLines(string pattern, RegexOptions option, int timeoutSeconds, long fromIndex, int[] cols, int[] rows, bool isplainText = true)
        {
            cols = cols ?? new int[0];
            rows = rows ?? new int[0];
            if (cols.Length < 1 && rows.Length < 1) foreach (var item in FindLines(pattern, fromIndex, option, timeoutSeconds)) yield return item;
            else if (cols.Length < 1 && rows.Length > 0)
            {
                Regex re = new Regex(Connector.EscapeChars(pattern), option, new TimeSpan(0, 0, timeoutSeconds));
                string line = "";
                IEnumerable<string> lines = ReadLines(fromIndex);
                for (int i = 0; i < rows.Length; i++)
                    if (re.IsMatch(line = lines.ElementAt(rows[i])))
                        yield return new KeyValuePair<long, string>(fromIndex+rows[i], line);
            }
            else if (cols.Length > 0 && rows.Length < 1)
            {
                pattern = CreateColumnsPattern(pattern, cols, isplainText);
                if (!InfoService.IsValidRegexPattern(pattern)) yield break;
                foreach (var item in FindLines(pattern, fromIndex, option, timeoutSeconds)) yield return item;
            }
            else
            {
                pattern = CreateColumnsPattern(pattern,cols, isplainText);
                if (!InfoService.IsValidRegexPattern(pattern)) yield break;
                Regex re = new Regex(pattern, option, new TimeSpan(0, 0, timeoutSeconds));
                string line = "";
                IEnumerable<string> lines = ReadLines(fromIndex);
                for (int i = 0; i < rows.Length; i++)
                    if (re.IsMatch(line = lines.ElementAt(rows[i])))
                        yield return new KeyValuePair<long, string>(fromIndex + rows[i], line);
            }
        }
        #endregion


        #region REPLACE
        public long ReplaceLines(string pattern, string replacement, long fromIndex, RegexOptions option = RegexOptions.IgnoreCase, int timeoutSeconds = 20)
        {
            Regex re = new Regex(pattern, option,new TimeSpan(0,0,timeoutSeconds));
            return ReplaceLines(re, replacement,fromIndex);
        }
        public long ReplaceLines(string pattern, string replacement, RegexOptions option = RegexOptions.IgnoreCase, int timeoutSeconds = 20)
        {
            return ReplaceLines(pattern, replacement,0, option, timeoutSeconds);
        }
        public long ReplaceLines(Regex re, string replacement, long fromIndex = 0)
        {
            long l = fromIndex;
            long rl = 0;
            while (l < LinesCount)
            {
                var line = ReadLine(l);
                if (re.IsMatch(line))
                {
                    rl++;
                    ChangeLine(l++, re.Replace(line, replacement));
                }
                else l++;
            }
            return rl;
        }
        //public long ReplaceLines(Regex re, string replacement, long fromIndex = 0)
        //{
        //    long l = 0;
        //    ChainedFile lf = new ChainedFile(PieceTemporaryPath);
        //    foreach (var line in ReadLines().Take((int)fromIndex))
        //        lf.WriteLine(line);
        //    foreach (var line in ReadLines(fromIndex))
        //        if (re.IsMatch(line))
        //        {
        //            lf.WriteLine(re.Replace(line, replacement));
        //            l++;
        //        }
        //        else lf.WriteLine(line);
        //    if (l > 0)
        //    {
        //        lf.Save();
        //        lf.Move(Path);
        //    }
        //    lf.Delete();
        //    return l + (HasForePiece?ForePiece.ReplaceLines(re, replacement, fromIndex) :0);
        //}
        #endregion




        #region METADATA
        public bool HasMetaData => System.IO.File.Exists(MetaDataPath);
        public bool NeedsMetaData =>  HasMetaData || HasAttachs || (!IsMiddlePiece && HasForePiece && !HasMetaData);
        public XmlDocument MetaData { get; set; } = new XmlDocument();
        public static string DefaultMetaDataExtension { get; set; } = ".metadata";
        public string MetaDataExtension { get; set; } = DefaultMetaDataExtension;
        public string MetaDataFilter => "All MetaData Files (*"+ MetaDataExtension+")|*"+ MetaDataExtension;
        public string MetaDataName => Name + MetaDataExtension;
        public string MetaDataPath => Path + MetaDataExtension;



        public bool OpenMetaData(string path = null)
        {
            if (string.IsNullOrWhiteSpace(path)) path = MetaDataPath;
            return LoadMetaData(path);
        }
        public bool SaveMetaData(string path = null)
        {
            SetMetaData();
            if (string.IsNullOrWhiteSpace(path)) path = MetaDataPath;
            try { MetaData.Save(path); return true; } catch { }
            return false;
        }
        public bool SaveMetaData(XmlDocument metaData,string path = null)
        {
            ChangeMetaData(metaData);
            return SaveMetaData(path);
        }
        public bool DeleteMetaData(string path = null)
        {
            if (string.IsNullOrWhiteSpace(path)) path = MetaDataPath;
            if (File.Exists(path)) try { File.Delete(path); return ClearMetaData(); } catch { return false; }
            return ClearMetaData();
        }
        public bool ClearMetaData()
        {
            MetaData = new XmlDocument();
            Restore();
            return false;
        }
        public bool LoadMetaData(string metaDataPath)
        {
            bool load = false;
            if (File.Exists(metaDataPath)) try { MetaData.Load(metaDataPath); load = true; } catch { }
            if (!load) load = SetMetaData();
            else load = LoadMetaData();
            return load;
        }
        public bool LoadMetaData(XmlDocument metaData)
        {
            ChangeMetaData(metaData);
            return LoadMetaData();
        }
        public bool LoadMetaData()
        {
            try
            {
                LoadMetaDataProperties();
                if(IsFirstPiece) LoadMetaDataSequences();
                LoadMetaDataChanges();
                return true;
            }
            catch { }
            return false;
        }

        public bool SetMetaData()
        {
            try
            {
                SetMetaDataProperties();
                SetMetaDataSequences();
                SetMetaDataChanges();
                return true;
            }
            catch { }
            return false;
        }
        public bool ChangeMetaData(XmlDocument metaData)
        {
            try
            {
                MetaData = metaData;
                return true;
            }
            catch { }
            return false;
        }

        public XmlElement CreateMetaData(string element)
        {
            return MetaData.CreateElement(element);
        }
        public XmlElement CreateMetaData(string element, object value)
        {
            if (value is IEnumerable<object>)
                return SetMetaData(CreateMetaData(element), "I", (IEnumerable<object>)value);
            return CreateMetaData(element, value+"");
        }
        public XmlElement CreateMetaData(string element, string value)
        {
            GetMetaData();
            XmlElement child = CreateMetaData(element);
            return SetMetaDataValue(child,value);
        }
        public XmlElement CreateMetaData(XmlElement value)
        {
            GetMetaData();
            XmlElement child = CreateMetaData(value.Name);
            child.InnerXml = value.InnerXml;
            return child;
        }
        public IEnumerable<XmlElement> CreateMetaData(IEnumerable<object> values)
        {
            return CreateMetaData("I", values);
        }
        public IEnumerable<XmlElement> CreateMetaData(IEnumerable<string> values)
        {
            return CreateMetaData("I", values);
        }
        public IEnumerable<XmlElement> CreateMetaData(string name, IEnumerable<string> values)
        {
            foreach (string item in values)
                yield return CreateMetaData(name, item);
        }
        public IEnumerable<XmlElement> CreateMetaData(string name, IEnumerable<object> values)
        {
            foreach (object item in values)
                yield return CreateMetaData(name, item);
        }

        public XmlElement SetMetaData(string element)
        {
            return SetMetaData(CreateMetaData(element));
        }
        public XmlElement SetMetaData(string element, object value)
        {
            return SetMetaData(element, value + "");
        }
        public XmlElement SetMetaData(string element, string value)
        {
            GetMetaData();
            XmlElement child = CreateMetaData(element);
            child = SetMetaDataValue(child,value);
            return SetMetaData(child);
        }
        public XmlElement SetMetaData(string parent, string element, string value)
        {
            GetMetaData();
            return SetMetaData(GetMetaData(parent), element,value);
        }
        public XmlElement SetMetaData(XmlElement parent, XmlElement child)
        {
            try
            {
                if (parent[child.Name] == null || parent[child.Name].Name == null)
                    return AppendMetaData(parent, child);
                else
                    parent.ReplaceChild(child, parent[child.Name]);
                    //foreach (XmlAttribute item in child.Attributes)
                    //    parent[child.Name].SetAttribute(item.Name, item.Value);
                    //parent[child.Name].InnerXml = child.InnerXml;
                return parent[child.Name];
            }
            catch
            {
                return AppendMetaData(parent, child);
            }
        }
        public XmlElement SetMetaData(XmlElement parent, string name)
        {
            return SetMetaData(parent, CreateMetaData(name));
        }
        public XmlElement SetMetaData(XmlElement parent, string element, object value)
        {
            return SetMetaData(parent,element, value+"");
        }
        public XmlElement SetMetaData(XmlElement parent, string name, string value)
        {
            XmlElement child = CreateMetaData(name);
            child = SetMetaDataValue(child,value);
            return SetMetaData(parent, child);
        }
        public XmlElement SetMetaData(XmlElement parent, string name, IEnumerable<object> values)
        {
            return SetMetaData(parent,name, CreateMetaData(values));
        }
        public XmlElement SetMetaData(XmlElement parent, string name, IEnumerable<string> values)
        {
            return SetMetaData(parent,name, CreateMetaData(values));
        }
        public XmlElement SetMetaData(XmlElement parent, string name, IEnumerable<XmlElement> elements)
        {
            XmlElement child = CreateMetaData(name);
            foreach (var elem in elements)
                child.AppendChild(elem);
            child.SetAttribute("Collection","True");
            return SetMetaData(parent, child);
        }
        public XmlElement SetMetaData(XmlElement child)
        {
            return SetMetaData(GetMetaData(), child);
        }

        public XmlElement AppendMetaData(string element)
        {
            return AppendMetaData(CreateMetaData(element));
        }
        public XmlElement AppendMetaData(string element, object value)
        {
            return AppendMetaData(element, value + "");
        }
        public XmlElement AppendMetaData(string element, string value)
        {
            GetMetaData();
            XmlElement child = CreateMetaData(element);
            child = SetMetaDataValue(child,value);
            return AppendMetaData(child);
        }
        public XmlElement AppendMetaData(XmlElement parent, XmlElement child)
        {
            parent.AppendChild(child);
            return child;
        }
        public XmlElement AppendMetaData(XmlElement parent, string name)
        {
            return AppendMetaData(parent, CreateMetaData(name));
        }
        public XmlElement AppendMetaData(XmlElement parent, string element, object value)
        {
            return AppendMetaData(parent, element, value + "");
        }
        public XmlElement AppendMetaData(XmlElement parent, string name, string value)
        {
            return AppendMetaData(parent, CreateMetaData(name, value));
        }
        public XmlElement AppendMetaData(XmlElement parent, string name, IEnumerable<string> values)
        {
            return AppendMetaData(parent, name, from v in values select CreateMetaData("I", v));
        }
        public XmlElement AppendMetaData(XmlElement parent, string name, IEnumerable<XmlElement> elements)
        {
            XmlElement child = CreateMetaData(name);
            foreach (var elem in elements)
                child.AppendChild(elem);
            parent.SetAttribute("Collection","True");
            return AppendMetaData(parent, child);
        }
        public XmlElement AppendMetaData(XmlElement child)
        {
            return AppendMetaData(GetMetaData(), child);
        }

        public XmlElement FindMetaData(XmlElement parent, string element)
        {
            var els = parent.GetElementsByTagName(element);
            if (els.Count > 0)
                return (XmlElement)els[0];
            return null;
        }
        public XmlElement GetMetaData()
        {
            string element = "METADATA";
            var els = MetaData.GetElementsByTagName(element);
            if (MetaData == null || els.Count < 1)
            {
                MetaData = new XmlDocument();
                MetaData.AppendChild(CreateMetaData(element));
                return (XmlElement)MetaData.ChildNodes[0];
            }
            return MetaData[element];
        }
        public XmlElement GetMetaData(string element)
        {
            XmlElement parent = GetMetaData();
            foreach (XmlElement item in parent.ChildNodes)
                if(item.Name == element)
                    return item;
            return (XmlElement)parent.AppendChild(CreateMetaData(element));
        }

        public string GetMetaDataValue(string parent, string name)
        {
            return GetMetaDataValue(GetMetaData(parent), name);
        }
        public string GetMetaDataValue(XmlElement parent, string name)
        {
            return GetMetaDataValue(FindMetaData(parent, name));
        }
        public string GetMetaDataValue(XmlElement element)
        {
            if (element != null && !string.IsNullOrEmpty(element.InnerText))
                 return StringService.FromXML(element.InnerText);
            return null;
        }
        public IEnumerable<string> GetMeteDataValues(string parent, string name)
        {
            return GetMetaDataValues(GetMetaData(parent), name);
        }
        public IEnumerable<string> GetMetaDataValues(XmlElement parent, string name)
        {
            return GetMetaDataValues(FindMetaData(parent, name));
        }
        public IEnumerable<string> GetMetaDataValues(XmlElement element)
        {
            if (element != null && !string.IsNullOrEmpty(element.InnerText))
                if (element.HasChildNodes)
                    foreach (XmlElement item in element.ChildNodes)
                        yield return GetMetaDataValue(item);
                else yield return GetMetaDataValue(element);
            yield break;
        }

        public XmlElement SetMetaDataValue(string parent, string name, string value)
        {
            return SetMetaDataValue(GetMetaData(parent), name, value);
        }
        public XmlElement SetMetaDataValue(XmlElement parent, string name, string value)
        {
            return SetMetaDataValue(FindMetaData(parent, name), value);
        }
        public XmlElement SetMetaDataValue(XmlElement element, string value)
        {
            element.InnerText = StringService.ToXML(value+"");
            return element;
        }


        public XmlElement MetaDataProperties()
        {
            return GetMetaData("PROPERTIES");
        }
        public void SetMetaDataProperties()
        {
            FileInfo info = Info;
            XmlElement root = MetaDataProperties();
            SetMetaData(root,"ID", ID);
            SetMetaData(root, "Name", Name);
            SetMetaData(root, "RelativePath", RelativePath);
            SetMetaData(root,"Encoding", Encoding.WebName);
            SetMetaData(root,"Size", PieceSize+"");
            SetMetaData(root, "LastUndoBufferID", LastUndoBufferID + "");
            SetMetaData(root, "LastRedoBufferID", LastRedoBufferID + "");
            SetMetaData(root, "PieceWarpsCount", PieceWarpsCount + "");
            SetMetaData(root, "PieceLinesCount", PieceLinesCount + "");
            SetMetaData(root, "LastPieceWarpsCount", LastPieceWarpsCount + "");
            SetMetaData(root, "LastPieceLinesCount", LastPieceLinesCount + "");
            SetMetaData(root, "LastWarpIndex", LastWarpIndex + "");
            SetMetaData(root, "LastLineIndex", LastLineIndex + "");
            SetMetaData(root, "PieceColumnsLabelsIndex", PieceColumnsLabelsIndex + "");
            SetMetaData(root, "PieceRowsLabelsIndex", PieceRowsLabelsIndex + "");
            SetMetaData(root, "FreeColumnsLabels", FreeColumnsLabels + "");
            SetMetaData(root, "FreeRowsLabels", FreeRowsLabels + "");
            SetMetaData(root, "WarpsSplitters", WarpsSplitters);
            SetMetaData(root, "LinesSplitters", LinesSplitters);
            SetMetaData(root, "PieceWarpsSplitters", PieceWarpsSplitters);
            SetMetaData(root, "PieceLinesSplitters", PieceLinesSplitters);
            if(info != null)
            {
                SetMetaData(root, "CreationTimeUtc", info.CreationTimeUtc + "");
                SetMetaData(root, "LastAccessTimeUtc", info.LastAccessTimeUtc + "");
                SetMetaData(root, "LastWriteTimeUtc", info.LastWriteTimeUtc + "");
            }
        }
        public bool LoadMetaDataProperties()
        {
            bool notchanged = true;
            XmlElement root = MetaDataProperties();
            string elem = GetMetaDataValue(root, "ID");
            if (!string.IsNullOrEmpty(elem))
                ID = ConvertService.TryToLong(elem, ID);
            elem = GetMetaDataValue(root, "RelativePath");
            if (!string.IsNullOrEmpty(elem))
                RelativePath = ConvertPathFromRelative(elem);
            elem = GetMetaDataValue(root,"Encoding");
            if (!string.IsNullOrEmpty(elem))
                Encoding = Encoding.GetEncoding(elem);
            elem = GetMetaDataValue(root,"Size");
            if (!string.IsNullOrEmpty(elem))
                notchanged &=  ConvertService.TryToULong(elem, 0) == PieceSize;
            else notchanged = false;
            elem = GetMetaDataValue(root, "LastUndoBufferID");
            if (!string.IsNullOrEmpty(elem))
                LastUndoBufferID = ConvertService.TryToInt(elem);
            elem = GetMetaDataValue(root, "LastRedoBufferID");
            if (!string.IsNullOrEmpty(elem))
                LastRedoBufferID = ConvertService.TryToInt(elem);
            elem = GetMetaDataValue(root, "PieceWarpsCount");
            if (!string.IsNullOrEmpty(elem))
                PieceWarpsCount = ConvertService.TryToLong(elem, PieceWarpsCount);
            elem = GetMetaDataValue(root, "PieceLinesCount");
            if (!string.IsNullOrEmpty(elem))
                PieceLinesCount = ConvertService.TryToLong(elem, PieceLinesCount);
            elem = GetMetaDataValue(root, "LastPieceWarpsCount");
            if (!string.IsNullOrEmpty(elem))
                LastPieceWarpsCount = ConvertService.TryToLong(elem, LastPieceWarpsCount);
            elem = GetMetaDataValue(root, "LastPieceLinesCount");
            if (!string.IsNullOrEmpty(elem))
                LastPieceLinesCount = ConvertService.TryToLong(elem, LastPieceLinesCount);
            elem = GetMetaDataValue(root, "LastWarpIndex");
            if (!string.IsNullOrEmpty(elem))
                LastWarpIndex = ConvertService.TryToLong(elem, LastWarpIndex);
            elem = GetMetaDataValue(root, "LastLineIndex");
            if (!string.IsNullOrEmpty(elem))
                LastLineIndex = ConvertService.TryToLong(elem, LastLineIndex);
            elem = GetMetaDataValue(root, "FreeColumnsLabels");
            if (!string.IsNullOrEmpty(elem))
                FreeColumnsLabels = ConvertService.TryToBoolean(elem, FreeColumnsLabels);
            elem = GetMetaDataValue(root, "FreeRowsLabels");
            if (!string.IsNullOrEmpty(elem))
                FreeRowsLabels = ConvertService.TryToBoolean(elem, FreeRowsLabels);
            elem = GetMetaDataValue(root, "PieceColumnsLabelsIndex");
            if (!string.IsNullOrEmpty(elem))
                PieceColumnsLabelsIndex = ConvertService.TryToLong(elem, PieceColumnsLabelsIndex);
            elem = GetMetaDataValue(root, "PieceRowsLabelsIndex");
            if (!string.IsNullOrEmpty(elem))
                PieceRowsLabelsIndex = ConvertService.TryToLong(elem, PieceRowsLabelsIndex);
            string[] elems = GetMetaDataValues(root, "LinesSplitters").ToArray();
            LinesSplitters =elems;
            elems = GetMetaDataValues(root, "WarpsSplitters").ToArray();
            WarpsSplitters = elems;
            elems = GetMetaDataValues(root, "PieceWarpsSplitters").ToArray();
            PieceWarpsSplitters = elems;
            elems = GetMetaDataValues(root, "PieceLinesSplitters").ToArray();
            PieceLinesSplitters = elems;
            elem = GetMetaDataValue(root,"LastWriteTimeUtc");
            if(Info != null&&!string.IsNullOrEmpty(elem))
                notchanged &= Info.LastWriteTimeUtc + "" == elem;
            else notchanged = false;
            IsCountedPiece = notchanged && LastPieceLinesCount>0;
            return IsCountedPiece;
        }

        public XmlElement MetaDataSequences()
        {
            return GetMetaData("SEQUENCES");
        }
        public void SetMetaDataSequences()
        {
            if (HasForePiece)
            {
                if (ForePiece.MetaData == null || ForePiece.MetaData.DocumentElement == null)
                    ForePiece.SetMetaData();
                SetMetaData(MetaDataSequences(), CreateMetaData(ForePiece.MetaData.DocumentElement));
            }
        }
        public bool LoadMetaDataSequences()
        {
            try
            {
                ForePiece = null;
                RelativePath = Path;
                XmlElement links = MetaData.DocumentElement;
                while (true)
                {
                    links = FindMetaData(links, "SEQUENCES");
                    if (links == null || links.FirstChild == null) break;
                    else
                        try
                        {
                            ChainedFile lf = AppendForePiece(GetMetaDataValue(links.FirstChild["PROPERTIES"]["RelativePath"]));
                            if (lf == null) continue;
                            lf.ForePiece = null;
                            lf.MetaData.LoadXml(links.FirstChild.OuterXml);
                            lf.LoadMetaData();
                        }
                        catch { }
                }
                return true;
            }
            catch { }
            return false;
        }


        public XmlElement MetaDataChanges()
        {
            return GetMetaData("CHANGES");
        }
        public void SetMetaDataChanges()
        {
            MetaDataChanges().RemoveAll();
            PieceUndoBuffer.Compress();
            foreach (var filter in PieceUndoBuffer)
                SetMetaDataChange(filter);
        }
        public void SetMetaDataChange(FilterItem filter)
        {
            XmlElement child = AppendMetaData(MetaDataChanges(), filter.Type+"");
            child.SetAttribute("ID", filter.ID + "");
            child.SetAttribute("From", filter.From + "");
            child.SetAttribute("To", filter.To + "");
            foreach (var item in filter.Items)
                child.AppendChild(CreateMetaData("I",item));
        }
        public bool LoadMetaDataChanges()
        {
            XmlElement changes = MetaDataChanges();
            try
            {
                foreach (XmlElement item in changes.ChildNodes)
                    try
                    {
                        List<string> ls = GetMetaDataValues(item).ToList();
                        PieceUndoBuffer.Add(new FilterItem((TableChangeMode)Enum.Parse(typeof(TableChangeMode), item.Name),
                            ConvertService.TryToLong(item.GetAttribute("From"), 0),
                            ConvertService.TryToLong(item.GetAttribute("To"), 0),
                            ls,
                            ConvertService.TryToInt(item.GetAttribute("ID"), 0)
                            )
                        );
                    }
                    catch { }
                return true;
            }
            catch { }
            return false;
        }

        #endregion


        #region TEMPORARiES
        public bool HasPieceTemporary => File.Exists(PieceTemporaryPath);
        public string PieceTemporaryPath => Directory + "$" + Name;

        #endregion


    }
}
