using MiMFa.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MiMFa.Network.TCP
{
    public class Sender
    {
        public virtual TransportType TransType { get; set; }= TransportType.Tcp;
        public event SocketEventHandler ConnectedToInterlocutor = (ip) => { };
        public event SocketEventHandler Starting = (ip) => { };
        public event SocketEventHandler Stoping = (ip) => { };
        public event TransitDataEventHandler StartSendData = (o, ip, e) => { };
        public event SocketEventHandler StopSendData = ( ip) => { };
        public event TransitBinaryEventHandler EndSendData = (o, ip, e) => { };
        public event TransitDataErrorEventHandler ErrorSendData = (o, ip, e ,ex) => { };
        public event TransitBinaryEventHandler ReceiveCallBackData = (o, ip, e) => { };
        public event TransitDataErrorEventHandler ErrorReceiveCallBackData = (o, ip, e, ex) => { };

        public bool Run { get; set; } = true;
        public IPAddress InterlocutorIP { get; set; } = null;
        public string InterlocutorHostName { get; set; } = string.Empty;
        public int Port { get; set; } = 7950;
        public IPEndPoint InterlocutorIPEndPoint => new IPEndPoint(InterlocutorIP, Port);
        public int BufferSize { get; set; } = 1024 * 1024;
        public int Timeout { get; set; } = 3000;
        public int PendingQueueLength { get; set; } = 1000;

        Socket SenderSock;

        public void Start(object obj, string interlocutorHostName, int port)
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
            Start(obj);
        }
        public void Start(object obj, string interlocutorAddress)
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
            Start(obj);
        }
        public void Start(object obj,IPEndPoint interlocutorIPEndPoint)
        {
            InterlocutorHostName = string.Empty;
            InterlocutorIP = interlocutorIPEndPoint.Address;
            Port = interlocutorIPEndPoint.Port;
            Start(obj);
        }
        public void Start(object obj, IPAddress interlocutorIP, int port = -1)
        {
            InterlocutorHostName = string.Empty;
            InterlocutorIP = interlocutorIP;
            if (port > 1024) Port = port;
            Start(obj);
        }
        [STAThread]
        public void Start(object obj)
        {
            Thread th = new Thread(() =>
            {
                try
                {
                    if (InterlocutorIP == null && !string.IsNullOrEmpty(InterlocutorHostName))
                        try
                        {
                            InterlocutorIP = NetService.GetInternalIPv4(InterlocutorHostName);
                        }
                        catch { }
                    if (InterlocutorIP != null)
                    {
                        Starting(InterlocutorIPEndPoint);
                        SocketPermission Permission;
                       // Creates one SocketPermission object for access restrictions
                       Permission = new SocketPermission(
                          NetworkAccess.Accept,     // Allowed to accept connections 
                          TransType,        // Defines transport types 
                          "",                       // The IP addresses of local host 
                          SocketPermission.AllPorts // Specifies all ports 
                          );
                       // Listening Socket object 
                       SenderSock = null;
                       // Ensures the code to have permission to access a Socket 
                       Permission.Demand();
                       // Create one Socket object to listen the incoming connection 
                       SenderSock = new Socket(
                              InterlocutorIP.AddressFamily,
                              SocketType.Stream,
                              ProtocolType.Tcp
                              );
                       // Associates a Socket with a local endpoint 
                       SenderSock.SendTimeout = Timeout;
                        SenderSock.Connect(InterlocutorIPEndPoint);
                        ConnectedToInterlocutor(InterlocutorIPEndPoint);
                       // Places a Socket in a listening state and specifies the maximum 
                       // Length of the pending connections queue 
                       // Sending message 
                       byte[] data = (obj != null && obj is byte[]) ? (byte[])obj : IOService.Serialize(obj);
                        // Sends data to a connected Socket. 
                        StartSendData(obj, InterlocutorIPEndPoint, 0);
                       int bytesSend = SenderSock.Send(data);
                        EndSendData(data, InterlocutorIPEndPoint, bytesSend);
                        Receive();
                    }
                    else throw new ArgumentException("Not set IP or HostName");
                }
                catch (Exception ex)
                {
                    ErrorSendData(obj, InterlocutorIPEndPoint, 0, ex);
                }
            });
            th.IsBackground = true;
            th.Start();
        }

        public void Stop()
        {
            Run = false;
            // Disables sends and receives on a Socket. 
            try
            {
                Stoping(InterlocutorIPEndPoint);
                try { SenderSock.Shutdown(SocketShutdown.Both); } catch { }
                //Closes the Socket connection and releases all resources 
                SenderSock.Close();
                try { StopSendData(InterlocutorIPEndPoint); } catch { }
            }
            catch { }
        }

        public void Receive()
        {
            try
            {
                byte[] buffer = new byte[BufferSize];
                // Receives data from a bound Socket. 
                //int bytesRec = SenderSock.Receive(buffer);
                // Continues to read the data till data isn't available 
                while (SenderSock.Available > 0)
                {
                    int bytesRec = SenderSock.Receive(buffer);
                    if (bytesRec > 0)
                        ReceiveCallBackData(CollectionService.GetPart(buffer, 0, bytesRec - 1), InterlocutorIPEndPoint, bytesRec);
                }
            }
            catch (Exception ex)
            {
                ErrorReceiveCallBackData(SenderSock, InterlocutorIPEndPoint, 0, ex);
            }
        }
    }
}
