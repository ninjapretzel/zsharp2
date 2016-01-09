using UnityEngine;
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// Delegate type for functions handling joystick events.
/// </summary>
/// <param name="num">Joystick number, according to Unity's Input system.</param>
/// <param name="name">Joystick name.</param>
public delegate void JoystickEvent(int num, string name);

/// <summary>
/// Class providing an interface for Unity's input system to name controls on joysticks/gamepads.
/// Also contains events for detecting a joystick disconnect.
/// </summary>
public class Joysticks : MonoBehaviour {

	public const string blacklistedName = "IGNORE";
#if UNITY_STANDALONE_WIN
	public const string platformFolder = "Windows";
#elif UNITY_ANDROID
	public const string platformFolder = "Android";
#endif

	public static Joysticks instance;

	/// <summary>Delegate that is called if a joystick is connected.</summary>
	public static JoystickEvent OnJoystickConnected;
	/// <summary>Delegate that is called if a joystick is disconnected.</summary>
	public static JoystickEvent OnJoystickDisconnected;
	/// <summary>Delegate that is called if a joystick is reconnected.</summary>
	public static JoystickEvent OnJoystickReconnected;

	private static List<string> joystickNames = new List<string>(8);
	private static List<Dictionary<string, string>> controlNames = new List<Dictionary<string, string>>(8);

	public void Awake() {
		if (instance != null) {
			Destroy(this);
			return;
		}
		instance = this;
		joystickNames = Input.GetJoystickNames().ToList();
		foreach (string name in joystickNames) {
			controlNames.Add(LoadControlNamesForJoystick(name));
		}
	}

	/// <summary>
	/// Checks Unity's current list of joysticks against an internal one, and calls
	/// OnJoystickConnected when a new joystick is connected, OnJoystickDisconnected
	/// when a joystick is disconnected, and OnJoystickReconnected when a joystick is
	/// reconnected.
	/// </summary>
	public void Update() {
		string[] names = Input.GetJoystickNames();
		if (names.Length > joystickNames.Count) {
			// The length of GetJoystickNames is only ever incremented if changed.
			for (int i = joystickNames.Count; i < names.Length; ++i) {
				if (OnJoystickConnected != null) {
					OnJoystickConnected(i + 1, names[i]);
				}
				joystickNames.Add(names[i]);
				controlNames.Add(LoadControlNamesForJoystick(names[i]));
			}
		}
		
		// At this point, the locally tracked joystick names and Unity's list should be the same length.
		for (int i = 0; i < names.Length; ++i) {
			if (names[i].Length == 0 && joystickNames[i].Length != 0) {
				// If the name of the joystick is empty but the locally tracked name is not, we've got a disconnect.
				if (OnJoystickDisconnected != null) {
					OnJoystickDisconnected(i + 1, joystickNames[i]);
					// Replace locally tracked name so as not to spam the delegate.
				}
				joystickNames[i] = "";
			} else if (names[i].Length != 0 && joystickNames[i].Length == 0) {
				// If an empty joystick name suddenly has a name again, a joystick has been reconnected.
				if (OnJoystickReconnected != null) {
					OnJoystickReconnected(i + 1, names[i]);
				}
				joystickNames[i] = names[i];
				if (controlNames[i] == null) {
					controlNames[i] = LoadControlNamesForJoystick(names[i]);
				}
			}
		}
	}

	/// <summary>
	/// Gets the name for a specified axis.
	/// </summary>
	/// <param name="control">The control to get the name for, must be of format "JoystickXAxisY[+/-]" or "JoystickXButtonY".</param>
	/// <returns>The name of <paramref name="control"/> corresponding to the detected controller number <c>X</c>, or <paramref name="control"/> if one is not provided.</returns>
	public static string GetControlName(string control) {
		int joystickNum = 0;
		if (!int.TryParse(control[8].ToString(), out joystickNum)) {
			return control;
		}
		control = control.Substring(0, 8) + control.Substring(9);
		return GetControlName(joystickNum, control);
	}

	/// <summary>
	/// Gets the name for a specified axis.
	/// </summary>
	/// <param name="joystickNum">The number assigned to the joystick, between 1 and 8.</param>
	/// <param name="control">The control to get the name for, must be of format "JoystickAxisX[+/-]" or "JoystickButtonX".</param>
	/// <returns>The name of <paramref name="control"/> corresponding to the detected controller number <paramref name="joystickNum"/>, or <paramref name="control"/> if one is not provided.</returns>
	public static string GetControlName(int joystickNum, string control) {
		if (controlNames.Count >= joystickNum && controlNames[joystickNum - 1] != null && controlNames[joystickNum - 1].ContainsKey(control)) {
			return controlNames[joystickNum - 1][control];
		}
		return control.Substring(0, 8) + joystickNum.ToString() + control.Substring(8);
	}

