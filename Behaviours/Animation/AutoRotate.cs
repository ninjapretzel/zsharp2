using UnityEngine;
using System.Collections;

///Simple behaviour- animates an object rotating at a constant speed.
public class AutoRotate : MonoBehaviour {
	public Vector3 speed = Vector3.zero;
	
	void Update () {
		transform.Rotate(speed * Time.deltaTime);
	}
	
	
}
