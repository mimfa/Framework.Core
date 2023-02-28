using System;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using System.ComponentModel;
using System.Windows.Forms;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using MiMFa.General;

namespace MiMFa.Service
{
    public class WebService
    {
        public static void AddScriptFile(WebBrowser Wb, string SourceAddress)
        {
            AddScriptContent(Wb, ConvertService.ToTrimedString(SourceAddress));
        }
        public static void AddScriptContent(WebBrowser Wb, string ContentScript)
        {
            Wb.Document.InvokeScript("execScript", new Object[] { ContentScript, "JavaScript" });
        }
        public static void AddTagWithContentToHtmlElement(WebBrowser Wb, string ElementName, string TagName, string Content = "")
        {
            HtmlElement Elmnt = Wb.Document.GetElementsByTagName(ElementName)[0];
            HtmlElement Tag = Wb.Document.CreateElement(TagName);
            Tag.InnerText = Content;
            Elmnt.AppendChild(Tag);
        }
        public static void AddTagToHtmlElement(WebBrowser Wb, string ElementName, string TagName, string AttributeName, string AttributeValue)
        {
            //Wb.Document.CreateElement(ElementName);
            //HtmlElement Elmnt = Wb.Document.GetElementsByTagName(ElementName)[0];
            //HtmlElement Tag = Wb.Document.CreateElement(TagName);
            //Tag.SetAttribute(AttributeName, AttributeValue);
            //Elmnt.AppendChild(Tag);
        }
    }
}
