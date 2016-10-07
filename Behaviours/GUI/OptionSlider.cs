using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
#if TMPRO
using TMPro;
#endif

public class OptionSlider : MonoBehaviour {

	public string settingName = "dummySetting";

	public string prefix = "";
	public string suffix = "";
	public float multiplier = 1;
	public int places = 2;

	public string code { 
		get {
			string str = "{0:0";
			if (places >= 1) { 
				str += ".";
				for (int i = 0; i < places; i++) { str += "0"; }
			}
			str += "}";
			return str;
		}
	}

	Text valLabel;
#if TMPRO
	TextMeshProUGUI valLabelTMPro;
#endif
	Slider slider;
	
#if XtoJSON
	public static float Get(string setting) {
		return Settings.instance.Get<float>(setting);
	}
	public static void Set(string setting, float value) {
		Settings.instance.Apply(setting, value);
	}
#else
	public static float Get(string setting) {
		return 0.0f;
	}
	public static void Set(string setting, float value) {

	}
#endif

	void Start() {
		slider = GetComponent<Slider>();
		valLabel = transform.Find("Value").GetComponent<Text>();
#if TMPRO
		valLabelTMPro = transform.Find("Value").GetComponent<TextMeshProUGUI>();
#endif

		slider.onValueChanged.AddListener(Set);
		slider.value = Get(settingName);
		Set(slider.value);

	}
	
	void Update() {
		
	}

	void Set(float value) {
		Set(settingName, value);
#if TMPRO
		if (valLabelTMPro != null) {
			valLabelTMPro.text = prefix + string.Format(code, value * multiplier) + suffix;
		} else
#endif
		if (valLabel != null) {
			valLabel.text = prefix + string.Format(code, value * multiplier) + suffix;
			
		}
	}
	
}
