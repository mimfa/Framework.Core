using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using MiMFa.General;
using MiMFa.Model;

namespace MiMFa.Service
{
    public class CollectionService
    {
        public static void Sort<T>(T[] input, out T[] output, out int[] outIndex)
        {
            if (input == null) { throw new ArgumentNullException("input"); }
            if (input.Length == 0)
            {
                // give back empty lists
                output = new T[input.Length];
                outIndex = new int[input.Length];
                return;
            }
            int[] items = Enumerable.Range(0, input.Length).ToArray();
            T[] keys = input.ToArray();
            Array.Sort(keys, items);
            output = keys;
            outIndex = items;
        }
        public static void Sort<T>(List<T> input, out List<T> output, out List<int> outIndex)
        {
            T[] outp;
            int[] outI;

            Sort(input.ToArray(), out outp, out outI);

            output = outp.ToList();
            outIndex = outI.ToList();
        }
        public static T[] Sort<T>(T[] input, out int[] outIndex)
        {
            T[] outp;
            int[] outI;
            Sort(input.ToArray(), out outp, out outI);
            outIndex = outI;
            return outp;
        }
        public static List<T> Sort<T>(List<T> input, out List<int> outIndex)
        {
            T[] outp;
            int[] outI;
            Sort(input.ToArray(), out outp, out outI);
            outIndex = outI.ToList();
            return outp.ToList();
        }
        public static T[] Sort<T>(T[] input)
        {
            T[] outp;
            int[] outI;
            Sort(input.ToArray(), out outp, out outI);
            return outp;
        }
        public static List<T> Sort<T>(List<T> input)
        {
            T[] outp;
            int[] outI;
            Sort(input.ToArray(), out outp, out outI);
            return outp.ToList();
        }
        public static Dictionary<T, F> Sort<T, F>(Dictionary<T, F> input)
        {
            List<T> lt = new List<T>();
            List<F> lf = new List<F>();
            foreach (var item in input.Keys)
            {
                lt.Add(item);
                lf.Add(input[item]);
            }
            List<T> lout = new List<T>();
            List<int> li = new List<int>();
            Sort(lt, out lout, out li);

            Dictionary<T, F> Dic = new Dictionary<T, F>();
            for (int i = 0; i < lout.Count(); i++)
                Dic.Add(lout[i], lf[li[i]]);

            return Dic;
        }
        public static Dictionary<T, F> Sort<T, F>(Dictionary<T, F> input,Func<KeyValuePair<T, F>, KeyValuePair<T, F>,bool> comparer)
        {
            if (input.Count <= 0) return input;
            Dictionary<T, F> dic =  new Dictionary<T, F>();
            List<T> lt = new List<T>();
            List<F> lf = new List<F>();
            var arr = input.Keys.ToList();
            int i = 0;
            KeyValuePair<T, F> kvp =new KeyValuePair<T, F>(arr[i], input[arr[i]]);
            List<KeyValuePair<T, F>> lkvp = new List<KeyValuePair<T, F>>();
            foreach (var item in input)
                lkvp.Add(item);
            while (lkvp.Count > 0)
                if (i >= lkvp.Count)
                {
                    i = 0;
                    dic.Add(kvp.Key, kvp.Value);
                    lkvp.Remove(kvp);
                    if (lkvp.Count > 0) kvp = lkvp[0];
                }
                else if (lkvp.Count == 1)
                {
                    kvp = lkvp[0];
                    dic.Add(kvp.Key, kvp.Value);
                    lkvp.Clear();
                }
                else if (comparer(kvp, lkvp[i])) i++;
                else kvp = lkvp[i++];
            return dic;
        }
        public static void Sort<T>(SmartList<T> input, out SmartList<T> output, out SmartList<int> outIndex)
        {
            T[] outp;
            int[] outI;

            Sort(input.ToArray(), out outp, out outI);

            output =(SmartList<T>) outp.ToList();
            outIndex = (SmartList<int>)outI.ToList();
        }
        public static SmartList<T> Sort<T>(SmartList<T> input, out SmartList<int> outIndex)
        {
            T[] outp;
            int[] outI;
            Sort(input.ToArray(), out outp, out outI);
            outIndex =( SmartList<int>) outI.ToList();
            return ( SmartList<T>)outp.ToList();
        }
        public static SmartList<T> Sort<T>(SmartList<T> input)
        {
            T[] outp;
            int[] outI;
            Sort(input.ToArray(), out outp, out outI);
            return (SmartList<T>)outp.ToList();
        }
        public static SmartDictionary<T, F> Sort<T, F>(SmartDictionary<T, F> input)
        {
            List<T> lt = new List<T>();
            List<F> lf = new List<F>();
            foreach (var item in input.Keys)
            {
                lt.Add(item);
                lf.Add(input[item]);
            }
            List<T> lout = new List<T>();
            List<int> li = new List<int>();
            Sort(lt, out lout, out li);

            SmartDictionary<T, F> Dic = new SmartDictionary<T, F>();
            for (int i = 0; i < lout.Count(); i++)
                Dic.Add(lout[i], lf[li[i]]);

            return Dic;
        }
        public static SmartKeyValueList<T, F> Sort<T, F>(SmartKeyValueList<T, F> input)
        {
            List<T> lt = new List<T>();
            List<F> lf = new List<F>();
            foreach (var item in input.Keys)
            {
                lt.Add(item);
                lf.Add(input[item]);
            }
            List<T> lout = new List<T>();
            List<int> li = new List<int>();
            Sort(lt, out lout, out li);

            SmartKeyValueList<T, F> Dic = new SmartKeyValueList<T, F>();
            for (int i = 0; i < lout.Count(); i++)
                Dic.Add(lout[i], lf[li[i]]);

            return Dic;
        }


