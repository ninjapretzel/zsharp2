using UnityEngine;
using System.Collections;

///Class holding extensions and very simple common operations.
public static class CameraUtils {
	
	/// <summary> Get the ray for the pixel of the mouse from the main Camera object. </summary>
	public static Ray mouseRay {
		get { return Camera.main.ScreenPointToRay(Input.screenMousePosition); }
	}

	///<summary> Raycasts with default layermask into the world. Returns whatever point at which raycast stopped. </summary>
	public static Vector3 mouseWorldPosition {
		get {
			RaycastHit rayhit;
			if (Physics.Raycast(mouseRay, out rayhit)) { return rayhit.point; }
			return Vector3.zero;
		}
	}
	
	///<summary> Raycasts with default layermask into the world. Returns whatever collider intersected raycast. </summary>
	public static Collider mouseCollider {
		get {
			RaycastHit rayhit;
			if (Physics.Raycast(mouseRay, out rayhit)) { return rayhit.collider; }
			return null;
		}
	}
	
	/// <summary> Check if the transform.position of a component is on or off screen. </summary>
	/// <param name="c"> Component to access Transform of </param>
	/// <returns> True if the object is off-screen compared to the main camera. </returns>
	public static bool IsOffscreen(this Component c) { return !c.IsOnscreen(); }

	/// <summary> Check if the transform.position of a component is on or off screen. </summary>
	/// <param name="c"> Component to access Transform of </param>
	/// <returns> True if the object is on-screen compared to the main camera. </returns>
	public static bool IsOnscreen(this Component c) {
		Vector3 pos = c.GetViewPosition();
		if (pos.z < 0) { return false; }
		return pos.x >= 0 && pos.x <= 1 && pos.y >= 0 && pos.y <= 1;
	}
	
	/// <summary> Get a square pixel rect around the transform.position of a given component </summary>
	/// <param name="c"> Component to use the transform.position of </param>
	/// <param name="h"> Side length for one side. </param>
	/// <returns> Square pixel rect around the center of <paramref name="c"/>, with side length based on the lowest value of <paramref name="h"/> * screen width/height </returns>
	public static Rect GetScreenSquare(this Component c, float h) { 
		return c.GetScreenRect(h, h).MiddleCenterSquare(1);
	}
	
	/// <summary> Gets the pixel rect around the transform.position of <paramref name="c"/>, based on <paramref name="s"/> times the screen width/height </summary>
	/// <param name="c"> Component to use (to accept any component type for ease of use) </param>
	/// <param name="s"> Scale of screen width/height for sides of constructed rectangle </param>
	/// <returns> Pixel Rect around position of <paramref name="c"/> with sides of <paramref name="s"/> times the screen width/height </returns>
	public static Rect GetScreenRect(this Component c, float s) { return c.GetScreenRect(s, s); }
	/// <summary> Gets the pixel rect around the transform.position of <paramref name="c"/>, based on <paramref name="w"/>/<paramref name="h"/> times the screen width/height</summary>
	/// <param name="c"> Component to use (to accept any component type for ease of use) </param>
	/// <param name="h"> height percentage </param>
	/// <param name="w"> width percentage </param>
	/// <returns> Pixel Rect around the position of <paramref name="c"/>, with sides of <paramref name="w"/>/<paramref name="h"/> times the screen width/height </returns>
	public static Rect GetScreenRect(this Component c, float w, float h) {
		Vector3 pos = c.GetScreenPosition();
		pos.x -= w * Screen.width * .5f;
		pos.y -= h * Screen.height * .5f;
		return new Rect(pos.x, pos.y, w * Screen.width, h * Screen.height);
	}

	/// <summary> Gets a pixel rect around <paramref name="v"/>, that is <paramref name="s"/> times the width/height of the screen </summary>
	/// <param name="v"> Point in worldspace at center of rect </param>
	/// <param name="s"> Scale of generated rectangle </param>
	/// <returns> A rect, in pixels, centered around <paramref name="v"/>, with sides based off <paramref name="s"/> times the screen width/height </returns>
	public static Rect GetScreenRect(this Vector3 v, float s) { return v.GetScreenRect(s, s); }
	/// <summary> Gets a pixel rect around <paramref name="v"/>, that is <paramref name="w"/>/<paramref name="h"/> times the width/height of the screen. </summary>
	/// <param name="v"> Point in worldspace at center of rect </param>
	/// <param name="h"> height percentage </param>
	/// <param name="w"> width percentage </param>
	/// <returns> A rect, in pixels, centered around <paramref name="v"/>, with sides based off <paramref name="s"/> times the screen width/height </returns>
	public static Rect GetScreenRect(this Vector3 v, float w, float h) {
		Vector3 pos = v.GetScreenPosition();
		pos.x -= w * Screen.width * .5f;
		pos.y -= h * Screen.height * .5f;
		return new Rect(pos.x, pos.y, w * Screen.width, h * Screen.height);
	}

