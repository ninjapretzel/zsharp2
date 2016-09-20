using UnityEngine;
using System.Linq;
using System.Collections.Generic;
#if UNITY_STANDALONE && LG_STEAM
using Steamworks;
#elif UNITY_XBOXONE
using Gamepad;
#endif

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
#if (UNITY_STANDALONE_WIN && !UNITY_EDITOR) || UNITY_EDITOR_WIN
	public const string platformFolder = "Windows";
#elif (UNITY_STANDALONE_OSX && !UNITY_EDITOR) || UNITY_EDITOR_OSX
	public const string platformFolder = "OSX";
#elif (UNITY_STANDALONE_LINUX && !UNITY_EDITOR) || UNITY_EDITOR_LINUX
	public const string platformFolder = "Linux";
#elif UNITY_ANDROID
	public const string platformFolder = "Android";
#elif UNITY_XBOXONE
	public const string platformFolder = "XboxOne";
#else
	public const string platformFolder = "";
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

	private static JsonObject glyphData;

#if UNITY_STANDALONE && LG_STEAM
	public static Dictionary<string, ControllerAnalogActionHandle_t> analogActionHandles;
	public static Dictionary<string, ControllerDigitalActionHandle_t> digitalActionHandles;
	public static Dictionary<string, ControllerActionSetHandle_t> actionSetHandles;
	private static Dictionary<EControllerActionOrigin, string> actionOriginGlyphNames;

	protected ulong lastControlSet = ulong.MaxValue;
