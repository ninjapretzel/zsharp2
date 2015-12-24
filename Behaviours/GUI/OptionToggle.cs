using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class OptionToggle : MonoBehaviour {
	public string settingName = "dummySetting";
	
	Toggle toggle;

	void Start() {
		toggle = GetComponent<Toggle>();
		toggle.onValueChanged.AddListener(Set);

		toggle.isOn = Settings.instance.Get<bool>(settingName);

	}
	
	void Update() {
		
	}

	void Set(bool val) {
		Settings.instance.Apply(settingName, val);
	}
	
}
