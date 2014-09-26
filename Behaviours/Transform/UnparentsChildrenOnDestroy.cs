using UnityEngine;
using System.Collections;

public class UnparentsChildrenOnDestroy : MonoBehaviour {
	private static bool quitting = false;
	
	void OnApplicationQuit() {
		quitting = true;
	}
	
	void OnDestroy() {
		if (quitting) { return; }
		transform.DetachChildren();
	}
}
