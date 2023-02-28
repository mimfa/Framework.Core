using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Office.Interop.Excel;
using MiMFa.General;

namespace MiMFa.Model.IO.ChainedFiles
{
    public class FilterCollection : List<FilterItem>
    {
        public FilterCollection() : base() { }
        public FilterCollection(IEnumerable<FilterItem> collection) : base(collection) { }

        public IEnumerable<string> Filter(IEnumerable<string> list)
        {
            if (Count < 1)
            {
                if (list != null) foreach (var item in list) yield return item;
            }
            else
            {
                List<FilterItem> filters = new FilterCollection(this);
                int min = Convert.ToInt32(StartFilteredIndex());
                int max = Convert.ToInt32(EndFilteredIndex());

                foreach (var item in list.Take(min))
                    yield return item;
                list = list.Skip(min);

                long currectorInd = min;
                foreach (var item in list)
                {
                    bool hasResult;
                    string line = Filter(currectorInd, item, ref filters, out hasResult);
                    if (hasResult) yield return line;
                    if (++currectorInd > max) break;
                }
                list = list.Skip(Convert.ToInt32(currectorInd - min));

                foreach (var item in list)
                    yield return item;
            }
        }

        public static string Filter(long currectorInd, string line, ref List<FilterItem> filters, out bool hasResult)
        {
            hasResult = true;
            List<int> li = FindIndices(filters, currectorInd).ToList();
            foreach (var index in li)
            {
                FilterItem filter = filters[index];

                switch (filter.Type)
                {
                    //case TableChangeMode.Insert:
                    //    if (filter.Items.Count > 1)
                    //        result.AddRange(from v in filter.Items select v);
                    //    else
                    //    {
                    //        string iLine = filter.Items.First();
                    //        result.AddRange(from v in list.Take(len) select iLine);
                    //    }
                    //    list = result.Concat(list);
                    //    currectorInd -= len;
                    //    return Filter(ref list, ref filters, ref currectorInd, out hasResult);

                    case TableChangeMode.Delete:
                        hasResult = false;
                        line = null;
                        break;

                    case TableChangeMode.Modify:
                    default:
                        hasResult = true;
                        line = filter.GetItem(currectorInd);
                        break;
                }
            }

            for (int i = li.Count-1; i>=0;i--)
                RemoveAt(ref filters, li[i], currectorInd);
            
            return line;
        }


        /// <summary>
        /// Add new Filter
        /// </summary>
        /// <param name="filter">the new filter</param>
        /// <returns></returns>
        public void AddItem(FilterItem filter)
        {
            //Changing Indexes
            long curr = 0;
            long tol = 0;
            foreach (var item in this.OrderBy(f => f.From))
                if (item.From <= filter.From)
                    switch (item.Type)
                    {
                        case TableChangeMode.Insert:
                            tol = -item.ChangeNumber;
                            filter.From += tol;
                            curr += tol;
                            break;
                        case TableChangeMode.Delete:
                            tol = -item.ChangeNumber;
                            filter.From += tol;
                            curr += tol;
                            break;
                    }
            filter = new FilterItem(filter, filter.From, filter.To < 0 ? filter.To : filter.To + curr, filter.ID);

            //Removing Repeated Filter
            AddOrReplace(filter);
        }
        public void AddRangeItems(IEnumerable<FilterItem> filters)
        {
            foreach (var item in filters)
                AddItem(item);
        }

        //public new void RemoveAt(int index)
        //{
        //Changing Indexes
        //int curr = 0;
        //int tol = 0;
        //var filter = this[index];
        //switch (filter.Type)
        //{
        //    case TableChangeMode.Insert:
        //        tol = filter.Collection ? filter.MainToIndex - filter.MainIndex : 1;
        //        curr += tol;
        //        break;
        //    case TableChangeMode.Delete:
        //        tol = filter.Collection ? filter.MainIndex - filter.MainToIndex : -1;
        //        curr += tol;
        //        break;
        //}
        //for (int f = 0; f < this.Count; f++)
        //    if (this[f].MainIndex > filter.MainIndex)
        //        this[f] = new FilterItem(this[f], this[f].MainIndex + curr, this[f].MainToIndex < 0 ? this[f].MainToIndex : this[f].MainToIndex + curr);
        //    base.RemoveAt(index);
        //}

        /// <summary>
        /// Add Or Replace by Repeated Filter
        /// </summary>
        /// <param name="filter">the new filter</param>
        /// <returns></returns>
        public void AddOrReplace(FilterItem filter)
        {
            if (this.Count > 0)
            {
                FilterItem last = this.Last();
                switch (filter.Type)
                {
                    case TableChangeMode.Insert:
                        base.Add(filter);
                        break;
                    case TableChangeMode.Delete:
                        switch (last.Type)
                        {
                            case TableChangeMode.Delete:
                            case TableChangeMode.Insert:
                                base.Add(filter);
                                break;
                            default:
                                if (last.From == filter.From && last.To == filter.To)
                                    this[this.Count - 1] = filter;
                                else base.Add(filter);
                                break;
                        }
                        break;
                    default:
                        switch (last.Type)
                        {
                            case TableChangeMode.Delete:
                                base.Add(filter);
                                break;
                            default:
                                if (last.From == filter.From && last.To == filter.To)
                                    this[this.Count - 1] = new FilterItem(last, filter.Items, filter.From, filter.To, filter.ID);
                                else base.Add(filter);
                                break;
                        }
                        break;
                }
            }
            else base.Add(filter);
        }
        public void Compress()
        {
            for (int i = this.Count-1; i >= 0; i--)
                switch (this[i].Type)
                {
                    case TableChangeMode.Delete:
                        for (int j = 0; j < i; j++)
                            if (this[i].From == this[j].From && this[i].To == this[j].To)
                            {
                                this.RemoveAt(j);
                                i--;
                            }
                        break;

                    case TableChangeMode.Insert:
                        break;

                    default:
                        for (int j = 0; j < i; j++)
                            if(this[i].From == this[j].From && this[i].To == this[j].To)
                            switch (this[j].Type)
                            {
                                case TableChangeMode.Delete:
                                case TableChangeMode.Insert:
                                    break;

                                default:
                                        this.RemoveAt(j);
                                        i--;
                                    break;
                            }
                        break;
                }
        }

        public long StartFilteredIndex()
        {
            return this.Min(f=>f.From);
        }
        public long EndFilteredIndex()
        {
            return this.Max(f => f.IsCollection ? f.To : f.From);
        }


        public static IEnumerable<int> FindIndices(List<FilterItem> filters, long lineIndex)
        {
            int i = -1;
            return from f in filters let m = i++ where f.From <= lineIndex && (f.To < 0 || f.To >= lineIndex) select i;
        }
        public static int FindIndex(List<FilterItem> filters, long lineIndex)
        {
            return filters.FindIndex(f => f.From <= lineIndex && (f.To < 0 || f.To >= lineIndex));
        }
        public static bool RemoveAt(ref List<FilterItem> filters, int index, long toLineIndex)
        {
            if(filters[index].To < 0 || filters[index].To > toLineIndex) return false;
            filters.RemoveAt(index);
            return true;
        }
    }
}
