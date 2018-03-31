using System;
using System.IO;
using System.Net;
using System.Text;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Clover.Core;
using Clover.Core.Common;

namespace Clover.Net
{
    
    
    
    public class UDFFtp
    {
        private string strRemoteHost;
        private int strRemotePort;
        private string strRemotePath;
        private string strRemoteUser;
        private string strRemotePass;
        private Boolean bConnected;

        private readonly object sync = new object();

        #region 内部变量

        
        
        
        private string strMsg;

        
        
        
        private string strReply;

        
        
        
        private int iReplyCode;

        
        
        
        private Socket socketControl;

        
        
        
        private TransferType trType;

        
        
        
        public enum TransferType
        {
            
            
            
            Binary,

            
            
            
            ASCII
        };

        
        
        
        private static int BLOCK_SIZE = 512;

        private Byte[] buffer = new Byte[BLOCK_SIZE];

        
        
        
        private Encoding ASCII = Encoding.Default;

        #endregion

        #region 内部函数

        #region 构造函数

        
        
        
        public UDFFtp(string remoteHost, string remoteUser, string remotePass)
            : this(remoteHost, 21, string.Empty, remoteUser, remotePass)
        {
            
        }

        
        
        
        
        
        
        
        
        public UDFFtp(string remoteHost, int remotePort, string remotePath, string remoteUser, string remotePass)
        {
            strRemoteHost = remoteHost;
            strRemotePath = remotePath;
            strRemoteUser = remoteUser;
            strRemotePass = remotePass;
            strRemotePort = remotePort;
            Connect();
        }

        #endregion

        #region 登陆

        
        
        
        public string RemoteHost
        {
            get { return strRemoteHost; }
            set { strRemoteHost = value; }
        }

        
        
        
        public int RemotePort
        {
            get { return strRemotePort; }
            set { strRemotePort = value; }
        }

        
        
        
        public string RemotePath
        {
            get { return strRemotePath; }
            set { strRemotePath = value; }
        }

        
        
        
        public string RemoteUser
        {
            set { strRemoteUser = value; }
        }

        
        
        
        public string RemotePass
        {
            set { strRemotePass = value; }
        }

        
        
        
        public bool Connected
        {
            get { return bConnected; }
        }

        #endregion

        #region 链接

        
        
        
        public void Connect()
        {
            socketControl = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ep = new IPEndPoint(IPAddress.Parse(RemoteHost), strRemotePort);
            
            try
            {
                socketControl.Connect(ep);
            }
            catch (Exception)
            {
                throw new IOException("Couldn't connect to remote server");
            }

            
            ReadReply();
            
            
            
            
            

            
            SendCommand("USER " + strRemoteUser);
            
            
            
            
            
            if (iReplyCode != 230)
            {
                SendCommand("PASS " + strRemotePass);
                
                
                
                
                
            }
            bConnected = true;

            
            ChDir(strRemotePath);
        }


        
        
        
        public void DisConnect()
        {
            if (socketControl != null)
            {
                SendCommand("QUIT");
            }
            CloseSocketConnect();
        }

        #endregion

        #region 传输模式

        
        
        
        
        public void SetTransferType(TransferType ttType)
        {
            if (ttType == TransferType.Binary)
            {
                SendCommand("TYPE I"); 
            }
            else
            {
                SendCommand("TYPE A"); 
            }
            
            
            
            
            
            
            trType = ttType;
            
        }


        
        
        
        
        public TransferType GetTransferType()
        {
            return trType;
        }

        #endregion

        #region 文件操作

        
        
        
        
        
        public string[] DirFile(string strMask)
        {
            
            if (!bConnected)
            {
                Connect();
            }

            
            Socket socketData = CreateDataSocket();

            
            SendCommand("NLST " + strMask); 

            
            
            
            
            

            
            strMsg = string.Empty;
            while (true)
            {
                int iBytes = socketData.Receive(buffer, buffer.Length, 0);
                strMsg += ASCII.GetString(buffer, 0, iBytes);
                if (iBytes < buffer.Length)
                {
                    break;
                }
            }
            
            string seperator = "\r\n";
            string[] strsFileList = StringHelper.SplitString(strMsg, seperator);
            socketData.Close(); 
            if (iReplyCode != 226)
            {
                ReadReply();
                
                
                
                
            }
            return strsFileList;
        }

        
        
        
        
