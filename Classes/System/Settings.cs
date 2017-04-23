using UnityEngine;
using UnityStandardAssets.ImageEffects;
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
#if UNITY_XBOXONE
using Users;
using Storage;
#endif

/// <summary>
/// This class holds settings information for things.
/// </summary>
public partial class Settings {
	public static Settings instance {
		get {
			if (_instance == null) {
				_instance = new Settings();
			}
			return _instance;
		}
		set {
			_instance = value;
		}
	}
	private static Settings _instance;

}

#if XtoJSON
public partial class Settings : JsonObject {



	/// <summary> Select an option from parrallel arrays. </summary>
	/// <typeparam name="T"> Type of output </typeparam>
	/// <param name="choice">string to look for index of in choicesIn</param>
	/// <param name="choicesIn">array containing valid inputs for choice</param>
	/// <param name="choiceOut">array containing the outputs to choose from outputs </param>
	/// <returns> the output parallel to 'choice' in the two given arrays </returns>
	static T Select<T>(string choice, IEnumerable<string> choicesIn, IEnumerable<T> choiceOut) {
		int index = 0;
		foreach (string str in choicesIn) {
			if (str == choice) { break; }
			index++;
		}

		if (index < 0 || index >= choiceOut.Count()) { return default(T); }
		var t = choiceOut.ElementAt(index);
		return t;
		

	}

	/// <summary> Registers a bunch of callbacks that change settings inside of Unity, so that when the user changes settings, the change immediately happens. </summary>
	public static void RegisterDefaultCallbacks() {
		//Debug.Log("Default Callbacks Added!");
		
		/*
		//No Reason to have texture resolution actually, it completely breaks unity's rendering when switched at runtime in standalone...
		string[] texResStrs = { "FULL", "HALF", "QUARTER", "EIGHTH" };
		int[] texResInts = { 0, 1, 2, 3 };
		Register("Textures", (s) => {
			QualitySettings.masterTextureLimit = Select(s, texResStrs, texResInts);
		});
		*/

		string[] aaStrs = { "1x", "2x", "4x", "8x" };
		int[] aaInts = { 0, 2, 4, 8};
		Register("MSAA", (s) => {
			QualitySettings.antiAliasing = Select(s, aaStrs, aaInts);
		});

		//string[] offOn = { "OFF", "ON" };
		
		Register("AnisoFiltering", (s) => {
			QualitySettings.anisotropicFiltering = (s == "ON") ? AnisotropicFiltering.ForceEnable : AnisotropicFiltering.Disable;
		});

		Register("ForwardLights", (s) => {
			int i = 0;
			int.TryParse(s, out i);
			QualitySettings.pixelLightCount = i;
		});

		//Soft particles can't be changed... RIP.
		//They suck anyway, so nothing of value was lost...

		//ReflectionSize is pulled from 'AppliesReflectionProbeSettings'

		Register("RealtimeReflection", (s) => {
			QualitySettings.realtimeReflectionProbes = (s == "ON");
		});

		Register("Billboards", (s) => {
			QualitySettings.billboardsFaceCameraPosition = (s == "ON");
		});

		string[] camToggles = { "Bloom", "AmbientOcclusion", "DepthOfField", "Scanlines", "ScreenSpaceReflections" };
		foreach (string str in camToggles) {
			//Debug.Log("Registering " + str);
			string ztr = str;
			Register(ztr, (s) => {
				AppliesPostSettings.Set(ztr, s.Equals("ON"));
			});
		}

		//ShadowType, ShadowResolution are applied on cameras.

		string[] scasStrs = {"NONE", "TWO", "FOUR"};
		int[] scasInts = { 0, 2, 4};
		Register("ShadowCascades", (s) => {
			QualitySettings.shadowCascades = Select(s, scasStrs, scasInts);
		});

		string[] rpathStrs = { "DEFERRED", "FORWARD", "VERTEX" };
		RenderingPath[] rpaths = { RenderingPath.DeferredShading, RenderingPath.Forward, RenderingPath.VertexLit };
		Register("RenderingPath", (s) => {
			renderPath = Select(s, rpathStrs, rpaths);
		});

		
		string[] fxaaStrs = { "OFF", "V1A", "V1B", "V2", "V3" };
		AAMode[] fxaaModes = { AAMode.DLAA, AAMode.FXAA1PresetA, AAMode.FXAA1PresetB, AAMode.FXAA2, AAMode.FXAA3Console };
		Register("FXAA", (s) => {
			fxaaMode = Select(s, fxaaStrs, fxaaModes);
		});


		Register("borderless", (s)=> {
#if !UNITY_EDITOR
#if UNITY_STANDALONE_WIN
			//File.WriteAllText(dataPath+"/borderlessCallback.txt", s);
			/*
			if (s.ToLower() == "true") {
				WindowHandler.SetBorderless(true);
			} else {
				WindowHandler.SetBorderless(false);
			}
			//*/
			//Debug.Log("Borderless: " + s);
#elif UNITY_STANDALONE_LINUX

#endif

#endif
		});

		Register("language", (s) => {
			//Debug.Log("Callback: language set to " + s);
			Localization.language = (Language) Enum.Parse(typeof(Language), s);
			GSS.restyleEverything = true;
		});

#if UNITY_XBOXONE && !UNITY_EDITOR
		ConnectedStorageWrapper.OnSaveDataDidNotExist += OnSaveDataDidNotExist;
		ConnectedStorageWrapper.OnSaveDataRetrieved += OnSaveDataRetrieved;
#endif

	}

