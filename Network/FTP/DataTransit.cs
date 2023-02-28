using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MiMFa.Network.FTP
{
    public class DataTransit
    {
        public event FTPEventHandler LogInToInterlocutor = (ip) => { };
        public event FTPTransitDataEventHandler StartDownloadData = (l, r, ip) => { };
        public event FTPTransitDataEventHandler EndDownloadData = (l,r, ip) => { };
        public event FTPTransitDataEventHandler StartUploadData = (l,r, ip) => { };
        public event FTPTransitDataEventHandler EndUploadData = (l,r, ip) => { };

        public string InterlocutorAddress { get; set; } = null;
        public string UserName { get; set; } = null;
        public string Password { get; set; } = null;
        public FtpWebRequest FtpRequest { get; set; } = null;
        public FtpWebResponse FtpResponse { get; set; } = null;
        public Stream FtpStream { get; set; } = null;
        public int BufferSize { get; set; } = 1024 * 1024;

        /* Construct Object */
        public DataTransit(string hostIP, string userName, string password) { InterlocutorAddress = hostIP; UserName = userName; Password = password; }

        /* Download File */
        public void Download(string localFile, string remoteFile)
        {
            try
            {
                /* Create an FTP Request */
                FtpRequest = (FtpWebRequest)FtpWebRequest.Create(InterlocutorAddress + "/" + remoteFile);
                /* Log in to the FTP Server with the User Name and Password Provided */
                FtpRequest.Credentials = new NetworkCredential(UserName, Password);
                LogInToInterlocutor(InterlocutorAddress);
                /* When in doubt, use these options */
                FtpRequest.UseBinary = true;
                FtpRequest.UsePassive = true;
                FtpRequest.KeepAlive = true;
                /* Specify the Type of FTP Request */
                FtpRequest.Method = WebRequestMethods.Ftp.DownloadFile;
                /* Establish Return Communication with the FTP Server */
                FtpResponse = (FtpWebResponse)FtpRequest.GetResponse();
                /* Get the FTP Server's Response Stream */
                StartDownloadData(localFile,remoteFile,InterlocutorAddress);
                FtpStream = FtpResponse.GetResponseStream();
                /* Open a File Stream to Write the Downloaded File */
                FileStream localFileStream = new FileStream(localFile, FileMode.Create);
                /* Buffer for the Downloaded Data */
                byte[] byteBuffer = new byte[BufferSize];
                int bytesRead = FtpStream.Read(byteBuffer, 0, BufferSize);
                /* Download the File by Writing the Buffered Data Until the Transfer is Complete */
                try
                {
                    while (bytesRead > 0)
                    {
                        localFileStream.Write(byteBuffer, 0, bytesRead);
                        bytesRead = FtpStream.Read(byteBuffer, 0, BufferSize);
                    }
                }
                finally
                { /* Resource Cleanup */
                    localFileStream.Close();
                EndDownloadData(localFile,remoteFile,InterlocutorAddress);
                }
            }
            finally
            {
                FtpStream.Close();
                FtpResponse.Close();
                FtpRequest = null;
            }
        }

        /* Upload File */
        public void Upload(string localFile, string remoteFile)
        {
            try
            {
                /* Create an FTP Request */
                FtpRequest = (FtpWebRequest)FtpWebRequest.Create(InterlocutorAddress + "/" + remoteFile);
                /* Log in to the FTP Server with the User Name and Password Provided */
                FtpRequest.Credentials = new NetworkCredential(UserName, Password);
                LogInToInterlocutor(InterlocutorAddress);
                /* When in doubt, use these options */
                FtpRequest.UseBinary = true;
                FtpRequest.UsePassive = true;
                FtpRequest.KeepAlive = true;
                /* Specify the Type of FTP Request */
                FtpRequest.Method = WebRequestMethods.Ftp.UploadFile;
                /* Establish Return Communication with the FTP Server */
                StartUploadData(localFile, remoteFile, InterlocutorAddress);
                FtpStream = FtpRequest.GetRequestStream();
                /* Open a File Stream to Read the File for Upload */
                FileStream localFileStream = new FileStream(localFile, FileMode.Create);
                /* Buffer for the Downloaded Data */
                byte[] byteBuffer = new byte[BufferSize];
                int bytesSent = localFileStream.Read(byteBuffer, 0, BufferSize);
                /* Upload the File by Sending the Buffered Data Until the Transfer is Complete */
                try
                {
                    while (bytesSent != 0)
                    {
                        FtpStream.Write(byteBuffer, 0, bytesSent);
                        bytesSent = localFileStream.Read(byteBuffer, 0, BufferSize);
                    }
                }
                finally
                {
                    /* Resource Cleanup */
                    localFileStream.Close();
                EndUploadData(localFile,remoteFile,InterlocutorAddress);
                }
            }
            finally
            {
                FtpStream.Close();
                FtpRequest = null;
            }
        }

        /* Delete File */
        public void Delete(string deleteFile)
        {
            try
            {
                /* Create an FTP Request */
                FtpRequest = (FtpWebRequest)WebRequest.Create(InterlocutorAddress + "/" + deleteFile);
                /* Log in to the FTP Server with the User Name and Password Provided */
                FtpRequest.Credentials = new NetworkCredential(UserName, Password);
                LogInToInterlocutor(InterlocutorAddress);
                /* When in doubt, use these options */
                FtpRequest.UseBinary = true;
                FtpRequest.UsePassive = true;
                FtpRequest.KeepAlive = true;
                /* Specify the Type of FTP Request */
                FtpRequest.Method = WebRequestMethods.Ftp.DeleteFile;
                /* Establish Return Communication with the FTP Server */
                FtpResponse = (FtpWebResponse)FtpRequest.GetResponse();
            }
            finally
            {
                /* Resource Cleanup */
                FtpResponse.Close();
                FtpRequest = null;
            }
        }

        /* Rename File */
        public void Rename(string currentFileNameAndPath, string newFileName)
        {
            try
            {
                /* Create an FTP Request */
                FtpRequest = (FtpWebRequest)WebRequest.Create(InterlocutorAddress + "/" + currentFileNameAndPath);
                /* Log in to the FTP Server with the User Name and Password Provided */
                FtpRequest.Credentials = new NetworkCredential(UserName, Password);
                LogInToInterlocutor(InterlocutorAddress);
                /* When in doubt, use these options */
                FtpRequest.UseBinary = true;
                FtpRequest.UsePassive = true;
                FtpRequest.KeepAlive = true;
                /* Specify the Type of FTP Request */
                FtpRequest.Method = WebRequestMethods.Ftp.Rename;
                /* Rename the File */
                FtpRequest.RenameTo = newFileName;
                /* Establish Return Communication with the FTP Server */
                FtpResponse = (FtpWebResponse)FtpRequest.GetResponse();
                /* Resource Cleanup */
                FtpResponse.Close();
                FtpRequest = null;
            }
            finally
            {
                /* Resource Cleanup */
                FtpResponse.Close();
                FtpRequest = null;
            }
        }

        /* Create a New Directory on the FTP Server */
        public void CreateDirectory(string newDirectory)
        {
            try
            {
                /* Create an FTP Request */
                FtpRequest = (FtpWebRequest)WebRequest.Create(InterlocutorAddress + "/" + newDirectory);
                /* Log in to the FTP Server with the User Name and Password Provided */
                FtpRequest.Credentials = new NetworkCredential(UserName, Password);
                LogInToInterlocutor(InterlocutorAddress);
                /* When in doubt, use these options */
                FtpRequest.UseBinary = true;
                FtpRequest.UsePassive = true;
                FtpRequest.KeepAlive = true;
                /* Specify the Type of FTP Request */
                FtpRequest.Method = WebRequestMethods.Ftp.MakeDirectory;
                /* Establish Return Communication with the FTP Server */
                FtpResponse = (FtpWebResponse)FtpRequest.GetResponse();
                /* Resource Cleanup */
            }
            finally
            {
                /* Resource Cleanup */
                FtpResponse.Close();
                FtpRequest = null;
            }
        }

        /* Get the Date/Time a File was Created */
        public string GetFileCreatedDateTime(string fileName)
        {
            try
            {
                /* Create an FTP Request */
                FtpRequest = (FtpWebRequest)FtpWebRequest.Create(InterlocutorAddress + "/" + fileName);
                /* Log in to the FTP Server with the User Name and Password Provided */
                FtpRequest.Credentials = new NetworkCredential(UserName, Password);
                LogInToInterlocutor(InterlocutorAddress);
                /* When in doubt, use these options */
                FtpRequest.UseBinary = true;
                FtpRequest.UsePassive = true;
                FtpRequest.KeepAlive = true;
                /* Specify the Type of FTP Request */
                FtpRequest.Method = WebRequestMethods.Ftp.GetDateTimestamp;
                /* Establish Return Communication with the FTP Server */
                FtpResponse = (FtpWebResponse)FtpRequest.GetResponse();
                /* Establish Return Communication with the FTP Server */
                FtpStream = FtpResponse.GetResponseStream();
                /* Get the FTP Server's Response Stream */
                StreamReader ftpReader = new StreamReader(FtpStream);
                /* Store the Raw Response */
                string fileInfo = null;
                /* Read the Full Response Stream */
                try { fileInfo = ftpReader.ReadToEnd(); }
                finally
                {
                    /* Resource Cleanup */
                    ftpReader.Close();
                }
                return fileInfo;
            }
            finally
            {
                FtpStream.Close();
                FtpResponse.Close();
                FtpRequest = null;
                /* Return File Created Date Time */
            }
        }

        /* Get the Size of a File */
        public string GetFileSize(string fileName)
        {
            try
            {
                /* Create an FTP Request */
                FtpRequest = (FtpWebRequest)FtpWebRequest.Create(InterlocutorAddress + "/" + fileName);
                /* Log in to the FTP Server with the User Name and Password Provided */
                FtpRequest.Credentials = new NetworkCredential(UserName, Password);
                LogInToInterlocutor(InterlocutorAddress);
                /* When in doubt, use these options */
                FtpRequest.UseBinary = true;
                FtpRequest.UsePassive = true;
                FtpRequest.KeepAlive = true;
                /* Specify the Type of FTP Request */
                FtpRequest.Method = WebRequestMethods.Ftp.GetFileSize;
                /* Establish Return Communication with the FTP Server */
                FtpResponse = (FtpWebResponse)FtpRequest.GetResponse();
                /* Establish Return Communication with the FTP Server */
                FtpStream = FtpResponse.GetResponseStream();
                /* Get the FTP Server's Response Stream */
                StreamReader ftpReader = new StreamReader(FtpStream);
                /* Store the Raw Response */
                string fileInfo = null;
                /* Read the Full Response Stream */
                try { while (ftpReader.Peek() != -1) { fileInfo = ftpReader.ReadToEnd(); } }
                finally
                { 
                    /* Resource Cleanup */
                    ftpReader.Close();
                }
               
                /* Return File Size */
                return fileInfo;
            }
            finally
            {
                FtpStream.Close();
                FtpResponse.Close();
                FtpRequest = null;
            }
        }

        /* List Directory Contents File/Folder Name Only */
        public string[] DirectoryListSimple(string directory)
        {
            try
            {
                /* Create an FTP Request */
                FtpRequest = (FtpWebRequest)FtpWebRequest.Create(InterlocutorAddress + "/" + directory);
                /* Log in to the FTP Server with the User Name and Password Provided */
                FtpRequest.Credentials = new NetworkCredential(UserName, Password);
                LogInToInterlocutor(InterlocutorAddress);
                /* When in doubt, use these options */
                FtpRequest.UseBinary = true;
                FtpRequest.UsePassive = true;
                FtpRequest.KeepAlive = true;
                /* Specify the Type of FTP Request */
                FtpRequest.Method = WebRequestMethods.Ftp.ListDirectory;
                /* Establish Return Communication with the FTP Server */
                FtpResponse = (FtpWebResponse)FtpRequest.GetResponse();
                /* Establish Return Communication with the FTP Server */
                FtpStream = FtpResponse.GetResponseStream();
                /* Get the FTP Server's Response Stream */
                StreamReader ftpReader = new StreamReader(FtpStream);
                /* Store the Raw Response */
                string directoryRaw = null;
                /* Read Each Line of the Response and Append a Pipe to Each Line for Easy Parsing */
                try { while (ftpReader.Peek() != -1) { directoryRaw += ftpReader.ReadLine() + "|"; } }
               finally
                {    /* Resource Cleanup */
                    ftpReader.Close();
                }
                /* Return the Directory Listing as a string Array by Parsing 'directoryRaw' with the Delimiter you Append (I use | in This Example) */
                return directoryRaw.Split("|".ToCharArray());
            }
            finally
            {
                FtpStream.Close();
                FtpResponse.Close();
                FtpRequest = null;
            }
        }

        /* List Directory Contents in Detail (Name, Size, Created, etc.) */
        public string[] DirectoryListDetailed(string directory)
        {
            try
            {
                /* Create an FTP Request */
                FtpRequest = (FtpWebRequest)FtpWebRequest.Create(InterlocutorAddress + "/" + directory);
                /* Log in to the FTP Server with the User Name and Password Provided */
                FtpRequest.Credentials = new NetworkCredential(UserName, Password);
                LogInToInterlocutor(InterlocutorAddress);
                /* When in doubt, use these options */
                FtpRequest.UseBinary = true;
                FtpRequest.UsePassive = true;
                FtpRequest.KeepAlive = true;
                /* Specify the Type of FTP Request */
                FtpRequest.Method = WebRequestMethods.Ftp.ListDirectoryDetails;
                /* Establish Return Communication with the FTP Server */
                FtpResponse = (FtpWebResponse)FtpRequest.GetResponse();
                /* Establish Return Communication with the FTP Server */
                FtpStream = FtpResponse.GetResponseStream();
                /* Get the FTP Server's Response Stream */
                StreamReader ftpReader = new StreamReader(FtpStream);
                /* Store the Raw Response */
                string directoryRaw = null;
                /* Read Each Line of the Response and Append a Pipe to Each Line for Easy Parsing */
                try { while (ftpReader.Peek() != -1) { directoryRaw += ftpReader.ReadLine() + "|"; } }
                finally
                {  /* Resource Cleanup */
                    ftpReader.Close();
                }
                /* Return the Directory Listing as a string Array by Parsing 'directoryRaw' with the Delimiter you Append (I use | in This Example) */
               return directoryRaw.Split("|".ToCharArray());
            }
            finally
            {
                FtpStream.Close();
                FtpResponse.Close();
                FtpRequest = null;
            }
        }
    }
}