        public string[] DirDetail()
        {
            
            if (!bConnected)
            {
                Connect();
            }

            
            Socket socketData = CreateDataSocket();

            
            SendCommand("LIST");

            
            
            
            
            

            
            strMsg = string.Empty;
            while (true)
            {
                int iBytes = socketData.Receive(buffer, buffer.Length, 0);
                strMsg += ASCII.GetString(buffer, 0, iBytes);
                if (iBytes < buffer.Length)
                {
                    break;
                }
            }
            char[] seperator = {'\n'};
            string[] strsFileList = strMsg.Split(seperator);
            socketData.Close(); 
            if (iReplyCode != 226)
            {
                ReadReply();
                
                
                
                
            }
            return strsFileList;
        }

        
        
        
        
        public string[] DirDirectory()
        {
            string[] list = DirDetail();
            List<string> dirList = new List<string>();
            foreach (string s in list)
            {
                if (s.Length > 0 && s.ToUpper().IndexOf("<DIR>") >= 0) 
                    dirList.Add(s.Substring(s.IndexOf("<DIR>") + 5).Trim());
            }
            return dirList.ToArray();
        }


        
        
        
        
        
        private long GetFileSize(string strFileName)
        {
            if (!bConnected)
            {
                Connect();
            }
            SendCommand("SIZE " + Path.GetFileName(strFileName));
            long lSize = 0;
            if (iReplyCode == 213)
            {
                lSize = Int64.Parse(strReply.Substring(4));
            }
            
            
            
            
            return lSize;
        }


        
        
        
        
        public void DeleteFiles(string strFileNameMask)
        {
            if (!bConnected)
            {
                Connect();
            }
            string[] strFiles = DirFile(strFileNameMask);
            foreach (string strFile in strFiles)
            {
                if (!strFile.Equals(string.Empty)) 
                {
                    Delete(strFile);
                }
            }
        }

        
        
        
        
        public void Delete(string strFileName)
        {
            if (!bConnected)
            {
                Connect();
            }
            SendCommand("DELE " + strFileName);
            
            
            
            
        }


        
        
        
        
        
        public void Rename(string strOldFileName, string strNewFileName)
        {
            if (!bConnected)
            {
                Connect();
            }
            SendCommand("RNFR " + strOldFileName);
            
            
            
            
            
            SendCommand("RNTO " + strNewFileName);
            
            
            
            
        }

        #endregion

        #region 上传和下载

        
        
        
        
        
        public void Get(string strFolder, string strFileNameMask)
        {
            if (!bConnected)
            {
                Connect();
            }
            string[] strFiles = DirFile(strFileNameMask);
            foreach (string strFile in strFiles)
            {
                if (!strFile.Equals(string.Empty)) 
                {
                    Get(strFile, strFolder, strFile);
                }
            }
        }


        
        
        
        
        
        
        public void Get(string strRemoteFileName, string strFolder, string strLocalFileName)
        {
            if (!bConnected)
            {
                Connect();
            }
            SetTransferType(TransferType.Binary);
            if (strLocalFileName.Equals(string.Empty))
            {
                strLocalFileName = strRemoteFileName;
            }
            if (!File.Exists(strLocalFileName))
            {
                Stream st = File.Create(strLocalFileName);
                st.Close();
            }
            FileStream output = new
                FileStream(strFolder + "\\" + strLocalFileName, FileMode.Create);
            Socket socketData = CreateDataSocket();
            SendCommand("RETR " + strRemoteFileName);
            
            
            
            
            
            while (true)
            {
                int iBytes = socketData.Receive(buffer, buffer.Length, 0);
                output.Write(buffer, 0, iBytes);
                if (iBytes <= 0)
                {
                    break;
                }
            }
            output.Close();
            if (socketData.Connected)
            {
                socketData.Close();
            }
            if (!(iReplyCode == 226 || iReplyCode == 250))
            {
                ReadReply();
                
                
                
                
            }
        }


        
        
        
        
        
        public void Put(string strFolder, string strFileNameMask)
        {
            string[] strFiles = Directory.GetFiles(strFolder, strFileNameMask);
            foreach (string strFile in strFiles)
            {
                
                Put(strFile);
            }
        }


        
        
        
        
