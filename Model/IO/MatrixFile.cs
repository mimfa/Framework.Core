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

namespace MiMFa.Model.IO
{
    [Serializable]
    public class MatrixFile<T> : IEnumerable<IEnumerable<IEnumerable<T>>>
    {
        public IEnumerator<IEnumerable<IEnumerable<T>>> GetEnumerator() => Coplanar.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Coplanar.GetEnumerator();

        private ChainedFile _Source = null;
        public ChainedFile Source
        {
            get
            {
                if (_Source.IsPieceEmpty && _Source.HasForePiece)
                    return _Source.ForePiece;
                else return _Source;
            }
        }

        public int Space
        {
            get
            {
                long w = Width;
                long h = Height;
                long d = Depth;
                if (d > 1)
                    if (w > 1 && h > 1) return 3;
                    else if (w > 1 || h > 1) return 2;
                    else return 1;
                else if (h > 1)
                    if (w > 1 && d > 1) return 3;
                    else if (w > 1 || d > 1) return 2;
                    else return 1;
                else if (w > 1)
                    if (d > 1 && h > 1) return 3;
                    else if (d > 1 || h > 1) return 2;
                    else return 1;
                else return 0;
            }
        }
        public long Width => Source.WarpsCount;
        public long Height => Source.ForeMaxPieceLinesCount;
        public long Depth => Source.ForeLength;
        public long Number => Width * Height * Depth;

        public bool IsLine => Space == 1;
        public bool IsVector => Space == 1;
        public bool IsPlane => Space == 2;
        public bool IsSquare => Width == Height || Width == Depth || Depth == Height;
        public bool IsCoplanar => Space > 2;
        public bool IsCube => Width == Height && Width == Depth;

        public IEnumerable<IEnumerable<T>> this[long z]
        {
            get => ToPlane(Source.Piece(z, null).ReadPieceRows(),Width,Height);
            set => Source.Piece(z, null).ChangePieceRows(0, -1, FromPlane(value, Width, Height));
        }
        public IEnumerable<T> this[long z, long y]
        {
            get => ToLine(Source.Piece(z, null).ReadPieceRow(y), Width);
            set => Source.Piece(z, null).ChangePieceRow(y, FromLine(value,Width));
        }
        public T this[long z, long y, long x]
        {
            get => ToParameter(Source.Cell(x, y, z));
            set => Source.ChangeCell(x, y, z, FromParameter(value));
        }
       
        public IEnumerable<IEnumerable<IEnumerable<T>>> ToCoplanar(IEnumerable<IEnumerable<IEnumerable<string>>> cells, long width, long height, long depth)
        {
            foreach (var item in cells)
            {
                depth--;
                yield return ToPlane(item, width, height);
            }
            while (depth-- > 0)
                yield return ToPlane(new string[0][], width,height);
        }
        public IEnumerable<IEnumerable<T>> ToPlane(IEnumerable<IEnumerable<string>> cells, long width, long height)
        {
            foreach (var item in cells)
            {
                height--;
                yield return ToLine(item,width);
            }
            while (height-- > 0)
                yield return ToLine(new string[0], width);
        }
        public IEnumerable<T> ToLine(IEnumerable<IEnumerable<string>> cells, long width)
        {
            foreach (var row in cells)
                foreach (var item in row)
                {
                    width--;
                    yield return ToParameter(item);
                }
            while (width-- > 0)
                yield return DefaultParameter;
        } 
        public IEnumerable<T> ToLine(IEnumerable<string> cells, long width)
        {
            foreach (var item in cells)
            {
                width--;
                yield return ToParameter(item);
            }
            while (width-- > 0)
                yield return DefaultParameter;
        } 
        public virtual T ToParameter(string str) => DefaultToParameter(str);
        public T DefaultParameter;
        public IEnumerable<IEnumerable<IEnumerable<string>>> FromCoplanar(IEnumerable<IEnumerable<IEnumerable<T>>> cells, long width, long height, long depth)
        {
            foreach (var item in cells)
            {
                depth--;
                yield return FromPlane(item, width, height);
            }
            while (depth-- > 0)
                yield return FromPlane(new T[0][], width, height);
        }
        public IEnumerable<IEnumerable<string>> FromPlane(IEnumerable<IEnumerable<T>> cells, long width, long height)
        {
            foreach (var item in cells)
            {
                height--;
                yield return FromLine(item, width);
            }
            while (height-- > 0)
                yield return FromLine(new T[0], width);
        }
        public IEnumerable<string> FromLine(IEnumerable<T> cells, long width)
        {
            foreach (var item in cells)
            {
                width--;
                yield return FromParameter(item);
            }
            while (width-- > 0)
                yield return DefaultString;
        }
        public virtual string FromParameter(T param) => DefaultFromParameter(param);
        public string DefaultString = string.Empty;

