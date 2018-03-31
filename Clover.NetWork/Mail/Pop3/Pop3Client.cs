using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.IO;
using System.Collections.Generic;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using G = System.Collections.Generic;
using Clover.Net.Mail;
using Clover.Net.Command;

namespace Clover.Net.Pop3
{
	
	
	
	
	public class Pop3Client : IDisposable
	{
		private const int MaxBufferReadSize = 128;
		private Pop3AuthenticateMode _Mode = Pop3AuthenticateMode.Pop;
		private String _UserName = "";
		private String _Password = "";
		private String _ServerName = "";
		private Int32 _Port = 110;
		private Boolean _Ssl = false;
		private Int32 _ReceiveTimeout = 10 * 1000;
		private Int32 _SendBufferSize = 8192;
		private Int32 _ReceiveBufferSize = 8192;
		private Encoding _ResponseEncoding = Encoding.ASCII;
		private TcpClient _TcpClient = null;
		private Stream _Stream = null;
		private AutoResetEvent _SendCommandDone = new AutoResetEvent(false);
		private AutoResetEvent _GetResponseDone = new AutoResetEvent(false);
		private Pop3ConnectionState _State = Pop3ConnectionState.Disconnected;
		private Boolean _Commnicating = false;
		private RemoteCertificateValidationCallback _RemoteCertificateValidationCallback = Pop3Client.DefaultRemoteCertificateValidationCallback;
		
		
		
		
		
		public Pop3AuthenticateMode AuthenticateMode
		{
			get { return this._Mode; }
			set { this._Mode = value; }
		}
		
		
		
		
		
		public String UserName
		{
			get { return this._UserName; }
			set { this._UserName = value; }
		}
		
		
		
		
		
		public String Password
		{
			get { return this._Password; }
			set { this._Password = value; }
		}
		
		
		
		
		
		public String ServerName
		{
			get { return this._ServerName; }
			set { this._ServerName = value; }
		}
		
		
		
		
		
		public Int32 Port
		{
			get { return this._Port; }
			set { this._Port = value; }
		}
		
		
		
		
		
		public Boolean Ssl
		{
			get { return this._Ssl; }
			set { this._Ssl = value; }
		}
		
		
		
		
		
		public Int32 ReceiveTimeout
		{
			get { return this._ReceiveTimeout; }
			set
			{
				this._ReceiveTimeout = value;
				if (this._TcpClient != null)
				{
					this._TcpClient.ReceiveTimeout = this._ReceiveTimeout;
				}
			}
		}
		
		
		
		
		
		public Int32 SendBufferSize
		{
			get { return this._SendBufferSize; }
			set
			{
				this._SendBufferSize = value;
				if (this._TcpClient != null)
				{
					this._TcpClient.SendBufferSize = this._SendBufferSize;
				}
			}
		}
		
		
		
		
		
		public Int32 ReceiveBufferSize
		{
			get { return this._ReceiveBufferSize; }
			set
			{
				this._ReceiveBufferSize = value;
				if (this._TcpClient != null)
				{
					this._TcpClient.ReceiveBufferSize = this._ReceiveBufferSize;
				}
			}
		}
        
        
        
        
        public Encoding ResponseEncoding
        {
            get { return _ResponseEncoding; }
            set { _ResponseEncoding = value; }
        }
        
		
		
		
		
		public Pop3ConnectionState State
		{
            get
            {
                if (this._TcpClient == null ||
                    this._TcpClient.Connected == false)
                {
                    this._State = Pop3ConnectionState.Disconnected;
                }
                return this._State;
            }
		}
		
		
		
		
		
		public Boolean Available
		{
			get { return this._State != Pop3ConnectionState.Disconnected; }
		}
		
		
		
		
		
		
		
		public Boolean Commnicating
		{
			get { return this._Commnicating; }
		}
		
		
		
		
		public RemoteCertificateValidationCallback RemoteCertificateValidationCallback
		{
			get { return this._RemoteCertificateValidationCallback; }
			set { this._RemoteCertificateValidationCallback = value; }
		}
		
		
		
