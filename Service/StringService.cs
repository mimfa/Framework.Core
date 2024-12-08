using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using MiMFa.Model.Structure;
using MiMFa.Model;
using System.Text.RegularExpressions;

namespace MiMFa.Service
{
    public class StringService
    {
        public static Percent ComparePerCent(string str1, string str2)
        {
            if (string.IsNullOrEmpty(str1) && string.IsNullOrEmpty(str2)) return new Percent(0, 0, 100);
            if (string.IsNullOrEmpty(str1) && !string.IsNullOrEmpty(str2)) return new Percent(-100, 0, 0);
            if (!string.IsNullOrEmpty(str1) && string.IsNullOrEmpty(str2)) return new Percent(0, -100, 0);
            if (str1 == str2) return new Percent(0, 0, 100);
            str1 = str1.Trim().Replace(Environment.NewLine, " ").Replace("   ", " ").Replace("  ", " ");
            str2 = str2.Trim().Replace(Environment.NewLine, " ").Replace("   ", " ").Replace("  ", " ");
            if (str1 == str2) return new Percent(-1, 0, 99);
            int ct = str1.CompareTo(str2);
            if (ct == 0) return new Percent(-5, 0, 95);
            string str1l = str1.ToLower();
            string str2l = str2.ToLower();
            if (str1l == str2l)
            {
                Percent mm = new Percent(0, 0, 90);
                double fu = 10D / str1.Length;
                for (int i = 0; i < str1.Length; i++)
                    mm.AddValue((str1[i] == str2[i]) ? fu : -fu);
                return mm;
            }
            ct = str1l.CompareTo(str2l);
            if (ct == 0) return new Percent(-10, 0, 90);
            double unit = Convert.ToDouble(100) / ((str2.Length + str1.Length) / 2);
            int telor = Math.Abs(str1.Length - str2.Length);
            double tu = telor * unit;
            if (str1.Contains(str2l) || str2l.Contains(str1))
                return (new Percent(-tu, 0, 100 - tu));
            if (str1l.Contains(str2l) || str2l.Contains(str1l))
                return (new Percent(-10 - tu, 0, 90 - tu));
            if (str1.Length < 2 || str2.Length < 2) return new Percent(-100, 0, 0);
            int m1 = str1.Length / 2;
            int m2 = str2.Length / 2;
            Percent percent1 = ComparePerCent(str1.Substring(0, m1), str2.Substring(0, m2)) / 2;
            Percent percent2 = ComparePerCent(str1.Substring(m1), str2.Substring(m2)) / 2;
            Percent percent = percent1 + percent2;
            if (percent.Positive > 60) return percent;
            string s1 = str1, s2 = str2;
            if (str1.Length < str2.Length)
            {
                s1 = str2;
                s2 = str1;
            }
            int j = 0;
            percent = new Percent(-30, 0, 0);
            for (int i = 0; i < s2.Length; i++)
                if (s1[i] != s2[j++] && s1[i] != s2[(j>0)?j - 1: 0])
                    percent.AddValue(-unit);
                else
                    percent.AddValue(unit);
            percent.AddValue(-tu);
            percent = percent;
            return percent;
        }

        public static string Compress(string text,int maxlength= 20,string pressedsign = "⋯",bool reverse = false)
        {
            if (maxlength == 0) return "";
            if (string.IsNullOrEmpty(text) || maxlength < 0) return text;
            text = text.Trim();
            if (text.Length <= maxlength) return text;
            if(reverse) return pressedsign + text.Substring(maxlength - pressedsign.Length);
            else return text.Substring(0, maxlength - pressedsign.Length) + pressedsign;
        }
        
