/* * * * *
 * Tangible Displays
 * ------------------------------
 * 
 * This is part of the Tangible Displays Project, written over the week of March 2-6 in
 * Hamamatsu Japan.
 * 
 * This defines the connection to the outside world
 * * * * */
using System;
using UnityEngine;

using System.Collections.Generic;
using System.Threading;
using System.Reflection;
using SimpleJSON;
using COMProtocolLib;

public class COMClientConnection {
	private AndroidJavaObject _plugin;

	private List<MessagePair> _taskQ = new List<MessagePair>();

	public class MessagePair {
		private string _operation;
		private COMMessage _msg;
		
		public MessagePair(string operation, COMMessage msg) {
			_operation = operation;
			_msg = msg;
		}
		
		public COMMessage getMsg() {
			return _msg;
		}
		
		public string getOperation() {
			return _operation;
		}
	};
	
	public COMClientConnection(string host, int port) {
		_plugin = new AndroidJavaObject("com.table.commplugin.SocketConnection"); 
		if (_plugin == null) {
			Debug.Log ("Failed to create java object");
		} else {	
			Debug.Log ("About to connect");
			int _result = _plugin.Call<int> ("connect", host, port);
			Debug.Log ("The other result was " + _result);
		}
	}
	
	public Boolean isAlive() {
		if (_plugin == null)
			return false;
		return _plugin.Call<Boolean> ("connected");
	}

	public void Close() {
		if (_plugin != null)
			_plugin.Call("close");
		_taskQ = new List<MessagePair>();
	}

	public void Send(String operation, COMMessage msg) {
		if (_plugin != null) {
			
			string s = "{";
			s += "\"op\": \"" + operation + "\"";
			s += "," + "\"type\": \"" + msg.GetType ().ToString () + "\"";
			s += "," + "\"msg\": " + msg.ToYAMLString ();
			s += "}\n";

			_plugin.Call ("send", s);
		} else {
			Debug.Log ("Trying to send " + msg.ToString() + " but no connection.");
		}
	}

	public MessagePair dequeueMessage() {
		MessagePair pair = null;
		if (_taskQ.Count != 0) {
			pair = _taskQ[_taskQ.Count-1];
			_taskQ.RemoveAt(_taskQ.Count-1);
		}
		return pair;
	}
	
	public void Update() {

		string _result = _plugin.Call<string> ("getNextMessage");
		Debug.Log ("Update gets (raw) '" + _result + "'");

		if((_result != null) && !_result.Equals ("")) {
			JSONNode node = JSONNode.Parse(_result);
			
			string operation = node["op"];
			Type type = Type.GetType(node["type"]);
			if(type == null) {
				Debug.Log ("Update: We got a type that we don't know about " + _result);
				return;
			}
			ConstructorInfo constructor = type.GetConstructor(new Type[] { typeof(JSONNode) });
			
			if (constructor == null) {
				Debug.Log("could not find proper contruction in type : " + node["type"]);
				return;
			}

			COMMessage message = (COMMessage) constructor.Invoke(new object[] { node["msg"] });

			MessagePair pair = new MessagePair(operation, message);

			bool found = false;
			for(int i=0;i<_taskQ.Count;i++) {
				if(_taskQ[i].getOperation().Equals (operation)) {
					_taskQ.RemoveAt (i);
					_taskQ.Insert (i, pair);
					found = true;
					break;
				}
			}
			if(!found) {
				_taskQ.Add (pair);
			}
		}
	}
}