using UnityEngine;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class DevConsole : MonoBehaviour {
	
#if UNITY_EDITOR
	public bool runDefaultsOnStartup = false;
#endif
	public string initialText = "";
	public GUISkin consoleSkin;
	public static Color color = Color.white;
	public static bool cheats = false;
	public static string echoBuffer = "";
	[Inaccessible] public string[] blacklistedClasses;
	[Inaccessible] public static List<string> classBlacklist = new List<string>();

#if (UNITY_ANDROID || UNITY_IPHONE) && !UNITY_EDITOR
	private static Rect consoleWindowRect = new Rect(0.0f, 0.0f, Screen.width, Screen.height * 0.5f);
#endif

	public static bool consoleUp { get { return window.open; } }
	private static Dictionary<string, string> aliases = new Dictionary<string, string>();
	private static Dictionary<KeyCode, string> binds = new Dictionary<KeyCode, string>();
	private static Dictionary<string, string> axisMappings = new Dictionary<string, string>();
	public static string configPath { get { return Application.persistentDataPath + "/config.cfg"; } }
	public static string persistent { 
		get {
			string per = Resources.Load<TextAsset>("persistent").text;
			if (Resources.Load<TextAsset>("defaults") != null) {
				StringBuilder strBuilder = new StringBuilder(per);
				strBuilder += "\n";
				strBuilder += Resources.Load<TextAsset>("defaults").text; 
				return strBuilder.ToString();
			}
			return per;

		} 
	}
	public static string autoexecPath;

	private static ConsoleWindow window;

	public void Awake() {
		SetUpInitialData();

	}

	public void Start() {

#if UNITY_EDITOR
		if (runDefaultsOnStartup) {
			Defaults();
		}
#endif

		if (File.Exists(configPath)) {
			LoadConfigFile();
		} else {
			binds = new Dictionary<KeyCode, string>();
			aliases = new Dictionary<string, string>();
			axisMappings = new Dictionary<string, string>();
			Execute(persistent.Split('\n'));
			SaveConfigFile();
		}

	}

	public static void Defaults() {
		binds = new Dictionary<KeyCode, string>();
		aliases = new Dictionary<string, string>();
		axisMappings = new Dictionary<string, string>();
		File.Delete(configPath);
		Execute(persistent.Split('\n'));
		SaveConfigFile();

	}

	public void Update() {
#if UNITY_EDITOR
		// If window is null, we've probably had a script recompile. Let's reload all the things.
		if (window == null) {
			SetUpInitialData();
			InstantiateWindowObject();
			LoadConfigFile();
		}
#endif
		if (!window.open) {
			foreach (KeyValuePair<KeyCode, string> pair in binds) {
				if (Input.GetKeyDown(pair.Key)) {
					Execute(pair.Value);
				}
				if (Input.GetKeyUp(pair.Key)) {
					string[] cmds = pair.Value.SplitUnlessInContainer(';', '\"', System.StringSplitOptions.RemoveEmptyEntries);
					foreach (string cmd in cmds) {
						if (cmd[0] == '+') {
							Execute('-' + cmd.Substring(1));
						}
					}

				}
			}
			foreach (KeyValuePair<string, string> pair in axisMappings) {
				if (pair.Key[pair.Key.Length - 1] == '-') {
					string axis = pair.Key.Substring(0, pair.Key.Length - 1);
					if (pair.Value.Contains("%value%") || pair.Value.Contains("%nvalue%")) {
						if (Input.GetAxisRaw(axis) < 0) {
							Execute(pair.Value.Replace("%value%", Input.GetAxisRaw(axis).ToString()).Replace("%nvalue%", (-Input.GetAxisRaw(axis)).ToString()));
						} else {
							Execute(pair.Value.Replace("%value%", "0").Replace("%nvalue%", "0"));
						}
					} else {
						if (Input.GetAxisRaw(axis) < -0.5f) {
							Execute(pair.Value);
						}
						if (pair.Value[0] == '+') {
							if (Input.GetAxisRaw(axis) >= -0.5f) {
								int semicolonindex = pair.Value.IndexOf(';');
								if (semicolonindex > 0) {
									Execute('-' + pair.Value.Substring(1, semicolonindex - 1));
								} else {
									Execute('-' + pair.Value.Substring(1));
								}
							}
						}
					}
				} else if (pair.Key[pair.Key.Length - 1] == '+') {
					string axis = pair.Key.Substring(0, pair.Key.Length - 1);
					if (pair.Value.Contains("%value%") || pair.Value.Contains("%nvalue%")) {
						if (Input.GetAxisRaw(axis) > 0) {
							Execute(pair.Value.Replace("%value%", Input.GetAxisRaw(axis).ToString()).Replace("%nvalue%", (-Input.GetAxisRaw(axis)).ToString()));
						} else {
							Execute(pair.Value.Replace("%value%", "0").Replace("%nvalue%", "0"));
						}
					} else {
						if (Input.GetAxisRaw(axis) > 0.5f) {
							Execute(pair.Value);
						}
						if (pair.Value[0] == '+') {
							if (Input.GetAxisRaw(axis) <= 0.5f) {
								int semicolonindex = pair.Value.IndexOf(';');
								if (semicolonindex > 0) {
									Execute('-' + pair.Value.Substring(1, semicolonindex - 1));
								} else {
									Execute('-' + pair.Value.Substring(1));
								}
							}
						}
					}
				} else {
					Execute(pair.Value.Replace("%value%", Input.GetAxisRaw(pair.Key).ToString()).Replace("%nvalue%", (-Input.GetAxisRaw(pair.Key)).ToString()));
				}
			}
		} else {
			Screen.lockCursor = false;
		}

	}

	public void OnApplicationQuit() {
		SaveConfigFile();

	}

	public void OnGUI() {
		if (window == null) {
			SetUpInitialData();
			InstantiateWindowObject();
			LoadConfigFile();
		}

		if (window.open) {
			//GUI.skin = consoleSkin;
			//GUI.skin.FontSizeFull(12, 12);
			//GUI.skin.window.fontSize = 18;
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_MAC
			//consoleWindowRect = new Rect(Mathf.Min(Mathf.Max(consoleWindowRect.x, -1 * consoleWindowRect.width + 10), Screen.width - 10), Mathf.Min(Mathf.Max(consoleWindowRect.y, -1 * GUI.skin.window.fontSize + 10), Screen.height - 10), consoleWindowRect.width, consoleWindowRect.height);
			//consoleWindowRect = GUI.Window(1, consoleWindowRect, ConsoleWindow, "Developer Console");
			window.Draw();
#endif
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
			GUI.color = new Color(0.0f, 0.0f, 0.0f, 0.6667f);
			GUI.DrawTexture(consoleWindowRect, GUIUtils.pixel);
			DrawConsole(-1);
#endif
		} else {
			//ConsoleWindow.focusTheTextField = true;
		}

	}
	
#if (UNITY_ANDROID || UNITY_IOS) && !UNITY_EDITOR
	// The GUIWindow containing the console (simply occupies the top half of the screen on mobile)
	private static void DrawConsole(int id) {
		GUI.color = Color.white;
		float heightOfFont = GUI.skin.button.LineSize();

		// Handle some inputs
		if (((Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return) || (GUI.Button(new Rect(consoleWindowRect.width * 0.9f + 5.0f, consoleWindowRect.height - heightOfFont - 5.0f, consoleWindowRect.width * 0.1f - 10.0f, heightOfFont), "Send"))) && window.textField.Length > 0) {
			Echo("> " + window.textField);
			try {
				// Execute the current line
				Execute(window.textField);
			} catch (System.Exception e) {
				Debug.LogError("Internal error executing console command:\n" + e);
			}
			window.previousCommands.Add(window.textField);
			window.cmdIndex = window.previousCommands.Count;
			window.textField = "";
			window.focusTheTextField = true;
		} else if(Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.UpArrow && window.cmdIndex > 0) {
			window.cmdIndex--;
			window.textField = window.previousCommands[window.cmdIndex];
		} else if(Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.DownArrow && window.cmdIndex < window.previousCommands.Count - 1) {
			window.cmdIndex++;
			window.textField = window.previousCommands[window.cmdIndex];
		} else if(Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.Escape || (Event.current.keyCode == KeyCode.Menu && Application.platform == RuntimePlatform.Android))) {
			window.open = false;
			window.textField = "";
		}
		GUI.SetNextControlName("ConsoleInput");
		window.textField = GUI.TextField(new Rect(5.0f, consoleWindowRect.height - heightOfFont - 5.0f, consoleWindowRect.width * 0.9f - 10.0f, heightOfFont), window.textField);
		if (window.focusTheTextField) {
			GUI.FocusControl("ConsoleInput");
			window.focusTheTextField = false;
		}

		GUI.skin.label.alignment = TextAnchor.UpperLeft;
		GUI.skin.label.wordWrap = true;
		
		GUI.skin.FontSizeFull(12.0f, 12.0f);
		float heightOfGUIContent = GUI.skin.label.CalcHeight(new GUIContent(window.textWindow), consoleWindowRect.width - 26.0f);
		Rect sizeOfLabel = new Rect(0.0f, 0.0f, consoleWindowRect.width - 26.0f, Mathf.Max(heightOfGUIContent, consoleWindowRect.height - heightOfFont - 30.0f));
		window.scrollPos = GUI.BeginScrollView(new Rect(5.0f, 20.0f, consoleWindowRect.width - 10.0f, consoleWindowRect.height - heightOfFont - 30.0f), window.scrollPos, sizeOfLabel, false, true); {
			GUI.color = new Color(0.0f, 0.0f, 0.0f, 0.6667f);
			GUI.DrawTexture(sizeOfLabel, GUIUtils.pixel);
			GUI.color = color;
			GUI.Label(sizeOfLabel, window.textWindow);
			//message.str = consoleText;
			//message.Draw(sizeOfLabel);
		} GUI.EndScrollView();

	}
