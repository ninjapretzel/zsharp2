using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

#if XtoJSON
[RequireComponent(typeof(GUIDrag), typeof(GUITooltip))]
#endif
public class GUIRoot : MonoBehaviour {
	
	private class WindowInfo { 
		public string name { get; private set; }
		public ZWindow window { get; private set; }
		public int type { get; private set; }
		public WindowInfo(string n, ZWindow w, int t) {
			name = n; window = w; type = t;
		}
	}

	public static GUIRoot main;

	public static Action pause;
	
	public static List<ZWindow> windows = new List<ZWindow>();
	static List<WindowInfo> delayedAdd = new List<WindowInfo>();
	static List<string> delayedRemove = new List<string>();

	private static Dictionary<string, ZWindow> binds = new Dictionary<string, ZWindow>();
	private static Dictionary<string, ZWindow> holdBinds = new Dictionary<string,ZWindow>();
	private static Dictionary<string, ZWindow> registeredWindows = new Dictionary<string,ZWindow>();

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
				Sprite sprite = Sprite.Create(Texture2D.whiteTexture, new Rect(0,0,1,1), new Vector2(0.5f, 0.5f));
				img.sprite = sprite;
				img.color = new Color32(32, 32, 32, 192);
				_modalContainer.gameObject.SetActive(false);
			}
			return _modalContainer;
		}
	}

	/// <summary> History of GameObjects that have been visited as menus. </summary>
	private static Stack<GameObject> history = new Stack<GameObject>();
	/// <summary> What is the current menu's GameObject </summary>
	private static GameObject active { get { return history.Peek(); } }

	/// <summary> Push a menu GameObject on the stack from static context. </summary>
	public static void Push(GameObject next) { main.Pushx(next); }
	/// <summary> Push a menu GameObject on the stack from a instance context </summary>
	public void Pushx(GameObject next) {
		if (history.Count > 0) {
			active.SetActive(false);
		}
		next.SetActive(true);
		history.Push(next);
	}

	//Gay shit to call Pop() from a referenced thing
	/// <summary> Pop a menu GameObject from static context, return nothing. </summary>
	public static void Popz() { main.Popy(); }
	/// <summary> Pop a menu GameObject from static context, return popped game object. </summary>
	public static GameObject Pop() { return main.Popy(); }

	/// <summary> Pop a menu GameObject from instance context, return nothing. </summary>
	public void Popx() { Popy(); }
	/// <summary> Pop a menu GameObject from instance context, return popped game object. </summary>
	public GameObject Popy() {
		if (history.Count <= 1) { return null; }

		GameObject obj = history.Pop();
		obj.SetActive(false);

		if (active != null) {
			active.SetActive(true);
		}

		return obj;
	}

	public void PopTo(GameObject target) {
		while (history.Contains(target) && active != target) {
			Pop();
		}
	}

	/// <summary> Switch to a given menu GameObject from instance context, return nothing. </summary>
	public void Switchx(GameObject obj) {
		Pop();
		Push(obj);
	}

	/// <summary> Switch to a given menu GameObject from instance context, return popped game object. </summary>
	public GameObject Switch(GameObject obj) {
		var popped = Pop();
		Push(obj);
		return popped;
	}
	
	/// <summary> Switch to a given menu GameObject from static context, return popped game object. </summary>
	public static GameObject Switchz(GameObject obj) {
		return main.Switch(obj);
	}

	/// <summary> Function to run when escape key is hit. Set to null to enable default behaviour. </summary>
	public static Action onEscape = Popz;

	public const int REGISTER = 0;
	public const int BIND = 1;
	public const int BINDHOLD = 2;
		
	public static void AddWindow(ZWindow window) {
		windows.Add(window);
	}

	void Awake() {
		if (main == null) {
			main = this;
			DontDestroyOnLoad(gameObject);
		} else {
			Destroy(gameObject);
		}
	}
	
	void Update() {
		while (delayedRemove.Count > 0) {
			_Remove(delayedRemove[0]);
			delayedRemove.RemoveAt(0);

		}

		if (onEscape == null) { onEscape = Popz; }

		if (Input.GetKeyDown(KeyCode.Escape)) {
			onEscape();
		}

		while (delayedAdd.Count > 0) {
			WindowInfo info = delayedAdd[0];
			delayedAdd.RemoveAt(0);

			if (info.type == REGISTER) { _Register(info.name, info.window); }
			if (info.type == BINDHOLD) { _BindHold(info.name, info.window); }
			if (info.type == BIND) { _BindKey(info.name, info.window); }
			
		}


		foreach (var pair in binds) {
			if (ControlStates.Get<bool>(pair.Key)) {
				ControlStates.Set(pair.Key, "false");
				pair.Value.open = !pair.Value.open;
			}
		}

		foreach (var pair in holdBinds) {
			pair.Value.open = ControlStates.Get<bool>(pair.Key);
		}

		foreach (var window in windows) {
			window.Update();
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

	void LateUpdate() {
		foreach (var window in windows) {
			window.LateUpdate();
		}
		
	}

	static void _Register(string key, ZWindow window) {
		registeredWindows[key] = window;
		if (!windows.Contains(window)) { windows.Add(window); }
	}

	static void _BindKey(string key, ZWindow window) {
		binds[key] = window;
		if (!windows.Contains(window)) { windows.Add(window); }
	}
	static void _BindHold(string key, ZWindow window) {
		holdBinds[key] = window;
		if (!windows.Contains(window)) { windows.Add(window); }
	}

	public static void Register(string key, ZWindow window) { delayedAdd.Add(new WindowInfo(key, window, REGISTER)); }
	public static void BindKey(string key, ZWindow window) { delayedAdd.Add(new WindowInfo(key, window, BIND)); }
	public static void BindHold(string key, ZWindow window) { delayedAdd.Add(new WindowInfo(key, window, BINDHOLD)); }
	
	public static bool HasWindow(string key) {
		return registeredWindows.ContainsKey(key)
			|| binds.ContainsKey(key)
			|| holdBinds.ContainsKey(key);
	}

	public static void Unbind(string key) {
		if (binds.ContainsKey(key)) {
			registeredWindows[key] = binds[key];
			binds.Remove(key);
		}
		if (holdBinds.ContainsKey(key)) {
			registeredWindows[key] = holdBinds[key];
			holdBinds.Remove(key);
		}
	}


	public static void Remove(string key) {
		delayedRemove.Add(key);
	}

	static void _Remove(string key) {
		ZWindow window = null;
		if (binds.ContainsKey(key)) {
			window = binds[key];
			windows.Remove(window);
			binds.Remove(key);
		}
		if (holdBinds.ContainsKey(key)) {
			window = holdBinds[key];
			windows.Remove(window);
			holdBinds.Remove(key);
		}
		if (registeredWindows.ContainsKey(key)) {
			window = registeredWindows[key];
			windows.Remove(window);
			registeredWindows.Remove(key);
		}
	}
	
	public static void HideAllWindows() {
		foreach (ZWindow window in windows) {
			window.open = false;
		}
	}
	
	public static void HideAllWindows(IEnumerable<ZWindow> windowsToHide) {
		foreach (ZWindow window in windowsToHide) {
			window.open = false;
		}
	}
	
	public static void ShowAllWindows(IEnumerable<ZWindow> windowsToShow) {
		foreach (ZWindow window in windowsToShow) {
			window.open = true;
		}
	}



	void OnGUI() {
		GUI.depth = 0;

		foreach (ZWindow window in windows) {
			window.Draw();
		}
		
#if XtoJSON
		JsonObject dragging = GUIDrag.dragging;

		if (dragging != null) {
			Texture2D back = Resources.Load<Texture2D>(dragging.GetString("back"));
			
			string iconString = dragging.GetString("icon");
			SpriteInfo iconInfo = Icons.GetIcon(iconString);
			Texture2D icon = null;
			if (iconInfo == null) {
				icon = Resources.Load<Texture2D>(iconString);
			}


			Color backColor = Json.GetValue<Color>(dragging["backColor"]);
			Color iconColor = Json.GetValue<Color>(dragging["iconColor"]);
			if (backColor == Color.clear) { backColor = Color.white; }
			if (iconColor == Color.clear) { iconColor = Color.white; }
			Vector3 mouse = Input.mousePosition;
			Rect brush = new Rect(mouse.x, mouse.y, 32, 32);
			
			if (back != null) { GUI.color = backColor; GUI.DrawTexture(brush, back); }
			if (iconInfo != null) {
				GUI.color = iconColor; GUI.DrawTexture(brush, iconInfo);
			} else {
				if (icon != null) { GUI.color = iconColor; GUI.DrawTexture(brush, icon); }
			}
		}
#endif

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
