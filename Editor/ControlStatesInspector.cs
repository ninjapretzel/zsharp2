using UnityEngine;
using UnityEditor;

public class ControlStatesInspector : EditorWindow {

	public ControlStatesInspector() {
		titleContent = new GUIContent("Control States");
	}

	[MenuItem ("Tools/ControlStates")]
	public static void ShowWindow() {
		ControlStatesInspector main = EditorWindow.GetWindow<ControlStatesInspector>();
		main.autoRepaintOnSceneChange = true;
		UnityEngine.Object.DontDestroyOnLoad(main);
	}

	public void OnGUI() {
		EditorGUILayout.BeginVertical(); {
			foreach (var kvp in ControlStates.GetAll()) {
				EditorGUILayout.LabelField(kvp.Key, kvp.Value + " Down: " + ControlStates.GetDown(kvp.Key) + " Up: " + ControlStates.GetUp(kvp.Key), GUILayout.MinWidth(650));
			}
		}
	}

}
