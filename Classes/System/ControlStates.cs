using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary> Class for holding the current and previous states of controls. </summary>
public static class ControlStates {

	/// <summary> Set of values for the current frame </summary>
	private static Dictionary<string, string> values = new Dictionary<string,string>();

	/// <summary> Set of values for the previous frame </summary>
	private static Dictionary<string, string> previous = new Dictionary<string,string>();
	
	/// <summary> Advance the tracking of input to the next frame. Call before all other scripts </summary>
	public static void NextFrame() {
		//I dunno how much good this actually does in terms of speed.
		//It might be faster to just cache what axis were Unset() on a given frame.
		//previous.Clear(); 
		foreach (var pair in values) {
			previous[pair.Key] = pair.Value;
		}
	}

	
	/// <summary> Set ControlState 'thing' to value 'val' </summary>
	public static void Set(string thing, string val) { values[thing] = val; }
	/// <summary> Remove ControlState 'thing' from the dictionary </summary>
	public static void Unset(string thing) { if (values.ContainsKey(thing)) { values.Remove(thing); } }

	/// <summary> Get the previous state of ControlState 'thing' </summary>
	private static T GetPrevious<T>(string thing) { return GetInternal<T>(thing, previous); }

	/// <summary> Did the ControlState 'thing' just go down (as a button)? </summary>
	public static bool GetDown(string thing) {
		bool now = Get<bool>(thing);
		bool prev = GetPrevious<bool>(thing);

		return !prev && now;
	}

	/// <summary> Did the ControlState 'thing' just come up (as a button)? </summary>
	public static bool GetUp(string thing) {
		bool now = Get<bool>(thing);
		bool prev = GetPrevious<bool>(thing);

		return prev && !now;
	}

	/// <summary>
	/// Gets a value by name of some virtual input.
	/// This is intended to be used with primitiave types as the generic parameter.
	/// works with (string, bool, int, float, double, byte, short, long)
	/// 
	/// If the internal input represents a number (double.Parse is successful), the value of the number is returned
	/// Otherwise, 0 is returned.
	/// 
	/// If a bool is requested, false is returned unless:
	/// the data is 'true', or a number with an absolute value over the deadzone.
	/// 
	/// 
	/// </summary>
	public static T Get<T>(string thing) { return GetInternal<T>(thing, values); }

	/// <summary> Deadzone for treating axis as buttons</summary>
	public static float deadzone = .25f;

	/// <summary> Internal Get function, used by Get and GetPrevious. </summary>
	private static T GetInternal<T>(string thing, Dictionary<string, string> values) {
		if (values.ContainsKey(thing)) {
			object ret = default(T);

			string data = values[thing];
			
			if (typeof(T) == typeof(string)) { ret = data; }

			if (typeof(T) == typeof(int)
				|| typeof(T) == typeof(float)
				|| typeof(T) == typeof(double)
				|| typeof(T) == typeof(short)
				|| typeof(T) == typeof(byte)
				|| typeof(T) == typeof(long)) {
				if (data.ToLower() == "true") { ret = 1; }
				if (data.ToLower() == "false") { ret = 0; }

				double val;
				if (double.TryParse(data, out val)) {
					if (typeof(T) == typeof(int)) { ret = (T)(object)(int)val; }
					if (typeof(T) == typeof(float)) { ret = (T)(object)(float)val; }
					if (typeof(T) == typeof(double)) { ret = (T)(object)val; }
					if (typeof(T) == typeof(short)) { ret = (T)(object)(short)val; }
					if (typeof(T) == typeof(byte)) { ret = (T)(object)(byte)val; }
					if (typeof(T) == typeof(long)) { ret = (T)(object)(long)val; }
					
				}

			}

			if (typeof(T) == typeof(bool)) {
				double val;
				ret = false;
				if (data.ToLower() == "true") { ret = true; }
				if (double.TryParse(data, out val)) {
					double abs = System.Math.Abs(val);
					if (abs > deadzone) { ret = true; }
				}
	
			}

			return (T)ret;
		}

		if (typeof(T) == typeof(string)) { return (T)(object)""; }

		return default(T);
	}

	/// <summary> Standard axis wrapper property. Movement on the +/- Z axis. Typically represents the 'up/down' direction on the 'left' stick, or Keyboard W/S. </summary>
	public static float forwardAxis { get { return Get<float>("forwardAxis"); } set { Set("forwardAxis", value.ToString()); } }

	/// <summary> Standard axis wrapper property. Movement on the +/- X axis.  Typically represents the 'left/right' direction on the 'left' stick, or Keyboard A/D. </summary>
	public static float lateralAxis { get { return Get<float>("lateralAxis"); } set { Set("lateralAxis", value.ToString()); } }