#endif

	public static void Execute(string[] lines) {
		foreach (string line in lines) {
			Execute(line);
		}
	}

	public static void NetExec(string line) {
		// TODO: check to see if this line has the right attribute
		// Networked attribute on the command function
		Execute(line);
	}

	// Execute takes a line and attempts to turn it into commands which will be executed, through reflection.
	// Order: Fields (Variables), Methods (Functions), Properties, Aliases
	public static void Execute(string line) {
		line = line.Trim();
		if (line.Length < 1) { return; }
		// Allow for multiple commands separated by a semicolon
		string[] substrings = line.SplitUnlessInContainer(';', '\"');
		if (substrings.Length > 1) {
			foreach(string st in substrings) {
				Execute(st);
			}
		} else {
			// Disregard lines starting with '#'. These are "comments"!
			if (line[0] == '#') {
				return;
			}
			// Separate command from parameters
			int indexOfSpace = line.IndexOf(' ');
			string command = "";
			string parameters = "";
			if (indexOfSpace > 0) {
				command = line.Substring(0, indexOfSpace);
				parameters = line.Substring(indexOfSpace+1).Trim();
			} else {
				command = line;
				parameters = null;
			}
			string targetClassName = null;
			string targetMemberName = null;
			// Separate class specification from member call
			int indexOfDot = command.LastIndexOf('.');
			System.Type targetClass = null;
			if (indexOfDot > 0) {
				targetClassName = command.Substring(0, indexOfDot);
				if (classBlacklist.Contains(targetClassName)) {
#if !UNITY_DEBUG && !UNITY_EDITOR
					Echo("Unknown command: "+command);
					return;
#else
					Echo("Class "+targetClassName+" is blacklisted and cannot be accessed normally!");
#endif
				}
				targetMemberName = command.Substring(indexOfDot+1);
				foreach (string assembly in ReflectionUtils.assemblies) {
					targetClass = System.Type.GetType(targetClassName + assembly);
					if (targetClass != null) {
						break;
					}
				}
			} else {
				targetClass = typeof(DevConsole);
				targetMemberName = command;
			}
			// Attempt to reference the named member in named class
			if (targetClass != null) {
				try {
					if (!CallField(targetClass, targetMemberName, parameters)) {
						if (!CallMethod(targetClass, targetMemberName, parameters)) {
							if (!CallProperty(targetClass, targetMemberName, parameters)) {
								if (!aliases.ContainsKey(command)) {
									Echo("Unknown command: "+command);
								} else {
									Execute(aliases[command] + " " + parameters);
								}
							}
						}
					}
				} catch (TargetInvocationException e) {
					DevConsole.Echo("Console triggered an exception in the runtime.\n" + e.ToString().Substring(108, e.ToString().IndexOf('\n') - 109));
#if UNITY_DEBUG || UNITY_EDITOR
					throw e;
#endif
				}
			} else {
				Echo("Unknown command: "+command);
			}
		}

	}

	// Attempt to locate the member as a field, and deal with it based on the given parameters
	// Returns: boolean indicating whether the command was handled here
	public static bool CallField(System.Type targetClass, string varName, string parameters) {
		// Attempt to find the field
		FieldInfo targetVar = targetClass.GetField(varName, BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
		object targetInstance = null;
		if (targetVar == null) {
			targetInstance = GetMainOfClass(targetClass);
			if (targetInstance != null) {
				targetVar = targetClass.GetField(varName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
			}
		}
		if (targetVar == null || !IsAccessible(targetVar)) { return false; } // Fail: Couldn't find field, or it's marked inaccessible
		// If field is found, deal with it appropriately based on the parameters given
		if (parameters == null || parameters.Length < 1) {
			string output = GetFieldValue(targetInstance, targetVar);
			if (output == null) { return false; } // Fail: Field is not of a supported type
			Echo(varName + " is " + output);
			return true; // Success: Value is printed when no parameters given
		}
		if (IsCheat(targetVar) && !cheats) {
			PrintCheatMessage(targetVar.Name);
		} else {
			if (!SetFieldValue(targetInstance, targetVar, parameters.SplitUnlessInContainer(' ', '\"'))) {
				Echo("Invalid " + targetVar.FieldType.Name + ": " + parameters);
			}
		}
		return true; // Success: Whether or not the field could be set, the user is notified and the case is handled
	}

	// Get the current value of the specified field owned by instance. If instance is null then field is static.
	// Returns: results of the ToString method when called on the field, or null if field is of unsupported type.
	public static string GetFieldValue(object instance, FieldInfo fieldInfo) {
		if (fieldInfo == null) { return null; }
		// Only support types that can also be set by the user (see ParseParameterListIntoType)
		// so as not to mislead the user into thinking they can modify other types of variables
		switch (fieldInfo.FieldType.Name) {
			case "Vector2":
			case "Vector3":
			case "Color":
			case "String":
			case "Char":
			case "Byte":
			case "SByte":
			case "Int16":
			case "Int32":
			case "Int64":
			case "UInt16":
			case "UInt32":
			case "UInt64":
			case "Single":
			case "Double":
			case "Boolean": {
				return fieldInfo.GetValue(instance).ToString();
			}
			default: {
				return null;
			}
		}
	}

	// Set the current value of the specified field owned by instance. If instance is null then field is static.
	// Returns: boolean indicating whether the field was successfully changed
	public static bool SetFieldValue(object instance, FieldInfo fieldInfo, string[] parameters) {
		object result = parameters.ParseParameterListIntoType(fieldInfo.FieldType.Name);
		if (result != null) {
			fieldInfo.SetValue(instance, result);
			return true;
		} else {
			return false; // Fail: The parameters could not be parsed into the desired type
		}
	}

	// Attempt to locate the member as a property, and deal with it based on the given parameters
	// Returns: boolean indicating whether the command was handled here
	public static bool CallProperty(System.Type targetClass, string propertyName, string parameters) {
		// Attempt to find the property
		PropertyInfo targetProperty = targetClass.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);
		object targetInstance = null;
		if (targetProperty == null) {
			targetInstance = GetMainOfClass(targetClass);
			if (targetInstance != null) {
				targetProperty = targetClass.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.FlattenHierarchy);
			}
		}
		if (targetProperty == null || !IsAccessible(targetProperty)) { return false; } // Fail: Couldn't find property, or it's marked inaccessible
		// If field is found, deal with it appropriately based on the parameters given
		if (parameters == null || parameters.Length < 1) {
			string output = GetPropertyValue(targetInstance, targetProperty);
			if (output == null) { return false; } // Fail: Property is not of a supported type
			Echo(propertyName + " is " + output);
			return true; // Success: Value is printed when no parameters given
		}
		if (IsCheat(targetProperty) && !cheats) {
			PrintCheatMessage(targetProperty.Name);
		} else {
			if (!SetPropertyValue(targetInstance, targetProperty, parameters.SplitUnlessInContainer(' ', '\"'))) {
				Echo("Invalid " + targetProperty.PropertyType.Name + ": " + parameters);
			}
		}
		return true; // Success: Whether or not the field could be set (input was valid/invalid) the user is notified and the case is handled
	}

	// Get the current value of the specified property owned by instance. If instance is null then property is static.
	// Returns: results of the ToString method when called on the result of the property, "write-only" if property is write-only, or null if property is of unsupported type.
	public static string GetPropertyValue(object instance, PropertyInfo propertyInfo) {
		if (propertyInfo == null) { return null; }
		if (propertyInfo.GetGetMethod() == null) { return "write-only!"; }
		switch (propertyInfo.PropertyType.Name) {
			case "Vector2":
			case "Vector3":
			case "Color":
			case "String":
			case "Char":
			case "Byte":
			case "SByte":
			case "Int16":
			case "Int32":
			case "Int64":
			case "UInt16":
			case "UInt32":
			case "UInt64":
			case "Single":
			case "Double":
			case "Boolean": {
				return propertyInfo.GetValue(instance, null).ToString();
			}
			default: {
				return null;
			}
		}
	}

	// Set the current value of the specified property owned by instance. If instance is null then property is static.
	// Returns: boolean indicating whether the property was successfully changed, or if property is read-only.
	public static bool SetPropertyValue(object instance, PropertyInfo propertyInfo, string[] parameters) {
		if (propertyInfo.GetSetMethod() == null) {
			string output = GetPropertyValue(instance, propertyInfo);
			if (output == null) { return false; } // Fail: Property is not of a supported type
			Echo(propertyInfo.Name + " is read-only!");
			Echo(propertyInfo.Name + " is " + output);
			return true; // Success: Value is printed when property is read-only
		}
		object result = parameters.ParseParameterListIntoType(propertyInfo.PropertyType.Name);
		if (result != null) {
			propertyInfo.SetValue(instance, result, null);
			return true;
		} else {
			return false; // Fail: The parameters could not be parsed into the desired type
		}
	}

	// Attempt to find a method methodName matching the parameters given. If none is found, try to pass the entire string to a method methodName.
	// If neither of those things work, or no parameters are given, try to call a parameterless version of methodName. If none is found, either
	// methodName has no overload matching parameters given or methodName does not exist.
	// Returns: boolean indicating whether or not the command was handled here (true, if a method with the correct name was found, regardless of other failures).
	public static bool CallMethod(System.Type targetClass, string methodName, string parameters) {
		MethodInfo[] targetMethods = targetClass.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.FlattenHierarchy);
		MethodInfo[] targetInstancedMethods = new MethodInfo[0];
		object main = GetMainOfClass(targetClass);
		if (main != null) {
			targetInstancedMethods = targetClass.GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.FlattenHierarchy);
		}
		if (parameters != null && parameters.Length != 0) {
			// Try to find a static method matching name and parameters
			if (CallMethodMatchingParameters(null, methodName, targetMethods, parameters.SplitUnlessInContainer(' ', '\"'))) {
				return true;
			}
			// Try to find an instanced method matching name and parameters if a main object to invoke on exists
			if (main != null) {
				if (CallMethodMatchingParameters(main, methodName, targetInstancedMethods, parameters.SplitUnlessInContainer(' ', '\"'))) {
					return true;
				}
			}
			// Try to find a static method matching name with one string parameter
			MethodInfo targetMethod = targetClass.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.FlattenHierarchy, null, new System.Type[] { typeof(string) }, null);
			if (targetMethod != null && IsAccessible(targetMethod)) {
				if (IsCheat(targetMethod) && !cheats) {
					PrintCheatMessage(targetMethod.Name);
				} else {
					InvokeAndEchoResult(targetMethod, null, new string[] { parameters.SplitUnlessInContainer(' ', '\"').ParseParameterListIntoType("String").ToString() });
				}
				return true;
			}
			// Try to find a method matching name with one string parameter if a main object to invoke on exists
			if (main != null) {
				MethodInfo targetInstancedMethod = targetClass.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.FlattenHierarchy, null, new System.Type[] { typeof(string) }, null);
				if (targetInstancedMethod != null && IsAccessible(targetInstancedMethod)) {
					if (IsCheat(targetInstancedMethod) && !cheats) {
						PrintCheatMessage(targetInstancedMethod.Name);
					} else {
						InvokeAndEchoResult(targetInstancedMethod, main, new string[] { parameters.SplitUnlessInContainer(' ', '\"').ParseParameterListIntoType("String").ToString() });
					}
					return true;
				}
			}
		}
		// Try to find a static parameterless method matching name
		MethodInfo targetParameterlessMethod = targetClass.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.FlattenHierarchy, null, new System.Type[] { }, null);
		if (targetParameterlessMethod != null && IsAccessible(targetParameterlessMethod)) {
			if (IsCheat(targetParameterlessMethod) && !cheats) {
				PrintCheatMessage(targetParameterlessMethod.Name);
			} else {
				InvokeAndEchoResult(targetParameterlessMethod, null, new object[] { });
			}
			return true;
		}
		// Try to find a parameterless method matching name if a main object to invoke on exists
		if (main != null) {
			MethodInfo targetInstancedParameterlessMethod = targetClass.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.FlattenHierarchy, null, new System.Type[] { }, null);
			if (targetInstancedParameterlessMethod != null && IsAccessible(targetInstancedParameterlessMethod)) {
				if (IsCheat(targetInstancedParameterlessMethod) && !cheats) {
					PrintCheatMessage(targetInstancedParameterlessMethod.Name);
				} else {
					InvokeAndEchoResult(targetInstancedParameterlessMethod, main, new object[] { });
				}
				return true;
			}
		}
		// At this point no method will be invoked. Print an error message based on what has happened.
		if (targetMethods.Length > 0 || targetInstancedMethods.Length > 0) {
			bool methodWithRightNameFound = false;
			foreach (MethodInfo methodInfo in targetMethods) {
				if (methodInfo.Name == methodName && IsAccessible(methodInfo)) { methodWithRightNameFound = true; break; }
			}
			if (!methodWithRightNameFound) {
				foreach (MethodInfo methodInfo in targetInstancedMethods) {
					if (methodInfo.Name == methodName && IsAccessible(methodInfo)) { methodWithRightNameFound = true; break; }
				}
			}
			if (methodWithRightNameFound) {
				if (parameters != null && parameters.Length != 0) {
					Echo("No method "+methodName+" matching the parameters provided could be found.");
				} else {
					Echo("No method "+methodName+" taking no parameters could be found. Provide some parameters!");
				}
				// In either case, the error message is handled here, so return true;
				return true;
			}
		}
		// No method matched this command, therefore indicate a failure
		return false;
	}

	// Given a method name and an array of MethodInfo objects, try to match the name and parameter list provided on the given targetObject.
	// If targetObject is null methods are static.
	// Returns: boolean indicating whether a suitable method was found and invoked. Also whether command was handled here.
	public static bool CallMethodMatchingParameters(object targetObject, string methodName, MethodInfo[] targetMethods, string[] parameterList) {
		foreach (MethodInfo targetMethod in targetMethods) {
			if (targetMethod.Name != methodName || !IsAccessible(targetMethod)) { continue; }
			if (IsCheat(targetMethod) && !cheats) {
				PrintCheatMessage(targetMethod.Name);
			} else {
				ParameterInfo[] parameterInfos = targetMethod.GetParameters();
				if (parameterInfos.Length != parameterList.Length) { continue; }
				if (parameterInfos[0].ParameterType.Name == "String" && parameterInfos.Length == 1) { continue; }
				object[] parsedParameters = new object[parameterInfos.Length];
				bool failed = false;
				for (int i = 0; i < parsedParameters.Length; ++i) {
					// Need to split the given parameters AGAIN here if not in container, since ParseParameterListIntoType expects its parameters separately.
					// For example, if a method takes an int and a Color as an attribute, the user could type
					// Class.MethodName "7" "1 0.4 0.2 1"
					// which would get split into "7" and "1 0.4 0.2 1", and this method would try to find a method matching two parameters.
					// If such a method is found, it would further split "1 0.4 0.2 1" into four separate strings and pass them to ParseParameterListIntoType
					parsedParameters[i] = parameterList[i].SplitUnlessInContainer(' ', '\"').ParseParameterListIntoType(parameterInfos[i].ParameterType.Name);
					if (parsedParameters[i] == null) { failed = true; break; }
				}
				if (failed) { continue; }
				InvokeAndEchoResult(targetMethod, targetObject, parsedParameters);
			}
			return true;
		}
		return false;
	}

	// Invokes the target method on the target object using the parameters supplied, and echoes the ToString of the result to the console.
	// Echoes nothing if the method is void.
	public static void InvokeAndEchoResult(MethodInfo targetMethod, object targetObject, object[] parameters) {
		if (targetMethod.ReturnType == typeof(void)) {
			targetMethod.Invoke(targetObject, parameters);
		} else {
			Echo(targetMethod.Invoke(targetObject, parameters).ToString());
		}

	}

	// Returns: object reference to a "public static main" object of the same type as the class provided, if it exists within the class provided.
	public static object GetMainOfClass(System.Type targetClass) {
		FieldInfo mainField = targetClass.GetField("main", BindingFlags.Public | BindingFlags.Static);
		if (mainField != null && mainField.FieldType.IsAssignableFrom(targetClass) && IsAccessible(mainField)) {
			return mainField.GetValue(null);
		}
		mainField = targetClass.GetField("instance", BindingFlags.Public | BindingFlags.Static);
		if (mainField != null && mainField.FieldType.IsAssignableFrom(targetClass) && IsAccessible(mainField)) {
			return mainField.GetValue(null);
		}
		mainField = targetClass.GetField("singleton", BindingFlags.Public | BindingFlags.Static);
		if (mainField != null && mainField.FieldType.IsAssignableFrom(targetClass) && IsAccessible(mainField)) {
			return mainField.GetValue(null);
		}
		return null;
	}

	// Returns: boolean, true if member is not marked Inaccessible
	public static bool IsAccessible(MemberInfo member) {
#if UNITY_DEBUG || UNITY_EDITOR
		if (System.Attribute.GetCustomAttribute(member, typeof(InaccessibleAttribute)) != null) {
			Echo("Member "+member.Name+" is marked inaccessible and cannot be accessed normally!");
		}
		return true;
#else
		return System.Attribute.GetCustomAttribute(member, typeof(InaccessibleAttribute)) == null;
#endif
	}

	// Returns: boolean, true if member is marked cheat. Changing any property, field, or calling any method marked cheat through the console must trigger appropriate responses.
	public static bool IsCheat(MemberInfo member) {
#if UNITY_DEBUG
		if (System.Attribute.GetCustomAttribute(member, typeof(CheatAttribute)) != null) {
			Echo("Member "+member.Name+" is marked a cheat and cannot be accessed normally without cheats!");
		}
		return false;
#else
		return System.Attribute.GetCustomAttribute(member, typeof(CheatAttribute)) != null;
#endif
	}

	[Inaccessible] public static void PrintCheatMessage(string memberName) {
		Echo(memberName + " is a cheat command. Set \"cheats\" to 1 to use it.");
	}

	public static void ToggleConsole() {
		window.open = !window.open;
		window.textField = "";

	}

	public static void Echo() {
		Echo("");

	}

	public static void Echo(string st) {
		echoBuffer += "\n" + st.ParseNewlines();
		/*consoleText += "\n" + st.ParseNewlines();
		consoleScrollPos = new Vector2(0, heightOfGUIContent);*/
		window.textWindow += "\n" + st.ParseNewlines();

	}

	public static void Clear() {
		window.textWindow = "";
		/*consoleText = "";
		heightOfGUIContent = 0.0f;
		consoleScrollPos = Vector2.zero;*/

	}

	public static void AliasButton(string thing, string location) {
		Alias("+" + thing, location + " true");
		Alias("-" + thing, location + " false");
	}

	public static void BindButton(string key, string thing, string location) {
		AliasButton(thing, location);
		Bind(key, "+"+thing);
	}

	public static void Alias(string st) {
		string[] parameters = st.SplitUnlessInContainer(' ', '\"');
		switch (parameters.Length) {
			case 0: {
				Alias();
				break;
			}
			case 1: {
				if (aliases.ContainsKey(parameters[0])) {
					Echo(parameters[0] + " is " + aliases[parameters[0]]);
				} else {
					Echo(parameters[0] + " does not exist!");
				}
				break;
			}
			default: {
				bool containsQuote = (st.IndexOf('\"') >= 0);
				if (!containsQuote) {
					System.Text.StringBuilder rest = new System.Text.StringBuilder();
					for (int i = 1; i < parameters.Length; ++i) {
						rest.Append(' ' + parameters[i]);
					}
					Alias(parameters[0], rest.ToString().Substring(1));
				} else {
					Alias(parameters[0], parameters[1]);
				}
				break;
			}
		}

	}

	public static void Alias() {
		Echo("Alias: Allows multiple commands to be executed using one command.\nUsage: Alias <name> \"command1 \'[param1\' \'[params...]\'[;][commands...]\"");

	}

	public static void Alias(string name, string cmds) {
		if (!aliases.ContainsKey(name)) {
			aliases.Add(name, "");
		}
		aliases[name] = "";
		string[] cmdList = cmds.SplitUnlessInContainer(';', '\"');
		foreach (string cmd in cmdList) {
			aliases[name] += ';' + cmd.ReplaceFirstAndLast('\'', '\"');
		}
		aliases[name] = aliases[name].Substring(1);

	}

	public static void Unalias() {
		Echo("Unalias: Deletes an alias.\nUsage: Unalias <name>");

	}

	public static void Unalias(string st) {
		string unaliasMe = st.SplitUnlessInContainer(' ', '\"')[0];
		if (aliases.ContainsKey(unaliasMe)) {
			aliases.Remove(unaliasMe);
		}

	}

	public static void Bind(string st) {
		string[] parameters = st.SplitUnlessInContainer(' ', '\"');
		switch (parameters.Length) {
			case 0: {
				Bind();
				break;
			}
			case 1: {
				KeyCode targetKeyCode;
				try {
					targetKeyCode = (KeyCode)System.Enum.Parse(typeof(KeyCode), parameters[0]);
				} catch (System.ArgumentException) {
					if (axisMappings.ContainsKey(parameters[0])) {
						Echo(parameters[0] + " is " + axisMappings[parameters[0]]);
					} else {
						Echo(parameters[0] + " is not a valid KeyCode or the axis is not bound!");
					}
					break;
				}
				if (binds.ContainsKey(targetKeyCode)) {
					Echo(parameters[0] + " is " + binds[targetKeyCode]);
				} else {
					Echo(parameters[0] + " is unbound");
				}
				break;
			}
			default: {
				bool containsQuote = (st.IndexOf('\"') >= 0);
				if (!containsQuote) {
					System.Text.StringBuilder rest = new System.Text.StringBuilder();
					for (int i = 1; i < parameters.Length; ++i) {
						rest.Append(' ' + parameters[i]);
					}
					Bind(parameters[0], rest.ToString().Substring(1));
				} else {
					Bind(parameters[0], parameters[1]);
				}
				break;
			}
		}

	}

	public static void Bind() {
		Echo("Bind: Allows binding of commands to keypresses or axes.\nUsage: Bind <KeyCode> \"command1 \'[param1\' \'[params...]\'[;][commands...]\"");
		Echo("When binding axes, any instance of %value% will be replaced with the current position of the axis. %nvalue% gives the inverted value.");
		Echo("Additionally, you may use only half an axis by appending a \"+\" or \"-\" to the end of the axis name.");

	}

	public static void Bind(string name, string cmds) {
		KeyCode targetKeyCode;
		try {
			targetKeyCode = (KeyCode)System.Enum.Parse(typeof(KeyCode), name); // Will throw exception if KeyCode doesn't exist
			Bind(targetKeyCode, cmds);
		} catch (System.ArgumentException) {
			try {
				if (name[name.Length - 1] == '-' || name[name.Length - 1] == '+') {
					Input.GetAxisRaw(name.Substring(0, name.Length - 1)); // Will throw exception if axis doesn't exist
				} else {
					Input.GetAxisRaw(name); // Will throw exception if axis doesn't exist
				}
				if (!axisMappings.ContainsKey(name)) {
					axisMappings.Add(name, "");
				} else {
					axisMappings[name] = "";
				}
				string[] cmdList = cmds.SplitUnlessInContainer(';', '\"');
				foreach (string cmd in cmdList) {
					axisMappings[name] += ';' + cmd.ReplaceFirstAndLast('\'', '\"');
				}
				axisMappings[name] = axisMappings[name].Substring(1);
			} catch (UnityException) {
				Echo(name + " is not a valid KeyCode or axis!");
				return;
			}
		}

	}

	public static void Bind(KeyCode name, string cmds) {
		if (!binds.ContainsKey(name)) {
			binds.Add(name, "");
		} else {
			binds[name] = "";
		}
		string[] cmdList = cmds.SplitUnlessInContainer(';', '\"');
		foreach (string cmd in cmdList) {
			binds[name] += ';' + cmd.ReplaceFirstAndLast('\'', '\"');
		}
		binds[name] = binds[name].Substring(1);

	}

	public static void Poll() {
		Echo("Poll: Displays the current state of a given KeyCode or axis.\nUsage: Poll <KeyCode>");

	}

	public static void Poll(string surface) {
		KeyCode pollMe;
		string name = surface.SplitUnlessInContainer(' ', '\"')[0];
		try {
			pollMe = (KeyCode)System.Enum.Parse(typeof(KeyCode), name);
		} catch (System.ArgumentException) {
			try {
				Echo(name + " is " + Input.GetAxisRaw(name));
			} catch (UnityException) {
				Echo(name + " is not a valid KeyCode or axis!");
			}
			return;
		}
		Echo(name + " is " + Input.GetKey(pollMe));

	}

	public static void Unbind() {
		Echo("Unbind: Unbinds all commands from a key or axis.\nUsage: Unbind <KeyCode>");

	}

	public static void Unbind(string st) {
		KeyCode unbindMe;
		string name = st.SplitUnlessInContainer(' ', '\"')[0];
		try {
			unbindMe = (KeyCode)System.Enum.Parse(typeof(KeyCode), name);
			Unbind(unbindMe);
		} catch (System.ArgumentException) {
			if (axisMappings.ContainsKey(name)) {
				axisMappings.Remove(name);
			} else {
				Echo(name + " is not a valid KeyCode or the axis is not bound!");
			}
		}
	}

	public static void Unbind(KeyCode key) {
		if (binds.ContainsKey(key)) {
			binds.Remove(key);
		}
	}

	public static void Exec(string path) {
		StreamReader sr;
		if (File.Exists(path)) {
			sr = File.OpenText(path);
		} else {
			if (File.Exists(Application.persistentDataPath + "/" + path)) {
				sr = File.OpenText(Application.persistentDataPath + "/" + path);
			} else {
				if (File.Exists(Application.dataPath + "/" + path)) {
					sr = File.OpenText(Application.dataPath + "/" + path);
				} else {
					Echo("Unable to find script file to execute "+path);
					return;
				}
			}
		}
		while (!sr.EndOfStream) {
			Execute(sr.ReadLine());
		}
		sr.Close();

	}

	public static void SaveConfigFile() {
		#if !UNITY_WEBPLAYER
		if (File.Exists(configPath)) {
			File.Delete(configPath);
		}
		
		StreamWriter sw = File.CreateText(configPath);
		sw.WriteLine("autoexecPath \"" + autoexecPath + "\"");
		foreach (string alias in aliases.Keys) {
			sw.WriteLine("Alias \"" + alias + "\" \"" + aliases[alias].Replace('\"', '\'') + "\"");
		}
		foreach (KeyCode bind in binds.Keys) {
			sw.WriteLine("Bind \"" + bind.ToString() + "\" \"" + binds[bind].Replace('\"', '\'') + "\"");
		}
		foreach (string bind in axisMappings.Keys) {
			sw.WriteLine("Bind \"" + bind.ToString() + "\" \"" + axisMappings[bind].Replace('\"', '\'') + "\"");
		}
		sw.Close();

		#endif
	}

	public static void LoadConfigFile() {
		if (File.Exists(configPath)) {
			Execute(persistent.Split('\n'));
			// If config exists, clear binds after loading persistent file (we want the default aliases but not the keybinds)
			binds = new Dictionary<KeyCode, string>();
#if UNITY_EDITOR
			binds.Add(KeyCode.F1, "ToggleConsole");
#endif
			// All preexisting keybinds will be reloaded from this file instead
			Exec(configPath);
			if (File.Exists(autoexecPath)) {
				Exec(autoexecPath);
			}
		}
	}

	private void InstantiateWindowObject() {
		window = (ConsoleWindow)new ConsoleWindow()
			.Named("Developer Console")
			.Resizable()
			.Closed()
			.Area(Screen.all.MiddleCenter(0.7f, 0.8f).Move(0.1f, 0.0f));
		window.textWindow = initialText.ParseNewlines();
	}

	public void SetUpInitialData() {
		autoexecPath = Application.persistentDataPath + "/autoexec.cfg";
		classBlacklist = blacklistedClasses.ToList<string>();
		if (!classBlacklist.Contains("InAppPurchases")) { classBlacklist.Add("InAppPurchases"); }
		if (!classBlacklist.Contains("AdManager")) { classBlacklist.Add("AdManager"); }
	}

	public static void Quit() {
#if UNITY_EDITOR
		EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif

	}

	public static List<KeyCode> GetKeysByCommand(string command) {
		List<KeyCode> ret = new List<KeyCode>();
		foreach (KeyValuePair<KeyCode, string> kvp in binds) {
			if (command == kvp.Value) {
				ret.Add(kvp.Key);
			}
		}
		return ret;
	}

	public class CheatAttribute : System.Attribute {

		public CheatAttribute() { }

	}

	public class InaccessibleAttribute : System.Attribute {

		public InaccessibleAttribute() { }

	}
}

