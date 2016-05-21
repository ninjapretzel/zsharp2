using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
#if TMPRO
using TMPro;
#endif

public class LocalizeText : MonoBehaviour {

	Language lastLang;
	Text uiText;
#if TMPRO
	TextMeshPro proText;
	TextMeshProUGUI uiProText;
#endif

	string originalString;
	public bool noMarkup = false;
	

	void Start() {
		uiText = GetComponent<Text>();
#if TMPRO
		proText = GetComponent<TextMeshPro>();
		uiProText = GetComponent<TextMeshProUGUI>();

		if (proText != null) {
			originalString = proText.text;
			if (noMarkup) { proText.text = Localization.Localize_NOMARKUP(originalString); }
			else { proText.text = Localization.Localize(originalString); }
			
			lastLang = Localization.language;
		} else if (uiProText != null) {
			originalString = uiProText.text;
			if (noMarkup) { uiProText.text = Localization.Localize_NOMARKUP(originalString); }
			else { uiProText.text = Localization.Localize(originalString); }

			lastLang = Localization.language;
		} else //LOL conditional compiled else if chain... 
#endif	
		if (uiText != null) {
			originalString = uiText.text;
			if (noMarkup) { uiText.text = Localization.Localize_NOMARKUP(originalString); }
			else { uiText.text = Localization.Localize(originalString); }
			lastLang = Localization.language;
		}
		
	}
	
	void Update() {
		if (Localization.language != lastLang) {
#if TMPRO
			if (proText != null) {
				if (noMarkup) { proText.text = Localization.Localize_NOMARKUP(originalString); }
				else { proText.text = Localization.Localize(originalString); }
			} else if (uiProText != null) {
				if (noMarkup) { uiProText.text = Localization.Localize_NOMARKUP(originalString); }
				else { uiProText.text = Localization.Localize(originalString); }
			} else //goddamnit why must I do this?
#endif
			if (uiText != null) {
				if (noMarkup) { uiText.text = Localization.Localize_NOMARKUP(originalString); }
				else { uiText.text = Localization.Localize(originalString); }
			}


			lastLang = Localization.language;
		}
	}
	
	public void ChangeLanguage(string lang) {
		//Debug.Log("Changing to " + lang);
		Settings.instance.language = lang;
	}

}
