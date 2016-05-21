using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class ModalDialog : MonoBehaviour {

	public string prompt;
	public Action<string> callback;

	public Text mainTextBox;

	public GameObject defaultButton;

	private Action onEscapeBackup;

	public void Start() {
		mainTextBox.text = prompt;
		onEscapeBackup = UGUIRoot.onEscape;
		UGUIRoot.onEscape = () => { };

		if (defaultButton != null) {
			EventSystem.current.SetSelectedGameObject(defaultButton);
		}
	}

	public void OnModalButtonClicked(string choice) {
		if (callback != null) {
			callback(choice);
		}
		UGUIRoot.onEscape = onEscapeBackup;
		Destroy(gameObject);

	}

}
