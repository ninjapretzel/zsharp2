using UnityEngine;
using System.Collections;

///Forces the rotation of an object to a value constantly.
public class ForceRotationConstant : MonoBehaviour {
	public Vector3 euler = Vector3.zero;
	
	void LateUpdate() {
		transform.rotation = Quaternion.Euler(euler);
	}
	
}
