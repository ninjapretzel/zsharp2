using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DisableOnPlatform : MonoBehaviour {
	
	public bool android = false;
	public bool ios = false;
	
	void Start() {
		if (android && Application.platform == RuntimePlatform.Android) { Destroy(gameObject); }
		if (ios && Application.platform == RuntimePlatform.IPhonePlayer) { Destroy(gameObject); }
	}
	
	
}
