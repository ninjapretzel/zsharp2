using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

public class Switcher : MonoBehaviour {

	public GameObject nextWindow;
	public bool anyKey = false;

	public string keys;
	string[] keySplits;
	string split = "ALPHA,NUM";

	string[] ALPHA = { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
	string[] NUM = { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9" };




	void Update() {
		if (!ReferenceEquals(keys, split)) {
			keySplits = keys.Split(',');
			split = keys;
		}

		if (Input.anyKey && !Input.GetKey(KeyCode.Escape)) {
			if (anyKey) { Switch(); }
			else {
				if (keySplits.Contains("ALPHA")) {
					foreach (var code in ALPHA) { if (Input.GetKeyDown(code)) { Switch(); return; } }
				}
				if (keySplits.Contains("NUM")) {
					foreach (var code in NUM) { if (Input.GetKeyDown(code)) { Switch(); return; } }
				}

				foreach (var code in keySplits) {
					if (code == "ALPHA" || code == "NUM") { continue; }
					if (Input.GetKeyDown(code)) { Switch(); return; } 
				}

			}

		}

	}

	public void Switch() {
		if (nextWindow != null) {
			GUIRoot.Push(nextWindow);
		} else {
			GUIRoot.Pop();
		}
		
	}
	
}