        public static T[] Sort<T>(T[] input, int[] indexes)
        {
            int length = indexes.Length;
            T[] RandInput = new T[length];

            for (int i = 0; i < length; i++)
                RandInput[i] = input[indexes[i]];

            return RandInput;
        }
        public static List<T> Sort<T>(List<T> input, int[] indexes)
        {
            int length = indexes.Length;
            List<T> RandInput = new List<T>();

            for (int i = 0; i < length; i++)
                RandInput.Add(input[indexes[i]]);

            return RandInput;
        }
        public static Dictionary<T, F> Sort<T, F>(Dictionary<T, F> input, int[] indexes)
        {
            int length = indexes.Length;
            Dictionary<T, F> RandInput = new Dictionary<T, F>();
            for (int i = 0; i < length; i++)
                RandInput.Add(input.ElementAt(indexes[i]).Key, input.ElementAt(indexes[i]).Value);
            return RandInput;
        }
        public static SmartList<T> Sort<T>(SmartList<T> input, int[] indexes)
        {
            int length = indexes.Length;
            SmartList<T> RandInput = new SmartList<T>();

            for (int i = 0; i < length; i++)
                RandInput.Add(input[indexes[i]]);

            return RandInput;
        }
        public static SmartDictionary<T, F> Sort<T, F>(SmartDictionary<T, F> input, int[] indexes)
        {
            int length = indexes.Length;
            SmartDictionary<T, F> RandInput = new SmartDictionary<T, F>();
            for (int i = 0; i < length; i++)
                RandInput.Add(input.ElementAt(indexes[i]).Key, input.ElementAt(indexes[i]).Value);
            return RandInput;
        }
        public static SmartKeyValueList<T, F> Sort<T, F>(SmartKeyValueList<T, F> input, int[] indexes)
        {
            int length = indexes.Length;
            SmartKeyValueList<T, F> RandInput = new SmartKeyValueList<T, F>();
            for (int i = 0; i < length; i++)
                RandInput.Add(input.ElementAt(indexes[i]).Key, input.ElementAt(indexes[i]).Value);
            return RandInput;
        }

        public static int[] RandIndex(int Length)
        {
            Random r = new Random();
            int min = 0;
            int max = Length * 10;
            int[] rand = new int[Length];
            int[] outp = new int[Length];
            int[] outIndex = new int[Length];

            for (int i = 0; i < Length; i++)
                rand[i] = r.Next(min, max);

            Sort(rand, out outp, out outIndex);

            return outIndex;
        }

        public static T[] Shake<T>(T[] input)
        {
            return Sort(input, RandIndex(input.Length));
        }
        public static List<T> Shake<T>(List<T> input)
        {
            return Sort(input, RandIndex(input.Count));

        }
        public static Dictionary<T, F> Shake<T, F>(Dictionary<T, F> input)
        {
            return Sort(input, RandIndex(input.Count));

        }
        public static SmartList<T> Shake<T>(SmartList<T> input)
        {
            return Sort(input, RandIndex(input.Count));

        }
        public static SmartDictionary<T, F> Shake<T, F>(SmartDictionary<T, F> input)
        {
            return Sort(input, RandIndex(input.Count));

        }
        public static SmartKeyValueList<T, F> Shake<T, F>(SmartKeyValueList<T, F> input)
        {
            return Sort(input, RandIndex(input.Count));

        }

        public static T[] Distinct<T>(T[] array)
        {
            return array.Distinct().ToArray();
        }
        public static List<T> Distinct<T>(List<T> list)
        {
            return list.Distinct().ToList();
        }
        public static SmartList<T> Distinct<T>(SmartList<T> list)
        {
            SmartList<T> lt = new SmartList<T>();
            foreach (var item in list.Distinct())
                    lt.Add(item);
            return lt;
        }
        public static SmartKeyValueList<T,F> Distinct<T,F>(SmartKeyValueList<T,F> list)
        {
            SmartKeyValueList<T,F> lt = new SmartKeyValueList<T,F>();
            foreach (var item in list.Distinct())
                    lt.Add(item);
            return lt;
        }

