
#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class SortChildrenByName : MonoBehaviour {
	
	
	[MenuItem ("Edit/Sort Children By Name")]
	public static void Sort() {
		Transform selection = Selection.activeTransform;
		selection.SortChildrenByName();
	}
	
	
	
	
}

#endif