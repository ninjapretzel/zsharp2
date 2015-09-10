using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class UGUIzTests {
	public static void Log(string s) {
		Debug.Log(s);
	}
}

public class UGUIzTextField : MonoBehaviour {

	MethodInfo onSend;
	InputField input;
	Component targetComponent;

	public string invokeOnSend = "UGUIzTests.Log";
	public GameObject targetObject;

	string[] assemblies = new string[] {
		",UnityEngine",
		",Assembly-UnityScript",
		",Assembly-CSharp",
		",Assembly-UnityScript-firstpass",
		",Assembly-CSharp-firstpass",
	};


	void Start() {
		input = GetComponent<InputField>();
		input.onValueChange.AddListener(OnEdit);

		string targetName = invokeOnSend.UpToLast('.');
		string targetThing = invokeOnSend.FromLast('.');

		Type targetClass = null;
		foreach (string assembly in assemblies) {
			targetClass = System.Type.GetType(targetName + assembly);
			if (targetClass != null) { break; }
		}

		if (targetClass == null) {
			Debug.LogWarning("UGUIzTextField.Start: Could not find class by the name of " + targetName);
			return;
		}

		if (targetObject != null && typeof(Component).IsAssignableFrom(targetClass)) {
			targetComponent = targetObject.GetComponent(targetClass);
		}

		onSend = targetClass.GetMethod(targetThing);
		if (onSend == null) { 
			Debug.Log("UGUIzTextField.Start: could not find method at " + invokeOnSend);
		}


		

	}
	
	void Update() {
		

	}

	public void OnEdit(string str) {
		if (input.text.EndsWith("\n")) {
			//Debug.Log("Line Ended" + input.text);
			if (onSend != null) {
				onSend.Invoke(targetObject, new object[] { input.text.UpToLast('\n') } );
			}
				
			input.text = "";
		}

	}
	
}
