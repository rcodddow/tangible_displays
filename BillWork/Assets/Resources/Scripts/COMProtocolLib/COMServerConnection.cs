using System.Collections.Generic;
using System.Threading;
using System.Reflection;
using System;
using WebSocketSharp;
using WebSocketSharp.Net;
using WebSocketSharp.Server;
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

		public class Unity : WebSocketBehavior
		{
			protected override void OnMessage(MessageEventArgs e)
			{
				string s = e.Data;
				if ((s != null) && !s.Equals ("")) {
					JSONNode node = JSONNode.Parse (s);

					string operation = node["op"];
					Type type = Type.GetType(node["type"]);
					ConstructorInfo constructor = type.GetConstructor(new Type[] { typeof(JSONNode) });

					if (constructor == null) {
						Debug.Log("could not find proper contruction in type : " + node["type"]);
						return;
					}

					COMServerConnection conn = COMServerConnection.Instance;

					foreach (COMServerDelegate _delegate in conn.getDelegates()) {
						COMMessage message = (COMMessage) constructor.Invoke(new object[] { node["msg"] });
						conn.AddTaskToQueue(new RenderTask (_delegate, ID, operation, message));
					}
				}
			}
		}

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
			_myThread = null;
		}

		private WebSocketServer _ws;

		private List<COMServerDelegate> _delegates = new List<COMServerDelegate>();
		
		private System.Threading.Thread _myThread;
		private List<RenderTask> _taskQ = new List<RenderTask>();
		
		private object _queueLock = new object ();
		
		private int _port;
		public int Port {
			get {
				return _port;
			}
			set {
				if (value != _port) {
					if (_myThread != null && _myThread.IsAlive == true) {
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
			if (_myThread == null || _myThread.IsAlive == false) {
				_myThread = new System.Threading.Thread (Run);
				_myThread.Start ();
			}
		}

		/**
		 * Disconnect from the remote ros environment.
		 */
		public void Disconnect() {
			if (_myThread != null && _myThread.IsAlive == true) {
				_myThread.Abort ();
				_ws.Stop ();
			}
		}

		private void Run() {
			_ws = new WebSocketServer(_port);
			_ws.AddWebSocketService<Unity> ("/");

			_ws.Start();

			while(true) {
				Thread.Sleep (10000);
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
			if(_ws != null) {

				WebSocketServiceManager services = _ws.WebSocketServices;
				
				WebSocketServiceHost host = services["/"];
				
				WebSocketSessionManager sessions = host.Sessions;

				string s = "{";
				s += "\"op\": \"" + operation + "\"";
				s += "," + "\"type\": \"" + msg.GetType().ToString() + "\"";
				s += "," + "\"msg\": \"" + msg.ToYAMLString() + "\"";
				s += "}";
				
				//Debug.Log ("Sending " + s);
				sessions.Broadcast(s);
			}
		}

		public void Send(String identifier, String operation, COMMessage msg) {
			if(_ws != null) {

				WebSocketServiceManager services = _ws.WebSocketServices;

				WebSocketServiceHost host = services["/"];

				WebSocketSessionManager sessions = host.Sessions;

				string s = "{";
				s += "\"op\": \"" + operation + "\"";
				s += "," + "\"type\": \"" + msg.GetType().ToString() + "\"";
				s += "," + "\"msg\": \"" + msg.ToYAMLString() + "\"";
				s += "}";

				//Debug.Log ("Sending " + s);
				sessions.SendTo(identifier, s);
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
	}
}