        public static List<string> WordsBetween(string intoText, string fromThis, string toThis, bool withSigns = true, bool CaseSensitive = true)
        {
            List<string> result = new List<string>();
            if (string.IsNullOrEmpty(intoText)) return result;
            Stack <int> stack = new Stack<int>();
            bool start = false;
            string middletext = CaseSensitive ? intoText : intoText.ToLower();
            if (!CaseSensitive)
            {
                fromThis = fromThis.ToLower();
                toThis = toThis.ToLower();
            }
            for (int i = 0; i < intoText.Length; i++)
            {
                string ssubstr = middletext.Substring(i);
                if (ssubstr.StartsWith(fromThis) && ((fromThis == toThis && !start) || !ssubstr.StartsWith(toThis)))
                {
                    if (!withSigns) i += fromThis.Length;
                    start = true;
                    stack.Push(i);
                }
                else if (ssubstr.StartsWith(toThis))
                {
                    if (withSigns) i += toThis.Length;
                    start = false;
                    if (stack.Count > 0)
                    {
                        int startindex = stack.Pop();
                        result.Add(intoText.Substring(startindex,i - startindex));
                    }
                }
            }
            return result;
        }
        public static string FirstWordBetween(string intoText, string fromThis, string toThis,bool withSigns = true)
        {
            Stack<int> stack = new Stack<int>();
            bool start = false;
            for (int i = 0; i < intoText.Length; i++)
            {
                string ssubstr = intoText.Substring(i);
                if (ssubstr.StartsWith(fromThis) && ((fromThis == toThis && !start) || !ssubstr.StartsWith(toThis)))
                {
                    if (!withSigns) i += fromThis.Length;
                    start = true;
                    stack.Push(i);
                }
                else if (ssubstr.StartsWith(toThis) && start)
                {
                    if (withSigns) i += toThis.Length;
                    start = false;
                    if (stack.Count == 1)
                    {
                        int startindex = stack.Pop();
                        return intoText.Substring(startindex,i - startindex);
                    }
                }
            }
            return null;
        }
        public static List<Point> WordsIndecesBetween(string intoText, string fromThis, string toThis,bool withSigns = true)
        {
            List<Point> result = new List<Point>();
            Stack<int> stack = new Stack<int>();
            int fl = fromThis.Length;
            bool start = false;
            if (toThis.Length > 0)
                if (withSigns)
                    for (int i = 0; i < intoText.Length; i++)
                    {
                        string ssubstr = intoText.Substring(i);
                        if (ssubstr.StartsWith(fromThis) && ((fromThis == toThis && !start) || !ssubstr.StartsWith(toThis)))
                        {
                            start = true;
                            stack.Push(i);
                        }
                        else if (ssubstr.StartsWith(toThis))
                        {
                            i += toThis.Length;
                            start = false;
                            if (stack.Count > 0)
                            {
                                int startindex = stack.Pop();
                                result.Add(new Point(startindex, i));
                            }
                        }
                    }
                else for (int i = 0; i < intoText.Length; i++)
                    {
                        string ssubstr = intoText.Substring(i);
                        if (ssubstr.StartsWith(fromThis) && ((fromThis == toThis && !start) || !ssubstr.StartsWith(toThis)))
                        {
                            i += fl;
                            start = true;
                            stack.Push(i);
                        }
                        else if (ssubstr.StartsWith(toThis))
                        {
                            start = false;
                            if (stack.Count > 0)
                            {
                                int startindex = stack.Pop();
                                result.Add(new Point(startindex, i));
                            }
                        }
                    }
            else for (int i = 0; i < intoText.Length; i++)
                    if (intoText.Substring(i).StartsWith(fromThis))
                        result.Add(new Point(i, i += fl));

            return result;
        }
        public static string FilterCharacters(string text,string replacement,params int[][] range)
        {
            return string.Join("", text.Select((c) => 
            {
                int ci = (int)c;
                bool b = false;
                foreach (var item in range)
                    if (b = (ci >= item.First() && ci <= item.Last()))
                        break;
                return b?c.ToString(): replacement;
            }));
        }
        public static string FilterCharacters(string text, string replacement="")
        {
            return string.Join("", text.Select((c) => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c) ? c.ToString(): replacement));
        }
        public static string FilterLetters(string text, string replacement = "")
        {
            return string.Join("", text.Select((c) => char.IsLetter(c)  ? c.ToString(): replacement));
        }
        public static string FilterLettersAndDigits(string text, string replacement = "")
        {
            return string.Join("", text.Select((c) => char.IsLetterOrDigit(c) ? c.ToString(): replacement));
        }
        public static string FilterDigits(string text, string replacement = "")
        {
            return string.Join("", text.Select((c) => char.IsDigit(c) ? c.ToString(): replacement));
        }
        public static string CodeReplace(string intoText, Dictionary<string, string> startDic, string[] endSigns, Func<string, string, string> endFunction, bool CaseSensitive = false)
        {
            if (intoText == null) return intoText;
            string mText = intoText;
            string newText = "";
            bool find = false;
            Stack<string> stack = new Stack<string>();
            if (!CaseSensitive)
            {
                Dictionary<string, string> dic = new Dictionary<string, string>();
                mText = intoText.ToUpper();
                foreach (var item in startDic)
                    dic.Add(item.Key.ToUpper(), item.Value);
                startDic = dic;
            }
            for (int i = 0; i < mText.Length; i++)
            {
                find = false;
                string ssubstr = mText.Substring(i);
                foreach (var item in startDic)
                    if (ssubstr.StartsWith(item.Key))
                    {
                        stack.Push(item.Key);
                        newText += item.Value;
                        i += item.Key.Length -1;
                        find = true;
                        break;
                    }
                if (!find)
                    foreach (var item in endSigns)
                        if (ssubstr.StartsWith(item) && stack.Count > 0)
                        {
                            newText += endFunction(stack.Pop(), item);
                            i += item.Length - 1;
                            find = true;
                            break;
                        }
                if (!find)
                    newText += intoText[i];
            }
            return newText;
        }
        public delegate string Parser(string s,params object[] o);
        public static string CodeReplace(string intoText, Dictionary<string, string> startDic, Dictionary<string,Parser> parserDic,object[] operationObject, Dictionary<string, string> endDic, bool CaseSensitive = false)
        {
            if (intoText == null) return intoText;
            string middleText = intoText;
            string newText = "";
            bool find = false;
            Stack<string> tagStack = new Stack<string>();
            Stack<int> iStack = new Stack<int>();

            try
            {
                if (!CaseSensitive)
                {
                    middleText = intoText.ToUpper();
                    startDic = CollectionService.ExecuteInAllItemsValue(startDic, (key, value) => { return value.ToUpper(); });
                    endDic = CollectionService.ExecuteInAllItemsValue(endDic, (key, value) => { return value.ToUpper(); });
                }
                for (int i = 0; i < middleText.Length; i++)
                {
                    find = false;
                    string ssubstr = middleText.Substring(i);
                    foreach (var item in startDic)
                        if (ssubstr.StartsWith(item.Value))
                        {
                            tagStack.Push(item.Key);
                            iStack.Push(newText.Length);
                            newText += item.Value;
                            i += item.Value.Length - 1;
                            find = true;
                            break;
                        }
                    if (!find)
                        foreach (var item in endDic)
                            if (ssubstr.StartsWith(item.Value) && tagStack.Count > 0 && iStack.Count > 0)
                            {
                                string[] arr = (from v in endDic where v.Value == item.Value select v.Key).ToArray();
                                foreach (var itm in arr)
                                    if (itm == tagStack.First())
                                    {
                                        int iStart = iStack.Pop();
                                        i += item.Value.Length - 1;
                                        newText += item.Value;
                                        newText = newText.Substring(0, iStart) + parserDic[tagStack.Pop()](newText.Substring(iStart, newText.Length - iStart), operationObject);
                                        find = true;
                                        break;
                                    }
                                if (!find)
                                {
                                    iStack.Pop();
                                    tagStack.Pop();
                                }
                                break;
                            }
                    if (!find)
                    {
                        newText += intoText[i];
                    }
                }
            }
            catch { }
            return newText;
        }
        public static string CodeReplace(string intoText, Dictionary<string, string> startDic, Dictionary<string, Func<string,string>> parserDic, object[] operationObject, Dictionary<string, string> endDic, bool CaseSensitive = false)
        {
            if (intoText == null) return intoText;
            Dictionary<string, Parser> pDic = new Dictionary<string, Parser>();
            foreach (var item in parserDic)
                pDic.Add(item.Key, (s,o) => { return item.Value(s); });
            return CodeReplace(intoText, startDic, pDic, operationObject, endDic, CaseSensitive);
        }
        public static string CodeReplace(string intoText,Func<string,int,KeyValuePair<int,string>> func,bool recheck = true)
        {
            if (intoText == null) return intoText;
            for (int i = 0; i < intoText.Length; i++)
            {
                var kvp = func(intoText, i);
                if (kvp.Key > -1)
                {
                    intoText =string.Join("", ((i==0)?"":intoText.Substring(0, i)), kvp.Value ,(intoText.Length>kvp.Key?intoText.Substring(kvp.Key):""));
                    if (recheck) i--;
                    else i += kvp.Value.Length - 1;
                }
            }
            return intoText;
        }

