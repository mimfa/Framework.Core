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
using System.Net.Sockets;
using System.Net;
using System.Net.NetworkInformation;
using MiMFa.Network;

namespace MiMFa.Service
{
    public static class NetService
    {
        public static IPAddress GetExternalIP(string url)
        {
            MiMFa.Network.Web.WebClient webClient = new MiMFa.Network.Web.WebClient();
            string response = UTF8Encoding.UTF8.GetString(webClient.DownloadData(url));
            IPAddress ip = IPAddress.Parse(response);
            return ip;
        }
        public static string GetHostName()
        {
            return Dns.GetHostName();
        }
        public static List<IPAddress> GetInternalIPs(string hostName = null)
        {
            if (string.IsNullOrEmpty(hostName)) hostName= GetHostName();
            var host = Dns.GetHostEntry(hostName);
            return (from ip in host.AddressList where !IPAddress.IsLoopback(ip) select ip).ToList();
        }
        public static IPAddress GetInternalIPv4(string hostName = null)
        {
            if (string.IsNullOrEmpty(hostName)) hostName = GetHostName();
            var host = Dns.GetHostEntry(hostName);
            IPAddress[] addr = host.AddressList;
            foreach (var item in addr)
                if (item.AddressFamily == AddressFamily.InterNetwork)
                    return item;
            return null;
        }
        public static IPAddress GetInternalIPv6(string hostName = null)
        {
            if (string.IsNullOrEmpty(hostName)) hostName = GetHostName();
            var host = Dns.GetHostEntry(hostName);
            IPAddress[] addr = host.AddressList;
            foreach (var item in addr)
                if (item.AddressFamily == AddressFamily.InterNetworkV6)
                    return item;
            return null;
        }
        public static PhysicalAddress GetMAC()
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in nics)
            {
                    IPInterfaceProperties properties = adapter.GetIPProperties();
                    return adapter.GetPhysicalAddress();
            }
            return null;
        }
        public static List<PhysicalAddress> GetMACs()
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            List<PhysicalAddress> lpa = new List<PhysicalAddress>();
            foreach (NetworkInterface adapter in nics)
            {
                IPInterfaceProperties properties = adapter.GetIPProperties();
                lpa.Add( adapter.GetPhysicalAddress());
            }
            return lpa;
        }

        public static IPAddress GetBroadcast(IPAddress address, IPAddress subnetMask)
        {
            byte[] ipAdressBytes = address.GetAddressBytes();
            byte[] subnetMaskBytes = subnetMask.GetAddressBytes();

            if (ipAdressBytes.Length != subnetMaskBytes.Length)
                throw new ArgumentException("Lengths of IP address and subnet mask do not match.");

            byte[] broadcastAddress = new byte[ipAdressBytes.Length];
            for (int i = 0; i < broadcastAddress.Length; i++)
            {
                broadcastAddress[i] = (byte)(ipAdressBytes[i] | (subnetMaskBytes[i] ^ 255));
            }
            return new IPAddress(broadcastAddress);
        }
        public static IPAddress GetBroadcast(IPAddress address)
        {
           return GetBroadcast(address, GetSubnetMask(address));
        }
        public static IPAddress GetBroadcast()
        {
           return GetBroadcast(GetInternalIPv4());
        }
        public static IPAddress GetSubnetMask(IPAddress address)
        {
            foreach (NetworkInterface adapter in NetworkInterface.GetAllNetworkInterfaces())
            {
                foreach (UnicastIPAddressInformation unicastIPAddressInformation in adapter.GetIPProperties().UnicastAddresses)
                {
                    if (unicastIPAddressInformation.Address.AddressFamily == AddressFamily.InterNetwork)
                    {
                        if (address.Equals(unicastIPAddressInformation.Address))
                        {
                            return unicastIPAddressInformation.IPv4Mask;
                        }
                    }
                }
            }
            throw new ArgumentException(string.Format("Can't find subnetmask for IP address '{0}'", address));
        }

        public static string SendJSON(string url, string value, string method = "POST", string charSet = "UTF-8", bool retry = true)
        {
            return SEND(url, value, method, "application/json", "application/json", charSet, retry);
        }
        public static string SendURLENCODED(string url, string value, string method = "POST", string charSet = "UTF-8", bool retry = true)
        {
            string HtmlResult = null;
            using (MiMFa.Network.Web.WebClient wc = new MiMFa.Network.Web.WebClient())
            {
                wc.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                wc.Headers[HttpRequestHeader.AcceptCharset] = charSet;
                HtmlResult = wc.UploadString(url, value).Trim();
            }
            return HtmlResult;
        }

        public static string SEND(string url,string value, string method = "POST", string accept = "application/json", string type = "application/json",string charSet = "UTF-8", bool retry = true)
        {
            string HtmlResult = null;
            try
            {
                var http = (HttpWebRequest)WebRequest.Create(url);
                http.Accept = accept;
                http.ContentType = type;
                http.Headers.Add("Charset", charSet);
                http.Method = method;

                Encoding encoding;
                switch (charSet.ToLower())
                {
                    case "ascii":
                        encoding = new ASCIIEncoding();
                        break;
                    case "utf7":
                    case "utf-7":
                        encoding = new UTF7Encoding();
                        break;
                    case "utf32":
                    case "utf-32":
                        encoding = new UTF32Encoding();
                        break;
                    case "uni":
                    case "uc":
                    case "unicode":
                        encoding = new UnicodeEncoding();
                        break;
                    default:
                        encoding = new UTF8Encoding();
                        break;
                }
                Byte[] bytes = encoding.GetBytes(value);

                Stream newStream = http.GetRequestStream();
                newStream.Write(bytes, 0, bytes.Length);
                newStream.Close();

                var response = http.GetResponse();

                var stream = response.GetResponseStream();
                var sr = new StreamReader(stream);
                HtmlResult = sr.ReadToEnd();
            }
            catch
            {
                if (retry)
                    using (Network.Web.WebClient wc = new Network.Web.WebClient())
                    {
                        wc.Headers[HttpRequestHeader.ContentType] = type;
                        wc.Headers[HttpRequestHeader.Accept] = accept;
                        wc.Headers[HttpRequestHeader.AcceptCharset] = charSet;
                        HtmlResult = wc.UploadString(url, value).Trim();
                    }
            }
            return HtmlResult;
        }
    }
}
