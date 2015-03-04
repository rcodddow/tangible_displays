using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

public class Model {

	[XmlAttribute("identifier")]
	public string identifer;

	[XmlAttribute("modelFile")]
	public string modelFile;

	[XmlAttribute("scriptFile")]
	public string scriptFile;

	[XmlAttribute("scale")]
	public float scale;

	//[XmlArrayItem("Capability")]
	public List<Capability> capabilities;

	[XmlArrayItem("Model")]
	public List<Model> models;

	public Vector3 location {
		get {
			return new Vector3(x, y, z);
		}
		set {
			x = value.x;
			y = value.y;
			z = value.z;
		}
	}

	[XmlAttribute("x")]
	public float x;

	[XmlAttribute("y")]
	public float y;

	[XmlAttribute("z")]
	public float z;

	public Quaternion rotation {
		get {
			return new Quaternion(q_x,q_y,q_z,q_w);
		}
		set {
			q_x = value.x;
			q_y = value.y;
			q_z = value.z;
			q_z = value.w;
		}
	}

	[XmlAttribute("q_x")]
	public float q_x;
	
	[XmlAttribute("q_y")]
	public float q_y;
	
	[XmlAttribute("q_z")]
	public float q_z;

	[XmlAttribute("q_w")]
	public float q_w;
	
}