	/// <summary> Standard axis wrapper property. Camera Movement around the Y axis. Typically represents the 'left/right' direction on the 'right' stick, or Arrow Keys left/right. Camera Inversion is handled in a PlayerControl script if needed. </summary>
	public static float yawAxis { get { return Get<float>("yawAxis"); } set { Set("yawAxis", value.ToString()); } }
	/// <summary> Standard axis wrapper property. Camera Movement around the X axis. Typically represens the 'up/down' direction on the 'right' stick, or Arrow Keys up/down. Camera Inversion is handled in a PlayerControl script if needed. </summary>
	public static float pitchAxis { get { return Get<float>("pitchAxis"); } set { Set("pitchAxis", value.ToString()); } }

	/// <summary> Standard axis wrapper property. Camera Movement in/out. Typically represents the shoulder button analogs if present, or scroll wheel. </summary>
	public static float zoomAxis { get { return Get<float>("zoomAxis"); } set { Set("zoomAxis", value.ToString()); } }

	/// <summary> Standard button wrapper property. Movement on the +Z axis. Typically represents the 'up' direction on the 'left' stick, or Keyboard W. </summary>
	public static bool forward { get { return Get<bool>("forward"); } set { Set("forward", value.ToString()); } }
	/// <summary> Standard button wrapper property. Movement on the -Z axis. Typically represents the 'down' direction on the 'left' stick, or Keyboard S. </summary>
	public static bool backward { get { return Get<bool>("backward"); } set { Set("backward", value.ToString()); } }
	/// <summary> Standard button wrapper property. Movement on the -X axis. Typically represents the 'left' direction on the 'left' stick, or Keyboard A. </summary>
	public static bool left { get { return Get<bool>("left"); } set { Set("left", value.ToString()); } }
	/// <summary> Standard button wrapper property. Movement on the +X axis. Typically represents the 'right' direction on the 'left' stick, or Keyboard D. </summary>
	public static bool right { get { return Get<bool>("right"); } set { Set("right", value.ToString()); } }
	/// <summary> Standard button wrapper property. Movement on the +Y axis. Typcially represents the right bumper, or Keyboard E. </summary>
	public static bool up { get { return Get<bool>("up"); } set { Set("up", value.ToString()); } }
	/// <summary> Standard button wrapper property. Movement on the -Y axis. Typically represents the left bumper, or Keyboard Q. </summary>
	public static bool down { get { return Get<bool>("down"); } set { Set("down", value.ToString()); } }

	/// <summary> Standard button wrapper property. Camera Movement inwards. Typically represents the right analog trigger, Mouse Scroll Wheel Up. </summary>
	public static bool zoomIn { get { return Get<bool>("zoomIn"); } set { Set("zoomIn", value.ToString()); } }
	/// <summary> Standard button wrapper property. Camera Movement inwards. Typically represents the left analog trigger, Mouse Scroll Wheel Down. </summary>
	public static bool zoomOut { get { return Get<bool>("zoomOut"); } set { Set("zoomOut", value.ToString()); } }

	
	/// <summary> Standard button wrapper property. Camera around the X axis, raising the camera. By default, the Upwards direction on the 'right' stick, or Arrow Keys Down. Camera Inversion is handled in a PlayerControl script if needed. </summary>
	public static bool camUp { get { return Get<bool>("camUp"); } set { Set("camUp", value.ToString()); } }
	/// <summary> Standard button wrapper property. Camera around the X axis, lowering the camera. By default, the Downwards direction on the 'right' stick, or Arrow Keys Down. Camera Inversion is handled in a PlayerControl script if needed. </summary>
	public static bool camDown { get { return Get<bool>("camDown"); } set { Set("camDown", value.ToString()); } }
	/// <summary> Standard button wrapper property. Camera around the Y axis, panning the camera. By default, the Left direction on the 'right' stick, or Arrow Keys Left. Camera Inversion is handled in a PlayerControl script if needed. </summary>
	public static bool camLeft { get { return Get<bool>("camLeft"); } set { Set("camLeft", value.ToString()); } }
	/// <summary> Standard button wrapper property. Camera around the Y axis, panning the camera. By default, the Right direction on the 'right' stick, or Arrow Keys Right. Camera Inversion is handled in a PlayerControl script if needed. </summary>
	public static bool camRight { get { return Get<bool>("camRight"); } set { Set("camRight", value.ToString()); } }

	/// <summary> Standard button wrapper property. Makes the player jump. Typically represents Joystick Button 0, or Keyboard Spacebar. </summary>
	public static bool jump { get { return Get<bool>("jump"); } set { Set("jump", value.ToString()); } }
	
	/// <summary> Standard button wrapper property. Makes the player use a powerup they are holding. Typically represents Joystick Button 1, or Keyboard LeftShift. </summary>
	public static bool powerup { get { return Get<bool>("powerup"); } set { Set("powerup", value.ToString()); } }
	
}
