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
	
	void OnGUI() {
		GUI.depth = 0;
		
		foreach (ZWindow window in windows) {
			window.Draw();
		}
		
		
		
		
	}
	
	
}
