using UnityEngine;
using System.Collections;

using In = UnityEngine.Input;

/// <summary> Enum definitions with names matching axes as defined in DefaultProjectSettings/InputManager.asset </summary>
public enum AxisCode : int {
	Joystick1Axis0 = 0,
	Joystick1Axis1 = 1,
	Joystick1Axis2 = 2,
	Joystick1Axis3 = 3,
	Joystick1Axis4 = 4,
	Joystick1Axis5 = 5,
	Joystick1Axis6 = 6,
	Joystick1Axis7 = 7,
	Joystick1Axis8 = 8,
	Joystick1Axis9 = 9,
	Joystick2Axis0 = 10,
	Joystick2Axis1 = 11,
	Joystick2Axis2 = 12,
	Joystick2Axis3 = 13,
	Joystick2Axis4 = 14,
	Joystick2Axis5 = 15,
	Joystick2Axis6 = 16,
	Joystick2Axis7 = 17,
	Joystick2Axis8 = 18,
	Joystick2Axis9 = 19,
	Joystick3Axis0 = 20,
	Joystick3Axis1 = 21,
	Joystick3Axis2 = 22,
	Joystick3Axis3 = 23,
	Joystick3Axis4 = 24,
	Joystick3Axis5 = 25,
	Joystick3Axis6 = 26,
	Joystick3Axis7 = 27,
	Joystick3Axis8 = 28,
	Joystick3Axis9 = 29,
	Joystick4Axis0 = 30,
	Joystick4Axis1 = 31,
	Joystick4Axis2 = 32,
	Joystick4Axis3 = 33,
	Joystick4Axis4 = 34,
	Joystick4Axis5 = 35,
	Joystick4Axis6 = 36,
	Joystick4Axis7 = 37,
	Joystick4Axis8 = 38,
	Joystick4Axis9 = 39,
	JoystickAxis0 = 40,
	JoystickAxis1 = 41,
	JoystickAxis2 = 42,
	JoystickAxis3 = 43,
	JoystickAxis4 = 44,
	JoystickAxis5 = 45,
	JoystickAxis6 = 46,
	JoystickAxis7 = 47,
	JoystickAxis8 = 48,
	JoystickAxis9 = 49,
	MouseAxisX = 50,
	MouseAxisY = 51,
	MouseWheel = 52
}

///<summary>This is a static class that wraps all of Unity's internal Input static class'
///Then provides some additional extra features</summary>
public static class Input {

	#region Basics_Provided_By_UNITY
	public static Vector3 acceleration { get { return In.acceleration; } }
	public static int accelerationEventCount { get { return In.accelerationEventCount; } }
	public static AccelerationEvent[] accelerationEvents { get { return In.accelerationEvents; } }

	public static string inputString { get { return In.inputString; } }
	public static bool anyKey { get { return In.anyKey; } }
	public static bool anyKeyDown { get { return In.anyKeyDown; } }
	public static Compass compass { get { return In.compass; } }
	public static bool compensateSensors { get { return In.compensateSensors; } set { In.compensateSensors = value; } }
	public static Vector2 compositionCursorPos { get { return In.compositionCursorPos; } set { In.compositionCursorPos = value; } }
	public static string compositionString { get { return In.compositionString; } }
	public static DeviceOrientation deviceOrientation { get { return In.deviceOrientation; } }

	public static bool simulateMouseWithTouches { get { return In.simulateMouseWithTouches; } set { In.simulateMouseWithTouches = value; } }
	public static bool touchSupported { get { return In.touchSupported; } }

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

	public static bool mousePresent { get { return In.mousePresent; } }

	public static Vector3 screenMousePosition {
		get { return In.mousePosition; }
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


	/// <summary>
	/// Get the name of the primary controller that is connected
	/// </summary>
	public static string primaryController {
		get {
			foreach (string str in GetJoystickNames()) {
				if (str != null && str != "") { return str; }
			}
			return "";
		}
	}

	public static int primaryControllerIndex {
		get {
			var joysticks = GetJoystickNames();
			for (int i = 0; i < joysticks.Length; i++) {
				var str = joysticks[i];
				if (str != null && str != "") { return i; }
			}
			return -1;
		}
	}

	/// <summary> true if primaryController is not empty string, false otherwise. </summary>
	public static bool controllerConnected { get { return primaryController != ""; } }

	///Specific Touches
	public static Touch firstTouch { get { return In.touches[0]; } }
	public static Touch secondTouch { get { return In.touches[1]; } }

	///<summary>Get Average Touch position</summary>
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


	/// <summary> Direction from center of screen to mouse cursor</summary>
	public static Vector3 mouseDirection { get { return (mousePosition - new Vector3(Screen.width / 2, Screen.height / 2, 0)).normalized; } }

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
