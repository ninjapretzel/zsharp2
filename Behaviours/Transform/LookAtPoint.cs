using UnityEngine;
using System.Collections;

///Constantly makes a gameObject look at a givent target transform or point.
public class LookAtPoint : MonoBehaviour {
	public Transform target;
	public Vector3 targetV;
	
	void LateUpdate() {
		if (target) { transform.LookAt(target); }
		else { transform.LookAt(targetV); }
	}
}