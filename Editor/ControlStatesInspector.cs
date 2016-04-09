using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;

public class ControlStatesInspector : EditorWindow {

	private PropertyInfo[] controlStates;

	public ControlStatesInspector() {
		titleContent = new GUIContent("Control States");
		controlStates = typeof(ControlStates).GetProperties();
		controlStates = Array.FindAll<PropertyInfo>(controlStates, x => { return x.GetGetMethod() != null; });
	}

	[MenuItem ("Tools/ControlStates")]
	public static void ShowWindow() {
		ControlStatesInspector main = EditorWindow.GetWindow<ControlStatesInspector>();
		main.autoRepaintOnSceneChange = true;
		UnityEngine.Object.DontDestroyOnLoad(main);
	}

	public void OnGUI() {
		EditorGUILayout.BeginVertical(); {
			foreach (PropertyInfo pinfo in controlStates) {
				EditorGUILayout.LabelField(pinfo.Name, pinfo.GetGetMethod().Invoke(null, new object[0]).ToString());
			}
		}
	}

}