	/// <summary> Current active rendering path. </summary>
	public static RenderingPath renderPath = RenderingPath.DeferredShading;
	/// <summary> Current active fxAA mode </summary>
	public static AAMode fxaaMode = AAMode.DLAA;

	/// <summary> Load hax to avoid the static initializer </summary>
	public static bool loaded = DoLoad();
	/// <summary> Load method. </summary>
	public static bool DoLoad() {
		Debug.Log("Settings.DoLoad()");
		callbacks = new Dictionary<string,Action<string>>();
		RegisterDefaultCallbacks();
		Load();
		return true;
	}

	/// <summary> Has anything changed during the last frame? (Note: This needs to be manually set to false in LateUpdate!) </summary>
	public static bool changed = false;

	/// <summary> Dictionary of callbacks to do settings change. </summary>
	private static Dictionary<string, Action<string>> callbacks;
	/// <summary> Register a callback to a given setting </summary>
	public static void Register(string setting, Action<string> callback) { callbacks[setting] = callback; }

	public static string dataPath { get { return Application.dataPath.PreviousDirectory(); } }
	public static string savePath { get { return dataPath + "/save"; } } 

	/// <summary> Save settings to a json file inside </summary>
	public static void Save(string file = "settings") {
#if UNITY_XBOXONE && !UNITY_EDITOR
		// No user given; nowhere to save to!
		return;
#else
		string saveFile = savePath + "/" + file + ".json";
		string json = instance.ToString();
		
		if (!File.Exists(saveFile)) {
			if (!Directory.Exists(savePath)) { Directory.CreateDirectory(savePath); }
			
		}
		File.WriteAllText(saveFile, json);
		//PlayerPrefs.SetString(key, json);
#endif
	}

#if UNITY_XBOXONE
	/// <summary> Load settings from Connected Storage (or, default settings if none are saved) for the given user, and apply them. </summary>
	public static void Load(User user, string file = "settings") {
		if (user == null) {
			OnSaveDataDidNotExist(user, file);
			return;
		}
		if (!user.IsSignedIn || !StorageManager.AmFullyInitialized()) {
			return;
		}
#if !UNITY_EDITOR
		ConnectedStorageWrapper.LoadData(user, file);
#endif
	}

