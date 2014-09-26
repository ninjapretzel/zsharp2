using UnityEngine;
using System.Collections;


///Makes an object stick to a surface when the game starts.
///Uses a raycast to stick the object to the surface.
///Automatically removes this behaviour from the object after it does its action.
public class StickToSurfaceOnStart : MonoBehaviour {
	
	///Raycast Direction
	public Vector3 direction = Vector3.down;
	///Offset to move object from raycast hit point
	public Vector3 offset = Vector3.zero;
	///Distance of raycast
	public float maxDistance = 20;
	
	///Set transform.up to raycasthit.normal?
	public bool changeRotation = false;
	///Does this object have a 'z-up' coord system model attached?
	public bool useZUp = false;
	
	///Override the default layermask with a custom one?
	public LayerMask layers = Physics.DefaultRaycastLayers;
	///Delay the action until the next fixed update (if the object will be spawned in)
	public bool waitForFixedUpdate = false;
	
	///Option to do the stick in the editor (pre-position the object)
	#if UNITY_EDITOR
	public bool DO_IT_NOW = false;
	#endif
	
	void Start() {
		if (!waitForFixedUpdate) {
			Stick();
		}
		
	}
	
	void FixedUpdate() {
		Stick();
	}
	
	#if UNITY_EDITOR
	void OnDrawGizmos() {
		if (DO_IT_NOW) {
			DO_IT_NOW = false;
			Stick();
		}
		
	}
	#endif
	void Stick() {
		RaycastHit hit;
		if (Physics.Raycast(transform.position, direction, out hit, maxDistance, layers)) {
			transform.position = hit.point + offset;
			if (changeRotation) {
				if (useZUp) {
					transform.forward = hit.normal;
				
				} else {
					transform.up = hit.normal;
				}
			}
		}
		
		if (Application.isPlaying) {
			Destroy(this);
		}
	}
	
	
}
