using UnityEngine;
using System.Collections;
using COMProtocolLib;
using SimpleJSON;

public class NET_Handler:MonoBehaviour{
	public NET_TableState net_TableState;		//The current state of the table version of the program.

	private COMServerConnection conn;

	public UTIL_Scripts scripts;

	void Start(){
		//conn=COMServerConnection.Instance;
		//conn.Port=9090;
		//conn.Connect();

		net_TableState=new NET_TableState(scripts.ui_LayersMenuHandler.root);	//Create the TableState.
	}

	void Update(){
		//Update the different net handlers.
		net_TableState.Update();
		//SendBroadcast();
	}
	private void SendBroadcast(){
		//Send out the broadcast messages.
		conn.Broadcast("table_state", net_TableState);
	}
}
