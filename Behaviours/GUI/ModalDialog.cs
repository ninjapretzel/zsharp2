using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class ModalDialog : MonoBehaviour {

	public string prompt;
	public Action<string> callback;

	public Text mainTextBox;

	public void Start() {
		mainTextBox.text = prompt;

	}

	public void OnModalButtonClicked(string choice) {
		if (callback != null) {
			callback(choice);
		}
		Destroy(gameObject);

	}

}
