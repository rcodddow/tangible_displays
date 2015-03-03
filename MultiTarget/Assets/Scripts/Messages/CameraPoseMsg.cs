using UnityEngine;
using COMProtocolLib;
using System.Collections;
using System.Text;
using SimpleJSON;

namespace Messages {
public class CameraPoseMsg : COMMessage {

	public Vector3 _position;

	public Quaternion _rotation;

	public CameraPoseMsg(JSONNode msg) {
		_position = new Vector3 (float.Parse(msg["x"]),float.Parse(msg["y"]),float.Parse(msg["z"]));
		_rotation = new Quaternion (float.Parse(msg["qx"]),float.Parse(msg["qy"]),float.Parse(msg["qz"]), float.Parse(msg["qw"]));
	}

	public CameraPoseMsg(Vector3 position, Quaternion rotation) {
		_position = position;
		_rotation = rotation;
	}
	
	public override string ToYAMLString() {
		StringBuilder x = new StringBuilder();
		x.Append("{");
		x.Append (" \"x\" : " + _position.x + ",");
		x.Append (" \"y\" : " + _position.x + ",");
		x.Append (" \"z\" : " + _position.x + ",");
		x.Append (" \"qx\" : " + _rotation.x + ",");
		x.Append (" \"qy\" : " + _rotation.y + ",");
		x.Append (" \"qz\" : " + _rotation.z + ",");
		x.Append (" \"qw\" : " + _rotation.w);
		x.Append("}");
		return x.ToString();
	}
}
}
