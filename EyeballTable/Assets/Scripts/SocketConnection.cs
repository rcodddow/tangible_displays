/* * * * *
 * Tangible Displays
 * ------------------------------
 * 
 * This is part of the Tangible Displays Project, written over the week of March 2-6 in
 * Hamamatsu Japan.
 * 
 * This defines the wrapper for the Java code to do tcp/ip on the free version of unity on
 * an android platform. Its ugly. Sorry.
 * * * * */
using System;
using UnityEngine;


public class SocketConnection {
	AndroidJavaObject _plugin;

	public SocketConnection(string host, int port) {
		_plugin = new AndroidJavaObject("com.table.commplugin.SocketConnection"); 
		if (_plugin == null)
			Debug.Log ("Failed to create java object");
		else {	
//			Debug.Log ("About to connect");
			int _result = _plugin.Call<int> ("connect", host, port);
//			Debug.Log ("The other result was " + _result);
		}
	}

	public void Close() {
		if (_plugin != null)
			_plugin.Call("close");
	}

	public Boolean isAlive() {
		if (_plugin == null)
			return false;
		return _plugin.Call<Boolean> ("connected");
	}

	public void Send(string s) {
		if (_plugin != null)
			_plugin.Call ("send", s);
		else
			Debug.Log ("Trying to send " + s + " but no connection.");
	}
	
	public string Receive() {
		string _result = _plugin.Call<string> ("getNextMessage");
//		Debug.Log ("Got back " + _result);
		return _result;
	}
}