#endif

	public virtual void Awake() {
		if (instance != null) {
			Destroy(this);
			return;
		}
		instance = this;
#if UNITY_XBOXONE && !UNITY_EDITOR
		joystickNames = new List<string>() { "","","","","","","","" };
		controlNames.Add(LoadControlNamesForJoystick("Windows.Xbox.Input.Gamepad"));
		for (uint i = 0; i < 8; ++i) {
			if (XboxOneInput.IsGamepadActive(i + 1)) {
				joystickNames[(int)i] = "Windows.Xbox.Input.Gamepad";
			}
		}
#else
		joystickNames = Input.GetJoystickNames().ToList();
		for (int i = 0; i < joystickNames.Count; ++i) {
			string name = joystickNames[i];
			if (OnJoystickConnected != null) {
				OnJoystickConnected(i + 1, name);
			}
			controlNames.Add(LoadControlNamesForJoystick(GetControllerName(name)));
		}
#endif

		JsonObject gameActions = Json.Parse(Resources.Load<TextAsset>("GameActions").text) as JsonObject;
		glyphData = gameActions.Get<JsonObject>("GlyphSets");

#if UNITY_STANDALONE && LG_STEAM

		actionSetHandles = new Dictionary<string, ControllerActionSetHandle_t>();
		analogActionHandles = new Dictionary<string, ControllerAnalogActionHandle_t>();
		digitalActionHandles = new Dictionary<string, ControllerDigitalActionHandle_t>();
		
		JsonArray actionSets = gameActions.Get<JsonArray>("ActionSets");
		JsonArray analogActions = gameActions.Get<JsonArray>("AnalogActions");
		JsonArray digitalActions = gameActions.Get<JsonArray>("DigitalActions");
		if (SteamManager.Initialized) {
			if (actionSets != null) {
				foreach (var st in actionSets) {
					actionSetHandles.Add(st.stringVal, SteamController.GetActionSetHandle(st.stringVal));
				}
			}
			if (analogActions != null) {
				foreach (var st in analogActions) {
					analogActionHandles.Add(st.stringVal, SteamController.GetAnalogActionHandle(st.stringVal));
				}
			}
			if (digitalActions != null) {
				foreach (var st in digitalActions) {
					digitalActionHandles.Add(st.stringVal, SteamController.GetDigitalActionHandle(st.stringVal));
				}
			}
		}

		actionOriginGlyphNames = new Dictionary<EControllerActionOrigin, string>() {
			{ EControllerActionOrigin.k_EControllerActionOrigin_A, "steam_button_a" },
			{ EControllerActionOrigin.k_EControllerActionOrigin_B, "steam_button_b" },
			{ EControllerActionOrigin.k_EControllerActionOrigin_X, "steam_button_x" },
			{ EControllerActionOrigin.k_EControllerActionOrigin_Y, "steam_button_y" },
			{ EControllerActionOrigin.k_EControllerActionOrigin_LeftBumper, "steam_shoulder_l" },
			{ EControllerActionOrigin.k_EControllerActionOrigin_RightBumper, "steam_shoulder_r" },
			{ EControllerActionOrigin.k_EControllerActionOrigin_LeftGrip, "steam_grip_l" },
			{ EControllerActionOrigin.k_EControllerActionOrigin_RightGrip, "steam_grip_r" },
			{ EControllerActionOrigin.k_EControllerActionOrigin_Start, "steam_button_start" },
			{ EControllerActionOrigin.k_EControllerActionOrigin_Back, "steam_button_select" },
			{ EControllerActionOrigin.k_EControllerActionOrigin_LeftPad_Touch, "steam_pad_l_touch" },
			{ EControllerActionOrigin.k_EControllerActionOrigin_LeftPad_Swipe, "steam_pad_l_swipe" },
			{ EControllerActionOrigin.k_EControllerActionOrigin_LeftPad_Click, "steam_pad_l_click" },
			{ EControllerActionOrigin.k_EControllerActionOrigin_LeftPad_DPadNorth, "steam_pad_l_dpad_n" },
			{ EControllerActionOrigin.k_EControllerActionOrigin_LeftPad_DPadSouth, "steam_pad_l_dpad_s" },
			{ EControllerActionOrigin.k_EControllerActionOrigin_LeftPad_DPadWest, "steam_pad_l_dpad_w" },
			{ EControllerActionOrigin.k_EControllerActionOrigin_LeftPad_DPadEast, "steam_pad_l_dpad_e" },
			{ EControllerActionOrigin.k_EControllerActionOrigin_RightPad_Touch, "steam_pad_r_touch" },
			{ EControllerActionOrigin.k_EControllerActionOrigin_RightPad_Swipe, "steam_pad_r_swipe" },
			{ EControllerActionOrigin.k_EControllerActionOrigin_RightPad_Click, "steam_pad_r_click" },
			{ EControllerActionOrigin.k_EControllerActionOrigin_RightPad_DPadNorth, "steam_pad_r_dpad_n" },
			{ EControllerActionOrigin.k_EControllerActionOrigin_RightPad_DPadSouth, "steam_pad_r_dpad_s" },
			{ EControllerActionOrigin.k_EControllerActionOrigin_RightPad_DPadWest, "steam_pad_r_dpad_w" },
			{ EControllerActionOrigin.k_EControllerActionOrigin_RightPad_DPadEast, "steam_pad_r_dpad_e" },
			{ EControllerActionOrigin.k_EControllerActionOrigin_LeftTrigger_Pull, "steam_trigger_l_pull" },
			{ EControllerActionOrigin.k_EControllerActionOrigin_LeftTrigger_Click, "steam_trigger_l_click" },
			{ EControllerActionOrigin.k_EControllerActionOrigin_RightTrigger_Pull, "steam_trigger_r_pull" },
			{ EControllerActionOrigin.k_EControllerActionOrigin_RightTrigger_Click, "steam_trigger_r_click" },
			{ EControllerActionOrigin.k_EControllerActionOrigin_LeftStick_Move, "steam_stick_move" },
			{ EControllerActionOrigin.k_EControllerActionOrigin_LeftStick_Click, "steam_stick_click" },
			{ EControllerActionOrigin.k_EControllerActionOrigin_LeftStick_DPadNorth, "steam_stick_dpad_n" },
			{ EControllerActionOrigin.k_EControllerActionOrigin_LeftStick_DPadSouth, "steam_stick_dpad_s" },
			{ EControllerActionOrigin.k_EControllerActionOrigin_LeftStick_DPadWest, "steam_stick_dpad_w" },
			{ EControllerActionOrigin.k_EControllerActionOrigin_LeftStick_DPadEast, "steam_stick_dpad_e" },
			{ EControllerActionOrigin.k_EControllerActionOrigin_Gyro_Move, "steam_gyro" },
			{ EControllerActionOrigin.k_EControllerActionOrigin_Gyro_Pitch, "steam_gyro" },
			{ EControllerActionOrigin.k_EControllerActionOrigin_Gyro_Yaw, "steam_gyro" },
			{ EControllerActionOrigin.k_EControllerActionOrigin_Gyro_Roll, "steam_gyro" }
		};
#endif

#if UNITY_XBOXONE
		XboxOneInput.OnGamepadStateChange += OnGamepadStateChange;
#endif
	}

	/// <summary>
	/// Checks Unity's current list of joysticks against an internal one, and calls
	/// OnJoystickConnected when a new joystick is connected, OnJoystickDisconnected
	/// when a joystick is disconnected, and OnJoystickReconnected when a joystick is
	/// reconnected.
	/// </summary>
	public virtual void Update() {
#if !UNITY_XBOXONE || UNITY_EDITOR
		string[] names = Input.GetJoystickNames();
		if (names.Length > joystickNames.Count) {
			// On Windows, when a joystick is disconnected the array stays the same length but joystick name gets set to "".
			for (int i = joystickNames.Count; i < names.Length; ++i) {
				if (OnJoystickConnected != null) {
					OnJoystickConnected(i + 1, names[i]);
				}
				joystickNames.Add(names[i]);
				controlNames.Add(LoadControlNamesForJoystick(GetControllerName(names[i])));
			}
		} else if (names.Length < joystickNames.Count) {
			// On some other platforms, the joystick might actually be completely removed from the array.
			// This is INCORRECT behavior, but we still need to handle it until Unity fixes their BS.
			int iCurrentNames = 0;
			for (int i = 0; i < joystickNames.Count; ++i) {
				if (joystickNames[i].Length > 0 && (iCurrentNames >= names.Length || joystickNames[i] != names[iCurrentNames])) {
					if (OnJoystickDisconnected != null) {
						OnJoystickDisconnected(i + 1, joystickNames[i] + "");
					}
					joystickNames[i] = "";
				} else {
					++iCurrentNames;
				}
			}
		} else {
			// Name arrays are same length
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
						controlNames[i] = LoadControlNamesForJoystick(GetControllerName(names[i]));
					}
				}
			}
		}
