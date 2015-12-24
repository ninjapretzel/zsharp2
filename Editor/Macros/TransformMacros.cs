#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;




public class TransformMacros : Editor {
	[MenuItem("Macros/Snap Selection To Ground &v")]
	public static void SnapToGround() {

		foreach (Transform t in Selection.transforms) {
			RaycastHit rayhit;
			if (Physics.Raycast(t.position, Vector3.down, out rayhit)) {
				Undo.RecordObject(t, "Snap Selection to Ground");
				t.position = rayhit.point;
			}
		}
		Undo.IncrementCurrentGroup();
	}
	

	[MenuItem ("Macros/Fix Parent Rotation &#f")]
	public static void FixParentRotation() {
		foreach (Transform t in Selection.transforms) {
			Undo.RecordObject(t, "Fix Parent Rotation");
			FixParentRotation(t);
		}
		Undo.IncrementCurrentGroup();
	}

	[MenuItem("Macros/Duplicate &d")]
	public static void Duplicate() {
		//int group = Undo.GetCurrentGroup();
		
		List<GameObject> copies = new List<GameObject>();
		foreach (Transform t in Selection.transforms) {
			Transform tcopy = Instantiate(t, t.position, t.rotation) as Transform;
			tcopy.SetParent(t.parent);
			tcopy.gameObject.name = t.gameObject.name;
			copies.Add(tcopy.gameObject);
			Undo.RegisterCreatedObjectUndo(tcopy.gameObject, "Duplicate");
		}

		Selection.objects = copies.ToArray();
		
		Undo.IncrementCurrentGroup();
	}

	[MenuItem("Macros/Toggle Active State &q")]
	public static void ToggleActive() {
		GameObject sel = Selection.activeGameObject;
		Undo.RecordObject(sel, "Toggle Active State");
		sel.SetActive(!sel.activeSelf);
	}

	
	public static void FixParentRotation(Transform target) {
		Transform dummy = new GameObject("Dummy").transform;
		dummy.SetParent(target.parent);
		
		//dummy.parent = target.parent;
		dummy.localPosition = Vector3.zero;
		
		if (target != null) {
			Transform[] children = target.GetComponentsInChildren<Transform>();
			
			//Detach direct children
			foreach (Transform child in children) {
				if (child == target) { continue; }
				if (child.parent == target) {
					child.SetParent(dummy, true);
					//child.parent = dummy;
				}
			}
			
			target.rotation = Quaternion.identity;
			
			//Reattach direct children
			foreach (Transform child in children) {
				if (child == target) { continue; }
				if (child.parent == dummy) {
					child.SetParent(target, true);
					//child.parent = target;
				}
			}
			
			DestroyImmediate(dummy.gameObject);
			
			
		}
	
	}
	
}



#endif
