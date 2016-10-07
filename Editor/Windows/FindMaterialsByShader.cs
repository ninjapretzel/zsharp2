#if UNITY_EDITOR && !UNITY_WEBPLAYER
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class FindMaterialsByShader : ZEditorWindow {
	
	[MenuItem("Window/FindMaterialsByShader")]
	public static void ShowWindow() { 
		EditorWindow.GetWindow(typeof(FindMaterialsByShader)); 
	}
	
	public FindMaterialsByShader() {
		
	}

	string name = "";
	
	void OnGUI() { 
		name = TextField("Name of Shader", name);

		if (Button("Select")) {
			List<Material> list = new List<Material>();
			foreach (var matGUID in AssetDatabase.FindAssets("Material")) {
				var matPath = AssetDatabase.GUIDToAssetPath(matGUID);

				var mat = AssetDatabase.LoadAssetAtPath<Material>(matPath);

				if (mat != null && mat.shader.name == name) {
					list.Add(mat);
				}

				Debug.Log(list.Count());
			}
			Selection.objects = list.ToArray();


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