        public static T[] Reverse<T>(T[] array)
        {
            List<T> lt = new List<T>();
            foreach (var item in array.Distinct())
                lt.Insert(0, item);
            return lt.ToArray();
        }
        public static List<T> Reverse<T>(List<T> list)
        {
            List<T> lt = new List<T>();
            foreach (var item in list.Distinct())
                lt.Insert(0, item);
            return lt;
        }
        public static Dictionary<F, T> Reverse<T,F>(Dictionary<T, F> dic)
        {
            Dictionary<F, T> dt = new Dictionary<F, T>();
            foreach (var item in dic.Distinct())
                try { dt.Add(item.Value,item.Key); }
                catch { }
            return dt;
        }
        public static SmartList<T> Reverse<T>(SmartList<T> list)
        {
            SmartList<T> lt = new SmartList<T>();
            foreach (var item in list.Distinct())
                lt.Insert(0, item);
            return lt;
        }
        public static SmartDictionary<F, T> Reverse<T, F>(SmartDictionary<T, F> dic)
        {
            SmartDictionary<F, T> dt = new SmartDictionary<F, T>();
            foreach (var item in dic.Distinct())
                try { dt.Add(item.Value, item.Key); }
                catch { }
            return dt;
        }
        public static SmartKeyValueList<F, T> Reverse<T, F>(SmartKeyValueList<T, F> dic)
        {
            SmartKeyValueList<F, T> dt = new SmartKeyValueList<F, T>();
            foreach (var item in dic)
                try { dt.Add(item.Value, item.Key); }
                catch { }
            return dt;
        }
        public static int FindIndex<T>(T[] input, T Search)
        {
            for (int i = 0; i < input.Length; i++)
                if (input[i].Equals(Search)) return i;
            return -1;
        }
        public static int FindIndex<T>(List<T> input, T Search)
        {
            for (int i = 0; i < input.Count; i++)
                if (input[i].Equals(Search)) return i;
            return -1;
        }
        public static int FindIndex<T>(SmartList<T> input, T Search)
        {
            for (int i = 0; i < input.Count; i++)
                if (input[i].Equals(Search)) return i;
            return -1;
        }

        public static bool ExistIn<T>(T[] array, T Search)
        {
            return FindIndex(array, Search) > -1;
        }
        public static bool ExistIn<T>(List<T> list, T Search)
        {
            return FindIndex(list, Search) > -1;
        }
        public static bool ExistIn<T,F>(Dictionary<T, F> dic, T Search)
        {
            try
            {
               F r = dic[Search];
                return true;
            }
            catch { return false; }
        }
        public static bool ExistIn<T>(SmartList<T> list, T Search)
        {
            return FindIndex(list, Search) > -1;
        }
        public static bool ExistIn<T, F>(SmartDictionary<T, F> dic, T Search)
        {
            try
            {
                F r = dic[Search];
                return true;
            }
            catch { return false; }
        }
        public static bool ExistIn<T, F>(SmartKeyValueList<T, F> dic, T Search)
        {
            try
            {
                F r = dic[Search];
                return true;
            }
            catch { return false; }
        }
        public static T[] GetPart<T>(T[] intoArray, int ofThisIndex, int toThisIndex)
        {
            T[] a = new T[1 + toThisIndex - ofThisIndex];
            int j = 0;
            toThisIndex = Math.Min(toThisIndex , intoArray.Length-1);
            for (int i = ofThisIndex; i <= toThisIndex; i++)
                a[j++] = intoArray[i];
            return a;
        }
        public static List<T> GetPart<T>(List<T> intoList, int ofThisIndex, int toThisIndex)
        {
            toThisIndex = Math.Min(toThisIndex+1, intoList.Count);
            return intoList.GetRange(ofThisIndex, toThisIndex - ofThisIndex);
        }
        public static Dictionary<T, F> GetPart<T, F>(Dictionary<T, F> intoDic, T ofThisKey, T toThisKey)
        {
            Dictionary<T, F> lt = new Dictionary<T, F>();
            bool start = false;
            foreach (var item in intoDic)
                if (start && item.Key.Equals(toThisKey))
                {
                    lt.Add(item.Key, item.Value);
                    return lt;
                }
                else if (start)
                    lt.Add(item.Key, item.Value);
                else if (!start && item.Key.Equals(ofThisKey))
                {
                    start = true;
                    lt.Add(item.Key, item.Value);
                }
            return lt;
        }
        public static T[] GetPart<T>(T[] intoArray, int ofThisIndex)
        {
            return GetPart(intoArray, ofThisIndex, intoArray.Length-1) ;
        }
        public static List<T> GetPart<T>(List<T> intoList, int ofThisIndex)
        {
            return GetPart(intoList, ofThisIndex, intoList.Count-1);
        }
        public static Dictionary<T, F> GetPart<T, F>(Dictionary<T, F> intoDic, T ofThisKey)
        {
            return GetPart(intoDic, ofThisKey, intoDic.ElementAt(intoDic.Count -1).Key);
        }
        public static SmartList<T> GetPart<T>(SmartList<T> intoList, int ofThisIndex, int toThisIndex)
        {
            toThisIndex = Math.Min(toThisIndex+1, intoList.Count);
            return (SmartList<T>)intoList.GetRange(ofThisIndex, toThisIndex - ofThisIndex);
        }
        public static SmartDictionary<T, F> GetPart<T, F>(SmartDictionary<T, F> intoDic, T ofThisKey, T toThisKey)
        {
            SmartDictionary<T, F> lt = new SmartDictionary<T, F>();
            bool start = false;
            foreach (var item in intoDic)
                if (start && item.Key.Equals(toThisKey))
                {
                    lt.Add(item.Key, item.Value);
                    return lt;
                }
                else if (start)
                    lt.Add(item.Key, item.Value);
                else if (!start && item.Key.Equals(ofThisKey))
                {
                    start = true;
                    lt.Add(item.Key, item.Value);
                }
            return lt;
        }
        public static SmartKeyValueList<T, F> GetPart<T, F>(SmartKeyValueList<T, F> intoDic, T ofThisKey, T toThisKey)
        {
            SmartKeyValueList<T, F> lt = new SmartKeyValueList<T, F>();
            bool start = false;
            foreach (var item in intoDic)
                if (start && item.Key.Equals(toThisKey))
                {
                    lt.Add(item.Key, item.Value);
                    return lt;
                }
                else if (start)
                    lt.Add(item.Key, item.Value);
                else if (!start && item.Key.Equals(ofThisKey))
                {
                    start = true;
                    lt.Add(item.Key, item.Value);
                }
            return lt;
        }
        public static SmartList<T> GetPart<T>(SmartList<T> intoList, int ofThisIndex)
        {
            return GetPart(intoList, ofThisIndex, intoList.Count - 1);
        }
        public static SmartDictionary<T, F> GetPart<T, F>(SmartDictionary<T, F> intoDic, T ofThisKey)
        {
            return GetPart(intoDic, ofThisKey, intoDic.ElementAt(intoDic.Count - 1).Key);
        }
        public static SmartKeyValueList<T, F> GetPart<T, F>(SmartKeyValueList<T, F> intoDic, T ofThisKey)
        {
            return GetPart(intoDic, ofThisKey, intoDic.ElementAt(intoDic.Count - 1).Key);
        }

