using UnityEngine;
using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

public class DontSaveAttribute : Attribute { }

public class SavesState : MonoBehaviour {

	private class SavedBehaviour {
		public Type type;
		public bool enabled;
		public Behaviour reference;
		public Dictionary<FieldInfo, object> fields;
	}

	public static bool restore = false;
	public static bool reset = false;
	public static bool save = false;
	
	public string[] blacklist = new string[0];
	
	// Transform properties
	[NonSerialized] private Vector3 position;
	[NonSerialized] private Quaternion rotation;
	[NonSerialized] private Vector3 scale;
	[NonSerialized] private Transform savedParent;
	[NonSerialized] private Vector3 velocity;
	[NonSerialized] private Vector3 angularVelocity;
	[NonSerialized] private bool resetTransform = false;
	[NonSerialized] private Vector3 initialPosition;
	[NonSerialized] private Quaternion initialRotation;
	[NonSerialized] private Vector3 initialScale;
	[NonSerialized] private Transform initialSavedParent;
	[NonSerialized] private Vector3 initialVelocity;
	[NonSerialized] private Vector3 initialAngularVelocity;
	// Attached Behaviours
	[NonSerialized] private List<SavedBehaviour> savedBehaviours = null;
	[NonSerialized] private List<SavedBehaviour> initialBehaviours = null;
	
	public void Start() {
		SaveStateInitial();
		
	}
	
	public void Update() {
		if (reset) {
			savedBehaviours = initialBehaviours;
			position = initialPosition;
			rotation = initialRotation;
			scale = initialScale;
			velocity = initialVelocity;
			angularVelocity = initialAngularVelocity;
			savedParent = initialSavedParent;
			Restore();
			gameObject.BroadcastMessage("OnReset", SendMessageOptions.DontRequireReceiver);
		} else if(restore) {
			Restore();
			gameObject.BroadcastMessage("OnRestore", SendMessageOptions.DontRequireReceiver);
		} else if(save) {
			SaveState();
		}
	}

	public void FixedUpdate() {
		if (resetTransform) {
			transform.parent = savedParent;
			transform.localPosition = position;
			transform.localRotation = rotation;
			transform.localScale = scale;
			
			Rigidbody rb = GetComponent<Rigidbody>();
			if (rb != null) {
				rb.velocity = velocity;
				rb.angularVelocity = angularVelocity;
			}
			resetTransform = false;
		}
	}
	
	public void LateUpdate() {
		restore = false;
		save = false;
		reset = false;
	}
	
	public void SaveState() {
		Behaviour[] behaviours = gameObject.GetComponents<Behaviour>();
		savedBehaviours = new List<SavedBehaviour>();
		// Save transform properties
		savedParent = transform.parent;
		position = transform.localPosition;
		rotation = transform.localRotation;
		scale = transform.localScale;
		Rigidbody rb = GetComponent<Rigidbody>();
		if (rb != null) {
			velocity = rb.velocity;
			angularVelocity = rb.angularVelocity;
		} else {
			velocity = Vector3.zero;
			angularVelocity = Vector3.zero;
		}
		foreach(Behaviour c in behaviours) {
			// Use reflection to get the Type of this
			string name = c.GetType().Name;
			bool blacklisted = false;
			foreach(string blacklistedClass in blacklist) {
				if(name == blacklistedClass) { blacklisted = true; break; }
			}
			if(blacklisted) { continue; }
			if(name != this.GetType().Name) {
				// Get all fields in this Behaviour
				FieldInfo[] fields = c.GetType().GetFields();
				Dictionary<FieldInfo, object> savedFields = new Dictionary<FieldInfo, object>();
				// Save them in a dictionary
				foreach(FieldInfo f in fields) {
					if (Attribute.GetCustomAttribute(f, typeof(DontSaveAttribute)) != null) { continue; }
					object o = f.GetValue(c);
					savedFields.Add(f, o);
				}
				SavedBehaviour newSavedBehaviour = new SavedBehaviour() {
					type = c.GetType(),
					enabled = c.enabled,
					reference = c,
					fields = savedFields
				};
				savedBehaviours.Add(newSavedBehaviour);
			}
		}
		
	}
	
	public void SaveStateInitial() {
		SaveState();
		initialBehaviours = savedBehaviours;
		initialPosition = position;
		initialRotation = rotation;
		initialScale = scale;
		Rigidbody rb = GetComponent<Rigidbody>();
		if (rb != null) {
			initialVelocity = rb.velocity;
			initialAngularVelocity = rb.angularVelocity;
		} else {
			initialVelocity = Vector3.zero;
			initialAngularVelocity = Vector3.zero;
		}
		initialSavedParent = savedParent;
		SaveState();
	}
	
	public void Restore() {
		// Restore transform properties
		resetTransform = true;
		// Get all Behaviours currently attached to this object
		Behaviour[] behaviours = gameObject.GetComponents<Behaviour>();
		foreach(Behaviour c in behaviours) {
			string name = c.GetType().Name;
			if (name == this.GetType().Name) { continue; }
			if (blacklist.Contains<string>(name)) { continue; }
			
			bool handled = false;
			foreach (SavedBehaviour b in savedBehaviours) {
				// If they are the same type but not the same reference
				if (b.type == c.GetType()) {
					handled = true;
					if (!object.ReferenceEquals(c, b.reference)) {
						Destroy(c);
						b.reference = gameObject.AddComponent(b.type) as Behaviour;
					}
					break;
				}
			}
			// If this component wasn't in the list of saved behaviours, we don't want it
			if (!handled) {
				Destroy(c);
			}
		}
		foreach (SavedBehaviour b in savedBehaviours) {
			if (b.reference == null) {
				b.reference = gameObject.AddComponent(b.type) as Behaviour;
			}
			b.reference.enabled = b.enabled;
			foreach(KeyValuePair<FieldInfo, object> kvp in b.fields) {
				// Fortunately, even though "current" is a Behaviour above, setting fields of derivative classes works through reflection
				kvp.Key.SetValue(b.reference, kvp.Value);
			}
		}
	
		
	}

	
}
