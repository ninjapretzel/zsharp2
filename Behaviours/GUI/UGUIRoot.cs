using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class UGUIRoot : PageSwitcher {

	public static UGUIRoot main;
	private static GameObject _modalContainer;
	public static GameObject modalContainer {
		get {
			if (_modalContainer == null) {
				_modalContainer = new GameObject("ModalContainer");
				_modalContainer.transform.SetParent(main.transform);
				RectTransform rt = _modalContainer.AddComponent<RectTransform>();
				rt.anchorMin = Vector2.zero;
				rt.anchorMax = Vector2.one;
				rt.offsetMin = Vector2.zero;
				rt.offsetMax = Vector2.zero;
				Image img = _modalContainer.AddComponent<Image>();
				Sprite sprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0, 0, 1, 1), new Vector2(0.5f, 0.5f));
				img.sprite = sprite;
				img.color = new Color32(32, 32, 32, 192);
				_modalContainer.gameObject.SetActive(false);
			}
			return _modalContainer;
		}
	}
	/// <summary> Default function to run when the escape key is hit </summary>
	public static Action defaultOnEscape { get { return () => { Sounds.Play("MenuBack"); main.Pop(); }; } }

	/// <summary> Function to run when escape key is hit. Set to null to enable default behaviour. </summary>
	public static Action onEscape = defaultOnEscape;

	void Awake() {
		main = this;
	}

	void Start() {
		
	}
	
	void Update() {
		if (onEscape == null) { onEscape = defaultOnEscape; }

		if (Input.GetKeyDown(KeyCode.Escape)) {
			onEscape();
		}

		if (_modalContainer != null) {
			_modalContainer.transform.SetSiblingIndex(transform.childCount - 1);
			if (_modalContainer.transform.childCount != 0) {
				foreach (Transform child in _modalContainer.transform) {
					child.gameObject.SetActive(false);
				}
				_modalContainer.transform.GetChild(0).gameObject.SetActive(true);
			} else {
				_modalContainer.SetActive(false);
			}
		}
	}

	public static void TogglePause() {
		if (pause != null) {
			pause();
		}
	}

	#region ModalMethods
	public static void ShowAlertModal(Action<string> responseHandler, string prompt) {
		RectTransform modalTransform = Resources.Load<RectTransform>("AlertModal");
		if (modalTransform == null) {
			modalTransform = Resources.Load<RectTransform>("DefaultAlertModal");
		}
		ShowModal(modalTransform, responseHandler, prompt);
	}

	public static void ShowYesNoModal(Action<string> responseHandler, string prompt) {
		RectTransform modalTransform = Resources.Load<RectTransform>("YesNoModal");
		if (modalTransform == null) {
			modalTransform = Resources.Load<RectTransform>("DefaultYesNoModal");
		}
		ShowModal(modalTransform, responseHandler, prompt);
	}

	public static void ShowModal(RectTransform modalPrefab, Action<string> responseHandler, string prompt) {
		RectTransform modal = Instantiate(modalPrefab) as RectTransform;
		modalContainer.SetActive(true);
		modal.SetParent(modalContainer.transform);
		modal.offsetMin = Vector2.zero;
		modal.offsetMax = Vector2.zero;
		ModalDialog dialog = modal.GetComponent<ModalDialog>();
		dialog.prompt = prompt;
		dialog.callback = responseHandler;

	}
	#endregion
	
}
