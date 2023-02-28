using MiMFa.General;
using MiMFa.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MiMFa.Engine
{
    public class Search
    {
         
        public string SearchWord { get; internal set; } 
        public string SearchPattern { get; internal set; } 
        public string SearchUnsignedLowerWord { get; internal set; }
        public string[] SearchLowerWords { get; internal set; }
        public string SearchUnsignedWord { get; internal set; }
        public string[] SearchWords { get; internal set; }

        public Search(string searchWord)
        {
            Set(searchWord);
        }
        public void Set(string searchWord)
        {
            SearchWord = searchWord;
            SearchPattern = ConvertService.FromRegexPattern(searchWord);
            SearchUnsignedLowerWord = ConvertService.ToUnSigned(searchWord.ToLower());
            SearchLowerWords = ConvertService.ToSignSplitted(searchWord.ToLower());
            SearchUnsignedWord = ConvertService.ToUnSigned(searchWord);
            SearchWords = ConvertService.ToSignSplitted(searchWord);
        }

        public bool FindSameIn(string text)
        {
            if (string.IsNullOrEmpty(text)) return false;
            return text.Contains(SearchWord);
        }
        public bool FindLikeIn(string text)
        {
            if (string.IsNullOrEmpty(text)) return false;
            return ConvertService.ToUnSigned(text.ToLower()).Contains(SearchUnsignedLowerWord);
        }
        public bool FindAnyIn(string text)
        {
            if (string.IsNullOrEmpty(text)) return false;
            foreach (var item in SearchLowerWords)
                if (ConvertService.ToUnSigned(text.ToLower()).Contains(item)) return true;
            return false;
        }
        public bool FindPatternIn(string text)
        {
            if (string.IsNullOrEmpty(text)) return false;
            if (!InfoService.IsValidRegexPattern(SearchPattern, text, 20)) throw new Exception("The pattern is not valid!");
            return Regex.IsMatch(text, SearchPattern);
        }

        public string ReplaceSameIn(string text, string newWord)
        {
            if (string.IsNullOrEmpty(text)) return text;
            return text.Replace(SearchWord, newWord);
        }
        public string ReplaceLikeIn(string text, string newWord)
        {
            if (string.IsNullOrEmpty(text)) return text;
            return Regex.Replace(text, SearchWord, newWord, RegexOptions.IgnoreCase);
        }
        public string ReplaceAnyIn(string text, string newWord)
        {
            if (string.IsNullOrEmpty(text)) return text;
            foreach (var item in SearchWords)
                text = Regex.Replace(text, item, newWord, RegexOptions.IgnoreCase);
            return text;
        }
        public string ReplacePatternIn(string text,string newWord)
        {
            if (string.IsNullOrEmpty(text)) return text;
            return Regex.Replace(text, SearchPattern, newWord);
        }


        public static bool FindSame(string text, string searchWord)
        {
            if (string.IsNullOrEmpty(text)) return false;
            return text.Contains(searchWord);
        }
        public static bool FindLike(string text, string searchWord)
        {
            if (string.IsNullOrEmpty(text)) return false;
            return ConvertService.ToUnSigned(text.ToLower()).Contains(ConvertService.ToUnSigned(searchWord.ToLower()));
        }
        public static bool FindAny(string text, string searchWord)
        {
            if (string.IsNullOrEmpty(text)) return false;
            string[] stra = ConvertService.ToSignSplitted(searchWord.ToLower());
            foreach (var item in stra)
                if (ConvertService.ToUnSigned(text.ToLower()).Contains(item)) return true;
            return false;
        }
        public static bool FindPattern(string text, string searchPattern)
        {
            if (string.IsNullOrEmpty(text)) return false;
            return Regex.IsMatch(text, searchPattern);
        }

        public static string ReplaceSame(string text, string searchWord, string newWord)
        {
            if (string.IsNullOrEmpty(text)) return text;
            return text.Replace(searchWord, newWord);
        }
        public static string ReplaceLike(string text, string searchWord, string newWord)
        {
            if (string.IsNullOrEmpty(text)) return text;
            return Regex.Replace(text, searchWord, newWord, RegexOptions.IgnoreCase);
        }
        public static string ReplaceAny(string text, string searchWord, string newWord)
        {
            if (string.IsNullOrEmpty(text)) return text;
            string[] stra = ConvertService.ToSignSplitted(searchWord);
            foreach (var item in stra)
                text = Regex.Replace(text, item, newWord, RegexOptions.IgnoreCase);
            return text;
        }
        public static string ReplacePattern(string text, string searchPattern, string newWord)
        {
            if (string.IsNullOrEmpty(text)) return text;
            return Regex.Replace(text, searchPattern, newWord);
        }
    }
}
