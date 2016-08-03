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
				_modalContainer = new GameObject("M0dalz");
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
	public static Action defaultOnEscape { get { return () => { Sounds.Play("MenuBack"); main.PopAndDestroy(); }; } }

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

		if (modalContainer != null) {
			modalContainer.transform.SetSiblingIndex(transform.childCount - 1);
			if (modalContainer.transform.childCount != 0) {
				for (int i = 0; i < modalContainer.transform.childCount; ++i) {
					if (i == 0) {
						ModalDialog.current = modalContainer.transform.GetChild(i).GetComponent<ModalDialog>();
						modalContainer.transform.GetChild(i).gameObject.SetActive(true);
					} else {
						modalContainer.transform.GetChild(i).gameObject.SetActive(false);
					}
				}
			} else {
				modalContainer.SetActive(false);
			}
		}
	}

	public static void TogglePause() {
		if (pause != null) {
			pause();
		}
	}

	#region ModalMethods
	public static ModalDialog ShowAlertModal(Action<string> responseHandler, string prompt) {
		RectTransform modalTransform = Resources.Load<RectTransform>("AlertModal");
		if (modalTransform == null) {
			modalTransform = Resources.Load<RectTransform>("DefaultAlertModal");
		}
		return ShowModal(modalTransform, responseHandler, prompt);
	}

	public static ModalDialog ShowYesNoModal(Action<string> responseHandler, string prompt) {
		RectTransform modalTransform = Resources.Load<RectTransform>("YesNoModal");
		if (modalTransform == null) {
			modalTransform = Resources.Load<RectTransform>("DefaultYesNoModal");
		}
		return ShowModal(modalTransform, responseHandler, prompt);
	}

	public static ModalDialog ShowModal(RectTransform modalPrefab, Action<string> responseHandler, string prompt) {
		RectTransform modal = Instantiate(modalPrefab) as RectTransform;
		modal.gameObject.name = modal.gameObject.name.Replace("(Clone)", "");
		if (!modal.gameObject.name.EndsWith("Modal")) {
			modal.gameObject.name += "Modal";
		}
		modalContainer.SetActive(true);
		modal.SetParent(modalContainer.transform);
		modal.offsetMin = Vector2.zero;
		modal.offsetMax = Vector2.zero;
		ModalDialog dialog = modal.GetComponent<ModalDialog>();
		dialog.prompt = prompt;
		dialog.callback = responseHandler;
		modal.SetAsFirstSibling();
		return dialog;
	}
	#endregion
	
}