#endif
	}

	/// <summary>
	/// Parses a name from GetJoystickNames() to get the actual name of the controller.
	/// For example, "Xbox One for Windows (Controller)" and "Controller (Xbox One for Windows)"
	/// will both return "Xbox One for Windows".
	/// </summary>
	/// <param name="name">The name from GetJoystickNames().</param>
	/// <returns>The parsed name, or <paramref name="name"/> if nothing changed.</returns>
	public static string GetControllerName(string name) {
		if (name.EndsWith(" (Controller)")) {
			return name.Substring(0, name.Length - " (Controller)".Length);
		} else if (name.StartsWith("Controller (")) {
			return name.Substring("Controller (".Length, name.Length - "Controller (".Length - 1);
		}
		return name.Replace(":", "").Replace("/", "").Replace("\\", "");
	}

	/// <summary>
	/// Gets the name for a specified axis. Out parameter will be filled with the joystick's number as determined from <param name="control"/>.
	/// </summary>
	/// <param name="joystickNum"><c>out</c> parameter that will be filled with the joystick's number as determined from <param name="control"/>, or -1 if not a joystick.</param>
	/// <param name="control">The control to get the name for, must be of format "JoystickXAxisY[+/-]" or "JoystickXButtonY".</param>
	/// <returns>The name of <paramref name="control"/> corresponding to the detected controller number <c>X</c>, or <paramref name="control"/> if one is not provided.</returns>
	public static string GetControlName(out int joystickNum, string control) {
		if (control.Length < 9) { joystickNum = -1; return control; }
		if (!int.TryParse(control[8].ToString(), out joystickNum)) {
			joystickNum = -1;
			return control;
		}
		return GetControlName(joystickNum, control.Substring(9));
	}

	/// <summary>
	/// Gets the name for a specified axis.
	/// </summary>
	/// <param name="joystickNum">The number assigned to the joystick, between 1 and 8.</param>
	/// <param name="control">The control to get the name for, must be of format "AxisX[+/-]" or "ButtonX".</param>
	/// <returns>The name of <paramref name="control"/> corresponding to the detected controller number <paramref name="joystickNum"/>, or <paramref name="control"/> if one is not provided.</returns>
	public static string GetControlName(int joystickNum, string control) {
		if (controlNames.Count >= joystickNum && controlNames[joystickNum - 1] != null && controlNames[joystickNum - 1].ContainsKey(control)) {
			return controlNames[joystickNum - 1][control];
		}
		return "Joystick" + joystickNum.ToString() + control;
	}

	/// <summary>
	/// Gets the name for a specified axis, of the primary controller
	/// </summary>
	/// <param name="control">The control to get the name for, must be of format "AxisX[+/-]" or "ButtonX".</param>
	/// <returns><paramref name="control"/> corresponding to the primary controller number.</returns>
	public static string GetControlName(string control) {
		int primary = Input.primaryControllerIndex;
		return GetControlName(primary, control);
	}

	/// <summary>
	/// Gets the name for a specified button. Out parameter will be filled with the joystick's number as determined from <param name="button"/>.
	/// </summary>
	/// <param name="joystickNum"><c>out</c> parameter that will be filled with the joystick's number as determined from <param name="button"/>, or -1 if not a joystick.</param>
	/// <param name="button">The button to get the name for. Must refer to a specific joystick <c>X</c>.</param>
	/// <returns>The name of <paramref name="button"/> corresponding to the detected controller number <c>X</c>, or <paramref name="button"/> if one is not provided.</returns>
	public static string GetButtonName(out int joystickNum, KeyCode button) {
		if (button < KeyCode.Joystick1Button0) { joystickNum = -1; return button.ToString(); }
		return GetControlName(out joystickNum, button.ToString());
	}

	/// <summary>
	/// Gets the name for a specified button. 
	/// </summary>
	/// <param name="button">The button to get the name for. Must refer to a specific joystick <c>X</c>.</param>
	/// <returns>The name of <paramref name="button"/> corresponding to the primary controller number <c>x</c></returns>
	public static string GetButtonName(KeyCode button) {
		int primary = Input.primaryControllerIndex;
		return GetButtonName(primary, button);
	}

	/// <summary>
	/// Gets the name for a specified button.
	/// </summary>
	/// <param name="button">The button to get the name for.</param>
	/// <param name="joystickNum">The number assigned to the joystick, between 1 and 8.</param>
	/// <returns>The name of <paramref name="button"/> corresponding to the detected controller number <paramref name="joystickNum"/>, or <paramref name="button"/> if one is not provided.</returns>
	public static string GetButtonName(int joystickNum, KeyCode button) {
		if (button < KeyCode.JoystickButton0) { return button.ToString(); }
		if (button > KeyCode.JoystickButton19) {
			button -= 20 * joystickNum;
		}
		return GetControlName(joystickNum, button.ToString().Substring(8));
	}

	/// <summary>
	/// Gets whether a specified control is blacklisted. This is useful for axes or buttons that work improperly on a controller.
	/// </summary>
	/// <param name="joystickNum">The number assigned to the joystick, between 1 and 8.</param>
	/// <param name="control">The control to get the name for, must be of format "JoystickXAxisY[+/-]" or "JoystickXButtonY".</param>
	/// <returns><c>true</c> if the name of <paramref name="control"/> corresponding to the detected controller number <c>X</c> is equivalent to <c>blacklistedName</c>.</returns>
	public static bool IsBlacklisted(string control) {
		int joystickNum;
		return GetControlName(out joystickNum, control) == blacklistedName;
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
		int joystickNum;
		return GetButtonName(out joystickNum, button) == blacklistedName;
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
		name = name.RemoveAll(':');
		string path = "Controllers/" + platformFolder + "/" + name;
		// Terrible crazy icky hack to load configs specifically on Windows 10 (and up, presumably).
		TextAsset ta = null;
#if (UNITY_STANDALONE_WIN && !UNITY_EDITOR) || UNITY_EDITOR_WIN
		ta = Resources.Load<TextAsset>(path + "_w10");
		if (ta == null || !SystemInfo.operatingSystem.StartsWith("Windows 1")) {
#endif
			ta = Resources.Load<TextAsset>(path);
#if (UNITY_STANDALONE_WIN && !UNITY_EDITOR) || UNITY_EDITOR_WIN
		}
#endif
		if (ta != null) {
			controlMap = new Dictionary<string, string>();
			controlMap.LoadCSV(ta.text);
		}
		return controlMap;
	}

	/// <summary>
	/// Gets the glyph replacement for the controls bound to a controller, or a human-readable string
	/// of the controls otherwise.
	/// </summary>
	/// <param name="thing">The control to get a glyph for. Defined in GameActions.json.</param>
	/// <returns>The name of the glyph to use in place of this control (in the format "{glyphname}"),
	/// or if no glyph replacement is available, a human-readable string of the relevant controls.</returns>
	public static string Glyph(string thing) {
		if (glyphData != null) {
			JsonObject glyph = glyphData.Get<JsonObject>(thing);
			if (glyph != null) {
#if UNITY_STANDALONE && LG_STEAM
				string steamAction = glyph.Get<string>("steam_action");
				if (analogActionHandles.ContainsKey(steamAction)) {
					string steamGlyph = GetSteamGlyph(GetCurrentActionSet(), analogActionHandles[steamAction]);
					if (steamGlyph != null) { return steamGlyph; }
				} else if (digitalActionHandles.ContainsKey(steamAction)) {
					string steamGlyph = GetSteamGlyph(GetCurrentActionSet(), digitalActionHandles[steamAction]);
					if (steamGlyph != null) { return steamGlyph; }
				}
#endif
				string[] keys = glyph.Get<string[]>("keys");
				string[] axes = glyph.Get<string[]>("axes");
				string[] tokens = new string[keys.Length];

				int joystickNum = -1;
				for (int i = 0; i < tokens.Length; ++i) {
					tokens[i] = GetControlName(out joystickNum, DevConsole.GetBindForCommand(keys[i], axes[i]));
				}
				
				string oneThing = "";
				if (tokens.Length == 0) {
					return thing;
				} else if (tokens.Length == 1) {
					if (joystickNum > 0 && joystickNames.Count >= joystickNum) {
						if (controlNames[joystickNum - 1] != null && controlNames[joystickNum - 1].ContainsKey("Sheet")) {
							return "[" + controlNames[joystickNum - 1]["Sheet"] + "_" + tokens[0].RemoveAll(' ') + "]";
						} else {
							return Localization.Localize(tokens[0]);
						}
					} else {
						return Localization.Localize(tokens[0]);
					}
				} else {
					if (tokens[0].LastIndexOf(' ') >= 0) {
						oneThing = tokens[0].Substring(0, tokens[0].LastIndexOf(' '));
					} else if (tokens[0].StartsWith("MouseAxis")) {
						oneThing = "MouseAxis";
					}
				}

				StringBuilder output = new StringBuilder();
				for (int i = 0; i < tokens.Length; ++i) {
					if (oneThing.Length > 0) {
						if(!tokens[i].StartsWith(oneThing) || (oneThing != "MouseAxis" && !(tokens[i].EndsWith("Left") || tokens[i].EndsWith("Right") || tokens[i].EndsWith("Up") || tokens[i].EndsWith("Down")))) {
							oneThing = "";
						}
					}
					output.Append(Localization.Localize(tokens[i]));
					if (i < tokens.Length - 1) {
						output.Append(", ");
					}
					if (i == tokens.Length - 2) {
						output.Append(Localization.Localize("and"))
						.Append(' ');
					}
				}
				if (oneThing.Length > 0) {
					if (oneThing == "MouseAxis") {
						return Localization.Localize("The Mouse");
					} else {
						if (joystickNum > 0 && joystickNames.Count >= joystickNum) {
							if (controlNames[joystickNum - 1] != null && controlNames[joystickNum - 1].ContainsKey("Sheet")) {
								return "[" + controlNames[joystickNum - 1]["Sheet"] + "_" + oneThing.RemoveAll(' ') + "]";
							} else {
								return Localization.Localize(oneThing);
							}
						} else {
							return Localization.Localize(oneThing);
						}
					}
				}
				return output.ToString();
			} else {
				int joystickNum = -1;
				string token = GetControlName(out joystickNum, thing);
				
				if (joystickNum > 0 && joystickNames.Count >= joystickNum) {
					if (controlNames[joystickNum - 1] != null && controlNames[joystickNum - 1].ContainsKey("Sheet")) {
						return "[" + controlNames[joystickNum - 1]["Sheet"] + "_" + token.RemoveAll(' ') + "]";
					} else {
						return Localization.Localize(token);
					}
				} else {
					return Localization.Localize(token);
				}
			}
		}

		return thing;
	}

	public virtual void LateUpdate() {
#if UNITY_STANDALONE && LG_STEAM
		lastControlSet = GetCurrentActionSet().m_ControllerActionSetHandle;
#endif
	}