		public Pop3Client()
		{
		}
		
		
		
		
		
		
		public Pop3Client(String userName, String password, String serverName)
		{
			this._UserName = userName;
			this._Password = password;
			this._ServerName = serverName;
		}
		
		
		
		
		
		
		private TcpClient GetTcpClient()
		{
			TcpClient tc = null;
			IPHostEntry hostEntry = null;

			
			hostEntry = this.GetHostEntry();
			
			if (hostEntry != null)
			{
				foreach (IPAddress address in hostEntry.AddressList)
				{
					tc = this.TryGetTcpClient(address);
					if (tc != null) { break; }
				}
			}
			if (tc == null)
			{
				this._State = Pop3ConnectionState.Disconnected;
				return null;
			}
			this._State = Pop3ConnectionState.Connected;
			return tc;
		}
		private IPHostEntry GetHostEntry()
		{
			try
			{
				return Dns.GetHostEntry(this._ServerName);
			}
			catch { }
			return null;
		}
		private TcpClient TryGetTcpClient(IPAddress address)
		{
			IPEndPoint ipe = new IPEndPoint(address, this._Port);
			TcpClient tc = null;

			try
			{
				tc = new TcpClient(ipe.AddressFamily);
				tc.Connect(ipe);
				if (tc.Connected == true)
				{
					tc.ReceiveTimeout = this._ReceiveTimeout;
					tc.SendBufferSize = this._SendBufferSize;
					tc.ReceiveBufferSize = this._ReceiveBufferSize;
				}
			}
			catch
			{
				tc = null;
			}
			return tc;
		}
		private static Boolean DefaultRemoteCertificateValidationCallback(Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			return true;
		}
		
		
		
		
		
		public Pop3ConnectionState Open()
		{
			this._TcpClient = this.GetTcpClient();
			if (this._TcpClient == null)
			{
				this._State = Pop3ConnectionState.Disconnected;
			}
			else
			{
				if (this._Ssl == true)
				{
					SslStream ssl = new SslStream(this._TcpClient.GetStream(), false, this.RemoteCertificateValidationCallback);
					ssl.AuthenticateAsClient(this._ServerName);
					if (ssl.IsAuthenticated == false)
					{
						this._State = Pop3ConnectionState.Disconnected;
						this._TcpClient = null;
						return this._State;
					}
					this._Stream = ssl;
				}
				else
				{
					this._Stream = this._TcpClient.GetStream();
				}
				if (this.GetResponse().StartsWith("+OK") == true)
				{
					this._State = Pop3ConnectionState.Connected;
				}
				else
				{
					this._TcpClient = null;
					this._State = Pop3ConnectionState.Disconnected;
				}
			}
			return this._State;
		}
		
		
		
		
		
