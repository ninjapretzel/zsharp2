using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class LocalizeText : MonoBehaviour {

	Language lastLang;
	Text uiText;
	string originalString;


	void Start() {
		uiText = GetComponent<Text>();

		if (uiText != null) {
			originalString = uiText.text;
			uiText.text = Localization.Localize(originalString);
		}
		
	}
	
	void Update() {
		if (Localization.language != lastLang) {
			uiText.text = Localization.Localize(originalString);
		}
	}
	

	void LateUpdate() {
		lastLang = Localization.language;
	}
	
	public void ChangeLanguage(string lang) {
		//Debug.Log("Changing to " + lang);
		Settings.instance.language = lang;
	}

}
