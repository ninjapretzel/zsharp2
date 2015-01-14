using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class UnityUtils {
	public static Vector3 GetCenter(this BoxCollider c) {
		Vector3 pos = c.transform.position;
		Vector3 offset = Vector3.Scale(c.center, c.transform.lossyScale);
		return pos + c.transform.rotation * offset;
	}
	
	public static T GetComponentOnOrAbove<T>(this Component c) where T : Component {
		Transform test = c.transform;
		Component check;
		while (test != null) {
			check = test.GetComponent<T>();
			if (check) { return check as T; }
			test = test.parent;
		}
		return null;
	}
	
	public static T GetComponentAbove<T>(this Component c) where T : Component {
		Transform test = c.transform;
		Component check;
		while (test.parent != null) {
			test = test.parent;
			check = test.GetComponent<T>();
			if (check) { return check as T; }
		}
		return null;
	}
	public static void TrySetActive(this Component c, bool activate) {
		if (c != null) {
			if (c.GetType().IsSubclassOf(typeof(Behaviour))) {
				(c as Behaviour).enabled = activate;
			}
			
			if (c.GetType().IsSubclassOf(typeof(Renderer))) {
				(c as Renderer).enabled = activate;
			}
			
			if (c.GetType().IsSubclassOf(typeof(Collider))) {
				(c as Collider).enabled = activate;
			}
		}
	}
	
	public static void TryDeactivate(this Component c) { c.TrySetActive(false); }
	public static void TryActivate(this Component c) { c.TrySetActive(true); }
	
	public static GameObject Duplicate(this GameObject c) { 
		GameObject g = (GameObject)GameObject.Instantiate(c, c.transform.position, c.transform.rotation);
		g.transform.parent = c.transform.parent;
		return g;
	}
	public static T DuplicateAs<T>(this T c) where T : Component { return c.Duplicate<T>(); }
	public static T Duplicate<T>(this T c) where T : Component { 
		return c.gameObject.Duplicate().GetComponent<T>() as T;
	}

	
	public static void Destroy(this Component c) {
		GameObject.Destroy(c.gameObject);
	}
	
	public static T GrabFromChild<T>(this Component c, string childName) where T : Component {
		Transform t = c.transform.Find(childName);
		if (t != null) { return t.GetComponent<T>(); }
		return null;
	}
	
	public static void SendMSG(this Component c, string message) {
		c.SendMessage(message, SendMessageOptions.DontRequireReceiver);
	}
	
	public static void SendMSG(this Component c, string message, System.Object o) {
		c.SendMessage(message, o, SendMessageOptions.DontRequireReceiver);
	}
	
	public static void Broadcast(this Component c, string message) {
		c.SendMessage(message, SendMessageOptions.DontRequireReceiver);
	}
	
	public static void BroadcastAll(this Component c, string message) {
		c.BroadcastMessage(message, SendMessageOptions.DontRequireReceiver);
	}
	
	public static T AddComponent<T>(this Component c) where T : Component { return c.gameObject.AddComponent<T>(); }
	
	public static T Require<T>(this GameObject o) where T : Component { return o.transform.Require<T>(); }
	public static T Require<T>(this Component c) where T : Component {
		Component check = c.GetComponent<T>();
		return (check != null ? check : c.gameObject.AddComponent<T>()) as T;
		
	}
	
	public static void SetColor(this Component c, string property, Color color) {
		foreach (Renderer r in c.GetComponentsInChildren<Renderer>() as Renderer[]) {
			if (r.material.HasProperty(property)) {
				r.material.SetColor(property, color);
			}
		}
	}
	
	
	
}
