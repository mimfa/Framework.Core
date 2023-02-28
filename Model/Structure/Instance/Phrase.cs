using System;
using System.Collections.Generic;
using MiMFa.Exclusive.DateAndTime;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;

namespace MiMFa.Model.Structure.Instance
{
    [Serializable]
    public class Phrase
    {
        public int ID;
        public int Length;
        public string[] Words;
        public IEnumerable<string> OrderedWords => from v in Orders select Words[v];
        public int[] Orders;

        public override string ToString() => ToString(" ");
        public string ToString(string separator)
        {
            return string.Join(separator, OrderedWords);
        }

        public override int GetHashCode()
        {
            return ID;
        }
        public bool Equals(Phrase y)
        {
            if (Length != y.Length) return false;
            if (Words.Length != y.Words.Length) return false;
            for (int i = 0; i < Words.Length; i++)
                if (Words[i].ToLower() != y.Words[i].ToLower()) 
                    return false;
            return true;
        }
        public bool Likes(Phrase y)
        {
            if (Words.Length != y.Words.Length) return false;
            for (int i = 0; i < Words.Length; i++)
            {
                string xv = Words[i].ToLower();
                string yv = y.Words[i].ToLower();
                if (xv!=yv && !xv.StartsWith(yv) && !yv.StartsWith(xv))
                    return false;
            }
            return true;
        }
        public void Complete(Phrase y)
        {
            for (int i = 0; i < Words.Length; i++)
                if (y.Words[i].ToLower().StartsWith(Words[i].ToLower()))
                    Words[i] = y.Words[i];
        }

        public Phrase(Regex wordSeparatorRegex, string phrase)
            : this(wordSeparatorRegex.Split(phrase))
        {
        }
        public Phrase(string[] words) => Set(words);

        public void Refresh() => Set(OrderedWords.ToArray());
        public void Set(string[] words)
        {
            Length = (from v in words select v.Length).Sum();
            Words = words.OrderByDescending(v => v).ToArray();
            ID = string.Join("", Words).GetHashCode();
            Orders = new int[Words.Length];

            for (int i = 0; i < Orders.Length; i++)
                Orders[i] = -1;

            for (int i = 0; i < Words.Length; i++)
                for (int j = 0; j < words.Length; j++)
                    if (Words[i] == words[j] && !Orders.Contains(j))
                        Orders[i] = j;
        }
    }
    public class PhrasesEqualityComparer : IEqualityComparer<Phrase>
    {
        public bool Equals(Phrase x, Phrase y)
        {
            return x.Equals(y);
        }

        public int GetHashCode(Phrase obj)
        {
            return obj.ID;
        }
    }
    public class PhrasesLikenessComparer : IEqualityComparer<Phrase>
    {
        public bool Equals(Phrase x, Phrase y)
        {
            return x.Likes(y);
        }

        public int GetHashCode(Phrase obj)
        {
            return obj.ID;
        }
    }

}
