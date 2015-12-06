using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FirstPage : MonoBehaviour {

	public GameObject target;

	void Start() {
		foreach (Transform t in transform.GetChildren()) {
			t.gameObject.SetActive(false);
		}
		GUIRoot.Push(target);
	}


	
}
