using UnityEngine;
using System.Collections;

///Constantly makes a gameObject look at a givent target transform or point.
public class LookAtCamera : MonoBehaviour {
	private Transform target;
	
	void LateUpdate() {
		target = Camera.main.transform;
		if (target != null) { transform.LookAt(target); }
	}
}