using UnityEngine;
using System.Collections;

public class RotateOnStart : MonoBehaviour {
	public Vector3 rotation = new Vector3(0, 0, 0);
	void Start () {
		transform.Rotate(rotation);
		Destroy(this);
	}
	
}
