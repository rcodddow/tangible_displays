using UnityEngine;
using System.Collections;
using COMProtocolLib;
using SimpleJSON;
using Messages;

public class NET_Handler : MonoBehaviour{
	public TableStateMsg tableState;		//The current state of the table version of the program.
	public string currentTableMode;			//Either "EYE" or "SCOPE".

	private COMServerConnection conn;

	public UTIL_Scripts scripts;

	void Start() {
		currentTableMode="EYE";
		tableState=new TableStateMsg(currentTableMode, transform.rotation);	//Create the TableState.
		conn = COMServerConnection.Instance;
		conn.Port = 9090;
		conn.Connect();
	}

	void Update(){
		//Update the different net handlers.
		if (tableState != null) {
			tableState._mode=currentTableMode;
			tableState._rotation = transform.rotation;
			SendBroadcast ();
		}
	}
	private void SendBroadcast(){
		//Send out the broadcast messages.
		conn.Broadcast("table_state", tableState);
	}
}
