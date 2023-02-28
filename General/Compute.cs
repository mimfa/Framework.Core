using System;
using System.Data;
using MiMFa.Service;
using System.Collections.Generic;

namespace MiMFa.General
{
    public class Compute
    {
        public static double EvaluateString(string expr)
        {
               expr = "0+" + expr.Replace(" ","");
            while (expr.Contains("+-") || expr.Contains("-+") || expr.Contains("++") || expr.Contains("--"))
                expr = expr.Replace("()", "0").Replace(")(", ")*(").Replace("+-", "-").Replace("-+", "-").Replace("++", "+").Replace("--", "+");
            string[] sa = StringService.WordsBetween(expr, "(", ")", true).ToArray();
            if (sa.Length > 0)
            {
                string[] sanew = new string[sa.Length];
                for (int i = 0; i < sa.Length; i++)
                    sanew[i] = EvaluateString(sa[i].Replace("(", "").Replace(")", "")).ToString();
                expr = StringService.ReplaceWordsByOrder(expr, sa, sanew);
            }
            expr = expr.Trim();
            expr = LikeLapSignE(expr,"√");
            expr = LikeLapSignE(expr, "^");
            expr = LikeLapSignE(expr,"%");
            expr = LikeLapSignE(expr, "×", "*", "÷", "\\");
            expr = LikeLapSignE(expr, "+", "-");
            return ConvertService.ForceToDouble(expr);
        }
        private static string LikeLapSignE(string expr,params string[] signs)
        {
            while (expr.Contains("+-") || expr.Contains("-+") || expr.Contains("++") || expr.Contains("--"))
                expr = expr.Replace("()", "0").Replace(")(", ")*(").Replace("+-", "-").Replace("-+", "-").Replace("++", "+").Replace("--", "+");
            string[] ca1 = {  "√","^", "%", "×", "*", "÷", "\\", "+", "-" };
            Func<string, string> func = (sign) =>
                 {
                     string[] sa = expr.Split(new string[] { sign }, StringSplitOptions.None);
                     if (sa.Length <= 1) return expr;
                     string[] arr = sa[0].Split(ca1, StringSplitOptions.None);
                     string op1 = arr[arr.Length - 1];
                     string op2 = sa[1];
                     double dop2 = 0;
                     if (op2.StartsWith("+"))
                     {
                         arr = sa[1].Split(ca1, StringSplitOptions.None);
                         op2 = "+" + arr[1];
                         dop2 = Convert.ToDouble(arr[1]);
                     }
                     else if (op2.StartsWith("-"))
                     {
                         arr = sa[1].Split(ca1, StringSplitOptions.None);
                         op2 = "-" + arr[1];
                         dop2 = -1 * Convert.ToDouble(arr[1]);
                     }
                     else
                     {
                         arr = sa[1].Split(ca1, StringSplitOptions.None);
                         op2 = arr[0];
                         dop2 = Convert.ToDouble(arr[0]);
                     }
                     string result = EvaluateString(op1, sign, dop2).ToString();
                     expr = expr.Replace(op1 + sign + op2, ConvertService.ForceToDouble(result).ToString());
                     return expr;
                 };
            if (expr.Split(signs, StringSplitOptions.None).Length > 1)
                do
                {
                    Dictionary<int, string> collection = new Dictionary<int, string>();
                    for (int i = 0; i < expr.Length; i++)
                        foreach (var item in signs)
                            if (expr[i].ToString() == item) { collection.Add(i, item); break; }
                    collection = CollectionService.Sort(collection, (d1, d2) => d1.Key < d2.Key);
                    foreach (var item in collection)
                        expr = func(item.Value);
                }
                while (expr.Split(signs, StringSplitOptions.RemoveEmptyEntries).Length > 1);
            return expr;
        }
        public static dynamic EvaluateString(object op1, string sign, object op2)
        {
            Func<object, double> normal = (o) =>
            {
                if (!string.IsNullOrEmpty(o + "")) return Convert.ToDouble(o);
                return 1;
            };

            double d1 = ConvertService.ForceToDouble(normal(op1));
            double d2 = ConvertService.ForceToDouble(normal(op2));
            switch (sign)
            {
                case "^":
                    return Math.Pow(d1, d2);
                case "√":
                    return d1 * Math.Sqrt(d2);
                case "%":
                    return d1 % d2;
                case "×":
                    return d1 * d2;
                case "*":
                    return d1 * d2;
                case "÷":
                    return d1 / d2;
                case "\\":
                    return d1 / d2;
                case "+":
                    return d1 + d2;
                case "-":
                    return d1 - d2;
            }
            DataTable dt = new DataTable();
            return ConvertService.ForceToDouble(dt.Compute(op1 + sign + op2, ""));
        }

