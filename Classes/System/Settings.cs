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

	public static RenderingPath renderPath = RenderingPath.DeferredShading;
	public static AAMode fxaaMode = AAMode.DLAA;
	
	public static bool loaded = DoLoad();
	static bool DoLoad() {
		callbacks = new Dictionary<string,Action<string>>();
		RegisterDefaultCallbacks();
		return true;
	}
	
	private static Dictionary<string, Action<string>> callbacks;
	public static void Register(string setting, Action<string> callback) { callbacks[setting] = callback; }


	public static void Save(string key = "settings") {
		string json = instance.ToString();
		
		PlayerPrefs.SetString(key, json);
	}

	public static void Load(string key = "settings") {
		TextAsset defaultSettingsFile = Resources.Load<TextAsset>("defaultSettings");
		string json = defaultSettingsFile != null ? defaultSettingsFile.text : "";
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

	public Settings() : base() {
		qualityLevel = 3;
		musicVolume = 1;
		soundVolume = 1;
		overscanRatio = 0;
		
		userName = "New User";
		color = Color.red;
		sensitivity = 5;
	}

	public void Apply(string thing, object value) {
		JsonValue val = Json.Reflect(value);
		this[thing] = val;
		//Debug.Log("Set " + thing + " to " + val);
		
		string strValue = value.ToString();
		if (val.isString) { strValue = val.stringVal; }

		Callback(thing, strValue); 
	}
	private void Callback(string thing, string value) {
		if (callbacks.ContainsKey(thing)) {
			var callback = callbacks[thing];
			if (callback != null) {
				callback(value);
				//Debug.Log("Callback'd " + thing + " " + value);
			}
		}
	}

	public int qualityLevel { get { return this.Get<int>("qualityLevel"); } set { Apply("qualityLevel", value); } }

	public float musicVolume { get { return this.Get<float>("musicVolume"); } set { Apply("musicVolume", value); } }
	public float soundVolume { get { return this.Get<float>("soundVolume"); } set { Apply("soundVolume", value); } }
	public float overscanRatio { get { return this.Get<float>("overscanRatio"); } set { Apply("overscanRatio", value); } }
	public float sensitivity { get { return this.Get<float>("sensitivity"); } set { Apply("sensitivity", value); } }

	public string userName { get { return this.Get<string>("userName"); } set { Apply("userName", value); } }

	public Color color { get { return this.Get<Color>("color"); } set { Apply("color", value); } }


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
