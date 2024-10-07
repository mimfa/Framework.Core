using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Net.Mail;

namespace MiMFa.Service
{
    public static class InternetService
    {
        public static Uri CreateUri(string baseUrl, string url) => CreateUri(new Uri(baseUrl), url);
        public static Uri CreateUri(Uri baseUrl, string url) => new Uri(baseUrl, url);
        public static Uri CreateUri(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return null;
            string ss = url.ToLower();
            if (ss.StartsWith("http://") || ss.StartsWith("https://"))
                return new Uri(url);
            if (url.Contains("\\")) return new Uri(url);
            return new Uri("http://" + url);
        }

        /// <summary>
        /// This method will check a url to see that it does not return server or protocol errors
        /// </summary>
        /// <param name="url">The path to check</param>
        /// <returns></returns>
        public static bool IsWellURL(string url) => Uri.IsWellFormedUriString(url, UriKind.Absolute);
        public static string GetBaseWebURL(string url)
        {
            string[] sa = url.Split('/');
            url = sa.First();
            int len = Math.Min(sa.Length, 3);
            for (int i = 1; i < len; i++)
                url += "/" + sa[i];
            return url;
        }
        public static bool ValidateURL(string url)
        {
            if (!IsWellURL(url)) return false;
            try
            {
                HttpWebRequest request = HttpWebRequest.Create(url) as HttpWebRequest;
                request.Timeout = 5000; //set the timeout to 5 seconds to keep the user from waiting too long for the page to load
                request.Method = "HEAD"; //Get only the header information -- no need to download any content
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                int statusCode = (int)response.StatusCode;
                if (statusCode >= 100 && statusCode < 400) //Good requests
                    return true;
                else if (statusCode >= 500 && statusCode <= 510) //Server Errors
                    return false;
            }
            catch
            {
                return false;
            }
            return false;
        }

        public static HtmlDocument Navigate(string url,CookieContainer cookie, Encoding encoding = null)=>Navigate(new Uri(url),cookie, encoding);
        public static HtmlDocument Navigate(Uri url,CookieContainer cookie, Encoding encoding = null) => cookie==null|| cookie.Count ==0?Navigate(url,false,1,encoding) : ConvertService.ToHtmlDocument(Download(url,cookie, encoding));
        public static HtmlDocument Navigate(string url, int timeout, int delay = 350, CookieContainer cookie = null, int tryNum = 1, Encoding encoding = null) => cookie == null || cookie.Count == 0 ? Navigate(url, true, tryNum, encoding) : ConvertService.ToHtmlDocument(Download(new Uri(url), timeout, delay, cookie, tryNum, encoding));
        public static HtmlDocument Navigate(Uri url, int timeout, int delay = 350, CookieContainer cookie = null, int tryNum = 1, Encoding encoding = null) => cookie == null || cookie.Count == 0 ? Navigate(url, true, tryNum, encoding) : ConvertService.ToHtmlDocument(Download(url, timeout, delay, cookie, tryNum, encoding));
        public static HtmlDocument Navigate(string url, bool useCookie, int tryNum, Encoding encoding = null, Func<string, bool> isBrowserScriptCompleted = null) => Navigate(new Uri(url),  useCookie, default, default, tryNum, encoding, isBrowserScriptCompleted);
        public static HtmlDocument Navigate(Uri url, bool useCookie, int tryNum, Encoding encoding = null, Func<string, bool> isBrowserScriptCompleted = null) => Navigate(url,  useCookie, default, default, tryNum, encoding, isBrowserScriptCompleted);
        public static HtmlDocument Navigate(string url, bool useCookie = true, TimeSpan timeout = default, TimeSpan delay = default, int tryNum = 1, Encoding encoding = null, Func<string, bool> isBrowserScriptCompleted = null) => Navigate(new Uri(url),  useCookie, timeout, delay, tryNum, encoding, isBrowserScriptCompleted);
        public static HtmlDocument Navigate(Uri url, bool useCookie = true, TimeSpan timeout = default, TimeSpan delay = default, int tryNum = 1, Encoding encoding = null, Func<string,bool> isBrowserScriptCompleted = null)
        {
            var web = new HtmlWeb();
            web.OverrideEncoding = encoding??Encoding.Default;
            if (timeout != default && timeout > web.BrowserTimeout) web.BrowserTimeout = timeout;
            if(delay != default && delay > web.BrowserDelay) web.BrowserDelay = delay;
            web.UseCookies = useCookie;
            HtmlDocument doc = null;
            while (tryNum-- > 0 && (doc == null || doc.DocumentNode == null || doc.DocumentNode.ChildNodes.Count < 1))
                if (isBrowserScriptCompleted == null)
                    try { doc = web.Load(url); } catch { }
                else try { doc = web.LoadFromBrowser(url.ToString(), isBrowserScriptCompleted); } catch { }
            return doc;
        }