		public Pop3ConnectionState EnsureOpen()
		{
			if (this._TcpClient != null)
			{ return this._State; }

			return this.Open();
		}
		private void CheckAuthenticate()
		{
			if (this._State == Pop3ConnectionState.Authenticated) { return; }
			throw new Pop3Exception("You must authenticate to pop3 server before executing this command.");
		}
		private void SendCommand(String command)
		{
			this.SendCommand(Encoding.ASCII.GetBytes(command + MailParser.NewLine));
		}
		private void SendCommand(Byte[] bytes)
		{
			AsynchronousSendContext cx = null;

			if (this._TcpClient == null)
			{
				throw new Pop3ConnectException("Pop3 connection is closed");
			}
			try
			{
				cx = new AsynchronousSendContext(Encoding.ASCII, bytes);
				cx.FillBuffer();
				_Stream.BeginWrite(cx.Buffer.Array, 0, cx.SendBufferSize, this.SendCommandCallback, cx);
				_SendCommandDone.WaitOne();
            }
			catch (Exception ex)
			{
				throw new Pop3SendException(ex);
			}
			finally
			{
				if (cx != null)
				{
					cx.Dispose();
				}
			}
            
            if (cx.Exception != null)
            {
                throw cx.Exception;
            }
        }
		private void SendCommandCallback(IAsyncResult result)
		{
            AsynchronousSendContext cx = null;
            try
            {
                cx = (AsynchronousSendContext)result.AsyncState;
                _Stream.EndWrite(result);
                if (cx.IsSendAllBytes == true)
                {
                    cx.FillBuffer();
                    _Stream.BeginWrite(cx.Buffer.Array, 0, cx.SendBufferSize, this.SendCommandCallback, cx);
                }
                else
                {
                    _SendCommandDone.Set();
                }
            }
            catch (Exception ex)
            {
                cx.Exception = ex;
            }
            if (cx.Exception != null)
            {
                try
                {
                    _SendCommandDone.Set();
                }
                catch (ObjectDisposedException) { }
            }
		}
		private String GetResponse()
		{
			return this.GetResponse(false);
		}
		private String GetResponse(Boolean isMultiLine)
		{
			MemoryStream ms = new MemoryStream();
			this.GetResponse(ms, isMultiLine);
			return this._ResponseEncoding.GetString(ms.ToArray());
		}
		private void GetResponse(Stream stream)
		{
			this.GetResponse(stream, false);
		}
		private void GetResponse(Stream stream, Boolean isMultiLine)
		{
			if (this._TcpClient == null)
			{
				throw new Pop3ConnectException("Connection to POP3 server is closed");
			}
			using (var cx = new AsynchronousPop3ResponseContext(_ResponseEncoding, isMultiLine))
			{
				_Stream.BeginRead(cx.Buffer.Array, 0, cx.Buffer.Array.Length, this.GetResponseCallback, cx);
				var bl = _GetResponseDone.WaitOne(_ReceiveTimeout);
                if (cx.Exception != null)
                {
                    throw cx.Exception;
                }
				if (cx.Timeout == true || bl == false)
				{
					throw new Pop3ReceiveException("Response timeout");
				}
				this.ReadText(stream, isMultiLine, cx.Data.ToArray());
			}
			this._Commnicating = false;
		}
		private void GetResponseCallback(IAsyncResult result)
		{
            AsynchronousPop3ResponseContext cx = null;

            try
            {
                cx = (AsynchronousPop3ResponseContext)result.AsyncState;
                if (this._TcpClient == null)
                {
                    throw new Pop3ConnectException("Connection to POP3 server is closed");
                }
                Int32 size = _Stream.EndRead(result);
                TimeSpan ts = DateTime.Now - cx.StartTime;

                if (ts.TotalMilliseconds > this._ReceiveTimeout)
                {
                    cx.Timeout = true;
                    _GetResponseDone.Set();
                }
                if (cx.ReadBuffer(size) == true)
                {
                    _Stream.BeginRead(cx.Buffer.Array, 0, cx.Buffer.Array.Length, this.GetResponseCallback, cx);
                }
                else
                {
                    _GetResponseDone.Set();
                }
            }
            catch(Exception ex)
            {
                cx.Exception = ex;
            }
            if (cx.Exception != null)
            {
                try
                {
                    _GetResponseDone.Set();
                }
                catch (ObjectDisposedException) { }
            }
        }
		private void ReadText(Stream stream, Boolean isMultiLine, Byte[] bytes)
		{
			String CurrentLine = "";
			Byte[] bb = null;
			Int64 LineIndex = 0;
			StringReader sr = new StringReader(_ResponseEncoding.GetString(bytes));
			while (true)
			{
				CurrentLine = sr.ReadLine();
				if (CurrentLine == null) { CurrentLine = ""; }
				bb = _ResponseEncoding.GetBytes(CurrentLine + MailParser.NewLine);
				stream.Write(bb, 0, bb.Length);

				LineIndex += 1;
				
				if (isMultiLine == false)
				{ break; }
				
				if (CurrentLine == ".")
				{ break; }
			}
		}
		
		
		
		
		
		
		public Boolean Authenticate()
		{
			if (this._Mode == Pop3AuthenticateMode.Auto)
			{
				if (this.AuthenticateByPop() == true)
				{
					this._Mode = Pop3AuthenticateMode.Pop;
					return true;
				}
				else if (this.AuthenticateByAPop() == true)
				{
					this._Mode = Pop3AuthenticateMode.APop;
					return true;
				}
				return false;
			}
			else
			{
				switch (this._Mode)
				{
					case Pop3AuthenticateMode.Pop: return this.AuthenticateByPop();
					case Pop3AuthenticateMode.APop: return this.AuthenticateByAPop();
				}
			}
			return false;
		}
		
		
		
		
		
		
		public Boolean AuthenticateByPop()
		{
			String s = "";

			if (this.EnsureOpen() == Pop3ConnectionState.Connected)
			{
				
				s = this.Execute("user " + this._UserName, false);
				if (s.StartsWith("+OK") == true)
				{
					
					s = this.Execute("pass " + this._Password, false);
					if (s.StartsWith("+OK") == true)
					{
						this._State = Pop3ConnectionState.Authenticated;
					}
				}
			}
			return this._State == Pop3ConnectionState.Authenticated;
		}
		
		
		
		
		
		
		public Boolean AuthenticateByAPop()
		{
			String s = "";
			String TimeStamp = "";
			Int32 StartIndex = 0;
			Int32 EndIndex = 0;

			if (this.EnsureOpen() == Pop3ConnectionState.Connected)
			{
				
				s = this.Execute("user " + this._UserName, false);
				if (s.StartsWith("+OK") == true)
				{
					if (s.IndexOf("<") > -1 &&
						s.IndexOf(">") > -1)
					{
						StartIndex = s.IndexOf("<");
						EndIndex = s.IndexOf(">");
						TimeStamp = s.Substring(StartIndex, EndIndex - StartIndex + 1);
						
						s = this.Execute("pass " + MailParser.ToMd5DigestString(TimeStamp + this._Password), false);
						if (s.StartsWith("+OK") == true)
						{
							this._State = Pop3ConnectionState.Authenticated;
						}
					}
				}
			}
			return this._State == Pop3ConnectionState.Authenticated;
		}
		
		
		
		
		
		
		
