using UnityEngine;
using System.Collections;
using System.Collections.Generic;

#if XtoJSON
[RequireComponent(typeof(GUIDrag), typeof(GUITooltip))]
#endif
public class GUIRoot : MonoBehaviour {
	
	public static GUIRoot main;
	
	public static List<ZWindow> windows = new List<ZWindow>();
	
	public static Dictionary<string, ZWindow> binds = new Dictionary<string, ZWindow>();
	public static Dictionary<string, ZWindow> holdBinds = new Dictionary<string,ZWindow>();
	public static Dictionary<string, ZWindow> registeredWindows = new Dictionary<string,ZWindow>();


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
		
		foreach (var pair in binds) {
			
			if (Input.GetKeyDown(pair.Key)) {
				pair.Value.open = !pair.Value.open;
			}
		}

		foreach (var pair in holdBinds) {
			pair.Value.open = Input.GetKey(pair.Key);
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

	public static void Register(string key, ZWindow window) {
		registeredWindows[key] = window;
		if (!windows.Contains(window)) { windows.Add(window); }
	}
	
	public static void BindKey(string key, ZWindow window) {
		binds[key] = window;
		if (!windows.Contains(window)) { windows.Add(window); }
	}
	
	public static void BindHold(string key, ZWindow window) {
		holdBinds[key] = window;
		if (!windows.Contains(window)) { windows.Add(window); }
	}

	public static void Unbind(string key) {
		if (binds.ContainsKey(key)) {
			binds.Remove(key);
		}
		if (holdBinds.ContainsKey(key)) {
			holdBinds.Remove(key);
		}
	}

	public static ZWindow Remove(string key) {
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
		return window;
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