public class ConsoleWindow : ZWindow {

	[DevConsole.Inaccessible] public Vector2 scrollPos = Vector2.zero;
	[DevConsole.Inaccessible] public string textWindow = "";
	[DevConsole.Inaccessible] public string textField = "";

	public List<string> previousCommands = new List<string>();
	[DevConsole.Inaccessible] public int cmdIndex = 0;
	[DevConsole.Inaccessible] public bool focusTheTextField = false;

	public override void Window() {
		GUILayout.BeginVertical(); {
			scrollPos = GUILayout.BeginScrollView(scrollPos, GUIStyle.none, GUI.skin.verticalScrollbar); {
				if (textWindow.Length > 16382) {
					textWindow = textWindow.Substring(textWindow.Length - 16382, 16382);
				}
				Label(textWindow);
			} GUILayout.EndScrollView();
			GUILayout.BeginHorizontal(); {
				GUI.SetNextControlName("ConsoleInput");
				if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return && GUI.GetNameOfFocusedControl() == "ConsoleInput" && textField.Length > 0) {
					TryExecute(textField);
					textField = "";
					Event.current.Use();
				} else if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.UpArrow && cmdIndex > 0) {
					cmdIndex--;
					textField = previousCommands[cmdIndex];
				} else if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.DownArrow && cmdIndex < previousCommands.Count - 1) {
					cmdIndex++;
					textField = previousCommands[cmdIndex];
				} else if (Event.current.type == EventType.KeyDown && (Event.current.keyCode == KeyCode.Escape || (Event.current.keyCode == KeyCode.Menu && Application.platform == RuntimePlatform.Android))) {
					open = false;
					textField = "";
				}
				textField = GUILayout.TextField(textField);
				if (GUILayout.Button("Send", GUILayout.ExpandWidth(false)) && textField.Length > 0) {
					TryExecute(textField);
					textField = "";
				}
			} GUILayout.EndHorizontal();
		} GUILayout.EndVertical();
		if (focusTheTextField && Event.current.type == EventType.Repaint) {
			GUI.FocusControl("ConsoleInput");
		}
	}

	public void TryExecute(string cmd) {
		DevConsole.Echo("> " + cmd);
		try {
			// Execute the current line
			DevConsole.Execute(cmd);
		} catch (System.Exception e) {
			Debug.LogError("Internal error executing console command:\n" + e);
		}
		previousCommands.Add(cmd);
		cmdIndex = previousCommands.Count;
		focusTheTextField = true;
	}

}
