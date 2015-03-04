using UnityEngine;
using System.Collections;

public class NET_Handler:MonoBehaviour{
	public NET_TableState net_TableState;		//The current state of the table version of the program.
	
	public UTIL_Scripts scripts;

	void Start(){
		net_TableState=new NET_TableState(scripts.ui_LayersMenuHandler.root);	//Create the TableState.
	}

	void Update(){
		//Update the different net handlers.
		net_TableState.Update();
	}
	private void SendBroadcast(){
		//Send out the broadcast messages.
	}
}
