using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Loader {
	
	static Dictionary<string, Transform> roots = new Dictionary<string,Transform>();

	public static Transform Forge(this Transform t) { 
		return Forge(t, Vector3.zero, Quaternion.identity, "Root"); 
	}

	public static Transform Forge(this Transform t, Vector3 pos) {
		return Forge(t, pos, Quaternion.identity, "Root");
	}

	public static Transform Forge(this Transform t, Vector3 pos, Quaternion rot, string root = "Root") {
		Transform made = GameObject.Instantiate(t, pos, rot) as Transform;
		Transform r;
		if (roots.ContainsKey(root) && roots[root] != null) {
			r = roots[root];
		} else {
			r = new GameObject(root).transform;
			roots[root] = r;
		}
		made.SetParent(r);
		return made;
	}

	
	
}
