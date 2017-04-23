using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;

#if UNITY_EDITOR && XtoJSON
using UnityEditor;

public class BuildSettingsMacros {
	public string[] scenes;
	public static string[] ReadSceneNames() {
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

	public static string[] ReadScenePaths() {
		List<string> temp = new List<string>();
		foreach (UnityEditor.EditorBuildSettingsScene S in UnityEditor.EditorBuildSettings.scenes) {
			if (S.enabled) { temp.Add(S.path); }
		}
		return temp.ToArray();
	}

	[UnityEditor.MenuItem("ZSharp/Macros/Update Scene Names")]
	private static void UpdateNames(UnityEditor.MenuCommand command) {
		JsonArray names = new JsonArray();
		foreach (string name in ReadSceneNames()) {
			names.Add(name);
		}
		string path = Application.dataPath + "/Data/Resources/sceneNames.json";
		File.WriteAllText(path, names.ToString());
	}


	private void Reset() {
		scenes = ReadSceneNames();
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
