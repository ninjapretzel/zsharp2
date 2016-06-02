using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityStandardAssets.ImageEffects;

public class AppliesPostSettings : MonoBehaviour {
#if XtoJSON
	static Dictionary<string, bool> settings = new Dictionary<string,bool>();
	public static void Set(string key, bool val) { 
		settings[key] = val; 
		//Debug.Log("Post " + key + " : " + val);
	}

	[System.Serializable] public class PostSettingData {
		public string setting;
		public Behaviour target;
		[System.NonSerialized] public bool last = false;

		public void Toggle() {
			target.enabled = !last;
			last = !last;
		}
	}

	public PostSettingData[] sets;

	Camera cam;
	Antialiasing fxaa;

	void Start() {
		cam = GetComponent<Camera>();
		fxaa = GetComponent<Antialiasing>();

		foreach (var set in sets) {
			set.last = set.target.enabled;
		}

		if (Settings.fxaaMode == AAMode.DLAA) {
			fxaa.enabled = false;
		} else {
			fxaa.enabled = true;
		}
		fxaa.mode = Settings.fxaaMode;
	}
	
	void Update() {
		if (cam != null && cam.renderingPath != Settings.renderPath) {
			cam.renderingPath = Settings.renderPath;
		}


		if (fxaa != null && fxaa.mode != Settings.fxaaMode) {
			if (Settings.fxaaMode == AAMode.DLAA) {
				fxaa.enabled = false;
			} else {
				fxaa.enabled = true;
			}
			fxaa.mode = Settings.fxaaMode;
		}

		foreach (var set in sets) {
			//Debug.Log("set " + set.setting);
			if (set.last != settings[set.setting]) { set.Toggle(); }
		}
	}
#endif
}
