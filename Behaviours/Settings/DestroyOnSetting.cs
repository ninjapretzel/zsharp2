using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DestroyOnSetting : MonoBehaviour {
	public string setting = "showParticles";
	
	public bool invert = false;
	
	public bool destroy { 
		get { 
			bool d = !(Settings.custom[setting] == 1);
			if (invert) { d = !d; }
			return d;
		} 
	}
	
	void Start() {
		if (destroy) { Destroy(gameObject); }
		
	}
	
}




