        public static void Fill<T>(ref T[] thisArray, T[] fromThisArray)
        {
            int len = Math.Min(thisArray.Length, fromThisArray.Length);
            int i = 0;
            for (i = 0; i < len; i++)
                thisArray[i] = fromThisArray[i];
        }
        public static void Fill<T>(ref List<T> thisList, List<T> fromThisList)
        {
            int len = Math.Min(thisList.Count, fromThisList.Count);
            int i = 0;
            for (i = 0; i < len; i++)
                thisList[i] = fromThisList[i];
        }
        public static void Fill<T, F>(Dictionary<T, F> thisDic, Dictionary<T, F> fromThisDic)
        {
            foreach (var item in thisDic)
                try
                {
                    thisDic[item.Key] = fromThisDic[item.Key];
                }
                catch {  }
        }
        public static void Fill<T>(ref SmartList<T> thisList, SmartList<T> fromThisList)
        {
            int len = Math.Min(thisList.Count, fromThisList.Count);
            int i = 0;
            for (i = 0; i < len; i++)
                thisList[i] = fromThisList[i];
        }
        public static void Fill<T, F>(SmartDictionary<T, F> thisDic, SmartDictionary<T, F> fromThisDic)
        {
            foreach (var item in thisDic)
                try
                {
                    thisDic[item.Key] = fromThisDic[item.Key];
                }
                catch {  }
        }
        public static void Fill<T, F>(SmartKeyValueList<T, F> thisDic, SmartKeyValueList<T, F> fromThisDic)
        {
            foreach (var item in thisDic)
                try
                {
                    thisDic[item.Key] = fromThisDic[item.Key];
                }
                catch {  }
        }
        public static void Fill<T>(ref T[] thisArray, T[] fromThisArray,T otherValue)
        {
            int len = Math.Min(thisArray.Length, fromThisArray.Length);
            int i = 0;
            for ( i = 0; i < len; i++)
                thisArray[i] = fromThisArray[i];
            for (; i < thisArray.Length; i++)
                thisArray[i] = otherValue;
        }
        public static void Fill<T>(ref List<T> thisList, List<T> fromThisList, T otherValue)
        {
            int len = Math.Min(thisList.Count, fromThisList.Count);
            int i = 0;
            for ( i = 0; i < len; i++)
                thisList[i] = fromThisList[i];
            for (; i < thisList.Count; i++)
                thisList[i] = otherValue;
        }
        public static void Fill<T, F>(Dictionary<T, F> thisDic, Dictionary<T, F> fromThisDic, F otherValue)
        {
            foreach (var item in thisDic)
                try
                {
                    thisDic[item.Key] = fromThisDic[item.Key];
                }
                catch { thisDic[item.Key] = otherValue; }
        }
        public static void Fill<T>(ref SmartList<T> thisList, SmartList<T> fromThisList, T otherValue)
        {
            int len = Math.Min(thisList.Count, fromThisList.Count);
            int i = 0;
            for (i = 0; i < len; i++)
                thisList[i] = fromThisList[i];
            for (; i < thisList.Count; i++)
                thisList[i] = otherValue;
        }
        public static void Fill<T, F>(SmartDictionary<T, F> thisDic, SmartDictionary<T, F> fromThisDic, F otherValue)
        {
            foreach (var item in thisDic)
                try
                {
                    thisDic[item.Key] = fromThisDic[item.Key];
                }
                catch { thisDic[item.Key] = otherValue; }
        }
        public static void Fill<T, F>(SmartKeyValueList<T, F> thisDic, SmartKeyValueList<T, F> fromThisDic, F otherValue)
        {
            foreach (var item in thisDic)
                try
                {
                    thisDic[item.Key] = fromThisDic[item.Key];
                }
                catch { thisDic[item.Key] = otherValue; }
        }

  
        public static IEnumerable<T> Concat<T>(params IEnumerable<T>[] arrays)
        {
            foreach (var arr in arrays)
                foreach (var item in arr)
                    yield return item;
        }
        public static T[] Concat<T>(params T[][] arrays)
        {
            int l, i;
            for (l = i = 0; i < arrays.Length; l += arrays[i].Length, i++) ;
            var a = new T[l];
            for (l = i = 0; i < arrays.Length; l += arrays[i].Length, i++)
                arrays[i].CopyTo(a, l);

            return a;
        }
        public static List<T> Concat<T>(params List<T>[] lists)
        {
            for (int i = 1; i < lists.Length; i++)
                lists[0] = lists[0].Concat(lists[i]).ToList();
            return lists[0];
        }
        public static Dictionary<T, F> Concat<T, F>(params Dictionary<T, F>[] dics)
        {
            Dictionary<T, F> lt = new Dictionary<T, F>();
            foreach (var item in dics)
                foreach (var itm in item)
                    try { lt.Add(itm.Key,itm.Value); }
                    catch { }
            return lt;
        }
        public static T[] ConcatWith<T>(T[] array,params T[] args)
        {
            return Concat(array, args);
        }
        public static List<T> ConcatWith<T>(List<T> list,params T[] args)
        {
            return Concat(list, args.ToList());
        }
        public static Dictionary<T, F> ConcatWith<T, F>(Dictionary<T, F> dic, params KeyValuePair<T, F>[] args)
        {
            return Concat(dic, ConvertService.ToDictionary(args));
        }
        public static SmartList<T> Concat<T>(params SmartList<T>[] lists)
        {
            SmartList<T> lt = new SmartList<T>();
            foreach (var item in lists)
                foreach (var itm in item)
                    lt.Add(itm);
            return lt;
        }
        public static SmartDictionary<T, F> Concat<T, F>(params SmartDictionary<T, F>[] dics)
        {
            SmartDictionary<T, F> lt = new SmartDictionary<T, F>();
            foreach (var item in dics)
                foreach (var itm in item)
                    try { lt.Add(itm.Key, itm.Value); }
                    catch { }
            return lt;
        }
        public static SmartKeyValueList<T, F> Concat<T, F>(params SmartKeyValueList<T, F>[] dics)
        {
            SmartKeyValueList<T, F> lt = new SmartKeyValueList<T, F>();
            foreach (var item in dics)
                foreach (var itm in item)
                    try { lt.Add(itm.Key, itm.Value); }
                    catch { }
            return lt;
        }
        public static SmartList<T> ConcatWith<T>(SmartList<T> list, params T[] args)
        {
            return Concat(list, (SmartList<T>)args.ToList());
        }
        public static SmartDictionary<T, F> ConcatWith<T, F>(SmartDictionary<T, F> dic, params KeyValuePair<T, F>[] args)
        {
            return Concat(dic, ConvertService.ToMiMFaDictionary(args));
        }
        public static SmartKeyValueList<T, F> ConcatWith<T, F>(SmartKeyValueList<T, F> dic, params SmartKeyValue<T, F>[] args)
        {
             dic.AddRange(args);
            return dic;
        }


