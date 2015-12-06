using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnyKeyToSwitchTo : MonoBehaviour {

	public GameObject nextWindow;
	
	void Update() {
		if (Input.anyKey && !Input.GetKey(KeyCode.Escape)) {
			Switch();
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
