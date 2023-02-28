using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MiMFa.Network
{
    public delegate void FTPEventHandler(string hostIP);
    public delegate void FTPAddressRecieveEventHandler(string address);
    public delegate void FTPTransitDataEventHandler(string localAddress,string remoteAddress, string hostIP);
    public delegate void SocketEventHandler(IPEndPoint ip);
    public delegate void TransitBinaryEventHandler(byte[] data, IPEndPoint ip, long length);
    public delegate void TransitDataEventHandler(object data, IPEndPoint ip, long length);
    public delegate void TransitSimplePacketEventHandler(SimplePacket data, IPEndPoint ip);
    public delegate void TransitBinaryErrorEventHandler(byte[] data, IPEndPoint ip, long length,Exception ex);
    public delegate void TransitDataErrorEventHandler(object data, IPEndPoint ip, long length,Exception ex);
}