		public String Execute(Pop3Command command)
		{
			Boolean IsResponseMultiLine = false;

			if (command is Top ||
				command is Retr ||
				command is List ||
				command is Uidl)
			{
				IsResponseMultiLine = true;
			}
			return this.Execute(command.GetCommandString(), IsResponseMultiLine);
		}
		
		
		
		
		
		
		
		
		private String Execute(String command, Boolean isMultiLine)
		{
			this.SendCommand(command);
			this._Commnicating = true;
			return this.GetResponse(isMultiLine);
		}
		
		
		
		
		
		
		
		
		public void Execute(Stream stream, Pop3Command command)
		{
			Boolean IsResponseMultiLine = false;

			if (command is Top ||
				command is Retr ||
				command is List ||
				command is Uidl)
			{
				IsResponseMultiLine = true;
			}
			this.Execute(stream, command.GetCommandString(), IsResponseMultiLine);
		}
		
		
		
		
		
		
		
		
		
		private void Execute(Stream stream, String command, Boolean isMultiLine)
		{
			this.SendCommand(command);
			this._Commnicating = true;
			this.GetResponse(stream, isMultiLine);
		}
		
		
		
		
		
		
		
		public void BeginExecute(Pop3Command command, EndGetResponse callbackFunction)
		{
			AsynchronousContext cx = null;
			Boolean IsResponseMultiLine = false;
			Boolean IsException = false;

			if (command is Top ||
				command is Retr ||
				command is List ||
				command is Uidl)
			{
				IsResponseMultiLine = true;
			}
			try
			{
				cx = new AsynchronousPop3ResponseContext(_ResponseEncoding, IsResponseMultiLine, callbackFunction);
				this.SendCommand(command.GetCommandString());
				this._Stream.BeginRead(cx.Buffer.Array, 0, cx.Buffer.Array.Length, this.BeginExecuteCallBack, cx);
			}
			catch
			{
				IsException = true;
				throw;
			}
			finally
			{
				if (IsException == true && cx != null)
				{
					cx.Dispose();
				}
			}
		}
		
		
		
		
		
		
		
		
		
