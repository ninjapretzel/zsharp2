using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using System.Collections.Generic;


[ExecuteInEditMode]
public class OptionSelecter : MonoBehaviour {

	public string settingName = "dummySetting";
	

	public string defaultOption = "ULTRA";

	public int currentIndex = 0;
	public string currentOption { get { return options[currentIndex]; } }
	public List<string> options = new List<string>(new string[] {"LOW", "MEDIUM", "HIGH", "ULTRA"});

	Text display;
	Text label;
	
	string last;

	public static Settings sets { get { return Settings.instance; } }
#if XtoJSON
	public static string Get(string setting) {
		return sets[setting].stringVal;
	}
	public static void Set(string setting, string value) {
		Settings.instance.Apply(setting, value);
	}
#else
	public static string Get(string setting) {
		return "";
	}
	public static void Set(string setting, string value) {
		
	}
#endif

	void Start() {
		if (Application.isPlaying) {
			label = transform.Find("Label").GetComponent<Text>();
			label.text = gameObject.name;
			display = transform.Find("Display").GetComponent<Text>();
			string current = Get(settingName);
			if (current == null || current == "") {
				current = defaultOption;
			}

			for (int i = 0; i < options.Count; i++) {
				if (options[i] == current) {
					currentIndex = i;
					display.text = currentOption;
					break;
				}
			}

			//Settings.instance.Apply(settingName, currentOption);
		}

		
	}

	void Update() {
		if (!Application.isPlaying) {
			if (label == null) { label = transform.Find("Label").GetComponent<Text>(); }
			if (display == null) { display = transform.Find("Display").GetComponent<Text>(); }

			if (label != null) { label.text = gameObject.name; }
			if (display != null) { display.text = defaultOption; }
		} else {
			//Debug.Log(settingName + " : " + Settings.instance[settingName]);

			string setting = Get(settingName);

			if (setting != last) {
				currentIndex = options.IndexOf(setting);
				//Debug.Log(settingName + " Updated to " + setting + ":" + currentIndex);
				if (currentIndex <= -1) { currentIndex = 0; }
				display.text = setting;
			}
			last = setting;


		}

	}
	

	public void SelectNext() {
		currentIndex = (currentIndex + 1) % options.Count;
		Set();
	}

	public void SelectPrev() {
		currentIndex = (currentIndex - 1);
		if (currentIndex < 0) { 
			currentIndex += options.Count;
		}
		Set();
	}

	public void Set() {
		Set(settingName, currentOption);
		//Debug.Log(settingName + " set to " + currentOption);
		display.text = currentOption;
		


	}
}
