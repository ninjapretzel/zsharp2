using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FirstPage : MonoBehaviour {

	public string target;

	void Start() {
		GetComponent<PageSwitcher>().Push(target);
	}
	
}
