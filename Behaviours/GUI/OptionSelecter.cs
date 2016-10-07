using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;

[ExecuteInEditMode]
public class OptionSelecter : Selectable {

	public string settingName = "dummySetting";
	
	public string defaultOption = "ULTRA";

	public int currentIndex = 0;
	public string currentOption { get { return options[currentIndex]; } }
	public List<string> options = new List<string>(new string[] {"LOW", "MEDIUM", "HIGH", "ULTRA"});

	protected Text display;
	protected Text label;
	
	string last;

	public static Settings sets { get { return Settings.instance; } }
#if XtoJSON
	public virtual string Get(string setting) {
		if (sets[setting].isString) {
			return sets[setting].stringVal;
		} 

		return sets[setting].ToString();
	}
	public virtual void Set(string setting, string value) {
		Settings.instance.Apply(setting, value);
	}
#else
	public static string Get(string setting) {
		return "";
	}
	public static void Set(string setting, string value) {
		
	}
#endif

	protected override void Start() {
		if (Application.isPlaying) {
			base.Start();

			Transform t;
			t = transform.Find("Label");
			if (t == null) { t = transform.Find("LLabel"); }
			label = t.GetComponent<Text>(); 
			label.text = gameObject.name.FromFirst("OptionSelector");

			t = transform.Find("Display");
			if (t == null) { t = transform.Find("DDisplay"); }
			display = t.GetComponent<Text>();
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

	protected virtual void Update() {
		if (label == null) { 
			var t = transform.Find("Label");
			if (t == null) { t = transform.Find("LLabel"); }
			label = t.GetComponent<Text>(); 
		}
		if (display == null) {
			var t = transform.Find("Display");
			if (t == null) { t = transform.Find("DDisplay"); }
			display = t.GetComponent<Text>(); 
		}

		if (!Application.isPlaying) {
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
		Debug.Log(settingName + " set to " + currentOption);
		
	}

	public override void OnMove(AxisEventData eventData) {
		if (!IsActive() || !IsInteractable()) {
			base.OnMove(eventData);
			return;
		}
		
		switch (eventData.moveDir) {
			case MoveDirection.Left: {
				SelectPrev();
				break;
			}
			case MoveDirection.Right: {
				SelectNext();
				break;
			}
			case MoveDirection.Up:
			case MoveDirection.Down: {
				base.OnMove(eventData);
				break;
			}
		}
	}

}
