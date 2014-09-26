﻿using UnityEngine;
using System.Collections;

using In = UnityEngine.Input;

///This is a static class that wraps all of Unity's internal Input static class'
///Then provides some additional extra features
public static class Input {
	
	#region Basics_Provided_By_UNITY
	public static Vector3 acceleration { get { return In.acceleration; } }
	public static int accelerationEventCount { get { return In.accelerationEventCount; } }
	public static AccelerationEvent[] accelerationEvents { get { return In.accelerationEvents; } }
	
	public static bool anyKey { get { return In.anyKey; } }
	public static bool anyKeyDown { get { return In.anyKeyDown; } }
	public static Compass compass { get { return In.compass; } } 
	public static bool compensateSensors { get { return In.compensateSensors; } set { In.compensateSensors = value; } }
	public static Vector2 compositionCursorPos { get { return In.compositionCursorPos; } set { In.compositionCursorPos = value; } }
	public static string compositionString { get { return In.compositionString; } }
	public static DeviceOrientation deviceOrientation { get { return In.deviceOrientation; } }
	public static Gyroscope gyro { get { return In.gyro; } }
	public static IMECompositionMode imeCompositionMode { get { return In.imeCompositionMode; } set { In.imeCompositionMode = value; } }
	public static bool imeIsSelected { get { return In.imeIsSelected; } }
	public static LocationService location { get { return In.location; } }
	
	///This one is overloaded to 'flip' the mouse position.
	///Does not affect code which is compiled outside the project (eg, other binaries or plugins )
	public static Vector3 mousePosition { 
		get {
			Vector3 pos = In.mousePosition;
			pos.y = Screen.height - pos.y;
			return pos;
		}
	}
	
	public static bool multiTouchEnabled { get { return In.multiTouchEnabled; } set { In.multiTouchEnabled = value; } }
	public static int touchCount { get { return In.touchCount; } }
	public static Touch[] touches { get { return In.touches; } }
	
	public static AccelerationEvent GetAccelerationEvent(int index) { return In.GetAccelerationEvent(index); }
	public static float GetAxis(string axis) { return In.GetAxis(axis); }
	public static float GetAxisRaw(string axis) { return In.GetAxisRaw(axis); }
	public static bool GetButton(string axis) { return In.GetButton(axis); }
	public static bool GetButtonUp(string axis) { return In.GetButtonUp(axis); }
	public static bool GetButtonDown(string axis) { return In.GetButtonDown(axis); }
	public static string[] GetJoystickNames() { return In.GetJoystickNames(); }
	
	public static bool GetKey(string key) { return In.GetKey(key); }
	public static bool GetKey(KeyCode key) { return In.GetKey(key); }
	public static bool GetKeyUp(string key) { return In.GetKeyUp(key); }
	public static bool GetKeyUp(KeyCode key) { return In.GetKeyUp(key); }
	public static bool GetKeyDown(string key) { return In.GetKeyDown(key); }
	public static bool GetKeyDown(KeyCode key) { return In.GetKeyDown(key); }
	
	public static bool GetMouseButton(int button) { return In.GetMouseButton(button); }
	public static bool GetMouseButtonUp(int button) { return In.GetMouseButtonUp(button); }
	public static bool GetMouseButtonDown(int button) { return In.GetMouseButtonDown(button); }
	public static Touch GetTouch(int index) { return In.GetTouch(index); }
	
	public static void ResetInputAxes() { In.ResetInputAxes(); }
	#endregion
	
	///Specific Touches
	public static Touch firstTouch { get { return In.touches[0]; } }
	public static Touch secondTouch { get { return In.touches[1]; } }
	
	///Get Average Touch position
	public static Vector2 averageTouch {
		get { 
			if (In.touches.Length == 0) { return -Vector2.one; }
			Vector2 avg = Vector2.zero;
			foreach (Touch t in In.touches) {
				avg += t.position;
			}
			
			return avg / In.touches.Length;
		}
		
	}
	
	
	public static Vector3 mouseDirection { get { return (mousePosition - new Vector3(Screen.width/2, Screen.height/2, 0)).normalized; } }
	
	public static Vector2 TouchScroll(this Vector2 v, Rect area, float sensitivity = 1f) {
		foreach (Touch t in In.touches) {
			if (t.phase == TouchPhase.Moved) {
				Vector2 pos = t.ScreenPosition();
				if (area.Contains(pos)) {
					Vector2 dpos = t.deltaPosition * sensitivity;
					dpos.y *= -1;
					return v - dpos;
				}
			}
		}
		return v;
	}
	
	
	public static Vector2 TouchVelocity(Rect area, float sensitivity = 1f) {
		foreach (Touch t in In.touches) {
			if (t.phase == TouchPhase.Moved) {
				if (area.Contains(t.ScreenPosition())) {
					return t.deltaPosition * sensitivity;
				}
			}
		}
		return Vector2.zero;
	}
	
}
