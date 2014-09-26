using UnityEngine;
using System.Collections;

///Makes an object follow another without being parented.
public class FakeParent : MonoBehaviour {
	public Transform target;
	public Vector3 offset = Vector3.zero;
	
	public bool doLate = false;
	
	public bool takeRotation = false;
	public Vector3 offsetRotation;
	
	void Update() {
		if (doLate) { return; }
		Move();
	}
	
	void LateUpdate() {
		if (!doLate) { return; }
		Move();
	}
	
	void Move() {
		if (target != null) {
			transform.position = target.position + offset;
			if (takeRotation) { 
				transform.rotation = target.rotation;
			}
			transform.Rotate(offsetRotation);
		}
		
	}
	
}