        public static T[] RemoveOf<T>(T[] array, params T[][] thisArrays)
        {
            T[] lt = new T[array.Length];
            int ix = 0;
            for (int i = 0; i < thisArrays.Length; i++)
                for (int j = 0; j < thisArrays[i].Length; j++)
                    if ( FindIndex(array, thisArrays[i][j]) == -1) lt[ix] = thisArrays[i][j];
            return lt;
        }
        public static List<T> RemoveOf<T>(List<T> list, params List<T>[] thisLists)
        {
            List<T> lt = Concat(list);
            for (int i = 0; i < thisLists.Length; i++)
                for (int j = 0; j < thisLists[i].Count; j++)
                {
                    int index = -1;
                    if ((index = FindIndex(lt, thisLists[i][j])) > -1) lt.RemoveAt(index);
                }
            return lt;
        }
        public static Dictionary<T, F> RemoveOf<T, F>(Dictionary<T, F> dic, params Dictionary<T, F>[] thisDics)
        {
            Dictionary<T, F> Dtf = Concat(dic);
            for (int i = 0; i < thisDics.Length; i++)
                foreach (var item in thisDics[i].Keys)
                    try { Dtf.Remove(item); }
                    catch { }
            return Dtf;
        }
        public static SmartList<T> RemoveOf<T>(SmartList<T> list, params SmartList<T>[] thisLists)
        {
            SmartList<T> lt = Concat(list);
            for (int i = 0; i < thisLists.Length; i++)
                for (int j = 0; j < thisLists[i].Count; j++)
                {
                    int index = -1;
                    if ((index = FindIndex(lt, thisLists[i][j])) > -1) lt.RemoveAt(index);
                }
            return lt;
        }
        public static SmartDictionary<T, F> RemoveOf<T, F>(SmartDictionary<T, F> dic, params SmartDictionary<T, F>[] thisDics)
        {
            SmartDictionary<T, F> Dtf = Concat(dic);
            for (int i = 0; i < thisDics.Length; i++)
                foreach (var item in thisDics[i].Keys)
                    try { Dtf.Remove(item); }
                    catch { }
            return Dtf;
        }
        public static SmartKeyValueList<T, F> RemoveOf<T, F>(SmartKeyValueList<T, F> dic, params SmartKeyValueList<T, F>[] thisDics)
        {
            SmartKeyValueList<T, F> Dtf = Concat(dic);
            for (int i = 0; i < thisDics.Length; i++)
                foreach (var item in thisDics[i])
                    try { Dtf.Remove(item.Key); }
                    catch { }
            return Dtf;
        }


