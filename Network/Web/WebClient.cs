using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace MiMFa.Network.Web
{
    public class WebClient : System.Net.WebClient
    {
        public CookieContainer CookieContainer { get; set; }

        public WebClient() : this(null, null)
        {
        }
        public WebClient(CookieContainer container) : this(container, null)
        {
        }
        public WebClient(string contentType) : this(null, contentType)
        {
        }
        public WebClient(CookieContainer container,string contentType)
        {
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };
            CookieContainer = container ?? new CookieContainer();
            if(contentType ==null) UpdateHeaders();
            else UpdateHeaders(contentType);
        }


        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest r = base.GetWebRequest(address);
            var request = r as HttpWebRequest;
            //request.ContinueTimeout = 5000;
            if (request != null) 
            {
                request.CookieContainer = CookieContainer;
                request.KeepAlive = true;
                request.UserAgent = "Mozilla/5.0 (Windows NT 6.1; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/80.0.3987.149 Safari/537.36";
                request.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,image/apng,*/*;q=0.8,application/signed-exchange;v=b3;q=0.9";
                request.ContentType = "application/x-www-form-urlencoded";
            }
            return r;
        }

        protected override WebResponse GetWebResponse(WebRequest request, IAsyncResult result)
        {
            WebResponse response = base.GetWebResponse(request, result);
            ReadCookies(response);
            return response;
        }
        protected override WebResponse GetWebResponse(WebRequest request)
        {
            WebResponse response = base.GetWebResponse(request);
            ReadCookies(response);
            return response;
        }

        private void ReadCookies(WebResponse r)
        {
            var response = r as HttpWebResponse;
            if (response != null) CookieContainer.Add(response.Cookies);
        }


        public virtual void UpdateHeaders(string contentType = "application/x-www-form-urlencoded")
        {
            this.Headers.Set(System.Net.HttpRequestHeader.UserAgent, "Mozilla/5.0 (Windows NT 10.0; Trident/7.0; rv:11.0)");
            this.Headers.Set(System.Net.HttpRequestHeader.ContentType, contentType);
            try
            {
                this.Headers.Add("Cache-Control", "no-cache");
                this.Headers.Add("Pragma", "no-cache");
                this.Headers.Add("Sec-Fetch-Dest", "empty");
                this.Headers.Add("Sec-Fetch-Mode", "cors");
                this.Headers.Add("Sec-Fetch-Site", "same-origin");
                this.Headers.Add("X-Requested-With", "XMLHttpRequest");
            }
            catch { }
        }

        public virtual string PostWebRequest(string url,Dictionary<string,string> requests)
        {
            return PostWebRequest(url, (from v in requests
                                        select System.Web.HttpUtility.UrlEncode(v.Key) +
                                        "=" +
                                        System.Web.HttpUtility.UrlEncode(v.Value)).ToArray());
        }
        public virtual string PostWebRequest(string url, params string[] parameters)
        {
            return PostWebRequest(url, string.Join("&", parameters));
        }
        public virtual string PostWebRequest(string url, string parameters)
        {
            Headers[System.Net.HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
            return System.Web.HttpUtility.UrlDecode(UploadString(url, parameters));
        }
    }
}
