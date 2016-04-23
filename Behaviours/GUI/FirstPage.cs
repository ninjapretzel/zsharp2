using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FirstPage : MonoBehaviour {

	public GameObject target;

	void Start() {
		foreach (Transform t in transform.GetChildren()) {
			//Awake Object...
			t.gameObject.SetActive(true);
			//Sleep Object...
			t.gameObject.SetActive(false);
		}
		
		UGUIRoot.main.Push(target);
	}


	
}
