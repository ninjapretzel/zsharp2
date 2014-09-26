using UnityEngine;
using System.Collections;

///This class simulates a control stick on the screen of a device.
public class TouchControlStick : MonoBehaviour {
	
	///This Vector2 reports the axis of the simulated stick.
	public Vector2 value;
	
	///This is the Viewport-Space (normalized) area of the stick
	public Rect normalizedArea = new Rect(0, 0, .5f, 1f);
	///This is the Screen-Space (pixel) area of the stick
	public Rect area { get { return normalizedArea.Denormalized(); } }
	
	///This is a handle for the stick.
	public string stickName = "LeftStick";
	
	public float sensitivity = 1;
	
	///Scaling size for the different parts
	///Root is the center of the stick, and stays in place.
	///Thumb follows the touch as it moves across the screen.
	public float rootSize = 1;
	public float thumbSize = 1;
	
	///Invert axis?
	public bool invertX = false;
	public bool invertY = false;
	
	Vector2 touchDown;
	bool hasTouch = false;
	
	///Assign these to use things prettier than a square.
	public Texture2D rootGraphic;
	public Texture2D thumbGraphic;
	
	///Get the closest touch in the area on the screen to the stick.
	Touch closestTouch {
		get {
			float distance = System.Single.MaxValue;
			
			Touch closest = default(Touch);
			
			foreach (Touch t in Input.touches) {
				float dist = (t.position - touchDown).magnitude;
				if (dist < distance) {
					distance = dist;
					closest = t;
				}
			}
			
			return closest;
		}
	}
	
	//Automatically disable this object on non-mobile devices.
	void Start() {
		if (!GameSys.isMobile) {
			Destroy(this);
		}
		
	}
	
	void Update() {
		
	}
	
	///Touch processing and drawing is done in the legacy GUI system.
	void OnGUI() {
		if (Input.touches.Length == 0) { 
			hasTouch = false; 
			value = Vector2.zero; 
			return;
		}
		
		if (!hasTouch) {
			value = Vector2.zero;
			
			FindTouch();
		} else {
			
			ProcessTouch();
		}
		
	}
	
	///Looks for and handles any touchdown happening in the assigned area
	void FindTouch() {
		foreach (Touch t in Input.touches) {
			if (t.IsPress()) {
				
				if (area.Contains(t)) {
					touchDown = t.ScreenPosition();
					hasTouch = true;
					
				}
				
			}
			
		}
		
	}
	
	///Processes the current touch
	void ProcessTouch() {
		Touch t = closestTouch;
		if (area.Contains(t)) {
		
			Vector2 pos = t.ScreenPosition();
			Vector2 diff = pos - touchDown;
			
			diff /= (Screen.width * .15f);
			diff *= sensitivity;
			
			//GUI.Label(area, "" + diff + "." + diff.magnitude);
			
			Rect brush = new Rect(touchDown.x, touchDown.y, 0, 0).Pad(Screen.height * .1f * rootSize);
			GUI.DrawTexture(brush, rootGraphic ? rootGraphic : GUIUtils.pixel);
			
			brush = new Rect(pos.x, pos.y, 0, 0).Pad(Screen.height * .1f * thumbSize);
			GUI.DrawTexture(brush, thumbGraphic ? thumbGraphic : GUIUtils.pixel);
				
			
			if (diff.magnitude > 1) {
				value = diff.normalized;
			} else {
				value = diff;
			}
			
			if (!invertY) { value.y *= -1; }
			if (invertX) { value.x *= -1; }
			
			
			if (t.IsRelease()) {
				hasTouch = false;
			}
			
		}
	}
	
}