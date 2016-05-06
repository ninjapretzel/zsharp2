using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Provides an interface for the user to run code or easily change variables in the game.
/// </summary>
public class DevConsole : MonoBehaviour, ILogHandler {

	[Tooltip("Text that will be initially displayed in the console.")]
	[SerializeField] private string initialText = "";
	/// <summary>Color of the text in the console.</summary>
	public static Color color = Color.white;
	/// <summary>Boolean value indicating whether to allow cheat commands to be run.</summary>
	public static bool cheats = false;
	/// <summary>Debug level of the console. Errors and exceptions will always be printed in the console.
	/// Setting this value to 1 will print warnings as well, and 2 will print all debug logs as well.
	/// Setting to 3 will also echo every single command that is executed, and is not recommended unless
	/// trying to see what exactly is being executed and how often.</summary>
	public static int debug = 0;
	[Tooltip("Classes, namespaces or members which should never be accessed from the DevConsole.")]
	[SerializeField] private string[] blacklist;
	/// <summary>Classes, namespaces or members which should never be accessed from the DevConsole, static version.
	/// The serialized blacklist is copied into the static one on start.</summary>
	private static List<string> classBlacklist = new List<string>();
	[Inaccessible] private static bool _wasCheats = false;
	/// <summary>Tracks whether cheats were ever enabled during this session.</summary>
	public static bool wasCheats { get { return _wasCheats; } }

	/// <summary>Action to be taken when an exception is logged by the DevConsole.</summary>
	public static Action<Exception, object> OnException;
	/// <summary>Is the console window currently showing?</summary>
	public static bool consoleUp { get { return window.open; } }
	/// <summary>Aliases can be called like regular commands and contain commands to be run when the alias is called.
	/// This way, very long or complicated strings of commands can be run using a single, short, easy to remember command.</summary>
	private static Dictionary<string, string> aliases = new Dictionary<string, string>();
	/// <summary>Binds map a list of commands to a key, mouse or joystick button. The commands will be run when the key is pressed.</summary>
	private static Dictionary<KeyCode, string> binds = new Dictionary<KeyCode, string>();
	/// <summary>Axis mappings are commands that are run every frame, replacing some part of the command with the value from the specified axis.
	/// Since these run every frame regardless, use them sparingly. In the command <c>%value%</c> will be replaced with the value of the axis,
	/// and <c>%nvalue%</c> will be replaced with the value of the axis multiplied by -1.</summary>
	private static Dictionary<string, string> axisMappings = new Dictionary<string, string>();
	/// <summary>Returns the path there the config.cfg lives, where binds and aliases are saved across sessions.</summary>
	public static string configPath { get { return Application.persistentDataPath + "/config.cfg"; } }
	/// <summary>Contains all commands in the "persistent" and "defaults" files in Resources.</summary>
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

	/// <summary>User-definable autoexec path, pointing to a script that will automatically execute when the game is run.</summary>
	public static string autoexecPath = "";

	/// <summary>The window being used to interact with the console systems.</summary>
	private static ConsoleWindow window;

	/// <summary>Run as soon as the Behaviour is created.</summary>
	protected void Awake() {
		SetUpInitialData();
		Application.logMessageReceived += LogCallback;

	}

	/// <summary>Run when the GameObject this Behaviour is attached to is destroyed.</summary>
	protected void OnDestroy() {
		Application.logMessageReceived -= LogCallback;

	}

	/// <summary>Run before Update on the frame after the Behaviour is created.</summary>
	protected void Start() {
		if (File.Exists(configPath)) {
			LoadConfigFile();
		} else {
			Defaults();
		}

	}


	/// <summary> Calls Defaults(true, true); for easy use from console. </summary>
	public static void Defaults() { Defaults(true, true); }

	/// <summary>
	/// Deletes the config.cfg and recreates it using the settings specified in <see cref="persistent"/>.
	/// </summary>
	/// <param name="resetBinds">Optional boolean specifying whether to wipe all binds and axis mappings before restoring persistent settings.</param>
	/// <param name="resetAliases">Optional boolean specifying whether to wipe all aliases before restoring persistent settings.</param>
	public static void Defaults(bool resetBinds, bool resetAliases) {
		if (resetAliases) {
			aliases = new Dictionary<string, string>();
		}
		if (resetBinds) {
			binds = new Dictionary<KeyCode, string>();
			axisMappings = new Dictionary<string, string>();
		}
		if (File.Exists(configPath)) {
			File.Delete(configPath);
		}
		Execute(persistent.Split('\n'));
		SaveConfigFile();

	}

