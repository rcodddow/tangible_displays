using UnityEngine;
using COMProtocolLib;
using Messages;
using System.Collections;

public class ScenarioManager : MonoBehaviour, COMServerDelegate {

	private COMServerConnection _conn;

	// Use this for initialization
	void Start () {
		_conn = COMServerConnection.Instance;
		_conn.Port = 9090;

		_conn.AddDelegate (this);

		_conn.Connect ();
	}

	public void OnMessage(string identifier, string operation, COMMessage msg) {
		Debug.Log("Client : " + identifier + " sent operation : " + operation + " with content : " + msg.ToYAMLString());
	}
	
	// Update is called once per frame
	void Update () {
	}
}