        public static T[] RemoveAt<T>(T[] array, params int[] indeces)
        {
            return RemoveAt(array.ToList(),indeces).ToArray();
        }
        public static List<T> RemoveAt<T>(List<T> list, params int[] indeces)
        {
            for (int i = 0; i < indeces.Length; i++)
                list.RemoveAt(indeces[i]);
            return list;
        }
        public static Dictionary<T, F> RemoveAt<T, F>(Dictionary<T, F> dic, params T[] keys)
        {
            for (int i = 0; i < keys.Length; i++)
                dic.Remove(keys[i]);
            return dic;
        }
        public static SmartList<T> RemoveAt<T>(SmartList<T> list, params int[] indeces)
        {
            for (int i = 0; i < indeces.Length; i++)
                list.RemoveAt(indeces[i]);
            return list;
        }
        public static SmartDictionary<T, F> RemoveAt<T, F>(SmartDictionary<T, F> dic, params T[] keys)
        {
            for (int i = 0; i < keys.Length; i++)
                dic.Remove(keys[i]);
            return dic;
        }
        public static SmartKeyValueList<T, F> RemoveAt<T, F>(SmartKeyValueList<T, F> dic, params T[] keys)
        {
            for (int i = 0; i < keys.Length; i++)
                dic.Remove(keys[i]);
            return dic;
        }


        public static void CopyTo<T>(ref T[] array, params T[][] thisArray)
        {
            int len = array.Length;
            foreach (var item in thisArray)
                len += item.Length;
            T[] newt = new T[len];
            int i = 0;
            foreach (var item in array)
                newt[i++] = item;
            foreach (var item in thisArray)
                foreach (var itm in item)
                    newt[i++] = itm;
            array = newt;
        }
        public static void CopyTo<T>(ref Stack<T> stack, params Stack<T>[] thisStacks)
        {
            foreach (var item in thisStacks)
                foreach (var itm in item)
                    stack.Push(itm);
        }
        public static void CopyTo<T>(ref List<T> list, params List<T>[] thisLists)
        {
            foreach (var item in thisLists)
                foreach (var itm in item)
                    list.Add(itm);
        }
        public static void CopyTo<T, F>(ref Dictionary<T, F> dic, params Dictionary<T, F>[] thisDics)
        {
            foreach (var item in thisDics)
                foreach (var itm in item)
                    try { dic.Add(itm.Key, itm.Value); }
                    catch { }
        }
        public static void CopyTo<T>(ref SmartStack<T> stack, params SmartStack<T>[] thisStacks)
        {
            foreach (var item in thisStacks)
                foreach (var itm in item)
                    stack.Push(itm);
        }
        public static void CopyTo<T>(ref SmartList<T> list, params SmartList<T>[] thisLists)
        {
            foreach (var item in thisLists)
                foreach (var itm in item)
                    list.Add(itm);
        }
        public static void CopyTo<T, F>(ref SmartDictionary<T, F> dic, params SmartDictionary<T, F>[] thisDics)
        {
            foreach (var item in thisDics)
                foreach (var itm in item)
                    try { dic.Add(itm.Key, itm.Value); }
                    catch { }
        }
        public static void CopyTo<T, F>(ref SmartKeyValueList<T, F> dic, params SmartKeyValueList<T, F>[] thisDics)
        {
            foreach (var item in thisDics)
                foreach (var itm in item)
                    try { dic.Add(itm.Key, itm.Value); }
                    catch { }
        }