        public static async Task<HtmlDocument> NavigateAsync(string url, int timeout, int delay = 350, CookieContainer cookie = null, Encoding encoding = null) => await NavigateAsync(new Uri(url), timeout, delay, cookie, encoding);
        public static async Task<HtmlDocument> NavigateAsync(Uri url, int timeout, int delay = 350, CookieContainer cookie = null, Encoding encoding = null) => cookie == null || cookie.Count == 0 ? await NavigateAsync(url, true, encoding) : ConvertService.ToHtmlDocument(await DownloadAsync(url, timeout, delay, cookie, encoding));
        public static async Task<HtmlDocument> NavigateAsync(string url, Encoding encoding) => await NavigateAsync(new Uri(url), encoding);
        public static async Task<HtmlDocument> NavigateAsync(Uri url, Encoding encoding) => ConvertService.ToHtmlDocument(await DownloadAsync(url, encoding));
        public static async Task<HtmlDocument> NavigateAsync(Uri url, bool useCookie, Encoding encoding) => await NavigateAsync(url.ToString(), useCookie, default, default, encoding);
        public static async Task<HtmlDocument> NavigateAsync(string url, bool useCookie, Encoding encoding) => await NavigateAsync(url, useCookie, default, default, encoding);
        public static async Task<HtmlDocument> NavigateAsync(Uri url, bool useCookie = true, TimeSpan timeout = default, TimeSpan delay = default, Encoding encoding = null) => await NavigateAsync(url.ToString(), useCookie, timeout, delay, encoding);
        public static async Task<HtmlDocument> NavigateAsync(string url, bool useCookie = true, TimeSpan timeout = default, TimeSpan delay = default, Encoding encoding = null)
        {
            var web = new HtmlWeb();
            web.OverrideEncoding = encoding ?? Encoding.Default;
            if (timeout != default) web.BrowserTimeout = timeout;
            if (delay != default) web.BrowserDelay = delay;
            web.UseCookies = useCookie;
            return await web.LoadFromWebAsync(url, web.OverrideEncoding);
        }

