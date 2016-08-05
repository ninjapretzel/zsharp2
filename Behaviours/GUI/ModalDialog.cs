using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class ModalDialog : MonoBehaviour {

	public static ModalDialog current = null;

	public string prompt;
	public Action<string> callback;

#if TMPRO
	public TMPro.TextMeshProUGUI mainTextBox;
#else
	public Text mainTextBox;
#endif

	public GameObject defaultButton;

	protected Action onEscapeBackup;

	public virtual void Start() {
		if (mainTextBox != null) {
			mainTextBox.text = prompt;
		}
		onEscapeBackup = UGUIRoot.onEscape;
		UGUIRoot.onEscape = () => { };
		
		EventSystem.current.SetSelectedGameObject(defaultButton);
	}

	public void OnModalButtonClicked(string choice) {
		if (callback != null) {
			callback(choice);
		}
		UGUIRoot.onEscape = onEscapeBackup;
		Destroy(gameObject);

	}

}