        public static dynamic ExecuteInAllItems( dynamic collection, Func<dynamic, dynamic> thisFunction)
        {
            for (int i = 0; i < collection.Count; i++)
                collection[i] = thisFunction(collection[i]);
            return collection;
        }
        public static T[] ExecuteInAllItems<T>( T[] array, Func<T,T> thisFunction)
        {
            for (int i = 0; i < array.Length; i++)
                array[i] = thisFunction(array[i]);
            return array;
        }
        public static List<T> ExecuteInAllItems<T>( List<T> list, Func<T, T> thisFunction)
        {
            for (int i = 0; i < list.Count; i++)
                list[i] = thisFunction(list[i]);
            return list;
        }
        public static Dictionary<T, F> ExecuteInAllItems<T, F>( Dictionary<T, F> dic, Func<T, F,KeyValuePair<T,F>> thisFunction)
        {
            Dictionary<T, F> ndic = new Dictionary<T, F>();
            foreach (var item in dic.Keys)
            {
                KeyValuePair<T, F>  kvp = thisFunction(item, dic[item]);
                ndic.Add(kvp.Key,kvp.Value);
            }
            return ndic;
        }
        public static Dictionary<T, F> ExecuteInAllItemsKey<T, F>( Dictionary<T, F> dic, Func<T,F,T> thisFunction)
        {
            Dictionary<T, F> ndic = new Dictionary<T, F>();
            foreach (var item in dic.Keys)
                ndic.Add(thisFunction(item, dic[item]),dic[item]);
            return ndic;
        }
        public static Dictionary<T, F> ExecuteInAllItemsValue<T, F>( Dictionary<T, F> dic, Func<T,F,F> thisFunction)
        {
            Dictionary<T, F> ndic = new Dictionary<T, F>();
            foreach (var item in dic.Keys)
                ndic.Add(item,thisFunction(item, dic[item]));
            return ndic;
        }
        public static SmartList<T> ExecuteInAllItems<T>(SmartList<T> list, Func<T, T> thisFunction)
        {
            for (int i = 0; i < list.Count; i++)
                list[i] = thisFunction(list[i]);
            return list;
        }
        public static SmartDictionary<T, F> ExecuteInAllItems<T, F>(SmartDictionary<T, F> dic, Func<T, F, KeyValuePair<T, F>> thisFunction)
        {
            SmartDictionary<T, F> ndic = new SmartDictionary<T, F>();
            foreach (var item in dic.Keys)
            {
                KeyValuePair<T, F> kvp = thisFunction(item, dic[item]);
                ndic.Add(kvp.Key, kvp.Value);
            }
            return ndic;
        }
        public static SmartDictionary<T, F> ExecuteInAllItemsKey<T, F>(SmartDictionary<T, F> dic, Func<T, F, T> thisFunction)
        {
            SmartDictionary<T, F> ndic = new SmartDictionary<T, F>();
            foreach (var item in dic.Keys)
                ndic.Add(thisFunction(item, dic[item]), dic[item]);
            return ndic;
        }
        public static SmartDictionary<T, F> ExecuteInAllItemsValue<T, F>(SmartDictionary<T, F> dic, Func<T, F, F> thisFunction)
        {
            SmartDictionary<T, F> ndic = new SmartDictionary<T, F>();
            foreach (var item in dic.Keys)
                ndic.Add(item, thisFunction(item, dic[item]));
            return ndic;
        }
        public static SmartKeyValueList<T, F> ExecuteInAllItems<T, F>(SmartKeyValueList<T, F> dic, Func<T, F, KeyValuePair<T, F>> thisFunction)
        {
            SmartKeyValueList<T, F> ndic = new SmartKeyValueList<T, F>();
            foreach (var item in dic.Keys)
            {
                KeyValuePair<T, F> kvp = thisFunction(item, dic[item]);
                ndic.Add(kvp.Key, kvp.Value);
            }
            return ndic;
        }
        public static SmartKeyValueList<T, F> ExecuteInAllItemsKey<T, F>(SmartKeyValueList<T, F> dic, Func<T, F, T> thisFunction)
        {
            SmartKeyValueList<T, F> ndic = new SmartKeyValueList<T, F>();
            foreach (var item in dic.Keys)
                ndic.Add(thisFunction(item, dic[item]), dic[item]);
            return ndic;
        }
        public static SmartKeyValueList<T, F> ExecuteInAllItemsValue<T, F>(SmartKeyValueList<T, F> dic, Func<T, F, F> thisFunction)
        {
            SmartKeyValueList<T, F> ndic = new SmartKeyValueList<T, F>();
            foreach (var item in dic.Keys)
                ndic.Add(item, thisFunction(item, dic[item]));
            return ndic;
        }