        public static string Download(string url, CookieContainer cookie=null, Encoding encoding = null)
        {
            return Download(new Uri(url), cookie, encoding);
        }
        public static string Download(Uri url, CookieContainer cookie = null, Encoding encoding = null)
        {
            if (url.IsAbsoluteUri)
                try
                {
                    Network.Web.WebClient myWebClient = new Network.Web.WebClient(cookie);
                    //myWebClient.Headers.Add("Accept", "application/json");
                    myWebClient.Encoding = encoding ?? Encoding.UTF8;
                    return myWebClient.DownloadString(url);
                }
                catch{ }
            return null;
        }
        public static bool Download(string url, string fileAddress, CookieContainer cookie = null, Encoding encoding = null)
        {
            return Download(new Uri(url), fileAddress, cookie, encoding);
        }
        public static bool Download(Uri url, string fileAddress, CookieContainer cookie = null, Encoding encoding = null)
        {
            if (url.IsAbsoluteUri)
                try
                {
                    Network.Web.WebClient myWebClient = new Network.Web.WebClient(cookie);
                    myWebClient.Encoding = encoding?? Encoding.UTF8;
                    myWebClient.DownloadFile(url, fileAddress);
                    return true;
                }
                catch { }
            return false;
        }
        public static string Download(string url, int timeout, int delay = 350, CookieContainer cookie = null, int retryNum = 1, Encoding encoding = null) => Download(new Uri(url), timeout, delay, cookie, retryNum, encoding);
        public static string Download(Uri url, int timeout, int delay = 350, CookieContainer cookie = null, int retryNum = 1, Encoding encoding = null)
        {
            string str = null;
            if (url.IsAbsoluteUri)
                try
                {
                    Network.Web.WebClient myWebClient = new Network.Web.WebClient(cookie);
                    myWebClient.Encoding = encoding ?? Encoding.UTF8;
                    str = myWebClient.DownloadString(url);
                }
                catch { }
            Thread.Sleep(delay);
            return str;
        }
        public static async Task<string> DownloadAsync(Uri url, int timeout, int delay = 350, CookieContainer cookie = null, Encoding encoding = null)
        {
            if (url.IsAbsoluteUri)
                try
                {
                    HttpWebRequest request = HttpWebRequest.Create(url) as HttpWebRequest;
                    request.CookieContainer = cookie ?? GetCookieContainer(url);
                    if (timeout > 100) request.Timeout = timeout;
                    if(delay > 350) request.ContinueTimeout = delay;
                    request.AllowAutoRedirect = true;
                    HttpWebResponse response = await request.GetResponseAsync() as HttpWebResponse;
                    int statusCode = (int)response.StatusCode;

                    StreamReader reader = encoding == null ? new StreamReader(response.GetResponseStream(), true) : new StreamReader(response.GetResponseStream(), encoding);

                    if (statusCode >= 100 && statusCode < 400) //Good requests
                        return reader.ReadToEnd();
                }
                catch { }
            return null;
        }
        public static async Task<string> DownloadAsync(Uri url, Encoding encoding = null)
        {
            if (url.IsAbsoluteUri)
            {
                CancellationTokenSource cancellationToken = new CancellationTokenSource();
                HttpClient httpClient = new HttpClient();
                HttpResponseMessage request = await httpClient.GetAsync(url,HttpCompletionOption.ResponseContentRead);
                int statusCode = (int)request.StatusCode;
                cancellationToken.Token.ThrowIfCancellationRequested();

                Stream response = await request.Content.ReadAsStreamAsync();

                cancellationToken.Token.ThrowIfCancellationRequested();
                StreamReader reader = encoding ==null?new StreamReader(response, true): new StreamReader(response, encoding);
                if (statusCode >= 100 && statusCode < 400) //Good requests
                    return reader.ReadToEnd();
            }
            return null;
        }

        public static bool DownloadOrSave(string urlOrText,string fileAddress,string ifTextFormat = ".txt")
        {
            if (Download(urlOrText, fileAddress)) return true;
            IOService.AppendText(fileAddress + ifTextFormat, urlOrText );
            return false;
        }

        public static string ConcatRequests(IEnumerable<KeyValuePair<string, string>> namevaluecollection, bool convertKeys = false, bool convertValues = false)
        {
            if(convertKeys && convertValues) return string.Join("&",from v in namevaluecollection select string.Join("=",ConvertService.ToURLCharacters(v.Key), ConvertService.ToURLCharacters(v.Value)));
            if (convertKeys) return string.Join("&",from v in namevaluecollection select string.Join("=", ConvertService.ToURLCharacters(v.Key), v.Value));
            if (convertValues) return string.Join("&",from v in namevaluecollection select string.Join("=",v.Key, ConvertService.ToURLCharacters(v.Value)));
            return string.Join("&",from v in namevaluecollection select string.Join("=",v.Key,v.Value));
        }
        public static string ConcatRequests(Dictionary<string, object> namevaluecollection, bool convertKeys = false, bool convertValues = false)
        {
            if(convertKeys && convertValues) return string.Join("&",from v in namevaluecollection select string.Join("=",ConvertService.ToURLCharacters(v.Key), ConvertService.ToURLCharacters(v.Value+"")));
            if (convertKeys) return string.Join("&",from v in namevaluecollection select string.Join("=", ConvertService.ToURLCharacters(v.Key), v.Value));
            if (convertValues) return string.Join("&",from v in namevaluecollection select string.Join("=",v.Key, ConvertService.ToURLCharacters(v.Value+"")));
            return string.Join("&",from v in namevaluecollection select string.Join("=",v.Key,v.Value));
        }
        public static string AJAX(string url, IEnumerable<KeyValuePair<string, string>> namevaluecollection, string method = "Get")
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ReadWriteTimeout = 100000;
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Accept = "*/*";
            httpWebRequest.Method = method;
            foreach (var item in namevaluecollection)
                httpWebRequest.Headers.Add(item.Key, item.Value);