		private void BeginExecuteCallBack(IAsyncResult result)
		{
			AsynchronousPop3ResponseContext cx = (AsynchronousPop3ResponseContext)result.AsyncState;
			Boolean IsException = false;

			try
			{
				Int32 size = this._Stream.EndRead(result);
				if (cx.ReadBuffer(size) == true)
				{
					
					this._Stream.BeginRead(cx.Buffer.Array, 0, cx.Buffer.Array.Length, this.BeginExecuteCallBack, cx);
				}
				else
				{
					cx.OnEndGetResponse();
					cx.Dispose();
				}
			}
			catch
			{
				IsException = true;
				throw;
			}
			finally
			{
				if (IsException == true && cx != null)
				{
					cx.Dispose();
				}
			}
		}
		
		
		
		
		
		
		
		public List<List.Result> ExecuteList(List command)
		{
			List<List.Result> l = new List<List.Result>();
			if (command.MailIndex.HasValue == true)
			{
				var rs = this.ExecuteList(command.MailIndex.Value);
				l.Add(rs);
			}
			else
			{
				l = this.ExecuteList();
			}
			return l;
		}
		
		
		
		
		
		
		
		public List.Result ExecuteList(Int64 mailIndex)
		{
			List cm = new List(mailIndex);
			List.Result rs = null;
			String s = "";

			this.CheckAuthenticate();
			s = this.Execute(cm);
			rs = new List.Result(s);
			return rs;
		}
		
		
		
		
		
		
		public List<List.Result> ExecuteList()
		{
			List cm = new List();
			List<List.Result> l = new List<List.Result>();
			StringReader sr = null;
			String s = "";
			String line = "";

			this.CheckAuthenticate();
			s = this.Execute(cm);
			sr = new StringReader(s);
			while (sr.Peek() > -1)
			{
				line = sr.ReadLine();
				if (line == ".")
				{ break; }
				if (line.StartsWith("+OK", StringComparison.InvariantCultureIgnoreCase) == true)
				{ continue; }

				l.Add(new List.Result(line));
			}
			return l;
		}
		
		
		
		
		
		
		
		public Uidl.Result ExecuteUidl(Int64 mailIndex)
		{
			Uidl cm = new Uidl(mailIndex);
			Uidl.Result rs = null;
			String s = "";

			this.CheckAuthenticate();
			s = this.Execute(cm);
			if (Uidl.Result.CheckFormat(s) == true)
			{
				rs = new Uidl.Result(s);
			}
			return rs;
		}
		
		
		
		
		
		
		public List<Uidl.Result> ExecuteUidl()
		{
			Uidl cm = new Uidl();
			List<Uidl.Result> l = new List<Uidl.Result>();
			StringReader sr = null;
			String s = "";
			String line = "";

			this.CheckAuthenticate();
			s = this.Execute(cm);
			sr = new StringReader(s);
			while (sr.Peek() > -1)
			{
				line = sr.ReadLine();
				if (line == ".")
				{ break; }
				if (line.StartsWith("+OK", StringComparison.InvariantCultureIgnoreCase) == true)
				{ continue; }
				if (Uidl.Result.CheckFormat(line) == false)
				{ continue; }

				l.Add(new Uidl.Result(line));
			}
			return l;
		}
		
		
		
		
		
		
		
