using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR && XtoJSON
using UnityEditor;

public class BuildSettingsMacros {
	public string[] scenes;
	private static string[] ReadNames() {
		List<string> temp = new List<string>();
		foreach (UnityEditor.EditorBuildSettingsScene S in UnityEditor.EditorBuildSettings.scenes) {
			if (S.enabled) {
				string name = S.path.Substring(S.path.LastIndexOf('/') + 1);
				name = name.Substring(0, name.Length - 6);
				temp.Add(name);
			}
		}
		return temp.ToArray();
	}

	[UnityEditor.MenuItem("Utilities/Update Scene Names")]
	private static void UpdateNames(UnityEditor.MenuCommand command) {
		//ReadSceneNames context = (ReadSceneNames)command.context;
		//context.scenes = ReadNames();
		JsonArray names = new JsonArray();
		foreach (string name in ReadNames()) {
			names.Add(name);
		}
		string path = Application.dataPath + "/Data/Resources/sceneNames.json";
		File.WriteAllText(path, names.ToString());
	}


	private void Reset() {
		scenes = ReadNames();
	}


}
#endif

public static class SceneNames {
	public static List<string> list;

#if XtoJSON
	public static bool loaded = Load();
	static bool Load() {
		TextAsset file = Resources.Load<TextAsset>("sceneNames");
		if (file == null) { list = null; }
		JsonArray arr = Json.Parse(file.text) as JsonArray;

		list = arr.OnlyStringToList();
		return true;
	}
#endif

}
