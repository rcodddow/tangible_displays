/* * * * *
 * Tangible Displays
 * ------------------------------
 * 
 * This is part of the Tangible Displays Project, written over the week of March 2-6 in
 * Hamamatsu Japan.
 * 
 * This defines the TableState 
 * * * * */
using UnityEngine;
using System.Collections;
using System.Text;
using COMProtocolLib;
using SimpleJSON;

/// <summary>
/// Used to hold the current state information of the table verison of the software. 
/// Used to pass this information to the clients.
/// </summary>

public class NET_TableState:COMMessage{
	//Object information
	public GameObject obj;				//The object on the table.
	public Quaternion objRotation;		//The object's rotation.

	public NET_TableState(JSONNode msg){
		objRotation=new Quaternion(float.Parse(msg["qx"]), float.Parse(msg["qy"]), float.Parse(msg["qz"]), float.Parse(msg["qw"]));
	}
	public NET_TableState(GameObject _obj){
		obj=_obj;
		objRotation=obj.transform.rotation;
	}

	public override string ToYAMLString(){
		StringBuilder x = new StringBuilder();
		x.Append("{");
		x.Append (" \"qx\" : " + objRotation.x + ",");
		x.Append (" \"qy\" : " + objRotation.y + ",");
		x.Append (" \"qz\" : " + objRotation.z + ",");
		x.Append (" \"qw\" : " + objRotation.w);
		x.Append("}");
		return x.ToString();
	}

	public void Update(){
		//Updates any needed information per frame.
		if(obj) objRotation=obj.transform.rotation;
	}
}
