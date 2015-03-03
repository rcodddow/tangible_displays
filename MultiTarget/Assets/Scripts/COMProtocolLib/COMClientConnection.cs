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
	public class COMClientConnection {
		private class RenderTask {
			private Action<string, COMMessage> _action;
			private string _operation;
			private COMMessage _msg;

			public RenderTask(Action<string, COMMessage> action, string operation, COMMessage msg) {
				_action = action;
				_operation = operation;
				_msg = msg;
			}

			public Action<string, COMMessage> getAction() {
				return _action;
			}

			public COMMessage getMsg() {
				return _msg;
			}

			public string getOperation() {
				return _operation;
			}
		};

		private static COMClientConnection instance;
		
		public static COMClientConnection Instance
		{
			get {
				if (instance == null) {
					instance = new COMClientConnection();
				}
				return instance;
			}
		}

		/**
		 * Make a connection to a host/port. 
		 * This does not actually start the connection, use Connect to do that.
		 */
		public COMClientConnection() {
			_host = "127.0.0.1";
			_port = 9090;
			_myThread = null;
			_delegates = new List<Action<string, COMMessage>> ();
		}

		private WebSocket _ws;

		private System.Threading.Thread _myThread;
		
		private List<Action<string, COMMessage>> _delegates;
		private List<RenderTask> _taskQ = new List<RenderTask>();
		
		private object _queueLock = new object ();

		private string _host;
		public string Host {
			get {
				return _host;
			}
			set {
				if (value != _host) {
					if (_myThread != null && _myThread.IsAlive == true) {
						Disconnect();
					}
				}
			}
		}

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

		public void addAction(Action<string, COMMessage> action) {
			_delegates.Add (action);
		}

		public void removeAction(Action<string, COMMessage> action) {
			_delegates.Remove (action);
		}

		/**
		 * Connect to the remote ros environment.
		 */
		public void Connect() {
			if (_myThread == null || _myThread.IsAlive == false) {
				_myThread = new System.Threading.Thread (Run);
				_myThread.Start ();
			} else {
				Debug.Log ("Cannot connect, because there is already an active connection");
			}
		}

		/**
		 * Disconnect from the remote ros environment.
		 */
		public void Disconnect() {
			if (_myThread != null && _myThread.IsAlive == true) {
				_myThread.Abort ();
				_ws.Close ();
			} else {
				Debug.Log ("Cannot disconnect, because there is no active connection");
			}
		}

		private void Run() {
			_ws = new WebSocket(_host + ":" + _port);
			_ws.OnMessage += (sender, e) => this.OnMessage(e.Data);
			_ws.Connect();

			while(true) {
				Thread.Sleep (10000);
			}
		}

		private void OnMessage(string s) {
			// Debug.Log ("Got message " + s);
			if((s != null) && !s.Equals ("")) {
				JSONNode node = JSONNode.Parse(s);

				string operation = node["op"];
				Type type = Type.GetType(node["type"]);
				ConstructorInfo constructor = type.GetConstructor(new Type[] { typeof(JSONNode) });

				if (constructor == null) {
					Debug.Log("could not find proper contruction in type : " + node["type"]);
					return;
				}

				foreach (Action<string, COMMessage> action in _delegates) {
					COMMessage message = null;//constructor.Invoke(new object[] { (object) node["msg"] });
					RenderTask newTask = new RenderTask(action, operation, message);
					lock(_queueLock) {
						bool found = false;
						for(int i=0;i<_taskQ.Count;i++) {
							if(_taskQ[i].getOperation().Equals (operation)) {
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

		public void Render() {
			RenderTask newTask = null;
			lock (_queueLock) {
				if(_taskQ.Count > 0) {
					newTask = _taskQ[0];
					_taskQ.RemoveAt (0);
				}
			}
			if (newTask != null) {
				Action<string, COMMessage> action = newTask.getAction();
				action(newTask.getOperation(), newTask.getMsg ());
			}
		}

		public void Send(String operation, COMMessage msg) {
			if(_ws != null) {

				string s = "{";
				s += "\"op\": \"" + operation + "\"";
				s += "," + "\"type\": \"" + msg.GetType().ToString() + "\"";
				s += "," + "\"msg\": \"" + msg.ToYAMLString() + "\"";
				s += "}";

				//Debug.Log ("Sending " + s);
				_ws.Send (s);
			}
		}
	}
}
