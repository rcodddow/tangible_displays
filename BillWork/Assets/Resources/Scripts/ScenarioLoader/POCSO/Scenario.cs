using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

public class Scenario {

	[XmlAttribute("name")]
	public string name;

	[XmlElement("tabletScene")]
	public Scene tablet_scene; 
	
	[XmlElement("tabletopScene")]
	public Scene tabletop_scene; 
	
}
