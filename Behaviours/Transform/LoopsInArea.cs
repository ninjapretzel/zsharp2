using UnityEngine;
using System.Collections;

///Loops objects in an area, simulating an infinite scrollable repeatable world
public class LoopsInArea : MonoBehaviour {
	public Bounds area;
	
	
	void Start() {
		
	}
	
	void Update() {
		float x = transform.position.x;
		float y = transform.position.y;
		float z = transform.position.z;
		
		if (x > area.max.x) { x -= area.size.x; LoopX(); }
		if (y > area.max.y) { y -= area.size.y; LoopY(); }
		if (z > area.max.z) { z -= area.size.z; LoopZ(); }
		
		if (x < area.min.x) { x += area.size.x; LoopX(); }
		if (y < area.min.y) { y += area.size.y; LoopY(); }
		if (z < area.min.z) { z += area.size.z; LoopZ(); }
		
		transform.position = new Vector3(x,y,z);
		
	}
	
	void OnDrawGizmosSelected() {
		Gizmos.color = new Color(0, 0, 1, .1f);
		Gizmos.DrawCube(area.center, area.size);
	}
	

	void LoopX() {
		transform.SendMessage("OnLoopX", SendMessageOptions.DontRequireReceiver);
	}
	
	void LoopY() {
		transform.SendMessage("OnLoopY", SendMessageOptions.DontRequireReceiver);
	}
	
	void LoopZ() {
		transform.SendMessage("OnLoopZ", SendMessageOptions.DontRequireReceiver);
	}
	
}
