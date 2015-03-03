using UnityEngine;
using COMProtocolLib;
using Messages;
using System.Collections;

public class ARCameraPosePublisher : MonoBehaviour {

	private COMClientConnection _conn;

	// Use this for initialization
	void Start () {
		_conn = COMClientConnection.Instance;

		_conn.Host = "127.0.0.1";
		_conn.Port = 9090;

		_conn.Connect ();
	}
	
	// Update is called once per frame
	void Update () {
		_conn.Send("location_update", new CameraPoseMsg(transform.position, transform.rotation));
	}
}