        public Func<string, T> DefaultToParameter = null;
        public Func<T, string> DefaultFromParameter = null;

        public MatrixFile(ChainedFile document,T dp,string ds="", Func<string,T> tp = null,Func<T,string> fp = null)
        {
            if (document == null) return;
            _Source = document;
            DefaultParameter = dp;
            DefaultString = ds;
            DefaultToParameter = tp?? new Func<string, T>(s =>DefaultParameter);
            DefaultFromParameter = fp ?? new Func<T, string>(p=>p==null? DefaultString: p.ToString());
        }


        public virtual IEnumerable<IEnumerable<IEnumerable<T>>> Coplanar
        {
            get => ToCoplanar(from doc in Source.ForeChain select doc.ReadPieceRows(), Width, Height,Depth);
            set
            {
                Source.Clear();
                foreach (var doc in value)
                    Source.AppendForePiece(new ChainedFile(FromPlane(doc,Width,Height)));
                Source.Save();
            }
        }
        public virtual IEnumerable<IEnumerable<T>> Plane { get => ToPlane(Source.ReadPieceRows(), Width, Height); set => Source.ChangePieceRows(0,-1,FromPlane(value,Width,Height)); }
        public virtual IEnumerable<IEnumerable<T>> XPlane(long x = 0)=> ToPlane(from ch in Source.ForeChain select ch.ReadPieceColumn(x),Height,Depth);
        public virtual IEnumerable<IEnumerable<T>> YPlane(long y = 0)=>ToPlane(from ch in Source.ForeChain select ch.ReadPieceRow(y), Width, Depth);
        public virtual IEnumerable<IEnumerable<T>> ZPlane(long z = 0) => ToPlane(Source.Piece(z, null).ReadPieceRows(), Width, Height);

        public virtual IEnumerable<T> Line
        {
            get => ToLine(Source.ReadPieceCells(), Width * Height); 
            set 
            { 
                int w = Convert.ToInt32(Width);
                long l = Width * Height;
                var v = FromLine(value, Width * Height);
                Source.ClearPiece();
                for (int i = 0; i < l; i += w)
                {
                    Source.WritePieceRow(v.Take(w));
                    v = v.Skip(w);
                }
                Source.Save();
            }
        }
        public virtual IEnumerable<T> XLine(long x = 0) => ToLine(Source.ReadColumn(x),Height * Depth);
        public virtual IEnumerable<T> YLine(long y = 0) => ToLine(from ch in Source.ForeChain select ch.ReadPieceRow(y), Width * Depth);
        public virtual IEnumerable<T> ZLine(long z = 0) => ToLine(Source.Piece(z, null).ReadPieceCells(), Width * Height);

        public virtual IEnumerable<T> XYLine(long x, long y) => ToLine(from ch in Source.ForeChain select ch.ReadCell(x, y), Depth);
        public virtual IEnumerable<T> XZLine(long x, long z) => ToLine(Source.Piece(z, null).ReadPieceColumn(x), Height);
        public virtual IEnumerable<T> YZLine(long y, long z) => ToLine(Source.Piece(z, null).ReadPieceRow(y), Width);

        public virtual T XYZ(long x = 0, long y = 0, long z=0) => ToParameter(Source.ReadCell(x, y, z));

        public virtual IEnumerable<IEnumerable<T>> Get(long z) => this[z];
        public virtual IEnumerable<IEnumerable<T>> Set(long z, IEnumerable<IEnumerable<T>> value) => this[z] = value;
        public virtual IEnumerable<T> Get(long y, long z) => this[z,y];
        public virtual IEnumerable<T> Set(long y, long z, IEnumerable<T> value) => this[z,y] = value;
        public virtual T Get(long x, long y, long z) => this[z,y,x];
        public virtual T Set(long x, long y,long z,  T value) => this[z, y, x] = value;

    }
}
