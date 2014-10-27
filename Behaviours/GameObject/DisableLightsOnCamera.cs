using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DisableLightsOnCamera : MonoBehaviour {
	public List<Light> lights;
	Dictionary<Light, bool> states;
	
	void Awake() {
		states = new Dictionary<Light, bool>();
	}
	
	void OnPreCull() {
		states.Clear();
		foreach (Light l in lights) {
			states[l] = l.enabled;
			l.enabled = false; 
		}
	}
	
	void OnPostRender() {
		foreach (Light l in lights) { 
			l.enabled = states[l];
		}
	}
	
}
