using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class ModalDialog : MonoBehaviour {

	public string prompt;
	public Action<string> callback;

	public Text mainTextBox;

	private Action onEscapeBackup;

	public void Start() {
		mainTextBox.text = prompt;
		onEscapeBackup = UGUIRoot.onEscape;
		UGUIRoot.onEscape = () => { };

	}

	public void OnModalButtonClicked(string choice) {
		if (callback != null) {
			callback(choice);
		}
		UGUIRoot.onEscape = onEscapeBackup;
		Destroy(gameObject);

	}

}
