using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DisableOnSetting : MonoBehaviour {
	public string setting = "showParticles";
	
	public List<string> toDisable = new List<string>();
	
	public bool invert = false;
	public bool lastEnabled = false;
	
	public bool isEnabled { 
		get { 
			bool e = Settings.custom[setting] == 1;
			if (invert) { e = !e; }
			return e;
		} 
	}
	
	void Start() {
		if (!isEnabled) { Disable(); }
		
	}
	
	void Update() {
		if (isEnabled && !lastEnabled) {
			Enable();
		} else if (!isEnabled && lastEnabled) {
			Disable();
		}
		
	}
	
	void LateUpdate() {
		lastEnabled = isEnabled;
	}
	
	void Enable() { SetBehavioursEnabled(true); }
	void Disable() { SetBehavioursEnabled(false); }
	
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
	
	
	
	
}




