		public Pop3Message ExecuteRetr(Int64 mailIndex)
		{
			return this.GetMessage(mailIndex, Int32.MaxValue);
		}
		
		
		
		
		
		
		
		
		public Pop3Message ExecuteTop(Int64 mailIndex, Int32 lineCount)
		{
			return this.GetMessage(mailIndex, lineCount);
		}
		
		
		
		
		
		
		
		public Pop3CommandResult ExecuteDele(Int64 mailIndex)
		{
			Dele cm = new Dele(mailIndex);
			Pop3CommandResult rs = null;
			String s = "";

			this.CheckAuthenticate();
			s = this.Execute(cm);
			rs = new Pop3CommandResult(s);
			return rs;
		}
		
		
		
		
		
		
		public Stat.Result ExecuteStat()
		{
			Stat cm = new Stat();
			Stat.Result rs = null;
			String s = "";

			this.CheckAuthenticate();
			s = this.Execute(cm);
			rs = new Stat.Result(s);
			return rs;
		}
		
		
		
		
		
		
		public Pop3CommandResult ExecuteNoop()
		{
			Noop cm = new Noop();
			Pop3CommandResult rs = null;
			String s = "";

			this.EnsureOpen();
			s = this.Execute(cm);
			rs = new Pop3CommandResult(s);
			return rs;
		}
		
		
		
		
		
		
		public Pop3CommandResult ExecuteRset()
		{
			Rset cm = new Rset();
			Pop3CommandResult rs = null;
			String s = "";

			this.CheckAuthenticate();
			s = this.Execute(cm);
			rs = new Pop3CommandResult(s);
			return rs;
		}
		
		
		
		
		
		
		public Pop3CommandResult ExecuteQuit()
		{
			Quit cm = new Quit();
			Pop3CommandResult rs = null;
			String s = "";

			this.EnsureOpen();
			s = this.Execute(cm);
			rs = new Pop3CommandResult(s);
			return rs;
		}
		
		
		
		
		
		
		public Int64 GetTotalMessageCount()
		{
			Stat.Result rs = null;
			rs = this.ExecuteStat();
			return rs.TotalMessageCount;
		}
		
		
		
		
		
		
		
		public Pop3Message GetMessage(Int64 mailIndex)
		{
			Pop3Message pm = null;
			Retr cm = null;

			this.CheckAuthenticate();
			try
			{
				cm = new Retr(mailIndex);
				String MailData = this.Execute(cm);
				pm = new Pop3Message(MailData, mailIndex);
			}
			catch (Exception ex)
			{
				throw new Pop3ReceiveException(ex);
			}
			return pm;
		}
		
		
		
		
		
		
		
		
		public Pop3Message GetMessage(Int64 mailIndex, Int32 lineCount)
		{
			Pop3Message pm = null;
			Top cm = null;

			this.CheckAuthenticate();
			try
			{
				cm = new Top(mailIndex, lineCount);
				String MailData = this.Execute(cm);
				pm = new Pop3Message(MailData, mailIndex);
			}
			catch (Exception ex)
			{
				throw new Pop3ReceiveException(ex);
			}
			return pm;
		}
		
		
		
		
		
		
		
		public String GetMessageText(Int64 mailIndex)
		{
			Retr cm = null;

			this.CheckAuthenticate();
			try
			{
				cm = new Retr(mailIndex);
				return this.Execute(cm);
			}
			catch (Exception ex)
			{
				throw new Pop3ReceiveException(ex);
			}
		}
		
		
		
		
		
		
		
		
		public String GetMessageText(Int64 mailIndex, Int32 lineCount)
		{
			Top cm = null;

			this.CheckAuthenticate();
			try
			{
				cm = new Top(mailIndex, lineCount);
				return this.Execute(cm);
			}
			catch (Exception ex)
			{
				throw new Pop3ReceiveException(ex);
			}
		}
		
		
		
		
		
		
		
		
		public void GetMessageText(Stream stream, Int64 mailIndex)
		{
			Retr cm = null;

			this.CheckAuthenticate();
			try
			{
				cm = new Retr(mailIndex);
				this.Execute(stream, cm);
			}
			catch (Exception ex)
			{
				throw new Pop3ReceiveException(ex);
			}
		}
		
		
		
		
		
		
		
		
		