	/// <summary>
	/// Callback handler for connected storage data retrieved.
	/// </summary>
	/// <param name="user">User the data was retrieved for.</param>
	/// <param name="name">Name of the data container that was retrieved.</param>
	/// <param name="bytes">Data that was retrieved.</param>
	private static string[] xboxBlacklist = new string[] {
		"RenderingPath",
		"ForwardLights",
		"Textures",
		"Particles",
		"MSAA",
		"FXAA",
		"AmbientOcclusion",
		"AnisoFiltering",
		"ScreenSpaceReflections",
		"RealtimeReflection",
		"ReflectionSize",
		"ShadowType",
		"ShadowResolution",
		"ShadowCascades",
		"color",
		"resolution"
	};
	private static void OnSaveDataRetrieved(User user, string name, byte[] bytes) {
		if (name == "settings") {
			//Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
			TextAsset defaultSettingsFile = Resources.Load<TextAsset>("xboxDefaultSettings");
			string json = defaultSettingsFile != null ? defaultSettingsFile.text : "{}";
			JsonObject jobj = Json.Parse(json) as JsonObject;

			string prefs = bytes.GetString();
			JsonObject pobj = Json.Parse(prefs) as JsonObject;

			//Debug.Log("Settings jobj:\n" + jobj.ToString() + "\nSettings pobj:\n" + pobj.ToString());

			jobj.Set(pobj);
			
			instance = new Settings();
			//Debug.Log("Applying loaded settings. Merged jobj:\n" + jobj.ToString());
			foreach (var pair in jobj) {
				//try {
					if (xboxBlacklist.Contains(pair.Key.stringVal)) {
						//Debug.Log("Setting " + (string)pair.Key + ": " + (string)pair.Value + " is blacklisted.");
						continue;
					}
					//Debug.Log("Applying setting " + (string)pair.Key + ": " + (string)pair.Value + "");
					instance.Apply(pair.Key, pair.Value);
					//Debug.Log("Applied setting " + (string)pair.Key + ": " + (string)pair.Value + "");
				//} catch(Exception e) {
				//	Debug.Log("Setting " + (string)pair.Key + " is throwing an exception!\n+" + e.ToString());
				//}
			}
			//Debug.Log("Applied loaded settings");
		}
	}

	/// <summary>
	/// Callback handler for connected storage data did not exist.
	/// </summary>
	/// <param name="user">User the data was requested for.</param>
	/// <param name="name">Name of the data container that was requested.</param>
	private static void OnSaveDataDidNotExist(User user, string name) {
		if (name == "settings") {
			Debug.Log("Settings data did not exist, so it was created");
			TextAsset defaultSettingsFile = Resources.Load<TextAsset>("xboxDefaultSettings");
			string json = defaultSettingsFile != null ? defaultSettingsFile.text : "{}";
			JsonObject jobj = Json.Parse(json) as JsonObject;
			instance = new Settings();
			foreach (var pair in jobj) {
				instance.Apply(pair.Key, pair.Value);
			}
		}
	}

	public static void Save(User user, string file = "settings") {
		if (user == null || !user.IsSignedIn || !StorageManager.AmFullyInitialized()) {
			return;
		}
		string json = instance.ToString();
		Debug.Log("Saving settings to connected storage. Raw JSON:\n" + json);
#if !UNITY_EDITOR
		ConnectedStorageWrapper.SaveData(user, file, json.GetBytes());
#endif
	}
#endif