	/// <summary>Run every frame.</summary>
	protected void Update() {
		if (cheats) { _wasCheats = true; }

		// If window is null, we've probably had a script recompile. Or the first Update happened
		// before the first OnGUI. Let's reload all the things.
		if (window == null) {
			SetUpInitialData();
			InstantiateWindowObject();
			LoadConfigFile();
		}
		
		//Move inputs to the next frame.
		ControlStates.NextFrame();


		if (!window.open) {
			foreach (KeyValuePair<KeyCode, string> pair in binds) {
				if (Input.GetKeyDown(pair.Key)) {
					Execute(pair.Value);
				}
				if (Input.GetKeyUp(pair.Key)) {
					string[] cmds = pair.Value.SplitUnlessInContainer(';', '\"', StringSplitOptions.RemoveEmptyEntries);
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
						if (Input.GetAxisRaw(axis) <= 0) {
							Execute(pair.Value.Replace("%value%", (-Input.GetAxisRaw(axis)).ToString()).Replace("%nvalue%", (Input.GetAxisRaw(axis)).ToString()));
						} else if (!axisMappings.ContainsKey(pair.Key.Substring(0, pair.Key.Length - 1) + "+")) {
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
						if (Input.GetAxisRaw(axis) >= 0) {
							Execute(pair.Value.Replace("%value%", Input.GetAxisRaw(axis).ToString()).Replace("%nvalue%", (-Input.GetAxisRaw(axis)).ToString()));
						} else if (!axisMappings.ContainsKey(pair.Key.Substring(0, pair.Key.Length - 1) + "-")) {
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
			Cursor.visible = true;
		}

	}

	/// <summary>Run when the application exits.</summary>
	protected void OnApplicationQuit() {
		SaveConfigFile();

	}

	/// <summary>Renders the console window if the console is showing.</summary>
	protected void OnGUI() {
		if (window == null) {
			SetUpInitialData();
			InstantiateWindowObject();
			LoadConfigFile();
		}

		if (window.open) {
#if UNITY_EDITOR || UNITY_STANDALONE_WIN || UNITY_STANDALONE_MAC
			GUI.depth = -2000000000;
			window.Draw();
#endif
		} else {
			//ConsoleWindow.focusTheTextField = true;
		}

	}

	/// <summary>
	/// Executes a collection of commands one-by-one in the console.
	/// </summary>
	/// <param name="lines">The lines to execute.</param>
	public static void Execute(IEnumerable<string> lines) {
		foreach (string line in lines) {
			Execute(line);
		}
	}

	/// <summary>
	/// Takes a line and attempts to turn it into commands which will be executed, through reflection.
	/// </summary>
	/// <param name="line">Commands to execute.</param>
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
			if (debug >= 3) { Echo(line); }
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
			if (IsBlacklisted(command)) {
#if !DEVELOPMENT_BUILD && !UNITY_EDITOR
				Echo("Unknown command: "+command);
				return;
#else
				Echo("Command " + command + " is blacklisted and cannot be accessed normally!");
#endif
			}
			string targetClassName = null;
			string targetMemberName = null;
			// Separate class specification from member call
			int indexOfDot = command.LastIndexOf('.');
			Type targetClass = null;
			if (indexOfDot > 0) {
				targetClassName = command.Substring(0, indexOfDot);
				targetMemberName = command.Substring(indexOfDot+1);
				targetClass = ReflectionUtils.GetTypeInUnityAssemblies(targetClassName);
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
#if DEVELOPMENT_BUILD || UNITY_EDITOR
					throw e;
#endif
				}
			} else {
				Echo("Unknown command: "+command);
			}
		}

	}

	/// <summary>
	/// Attempt to locate the member as a field of the specified class, and deal with it based on the given parameters.
	/// </summary>
	/// <param name="targetClass">The Type reference of the target class.</param>
	/// <param name="varName">The field name to find.</param>
	/// <param name="parameters">String containing the value to set the field to. The current value will be echoed if this is null or empty.</param>
	/// <returns>Boolean indicating whether the command was handled here.</returns>
	public static bool CallField(Type targetClass, string varName, string parameters = null) {
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

	/// <summary>
	/// Get the current value of the specified field owned by instance. If instance is null then field is static.
	/// </summary>
	/// <param name="instance">The instance of the object to get the value of <paramref name="fieldInfo"/> from. If <c>null</c> then it's a static field.</param>
	/// <param name="fieldInfo">The reflected <c>FieldInfo</c> to get the value of.</param>
	/// <returns>The value of the field parsed into a string.</returns>
	public static string GetFieldValue(object instance, FieldInfo fieldInfo) {
		if (fieldInfo == null) { return null; }
		return ParseObjectIntoString(fieldInfo.GetValue(instance));
	}

	/// <summary>
	/// Set the current value of the specified field owned by instance. If instance is null then field is static.
	/// </summary>
	/// <param name="instance">The instance of the object to set the value of <paramref name="fieldInfo"/> from. If <c>null</c> then it's a static field.</param>
	/// <param name="fieldInfo">The reflected <c>FieldInfo</c> to set the value of.</param>
	/// <param name="parameters">An array of strings to parse into a value to use for the field.</param>
	/// <returns>Boolean value indicating whether the field was set successfully.</returns>
	public static bool SetFieldValue(object instance, FieldInfo fieldInfo, string[] parameters) {
		object result = parameters.ParseParameterListIntoType(fieldInfo.FieldType.Name);
		if (result != null) {
			fieldInfo.SetValue(instance, result);
			return true;
		} else {
			return false; // Fail: The parameters could not be parsed into the desired type
		}
	}

	/// <summary>
	/// Attempt to locate the member as a property of the specified class, and deal with it based on the given parameters.
	/// </summary>
	/// <param name="targetClass">The Type reference of the target class.</param>
	/// <param name="propertyName">The property name to find.</param>
	/// <param name="parameters">String containing the value to set the property to. The current value will be echoed if this is null or empty.</param>
	/// <returns>Boolean indicating whether the command was handled here.</returns>
	public static bool CallProperty(Type targetClass, string propertyName, string parameters = null) {
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

	/// <summary>
	/// Get the current value of the specified property owned by <paramref name="instance"/>. If <paramref name="instance"/> is null then property is static.
	/// </summary>
	/// <param name="instance">The instance of the object to get the value of <paramref name="propertyInfo"/> from. If <c>null</c> then it's a static property.</param>
	/// <param name="propertyInfo">The reflected <c>PropertyInfo</c> to get the value of.</param>
	/// <returns>The value of the property parsed into a string, or "write-only" if property is write-only.</returns>
	public static string GetPropertyValue(object instance, PropertyInfo propertyInfo) {
		if (propertyInfo == null) { return null; }
		if (propertyInfo.GetGetMethod() == null) { return "write-only!"; }
		return ParseObjectIntoString(propertyInfo.GetGetMethod().Invoke(instance, new object[] { }));
	}

	/// <summary>
	/// Set the current value of the specified property owned by instance. If instance is null then property is static.
	/// </summary>
	/// <param name="instance">The instance of the object to set the value of <paramref name="propertyInfo"/> from. If <c>null</c> then it's a static property.</param>
	/// <param name="propertyInfo">The reflected <c>PropertyInfo</c> to set the value of.</param>
	/// <param name="parameters">An array of strings to parse into a value to use for the property.</param>
	/// <returns>Boolean value indicating whether the property was set successfully. Will return <c>true</c> if property is read-only.</returns>
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

	/// <summary>
	/// Attempts to find a method named <paramref name="methodName"/> matching the parameters given. If none is found, try to pass the entire string to a method 
	/// named <paramref name="methodName"/>. If neither of those things work, or no parameters are given, try to call a parameterless version of a method named 
	/// <paramref name="methodName"/>. If none is found, either no method has an overload matching the parameters given, or no method named <paramref name="methodName"/> exists.
	/// </summary>
	/// <param name="targetClass">The Type reference of the target class.</param>
	/// <param name="methodName">The name of the method to find.</param>
	/// <param name="parameters">String containing parameters to pass to the method.</param>
	/// <returns>Boolean value indicating whether a method with the name <paramref name="methodName"/> was found, regardless of if it was called or not.</returns>
	public static bool CallMethod(Type targetClass, string methodName, string parameters) {
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
			MethodInfo targetMethod = targetClass.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.FlattenHierarchy, null, new Type[] { typeof(string) }, null);
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
				MethodInfo targetInstancedMethod = targetClass.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.FlattenHierarchy, null, new Type[] { typeof(string) }, null);
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
		MethodInfo targetParameterlessMethod = targetClass.GetMethod(methodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.FlattenHierarchy, null, new Type[] { }, null);
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
			MethodInfo targetInstancedParameterlessMethod = targetClass.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.InvokeMethod | BindingFlags.FlattenHierarchy, null, new Type[] { }, null);
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

	/// <summary>
	/// Given a method with name <paramref name="methodName"/> and an array of <c>MethodInfo</c> objects, try to match the name and
	/// parameter list provided and invoke the method on the given <paramref name="targetObject"\>, or invokes it statically if <paramref name="targetObject"/> is <c>null</c>.
	/// </summary>
	/// <param name="targetClass">The Type reference of the target class.</param>
	/// <param name="methodName">The name of the method to find.</param>
	/// <param name="parameters">String containing parameters to pass to the method.</param>
	/// <returns>Boolean value indicating whether a suitable method was found and invoked.</returns>
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

	// 
	// Echoes nothing if the method is void.
	/// <summary>
	/// Invokes the target method on the target object using the parameters supplied, and echoes a nice string representation of the result to the console,
	/// or does nothing with the return value if it returns <c>void</c>.
	/// </summary>
	/// <param name="targetMethod">The reflected <c>MethodInfo</c> of the method to invoke.</param>
	/// <param name="targetObject">The <c>object</c> to invoke <paramref name="targetMethod"/> on, or <c>null</c> if <paramref name="targetMethod"/> is static.</param>
	/// <param name="parameters">Parameters to pass into <paramref name="targetMethod"/>.</param>
	public static void InvokeAndEchoResult(MethodInfo targetMethod, object targetObject, object[] parameters) {
		if (targetMethod.ReturnType == typeof(void)) {
			targetMethod.Invoke(targetObject, parameters);
		} else {
			Echo(ParseObjectIntoString(targetMethod.Invoke(targetObject, parameters)));
		}

	}

	/// <summary>
	/// Gets a string representation of <paramref name="obj"/>. If <paramref name="obj"/> is an <c>ICollection</c>, also gets the ToString of each element.
	/// </summary>
	/// <param name="obj"><c>object</c> to parse into a <c>string</c>.</param>
	/// <returns><c>string</c> representation of <paramref name="obj"/>.</returns>
	public static string ParseObjectIntoString(object obj) {
		//Type type = obj.GetType();
		if (obj is ICollection) {
			StringBuilder sb = new StringBuilder();
			sb.Append(obj.ToString())
			.Append(" {\n");
			foreach (object o in (ICollection)obj) {
				sb.Append("\t")
				.Append(o.ToString())
				.Append("\n");
			}
			sb.Append("}");
			return sb.ToString();
		}
		return obj.ToString();
	}

	/// <summary>
	/// Tries to find a static reference to a singleton instance of a given class. The instance should be named "main", "instance" or "singleton".
	/// </summary>
	/// <param name="targetClass">The class to get the singleton of.</param>
	/// <returns>The singleton instance of <paramref name="targetClass"/>, or <c>null</c>.</returns>
	public static object GetMainOfClass(Type targetClass) {
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

	/// <summary>
	/// Gets whether the given <paramref name="member"/> does not have an <see cref="Inaccessible"/> attribute applied to it.
	/// </summary>
	/// <param name="member">The <c>MemberInfo</c> to check.</param>
	/// <returns><c>true</c> if <paramref name="member"/> does NOT have an <see cref="Inaccessible"/> attribute.</returns>
	public static bool IsAccessible(MemberInfo member) {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
		if (Attribute.GetCustomAttribute(member, typeof(InaccessibleAttribute)) != null || Attribute.GetCustomAttribute(member.DeclaringType, typeof(InaccessibleAttribute)) != null) {
			Echo("Member "+member.Name+" is marked inaccessible and cannot be accessed normally!");
		}
		return true;
#else
		return Attribute.GetCustomAttribute(member, typeof(InaccessibleAttribute)) == null || Attribute.GetCustomAttribute(member.DeclaringType, typeof(InaccessibleAttribute)) != null;
#endif
	}

	/// <summary>
	/// Gets whether the given <paramref name="member"/> has a <see cref="Cheat"/> attribute applied to it.
	/// </summary>
	/// <param name="member">The <c>MemberInfo</c> to check.</param>
	/// <returns><c>true</c> if <paramref name="member"/> has a <see cref="Cheat"/> attribute.</returns>
	public static bool IsCheat(MemberInfo member) {
#if DEVELOPMENT_BUILD
		if (Attribute.GetCustomAttribute(member, typeof(CheatAttribute)) != null || Attribute.GetCustomAttribute(member.DeclaringType, typeof(CheatAttribute)) != null) {
			Echo("Member "+member.Name+" is marked a cheat and cannot be accessed normally without cheats!");
		}
		return false;
#else
		return Attribute.GetCustomAttribute(member, typeof(CheatAttribute)) != null || Attribute.GetCustomAttribute(member.DeclaringType, typeof(CheatAttribute)) != null;
#endif
	}

	/// <summary>
	/// Determines if the given <paramref name="command"/> is blacklisted through the values set in the inspector.
	/// </summary>
	/// <param name="command">Command to check.</param>
	/// <returns>Boolean value indicating whether this command is blacklisted.</returns>
	public static bool IsBlacklisted(string command) {
		foreach (string cls in classBlacklist) {
			if ((command == cls) || (command.StartsWith(cls) && command[cls.Length] == '.')) {
				return true;
			}
		}
		return false;
	}

	/// <summary>
	/// Echoes a "This command is a cheat" for <paramref name="memberName"/>.
	/// </summary>
	/// <param name="memberName">Member to print cheat message for.</param>
	[Inaccessible] public static void PrintCheatMessage(string memberName) {
		Echo(memberName + " is a cheat command. Set \"cheats\" to 1 to use it.");
	}

#if UNITY_EDITOR || DEVELOPMENT_BUILD
	/// <summary>
	/// Toggles the console open or closed.
	/// </summary>
	public static void ToggleConsole() {
		window.open = !window.open;
		window.textField = "";
		if (window.open) {
			foreach (KeyValuePair<KeyCode, string> bind in binds) {
				string[] cmds = bind.Value.SplitUnlessInContainer(';', '\"', StringSplitOptions.RemoveEmptyEntries);
				foreach (string cmd in cmds) {
					if (cmd[0] == '+') {
						Execute('-' + cmd.Substring(1));
					}
				}
			}
			foreach (KeyValuePair<string, string> mapping in axisMappings) {
				if (mapping.Value.Contains("%value%")) {
					Execute(mapping.Value.Replace("%value%", "0"));
				}
				string[] cmds = mapping.Value.SplitUnlessInContainer(';', '\"', StringSplitOptions.RemoveEmptyEntries);
				foreach (string cmd in cmds) {
					if (cmd[0] == '+') {
						Execute('-' + cmd.Substring(1));
					}
				}
			}
		}

	}
#endif

	/// <summary>
	/// Echoes an empty line to the console.
	/// </summary>
	public static void Echo() {
		Echo("");

	}

	/// <summary>
	/// Echoes the passed string to the console.
	/// </summary>
	/// <param name="st">The string to echo.</param>
	public static void Echo(string st) {
		/*consoleText += "\n" + st.ParseNewlines();
		consoleScrollPos = new Vector2(0, heightOfGUIContent);*/
		window.textWindow += "\n" + st.ParseNewlines();

	}

	/// <summary>
	/// Clears all text in the console.
	/// </summary>
	public static void Clear() {
		window.textWindow = "";
		/*consoleText = "";
		heightOfGUIContent = 0.0f;
		consoleScrollPos = Vector2.zero;*/

	}

	/// <summary>
	/// Creates + and - aliases for <paramref name="thing"/> which sets <paramref name="location"/> to "true" or "false", respectively.
	/// </summary>
	/// <param name="thing">The name of the alias to make.</param>
	/// <param name="location">The command to run, passing "true" or "false".</param>
	public static void AliasButton(string thing, string location) {
		Alias("+" + thing, location + " true");
		Alias("-" + thing, location + " false");
	}

	/// <summary>
	/// Creates + and - aliases for <paramref name="thing"/> which sets <paramref name="location"/> to "true" or "false", respectively, and binds it to <paramref name="key"/>.
	/// </summary>
	/// <param name="key">Key to bind.</param>
	/// <param name="thing">The name of the alias to make.</param>
	/// <param name="location">The command to run, passing "true" or "false".</param>
	public static void BindButton(string key, string thing, string location) {
		AliasButton(thing, location);
		Bind(key, "+" + thing);
	}

	/// <summary>
	/// Parses <paramref name="st"/> into an alias. If <paramref name="st"/> is empty, runs <see cref="Alias"/>. If it has one
	/// parameter, echoes what the alias with that parameter as its name does. If it has two parameters, aliases the first parameter
	/// to execute the rest of the string.
	/// </summary>
	/// <param name="st">Parameters.</param>
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
					StringBuilder rest = new StringBuilder();
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

	/// <summary>
	/// Prints a helpful message about how to use the Alias command.
	/// </summary>
	public static void Alias() {
		Echo("Alias: Allows multiple commands to be executed using one command.\nUsage: Alias <name> \"command1 \'[param1\' \'[params...]\'[;][commands...]\"");

	}

	/// <summary>
	/// Adds an alias <paramref name="name"/> that will execute <paramref name="cmds"/>.
	/// </summary>
	/// <param name="name">Name of the new alias.</param>
	/// <param name="cmds">Commands this alias will run.</param>
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

	/// <summary>
	/// Prints a helpful message on how to use the Unalias command.
	/// </summary>
	public static void Unalias() {
		Echo("Unalias: Deletes an alias.\nUsage: Unalias <name>");

	}

	/// <summary>
	/// Removes the alias named <paramref name="st"/> from the aliases.
	/// </summary>
	/// <param name="st">The name of the alias to remove.</param>
	public static void Unalias(string st) {
		string unaliasMe = st.SplitUnlessInContainer(' ', '\"')[0];
		if (aliases.ContainsKey(unaliasMe)) {
			aliases.Remove(unaliasMe);
		}

	}

	/// <summary>
	/// Parses <paramref name="st"/> into a binding. If st has only no parameters, prints a helpful message on how to use Bind.
	/// If it has one parameter, prints what the key with that name does. If it has two or more, sets the key (first parameter)
	/// to execute the rest of the string as commands.
	/// </summary>
	/// <param name="st">String to parse.</param>
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
					targetKeyCode = (KeyCode)Enum.Parse(typeof(KeyCode), parameters[0], true);
				} catch (ArgumentException) {
					if (axisMappings.ContainsKey(parameters[0])) {
						Echo(parameters[0] + " is " + axisMappings[parameters[0]]);
					} else {
						try {
							string tryme = parameters[0];
							if (tryme[tryme.Length - 1] == '+' || tryme[tryme.Length - 1] == '-') {
								tryme = tryme.Substring(0, tryme.Length - 1);
							}
							Input.GetAxisRaw(tryme);
							Echo(parameters[0] + " is unbound");
						} catch(ArgumentException) {
							Echo(parameters[0] + " is not a valid KeyCode or axis!");
						}
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
					StringBuilder rest = new StringBuilder();
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

	/// <summary>
	/// Prints a helpful message on how to use Bind.
	/// </summary>
	public static void Bind() {
		Echo("Bind: Allows binding of commands to keypresses or axes.\nUsage: Bind <KeyCode> \"command1 \'[param1\' \'[params...]\'[;][commands...]\"");
		Echo("When binding axes, any instance of %value% will be replaced with the current position of the axis. %nvalue% gives the inverted value.");
		Echo("Additionally, you may use only half an axis by appending a \"+\" or \"-\" to the end of the axis name.");

	}

	/// <summary>
	/// Binds the key named <paramref name="name"/> to the commands <paramref name="cmds"/>.
	/// </summary>
	/// <param name="name">Name of the key to bind.</param>
	/// <param name="cmds">Commands to run when key <paramref name="name"/> is pressed.</param>
	public static void Bind(string name, string cmds) {
		KeyCode targetKeyCode;
		try {
			targetKeyCode = (KeyCode)Enum.Parse(typeof(KeyCode), name, true); // Will throw exception if KeyCode doesn't exist
			Bind(targetKeyCode, cmds);
		} catch (ArgumentException) {
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
			} catch (ArgumentException) {
				Echo(name + " is not a valid KeyCode or axis!");
				return;
			}
		}

	}

	/// <summary>
	/// Binds the key <paramref name="name"/> to the commands <paramref name="cmds"/>.
	/// </summary>
	/// <param name="name">Key to bind.</param>
	/// <param name="cmds">Commands to run when key <paramref name="name"/> is pressed.</param>
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

	/// <summary>
	/// Prints a helpful message on how to use the Poll command.
	/// </summary>
	public static void Poll() {
		Echo("Poll: Displays the current state of a given KeyCode or axis.\nUsage: Poll <KeyCode>");

	}

	/// <summary>
	/// Gets the current value of a given key or axis and prints it to the console.
	/// </summary>
	/// <param name="surface">Name of the key or axis to get the value of.</param>
	public static void Poll(string surface) {
		KeyCode pollMe;
		string name = surface.SplitUnlessInContainer(' ', '\"')[0];
		try {
			pollMe = (KeyCode)Enum.Parse(typeof(KeyCode), name, true);
		} catch (ArgumentException) {
			try {
				Echo(name + " is " + Input.GetAxisRaw(name));
			} catch (ArgumentException) {
				Echo(name + " is not a valid KeyCode or axis!");
			}
			return;
		}
		Echo(name + " is " + Input.GetKey(pollMe));

	}

	/// <summary>
	/// Prints a helpful message on how to use the Unbind command.
	/// </summary>
	public static void Unbind() {
		Echo("Unbind: Unbinds all commands from a key or axis.\nUsage: Unbind <KeyCode>");

	}

	/// <summary>
	/// Removes the binding for the key <paramref name="st"/> from the bindings.
	/// </summary>
	/// <param name="st">Key to remove from bindings.</param>
	public static void Unbind(string st) {
		KeyCode unbindMe;
		string name = st.SplitUnlessInContainer(' ', '\"')[0];
		try {
			unbindMe = (KeyCode)Enum.Parse(typeof(KeyCode), name, true);
			Unbind(unbindMe);
		} catch (ArgumentException) {
			try {
				if (name[name.Length - 1] == '-' || name[name.Length - 1] == '+') {
					Input.GetAxisRaw(name.Substring(0, name.Length - 1)); // Will throw exception if axis doesn't exist
				} else {
					Input.GetAxisRaw(name); // Will throw exception if axis doesn't exist
				}
				if (axisMappings.ContainsKey(name)) {
					axisMappings.Remove(name);
				} else {
					Echo(name + " is not bound.");
				}
			} catch (ArgumentException) {
				Echo(name + " is not a valid KeyCode or axis!");
			}
		}
	}

	/// <summary>
	/// Removes the binding for the key <paramref name="key"/> from the bindings.
	/// </summary>
	/// <param name="key">Key to remove from bindings.</param>
	public static void Unbind(KeyCode key) {
		if (binds.ContainsKey(key)) {
			binds.Remove(key);
		} else {
			Echo(key + " is not bound.");
		}
	}

	/// <summary>
	/// Unbinds all keys and removes all axis mappings.
	/// </summary>
	public static void UnbindAll() {
		binds = new Dictionary<KeyCode, string>();
		axisMappings = new Dictionary<string, string>();
	}

	/// <summary>
	/// Calls <see cref="Execute"/> on the contents of the file at <paramref name="path"/> line-by-line.
	/// </summary>
	/// <param name="path">Path of the file to execute its contents.</param>
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
					Echo("Unable to find script file to execute " + path);
					return;
				}
			}
		}
		while (!sr.EndOfStream) {
			Execute(sr.ReadLine());
		}
		sr.Close();

	}

	#region ILogHandler
	/// <summary>
	/// Logs an exception into the console and calls the <see cref="OnException"/> callback.
	/// </summary>
	/// <param name="exception">The exception that is being logged.</param>
	/// <param name="context">The Unity Object which threw the exception.</param>
	public void LogException(Exception exception, UnityEngine.Object context) {
		if (OnException != null) {
			OnException(exception, context);
		}
		Debug.logger.LogException(exception, context);
	}

	/// <summary>
	/// Logs something to the console.
	/// </summary>
	/// <param name="logType">The type of the log.</param>
	/// <param name="context">The Unity Object which made the log call.</param>
	/// <param name="format">The string to log.</param>
	/// <param name="args">The arguments to insert into the <paramref name="format"/> string.</param>
	public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args) {
		// This will ultimately make it back into the console through LogCallback
		Debug.logger.logHandler.LogFormat(logType, context, format, args);
	}
	#endregion

	/// <summary>
	/// Callback which is hooked into Unity's debuglogger. Runs any time Unity outputs something to the debug logger.
	/// </summary>
	/// <param name="logString">The string that was logged.</param>
	/// <param name="stackTrace">The stack trace causing the log.</param>
	/// <param name="type">The type of log. Depending on this value and the value of <see cref="debug"/> it will be echoed into the console.</param>
	private void LogCallback(string logString, string stackTrace, LogType type) {
		if (type == LogType.Exception) {
			Echo(logString + "\n" + stackTrace);
			return;
		}
		if (type == LogType.Warning && debug >= 1) { Echo("Warning: " + logString); return; }
		if ((type == LogType.Error || type == LogType.Assert) && debug >= 1) { Echo("Error: " + logString); return; }
		if (debug >= 2) { Echo(logString); return; }
	}

	/// <summary>
	/// Saves the current aliases, binds and axis mappings to a config file to be executed when the game starts.
	/// </summary>
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

	/// <summary>
	/// Loads the file at <see cref="configPath"/> and runs <see cref="Execute"/> on its contents, line-by-line.
	/// </summary>
	public static void LoadConfigFile() {
		if (File.Exists(configPath)) {
			Execute(persistent.Split('\n'));
			// If config exists, clear binds after loading persistent file (we want the default aliases but not the binds)
			binds = new Dictionary<KeyCode, string>();
			axisMappings = new Dictionary<string, string>();
#if UNITY_EDITOR
			binds.Add(KeyCode.BackQuote, "ToggleConsole");
#endif
			// All preexisting keybinds will be reloaded from this file instead
			Exec(configPath);
			if (File.Exists(autoexecPath)) {
				Exec(autoexecPath);
			}
		}
	}

	/// <summary>
	/// Creates a new Window to use as the interface for the console.
	/// </summary>
	private void InstantiateWindowObject() {
		window = (ConsoleWindow)new ConsoleWindow()
			.Named("Developer Console")
			.Resizable()
			.Closed()
			.Area(Screen.all.MiddleCenter(0.7f, 0.8f).Move(0.1f, 0.0f));
		window.textWindow = initialText.ParseNewlines();
		window.depth = -2000000000;
	}

	/// <summary>
	/// Initializes the instance values to good defaults. These may be overwritten when the config file is loaded.
	/// </summary>
	public void SetUpInitialData() {
		autoexecPath = Application.persistentDataPath + "/autoexec.cfg";
		classBlacklist = blacklist.ToList<string>();
		if (!classBlacklist.Contains("InAppPurchases")) { classBlacklist.Add("InAppPurchases"); }
		if (!classBlacklist.Contains("AdManager")) { classBlacklist.Add("AdManager"); }
	}

	/// <summary>
	/// Exits the game immediately.
	/// </summary>
	public static void Quit() {
#if UNITY_EDITOR
		EditorApplication.isPlaying = false;
#else
		Application.Quit();
#endif

	}

	/// <summary> Get a list of ALL keys bound to a given command. </summary>
	/// <param name="command">Command to look up.</param>
	/// <returns>List containing all <c>KeyCode</c>s bound to <paramref name="command"/>.</returns>
	public static List<KeyCode> GetKeysByCommand(string command) {
		List<KeyCode> ret = new List<KeyCode>();
		foreach (KeyValuePair<KeyCode, string> kvp in binds) {
			if (command == kvp.Value) {
				ret.Add(kvp.Key);
			}
		}
		return ret;
	}

	/// <summary> Returns the primary bound thing for the given command. </summary>
	/// <param name="command">Command name to look up (as a button).</param>
	/// <param name="alternate">Alternate command to look up (as an axis).</param>
	/// <returns>String representation of the key bound to command.</returns>
	public static string GetBindForCommand(string command, string alternate = null) {
		string bind = "NOT BOUND";
		if (alternate == null) { alternate = command; }

		var keys = GetKeysByCommand(command);
		if (keys.Count > 0) {
			bind = Joysticks.GetControlName(keys[0].ToString());
		} else {
			var axis = GetAxesByCommand(alternate);
			if (axis.Count > 0) {
				bind = Joysticks.GetControlName(axis[0]);
			}
		}

		return bind;
	}

	/// <summary> Get a list of ALL axes mapped to a given command. </summary>
	/// <param name="command">Command to look up.</param>
	/// <returns>List containing all axes bound to <paramref name="command"/>.</returns>
	public static List<string> GetAxesByCommand(string command) {
		List<string> ret = new List<string>();
		foreach (KeyValuePair<string, string> kvp in axisMappings) {
			if (command == kvp.Value) {
				ret.Add(kvp.Key);
			}
		}
		return ret;
	}

	/// <summary>
	/// Determines if a key is bound to a command.
	/// </summary>
	/// <param name="key">The key to test.</param>
	/// <param name="command">The command to check for.</param>
	/// <returns><c>true</c> if <paramref name="key"/> is bound to <paramref name="command"/>.</returns>
	public static bool IsBoundTo(KeyCode key, string command) {
		return binds.ContainsKey(key) && binds[key] == command;
	}

	/// <summary>
	/// Determines if an axis is mapped to a command.
	/// </summary>
	/// <param name="axis">The axis to test.</param>
	/// <param name="command">The command to check for.</param>
	/// <returns><c>true</c> if <paramref name="axis"/> is mapped to <paramref name="command"/>.</returns>
	public static bool IsBoundTo(string axis, string command) {
		return axisMappings.ContainsKey(axis) && axisMappings[axis] == command;
	}

	/// <summary>
	/// Custom attribute indicating that this member should be treated as a "cheat" by the console. Anything marked 
	/// "cheat" will not be allowed to run unless <see cref="cheats"/> is true.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Struct)]
	public class CheatAttribute : Attribute {
		public CheatAttribute() { }
	}

	/// <summary>
	/// Custom attribute indicating that this member should never be accessible from the console.
	/// </summary>
	[AttributeUsage(AttributeTargets.Class | AttributeTargets.Field | AttributeTargets.Method | AttributeTargets.Property | AttributeTargets.Struct)]
	public class InaccessibleAttribute : Attribute {
		public InaccessibleAttribute() { }
	}
}

/// <summary>
/// Class providing the GUI for the <see cref="DevConsole"/> class.
/// </summary>
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
		} catch (Exception e) {
			Debug.LogError("Internal error executing console command:\n" + e);
		}
		previousCommands.Add(cmd);
		cmdIndex = previousCommands.Count;
		focusTheTextField = true;
		scrollPos = new Vector2(0, 99999);
	}

}
