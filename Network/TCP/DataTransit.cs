using MiMFa.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace MiMFa.Network.TCP
{
    public class DataTransit
    {
        public event SocketEventHandler ConnectedToInterlocutor = (ip)=> { };
        public event TransitDataEventHandler StartDownloadData = (o,ip,l)=> { };
        public event TransitBinaryEventHandler StopDownloadData = (o, ip, l) => { };
        public event TransitDataEventHandler EndDownloadData = (o, ip, l) => { };
        public event TransitDataEventHandler StartUploadData = (o, ip, l) => { };
        public event TransitDataEventHandler StopUploadData = (o, ip, l) => { };
        public event TransitDataEventHandler EndUploadData = (o, ip, l) => { };
        public bool Run { get; set; } = true;
        public IPAddress InterlocutorIP { get; set; } = NetService.GetInternalIPv4();
        public string InterlocutorHostName { get; set; } = string.Empty;
        public int Port { get; set; } = 7950;
        public IPEndPoint InterlocutorIPEndPoint => new IPEndPoint(InterlocutorIP, Port);
        public int SendTimeout { get; set; } = 3000;
        public int ReceiveTimeout { get; set; } = 3000;
        public int BufferSize { get; set; } = 1024 * 1024;

        public void Send(object obj, string interlocutorHostName, int port)
        {
            InterlocutorHostName = interlocutorHostName.Trim();
            Port = port;
            try
            {
                InterlocutorIP = NetService.GetInternalIPv4(InterlocutorHostName);
            }
            catch
            {
                try
                {
                    InterlocutorIP = IPAddress.Parse(InterlocutorHostName);
                }
                catch
                {
                    InterlocutorIP = null;
                }
            }
            Send(obj);
        }
        public void Send(object obj, string interlocutorAddress)
        {
            string[] sa = interlocutorAddress.Split(':');
            InterlocutorHostName = sa[0].Trim();
            if (sa.Length > 1) Port = int.Parse(sa[1].Trim());
            try
            {
                InterlocutorIP = NetService.GetInternalIPv4(InterlocutorHostName);
            }
            catch
            {
                try
                {
                    InterlocutorIP = IPAddress.Parse(InterlocutorHostName);
                }
                catch
                {
                    InterlocutorIP = null;
                }
            }
            Send(obj);
        }
        public void Send(object obj, IPAddress interlocutorIP, int port = -1)
        {
            InterlocutorHostName = string.Empty;
            InterlocutorIP = interlocutorIP;
            if (port > 1024) Port = port;
            Send(obj);
        }
        public void Send(object obj)
        {
            if (InterlocutorIP == null && !string.IsNullOrEmpty(InterlocutorHostName))
                try
                {
                    InterlocutorIP = NetService.GetInternalIPv4(InterlocutorHostName);
                }
                catch { }
            if (InterlocutorIP != null)
            {
                TcpClient client = new TcpClient(new IPEndPoint(InterlocutorIP, Port)); // have my connection established with a Tcp Server 
                ConnectedToInterlocutor(InterlocutorIPEndPoint);
                client.SendTimeout = SendTimeout;
                client.ReceiveTimeout = ReceiveTimeout;
                NetworkStream strm = client.GetStream(); // the stream 
                StartUploadData(obj, InterlocutorIPEndPoint, 0);
                IFormatter formatter = new BinaryFormatter(); // the formatter that will serialize my object on my stream 
                formatter.Serialize(strm, obj); // the serialization process 
                strm.Close();
                client.Close();
                EndUploadData(obj, InterlocutorIPEndPoint, strm.Length);
            }
            else throw new ArgumentException("Not set IP or HostName");
        }

        public void Receive(string interlocutorHostName, int port)
        {
            InterlocutorHostName = interlocutorHostName.Trim();
            Port = port;
            try
            {
                InterlocutorIP = NetService.GetInternalIPv4(InterlocutorHostName);
            }
            catch
            {
                try
                {
                    InterlocutorIP = IPAddress.Parse(InterlocutorHostName);
                }
                catch
                {
                    InterlocutorIP = null;
                }
            }
            Receive();
        }
        public void Receive(string interlocutorAddress)
        {
            string[] sa = interlocutorAddress.Split(':');
            InterlocutorHostName = sa[0].Trim();
            if (sa.Length > 1) Port = int.Parse(sa[1].Trim());
            try
            {
                InterlocutorIP = NetService.GetInternalIPv4(InterlocutorHostName);
            }
            catch
            {
                try
                {
                    InterlocutorIP = IPAddress.Parse(InterlocutorHostName);
                }
                catch
                {
                    InterlocutorIP = null;
                }
            }
            Receive();
        }
        public void Receive(IPAddress interlocutorIP, int port = -1)
        {
            InterlocutorHostName = string.Empty;
            InterlocutorIP = interlocutorIP;
            if (port > 1024) Port = port;
            Receive();
        }
        public void Receive()
        {
            if (InterlocutorIP == null && !string.IsNullOrEmpty(InterlocutorHostName))
                try
                {
                    InterlocutorIP = NetService.GetInternalIPv4(InterlocutorHostName);
                }
                catch { }
            if (InterlocutorIP != null)
            {
                TcpListener listener = new TcpListener(InterlocutorIP, Port);
                listener.Start();
                ConnectedToInterlocutor(InterlocutorIPEndPoint);
                while (Run)
                {
                    TcpClient client = listener.AcceptTcpClient();
                    StartDownloadData(null, InterlocutorIPEndPoint, 0);
                    NetworkStream strm = client.GetStream();
                    BinaryFormatter formatter = new BinaryFormatter();
                    EndDownloadData(formatter.Deserialize(strm), InterlocutorIPEndPoint, client.ReceiveBufferSize);
                    strm.Close();
                    client.Close();
                }
                listener.Stop();
                StopDownloadData(null, InterlocutorIPEndPoint, 0);
            }
            else throw new ArgumentException("Not set IP or HostName");
        }


        public void SocketSend(object obj, string interlocutorHostName, int port)
        {
            InterlocutorHostName = interlocutorHostName.Trim();
            Port = port;
            try
            {
                InterlocutorIP = NetService.GetInternalIPv4(InterlocutorHostName);
            }
            catch
            {
                try
                {
                    InterlocutorIP = IPAddress.Parse(InterlocutorHostName);
                }
                catch
                {
                    InterlocutorIP = null;
                }
            }
            SocketSend(obj);
        }
        public void SocketSend(object obj, string interlocutorAddress)
        {
            string[] sa = interlocutorAddress.Split(':');
            InterlocutorHostName = sa[0].Trim();
            if (sa.Length > 1) Port = int.Parse(sa[1].Trim());
            try
            {
                InterlocutorIP = NetService.GetInternalIPv4(InterlocutorHostName);
            }
            catch
            {
                try
                {
                    InterlocutorIP = IPAddress.Parse(InterlocutorHostName);
                }
                catch
                {
                    InterlocutorIP = null;
                }
            }
            SocketSend(obj);
        }
        public void SocketSend(object obj, IPAddress interlocutorIP, int port = -1)
        {
            InterlocutorHostName = string.Empty;
            InterlocutorIP = interlocutorIP;
            if (port > 1024) Port = port;
            SocketSend(obj);
        }
        public void SocketSend(object obj)
        {
            if (InterlocutorIP == null && !string.IsNullOrEmpty(InterlocutorHostName))
                try
                { InterlocutorIP = IPAddress.Parse(InterlocutorHostName); }
                catch
                {
                    try
                    { InterlocutorIP = NetService.GetExternalIP(InterlocutorHostName); }
                    catch { }
                }
            if (InterlocutorIP != null)
            {
                TcpClient client = new TcpClient();
                client.Connect(InterlocutorIP, Port);
                ConnectedToInterlocutor(InterlocutorIPEndPoint);
                Stream stream = client.GetStream();
                StartUploadData(obj, InterlocutorIPEndPoint, 0);
                byte[] bytarr = (obj != null && obj is byte[]) ? (byte[])obj : IOService.Serialize(obj);
                stream.Write(bytarr, 0, bytarr.Length);
                client.Close();
                EndUploadData(obj, InterlocutorIPEndPoint, bytarr.Length);
            }
            else throw new ArgumentException("Not set IP or HostName");
        }

        public void SocketReceive(object obj, string interlocutorHostName, int port)
        {
            InterlocutorHostName = interlocutorHostName.Trim();
            Port = port;
            try
            {
                InterlocutorIP = NetService.GetInternalIPv4(InterlocutorHostName);
            }
            catch
            {
                try
                {
                    InterlocutorIP = IPAddress.Parse(InterlocutorHostName);
                }
                catch
                {
                    InterlocutorIP = null;
                }
            }
            SocketReceive();
        }
        public void SocketReceive(string interlocutorAddress)
        {
            string[] sa = interlocutorAddress.Split(':');
            InterlocutorHostName = sa[0].Trim();
            if (sa.Length > 1) Port = int.Parse(sa[1].Trim());
            try
            {
                InterlocutorIP = NetService.GetInternalIPv4(InterlocutorHostName);
            }
            catch
            {
                try
                {
                    InterlocutorIP = IPAddress.Parse(InterlocutorHostName);
                }
                catch
                {
                    InterlocutorIP = null;
                }
            }
            SocketReceive();
        }
        public void SocketReceive(IPAddress interlocutorIP, int port = -1)
        {
            InterlocutorHostName = string.Empty;
            InterlocutorIP = interlocutorIP;
            if (port > 1024) Port = port;
            SocketReceive();
        }
        public void SocketReceive()
        {
            if (InterlocutorIP == null && !string.IsNullOrEmpty(InterlocutorHostName))
                try
                { InterlocutorIP = NetService.GetInternalIPv4(InterlocutorHostName); }
                catch { }
            if (InterlocutorIP != null)
            {
                TcpListener listener = new TcpListener(InterlocutorIP, Port);
                listener.Start();
                ConnectedToInterlocutor(InterlocutorIPEndPoint);
                while (Run)
                {
                    Socket socket = listener.AcceptSocket();
                    byte[] bytarr = new byte[BufferSize];
                    StartDownloadData(null, InterlocutorIPEndPoint, 0);
                    int mi = socket.Receive(bytarr);
                    bool b = mi > 0;
                    if (b && EndDownloadData != null) EndDownloadData(IOService.Deserialize(bytarr), InterlocutorIPEndPoint, mi);
                    socket.Close();
                }
                listener.Stop();
                StopDownloadData(null, InterlocutorIPEndPoint, 0);
            }
            else throw new ArgumentException("Not set IP or HostName");
        }

    }
}
