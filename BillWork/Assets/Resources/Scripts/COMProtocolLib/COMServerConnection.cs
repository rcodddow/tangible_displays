using System.Collections.Generic;
using System.Security.Cryptography;
using System.Threading;
using System.Reflection;
using System.Linq;

using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

using SimpleJSON;
using UnityEngine;

/**
 * This class handles the connection with the external ROS world, deserializing
 * json messages into appropriate instances of packets and messages.
 * 
 * This class also provides a mechanism for having the callback's exectued on the rendering thread.
 * (Remember, Unity has a single rendering thread, so we want to do all of the communications stuff away
 * from that. 
 * 
 * The one other clever thing that is done here is that we only keep 1 (the most recent!) copy of each message type
 * that comes along.
 * 
 * Version History
 * 3.1 - changed methods to start with an upper case letter to be more consistent with c#
 * style.
 * 3.0 - modification from hand crafted version 2.0
 * 
 * @author Michael Jenkin, Robert Codd-Downey and Andrew Speers
 * @version 3.1
 */

namespace COMProtocolLib {
	public class COMServerConnection {

		private class RenderTask {
			private COMServerDelegate _delegate;
			private string _identifier;
			private string _operation;
			private COMMessage _msg;

			public RenderTask(COMServerDelegate d, string id, string op, COMMessage msg) {
				_delegate = d;
				_operation = op;
				_identifier = id;
				_msg = msg;
			}

			public COMServerDelegate getDelegate() {
				return _delegate;
			}

			public COMMessage getMsg() {
				return _msg;
			}

			public string getOperation() {
				return _operation;
			}

			public string getIdentifier() {
				return _identifier;
			}
		};

		private static COMServerConnection instance;
		
		public static COMServerConnection Instance
		{
			get 
			{
				if (instance == null)
				{
					instance = new COMServerConnection();
				}
				return instance;
			}
		}

		/**
		 * Make a connection to a host/port. 
		 * This does not actually start the connection, use Connect to do that.
		 */
		public COMServerConnection() {
			_port = 9090;
			_listenerThread = null;
		}

		private TcpListener _server;

		private ConcurrentDictionary<string, NetworkStream> _client = new ConcurrentDictionary<string, NetworkStream>(); 

		private List<COMServerDelegate> _delegates = new List<COMServerDelegate>();
		
		private System.Threading.Thread _listenerThread;

		private List<RenderTask> _taskQ = new List<RenderTask>();
		
		private object _queueLock = new object ();
		
		private int _port;
		public int Port {
			get {
				return _port;
			}
			set {
				if (value != _port) {
					if (_listenerThread != null && _listenerThread.IsAlive == true) {
						Disconnect();
					}
				}
			}
		}

		public COMServerDelegate[] getDelegates() {
			return _delegates.ToArray ();
		}

		public void AddDelegate(COMServerDelegate d) {
			_delegates.Add (d);
		}

		public void RemoveDelegate(COMServerDelegate d) {
			_delegates.Remove (d);
		}

		/**
		 * Connect to the remote ros environment.
		 */
		public void Connect() {
			if (_listenerThread == null || _listenerThread.IsAlive == false) {
				_listenerThread = new System.Threading.Thread (ConnnectionListener);
				_listenerThread.Start ();
			}
		}

		/**
		 * Disconnect from the remote ros environment.
		 */
		public void Disconnect() {
			if (_listenerThread != null && _listenerThread.IsAlive == true) {
				_listenerThread.Abort ();
				_server.Stop();
			}
		}

		private void ConnnectionListener() {

			_server = new TcpListener(IPAddress.Parse("127.0.0.1"), _port);
			_server.Start();

			while (true) {
				TcpClient client = _server.AcceptTcpClient();
				Thread clientThread = new Thread(new ParameterizedThreadStart(ConnectionHandler));
				clientThread.Start(client);
			}
		}

		private void ConnectionHandler(object client) {

			TcpClient tcpClient = (TcpClient)client;
			NetworkStream clientStream = tcpClient.GetStream();

			string identifier = RandomString(32); 

			_client.Add (identifier, clientStream);

			string message = "";
			
			int bytesRead;
			byte[] buffer = new byte[4096];
			
			while (true)
			{
				bytesRead = 0;
				
				try {
					//blocks until a client sends a message
					bytesRead = clientStream.Read(buffer, 0, 4096);
				} catch {
					//a socket error has occured
					break;
				}
				
				if (bytesRead == 0) {
					//the client has disconnected from the server
					break;
				}

				int frag_size = 0;
				while (frag_size < bytesRead) {
					if (buffer[frag_size++] == '\n')
						break;
				}

				ASCIIEncoding encoder = new ASCIIEncoding();
				message += encoder.GetString(buffer, 0, frag_size);

				if (frag_size < bytesRead) { // recieved complete JSON string
					OnMessage(message, identifier);

					if (bytesRead - frag_size > 1) {
						message = encoder.GetString(buffer, frag_size, bytesRead - frag_size);
					}
				}
			}

			_client.Remove (identifier);
			
			tcpClient.Close();
		}

