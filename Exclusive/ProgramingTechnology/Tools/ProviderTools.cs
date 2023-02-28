using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Drawing;
using MiMFa.Service;
using MiMFa.General;
using MiMFa.Exclusive.DateAndTime;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using MiMFa.Exclusive.ProgramingTechnology.Tools.Pickup;

namespace MiMFa.Exclusive.ProgramingTechnology.Tools
{
    public class ProviderTools
    {
        #region Sign
        public static string StartSignStrong => "$";
        public static string EndSignStrong => "$";
        public static char SplitSign { get; } = ',';
        public static string StartSignCollection { get; } = "{";
        public static string EndSignCollection { get; } = "}";
        public static string StartSignParenthesis { get; } = "(";
        public static string EndSignParenthesis { get; } = ")";
        public static string StartSignStrongCollection { get; } = StartSignStrong + StartSignCollection;
        public static string EndSignStrongCollection { get; } = EndSignCollection + EndSignStrong;
        public static string StartSignStrongParenthesis { get; } = StartSignStrong + StartSignParenthesis;
        public static string EndSignStrongParenthesis { get; } = EndSignParenthesis + EndSignStrong;
        public static string StartSignString { get; } = '"'.ToString();
        public static string EndSignString { get; } = '"'.ToString();
        public static string StartSignStrongString { get; } = StartSignStrong + StartSignString;
        public static string EndSignStrongString { get; } = EndSignString + EndSignStrong;
        public static string StartSignComment { get; } = "/<";
        public static string EndSignComment { get; } = ">/";
        #endregion

        #region Tools
        public static StrongParenthesisPickup StrongParenthesisPU { get; } = new StrongParenthesisPickup();
        public static ParenthesisPickup ParenthesisPU { get; } = new ParenthesisPickup();
        public static StrongCollectionPickup StrongCollectionPU { get; } = new StrongCollectionPickup();
        public static CollectionPickup CollectionPU { get; } = new CollectionPickup();
        public static StrongStringPickup StrongStringPU { get; } = new StrongStringPickup();
        public static StringPickup StringPU { get; } = new StringPickup();
        public static CommentProvider CommentPU { get; } = new CommentProvider();
        #endregion

        public static string Normalization(string code, bool withpickup = false)
        {
            if (code == null) return null;
            code = code.Replace('\t', ' ')
                .Replace('\n', ' ')
                .Replace('\r', ' ');
            if (withpickup)
            {
                code = CommentPU.Replace(code, "");
                code = StrongStringPU.RePick(code);
                code = StringPU.RePick(code);
                code = StrongParenthesisPU.RePick(code);
                code = ParenthesisPU.RePick(code);
                code = StrongCollectionPU.RePick(code);
                code = CollectionPU.RePick(code);
            }
            return code;
        }

    }
}