            var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();

            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                return streamReader.ReadToEnd();
            }
        }
        public static string GET(string url, IEnumerable<KeyValuePair<string, string>> namevaluecollection)
        {
            return AJAX(url, namevaluecollection, "GET");
        }
        public static string POST(string url, IEnumerable<KeyValuePair<string, string>> namevaluecollection)
        {
            return AJAX(url, namevaluecollection,"POST");
        }
        public static void SendEmail(string userName, string password, string mailServer, string from, string to, string message, string subject = null, bool isHTML = true, params string[] cc)
            => SendEmail(userName, password, mailServer, new MailAddress(from), new MailAddress(to), message, subject, isHTML, (from c in cc select new MailAddress(c)).ToList());
        public static void SendEmail(string userName, string password, string mailServer, MailAddress from, MailAddress to, string message, string subject = null, bool isHTML = true, List<MailAddress> cc = null, List<MailAddress> bcc = null)
        {
            SmtpClient mailClient = new SmtpClient(mailServer);
            MailMessage mailMessage = new MailMessage();
            mailMessage.From = from;
            mailMessage.To.Add(to);
            if (cc != null) foreach (MailAddress addr in cc) mailMessage.CC.Add(addr);
            if (bcc != null) foreach (MailAddress addr in bcc) mailMessage.Bcc.Add(addr);
            mailMessage.Subject = subject;
            mailMessage.Body = message;
            mailMessage.IsBodyHtml = isHTML;
            mailClient.EnableSsl = true;
            SendEmail(new NetworkCredential(userName, password), mailClient, mailMessage);
            mailMessage.Dispose();
        }
        public static void SendEmail(NetworkCredential credentials, SmtpClient mailClient, MailMessage message)
        {
            mailClient.Credentials = credentials;
            SendEmail(mailClient, message);
        }
        public static void SendEmail(SmtpClient mailClient, MailMessage message)
        {
            mailClient.Send(message);
        }

        /// <summary>
        /// Gets the URI cookie container.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <returns></returns>
        public static CookieContainer GetCookieContainer(params Uri[] uris)
        {
            CookieContainer cookies = new CookieContainer();
            // Determine the size of the cookie
            foreach (var uri in uris)
            {
                string url = uri.ToString();
                int datasize = 8192 * 16;
                StringBuilder cookieData = new StringBuilder(datasize);
                if (!InternetGetCookieEx(url, null, cookieData, ref datasize, InternetCookieHttponly, IntPtr.Zero))
                {
                    if (datasize < 0)
                        return null;
                    // Allocate stringbuilder large enough to hold the cookie
                    cookieData = new StringBuilder(datasize);
                    if (!InternetGetCookieEx(
                        url,
                        null, cookieData,
                        ref datasize,
                        InternetCookieHttponly,
                        IntPtr.Zero))
                        return null;
                }
                if (cookieData.Length > 0)
                    cookies.SetCookies(uri, cookieData.ToString().Replace(';', ','));
            }
            return cookies;
        }
        [DllImport("wininet.dll", SetLastError = true)]
        public static extern bool InternetGetCookieEx(
        string url,
        string cookieName,
        StringBuilder cookieData,
        ref int size,
        Int32 dwFlags,
        IntPtr lpReserved);

        public static void SetCookieContainer(CookieContainer cookies,params Uri[] uris)
        {
            if(cookies != null)
            foreach (var uri in uris)
                foreach (Cookie item in cookies.GetCookies(uri))
                    InternetSetCookieEx(
                        uri.ToString(),
                        item.Name,
                        item.Value,
                        InternetCookieHttponly,
                        IntPtr.Zero);
        }
        [DllImport("wininet.dll", SetLastError = true)]
        static extern bool InternetSetCookieEx(
            string url,
            string cookieName,
            string cookieData,
            uint dwFlags,
            IntPtr dwReserved);
        private const Int32 InternetCookieHttponly = 0x2000;



        public static long DeleteUrlCache(string lpszUrlName) => DeleteUrlCacheEntry(lpszUrlName);
        [DllImport("wininet.dll")]
        private static extern long DeleteUrlCacheEntry(string lpszUrlName);


        private static bool DeleteSessions() => InternetSetOption(IntPtr.Zero, 42, IntPtr.Zero, 0);
        [DllImport("wininet.dll", SetLastError = true)]
        private static extern bool InternetSetOption(IntPtr hInternet, int dwOption, IntPtr lpBuffer, int lpdwBufferLength);


        public static void DeleteAllInternetTemporaryFiles()=> StartSilentProcess( "InetCpl.cpl,ClearMyTracksByProcess 8");
        public static void DeleteAllInternetCookies() => StartSilentProcess( "InetCpl.cpl,ClearMyTracksByProcess 2");
        public static void DeleteAllInternetHistory() => StartSilentProcess( "InetCpl.cpl,ClearMyTracksByProcess 1");
        public static void DeleteAllInternetForm() => StartSilentProcess( "InetCpl.cpl,ClearMyTracksByProcess 16");
        public static void DeleteAllInternetPasswords() => StartSilentProcess( "InetCpl.cpl,ClearMyTracksByProcess 32");
        public static void DeleteAllInternetStores() =>  StartSilentProcess( "InetCpl.cpl,ClearMyTracksByProcess 255");
        public static void DeleteAllInternetAddOns() => StartSilentProcess( "InetCpl.cpl,ClearMyTracksByProcess 4351");

        private static void StartSilentProcess(string argumant)
        {
            string args = "";
            args = (argumant);
            System.Diagnostics.Process process = null;
            System.Diagnostics.ProcessStartInfo processStartInfo;
            processStartInfo = new System.Diagnostics.ProcessStartInfo();
            processStartInfo.FileName = Environment.GetFolderPath(Environment.SpecialFolder.System) + "\\Rundll32.exe";
            if ((System.Environment.OSVersion.Version.Major >= 6))
                //  Windows Vista or higher
                processStartInfo.Verb = "runas";
            processStartInfo.Arguments = args;
            processStartInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
            processStartInfo.UseShellExecute = false;
            try
            {
                process = System.Diagnostics.Process.Start(processStartInfo);
            }
            catch
            {
            }
            finally
            {
                if (!(process == null))
                    process.Dispose();
            }
        }


        /// <summary>
        /// Clears the cache of the web browser
        /// </summary>
        public static void ClearInternetCache()
        {
            // Indicates that all of the cache groups in the user's system should be enumerated
            const int CACHEGROUP_SEARCH_ALL = 0x0;
            // Indicates that all the cache entries that are associated with the cache group
            // should be deleted, unless the entry belongs to another cache group.
            const int CACHEGROUP_FLAG_FLUSHURL_ONDELETE = 0x2;
            // File not found.
            const int ERROR_FILE_NOT_FOUND = 0x2;
            // No more items have been found.
            const int ERROR_NO_MORE_ITEMS = 259;
            // Pointer to a GROUPID variable
            long groupId = 0;

            // Local variables
            int cacheEntryInfoBufferSizeInitial = 0;
            int cacheEntryInfoBufferSize = 0;
            IntPtr cacheEntryInfoBuffer = IntPtr.Zero;
            INTERNET_CACHE_ENTRY_INFOA internetCacheEntry;
            IntPtr enumHandle = IntPtr.Zero;
            bool returnValue = false;

            // Delete the groups first.
            // Groups may not always exist on the system.
            // For more information, visit the following Microsoft Web site:
            // http://msdn.microsoft.com/library/?url=/workshop/networking/wininet/overview/cache.asp           
            // By default, a URL does not belong to any group. Therefore, that cache may become
            // empty even when the CacheGroup APIs are not used because the existing URL does not belong to any group.          
            enumHandle = FindFirstUrlCacheGroup(0, CACHEGROUP_SEARCH_ALL, IntPtr.Zero, 0, ref groupId, IntPtr.Zero);
            // If there are no items in the Cache, you are finished.
            if (enumHandle != IntPtr.Zero && ERROR_NO_MORE_ITEMS == Marshal.GetLastWin32Error())
                return;

            // Loop through Cache Group, and then delete entries.
            while (true)
            {
                if (ERROR_NO_MORE_ITEMS == Marshal.GetLastWin32Error() || ERROR_FILE_NOT_FOUND == Marshal.GetLastWin32Error())
                    break;
                // Delete a particular Cache Group.
                returnValue = DeleteUrlCacheGroup(groupId, CACHEGROUP_FLAG_FLUSHURL_ONDELETE, IntPtr.Zero);
                if (returnValue || (ERROR_NO_MORE_ITEMS != Marshal.GetLastWin32Error() && ERROR_FILE_NOT_FOUND != Marshal.GetLastWin32Error()))
                    returnValue = FindNextUrlCacheGroup(enumHandle, ref groupId, IntPtr.Zero);

                if (!returnValue && ERROR_NO_MORE_ITEMS == Marshal.GetLastWin32Error())
                    break;
            }
            Marshal.FreeHGlobal(cacheEntryInfoBuffer);

            // Start to delete URLs that do not belong to any group.
            enumHandle = FindFirstUrlCacheEntry(null, IntPtr.Zero, ref cacheEntryInfoBufferSizeInitial);
            if (enumHandle == IntPtr.Zero && ERROR_NO_MORE_ITEMS == Marshal.GetLastWin32Error())
                return;

            cacheEntryInfoBufferSize = cacheEntryInfoBufferSizeInitial;
            cacheEntryInfoBuffer = Marshal.AllocHGlobal(cacheEntryInfoBufferSize);
            enumHandle = FindFirstUrlCacheEntry(null, cacheEntryInfoBuffer, ref cacheEntryInfoBufferSizeInitial);

            while (true)
            {
                internetCacheEntry = (INTERNET_CACHE_ENTRY_INFOA)Marshal.PtrToStructure(cacheEntryInfoBuffer, typeof(INTERNET_CACHE_ENTRY_INFOA));

                if (ERROR_NO_MORE_ITEMS == Marshal.GetLastWin32Error())
                    break;

                cacheEntryInfoBufferSizeInitial = cacheEntryInfoBufferSize;
                returnValue = DeleteUrlCacheEntry(internetCacheEntry.lpszSourceUrlName);
                if (returnValue || (ERROR_NO_MORE_ITEMS != Marshal.GetLastWin32Error() && ERROR_FILE_NOT_FOUND != Marshal.GetLastWin32Error()))
                    returnValue = FindNextUrlCacheEntry(enumHandle, cacheEntryInfoBuffer, ref cacheEntryInfoBufferSizeInitial);

                if (!returnValue && (ERROR_NO_MORE_ITEMS == Marshal.GetLastWin32Error() || ERROR_FILE_NOT_FOUND == Marshal.GetLastWin32Error()))
                    break;

                if (!returnValue && cacheEntryInfoBufferSizeInitial > cacheEntryInfoBufferSize)
                {
                    cacheEntryInfoBufferSize = cacheEntryInfoBufferSizeInitial;
                    cacheEntryInfoBuffer = Marshal.ReAllocHGlobal(cacheEntryInfoBuffer, (IntPtr)cacheEntryInfoBufferSize);
                    returnValue = FindNextUrlCacheEntry(enumHandle, cacheEntryInfoBuffer, ref cacheEntryInfoBufferSizeInitial);
                }
            }
            Marshal.FreeHGlobal(cacheEntryInfoBuffer);
        }
        // For PInvoke: Contains information about an entry in the Internet cache
        [StructLayout(LayoutKind.Explicit, Size = 80)]
        public struct INTERNET_CACHE_ENTRY_INFOA
        {
            [FieldOffset(0)]
            public uint dwStructSize;
            [FieldOffset(4)]
            public IntPtr lpszSourceUrlName;
            [FieldOffset(8)]
            public IntPtr lpszLocalFileName;
            [FieldOffset(12)]
            public uint CacheEntryType;
            [FieldOffset(16)]
            public uint dwUseCount;
            [FieldOffset(20)]
            public uint dwHitRate;
            [FieldOffset(24)]
            public uint dwSizeLow;
            [FieldOffset(28)]
            public uint dwSizeHigh;
            [FieldOffset(32)]
            public FILETIME LastModifiedTime;
            [FieldOffset(40)]
            public FILETIME ExpireTime;
            [FieldOffset(48)]
            public FILETIME LastAccessTime;
            [FieldOffset(56)]
            public FILETIME LastSyncTime;
            [FieldOffset(64)]
            public IntPtr lpHeaderInfo;
            [FieldOffset(68)]
            public uint dwHeaderInfoSize;
            [FieldOffset(72)]
            public IntPtr lpszFileExtension;
            [FieldOffset(76)]
            public uint dwReserved;
            [FieldOffset(76)]
            public uint dwExemptDelta;
        }

        // For PInvoke: Initiates the enumeration of the cache groups in the Internet cache
        [DllImport(@"wininet",
            SetLastError = true,
            CharSet = CharSet.Auto,
            EntryPoint = "FindFirstUrlCacheGroup",
            CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr FindFirstUrlCacheGroup(
            int dwFlags,
            int dwFilter,
            IntPtr lpSearchCondition,
            int dwSearchCondition,
            ref long lpGroupId,
            IntPtr lpReserved);

        // For PInvoke: Retrieves the next cache group in a cache group enumeration
        [DllImport(@"wininet",
            SetLastError = true,
            CharSet = CharSet.Auto,
            EntryPoint = "FindNextUrlCacheGroup",
            CallingConvention = CallingConvention.StdCall)]
        public static extern bool FindNextUrlCacheGroup(
            IntPtr hFind,
            ref long lpGroupId,
            IntPtr lpReserved);

        // For PInvoke: Releases the specified GROUPID and any associated state in the cache index file
        [DllImport(@"wininet",
            SetLastError = true,
            CharSet = CharSet.Auto,
            EntryPoint = "DeleteUrlCacheGroup",
            CallingConvention = CallingConvention.StdCall)]
        public static extern bool DeleteUrlCacheGroup(
            long GroupId,
            int dwFlags,
            IntPtr lpReserved);

        // For PInvoke: Begins the enumeration of the Internet cache
        [DllImport(@"wininet",
            SetLastError = true,
            CharSet = CharSet.Auto,
            EntryPoint = "FindFirstUrlCacheEntryA",
            CallingConvention = CallingConvention.StdCall)]
        public static extern IntPtr FindFirstUrlCacheEntry(
            [MarshalAs(UnmanagedType.LPTStr)] string lpszUrlSearchPattern,
            IntPtr lpFirstCacheEntryInfo,
            ref int lpdwFirstCacheEntryInfoBufferSize);

        // For PInvoke: Retrieves the next entry in the Internet cache
        [DllImport(@"wininet",
            SetLastError = true,
            CharSet = CharSet.Auto,
            EntryPoint = "FindNextUrlCacheEntryA",
            CallingConvention = CallingConvention.StdCall)]
        public static extern bool FindNextUrlCacheEntry(
            IntPtr hFind,
            IntPtr lpNextCacheEntryInfo,
            ref int lpdwNextCacheEntryInfoBufferSize);

        // For PInvoke: Removes the file that is associated with the source name from the cache, if the file exists
        [DllImport(@"wininet",
            SetLastError = true,
            CharSet = CharSet.Auto,
            EntryPoint = "DeleteUrlCacheEntryA",
            CallingConvention = CallingConvention.StdCall)]
        public static extern bool DeleteUrlCacheEntry(
            IntPtr lpszUrlName);


    }
}
