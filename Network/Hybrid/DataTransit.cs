using MiMFa.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MiMFa.Network.Hybrid
{
    public class DataTransit
    {
        #region Property
        public event FTPAddressRecieveEventHandler AddressReceive = (ad) => { };
        public event TransitBinaryEventHandler DataReceive = (d,ip,l) => { };
        public event SocketEventHandler ConnectedToInterlocutor = (ip) => { };
        public event TransitDataEventHandler StartSendData = (o, ip, e) => { };
        public event SocketEventHandler StopSendData = (ip) => { };
        public event TransitBinaryEventHandler EndSendData = (o, ip, e) => { };
        public event TransitDataErrorEventHandler ErrorSendData = (o, ip, e, ex) => { };
        public event SocketEventHandler ListeningToInterlocutor = (ip) => { };
        public event TransitDataEventHandler StartReceiveData = (o, ip, e) => { };
        public event TransitBinaryEventHandler EndReceiveData = (o, ip, e) => { };
        public event TransitDataErrorEventHandler ErrorReceiveData = (o, ip, e, ex) => { };
        public event SocketEventHandler StopReceiveData = (ip) => { };
        public event FTPEventHandler LogINToInterlocutor = (ip) => { };
        public event FTPTransitDataEventHandler StartDownloadData = (l, r, ip) => { };
        public event FTPTransitDataEventHandler EndDownloadData = (l, r, ip) => { };
        public event FTPTransitDataEventHandler StartUploadData = (l, r, ip) => { };
        public event FTPTransitDataEventHandler EndUploadData = (l, r, ip) => { };

        public bool Run { get { return Receiver.Run; } set { Receiver.Run = Sender.Run = value; } }

        public FTP.DataTransit FTP { get; set; } = null;
        public TCP.Sender Sender { get; set; } = new TCP.Sender();
        public TCP.Receiver Receiver { get; set; } = new TCP.Receiver();

        public string UserName { get; set; } = null;
        public string Password { get; set; } = null;
        public string FTPInterlocutorHostName { get; set; } ="";
        public string SenderInterlocutorHostName { get { return Sender.InterlocutorHostName; } set { Sender.InterlocutorHostName = value; } }
        public string ReceiverInterlocutorHostName { get { return Receiver.InterlocutorHostName; } set { Receiver.InterlocutorHostName = value; } }
        public IPAddress FTPInterlocutorIP { get; set; } = NetService.GetInternalIPv4();
        public IPAddress SenderInterlocutorIP { get { return Sender.InterlocutorIP; } set { Sender.InterlocutorIP = value; } }
        public IPAddress ReceiverInterlocutorIP { get { return Receiver.InterlocutorIP; } set { Receiver.InterlocutorIP = value; } }
        public IPEndPoint FTPInterlocutorIPEndPoint => new IPEndPoint(FTPInterlocutorIP, FTPPort);
        public IPEndPoint SenderInterlocutorIPEndPoint => new IPEndPoint(SenderInterlocutorIP, SenderPort);
        public IPEndPoint ReceiverInterlocutorIPEndPoint => new IPEndPoint(ReceiverInterlocutorIP, ReceiverPort);

        public int FTPPort { get; set; } = 21;
        public int SenderPort { get; set; } = 7961;
        public int ReceiverPort { get; set; } = 7962;
        public int SendTimeout { get; set; } = 3000;
        public int ReceiveTimeout { get; set; } = 3000;
        public int FTPBufferSize { get { return FTP.BufferSize; } set { FTP.BufferSize = value; } }
        public int TransitBufferSize { get; set; } = 1024;

        public string LocalTempDirectory { get; set; } = "";
        public string RemoteTempDirectory { get; set; } = "";
        #endregion

        #region Constructor
        public DataTransit(string hostIP, string userName, string password)
        {
            SenderInterlocutorIP = FTPInterlocutorIP = IPAddress.Parse(hostIP); UserName = userName; Password = password;
            FTP = new Network.FTP.DataTransit(@"ftp://" + hostIP + "/", userName, password);
            AddEvent();
        }
        public DataTransit(IPAddress hostIP, string userName, string password)
        {
            SenderInterlocutorIP = FTPInterlocutorIP = hostIP; UserName = userName; Password = password;
            FTP = new Network.FTP.DataTransit(@"ftp://" + hostIP + "/", userName, password);
            AddEvent();
        }
        private void AddEvent()
        {
            FTP.LogInToInterlocutor += LogINToInterlocutor;
            FTP.StartDownloadData += StartDownloadData;
            FTP.EndDownloadData += EndDownloadData;
            FTP.StartUploadData += StartUploadData;
            FTP.EndUploadData += EndUploadData;
            Sender.ConnectedToInterlocutor += ConnectedToInterlocutor;
            Sender.StartSendData += StartSendData;
            Sender.EndSendData += EndSendData;
            Sender.StopSendData += StopSendData;
            Sender.ErrorSendData += ErrorSendData;
            Receiver.ConnectedToInterlocutor += ConnectedToInterlocutor;
            Receiver.ListeningToInterlocutor += ListeningToInterlocutor;
            Receiver.StartReceiveData += StartReceiveData;
            Receiver.EndReceiveData += Receiver_EndReceiveData;
            Receiver.EndReceiveData += EndReceiveData;
            Receiver.ErrorReceiveData += ErrorReceiveData;
            Receiver.StopReceiveData += StopReceiveData;
        }
        #endregion

        public void Start()
        {
            Receiver.Start(ReceiverInterlocutorIPEndPoint);
        }

        public void Send(string fileAddress, string interlocutorHostName, int port)
        {
            SenderInterlocutorHostName = interlocutorHostName.Trim();
            SenderPort = port;
            try
            {
                SenderInterlocutorIP = NetService.GetInternalIPv4(SenderInterlocutorHostName);
            }
            catch
            {
                try
                {
                    SenderInterlocutorIP = IPAddress.Parse(SenderInterlocutorHostName);
                }
                catch
                {
                    SenderInterlocutorIP = null;
                }
            }
            Send(fileAddress);
        }
        public void Send(string fileAddress, string interlocutorAddress)
        {
            string[] sa = interlocutorAddress.Split(':');
            SenderInterlocutorHostName = sa[0].Trim();
            if (sa.Length > 1) SenderPort = int.Parse(sa[1].Trim());
            try
            {
                SenderInterlocutorIP = NetService.GetInternalIPv4(SenderInterlocutorHostName);
            }
            catch
            {
                try
                {
                    SenderInterlocutorIP = IPAddress.Parse(SenderInterlocutorHostName);
                }
                catch
                {
                    SenderInterlocutorIP = null;
                }
            }
            Send(fileAddress);
        }
        public void Send(string fileAddress, IPEndPoint interlocutorIPEndPoint)
        {
            SenderInterlocutorHostName = string.Empty;
            SenderInterlocutorIP = interlocutorIPEndPoint.Address;
            SenderPort = interlocutorIPEndPoint.Port;
            Send(fileAddress);
        }
        public void Send(string fileAddress, IPAddress interlocutorIP, int port = -1)
        {
            SenderInterlocutorHostName = string.Empty;
            SenderInterlocutorIP = interlocutorIP;
            if (port > 1024) SenderPort = port;
            Send(fileAddress);
        }
        public void Send(object obj, string interlocutorHostName, int port)
        {
            SenderInterlocutorHostName = interlocutorHostName.Trim();
            SenderPort = port;
            try
            {
                SenderInterlocutorIP = NetService.GetInternalIPv4(SenderInterlocutorHostName);
            }
            catch
            {
                try
                {
                    SenderInterlocutorIP = IPAddress.Parse(SenderInterlocutorHostName);
                }
                catch
                {
                    SenderInterlocutorIP = null;
                }
            }
            Send(obj);
        }
        public void Send(object obj, string interlocutorAddress)
        {
            string[] sa = interlocutorAddress.Split(':');
            SenderInterlocutorHostName = sa[0].Trim();
            if (sa.Length > 1) SenderPort = int.Parse(sa[1].Trim());
            try
            {
                SenderInterlocutorIP = NetService.GetInternalIPv4(SenderInterlocutorHostName);
            }
            catch
            {
                try
                {
                    SenderInterlocutorIP = IPAddress.Parse(SenderInterlocutorHostName);
                }
                catch
                {
                    SenderInterlocutorIP = null;
                }
            }
            Send(obj);
        }
        public void Send(object obj, IPEndPoint interlocutorIPEndPoint)
        {
            SenderInterlocutorHostName = string.Empty;
            SenderInterlocutorIP = interlocutorIPEndPoint.Address;
            SenderPort = interlocutorIPEndPoint.Port;
            Send(obj);
        }
        public void Send(object obj, IPAddress interlocutorIP, int port = -1)
        {
            SenderInterlocutorHostName = string.Empty;
            SenderInterlocutorIP = interlocutorIP;
            if (port > 1024) SenderPort = port;
            Send(obj);
        }
        public void Send(object obj)
        {
            string name = FTP.InterlocutorAddress.Replace("ftp:", "").Replace("/", "").Replace("\\", "").Replace(".", "") + DateTime.Now.Ticks +
                 "." +  InfoService.GetMimeObject(obj).Split('/').Last();
            string la = LocalTempDirectory + name;
            IOService.SaveSerializeFile(la,obj);
            Send(la);
        }
        [STAThread]
        public void Send(string fileAddress)
        {
            if (SenderInterlocutorIP == null && !string.IsNullOrEmpty(SenderInterlocutorHostName))
                try
                {
                    SenderInterlocutorIP = NetService.GetInternalIPv4(SenderInterlocutorHostName);
                }
                catch { }
            if (SenderInterlocutorIP != null)
            {
                string ra = RemoteTempDirectory + Path.GetFileName(fileAddress);
                FTP.Upload(fileAddress, ra);
                Sender.Start(ra, SenderInterlocutorIPEndPoint);
            }
            else throw new ArgumentException("Not set IP or HostName");
        }

        public void Receive(string remoteFileAddress, string interlocutorHostName, int port)
        {
            ReceiverInterlocutorHostName = interlocutorHostName.Trim();
            ReceiverPort = port;
            try
            {
                ReceiverInterlocutorIP = NetService.GetInternalIPv4(ReceiverInterlocutorHostName);
            }
            catch
            {
                try
                {
                    ReceiverInterlocutorIP = IPAddress.Parse(ReceiverInterlocutorHostName);
                }
                catch
                {
                    ReceiverInterlocutorIP = null;
                }
            }
            Receive(remoteFileAddress);
        }
        public void Receive(string remoteFileAddress, string interlocutorAddress)
        {
            string[] sa = interlocutorAddress.Split(':');
            ReceiverInterlocutorHostName = sa[0].Trim();
            if (sa.Length > 1) ReceiverPort = int.Parse(sa[1].Trim());
            try
            {
                ReceiverInterlocutorIP = NetService.GetInternalIPv4(ReceiverInterlocutorHostName);
            }
            catch
            {
                try
                {
                    ReceiverInterlocutorIP = IPAddress.Parse(ReceiverInterlocutorHostName);
                }
                catch
                {
                    ReceiverInterlocutorIP = null;
                }
            }
            Receive(remoteFileAddress);
        }
        public void Receive(string remoteFileAddress, IPEndPoint interlocutorIPEndPoint)
        {
            ReceiverInterlocutorHostName = string.Empty;
            ReceiverInterlocutorIP = interlocutorIPEndPoint.Address;
            ReceiverPort = interlocutorIPEndPoint.Port;
            Receive(remoteFileAddress);
        }
        public void Receive(string remoteFileAddress, IPAddress interlocutorIP, int port = -1)
        {
            ReceiverInterlocutorHostName = string.Empty;
            ReceiverInterlocutorIP = interlocutorIP;
            if (port > 1024) ReceiverPort = port;
            Receive(remoteFileAddress);
        }
        [STAThread]
        public void Receive(string remoteFileAddress)
        {
            if (SenderInterlocutorIP == null && !string.IsNullOrEmpty(SenderInterlocutorHostName))
                try
                {
                    SenderInterlocutorIP = NetService.GetInternalIPv4(SenderInterlocutorHostName);
                }
                catch { }
            if (SenderInterlocutorIP != null)
            {
                string la = LocalTempDirectory + Path.GetFileName(remoteFileAddress);
                FTP.Download(la, remoteFileAddress);
            }
            else throw new ArgumentException("Not set IP or HostName");
        }

        private void Receiver_EndReceiveData(byte[] data, IPEndPoint ip, long length)
        {
            try
            {
                string str = IOService.TryDeserialize(data);
                if (File.Exists(str))
                { AddressReceive(str); return; }
                else throw new Exception("Is not address");
            }
            catch { DataReceive(data,ip,length); }
        }
    }
}
