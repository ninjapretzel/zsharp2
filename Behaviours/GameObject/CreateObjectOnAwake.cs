using UnityEngine;
using System.Collections;

public class CreateObjectOnAwake : MonoBehaviour {
	public Transform target;
	public Vector3 offset;
	public bool parentIt = false;
	public string root = "Root";
	
	void Awake() {
		Transform obj = target.Forge(transform.position + offset, transform.rotation, root);
		if (parentIt) { obj.parent = transform.parent; }
		Destroy(this);
	}
	
}