		public void GetMessageText(Stream stream, Int64 mailIndex, Int32 lineCount)
		{
			Top cm = null;

			this.CheckAuthenticate();
			try
			{
				cm = new Top(mailIndex, lineCount);
				this.Execute(stream, cm);
			}
			catch (Exception ex)
			{
				throw new Pop3ReceiveException(ex);
			}
		}
		
		
		
		
		
		
		
		public void GetMessageText(Int64 mailIndex, EndGetResponse callbackFunction)
		{
			Retr cm = null;
			EndGetResponse md = callbackFunction;

			this.CheckAuthenticate();
			cm = new Retr(mailIndex);
			this.BeginExecute(cm, md);
		}
		
		
		
		
		
		
		
		
		
		public Boolean DeleteEMail(params Int64[] mailIndex)
		{
			Dele cm = null;
			String s = "";

            if (this.EnsureOpen() == Pop3ConnectionState.Disconnected) { return false; }
            if (this.Authenticate() == false) { return false; }
            for (int i = 0; i < mailIndex.Length; i++)
            {
                cm = new Dele(mailIndex[i]);
                s = this.Execute(cm);
            }
			Boolean bl = MailParser.IsResponseOk(s);
            this.ExecuteQuit();
            return bl;
		}
		
		
		
		
		
		
		
		
		public void ExecuteList(Int64 mailIndex, Action<List<List.Result>> callbackFunction)
		{
			List cm = new List(mailIndex);
			EndGetResponse md = null;

			md = new EndGetResponse(delegate(String responseString)
			{
				List<List.Result> l = new List<List.Result>();
				var rs = new List.Result(responseString);
				l.Add(rs);
				callbackFunction(l);
			});
			this.CheckAuthenticate();
			this.BeginExecute(cm, md);
		}
		
		
		
		
		
		
		
		public void ExecuteList(Action<List<List.Result>> callbackFunction)
		{
			List cm = new List();
			EndGetResponse md = null;

			md = new EndGetResponse(delegate(String responseString)
			{
				List<List.Result> l = new List<List.Result>();
				StringReader sr = null;
				String line = "";

				sr = new StringReader(responseString);
				while (sr.Peek() > -1)
				{
					line = sr.ReadLine();
					if (line == ".")
					{ break; }
					if (line.StartsWith("+OK", StringComparison.InvariantCultureIgnoreCase) == true)
					{ continue; }

					l.Add(new List.Result(line));
				}
				callbackFunction(l);
			});
			this.CheckAuthenticate();
			this.BeginExecute(cm, md);
		}
		
		
		
		
		
		
		
		public void ExecuteUidl(Int64 mailIndex, Action<Uidl.Result[]> callbackFunction)
		{
			Uidl cm = new Uidl(mailIndex);
			EndGetResponse md = null;

			md = new EndGetResponse(delegate(String responseString)
			{
				Uidl.Result[] rs = new Uidl.Result[1];
				rs[0] = new Uidl.Result(responseString);
				callbackFunction(rs);
			});
			this.CheckAuthenticate();
			this.BeginExecute(cm, md);
		}
		
		
		
		
		
		
		public void ExecuteUidl(Action<List<Uidl.Result>> callbackFunction)
		{
			Uidl cm = new Uidl();
			EndGetResponse md = null;

			md = new EndGetResponse(delegate(String responseString)
			{
				List<Uidl.Result> l = new List<Uidl.Result>();
				StringReader sr = null;
				String line = "";

				sr = new StringReader(responseString);
				while (sr.Peek() > -1)
				{
					line = sr.ReadLine();
					if (line == ".")
					{ break; }
					if (line.StartsWith("+OK", StringComparison.InvariantCultureIgnoreCase) == true)
					{ continue; }
					if (Uidl.Result.CheckFormat(line) == false)
					{ continue; }

					l.Add(new Uidl.Result(line));
				}
				callbackFunction(l);
			});
			this.CheckAuthenticate();
			this.BeginExecute(cm, md);
		}
		
		
		
		
		
		
		
