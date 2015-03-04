using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

public class Scene {

	[XmlArrayItem("Model")]
	public List<Model> models;

	[XmlAttribute("scriptFile")]
	public string scriptFile;
}
