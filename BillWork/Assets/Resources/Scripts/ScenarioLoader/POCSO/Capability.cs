using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

public class Capability {

	[XmlAttribute("type")]
	public string type;

	public bool isKnownCapability() {
		if (type == "hide") {
			return true;
		} else if (type == "roll") {
			return true;
		} else if (type == "pitch") {
			return true;
		} else if (type == "yaw") {
			return true;
		} else if (type == "zoom") {
			return true;
		} else if (type == "pan") {
			return true;
		} else if (type == "select") {
			return true;
		} else if (type == "focus") {
			return true;
		}
		return false;
	}

}