	/// <summary> Get the screen pixel coordinate of <paramref name="v"/> from the main camera </summary>
	/// <param name="v"> Point in world </param>
	/// <returns> Point on screen (with y corrected) </returns>
	public static Vector3 GetScreenPosition(this Vector3 v) { return GetScreenPosition(Camera.main, v); }
	/// <summary> Get the screen pixel coordinate of <paramref name="v"/> from the given <paramref name="cam"/>. </summary>
	/// <param name="v"> Point in world </param>
	/// <param name="cam"> Camera to get screen coord in reference to </param>
	/// <returns> Point on screen (with y corrected) </returns>
	public static Vector3 GetScreenPosition(this Vector3 v, Camera cam) { return GetScreenPosition(cam, v); }
	/// <summary> Get the screen pixel coordinate of <paramref name="v"/> from the given <paramref name="cam"/>. </summary>
	/// <param name="v"> Point in world </param>
	/// <param name="cam"> Camera to get screen coord in reference to </param>
	/// <returns> Point on screen (with y corrected) </returns>
	public static Vector3 GetScreenPosition(this Camera cam, Vector3 v) {
		Vector3 pos = cam.WorldToScreenPoint(v);
		pos.y = Screen.height - pos.y;
		return pos;
	}

	/// <summary> Get the screen pixel coordinate of <paramref name="c"/> from the main camera </summary>
	/// <param name="c"> Component to use the transform.position of </param>
	/// <returns> Point on screen (with y corrected) </returns>
	public static Vector3 GetScreenPosition(this Component c) { return GetScreenPosition(Camera.main, c.transform); }
	/// <summary> Get the screen pixel coordinate of <paramref name="c"/> from the given camera </summary>
	/// <param name="c"> Component to use the transform.position of </param>
	/// <param name="cam"> Camera to get screen coord in reference to </param>
	/// <returns> Point on screen (with y corrected) </returns>
	public static Vector3 GetScreenPosition(this Component c, Camera cam) { return GetScreenPosition(cam, c.transform); }
	/// <summary> Get the screen pixel coordinate of <paramref name="t"/> from the given camera </summary>
	/// <param name="t"> Transform to use the position of </param>
	/// <param name="cam"> Camera to get screen coord in reference to </param>
	/// <returns> Point on screen (with y corrected) </returns>
	public static Vector3 GetScreenPosition(this Camera cam, Transform t) {
		Vector3 pos = cam.WorldToScreenPoint(t.position);
		pos.y = Screen.height - pos.y;
		return pos;
	}
	
	/// <summary> Get view coordinate of <paramref name="c"/> from the main camera</summary>
	/// <param name="c"> Component to use the transform.position of</param>
	/// <returns> Point in viewspace (with y corrected) </returns>
	public static Vector3 GetViewPosition(this Component c) { return GetViewPosition(Camera.main, c.transform); }
	/// <summary> Get view coordinate of <paramref name="c"/> from the given camera</summary>
	/// <param name="c"> Component to use the transform.position of</param>
	/// <param name="cam"> Camera to use perspective of </param>
	/// <returns> Point in viewspace (with y corrected) </returns>
	public static Vector3 GetViewPosition(this Component c, Camera cam) { return GetViewPosition(cam, c.transform); }
	/// <summary> Get view coordinate of <paramref name="t"/> from the main camera</summary>
	/// <param name="t"> Transform to use the position of</param>
	/// <param name="cam"> Camera to use perspective of </param>
	/// <returns> Point in viewspace (with y corrected) </returns>
	public static Vector3 GetViewPosition(this Camera cam, Transform t) {
		Vector3 pos = cam.WorldToViewportPoint(t.position);
		pos.y = 1.0f - pos.y;
		return pos;
	}
	
}
