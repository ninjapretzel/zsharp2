using UnityEngine;
using System.Collections;

public class TouchScrollMovesObject : MonoBehaviour {
	public float sensitivity = 1;
	public float mobileSensitivity = 4;
	public float dampening = 3;
	public Vector2 velocity;
	public bool invertx = false;
	public bool inverty = false;
	
	Vector2 lastMouse = new Vector2(0, 0);
	
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		Vector2 diff;
		if (Input.touches.Length > 0) {
			if (Input.touches[0].phase == TouchPhase.Moved) {
				diff = Input.touches[0].deltaPosition;
				if (invertx) { diff.x *= -1; }
				if (inverty) { diff.y *= -1; }
				
				velocity += diff * mobileSensitivity;
			}
		} else {
			Vector2 mouse = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
			if (Input.GetMouseButtonDown(0)) { lastMouse = mouse; return; }
			if (Input.GetMouseButton(0)) {
				diff = mouse - lastMouse;
				if (invertx) { diff.x *= -1; }
				if (inverty) { diff.y *= -1; }
				
				velocity += diff * sensitivity;
				lastMouse = mouse;
			}
		}
		
		Vector3 movement = new Vector3(velocity.x, 0, velocity.y);
		transform.position += movement * Time.deltaTime;
		velocity = Vector2.Lerp(velocity, Vector2.zero, Time.deltaTime * dampening);
	}
}