        public static string GetAllItems<T>(T[] array, string splitor = " ")
        {
            return string.Join(splitor, array);
        }
        public static string GetAllItems<T>(Stack<T> stack, string splitor = " ")
        {
            return string.Join(splitor, stack);
        }
        public static string GetAllItems<T>(List<T> list, string splitor = " ")
        {
            return string.Join(splitor, list);
        }
        public static string GetAllItems<T>(T[] array, string splitor, int ofThisIndex, int toThisIndex = -1)
        {
            int length = (toThisIndex < 0) ? array.Length : Math.Min(array.Length - ofThisIndex, toThisIndex - ofThisIndex);
            return string.Join(splitor, array.ToList().GetRange(ofThisIndex, length));
        }
        public static string GetAllItems<T>(Stack<T> stack, string splitor, int ofThisIndex , int toThisIndex = -1)
        {
            int length = (toThisIndex < 0) ? stack.Count : Math.Min(stack.Count- ofThisIndex, toThisIndex - ofThisIndex);
            return string.Join(splitor, stack.ToList().GetRange(ofThisIndex, length));
        }
        public static string GetAllItems<T>(List<T> list, string splitor, int ofThisIndex, int toThisIndex = -1)
        {
            int length = (toThisIndex < 0) ? list.Count : Math.Min(list.Count - ofThisIndex, toThisIndex - ofThisIndex);
            return string.Join(splitor, list.GetRange(ofThisIndex, length));
        }
        public static string GetAllItems<T,F>(Dictionary<T,F> dic, string middlesign = ": ", string splitor = " ")
        {
            List<string> ls = new List<string>();
            foreach (var item in dic)
                ls.Add(string.Join(middlesign, item.Key, item.Value));
            return string.Join(splitor, ls);
        }
        public static string GetAllKeysItem<T,F>(Dictionary<T,F> dic, string splitor = " ")
        {
            return string.Join(splitor, dic.Keys);
        }
        public static string GetAllValuesItem<T,F>(Dictionary<T,F> dic, string splitor = " ")
        {
            return string.Join(splitor, dic.Values);
        }
        public static List<T> GetKeysList<T, F>(Dictionary<T, F> dic)
        {
            return dic.Keys.ToList<T>();
        }
        public static List<F> GetValuesList<T, F>(Dictionary<T, F> dic)
        {
            return dic.Values.ToList<F>();
        }
        public static T[] GetKeysArray<T, F>(Dictionary<T, F> dic)
        {
            return dic.Keys.ToArray<T>();
        }
        public static F[] GetValuesArray<T, F>(Dictionary<T, F> dic)
        {
            return dic.Values.ToArray<F>();
        }
        public static string GetAllItems<T>(SmartStack<T> stack, string splitor = " ", int ofThisIndex = 0, int toThisIndex = -1)
        {
            int length = (toThisIndex < 0) ? stack.Count : Math.Min(stack.Count - ofThisIndex, toThisIndex - ofThisIndex);
            return string.Join(splitor, stack.ToList().GetRange(ofThisIndex, length));
        }
        public static string GetAllItems<T>(SmartList<T> list, string splitor = " ", int ofThisIndex = 0, int toThisIndex = -1)
        {
            int length = (toThisIndex < 0) ? list.Count : Math.Min(list.Count - ofThisIndex, toThisIndex - ofThisIndex);
            return string.Join(splitor, list.GetRange(ofThisIndex, length));
        }
        public static string GetAllItems<T, F>(SmartDictionary<T, F> dic, string middlesign = ": ", string splitor = " ")
        {
            List<string> ls = new List<string>();
            foreach (var item in dic)
                ls.Add(string.Join(middlesign, item.Key,item.Value));
            return string.Join(splitor,ls);
        }
        public static string GetAllKeysItem<T, F>(SmartDictionary<T, F> dic, string splitor = " ")
        {
            return string.Join(splitor, dic.Keys);
        }
        public static string GetAllValuesItem<T, F>(SmartDictionary<T, F> dic, string splitor = " ")
        {
            return string.Join(splitor, dic.Values);
        }
        public static List<T> GetKeysList<T, F>(SmartDictionary<T, F> dic)
        {
            return dic.Keys.ToList<T>();
        }
        public static List<F> GetValuesList<T, F>(SmartDictionary<T, F> dic)
        {
            return dic.Values.ToList<F>();
        }
        public static T[] GetKeysArray<T, F>(SmartDictionary<T, F> dic)
        {
            return dic.Keys.ToArray<T>();
        }
        public static F[] GetValuesArray<T, F>(SmartDictionary<T, F> dic)
        {
            return dic.Values.ToArray<F>();
        }
        public static string GetAllItems<T, F>(SmartKeyValueList<T, F> dic, string middlesign = ": ", string splitor = " ")
        {
            List<string> ls = new List<string>();
            foreach (var item in dic)
                ls.Add(string.Join(middlesign, item.Key, item.Value));
            return string.Join(splitor, ls);
        }
        public static string GetAllKeysItem<T, F>(SmartKeyValueList<T, F> dic, string splitor = " ")
        {
            return string.Join(splitor, dic.Keys);
        }
        public static string GetAllValuesItem<T, F>(SmartKeyValueList<T, F> dic, string splitor = " ")
        {
            return string.Join(splitor, dic.Values);
        }
        public static List<T> GetKeysList<T, F>(SmartKeyValueList<T, F> dic)
        {
            return dic.Keys.ToList<T>();
        }
        public static List<F> GetValuesList<T, F>(SmartKeyValueList<T, F> dic)
        {
            return dic.Values.ToList<F>();
        }
        public static T[] GetKeysArray<T, F>(SmartKeyValueList<T, F> dic)
        {
            return dic.Keys.ToArray<T>();
        }
        public static F[] GetValuesArray<T, F>(SmartKeyValueList<T, F> dic)
        {
            return dic.Values.ToArray<F>();
        }

        public static KeyValuePair<T,F>? Find<T, F>(Dictionary<T, F> dic,Func<KeyValuePair<T, F>,bool> func)
        {
            foreach (var item in dic)
                if (func(item)) return item;
            return null;
        }
        public static SmartKeyValue<T,F> Find<T, F>(SmartKeyValueList<T, F> dic,Func<SmartKeyValue<T, F>,bool> func)
        {
            foreach (var item in dic)
                if (func(item)) return item;
            return null;
        }

        public static IEnumerable<int> Create(int start, int length)
        {
            for (int i = 0; i < length; i++)
                yield return start++;
        }
        public static IEnumerable<long> Create(long start, int length)
        {
            for (int i = 0; i < length; i++)
                yield return start++;
        }

        public static IEnumerable<T> GetItems<T>(IEnumerable<T> collection, IEnumerable<int> colIndexes)
        {
            foreach (var item in colIndexes) yield return collection.ElementAtOrDefault(item);
        }
        public static IEnumerable<T> GetItems<T>(IEnumerable<T> collection, params int[] colIndexes)
        {
            return GetItems(collection, colIndexes);
        }

        public static IEnumerable<T> GetLine<T>(IEnumerable<IEnumerable<T>> collection)
        {
            foreach (var item in collection)
                foreach (var val in item)
                        yield return val;
        }

        public static IEnumerable<T> TakeOrDefault<T>(IEnumerable<T> collection, long number, T def = default(T))
        {
            var coll = collection.GetEnumerator();
            long i = 0;
            for (; i < number; i++)
                if (coll.MoveNext()) yield return coll.Current;
                else break;
            for (; i < number; i++)
                yield return def;
        }
        public static IEnumerable<IEnumerable<T>> Split<T>(IEnumerable<T> collection, int sliceLength)
        {
            while (collection.Any())
            {
                yield return collection.Take(sliceLength);
                collection = collection.Skip(sliceLength);
            }
        }
    }
}
