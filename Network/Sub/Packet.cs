using System;
using System.Collections.Generic;
using MiMFa.Model;
using System.Drawing;
using MiMFa.Service;
using System.Collections;
using MiMFa.General;

namespace MiMFa.Network
{
    [Serializable]
    public enum SocketPacketSubject
    {
        CheckIn = 0,
        Status = 1,
        Ready = 2,
        Request = 3,
        Send = 4,
        Recieve = 5,
        Add = 6,
        Remove = 7,
        Next = 8,
        Back = 9,
        Start = 10,
        Stop = 11,
        Continue = 12,
        Break = 13,
        Yes = 14,
        No = 15,
        Ok = 16,
        Cancel = 17,
        Ignore = 18,
        Retry = 19,
        Error = 20,
        Success = 21,
        Alert = 22,
        Message = 23,
        Command = 24,
        Chat = 25,
        Mail = 26,
        Exit = 27
    }

    [Serializable]
    public enum SocketPacketDataType
    {
        Null = 0,
        Text = 1,
        Audio = 2,
        Image = 3,
        Video = 4,
        File = 5,
        Folder = 6,
        Mix = 7,
        EXE = 8,
        CMD = 9,
        MCL = 10,
        DataBase = 11,
        Online = 12,
        Packet = 13,
        Object = 14,
        None = 15
    }

    [Serializable]
    public class SimplePacket
    {
        private static int id = 0;
        public int ID = 0;
        public SocketPacketSubject Head = SocketPacketSubject.Send;
        public SocketPacketDataType DataType = SocketPacketDataType.File;
        public object Data = null;
        public object Attach = null;

        public SimplePacket(SocketPacketSubject msps = SocketPacketSubject.CheckIn,object obj = null,SocketPacketDataType dataType = SocketPacketDataType.File,object attach = null)
        {
            ID =  id++ ;
            SetSubject(msps);
            SetDataType(dataType);
            SetData(obj);
            SetAttach(attach);
        }

        public SocketPacketSubject GetSubject()
        {
            return  Head;
        }
        public SocketPacketDataType GetDataType()
        {
            return  DataType;
        }
        public object GetData()
        {
            if (GetDataType() == SocketPacketDataType.Image && Data != null && Data is byte[])
                return  ConvertService.ToImage((byte[])Data);
            else if (Data != null && Data is byte[])
                return IOService.TryDeserialize((byte[])Data);
            else
                return Data;
        }
        public object GetAttach()
        {
            if (Attach != null && Attach is byte[])
                return IOService.TryDeserialize((byte[])Attach);
            else
                return Attach;
        }
        public void SetSubject(SocketPacketSubject msps)
        {
            Head = msps;
        }
        public void SetDataType(SocketPacketDataType dataType)
        {
            DataType = dataType ;
        }
        public void SetData(object obj)
        {
            if (obj != null && obj is byte[])
                Data = (byte[])obj;
            else if (GetDataType() == SocketPacketDataType.Image && InfoService.IsBitmap(obj))
                Data =  ConvertService.ToByteArray((Bitmap)obj);
            else
                Data = obj;
        }
        public void SetAttach(object obj)
        {
            if (obj != null && obj is byte[])
                Attach = (byte[])obj;
            else if ( InfoService.IsBitmap(obj))
                Attach =  ConvertService.ToByteArray((Bitmap)obj);
            else
                Attach = obj;
        }
    }
}
