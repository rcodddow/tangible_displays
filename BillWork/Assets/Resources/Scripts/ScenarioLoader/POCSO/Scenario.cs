using UnityEngine;
using System.Collections;


public class Scenario {

	[XmlAttribute("name")]
	public string name;

	[XmlElement("tabletScene")]
	public Scene tablet_scene; 
	
	[XmlElement("tabletopScene")]
	public Scene tabletop_scene; 
	
}