#if UNITY_STANDALONE && LG_STEAM
	public static void SetCurrentActionSet(string name) {
		if (!SteamManager.Initialized) {
			return;
		}
		if (actionSetHandles.ContainsKey(name)) {
			ControllerHandle_t handle;
			if (SteamControllerConnected(out handle)) {
				SteamController.ActivateActionSet(handle, actionSetHandles[name]);
			}
		} else {
			Debug.LogErrorFormat("No Steam Controller action set defined with name {0}", name);
		}
	}
	
	public static ControllerActionSetHandle_t GetCurrentActionSet() {
		ControllerHandle_t handle;
		if (SteamManager.Initialized && SteamControllerConnected(out handle)) {
			return SteamController.GetCurrentActionSet(handle);
		}
		return default(ControllerActionSetHandle_t);
	}

	public static string GetSteamGlyph(string action) {
		if (analogActionHandles.ContainsKey(action)) {
			return GetSteamGlyph(GetCurrentActionSet(), analogActionHandles[action]);
		} else if (digitalActionHandles.ContainsKey(action)) {
			return GetSteamGlyph(GetCurrentActionSet(), digitalActionHandles[action]);
		}
		return null;
	}

	public static string GetSteamGlyph(ControllerActionSetHandle_t set, ControllerDigitalActionHandle_t action) {
		ControllerHandle_t controllerHandle;
		if (SteamControllerConnected(out controllerHandle)) {
			EControllerActionOrigin[] origins = new EControllerActionOrigin[Constants.STEAM_CONTROLLER_MAX_ORIGINS];
			if (SteamController.GetDigitalActionOrigins(controllerHandle, set, action, origins) > 0) {
				return "{" + actionOriginGlyphNames[origins[0]] + "}";
			}
		}
		return null;
	}

	public static string GetSteamGlyph(ControllerActionSetHandle_t set, ControllerAnalogActionHandle_t action) {
		ControllerHandle_t controllerHandle;
		if (SteamControllerConnected(out controllerHandle)) {
			EControllerActionOrigin[] origins = new EControllerActionOrigin[Constants.STEAM_CONTROLLER_MAX_ORIGINS];
			if (SteamController.GetAnalogActionOrigins(controllerHandle, set, action, origins) > 0) {
				return "{" + actionOriginGlyphNames[origins[0]] + "}";
			}
		}
		return null;
	}

	public static bool SteamControllerConnected(out ControllerHandle_t handle) {
		if (SteamManager.Initialized) {
			ControllerHandle_t[] controllers = new ControllerHandle_t[Constants.STEAM_CONTROLLER_MAX_COUNT];
			int numControllers = SteamController.GetConnectedControllers(controllers);
			if (numControllers > 0) {
				handle = controllers[0];
				return true;
			}
		}
		handle = default(ControllerHandle_t);
		return false;
	}

	public static bool SteamControllerConnected() {
		if (SteamManager.Initialized) {
			ControllerHandle_t[] controllers = new ControllerHandle_t[Constants.STEAM_CONTROLLER_MAX_COUNT];
			int numControllers = SteamController.GetConnectedControllers(controllers);
			if (numControllers > 0) {
				return true;
			}
		}
		return false;
	}
#endif

#if UNITY_XBOXONE
	public void OnGamepadStateChange(uint index, bool isConnected) {
		// Hardcode these to report the identifier for a controller on the Xbone, since this is Xbox specific code anyway.
		if (isConnected) {
			joystickNames[(int)index] = "Windows.Xbox.Input.Gamepad";
			if (OnJoystickConnected != null) {
				OnJoystickConnected((int)index, "Windows.Xbox.Input.Gamepad");
			}
		} else {
			joystickNames[(int)index] = "";
			if (OnJoystickDisconnected != null) {
				OnJoystickDisconnected((int)index, "Windows.Xbox.Input.Gamepad");
			}
		}
	}
#endif

}
