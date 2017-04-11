using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

public class BindingsInspectorWindow : EditorWindow {

	private FieldInfo bindsField;
	private FieldInfo axisMappingsField;

	private bool bindsfold;
	private bool axesfold;

	public BindingsInspectorWindow() {
		titleContent = new GUIContent("BindingsInspectorWindow");
	}

	[MenuItem ("ZSharp/Console/Bindings Inspector")]
	public static void ShowWindow() {
		BindingsInspectorWindow main = EditorWindow.GetWindow<BindingsInspectorWindow>();
		main.autoRepaintOnSceneChange = true;
		UnityEngine.Object.DontDestroyOnLoad(main);
	}
	
	protected void OnGUI() {
		EditorGUILayout.BeginVertical(); {
			if (!EditorApplication.isPlaying) {
				EditorGUILayout.LabelField("Waiting for editor enter Play mode...");
				return;
			}

			if (bindsField == null) {
				Type consoleType = typeof(DevConsole);
				bindsField = consoleType.GetField("binds", BindingFlags.NonPublic | BindingFlags.Static);
				axisMappingsField = consoleType.GetField("axisMappings", BindingFlags.NonPublic | BindingFlags.Static);
			}
			
			Dictionary<KeyCode, string> binds = (Dictionary<KeyCode, string>)bindsField.GetValue(null);
			Dictionary<string, string> axisMappings = (Dictionary<string, string>)axisMappingsField.GetValue(null);
			
			bindsfold = EditorGUILayout.Foldout(bindsfold, "Bindings");
			if (bindsfold) {
				++EditorGUI.indentLevel;
				foreach (var kvp in binds) {
					EditorGUILayout.LabelField(kvp.Key.ToString(), kvp.Value);
				}
				--EditorGUI.indentLevel;
			}
			axesfold = EditorGUILayout.Foldout(axesfold, "Axis mappings");
			if (axesfold) {
				++EditorGUI.indentLevel;
				foreach (var kvp in axisMappings) {
					EditorGUILayout.LabelField(kvp.Key, kvp.Value);
				}
				--EditorGUI.indentLevel;
			}
		} EditorGUILayout.EndVertical();
	}
}
