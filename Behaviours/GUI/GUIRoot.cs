using UnityEngine;
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
	
	public static List<ZWindow> windows = new List<ZWindow>();
	static List<WindowInfo> delayedAdd = new List<WindowInfo>();
	static List<string> delayedRemove = new List<string>();

	private static Dictionary<string, ZWindow> binds = new Dictionary<string, ZWindow>();
	private static Dictionary<string, ZWindow> holdBinds = new Dictionary<string,ZWindow>();
	private static Dictionary<string, ZWindow> registeredWindows = new Dictionary<string,ZWindow>();

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

		while (delayedAdd.Count > 0) {
			WindowInfo info = delayedAdd[0];
			delayedAdd.RemoveAt(0);

			if (info.type == REGISTER) { _Register(info.name, info.window); }
			if (info.type == BINDHOLD) { _BindHold(info.name, info.window); }
			if (info.type == BIND) { _BindKey(info.name, info.window); }
			
		}


		foreach (var pair in binds) {
			
			if (ControlStates.Get<bool>(pair.Key)) {
				pair.Value.open = !pair.Value.open;
			}
		}

		foreach (var pair in holdBinds) {
			pair.Value.open = ControlStates.Get<bool>(pair.Key);
		}

		foreach (var window in windows) {
			window.Update();
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
	
	
}
