using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary> Class to handle Scene Folders and create objects into a given 'folder'. </summary>
public static class Loader {

	/// <summary> All active Scene 'Folder' roots </summary>
	static Dictionary<string, Transform> roots = new Dictionary<string,Transform>();

	const string DEFAULT_ROOT = "Root";

	/// <summary> Instantiate an object and stick it under the given root. </summary>
	public static Transform Forge(this Transform t, string root = DEFAULT_ROOT) {
		return Forge(t, Vector3.zero, Quaternion.identity, root); 
	}
	
	/// <summary> Instantiate an object at a position and stick it under the given root. </summary>
	public static Transform Forge(this Transform t, Vector3 pos, string root = DEFAULT_ROOT) {
		return Forge(t, pos, Quaternion.identity, root);
	}

	/// <summary> Instantiate an object at a position and rotation, and stick it under the given root. </summary>
	public static Transform Forge(this Transform t, Vector3 pos, Quaternion rot, string root = DEFAULT_ROOT) {
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
