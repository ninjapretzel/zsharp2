using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GUIRoot : MonoBehaviour {
	
	public static GUIRoot main;
	
	public static List<ZWindow> windows = new List<ZWindow>();
	
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
		
		
		
		
	}
	
	
}