		/// <summary> Load settings from json (or, default settings if none are saved), and apply them. </summary>
	public static void Load(string file = "settings") {
		string defaultSettingsFilename = 
#if UNITY_XBOXONE && !UNITY_EDITOR
			"xboxDefaultSettings";
#else
			"defaultSettings";
#endif
		TextAsset defaultSettingsFile = Resources.Load<TextAsset>(defaultSettingsFilename);
		string json = defaultSettingsFile != null ? defaultSettingsFile.text : "{}";
		JsonObject jobj = Json.Parse(json) as JsonObject;
		// For Xbox One, we need more information, like what user to grab settings data for.
		// In the case where we aren't given a user (like this function) we may as well just
		// load the defaults.
#if !UNITY_XBOXONE || UNITY_EDITOR
		string saveFile = savePath + "/" + file + ".json";

		//if (PlayerPrefs.HasKey(key)) {
		if (File.Exists(saveFile)) {
			//string prefs = PlayerPrefs.GetString(key);
			string prefs = File.ReadAllText(saveFile);
			JsonObject pobj = Json.Parse(prefs) as JsonObject;
			//Debug.Log("Loaded Saved Settings");
			
			jobj.Set(pobj);
		}
#endif

		//Debug.Log("Applying Settings " + jobj.PrettyPrint());
		instance = new Settings();
		foreach (var pair in jobj) {
			instance.Apply(pair.Key, pair.Value);
		}
		//string s = instance.PrettyPrint();
		//Debug.Log("Settings.Load: Done- " + s);	
		//File.WriteAllText(dataPath +"/loadedSettings.json", s);
	}

	/// <summary> Directly set setting 'key' to 'value'. Does not call any callbacks. </summary>
	public static void Set(string key, float value) { instance[key] = value; }
	/// <summary> Directly set setting 'key' to 'value'. Does not call any callbacks. </summary>
	public static void Set(string key, string value) { instance[key] = value; }

	/// <summary> Default constructor </summary>
	public Settings() : base() {
		qualityLevel = 3;
		musicVolume = 1;
		soundVolume = 1;
		overscanRatio = 0;
		
		userName = "New User";
		color = Color.red;
		sensitivity = 5;
	}

	/// <summary> Set setting 'key' to 'value' and call the associated callback. </summary>
	public void Apply(string key, object value) {
		JsonValue val = (value is JsonValue) ? (JsonValue)value : Json.Reflect(value);
		this[key] = val;
		changed = true;

		string strValue = val.isString ? val.stringVal : val.ToString();
		Callback(key, strValue);
	}

	/// <summary> Call the callback for setting 'key' set to 'value' </summary>
	private void Callback(string key, string value) {
		if (callbacks.ContainsKey(key)) {
			var callback = callbacks[key];
			if (callback != null) {
				callback(value);
				//Debug.Log("Callback'd " + thing + " " + value);
			}
		}
	}

	/// <summary> General 'quality level' for use when more control over graphics settings is not given to the user </summary>
	public int qualityLevel { get { return this.Get<int>("qualityLevel"); } set { Apply("qualityLevel", value); } }

	/// <summary> Volume of Music. Range [0, 1] </summary>
	public float musicVolume { get { return this.Get<float>("musicVolume"); } set { Apply("musicVolume", value); } }
	/// <summary> Volume of Sound Effects. Range [0, 1] </summary>
	public float soundVolume { get { return this.Get<float>("soundVolume"); } set { Apply("soundVolume", value); } }
	/// <summary> Overscan for consoles. </summary>
	public float overscanRatio { get { return this.Get<float>("overscanRatio"); } set { Apply("overscanRatio", value); } }
	/// <summary> General mouse sensitivity </summary>
	public float sensitivity { get { return this.Get<float>("sensitivity"); } set { Apply("sensitivity", value); } }

	/// <summary> User/Display name in games where the user can set one manually. </summary>
	public string userName { get { return this.Get<string>("userName"); } set { Apply("userName", value); } }

	/// <summary> Primary ccolor of the user </summary>
	public Color color { get { return this.Get<Color>("color"); } set { Apply("color", value); } }

	/// <summary> Language the game is running in </summary>
	public string language { get { return this.Get<string>("language"); } set { Apply("language", value.ToString()); } }


	/// <summary> Helper function to query the settings object for a color, stored as a hex string, rather than an internal object</summary>
	public Color GetHexColor(string name) {
		return Colors.ToColorFromHex(this[name].stringVal);
	}
}
#else
public partial class Settings {


	public string language = "english";
	public int qualityLevel = 3;
	
	public float musicVolume = 1;
	public float soundVolume = 1;
	public float overscanRatio = 0;

	public string userName = "New User";
	public Color color = Color.red;
	public float sensitivity = 5;
}

#endif
