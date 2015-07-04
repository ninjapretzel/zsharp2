using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(GUIDrag), typeof(GUITooltip))]
public class GUIRoot : MonoBehaviour {
	
	public static GUIRoot main;
	
	public static List<ZWindow> windows = new List<ZWindow>();
	
	public static Dictionary<string, ZWindow> binds = new Dictionary<string, ZWindow>();


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
		
	}
	
	public static void BindKey(string key, ZWindow window) {
		binds[key] = window;
	}
	
	public static void Unbind(string key) {
		if (binds.ContainsKey(key)) {
			binds.Remove(key);
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
			Texture2D icon = Resources.Load<Texture2D>(dragging.GetString("icon"));
			Color backColor = Json.GetValue<Color>(dragging["backColor"]);
			Color iconColor = Json.GetValue<Color>(dragging["iconColor"]);
			if (backColor == Color.clear) { backColor = Color.white; }
			if (iconColor == Color.clear) { iconColor = Color.white; }
			Vector3 mouse = Input.mousePosition;
			Rect brush = new Rect(mouse.x, mouse.y, 32, 32);
			
			if (back != null) { GUI.color = backColor; GUI.DrawTexture(brush, back); }
			if (icon != null) { GUI.color = iconColor; GUI.DrawTexture(brush, icon); }

		}
#endif
		

	}
	
	
}
