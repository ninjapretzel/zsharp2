using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AppliesReflectionProbeSettings : MonoBehaviour {


	ReflectionProbe probe;
	int last;
	void Start() {
		probe = GetComponent<ReflectionProbe>();
		Set();
	}
	
	void Update() {
		Set();
	}

	void Set() {
		
		int setting = Settings.instance["ReflectionSize"].intVal;
		
		if (setting != last) {
			probe.resolution = setting;
			last = setting;
		}
	}
}
