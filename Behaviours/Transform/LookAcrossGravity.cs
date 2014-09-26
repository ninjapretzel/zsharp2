using UnityEngine;
using System.Collections;

///Makes an object look 'across' gravity
///The objects up vector will be -gravity
public class LookAcrossGravity : MonoBehaviour {
	Vector3 rotationAfter = Vector3.zero;
	
	void Awake() {
		transform.forward = -Physics.gravity.normalized;
		transform.Rotate(rotationAfter);
	}
	
}