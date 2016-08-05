using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FirstPage : MonoBehaviour {

	public string target;

	void Start() {
		foreach (var child in transform.GetChildren()) {
			child.gameObject.SetActive(false);
		}
		GetComponent<PageSwitcher>().Push(target);
	}
	
}
