using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;

namespace MiMFa.WPF.Service
{
    public class RichTextBoxService
    {
        public void SetText(ref RichTextBox rtb,string text)
        {
            TextRange textRange = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);
            textRange.Text = text;
        }
        public string GetText(ref RichTextBox rtb)
        {
           return (new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd)).Text;
        }
        public void Clear(ref RichTextBox rtb)
        {
            TextRange textRange = new TextRange(rtb.Document.ContentStart, rtb.Document.ContentEnd);
            textRange.Text = "";
        }
        public void ExecuteInCurrentText(ref RichTextBox rtb,Func<string,string> func)
        {
            TextPointer start = rtb.CaretPosition;
            string s = start.GetTextInRun(LogicalDirection.Backward);
            TextPointer end = start.GetNextContextPosition(LogicalDirection.Forward);
            start.InsertTextInRun(func(s.Split(new char[] {
                                ' ','~','!','@','#',
                                 '$','%','^','&',
                                 '*','(',')','_',
                                 '-','+','=','/',
                                 '\\','[',']','{',
                                 '}',';',':',',',
                                 '.','?','<','>' }, StringSplitOptions.None).Last()));
            rtb.Selection.Select(start, end);
        }
        public void SetInCurrentText(ref RichTextBox rtb,string text)
        {
            SetText(ref rtb, text);
        }
    }
}
