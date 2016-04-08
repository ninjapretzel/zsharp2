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
			lastLang = Localization.language;
		}
		
	}
	
	void Update() {
		if (Localization.language != lastLang) {
			uiText.text = Localization.Localize(originalString);
			lastLang = Localization.language;
		}
	}
	
	public void ChangeLanguage(string lang) {
		//Debug.Log("Changing to " + lang);
		Settings.instance.language = lang;
	}

}
