using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class OptionToggle : MonoBehaviour {
	public string settingName = "dummySetting";
	
	Toggle toggle;

#if XtoJSON
	public static bool Get(string setting) {
		return Settings.instance.Get<bool>(setting);
	}

	public static void Set(string setting, bool value) {
		Settings.instance.Apply(setting, value);
	}
#else
	public static bool Get(string setting) {
		return false;
	}

	public static void Set(string setting, bool value) {

	}
#endif

	void Start() {
		toggle = GetComponent<Toggle>();
		toggle.onValueChanged.AddListener(Set);

		toggle.isOn = Get(settingName);

	}
	
	void Update() {
		
	}

	void Set(bool value) {
		Set(settingName, value);
	}
	
}
