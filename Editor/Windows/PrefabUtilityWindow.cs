#if UNITY_EDITOR && !UNITY_WEBPLAYER
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class PrefabUtilityWindow : ZEditorWindow {
	
	[MenuItem("Window/PrefabUtilityWindow")]
	public static void ShowWindow() { 
		EditorWindow.GetWindow(typeof(PrefabUtilityWindow)); 
	}
	

	[System.NonSerialized] private Transform prefabObject;
	public PrefabUtilityWindow() {
		prefabObject = null;
	}
	
	void OnGUI() { 
		BeginVertical("box"); {
			Label("Replace Selection with Prefab instances");
			prefabObject = EditorGUILayout.ObjectField("Prefab to Replace with", prefabObject, typeof(Transform)) as Transform;

			Button("Replace Selection", ReplaceSelection);
		} EndVertical();
	}
	
	void ReplaceSelection() {
		 
		Transform temp = PrefabUtility.InstantiatePrefab(prefabObject) as Transform;
		List<GameObject> created = new List<GameObject>();

		foreach (Transform t in Selection.transforms) {
			Transform spawned = ZEditorHelpers.SpawnPrefab(temp, t);
			created.Add(spawned.gameObject);

			spawned.transform.SetParent(t.parent);
			Undo.RegisterCreatedObjectUndo(spawned, "Replace Selection");
			Undo.DestroyObjectImmediate(t.gameObject);
		}
		Selection.objects = created.ToArray();
		

		Undo.IncrementCurrentGroup();

		DestroyImmediate(temp.gameObject);
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