		private void OnMessage(string s, string id)
		{
			if ((s != null) && !s.Equals ("")) {
				JSONNode node = JSONNode.Parse (s);
				
				string operation = node["op"];
				Type type = Type.GetType(node["type"]);
				ConstructorInfo constructor = type.GetConstructor(new Type[] { typeof(JSONNode) });
				
				if (constructor == null) {
					Debug.Log("could not find proper contruction in type : " + node["type"]);
					return;
				}
				
				foreach (COMServerDelegate _delegate in _delegates) {
					COMMessage message = (COMMessage) constructor.Invoke(new object[] { node["msg"] });
					AddTaskToQueue(new RenderTask (_delegate, id, operation, message));
				}
			}
		}

		public void Render() {
			RenderTask newTask = null;
			lock (_queueLock) {
				if(_taskQ.Count > 0) {
					newTask = _taskQ[0];
					_taskQ.RemoveAt (0);
				}
			}
			if (newTask != null) {
				COMServerDelegate _delegate = newTask.getDelegate();
				_delegate.OnMessage(newTask.getIdentifier(), newTask.getOperation(), newTask.getMsg());
			}
		}

		public void Broadcast(String operation, COMMessage msg) {

			ASCIIEncoding encoder = new ASCIIEncoding();

			string s = "{";
			s += "\"op\": \"" + operation + "\"";
			s += "," + "\"type\": \"" + msg.GetType().ToString() + "\"";
			s += "," + "\"msg\": \"" + msg.ToYAMLString() + "\"";
			s += "}";

			byte[] buffer = encoder.GetBytes(s);

			string[] keys = _client.GetKeysArray();
			foreach (string identifier in keys) {

				NetworkStream stream = _client[identifier];
				
				if (stream != null && stream.CanWrite) {
					stream.Write(buffer, 0 , buffer.Length);
					stream.Flush();
				}
			}
		}

		public void Send(string identifier, String operation, COMMessage msg) {
		
			NetworkStream stream = _client[identifier];

			if (stream != null && stream.CanWrite) {

				string s = "{";
				s += "\"op\": \"" + operation + "\"";
				s += "," + "\"type\": \"" + msg.GetType().ToString() + "\"";
				s += "," + "\"msg\": \"" + msg.ToYAMLString() + "\"";
				s += "}";


				ASCIIEncoding encoder = new ASCIIEncoding();
				byte[] buffer = encoder.GetBytes(s);
				stream.Write(buffer, 0 , buffer.Length);
				stream.Flush();
			}
		}

		private void AddTaskToQueue(RenderTask newTask) {
			lock(_queueLock) {
				bool found = false;
				for(int i=0;i<_taskQ.Count;i++) {
					if(_taskQ[i].getOperation().Equals (newTask.getOperation())) {
						_taskQ.RemoveAt (i);
						_taskQ.Insert (i, newTask);
						found = true;
						break;
					}
				}
				if(!found) {
					_taskQ.Add (newTask);
				}
			}
		}

		string RandomString(int length, string alphabet = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789")
		{       
			var outOfRange = Byte.MaxValue + 1 - (Byte.MaxValue + 1) % alphabet.Length;
			
			return string.Concat(
				Enumerable
				.Repeat(0, Int32.MaxValue)
				.Select(e => RandomByte())
				.Where(randomByte => randomByte < outOfRange)
				.Take(length)
				.Select(randomByte => alphabet[randomByte % alphabet.Length])
				);
		}
		
		byte RandomByte()
		{
			var randomizationProvider = new RNGCryptoServiceProvider ();

			var randomBytes = new byte[1];
			randomizationProvider.GetBytes(randomBytes);
			return randomBytes.Single();
		}
	}
}

public class ConcurrentDictionary<tkey, tvalue>{
	private readonly object syncLock = new object();
	private Dictionary<tkey, tvalue> dict;
	
	public tvalue this[tkey key] {
		get { lock (syncLock) { return dict[key]; } }
		set { lock (syncLock) { dict[key] = value; } }
	}
	
	public int Count
	{
		get
		{
			lock(syncLock)
			{
				return dict.Count;
			}
		}
	}
	
	public bool ContainsKey(tkey item) { lock(syncLock) { return dict.ContainsKey(item); } }
	
	public ConcurrentDictionary()
	{
		this.dict = new Dictionary<tkey, tvalue>();
	}
	
	public void Add(tkey key, tvalue val)
	{
		lock(syncLock)
		{
			dict.Add(key, val);
		}
	}
	
	public void Remove(tkey key)
	{
		lock(syncLock)
		{
			dict.Remove(key);
		}
	}
	
	public void Clear()
	{
		lock(syncLock)
		{
			dict.Clear();
		}
	}
	
	public tkey[] GetKeysArray() { lock(syncLock) { tkey[] result = new tkey[dict.Keys.Count]; dict.Keys.CopyTo(result, 0); return result; } }
}
