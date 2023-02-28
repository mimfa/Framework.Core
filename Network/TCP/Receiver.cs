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
    public class Receiver
    {
        public virtual TransportType TransType { get; set; }= TransportType.Tcp;
        public virtual ProtocolType ProtType { get; set; }= ProtocolType.Tcp;
        public event SocketEventHandler ConnectedToInterlocutor = (ip) => { };
        public event SocketEventHandler ListeningToInterlocutor = (ip) => { };
        public event SocketEventHandler Starting = (ip) => { };
        public event SocketEventHandler Stoping = (ip) => { };
        public event TransitDataEventHandler StartReceiveData = (o, ip, e) => { };
        public event TransitBinaryEventHandler EndReceiveData = (o, ip, e) => { };
        public event TransitDataErrorEventHandler ErrorReceiveData = (o, ip, e ,ex) => { };
        public event SocketEventHandler StopReceiveData = ( ip) => { };
        public event TransitDataEventHandler SendCallBackData = (o, ip, e) => { };
        public event TransitDataErrorEventHandler ErrorSendCallBackData = (o, ip, e, ex) => { };

        public bool Run { get; set; } = true;
        public IPAddress InterlocutorIP { get; set; } = IPAddress.Any;
        public string InterlocutorHostName { get; set; } = string.Empty;
        public int Port { get; set; } = 7950;
        public IPEndPoint InterlocutorIPEndPoint => new IPEndPoint(InterlocutorIP, Port);
        public int Timeout { get; set; } = 3000;
        public int BufferSize { get; set; } = 1024 * 1024;
        public int PendingQueueLength { get; set; } = 1000;

        Socket Listener;
        Socket Handler;

        public void Start(string interlocutorHostName, int port)
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
            Start();
        }
        public void Start(string interlocutorAddress)
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
            Start();
        }
        public void Start(IPEndPoint interlocutorIPEndPoint)
        {
            InterlocutorHostName = string.Empty;
            InterlocutorIP = interlocutorIPEndPoint.Address;
            Port = interlocutorIPEndPoint.Port;
            Start();
        }
        public void Start(IPAddress interlocutorIP, int port = -1)
        {
            InterlocutorHostName = string.Empty;
            InterlocutorIP = interlocutorIP;
            if (port > 1024) Port = port;
            Start();
        }
        [STAThread]
        public void Start()
        {
            Run = true;
            if (InterlocutorIP == null && !string.IsNullOrEmpty(InterlocutorHostName))
                try
                {
                    InterlocutorIP = NetService.GetInternalIPv4(InterlocutorHostName);
                }
                catch { }
            if (InterlocutorIP == null) InterlocutorIP = IPAddress.Any;
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
                Listener = null;
                // Ensures the code to have permission to access a Socket 
                Permission.Demand();
                // Create one Socket object to listen the incoming connection 
                Listener = new Socket(
                    InterlocutorIP.AddressFamily,
                    SocketType.Stream,
                    ProtType
                    );
                // Associates a Socket with a local endpoint 
                Listener.ReceiveTimeout = Timeout;
                Listener.Bind(InterlocutorIPEndPoint);
                ConnectedToInterlocutor(InterlocutorIPEndPoint);
                // Places a Socket in a listening state and specifies the maximum 
                // Length of the pending connections queue 
                Listener.Listen(PendingQueueLength);
                // Begins an asynchronous operation to accept an attempt 
                ListeningToInterlocutor(InterlocutorIPEndPoint);
                if (Run) Listener.BeginAccept(new AsyncCallback(AcceptCallback), Listener);
            }
            else throw new ArgumentException("Not set IP or HostName");
        }

        public void Stop()
        {
            Run = false;
            // Disables sends and receives on a Socket. 
            try
            {
                Stoping(InterlocutorIPEndPoint);
                try { Handler.Shutdown(SocketShutdown.Both); } catch { }
                //Closes the Socket connection and releases all resources 
                Handler.Close();
                try { StopReceiveData(Handler.RemoteEndPoint as IPEndPoint); } catch { }
            }
            catch { }
            try
            {
                try
                { Listener.Shutdown(SocketShutdown.Both); }
                catch { }
                //Closes the Socket connection and releases all resources 
                Listener.Close();
            }
            catch { }
        }

        object Obj = null;
        public void Send(object obj)
        {
            Obj = obj;
            byte[] byteData = (obj != null && obj is byte[]) ? (byte[])obj : IOService.Serialize(obj);
            // Sends data asynchronously to a connected Socket 
            Handler.BeginSend(byteData, 0, byteData.Length, 0,
                new AsyncCallback(SendCallback), Handler);
        }

        private void SendCallback(IAsyncResult ar)
        {
            try
            { // A Socket which has sent the data to remote host 
            Socket handler = (Socket)ar.AsyncState;
            // The number of bytes sent to the Socket 
            int bytesSend = handler.EndSend(ar);
            SendCallBackData(Obj, Handler.RemoteEndPoint as IPEndPoint, bytesSend);
            }
            catch (Exception ex)
            {
                ErrorSendCallBackData(ar, InterlocutorIPEndPoint, 0, ex);
            }
        }
        public void AcceptCallback(IAsyncResult ar)
        {
            try
            {  // Receiving byte array 
                byte[] buffer = new byte[BufferSize];
                // Get Listening Socket object 
                Listener = (Socket)ar.AsyncState;
                // Create a new socket 
                Handler = Listener.EndAccept(ar);
                // Using the Nagle algorithm 
                Handler.NoDelay = false;
                // Creates one object array for passing data 
                // Begins to asynchronously receive data 
                Handler.BeginReceive(
                    buffer,        // An array of type Byt for received data 
                    0,             // The zero-based position in the buffer  
                    buffer.Length, // The number of bytes to receive 
                    SocketFlags.None,// Specifies send and receive behaviors 
                    new AsyncCallback(ReceiveCallback),//An AsyncCallback delegate 
                    buffer            // Specifies infomation for receive operation 
                    );
                // Begins an asynchronous operation to accept an attempt 
                if (Run) Listener.BeginAccept(new AsyncCallback(AcceptCallback), Listener);
            }
            catch (Exception ex)
            {
                ErrorReceiveData(ar, InterlocutorIPEndPoint, 0, ex);
            }
        }
        public void ReceiveCallback(IAsyncResult ar)
        {
            try {
                //Socket handler;
                StartReceiveData(ar.AsyncState, Handler.RemoteEndPoint as IPEndPoint, 0);
                // Fetch a user-defined object that contains information 
                // Received byte array 
                byte[] data = (byte[])ar.AsyncState;
                // A Socket to handle remote host communication. 
                // The number of bytes received. 
                int bytesRead = Handler.EndReceive(ar);
                if (bytesRead > 0)
                    EndReceiveData(CollectionService.GetPart(data, 0, bytesRead - 1), Handler.RemoteEndPoint as IPEndPoint, bytesRead);
            }
            catch(Exception ex)
            {
                ErrorReceiveData(ar,InterlocutorIPEndPoint, 0, ex);
            }
        }
    }
}
