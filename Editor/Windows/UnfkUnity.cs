#if UNITY_EDITOR && !UNITY_WEBPLAYER
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class UnfkUnity : ZEditorWindow {
	
	public static string filepath { get { return Application.persistentDataPath + "/unfk.json"; } }
	public class Settings {
		public bool remove_p1ep = true;
		public Vector3 snap = new Vector3(1,1,1);
		public bool autoSnap = false;
		public float angleSnap = 15f;
		public float scaleSnap = .1f;

	}

	Settings data;
	float setAllPosSnap = -1f;

	[MenuItem("ZSharp/Windows/UnfkUnity")]
	public static void ShowWindow() { 
		EditorWindow.GetWindow(typeof(UnfkUnity)); 
	}
	
	public UnfkUnity() {

	}
	
	void OnGUI() {
		BeginVertical(); {
			Label("Bunch of useful stuff");
			Label("Settings stored in: " + Application.persistentDataPath);
			Button("Save", ()=>{
				var obj = Json.Reflect(data);
				File.WriteAllText(filepath, obj.PrettyPrint());
			});

			if (data == null) { ReloadData(); }
			data.remove_p1ep = Toggle(data.remove_p1ep, "Remove \" (1)\" from clones");
			
			BeginVertical("box"); {
				data.autoSnap = Toggle(data.autoSnap, "Auto-snap");
				data.snap = EditorGUILayout.Vector3Field("Snap", data.snap);
				setAllPosSnap = FloatField("Set All Axis", setAllPosSnap);
				if (setAllPosSnap > 0) {
					data.snap = new Vector3(setAllPosSnap, setAllPosSnap, setAllPosSnap);
					setAllPosSnap = -1;
				}

				data.angleSnap = FloatField("Angle", data.angleSnap);
				data.scaleSnap = FloatField("Scale", data.scaleSnap);
				Button("Snap", ()=>{SnapSelection();});
			} EndVertical();


		} EndVertical();
	}

	void SnapSelection() {
		foreach (var obj in Selection.transforms) {
			Snap(obj);
		}
	}
	void Snap(Transform t) {
		t.position = SnapVect(t.position, data.snap);
		t.rotation = Quaternion.Euler(SnapVect( t.rotation.eulerAngles, data.angleSnap));
		t.localScale = SnapVect(t.localScale, data.scaleSnap);
	}

	Vector3 SnapVect(Vector3 v, float snap) { return SnapVect(v, new Vector3( snap, snap, snap)); }
	Vector3 SnapVect(Vector3 v, Vector3 snap) {
		Vector3 pos = v;
		pos.x = SnapVal(pos.x, snap.x);
		pos.y = SnapVal(pos.y, snap.y);
		pos.z = SnapVal(pos.z, snap.z);
		return pos;
	}

	float SnapVal(float value, float snap) {
		if (snap > 0) {
			var lo = Mathf.Floor(value / snap) * snap;
			var hi = lo + snap;
			var loDiff = Mathf.Abs(lo - value);
			var hiDiff = Mathf.Abs(hi - value);
			return (loDiff < hiDiff) ? lo : hi;
		}
		return value;
	}
	
	void ReloadData() {
		data = new Settings();
		if (File.Exists(filepath)) {
			var json = File.ReadAllText(filepath);
			JsonObject obj = Json.Parse(json) as JsonObject;

			if (obj != null) {
				Json.ReflectInto(obj, data);

			}
		} else {

		}
	}

	void Update() {
		if (data == null) { ReloadData(); }

		if (Selection.activeGameObject != null) {
			
			if (data.remove_p1ep) {
				
				if (Selection.activeGameObject.name.Contains(" (1)")) {
					Selection.activeGameObject.name = Selection.activeGameObject.name.Replace(" (1)", "");
				}

			}
		
		}

		if (data.autoSnap) {
			SnapSelection();
		}

	}

	void OnInspectorUpdate() { }
	
	void OnFocus() { }
	void OnLostFocus() { }

	void OnSelectionChange() { }
	void OnHierarchyChange() { }
	void OnProjectChange() { }
	
	void OnDestroy() { }
	
}

#endif
