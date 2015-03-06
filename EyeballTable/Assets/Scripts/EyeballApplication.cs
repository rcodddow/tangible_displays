/* * * * *
 * Tangible Displays
 * ------------------------------
 * 
 * This is part of the Tangible Displays Project, written over the week of March 2-6 in
 * Hamamatsu Japan.
 * 
 * This module provides the basic logic for the tablet portion of the display.
 *
 * The HOST and PORT below, define the connection to the server.
 * * * * */

using UnityEngine;
using System.Collections;
using COMProtocolLib;
using Messages;

public class EyeballApplication : MonoBehaviour {
//#if !UNITY_EDITOR
	private static string HOST = "192.168.42.172";
	private static int PORT = 9090;
//#endif

	private COMClientConnection connection = null;
	private GameObject eyeball = null;

	void OnGUI(){
		if(connection == null)
			GUI.Box (new Rect (Screen.width / 2 - 50, 0, 100, 30), "Disconnected");
	}

	void connect() {
//#if!UNITY_EDITOR
		Debug.Log ("Trying to connnect/reconnect");
		if ((connection == null) || (!connection.isAlive ())) {
			Debug.Log ("Update....dealing with failure with connection " + connection);
			if (connection != null) {
				Debug.Log ("Connection not alive: closing it");
				connection.Close ();
			}
			Debug.Log ("Trying to establish connection to remote server");
			connection = new COMClientConnection (HOST, PORT);
			if(!connection.isAlive())
				connection = null;
		} else {
			connection.Close ();
			connection = null;
		}
//#endif
	}


	void Update() {
//#if !UNITY_EDITOR
		if ((connection != null) &&(connection.isAlive ())) {
			connection.Update ();
			COMClientConnection.MessagePair pair = connection.dequeueMessage ();
			if (pair != null) {
				Debug.Log ("Pair " + pair.getOperation () + " " + pair.getMsg ());
				COMMessage msg = pair.getMsg ();
				if(msg.GetType() == typeof(TableStateMsg)) {
					TableStateMsg tstate = (TableStateMsg) msg;
					if(eyeball == null)
						Debug.Log ("NO EYEBALL????");
					else {
						eyeball.transform.rotation = tstate._rotation;
						Debug.Log("The mode right now is '" + tstate._mode + "'");
						if(tstate._mode.Equals ("EYE"))
							eyeball.SetActive (true);
						else
							eyeball.SetActive (false);
					}
				} else
					Debug.Log ("Sorry, I don't know that message!");

			}

			Transform cam = Camera.main.transform;
			CameraPoseMsg newmsg = new CameraPoseMsg(cam.position, cam.rotation);

			connection.Send ("TabletPose", newmsg);
		}
//#endif
	}
	
	
	void Start() {
//#if UNITY_EDITOR
		Debug.Log ("Running in the editor");
//#else
		Debug.Log ("Starting up our first connection with the server");
		connection = new COMClientConnection (HOST, PORT);
//#endif
		eyeball = GameObject.Find ("Eye");
	}
}
