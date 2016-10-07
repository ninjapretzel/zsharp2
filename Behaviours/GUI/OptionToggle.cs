using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class OptionToggle : MonoBehaviour {
	public string settingName = "dummySetting";
	
	Toggle toggle;

#if XtoJSON
	public virtual bool Get() {
		return Settings.instance.Get<bool>(settingName);
	}

	public virtual void Set(bool value) {
		Settings.instance.Apply(settingName, value);
	}
#else
	public virtual bool Get() {
		return false;
	}

	public virtual void Set(bool value) {

	}
#endif

	void Awake() {
		toggle = GetComponent<Toggle>();
		toggle.onValueChanged.AddListener(Set);

		toggle.isOn = Get();

	}

	
}
