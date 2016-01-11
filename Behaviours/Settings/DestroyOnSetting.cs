using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Obsolete("Please use DisableOnSetting")]
public class DestroyOnSetting : MonoBehaviour {
	public string settingName = "Particles";

	public bool destroyOnSetting = false;
	public string[] settings = { "Low", "Medium" };
	
	
	public bool destroy { 
		get {
			return false;
		} 
	}
	
	void Start() {
		//if (destroy) { Destroy(gameObject); }
		
	}
}




















