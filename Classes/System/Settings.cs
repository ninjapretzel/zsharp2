using UnityEngine;
using UnityStandardAssets.ImageEffects;
using System;
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// This class holds settings information for things.
/// </summary>
public partial class Settings {
	public static Settings instance = new Settings();

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
		string[] texResStrs = { "FULL", "HALF", "QUARTER", "EIGHTH" };
		int[] texResInts = { 0, 1, 2, 3 };
		Register("Textures", (s) => {
			QualitySettings.masterTextureLimit = Select(s, texResStrs, texResInts);
		});

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

	}

	/// <summary> Current active rendering path. </summary>
	public static RenderingPath renderPath = RenderingPath.DeferredShading;
	/// <summary> Current active fxAA mode </summary>
	public static AAMode fxaaMode = AAMode.DLAA;

	/// <summary> Load hax to avoid the static initializer </summary>
	public static bool loaded = DoLoad();
	/// <summary> Load method. </summary>
	static bool DoLoad() {
		callbacks = new Dictionary<string,Action<string>>();
		RegisterDefaultCallbacks();
		return true;
	}

	/// <summary> Dictionary of callbacks to do settings change. </summary>
	private static Dictionary<string, Action<string>> callbacks;
	/// <summary> Register a callback to a given setting </summary>
	public static void Register(string setting, Action<string> callback) { callbacks[setting] = callback; }

	/// <summary> Save settings to a JsonObject inside of PlayerPrefs </summary>
	public static void Save(string key = "settings") {
		string json = instance.ToString();
		
		PlayerPrefs.SetString(key, json);
	}

	/// <summary> Load settings from PlayerPrefs (or, default settings if none are saved), and apply them. </summary>
	public static void Load(string key = "settings") {
		TextAsset defaultSettingsFile = Resources.Load<TextAsset>("defaultSettings");
		string json = defaultSettingsFile != null ? defaultSettingsFile.text : "{}";
		JsonObject jobj = Json.Parse(json) as JsonObject;

		if (PlayerPrefs.HasKey(key)) {
			string prefs = PlayerPrefs.GetString(key);
			JsonObject pobj = Json.Parse(prefs) as JsonObject;
			Debug.Log("Loaded Saved Settings");
			jobj.Set(pobj);
		}

		//Debug.Log("Applying Settings " + jobj.PrettyPrint());
		instance = new Settings();
		foreach (var pair in jobj) {
			instance.Apply(pair.Key, pair.Value);
		}

		//Debug.Log("Done: " + instance.PrettyPrint());	
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
		JsonValue val = Json.Reflect(value);
		this[key] = val;
		//Debug.Log("Set " + thing + " to " + val);
		
		string strValue = value.ToString();
		if (val.isString) { strValue = val.stringVal; }

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


	/// <summary> Helper function to query the settings object for a color, stored as a hex string, rather than an internal object</summary>
	public Color GetHexColor(string name) {
		return Colors.ToColorFromHex(this[name].stringVal);
	}
}
#else 
public partial class Settings {

	public int qualityLevel = 3;
	
	public float musicVolume = 1;
	public float soundVolume = 1;
	public float overscanRatio = 0;

	public string userName = "New User";
	public Color color = Color.red;
	public float sensitivity = 5;
}

#endif
