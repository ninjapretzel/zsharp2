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
	/// <summary> Shows a modal on the screen, with the intent of just displaying the prompt to the player. The <paramref name="responseHandler"/> always recieves the string "ok" </summary>
	/// <param name="responseHandler">An action to happen when the modal is closed. Always recieves the string "ok" </param>
	/// <param name="prompt">Prompt to show on the created modal.</param>
	/// <returns>Reference to the modal window that is displayed to the user.</returns>
	public static ModalDialog ShowAlertModal(Action<string> responseHandler, string prompt) {
		RectTransform modalTransform = Resources.Load<RectTransform>("AlertModal");
		if (modalTransform == null) {
			modalTransform = Resources.Load<RectTransform>("DefaultAlertModal");
		}
		return ShowModal(modalTransform, responseHandler, prompt);
	}

	/// <summary> Shows a modal on the screen, displaying a prompt to the player, and giving the choice of Yes/No. The <paramref name="responseHandler"/> recieves the strings "yes" and "no", respectively</summary>
	/// <param name="responseHandler">Action to happen when the user decides. Recieves the strings "yes" or "no" regardless of localization</param>
	/// <param name="prompt">Prompt to display to the user</param>
	/// <returns>Reference to the modal window that is displayed to the user</returns>
	public static ModalDialog ShowYesNoModal(Action<string> responseHandler, string prompt) {
		RectTransform modalTransform = Resources.Load<RectTransform>("YesNoModal");
		if (modalTransform == null) {
			modalTransform = Resources.Load<RectTransform>("DefaultYesNoModal");
		}
		return ShowModal(modalTransform, responseHandler, prompt);
	}

	/// <summary> Instantiate an arbitrary <paramref name="modalPrefab"/> to the user. </summary>
	/// <param name="modalPrefab">Prefab object representing modal window</param>
	/// <param name="responseHandler">Action to call once the user dismisses the modal somehow. Recieves an arbitrary string, decided by the <paramref name="modalPrefab"/>'s internals</param>
	/// <param name="prompt">Prompt to display, placed into the 'prompt' field of the ModalDialog attached to the <paramref name="modalPrefab"/></param>
	/// <returns>Reference to the modal window that is displayed to the user.</returns>
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