		public void GetMessage(Int64 mailIndex, Action<Pop3Message> callbackFunction)
		{
			Retr cm = null;
			EndGetResponse md = null;

			md = new EndGetResponse(delegate(String responseString)
			{
				callbackFunction(new Pop3Message(responseString, mailIndex));
			});
			this.CheckAuthenticate();
			cm = new Retr(mailIndex);
			this.BeginExecute(cm, md);
		}
		
		
		
		
		
		
		
		public void ExecuteRetr(Int64 mailIndex, Action<Pop3Message> callbackFunction)
		{
			Retr cm = null;
			EndGetResponse md = null;

			md = new EndGetResponse(delegate(String responseString)
			{
				callbackFunction(new Pop3Message(responseString, mailIndex));
			});
			this.CheckAuthenticate();
			cm = new Retr(mailIndex);
			this.BeginExecute(cm, md);
		}
		
		
		
		
		
		
		public void ExecuteStat(Action<Stat.Result> callbackFunction)
		{
			Stat cm = null;
			EndGetResponse md = null;

			md = new EndGetResponse(delegate(String responseString)
			{
				callbackFunction(new Stat.Result(responseString));
			});
			this.CheckAuthenticate();
			cm = new Stat();
			this.BeginExecute(cm, md);
		}
		
		
		
		
		
		
		public void ExecuteNoop(Action<Pop3CommandResult> callbackFunction)
		{
			Noop cm = null;
			EndGetResponse md = null;

			md = new EndGetResponse(delegate(String responseString)
			{
				callbackFunction(new Pop3CommandResult(responseString));
			});
			this.EnsureOpen();
			cm = new Noop();
			this.BeginExecute(cm, md);
		}
		
		
		
		
		
		
		public void ExecuteReset(Action<Pop3CommandResult> callbackFunction)
		{
			Rset cm = null;
			EndGetResponse md = null;

			md = new EndGetResponse(delegate(String responseString)
			{
				callbackFunction(new Pop3CommandResult(responseString));
			});
			this.CheckAuthenticate();
			cm = new Rset();
			this.BeginExecute(cm, md);
		}
		
		
		
		
		
		
		public void ExecuteQuit(Action<Pop3CommandResult> callbackFunction)
		{
			Quit cm = null;
			EndGetResponse md = null;

			md = new EndGetResponse(delegate(String responseString)
			{
				Pop3CommandResult rs = new Pop3CommandResult(responseString);
				callbackFunction(rs);
			});
			this.EnsureOpen();
			cm = new Quit();
			this.BeginExecute(cm, md);
		}
		
		
		
		
		
		public void Close()
		{
			this._TcpClient.Close();
			this._State = Pop3ConnectionState.Disconnected;
		}
		
		
		
		
		
		
		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();

			sb.AppendFormat("UserName:{0}", this.UserName);
			sb.AppendLine();
			sb.AppendFormat("ServerName:{0}", this.ServerName);
			sb.AppendLine();
			sb.AppendFormat("Port:{0}", this.Port);
			sb.AppendLine();
			sb.AppendFormat("SSL:{0}", this.Ssl);

			return sb.ToString();
		}
		
		
		
		
		
		public void Dispose()
		{
			GC.SuppressFinalize(this);
			this.Dispose(true);
		}
		
		
		
		
		protected void Dispose(Boolean disposing)
		{
			if (disposing)
			{
				if (this._TcpClient != null)
				{
					((IDisposable)this._TcpClient).Dispose();
					this._TcpClient = null;
				}
				((IDisposable)_SendCommandDone).Dispose();
				((IDisposable)_GetResponseDone).Dispose();
			}
		}
		
		
		
		~Pop3Client()
		{
			this.Dispose(false);
		}
	}
}