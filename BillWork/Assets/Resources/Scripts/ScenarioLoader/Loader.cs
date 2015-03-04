using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;

using Boo.Lang.Compiler;
using Boo.Lang.Compiler.IO;
using Boo.Lang.Compiler.Pipelines;

public class ScenarioLoader {

	public enum SceneType { TabletScene, TableTopScene };

	SceneType _type;

	private Scenario _scenario;

	public ScenarioLoader(SceneType type, string file) {
		_type = type;

		XmlSerializer deserializer = new XmlSerializer(typeof(Scenario));

		TextReader textReader = new StreamReader(file);
		Scenario scenario = (Scenario)deserializer.Deserialize(textReader);
		textReader.Close();
	}

	public 
}
