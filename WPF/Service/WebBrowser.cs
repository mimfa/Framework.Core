using System;
using System.Collections.Generic;
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
    public class WebBrowserService
    {
        public readonly DependencyProperty HtmlProperty;

        public WebBrowserService()
        {
            HtmlProperty = DependencyProperty.RegisterAttached(
            Default.Time,
            typeof(string),
            typeof(WebBrowserService),
            new FrameworkPropertyMetadata(OnHTMLChanged));
        }

        [AttachedPropertyBrowsableForType(typeof(WebBrowser))]
        public string GetHTML( dynamic d)
        {
            return (string)d.GetValue(HtmlProperty);
        }
        public void SetHTML(dynamic d, string value)
        {
            d.SetValue(HtmlProperty, value);
        }
        public void Clear(dynamic d)
        {
            d.SetValue(HtmlProperty, "<html><body></body></html>");
        }

        private void OnHTMLChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            WebBrowser wb = d as WebBrowser;
            if (wb != null)
               if(string.IsNullOrEmpty(e.NewValue.ToString())) wb.NavigateToString("<!---->");
                else wb.NavigateToString(e.NewValue as string);
        }
    }
}