        public static bool CompareString(string expr)
        {
            expr =  expr.Replace(" ", "");
            expr = expr.Replace("()", "true").Replace(")(", ")&&(");
            string[] sa = StringService.WordsBetween(expr, "(", ")", true).ToArray();
            if (sa.Length > 0)
            {
                string[] sanew = new string[sa.Length];
                for (int i = 0; i < sa.Length; i++)
                    sanew[i] = CompareString(sa[i].Replace("(","").Replace(")","")).ToString();
                expr = StringService.ReplaceWordsByOrder(expr, sa, sanew);
            }
            expr = expr.Trim();
            expr = LikeLapSignC(expr, "<=", ">=", "<", ">");
            expr = LikeLapSignC(expr,"!==", "===", "~==", "~=", "!=",  "==");
            expr = LikeLapSignC(expr, "&&", "||");
            return ConvertService.ToBoolean(expr);
        }
        private static string LikeLapSignC(string expr,params string[] signs)
        {
            string[] ca = { "&&","||","<=", ">=","<", ">", "!==", "!=", "~==", "~=", "===","=="};
            Func<string, string> func = (sign) =>
                 {
                     string[] sa = expr.Split(new string[] { sign }, StringSplitOptions.None);
                     if (sa.Length <= 1) return expr;
                     string[] arr = sa[0].Split(ca, StringSplitOptions.None);
                     string op1 = arr[arr.Length - 1];
                     arr = sa[1].Split(ca, StringSplitOptions.None);
                     string op2 = arr[0];
                     string result = CompareString(op1, sign, op2).ToString();
                     expr = expr.Replace(op1 + sign + op2, result);
                     return expr;
                 };
            if (expr.Split(signs, StringSplitOptions.None).Length > 1)
                do
                {
                    Dictionary<int, string> collection = new Dictionary<int, string>();
                    for (int i = 0; i < expr.Length; i++)
                        foreach (var item in signs)
                            if (expr.Substring(i).StartsWith( item)) { collection.Add(i, item); break; }
                    collection = CollectionService.Sort(collection, (d1, d2) => d1.Key < d2.Key);
                    foreach (var item in collection)
                        expr = func(item.Value);
                }
                while (expr.Split(signs, StringSplitOptions.RemoveEmptyEntries).Length > 1);
            return expr;
        }
        public static bool CompareString(object op1, string sign, object op2)
        {
            try
            {
                Func<object, object> normal = (o) =>
                {
                    if (!string.IsNullOrEmpty(o.ToString())) return o;
                    return true;
                };
                op1 = normal(op1);
                op2 = normal(op2);
                double d1;
                double d2;
                bool b1;
                bool b2;
                switch (sign)
                {
                    case "===": return op1.Equals(op2);
                    case "!==": return !op1.Equals(op2);
                    case "==":
                        return op1.ToString() == op2.ToString();
                    case "!=":
                        return op1.ToString() != op2.ToString();
                    case "~==":
                        return op1.GetType() == op2.GetType() && StringService.ComparePerCent(op1.ToString() , op2.ToString()) > 70;
                    case "~=":
                        return StringService.ComparePerCent(op1.ToString() , op2.ToString()) > 70;
                    case ">":
                        d1 = ConvertService.ForceToDouble(op1);
                        d2 = ConvertService.ForceToDouble(op2);
                        return d1 > d2;
                    case "<":
                        d1 = ConvertService.ForceToDouble(op1);
                        d2 = ConvertService.ForceToDouble(op2);
                        return d1 < d2;
                    case ">=":
                        d1 = ConvertService.ForceToDouble(op1);
                        d2 = ConvertService.ForceToDouble(op2);
                        return d1 >= d2;
                    case "<=":
                        d1 = ConvertService.ForceToDouble(op1);
                        d2 = ConvertService.ForceToDouble(op2);
                        return d1 <= d2;
                    case "&&":
                        b1 = ConvertService.ToBoolean(op1);
                        b2 = ConvertService.ToBoolean(op2);
                        return b1 && b2;
                    case "||":
                        b1 = ConvertService.ToBoolean(op1);
                        b2 = ConvertService.ToBoolean(op2);
                        return b1 || b2;
                    case "is": return op1.GetType() == op2.GetType();
                }
                DataTable dt = new DataTable();
                return Convert.ToBoolean(dt.Compute(op1 + sign + op2, "")); ;
            }
            catch { throw new Exception("Invalid character."); }
        }
    }
}
