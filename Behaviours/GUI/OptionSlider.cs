using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

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

	Text label;
	Slider slider;
	

	void Start() {
		slider = GetComponent<Slider>();
		label = transform.Find("Label").GetComponent<Text>();
		
		slider.onValueChanged.AddListener(Set);
		slider.value = Settings.instance.Get<float>(settingName);
		//Set(slider.value);
	}
	
	void Update() {
		
	}

	void Set(float val) {
		Settings.instance.Apply(settingName, val);
		if (label != null) {
			label.text = prefix + string.Format(code, val * multiplier) + suffix;
			
		}
	}
	
}
