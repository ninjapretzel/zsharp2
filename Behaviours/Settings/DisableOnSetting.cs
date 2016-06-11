using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class DisableOnSetting : MonoBehaviour {
#if XtoJSON
	[Tooltip("What setting is checked?")]
	public string setting = "Particles";

	[Tooltip("Set to false to disable on given settings.")]
	public bool enableOnSettings = false;

	[Tooltip("What settings will enable/disable objects or behaviours?")]
	public string[] settings = { "low", "medium", "high" };
	
	//public List<string> toDisable = new List<string>();
	
	[Tooltip("Behaviours (on the same object) to enable/disable")]
	public List<Behaviour> toDisable = new List<Behaviour>();
	[Tooltip("Objects (should be children) to enable/disable")]
	public List<GameObject> toDisableObjects = new List<GameObject>();
	
	private bool lastEnabled = false;
	bool checkEnabled = false;
	
	public bool isEnabled { 
		get { 
			string set = Settings.instance[setting].stringVal.ToLower();;
			if (settings.Contains(set)) {
				return enableOnSettings;
			}
			return !enableOnSettings;
		} 
	}
	
	void Start() {
		if (!isEnabled) { Disable(); } else { Enable(); }
		lastEnabled = isEnabled;
		for (int i = 0; i < settings.Length; i++) {
			settings[i] = settings[i].ToLower();
		}
	}
	
	void Update() {
		if (Settings.changed) {
			if (isEnabled && !lastEnabled) {
				Enable();
			} else if (!isEnabled && lastEnabled) {
				Disable();
			}
		}
		checkEnabled = Settings.changed;

	}
	
	void LateUpdate() {
		if (checkEnabled) { lastEnabled = isEnabled; }
	}
	
	void Enable() { SetEnabled(true); }
	void Disable() { SetEnabled(false); }
	
	void SetEnabled(bool b) {
		foreach (Behaviour beh in toDisable) {
			beh.enabled = b;
		}

		foreach (GameObject gob in toDisableObjects) {
			gob.SetActive(b);
		}
	}

	/*
	void SetBehavioursEnabled(bool b) {
		foreach (string type in toDisable) {
			Component c = GetComponent(type) as Component;
			System.Type t = c.GetType();
			
			if (t.IsSubclassOf(typeof(Behaviour))) {
				Behaviour behaviour = c as Behaviour;
				behaviour.enabled = b;
			} else if (t.IsSubclassOf(typeof(Renderer))) {
				Renderer renderer = c as Renderer;
				renderer.enabled = b;
			} else if (t.IsSubclassOf(typeof(Collider))) {
				Collider collider = c as Collider;
				collider.enabled = b;
			} else if (t.IsSubclassOf(typeof(ParticleEmitter))) {
				ParticleEmitter emitter = c as ParticleEmitter;
				emitter.enabled = b;
			}
			
		}
		
	}
	//*/
#endif
}




















