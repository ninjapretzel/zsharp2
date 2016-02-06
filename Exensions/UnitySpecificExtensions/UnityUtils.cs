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

	/// <summary> Set the active state of the game object of a given component </summary>
	/// <param name="c">Component of GameObject to change activation of </param>
	/// <param name="active">Active state to set </param>
	public static void SetObjectActive(this Component c, bool active) { c.gameObject.SetActive(active); }

	/// <summary> Try to set the enabled state of a given component </summary>
	/// <param name="c"> Component to set the enabled/disabled state of </param>
	/// <param name="enabled"> Enabled state to set </param>
	public static void TrySetEnabled(this Component c, bool enabled) {
		if (c != null) {
			if (c.GetType().IsSubclassOf(typeof(Behaviour))) {
				(c as Behaviour).enabled = enabled;
			}
			
			if (c.GetType().IsSubclassOf(typeof(Renderer))) {
				(c as Renderer).enabled = enabled;
			}
			
			if (c.GetType().IsSubclassOf(typeof(Collider))) {
				(c as Collider).enabled = enabled;
			}
		}
	}
	
	/// <summary> Try to disable the given component. </summary>
	/// <param name="c">Component to disable </param>
	public static void TryDisable(this Component c) { c.TrySetEnabled(false); }
	/// <summary> Try to enable the given component. </summary>
	/// <param name="c">Component to enable </param>
	public static void TryEnable(this Component c) { c.TrySetEnabled(true); }
	
	/// <summary> Duplicate a given gameObject </summary>
	/// <param name="c">Game object to duplicate </param>
	/// <returns> Copy created from duplication </returns>
	public static GameObject Duplicate(this GameObject c) { 
		GameObject g = (GameObject)GameObject.Instantiate(c, c.transform.position, c.transform.rotation);
		g.transform.parent = c.transform.parent;
		return g;
	}
	/// <summary> Duplicate a given component, and get a component of another type on the same object </summary>
	/// <typeparam name="T">Type of Component to get from the given object</typeparam>
	/// <param name="c">Component on gameObject to duplicate </param>
	/// <returns>Component of type T on the duplicate object</returns>
	public static T DuplicateAs<T>(this Component c) where T : Component { return c.gameObject.Duplicate().GetComponent<T>(); }
	/// <summary> Duplicate a given component, and get a component of the same type </summary>
	/// <typeparam name="T">Generic type, inferred from parameter 'c' </typeparam>
	/// <param name="c">Component to duplicate</param>
	/// <returns>Component analogous to 'c' on the duplicate object</returns>
	public static T Duplicate<T>(this T c) where T : Component { return c.gameObject.Duplicate().GetComponent<T>() as T; }

	/// <summary> Destroy the GameObject a given component is attached to </summary>
	/// <param name="c">Component to destroy</param>
	public static void Destroy(this Component c) { GameObject.Destroy(c.gameObject); }
	
	/// <summary>
	/// Steal - I mean get - a component from a child object with a given name
	/// </summary>
	/// <typeparam name="T">Component type to get</typeparam>
	/// <param name="c">Component on parent object </param>
	/// <param name="childName">Name of child GameObject</param>
	/// <returns>Component of type 'T' on child GameObject of 'c' with name 'childName'</returns>
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
		return ((check != null) ? check : c.gameObject.AddComponent<T>()) as T;
	}
	
	public static void SetColor(this Component c, string property, Color color) {
		foreach (Renderer r in c.GetComponentsInChildren<Renderer>() as Renderer[]) {
			if (r.material.HasProperty(property)) {
				r.material.SetColor(property, color);
			}
		}
	}

	public static void SetLayerRecursively(this GameObject go, int layer) {
		if (go == null) { return; }
		go.layer = layer;
		foreach (Transform t in go.transform) {
			t.gameObject.SetLayerRecursively(layer);
		}
	}
	
	
	
}
