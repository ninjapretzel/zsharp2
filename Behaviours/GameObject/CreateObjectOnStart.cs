using UnityEngine;
using System.Collections;



public class CreateObjectOnStart : MonoBehaviour {
	public Transform target;
	public Transform[] otherObjects;
	
	public Vector3 offset;
	public bool parentIt = false;
	public bool parentItToParent = false;
	public bool waitForFixedUpdate = false;
	
	void Start() {
		if (!waitForFixedUpdate) {
			Make();
		}
		
	}
	
	void FixedUpdate() {
		Make();
	}
	
	
	void Make() {
		Transform obj;
		if (target != null) {
			obj = Instantiate(target, transform.position + offset, transform.rotation) as Transform;
			if (parentIt) { 
				obj.parent = parentItToParent ? transform.parent : transform;
			}
		}
		
		foreach (Transform o in otherObjects) {
			obj = Instantiate(o, transform.position + offset, transform.rotation) as Transform;
			if (parentIt) { 
				obj.parent = parentItToParent ? transform.parent : transform;
			}
		}
		
		Destroy(this);
	}
}
