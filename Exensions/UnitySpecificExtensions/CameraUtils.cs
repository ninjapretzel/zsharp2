using UnityEngine;
using System.Collections;

///Class holding extensions and very simple common operations.
public static class CameraUtils {
	
	///Raycasts with default layermask into the world
	///Returns whatever point at which raycast stopped.
	public static Vector3 mouseWorldPosition {
		get {
			RaycastHit rayhit;
			if (Physics.Raycast(mouseRay, out rayhit)) { return rayhit.point; }
			return Vector3.zero;
		}
	}
	
	//Get the ray for the pixel of the mouse from the main Camera object.
	public static Ray mouseRay {
		get { return Camera.main.ScreenPointToRay(Input.mousePosition); }
	}
	
	//Raycasts with default layermask into the world
	///Returns whatever collider intersected raycast.
	public static Collider mouseCollider {
		get {
			RaycastHit rayhit;
			if (Physics.Raycast(mouseRay, out rayhit)) { return rayhit.collider; }
			return null;
		}
	}
	
	///Extension to Component to see if one is onscreen or offscreen.
	public static bool IsOffscreen(this Component c) { return !c.IsOnscreen(); }
	public static bool IsOnscreen(this Component c) {
		Vector3 pos = c.GetViewPosition();
		if (pos.z < 0) { return false; }
		return pos.x >= 0 && pos.x <= 1 && pos.y >= 0 && pos.y <= 1;
	}
	
	///Extension to Component to get a square rectangle on screen
	///pass in a percentage of the size of the square returned.
	public static Rect GetScreenSquare(this Component c, float h) { 
		return c.GetScreenRect(h, h).MiddleCenterSquare(1);
	}
	
	///Extension to Component to get a rectangle on screen.
	///pass in one percentage to get a rectangle scalled to equal proportion, and two to control width and height.
	public static Rect GetScreenRect(this Component c, float s) { return c.GetScreenRect(s, s); }
	public static Rect GetScreenRect(this Component c, float w, float h) {
		Vector3 pos = c.GetScreenPosition();
		pos.x -= w * Screen.width * .5f;
		pos.y -= h * Screen.height * .5f;
		return new Rect(pos.x, pos.y, w * Screen.width, h * Screen.height);
	}
	
	///Get the position on the screen of a Component.
	///Optionally, a camera may be specified.
	public static Vector3 GetScreenPosition(this Component c) { return GetScreenPosition(Camera.main, c.transform); }
	public static Vector3 GetScreenPosition(this Component c, Camera cam) { return GetScreenPosition(cam, c.transform); }
	public static Vector3 GetScreenPosition(this Camera cam, Transform t) {
		Vector3 pos = cam.WorldToScreenPoint(t.position);
		pos.y = Screen.height - pos.y;
		return pos;
	}
	
	///Get the position in the view of a Component
	//Optionally, a camera may be specified.
	public static Vector3 GetViewPosition(this Component c) { return GetViewPosition(Camera.main, c.transform); }
	public static Vector3 GetViewPosition(this Component c, Camera cam) { return GetViewPosition(cam, c.transform); }
	public static Vector3 GetViewPosition(this Camera cam, Transform t) {
		Vector3 pos = cam.WorldToViewportPoint(t.position);
		pos.y = 1.0f - pos.y;
		return pos;
	}
	
}
