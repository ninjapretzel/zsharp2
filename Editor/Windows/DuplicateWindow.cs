#if UNITY_EDITOR && !UNITY_WEBPLAYER
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class DuplicateWindow : ZEditorWindow {
	
	[MenuItem("Window/Duplicate Wizard")]
	public static void ShowWindow() { 
		EditorWindow.GetWindow(typeof(DuplicateWindow)); 
	}
	
	public DuplicateWindow() {
		
	}
	
	int copies = 1;
	bool changeRotation;

	void OnGUI() {
		if (Button("Duplicate")) {
			Duplicate();
		}
	
		copies = IntField("Copies", copies);
		changeRotation = Toggle(changeRotation, "Change Rotation");
	}

	void Duplicate() {
		var target = Selection.activeTransform;
		Vector3 rotation = target.rotation.eulerAngles;
		Vector3 deltaRotation = Vector3.zero;
		if (changeRotation) { deltaRotation.y = 360f / ((float) copies); }
		var p = target.position;

		for (int i = 0; i < copies; i++) {
			var t = Instantiate(Selection.activeTransform, p, Quaternion.Euler(rotation) ) as Transform;
			t.parent = target.parent;
			rotation += deltaRotation;
		}

	}

	
	void Update() { }
	void OnInspectorUpdate() { }
	
	void OnFocus() { }
	void OnLostFocus() { }

	void OnSelectionChange() { }
	void OnHierarchyChange() { }
	void OnProjectChange() { }
	
	void OnDestroy() { }
	
}

#endif
