using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

using Boo.Lang.Compiler;
using Boo.Lang.Compiler.IO;
using Boo.Lang.Compiler.Pipelines;

public class ScenarioLoader {

	private static string scenarioFile = "scenario.xml";

	enum SceneType { TabletScene, TableTopScene };

	SceneType _type;

	public ScenarioLoader(SceneType type) {
		_type = type;
	}

	// Use this for initialization
	void Start () {
		XmlSerializer deserializer = new XmlSerializer(typeof(Scenario));

		TextReader textReader = new StreamReader(scenarioFile);
		Scenario scenario = (Scenario)deserializer.Deserialize(textReader);
		textReader.Close();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
