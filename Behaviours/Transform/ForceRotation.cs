using UnityEngine;
using System.Collections;

///Force an object's rotation to a given set of values when the object starts
public class ForceRotation : MonoBehaviour {
	public Vector3 euler = Vector3.zero;
	void Start() {
		transform.rotation = Quaternion.Euler(euler);
	}
	
}