	/// <summary>
	/// Gets the name for a specified button.
	/// </summary>
	/// <param name="button">The button to get the name for. Must refer to a specific joystick <c>X</c>.</param>
	/// <returns>The name of <paramref name="button"/> corresponding to the detected controller number <c>X</c>, or <paramref name="button"/> if one is not provided.</returns>
	public static string GetButtonName(KeyCode button) {
		return GetControlName(button.ToString());
	}

	/// <summary>
	/// Gets the name for a specified button.
	/// </summary>
	/// <param name="button">The button to get the name for.</param>
	/// <param name="joystickNum">The number assigned to the joystick, between 1 and 8.</param>
	/// <returns>The name of <paramref name="button"/> corresponding to the detected controller number <paramref name="joystickNum"/>, or <paramref name="button"/> if one is not provided.</returns>
	public static string GetButtonName(int joystickNum, KeyCode button) {
		if (button > KeyCode.JoystickButton19) {
			button -= 20 * joystickNum;
		}
		return GetControlName(joystickNum, button.ToString());
	}

	/// <summary>
	/// Gets whether a specified control is blacklisted. This is useful for axes or buttons that work improperly on a controller.
	/// </summary>
	/// <param name="joystickNum">The number assigned to the joystick, between 1 and 8.</param>
	/// <param name="control">The control to get the name for, must be of format "JoystickXAxisY[+/-]" or "JoystickXButtonY".</param>
	/// <returns><c>true</c> if the name of <paramref name="control"/> corresponding to the detected controller number <c>X</c> is equivalent to <c>blacklistedName</c>.</returns>
	public static bool IsBlacklisted(string control) {
		return GetControlName(control) == blacklistedName;
	}

	/// <summary>
	/// Gets whether a specified control is blacklisted. This is useful for axes or buttons that work improperly on a controller.
	/// </summary>
	/// <param name="joystickNum">The number assigned to the joystick, between 1 and 8.</param>
	/// <param name="control">The control to get the name for, must be of format "JoystickAxisX[+/-]" or "JoystickButtonX".</param>
	/// <returns><c>true</c> if the name of <paramref name="control"/> corresponding to the detected controller number <paramref name="joystickNum"/> is equivalent to <c>blacklistedName</c>.</returns>
	public static bool IsBlacklisted(int joystickNum, string control) {
		return GetControlName(joystickNum, control) == blacklistedName;
	}

	/// <summary>
	/// Gets whether a specified button is blacklisted. This is useful for buttons that work improperly on a controller.
	/// </summary>
	/// <param name="joystickNum">The number assigned to the joystick, between 1 and 8.</param>
	/// <param name="button">The button to get the name for. Must refer to a specific joystick <c>X</c>.</param>
	/// <returns><c>true</c> if the name of <paramref name="control"/> corresponding to the detected controller number <c>X</c> is equivalent to <c>blacklistedName</c>.</returns>
	public static bool IsBlacklisted(KeyCode button) {
		return GetButtonName(button) == blacklistedName;
	}

	/// <summary>
	/// Gets whether a specified button is blacklisted. This is useful for buttons that work improperly on a controller.
	/// </summary>
	/// <param name="joystickNum">The number assigned to the joystick, between 1 and 8.</param>
	/// <param name="button">The button to get the name for.</param>
	/// <returns><c>true</c> if the name of <paramref name="control"/> corresponding to the detected controller number <paramref name="joystickNum"/> is equivalent to <c>blacklistedName</c>.</returns>
	public static bool IsBlacklisted(int joystickNum, KeyCode button) {
		return GetButtonName(joystickNum, button) == blacklistedName;
	}

	/// <summary>
	/// Loads the file corresponding to <paramref name="name"/> and adds the control names from it
	/// into the controlNames list. Should only be called on Awake or when a new joystick is added.
	/// </summary>
	/// <param name="name">Name of the joystick to load control names for.</param>
	private static Dictionary<string, string> LoadControlNamesForJoystick(string name) {
		Dictionary<string, string> controlMap = null;
		string path = "Controllers/" + platformFolder + "/" + name;
		TextAsset ta = Resources.Load<TextAsset>(path);
		if (ta != null) {
			controlMap = new Dictionary<string, string>();
			controlMap.LoadCSV(ta.text);
		}
		return controlMap;
	}

}
