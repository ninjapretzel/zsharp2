using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class TransformMacros : Editor {
	
	[MenuItem ("Macros/Fix Parent Rotation &#f")]
	public static void FixParentRotation() {
		foreach (Transform t in Selection.transforms) {
			FixParentRotation(t);
		}
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