        public static object CapitalFirstLetter(string word) => string.IsNullOrEmpty(word)? word: word.First().ToString().ToUpper() + string.Join("", word.Skip(1)).ToLower();
        public static string ToSeparatedWords(string cancatedString)
        {

            string str = "";
            if (cancatedString != null)
            {
                cancatedString = cancatedString.Replace("_", " ");
                for (int i = 0; i < cancatedString.Length; i++)
                {
                    string ch = cancatedString[i].ToString();
                    if (ch == ch.ToUpper() && i > 0) str += " ";
                    str += ch;
                }
            }
            return str;
        }
        public static string ToXML(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            return text
                .Replace("\n", "&#xA;")
                .Replace("\r", "&#xD;")
                .Replace("\t", "&#x9;")
                .Replace(" ", "&#x20;")
                .Replace("<", "&#x3C;")
                .Replace(">", "&#x3E;");
        }
        public static string FromXML(string xml)
        {
            if (string.IsNullOrEmpty(xml)) return xml;
            return xml
                .Replace("&#xA;", "\n")
                .Replace("&#xD;", "\r")
                .Replace("&#x9;", "\t")
                .Replace("&#x20;", " ")
                .Replace("&#x3C;", "<")
                .Replace("&#x3E;", ">");
        }
        public static string ToHTML(string text)
        {
            if (string.IsNullOrEmpty(text)) return text;
            return Regex.Replace(text.Replace("<", "&lt;")
                .Replace(">", "&gt;").Replace(" ", "&nbsp;")
                .Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;")
                , "\\r?\\n", "<br>");
        }
        public static string FromHTML(string html)
        {
            if (string.IsNullOrEmpty(html)) return html;
            string str = html
                .Replace("</td>", "\t")
                .Replace("</TD>", "\t")
                .Replace("</TR>", Environment.NewLine)
                .Replace("</tr>", Environment.NewLine)
                .Replace("</LI>", Environment.NewLine)
                .Replace("</li>", Environment.NewLine);
            str = ReplaceWordsBetween(str, "<br", ">", Environment.NewLine, false);
            str = ReplaceWordsBetween(str, "<hr", ">", Environment.NewLine, false);
            str = Regex.Replace(str, "<.*?>", string.Empty);
            str = str
                .Replace("&nbsp;", " ")
                .Replace("&nbsp", " ")
                .Replace("&lt;", "<")
                .Replace("&gt;", ">")
                .Replace("&amp;", "&")
                .Replace("&quot;", "\"")
                .Replace("&apos;", "'")
                .Replace("&cent;", "¢")
                .Replace("&pound;", "£")
                .Replace("&yen;", "¥")
                .Replace("&euro;", "€")
                .Replace("&copy;", "©")
                .Replace("&reg;", "®")
                .Replace("&forall;", "∀")
                .Replace("&part;", "∂")
                .Replace("&exist;", "∃")
                .Replace("&empty;", "∅")
                .Replace("&nabla;", "∇")
                .Replace("&isin;", "∈")
                .Replace("&notin;", "∉")
                .Replace("&ni;", "∋")
                .Replace("&prod;", "∏")
                .Replace("&sum;", "∑")
                .Replace("&larr;", "←")
                .Replace("&uarr;", "↑")
                .Replace("&rarr;", "→")
                .Replace("&darr;", "↓")
                .Replace("&harr;", "↔")
                .Replace("&crarr;", "↵")
                .Replace("&lArr;", "⇐")
                .Replace("&uArr;", "⇑")
                .Replace("&rArr;", "⇒")
                .Replace("&dArr;", "⇓")
                .Replace("&hArr;", "⇔")
                .Replace("&Hacek;", "ˇ")
                .Replace("&hairsp;", " ")
                .Replace("&half;", "½")
                .Replace("&hamilt;", "ℋ")
                .Replace("&HARDcy;", "Ъ")
                .Replace("&hardcy;", "ъ")
                .Replace("&hArr;", "⇔")
                .Replace("&harr;", "↔")
                .Replace("&harrcir;", "⥈")
                .Replace("&harrw;", "↭")
                .Replace("&Hat;", "^")
                .Replace("&hbar;", "ℏ")
                .Replace("&Hcirc;", "Ĥ")
                .Replace("&hcirc;", "ĥ")
                .Replace("&hearts;", "♥")
                .Replace("&heartsuit;", "♥")
                .Replace("&hellip;", "…")
                .Replace("&hercon;", "⊹")
                .Replace("&Hfr;", "ℌ")
                .Replace("&hfr;", "𝔥")
                .Replace("&HilbertSpace; ", "ℋ")
                .Replace("&hksearow;", "⤥")
                .Replace("&hkswarow;", "⤦")
                .Replace("&hoarr;", "⇿")
                .Replace("&homtht;", "∻")
                .Replace("&hookleftarrow;", "↩")
                .Replace("&hookrightarrow;", "↪")
                .Replace("&Hopf;", "ℍ")
                .Replace("&hopf;", "𝕙")
                .Replace("&horbar;", "―")
                .Replace("&HorizontalLine;", "─")
                .Replace("&Hscr;", "ℋ")
                .Replace("&hscr;", "𝒽")
                .Replace("&hslash;", "ℏ")
                .Replace("&Hstrok;", "Ħ")
                .Replace("&hstrok;", "ħ")
                .Replace("&HumpDownHump;", "≎")
                .Replace("&HumpEqual;", "≏")
                .Replace("&hybull;", "⁃")
                .Replace("&hyphen;", "‐")
                .Replace("&Iacute;", "Í")
                .Replace("&iacute;", "í")
                .Replace("&ic;", "⁣")
                .Replace("&Icirc;", "Î")
                .Replace("&icirc;", "î")
                .Replace("&Icy;", "И")
                .Replace("&icy;", "и")
                .Replace("&Idot;", "İ")
                .Replace("&IEcy;", "Е")
                .Replace("&iecy;", "е")
                .Replace("&iexcl;", "¡")
                .Replace("&iff;", "⇔")
                .Replace("&Ifr;", "ℑ")
                .Replace("&ifr;", "𝔦")
                .Replace("&Igrave;", "Ì")
                .Replace("&igrave;", "ì")
                .Replace("&ii;", "ⅈ")
                .Replace("&iiiint;", "⨌")
                .Replace("&iiint;", "∭")
                .Replace("&iinfin;", "⧜")
                .Replace("&iiota;", "℩")
                .Replace("&IJlig;", "Ĳ")
                .Replace("&ijlig;", "ĳ")
                .Replace("&Im;", "ℑ")
                .Replace("&Imacr;", "Ī")
                .Replace("&imacr;", "ī")
                .Replace("&image;", "ℑ")
                .Replace("&ImaginaryI;", "ⅈ")
                .Replace("&imagline;", "ℐ")
                .Replace("&imagpart;", "ℑ")
                .Replace("&imath;", "ı")
                .Replace("&imof;", "⊷")
                .Replace("&imped;", "Ƶ")
                .Replace("&Implies;", "⇒")
                .Replace("&in;", "∈")
                .Replace("&incare;", "℅")
                .Replace("&infin;", "∞")
                .Replace("&infintie;", "⧝")
                .Replace("&inodot;", "ı")
                .Replace("&Int;", "∬")
                .Replace("&int;", "∫")
                .Replace("&intcal;", "⊺")
                .Replace("&integers;", "ℤ")
                .Replace("&Integral;", "∫")
                .Replace("&intercal;", "⊺")
                .Replace("&Intersection;", "⋂")
                .Replace("&intlarhk;", "⨗")
                .Replace("&intprod;", "⨼")
                .Replace("&InvisibleComma;", "⁣")
                .Replace("&InvisibleTimes;", "⁢")
                .Replace("&IOcy;", "Ё")
                .Replace("&iocy;", "ё")
                .Replace("&Iogon;", "Į")
                .Replace("&iogon;", "į")
                .Replace("&Iopf;", "𝕀")
                .Replace("&iopf;", "𝕚")
                .Replace("&Iota;", "Ι")
                .Replace("&iota;", "ι")
                .Replace("&iprod;", "⨼")
                .Replace("&iquest;", "¿")
                .Replace("&Iscr;", "ℐ")
                .Replace("&iscr;", "𝒾")
                .Replace("&isin;", "∈")
                .Replace("&isindot;", "⋵")
                .Replace("&isinE;", "⋹")
                .Replace("&isins;", "⋴")
                .Replace("&isinsv;", "⋳")
                .Replace("&isinv;", "∈")
                .Replace("&it;", "⁢")
                .Replace("&Itilde;", "Ĩ")
                .Replace("&itilde;", "ĩ")
                .Replace("&Iukcy;", "І")
                .Replace("&iukcy;", "і")
                .Replace("&Iuml;", "Ï")
                .Replace("&iuml;", "ï");
            if (str.Contains("&#"))
                str = CodeReplace(str, (s, i) =>
            {
                try
                {
                    if (s.Substring(i).StartsWith("&#"))
                    {
                        int index = -1;
                        if ((index = FirstIndex(s.Substring(i), ";")) > -1)
                            return new KeyValuePair<int, string>(i + index + 1, Char.ConvertFromUtf32(Convert.ToInt32(s.Substring(i + 2, index - 2))));
                        else return new KeyValuePair<int, string>(-1, "");
                    }
                    else return new KeyValuePair<int, string>(-1, "");
                }
                catch
                { return new KeyValuePair<int, string>(-1, ""); }
            }
            , false);
            return str;
        }
        public static string HTMLToPlainText(string htmlString)
        {
            var regexCss = new Regex("(\\<script(.+?)\\</script\\>)|(\\<style(.+?)\\</style\\>)", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            htmlString = regexCss.Replace(htmlString, string.Empty);
            htmlString = FromHTML(htmlString);
            //htmlString = Regex.Replace(htmlString, @"^\s+$[\r\n]*", "", RegexOptions.Multiline);
            return htmlString;
        }

        public static string Replace(string intoText, string selectPattern, Func<string,string> replacement, bool caseSensitive = false)
        {
            if (intoText == null) return intoText;
            return Regex.Replace(intoText, selectPattern, new MatchEvaluator(m => replacement(m.Value)), caseSensitive ? RegexOptions.None : RegexOptions.IgnoreCase);
        }
        public static string Replace(string intoText, string newWord, string oldWord)
        {
           return ReplaceWords(intoText, newWord, oldWord);
        }
        public static string ReplaceWords(string intoText, string newWord, params string[] oldWords)
        {
            if (intoText == null) return intoText;
            foreach (var item in oldWords)
                intoText = intoText.Replace(item, newWord);
            return intoText;
        }
        public static string ReplaceWords(string intoText, char newChar, params char[] oldChars)
        {
            if (intoText == null) return intoText;
            foreach (var item in oldChars)
                intoText = intoText.Replace(item, newChar);
            return intoText;
        }
        public static string ReplaceWords(string intoText, List<string> newWords, List<string> oldWords)
        {
            if (intoText == null) return intoText;
            int length = 0;
            if (newWords.Count > oldWords.Count) length = oldWords.Count;
            else length = newWords.Count;
            for (int i = 0; i < length; i++)
                intoText = StringService.FirstReplace(intoText,oldWords[i], newWords[i] );
            return intoText;
        }
        public static string ReplaceWords(string intoText, Dictionary<string,string> oldAndNewWords)
        {
            if (intoText == null) return intoText;
            CollectionService.Sort(oldAndNewWords);
            List<string> ls = oldAndNewWords.Keys.ToList();
            for (int i = ls.Count -1; i >= 0; i--)
                intoText = intoText.Replace(ls[i], oldAndNewWords[ls[i]]);
            return intoText;
        }
        public static string ReplaceFirstWith(string intoText, Dictionary<string, string> oldAndNewWords)
        {
            if (intoText == null) return intoText;
            CollectionService.Sort(oldAndNewWords);
            Dictionary<int, string> dic = new Dictionary<int, string>();
            foreach (var item in oldAndNewWords)
                try { dic.Add(FirstIndex(intoText, item.Key), item.Key); } catch { }
            CollectionService.Sort(dic);
            string old = null;
            string neww = null;
            foreach (var item in dic)
                if (item.Key >= 0)
                {
                    old = item.Value;
                    neww = oldAndNewWords[item.Value];
                    break;
                }
            if (old == null || neww == null) return intoText;
            return intoText.Replace(old, neww);
        }
        public static string ReplaceWithIndices(string intoText, string selectPattern, string replacePatternFormat, bool caseSensitive = false, int startIndex = 0, string indexFormat = "{0}")
        {
            Dictionary<string, string> dic = new Dictionary<string, string>();
            Regex re = caseSensitive ? new Regex(selectPattern): new Regex(selectPattern, RegexOptions.IgnoreCase);
            foreach (Match match in re.Matches(intoText))
                if (!dic.ContainsKey(match.Value))
                    dic.Add(match.Value, string.Format(indexFormat,(dic.Count + startIndex), match.Value));
            foreach (var kvp in dic)
                intoText = Regex.Replace(intoText, string.Format(replacePatternFormat, kvp.Key, kvp.Value), kvp.Value);
            return intoText;
        }
        public static string ReplaceWithIndices(string intoText, string selectPattern, bool caseSensitive = false, int startIndex = 0, string indexFormat = "{0}")
        {
            List<string> ls = new List<string>();
            return Replace(intoText, selectPattern,m =>
            {
                var fi = ls.FindIndex(v=> caseSensitive? v == m: v == m.ToLower());
                if (fi>-1) return string.Format(indexFormat, fi + startIndex, m);
                else
                {
                    ls.Add(caseSensitive ? m:m.ToLower());
                    return string.Format(indexFormat, (ls.Count-1) + startIndex, m);
                }
            }, caseSensitive);
        }
        public static string ReplaceWithUniqueIndices(string intoText, string selectPattern, bool caseSensitive = false, int startIndex = 0, string indexFormat = "{0}")
        {
            return Replace(intoText, selectPattern, m =>
            {
                return string.Format(indexFormat, startIndex++, m);
            }, caseSensitive);
        }

        public static string ReplaceWordsByOrder(string intoText, string[] oldWords, string[] newWords)
        {
            if (intoText == null) return intoText;
            int length = Math.Min(oldWords.Length,newWords.Length);
            for (int i = 0; i < length; i++)
                intoText = FirstReplace(intoText, oldWords[i], newWords[i]);
            return intoText;
        }
        public static string ReplaceWordsByOrder(string intoText, string oldWords, params string[] newWords)
        {
            if (intoText == null) return intoText;
            string newtext = "";
            int j = 0;
            int owi = oldWords.Length;
            for (int i = 0; i < intoText.Length; i++)
            {
                string ssubstr = intoText.Substring(i);
                if (newWords.Length < j) return newtext + ssubstr;
                else if (ssubstr.StartsWith(oldWords) && j < newWords.Length)
                {
                    newtext += newWords[j++];
                    i += owi;
                }
                else newtext += intoText[i];
            }
            return newtext;
        }
        public static string ReplaceWordsBetween(string intoText, string fromThis, string toThis, string replaceWith,out Dictionary<int, string> result , bool withIndex = true, bool caseSensivity = false)
        {
            result = new Dictionary<int, string>();
            if (intoText == null) return intoText;
            Stack<int> stack = new Stack<int>();
            string middle = intoText;
            string newText = "";
            bool start = false;
            bool find = false;
            int index = 0;

            if(!caseSensivity)
            {
                middle = middle.ToUpper();
                fromThis = fromThis.ToUpper();
                toThis = toThis.ToUpper();
            }
            bool signischar = false;
            for (int i = 0; i < middle.Length; i++)
            {
                string ssubstr = middle.Substring(i);
                try { signischar = middle.Substring(i - 3).StartsWith(@"\\\"); } catch { signischar = false; }
                find = false;
                if (ssubstr.StartsWith(fromThis) 
                    && !signischar
                    && ((fromThis == toThis && !start) || !ssubstr.StartsWith(toThis)))
                {
                    stack.Push(newText.Length);
                    newText += fromThis;
                    i += fromThis.Length - 1;
                    find = true;
                    start = true;
                }
                else if (!find && ssubstr.StartsWith(toThis)
                    && !signischar)
                {
                    if (stack.Count > 0)
                    {
                        find = true;
                        start = false;
                        int iStart = stack.Pop();
                        i += toThis.Length -1;
                        newText += toThis;
                        string word = newText.Substring(iStart, newText.Length - iStart);
                        newText = newText.Substring(0, iStart) + replaceWith;
                        index++;
                        if (withIndex) newText += index + '';
                        result.Add(index, word);
                    }
                }
                if(!find) newText += intoText[i];
            }
            return newText;
        }
        public static string ReplaceWordsBetween(string intoText, string fromThis, string toThis, Func<int, string, string> replaceWith, out Dictionary<int, string> result, bool caseSensivity = false)
        {
            result = new Dictionary<int, string>();
            int index = 0;
            string newText = "";
            string middle = intoText;
            if (!caseSensivity)
            {
                middle = middle.ToLower();
                fromThis = fromThis.ToLower();
                toThis = toThis.ToLower();
            }
            string[] sam = middle.Split(new string[] { fromThis }, StringSplitOptions.None);
            if(fromThis == toThis)
            {
                int startIndex = 0;
                for (int i = 0; i < sam.Length; i++)
                    if (i % 2 == 0)
                    {
                        newText += intoText.Substring(startIndex, sam[i].Length);
                        startIndex += sam[i].Length + fromThis.Length;
                    }
                    else
                    {
                        string sj = string.Join("", fromThis, intoText.Substring(startIndex, sam[i].Length), toThis);
                        result.Add(index, sj);
                        newText += replaceWith(index++, sj);
                        startIndex += sam[i].Length + fromThis.Length;
                    }
            }
            else
            {
                int si = fromThis.Length;
                int ei = toThis.Length;
                int length = 0;
                newText += intoText.Substring(0, length += sam[0].Length);
                for (int i = 1; i < sam.Length; i++)
                {
                    length += si;
                    string[] sea = sam[i].Split(new string[] { toThis }, StringSplitOptions.None);
                    int elen = sea[0].Length + ei;
                    string sj = string.Join("", fromThis, sea[0], toThis);
                    result.Add(index, sj);
                    newText += string.Join("", replaceWith(index++, sj) , ((sam[i].Length - elen > 0)?intoText.Substring(length + elen, sam[i].Length - elen):""));
                    length += sam[i].Length;
                }
            }
            return newText;
        }
        public static string ReplaceWordsBetween(string intoText, string fromThis, string toThis, string replaceWith, bool caseSensitive = false)
        {
            Dictionary<int, string> dic = new Dictionary<int, string>();
            return ReplaceWordsBetween( intoText,  fromThis,  toThis,  replaceWith,out dic,caseSensitive);
        }
        public static string ReplaceWordsBetween(string intoText, string fromThis, string toThis, Func<String, string> replaceFunc, bool caseSensivity = false)
        {
            string newText = "";
            string middle = intoText;
            if (!caseSensivity)
            {
                middle = middle.ToLower();
                fromThis = fromThis.ToLower();
                toThis = toThis.ToLower();
            }
            string[] sam = middle.Split(new string[] { fromThis }, StringSplitOptions.None);
            if (fromThis == toThis)
            {
                int startIndex = 0;
                for (int i = 0; i < sam.Length; i++)
                    if (i % 2 == 0)
                    {
                        newText += intoText.Substring(startIndex, sam[i].Length);
                        startIndex += sam[i].Length + fromThis.Length;
                    }
                    else
                    {
                        newText += replaceFunc(string.Join("",fromThis,intoText.Substring(startIndex, sam[i].Length),toThis));
                        startIndex += sam[i].Length + fromThis.Length;
                    }
            }
            else
            {
                int si = fromThis.Length;
                int ei = toThis.Length;
                int length = 0;
                newText += intoText.Substring(0, length += sam[0].Length);
                for (int i = 1; i < sam.Length; i++)
                {
                    length += si;
                    string[] sea = sam[i].Split(new string[] { toThis }, StringSplitOptions.None);
                    int elen = sea[0].Length + ei;
                    string sj = string.Join("", fromThis, sea[0], toThis);
                    newText += replaceFunc(string.Join("", fromThis, sea[0],toThis));
                    length += sam[i].Length;
                }
            }
            return newText;
        }

        public static string RemoveWords(string intoText, string fromThis, string toThis,bool caseSensitive, Func<string,string> condition = null)
        {
            if (intoText == null) return intoText;
            string text = intoText;
            if (!caseSensitive)
            {
                fromThis = fromThis.ToUpper();
                toThis = toThis.ToUpper();
                text = intoText.ToUpper();
            }
            List<Point> lp = WordsIndecesBetween(text, fromThis, toThis,true);
            for (int i = lp.Count - 1; i >= 0; i--)
                if (condition == null) intoText = string.Join("", intoText.Substring(0, lp[i].X) , intoText.Substring(lp[i].Y));
                else intoText = string.Join("",intoText.Substring(0, lp[i].X)
                     , condition(intoText.Substring(lp[i].X, lp[i].Y - lp[i].X))
                     , intoText.Substring(lp[i].Y));
            return intoText;
        }
        public static string RemoveWords(string intoText, string fromThis, string toThis, bool caseSensitive, Func<string,bool> condition = null)
        {
            if (intoText == null) return intoText;
            string text = intoText;
            if (!caseSensitive)
            {
                fromThis = fromThis.ToUpper();
                toThis = toThis.ToUpper();
                text = intoText.ToUpper();
            }
            List<Point> lp = WordsIndecesBetween(text, fromThis, toThis,true);
            foreach (var item in lp)
                if (condition == null) intoText = string.Join("", intoText.Substring(0, item.X) , intoText.Substring(item.Y));
                else if(condition(intoText.Substring(item.X, item.Y- item.X))) intoText = string.Join("", intoText.Substring(0, item.X) , intoText.Substring(item.Y));
            return intoText;
        }
        public static string RemoveHTMLTags(string html)
        {
            if (html == null) return html;
            List<string> ls = WordsBetween(html, "<", ">",true);
            return ReplaceWords(html, "", ls.ToArray());
        }

        public static string InsertPrefixAndSuffix(string intoText, string prefix, string Sufix,bool caseSensitive , params string[] thisTexts)
        {
            if (intoText == null) return intoText;
            string text = intoText;
            string[] words = thisTexts;
            if (!caseSensitive)
            {
                words = CollectionService.ExecuteInAllItems(thisTexts, (s) => { return s.ToUpper(); });
                text = intoText.ToUpper();
            }
            List<int> li = WordsIndexes(text, words);
            for (int i = li.Count - 1; i >= 0; i--)
            {
                for (int j = 0; j < thisTexts.Length; j++)
                    if (text.Substring(li[i]).StartsWith(words[j]))
                    {
                        intoText = string.Join(Sufix, intoText.Substring(0, li[i] + thisTexts[j].Length) , intoText.Substring(li[i] + thisTexts[j].Length));
                        break;
                    }
                intoText = string.Join(prefix, intoText.Substring(0, li[i] - 1) , intoText.Substring(li[i]));
            }
            return intoText;
        }
        public static string InsertPrefix(string intoText, string prefix,bool caseSensitive,params string[] thisTexts)
        {
            if (intoText == null) return intoText;
            string text = intoText;
            string[] words = thisTexts;
            if (!caseSensitive)
            {
                words = CollectionService.ExecuteInAllItems(thisTexts, (s) => { return s.ToUpper(); });
                text = intoText.ToUpper();
            }
            List<int> li = WordsIndexes(text, words);
            for (int i = li.Count - 1; i >= 0; i--)
                intoText = string.Join(prefix, intoText.Substring(0, li[i] - 1) , intoText.Substring(li[i]));
            return intoText;
        }
        public static string InsertSuffix(string intoText, string Sufix,bool caseSensitive,params string[] thisTexts)
        {
            if (intoText == null) return intoText;
            string text = intoText;
            string[] words = thisTexts;
            if (!caseSensitive)
            {
                words = CollectionService.ExecuteInAllItems(thisTexts, (s) => { return s.ToUpper(); });
                text = intoText.ToUpper();
            }
            List<int> li = WordsIndexes(text, words);
            for (int i = li.Count - 1; i >= 0; i--)
                for (int j = 0; j < thisTexts.Length; j++)
                    if (text.Substring(li[i]).StartsWith(words[j]))
                    {
                        intoText = string.Join(Sufix, intoText.Substring(0, li[i] + thisTexts[j].Length) , intoText.Substring(li[i] + thisTexts[j].Length));
                        break;
                    }
            return intoText;
        }

        public static bool ExistAny(string intoText,bool caseSensitive, params string[] thisTexts)
        {
            if (!caseSensitive)
            {
                thisTexts = CollectionService.ExecuteInAllItems(thisTexts, (s) => { return s.ToUpper(); });
                intoText = intoText.ToUpper();
            }
                foreach (var item in thisTexts)
                    if (intoText.Contains(item)) return true;
            return false;
        }
        public static bool ExistAll(string intoText,bool caseSensitive, params string[] thisTexts)
        {
            if (!caseSensitive)
            {
                thisTexts = CollectionService.ExecuteInAllItems(thisTexts, (s) => { return s.ToUpper(); });
                intoText = intoText.ToUpper();
            }
            foreach (var item in thisTexts)
                if (!intoText.Contains(item)) return false;
            return true;
        }

        public static string[] Split(string intoText, string pattern, RegexOptions option, int fromIndex = 0, int toindex = -1)
        {
            if (intoText == null) return new string[] { };
            if (toindex < 0) toindex = intoText.Length - 1;
            int length = Math.Min(intoText.Length, toindex + 1) - fromIndex;
            var res = Regex.Split(intoText.Substring(fromIndex,length), pattern, option);
            res[0] = intoText.Substring(0,fromIndex) + res[0];
            res[res.Length-1] = res[res.Length - 1] + ((toindex + 1 < intoText.Length) ? intoText.Substring(toindex+1):"");
            return res;
        }
        public static IEnumerable<string> Split(string text, params string[] splitors) => Split(text,splitors, new char[0]);
        public static IEnumerable<string> Split(string text, string[] splitors, char[] quoteChars = null, char? escapeChar = null)
        {
            if (text == null) yield break;
            if (splitors.Length < 1)
            {
                yield return text;
                yield break;
            }
            string cel = null;
            var ql = quoteChars.ToList();
            var endQF = escapeChar.HasValue ? @"(?<!(?<!\{1})\{1})\{0}$" : @"\{0}$";
            escapeChar = escapeChar?? '\\';
            int ind = -1;
            char? q = null;
            foreach (var item in text.Split(splitors, StringSplitOptions.None))
                if (q.HasValue)
                    if (Regex.IsMatch(item, string.Format(endQF, q, escapeChar)))
                    {
                        cel = string.Join(splitors.First(), cel, item);
                        yield return cel.Substring(1, cel.Length - 2);
                        cel = null;
                        q = null;
                    }
                    else cel = string.Join(splitors.First(), cel, item);
                else if ((ind = ql.FindIndex(c => item.FirstOrDefault()==c)) >= 0)
                    if (item.Length > 1 && Regex.IsMatch(item, string.Format(endQF, ql[ind], escapeChar)))
                        yield return item.Substring(1, item.Length-2);
                    else
                    {
                        cel = item;
                        q = ql[ind];
                    }
                else yield return item;

            if (cel != null)
                foreach (var item in cel.Split(splitors, StringSplitOptions.None))
                    yield return item;
        }
        public static IEnumerable<string> Split(string text, params char[] splitors) => Split(text, splitors, null, null);
        public static IEnumerable<string> Split(string text, char[] splitors, char[] quoteChars = null, char? escapeChar = null)
        {
            if (text == null) yield break;
            if (splitors.Length < 1)
            {
                yield return text;
                yield break;
            }
            string cel = null;
            var ql = quoteChars.ToList();
            var endQF = escapeChar.HasValue ? @"(?<!(?<!\{1})\{1})\{0}$" : @"\{0}$";
            escapeChar = escapeChar?? '\\';
            int ind = -1;
            char? q = null;
            var sp = splitors.First().ToString();
            foreach (var item in text.Split(splitors, StringSplitOptions.None))
                if (q.HasValue)
                    if (Regex.IsMatch(item, string.Format(endQF, q, escapeChar)))
                    {
                        cel = string.Join(sp, cel, item);
                        yield return cel.Substring(1, cel.Length - 2);
                        cel = null;
                        q = null;
                    }
                    else cel = string.Join(sp, cel, item);
                else if ((ind = ql.FindIndex(c => item.FirstOrDefault()==c)) >= 0)
                    if (item.Length > 1 && Regex.IsMatch(item, string.Format(endQF, ql[ind], escapeChar)))
                        yield return item.Substring(1, item.Length-2);
                    else
                    {
                        cel = item;
                        q = ql[ind];
                    }
                else yield return item;

            if (cel != null)
                foreach (var item in cel.Split(splitors, StringSplitOptions.None))
                    yield return item;
        }
        public static string[] FirstSplit(string intoText, string pattern, RegexOptions option, int fromIndex = 0, int toindex = -1)
        {
            if (intoText == null) return new string[] { };
            if (toindex < 0) toindex = intoText.Length - 1;
            int length = Math.Min(intoText.Length, toindex + 1) - fromIndex;
            var text = intoText.Substring(fromIndex, length);
            var res = Regex.Split(text, pattern, option);
            if (res.Length < 2) return new string[] { intoText };
            var scope = intoText.Substring(0, fromIndex) + res.First();
            return new string[] { scope, intoText.Substring(scope.Length + Regex.Match(text, pattern, option).Value.Length) };
        }
        public static string[] FirstSplit(string intoText, string splitor, int fromIndex = 0, int toindex = -1)
        {
            if (intoText == null) return new string[] { };
            string strsub;
            if (toindex < 0) toindex = intoText.Length;
            int length = Math.Min(intoText.Length, toindex+1);
            for (int i = fromIndex; i < length; i++)
                if ((strsub = intoText.Substring(i)).StartsWith(splitor))
                    return new string[] { intoText.Substring(0, i), strsub.Substring(splitor.Length) };
            return new string[] { intoText };
        }
        public static string[] LastSplit(string intoText, string pattern, RegexOptions option, int fromIndex = 0, int toindex = -1)
        {
            if (intoText == null) return new string[] { };
            if (toindex < 0) toindex = intoText.Length-1;
            int length = Math.Min(intoText.Length, toindex+1) - fromIndex;
            var text = intoText.Substring(fromIndex, length);
            var res = Regex.Split(text, pattern, option);
            if (res.Length < 2) return new string[] { intoText };
            var ml = ConvertService.ToEnumerable<Match>(Regex.Matches(text, pattern, option)).Last().Value.Length;
            var scope = text.Length - (ml + res.Last().Length);
            return new string[] { intoText.Substring(0, fromIndex)+ text.Substring(0, scope-ml), text.Substring(scope)+ ((toindex + 1 < intoText.Length)? intoText.Substring(toindex+1):"") };
        }
        public static string[] LastSplit(string intoText, string splitor, int fromIndex = 0, int toindex = -1)
        {
            if (intoText == null) return new string[] { };
            string strsub;
            if (toindex < 0) toindex = intoText.Length-1;
            int length = Math.Min(intoText.Length, toindex+1) -1;
            for (int i = length ; i >= fromIndex ; i--)
                if ((strsub = intoText.Substring(i)).StartsWith(splitor))
                    return new string[] { intoText.Substring(0, i), strsub.Substring(splitor.Length) };
            return new string[] { intoText };
        }
        public static string[] SplitToArguments(string arg, string splitter = ";")
        {
            return Regex.Split(arg, $"^|(?<=[^\\\\]){splitter}");
        }
        public static string JoinToArgument(string[] args, string splitter = ";")
        {
            return string.Join(splitter, from arg in args select arg.Replace(splitter, $"\\{splitter}"));
        }

        public static string FirstReplace(string intoText, string oldWords, string newWord, int fromIndex = 0)
        {
            if (intoText == null) return intoText;
            string newtext = intoText.Substring(0,fromIndex);
            int owi = oldWords.Length;
            for (int i = fromIndex; i < intoText.Length; i++)
                if (intoText.Substring(i).StartsWith(oldWords))
                {
                    newtext += newWord;
                    i += owi;
                    return newtext + intoText.Substring(i);
                }
                else newtext += intoText[i];
            return newtext;
        }
        public static string FirstReplace(string intoText, string oldWords, string newWord,out bool find, int fromIndex = 0)
        {
            string newtext = intoText.Substring(0,fromIndex);
            int owi = oldWords.Length;
            string strsub = "";
            for (int i = fromIndex; i < intoText.Length; i++)
                if ((strsub = intoText.Substring(i)).StartsWith(oldWords))
                {
                    newtext += newWord;
                    i += owi;
                    find = true;
                    return newtext + strsub;
                }
                else newtext += intoText[i];
            find = false;
            return newtext;
        }
        public static int FirstIndex(string intoText, params string[] thisTexts)
        {
            for (int i = 0; i < intoText.Length; i++)
                foreach (var item in thisTexts)
                    try
                    {
                        if (intoText.Substring(i).StartsWith(item)) return i;
                    }
                    catch { }
            return -1;
        }
        public static int WordsNumber(string text, params string[] words)
        {
            return text.Split(words, StringSplitOptions.None).Length - 1;
        }
        public static int WordNumber(string intoText, params string[] thisTexts)
        {
            int number = 0;
            for (int i = 0; i < intoText.Length; i++)
                foreach (var item in thisTexts)
                    if (intoText.Substring(i).StartsWith(item)) number++;
            return number;
        }
        public static int[] WordsNumbers(string intoText, params string[] thisTexts)
        {
            int[] numbers = new int[thisTexts.Length];
            for (int i = 0; i < numbers.Length; i++)
                numbers[i] = 0;
            for (int i = 0; i < intoText.Length; i++)
                for (int j = 0; j < thisTexts.Length; j++)
                    if (intoText.Substring(i).StartsWith(thisTexts[j])) numbers[j]++;
            return numbers;
        }
        public static Dictionary<string,int> ReferencedOWordsNumbers(string intoText, params string[] thisTexts)
        {
            Dictionary<string, int> dic = new Dictionary<string, int>();
            for (int i = 0; i < thisTexts.Length; i++)
                try { dic.Add(thisTexts[i],0); } catch { }
            for (int i = 0; i < intoText.Length; i++)
                foreach (var item in thisTexts)
                    if (intoText.Substring(i).StartsWith(item)) dic[item]++;
            return dic;
        }
        public static List<int> WordsIndexes(string intoText, params string[] thisTexts)
        {
            List<int> numbers = new List<int>();
            for (int i = 0; i < intoText.Length; i++)
                foreach (var item in thisTexts)
                    if (intoText.Substring(i).StartsWith(item)) numbers.Add(i);
            return numbers;
        }

    }
}
