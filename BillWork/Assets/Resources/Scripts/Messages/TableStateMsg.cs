using UnityEngine;
using COMProtocolLib;
using System.Collections;
using System.Text;
using SimpleJSON;

namespace Messages {
public class TableStateMsg : COMMessage {

	public string _mode;

	public Quaternion _rotation;

	public TableStateMsg(JSONNode msg) {
		_mode = msg ["mode"];
		_rotation = new Quaternion (float.Parse(msg["qx"]),float.Parse(msg["qy"]),float.Parse(msg["qz"]), float.Parse(msg["qw"]));
	}

	public TableStateMsg(string mode, Quaternion rotation) {
		_mode = mode;
		_rotation = rotation;
	}
	
	public override string ToYAMLString() {
		StringBuilder x = new StringBuilder();
		x.Append("{");
		x.Append (" \"mode\" : " + _mode + ",");
		x.Append (" \"qx\" : " + _rotation.x + ",");
		x.Append (" \"qy\" : " + _rotation.y + ",");
		x.Append (" \"qz\" : " + _rotation.z + ",");
		x.Append (" \"qw\" : " + _rotation.w);
		x.Append("}");
		return x.ToString();
	}
}
}