        public void Put(string strFileName)
        {
            if (!bConnected)
            {
                Connect();
            }
            Socket socketData = CreateDataSocket();
            SendCommand("STOR " + Path.GetFileName(strFileName));
            
            
            
            
            FileStream input = new
                FileStream(strFileName, FileMode.Open);
            int iBytes = 0;
            while ((iBytes = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                socketData.Send(buffer, iBytes, 0);
            }
            input.Close();
            if (socketData.Connected)
            {
                socketData.Close();
            }
            if (!(iReplyCode == 226 || iReplyCode == 250))
            {
                ReadReply();
                
                
                
                
            }
        }

        #endregion

        #region 目录操作

        
        
        
        
        public void MkDir(string strDirName)
        {
            if (!bConnected)
            {
                Connect();
            }
            SendCommand("MKD " + strDirName);
            
            
            
            
        }


        
        
        
        
        public void RmDir(string strDirName)
        {
            if (!bConnected)
            {
                Connect();
            }
            SendCommand("RMD " + strDirName);
            
            
            
            
        }


        
        
        
        
        public void ChDir(string strDirName)
        {
            if (string.IsNullOrEmpty(strDirName) || strDirName.Equals("."))
            {
                return;
            }
            if (!bConnected)
            {
                Connect();
            }
            SendCommand("CWD " + strDirName);
            
            
            
            
            this.strRemotePath = strDirName;
        }

        
        
        
        
        
        public bool IsCurrentRemotePath(string path)
        {
            bool r = true;
            if (StringHelper.IsEmpty(path)) return r;

            if (RemotePath != path) r = false;

            return r;
        }

        
        
        
        
        public void GotoNewPath(string path)
        {
            if (!IsCurrentRemotePath(path))
            {
                ChDir("\\");
                RemotePath = path;
                ChDir(RemotePath);
            }
        }

        #endregion

        
        
        
        
        private void ReadReply()
        {
            strMsg = string.Empty;
            strReply = ReadLine();
            iReplyCode = Int32.Parse(strReply.Substring(0, 3));
        }

        
        
        
        
        private Socket CreateDataSocket()
        {
            SendCommand("PASV");
            
            
            
            
            int index1 = strReply.IndexOf('(');
            int index2 = strReply.IndexOf(')');

            string ipData = string.Empty;
            if (index1 >= 0 || index2 > index1)
                ipData = strReply.Substring(index1 + 1, index2 - index1 - 1);

            int[] parts = new int[6];
            int len = ipData.Length;
            int partCount = 0;
            string buf = string.Empty;
            for (int i = 0; i < len && partCount <= 6; i++)
            {
                char ch = Char.Parse(ipData.Substring(i, 1));
                if (Char.IsDigit(ch))
                    buf += ch;
                
                
                
                
                
                if (ch == ',' || i + 1 == len)
                {
                    try
                    {
                        parts[partCount++] = Int32.Parse(buf);
                        buf = string.Empty;
                    }
                    catch (Exception)
                    {
                        throw new IOException("Malformed PASV strReply: " +
                                              strReply);
                    }
                }
            }
            string ipAddress = parts[0] + "." + parts[1] + "." +
                               parts[2] + "." + parts[3];
            int port = (parts[4] << 8) + parts[5];
            Socket s = new
                Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPEndPoint ep = new
                IPEndPoint(IPAddress.Parse(ipAddress), port);
            try
            {
                s.Connect(ep);
            }
            catch (Exception ex)
            {
                throw ex;
                
            }
            return s;
        }


        
        
        
        private void CloseSocketConnect()
        {
            if (socketControl != null)
            {
                socketControl.Close();
                socketControl = null;
            }
            bConnected = false;
        }

        
        
        
        
        private string ReadLine()
        {
            
            while (true)
            {
                int iBytes = socketControl.Receive(buffer, buffer.Length, 0);
                strMsg += ASCII.GetString(buffer, 0, iBytes);
                if (iBytes < buffer.Length)
                {
                    break;
                }
            }
            char[] seperator = {'\n'};
            string[] mess = strMsg.Split(seperator);
            if (strMsg.Length > 2)
            {
                strMsg = mess[mess.Length - 2];
                
                
                
                
            }
            else
            {
                strMsg = mess[0];
            }
            if (!strMsg.Substring(3, 1).Equals(" ")) 
            {
                return ReadLine();
            }
            return strMsg;
        }


        
        
        
        
        private void SendCommand(string strCommand)
        {
            Byte[] cmdBytes = ASCII.GetBytes((strCommand + "\r\n").ToCharArray());
            socketControl.Send(cmdBytes, cmdBytes.Length, 0);
            ReadReply();
        }

        #endregion
    }
}