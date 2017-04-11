#if UNITY_EDITOR && !UNITY_WEBPLAYER
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;

// This whole file is an abortion.
#pragma warning disable
public class QuickPlayerSettings : EditorWindow {
	
	public static string path { get { return Application.dataPath + "/Data/PlayerSettingsPresets/"; } }
	private static List<string> presets = new List<string>();
	private static int currentPreset = -1;
	private static bool changed = false;
	[NonSerialized] private static bool scriptLoaded = false;

	private static BuildTargetGroup currentPlatform = EditorUserBuildSettings.selectedBuildTargetGroup;
	private static BuildTargetGroup previousCurrentPlatform = EditorUserBuildSettings.selectedBuildTargetGroup;
	private static string saveAs = "Untitled Preset";
	
	[MenuItem ("Edit/Project Settings/Z# Quick Player Settings")]
	public static void ShowWindow() {
		QuickPlayerSettings main = (QuickPlayerSettings)EditorWindow.GetWindow(typeof(QuickPlayerSettings));
		main.minSize = new Vector2(400,0);
		main.autoRepaintOnSceneChange = true;
		UnityEngine.Object.DontDestroyOnLoad(main);
		main.Start();
		
	}
	
	public void Start() {
		if(!Directory.Exists(path)) { Directory.CreateDirectory(path); }
		OnScriptLoad();
		
	}

	public void OnGUI() {
		if(!scriptLoaded) { // Detected script recompile, Start() is not called when recompiled but this variable is reset
			OnScriptLoad();
		}

		EditorGUILayout.BeginHorizontal(); {
			EditorGUILayout.BeginVertical(); {
				EditorGUILayout.BeginVertical("box"); {
					if(presets.Count == 0) {
						GUILayout.Label("No saved presets!", GUILayout.Width(150));
					} else {
						GUILayout.Label("Saved presets", GUILayout.Width(150));
						for(int i = 0; i < presets.Count; i++) {
							if(GUILayout.Button(presets[i].Substring(presets[i].LastIndexOf("/") + 1, presets[i].Length - 5 - presets[i].LastIndexOf("/")), GUILayout.Width(150))) {
								LoadPreset(i);
								currentPreset = i;
								File.WriteAllText(Application.dataPath.Substring(0, Application.dataPath.Length - 7) + "/Library/LastPlayerSettingsPreset.tmp", presets[i].Substring(presets[i].LastIndexOf("/") + 1, presets[i].Length - 5 - presets[i].LastIndexOf("/")));
							}
						}
					}
				} EditorGUILayout.EndVertical();
				if(currentPreset >= 0) {
					if(changed) { GUI.color = Color.red; } else { GUI.color = Color.white; }
					if(GUILayout.Button("Save \"" + presets[currentPreset].Substring(presets[currentPreset].LastIndexOf("/") + 1, presets[currentPreset].Length - 5 - presets[currentPreset].LastIndexOf("/")) + "\"")) {
						SavePreset(currentPreset);
						changed = false;
					}
					GUI.color = Color.white;
				}
				EditorGUILayout.BeginHorizontal(); {
					if(GUILayout.Button("Save As")) {
						SavePresetAs(path + saveAs + ".csv");
						AssetDatabase.Refresh();
					}
					saveAs = EditorGUILayout.TextField(saveAs);
				} EditorGUILayout.EndHorizontal();
			} EditorGUILayout.EndVertical();
			EditorGUILayout.BeginVertical(); {
				if(GUILayout.Button("Open Player Settings")) {
					EditorApplication.ExecuteMenuItem("Edit/Project Settings/Player");
					//Debug.Log(typeof(PlayerSettings).Summary());
					changed = true;
				}
				EditorGUILayout.BeginVertical("box"); {
					GUILayout.Label("Additional settings", GUILayout.Width(position.width - 180));
					currentPlatform = (BuildTargetGroup)EditorGUILayout.EnumPopup("Platform", currentPlatform);
				} EditorGUILayout.EndVertical();
			} EditorGUILayout.EndVertical();
		}

		if(currentPlatform != previousCurrentPlatform) {
			changed = true;
			previousCurrentPlatform = currentPlatform;
		}
		
	}
	
	public void OnScriptLoad() {
		LoadPresets();
		if(File.Exists(Application.dataPath.Substring(0, Application.dataPath.Length - 7) + "/Library/LastPlayerSettingsPreset.tmp")) {
			string target = File.ReadAllText(Application.dataPath.Substring(0, Application.dataPath.Length - 7) + "/Library/LastPlayerSettingsPreset.tmp") + ".csv";
			if(File.Exists(path + target)) {
				LoadPreset(path + target);
				for(int i=0; i<presets.Count; i++) {
					if(presets[i].Contains(target)) {
						currentPreset = i;
						break;
					}
				}
			}
		}
		scriptLoaded = true;

	}
	
	public void OnProjectChange() {
		LoadPresets();
		currentPreset = -1;
		
	}
	
	public static void LoadPresets() {
		presets = Directory.GetFiles(path, "*.csv").ToList();
		
	}

	public static void LoadPreset(int index) {
		currentPreset = index;
		LoadPreset(presets[index]);

	}

	public static void LoadPreset(string path) {
		Dictionary<string, string> settings = new Dictionary<string, string>();
		settings.LoadCSV(File.ReadAllText(path));
		List<string> keys = settings.Keys.ToList();
		if(settings.ContainsKey("Platform")) {
			ProcessSetting("Platform", settings["Platform"]);
			settings.Remove("Platform");
		}
		foreach(string key in keys) {
			if(settings.ContainsKey(key)) {
				ProcessSetting(key, settings[key]);
				settings.Remove(key);
			}
		}
		previousCurrentPlatform = currentPlatform;
		changed = false;
		
	}
	
	// I'd really like to handle a lot of this through reflection, but Unity doesn't like me using Type.GetType(string) on its classes.
	public static void ProcessSetting(string name, string val) {
		switch(name) {
			#region shared
			case "Platform":
				EnumUtils.TryParse<BuildTargetGroup>(val, out currentPlatform);
				EditorUserBuildSettings.selectedBuildTargetGroup = currentPlatform;
				break;
			case "defaultIcon":
				Texture2D icon = AssetDatabase.LoadAssetAtPath(val, typeof(Texture2D)) as Texture2D;
				PlayerSettings.SetIconsForTargetGroup(BuildTargetGroup.Unknown, new Texture2D[] { icon });
				break;
			case "scriptingDefineSymbols":
				PlayerSettings.SetScriptingDefineSymbolsForGroup(currentPlatform, val);
				break;
			case "companyName":
				PlayerSettings.companyName = val;
				break;
			case "productName":
				PlayerSettings.productName = val;
				break;
			case "iPhoneBundleIdentifier":
				PlayerSettings.iPhoneBundleIdentifier = val;
				break;
			case "keystorePass":
				PlayerSettings.keystorePass = val;
				break;
			case "keyaliasPass":
				PlayerSettings.keyaliasPass = val;
				break;
			case "bundleVersion":
				PlayerSettings.bundleVersion = val;
				break;
			case "bundleIdentifier":
				PlayerSettings.bundleIdentifier = val;
				break;
			case "statusBarHidden":
				PlayerSettings.statusBarHidden = Boolean.Parse(val);
				break;
			case "defaultInterfaceOrientation":
				UIOrientation tempdefaultInterfaceOrientation;
				EnumUtils.TryParse<UIOrientation>(val, out tempdefaultInterfaceOrientation);
				PlayerSettings.defaultInterfaceOrientation = tempdefaultInterfaceOrientation;
				break;
			case "allowedAutorotateToPortrait":
				PlayerSettings.allowedAutorotateToPortrait = Boolean.Parse(val);
				break;
			case "allowedAutorotateToPortraitUpsideDown":
				PlayerSettings.allowedAutorotateToPortraitUpsideDown = Boolean.Parse(val);
				break;
			case "allowedAutorotateToLandscapeRight":
				PlayerSettings.allowedAutorotateToLandscapeRight = Boolean.Parse(val);
				break;
			case "allowedAutorotateToLandscapeLeft":
				PlayerSettings.allowedAutorotateToLandscapeLeft = Boolean.Parse(val);
				break;
			case "useAnimatedAutorotation":
				PlayerSettings.useAnimatedAutorotation = Boolean.Parse(val);
				break;
			case "use32BitDisplayBuffer":
				PlayerSettings.use32BitDisplayBuffer = Boolean.Parse(val);
				break;
			case "apiCompatibilityLevel":
				ApiCompatibilityLevel tempapiCompatibilityLevel;
				EnumUtils.TryParse<ApiCompatibilityLevel>(val, out tempapiCompatibilityLevel);
				PlayerSettings.apiCompatibilityLevel = tempapiCompatibilityLevel;
				break;
			case "stripUnusedMeshComponents":
				PlayerSettings.stripUnusedMeshComponents = Boolean.Parse(val);
				break;
			case "aotOptions":
				PlayerSettings.aotOptions = val;
				break;
			case "accelerometerFrequency":
				PlayerSettings.accelerometerFrequency = Int32.Parse(val);
				break;
			case "MTRendering":
				PlayerSettings.MTRendering = Boolean.Parse(val);
				break;
			case "mobileMTRendering":
				PlayerSettings.mobileMTRendering = Boolean.Parse(val);
				break;
			case "renderingPath":
				RenderingPath temprenderingPath;
				EnumUtils.TryParse<RenderingPath>(val, out temprenderingPath);
				PlayerSettings.renderingPath = temprenderingPath;
				break;
			case "mobileRenderingPath":
				RenderingPath tempmobileRenderingPath;
				EnumUtils.TryParse<RenderingPath>(val, out tempmobileRenderingPath);
				PlayerSettings.renderingPath = tempmobileRenderingPath;
				break;
			case "stereoscopic3D":
				PlayerSettings.SetPropertyString("VR::devices", Boolean.Parse(val) ? "stereo" : "mono");
				break;
			#endregion
			#region android
#if UNITY_4_6
			case "Android.use24BitDepthBuffer":
				PlayerSettings.Android.use24BitDepthBuffer = Boolean.Parse(val);
				break;
#else
			case "Android.disableDepthAndStencilBuffers":
				PlayerSettings.Android.disableDepthAndStencilBuffers = Boolean.Parse(val);
				break;
#endif
			case "Android.bundleVersionCode":
				PlayerSettings.Android.bundleVersionCode = Int32.Parse(val);
				break;
			case "Android.minSdkVersion":
				AndroidSdkVersions tempminSdkVersion;
				EnumUtils.TryParse<AndroidSdkVersions>(val, out tempminSdkVersion);
				PlayerSettings.Android.minSdkVersion = tempminSdkVersion;
				break;
			case "Android.preferredInstallLocation":
				AndroidPreferredInstallLocation temppreferredInstallLocation;
				EnumUtils.TryParse<AndroidPreferredInstallLocation>(val, out temppreferredInstallLocation);
				PlayerSettings.Android.preferredInstallLocation = temppreferredInstallLocation;
				break;
			case "Android.forceInternetPermission":
				PlayerSettings.Android.forceInternetPermission = Boolean.Parse(val);
				break;
			case "Android.forceSDCardPermission":
				PlayerSettings.Android.forceSDCardPermission = Boolean.Parse(val);
				break;
			case "Android.targetDevice":
				AndroidTargetDevice temptargetDevice;
				EnumUtils.TryParse<AndroidTargetDevice>(val, out temptargetDevice);
				PlayerSettings.Android.targetDevice = temptargetDevice;
				break;
			case "Android.splashScreenScale":
				AndroidSplashScreenScale tempsplashScreenScale;
				EnumUtils.TryParse<AndroidSplashScreenScale>(val, out tempsplashScreenScale);
				PlayerSettings.Android.splashScreenScale = tempsplashScreenScale;
				break;
			case "Android.keystoreName":
				PlayerSettings.Android.keystoreName = val;
				break;
			case "Android.keystorePass":
				PlayerSettings.Android.keystorePass = val;
				break;
			case "Android.keyaliasName":
				PlayerSettings.Android.keyaliasName = val;
				break;
			case "Android.keyaliasPass":
				PlayerSettings.Android.keyaliasPass = val;
				break;
			case "Android.useAPKExpansionFiles":
				PlayerSettings.Android.useAPKExpansionFiles = Boolean.Parse(val);
				break;
			case "Android.showActivityIndicatorOnLoading":
				AndroidShowActivityIndicatorOnLoading tempshowActivityIndicatorOnLoading;
				EnumUtils.TryParse<AndroidShowActivityIndicatorOnLoading>(val, out tempshowActivityIndicatorOnLoading);
				PlayerSettings.Android.showActivityIndicatorOnLoading = tempshowActivityIndicatorOnLoading;
				break;
			#endregion
			#region iOS
			case "iOS.applicationDisplayName":
				PlayerSettings.iOS.applicationDisplayName = val;
				break;
			case "iOS.scriptCallOptimization":
				ScriptCallOptimizationLevel tempscriptCallOptimization;
				EnumUtils.TryParse<ScriptCallOptimizationLevel>(val, out tempscriptCallOptimization);
				PlayerSettings.iOS.scriptCallOptimization = tempscriptCallOptimization;
				break;
			case "iOS.sdkVersion":
				iOSSdkVersion tempsdkVersion;
				EnumUtils.TryParse<iOSSdkVersion>(val, out tempsdkVersion);
				PlayerSettings.iOS.sdkVersion = tempsdkVersion;
				break;
			case "iOS.targetOSVersion":
				iOSTargetOSVersion temptargetOSVersion;
				EnumUtils.TryParse<iOSTargetOSVersion>(val, out temptargetOSVersion);
				PlayerSettings.iOS.targetOSVersion = temptargetOSVersion;
				break;
			case "iOS.targetDevice":
				iOSTargetDevice tempiostargetDevice;
				EnumUtils.TryParse<iOSTargetDevice>(val, out tempiostargetDevice);
				PlayerSettings.iOS.targetDevice = tempiostargetDevice;
				break;
			case "iOS.targetResolution":
				//iOSTargetResolution temptargetResolution;
				//EnumUtils.TryParse<iOSTargetResolution>(val, out temptargetResolution);
				//PlayerSettings.iOS.targetResolution = temptargetResolution;
				
				//iOSTargetResolution is obsolete as of 5.3.0
				//Use Screen.SetResolution(temptargetResolution);
				break;
			case "iOS.prerenderedIcon":
				PlayerSettings.iOS.prerenderedIcon = Boolean.Parse(val);
				break;
			case "iOS.requiresPersistentWiFi":
				PlayerSettings.iOS.requiresPersistentWiFi = Boolean.Parse(val);
				break;
			case "iOS.statusBarStyle":
				iOSStatusBarStyle tempstatusBarStyle;
				EnumUtils.TryParse<iOSStatusBarStyle>(val, out tempstatusBarStyle);
				PlayerSettings.iOS.statusBarStyle = tempstatusBarStyle;
				break;
#if UNITY_4_6
			case "iOS.exitOnSuspend":
				PlayerSettings.iOS.exitOnSuspend = Boolean.Parse(val);
				break;
#else
			case "iOS.appInBackgroundBehavior":
				iOSAppInBackgroundBehavior tempiosappInBackgroundBehavior;
				EnumUtils.TryParse<iOSAppInBackgroundBehavior>(val, out tempiosappInBackgroundBehavior);
				PlayerSettings.iOS.appInBackgroundBehavior = tempiosappInBackgroundBehavior;
				break;
#endif
			case "iOS.showActivityIndicatorOnLoading":
				iOSShowActivityIndicatorOnLoading tempiosshowActivityIndicatorOnLoading;
				EnumUtils.TryParse<iOSShowActivityIndicatorOnLoading>(val, out tempiosshowActivityIndicatorOnLoading);
				PlayerSettings.iOS.showActivityIndicatorOnLoading = tempiosshowActivityIndicatorOnLoading;
				break;
			#endregion
			default:
				Debug.LogWarning("Unprocessed setting: " + name + " Value: " + val);
				break;
		}
		
	}

	public static void SavePreset(int index) {
		SavePresetAs(presets[index]);

	}

	public static void SavePresetAs(string name) {
		if(File.Exists(name)) {
			File.Delete(name);
		}
		File.WriteAllText(name, CurrentSettingsAsCSV());

	}

	public static string CurrentSettingsAsCSV() {
		#region shared
		string output = "Platform," + currentPlatform + "\n";
		output += "defaultIcon," + AssetDatabase.GetAssetPath(PlayerSettings.GetIconsForTargetGroup(BuildTargetGroup.Unknown)[0]) + "\n";
		output += "scriptingDefineSymbols," + PlayerSettings.GetScriptingDefineSymbolsForGroup(currentPlatform) + "\n";
		output += "companyName," + PlayerSettings.companyName + "\n";
		output += "productName," + PlayerSettings.productName + "\n";
		output += "iPhoneBundleIdentifier," + PlayerSettings.iPhoneBundleIdentifier + "\n";
		output += "keystorePass," + PlayerSettings.keystorePass + "\n";
		output += "keyaliasPass," + PlayerSettings.keyaliasPass + "\n";
		output += "bundleVersion," + PlayerSettings.bundleVersion + "\n";
		output += "bundleIdentifier," + PlayerSettings.bundleIdentifier + "\n";
		output += "statusBarHidden," + PlayerSettings.statusBarHidden + "\n";
		output += "defaultInterfaceOrientation," + PlayerSettings.defaultInterfaceOrientation + "\n";
		output += "allowedAutorotateToPortrait," + PlayerSettings.allowedAutorotateToPortrait + "\n";
		output += "allowedAutorotateToPortraitUpsideDown," + PlayerSettings.allowedAutorotateToPortraitUpsideDown + "\n";
		output += "allowedAutorotateToLandscapeRight," + PlayerSettings.allowedAutorotateToLandscapeRight + "\n";
		output += "allowedAutorotateToLandscapeLeft," + PlayerSettings.allowedAutorotateToLandscapeLeft + "\n";
		output += "useAnimatedAutorotation," + PlayerSettings.useAnimatedAutorotation + "\n";
		output += "use32BitDisplayBuffer," + PlayerSettings.use32BitDisplayBuffer + "\n";
		output += "apiCompatibilityLevel," + PlayerSettings.apiCompatibilityLevel + "\n";
		output += "stripUnusedMeshComponents," + PlayerSettings.stripUnusedMeshComponents + "\n";
		output += "aotOptions," + PlayerSettings.aotOptions + "\n";
		output += "accelerometerFrequency," + PlayerSettings.accelerometerFrequency + "\n";
		output += "MTRendering," + PlayerSettings.MTRendering + "\n";
		output += "mobileMTRendering," + PlayerSettings.mobileMTRendering + "\n";
		output += "renderingPath," + PlayerSettings.renderingPath + "\n";
		output += "mobileRenderingPath," + PlayerSettings.renderingPath + "\n";
		//output += "stereoscopic3D," + PlayerSettings.stereoscopic3D + "\n";
		#endregion
		if(currentPlatform == BuildTargetGroup.Android) {
			#region android
#if UNITY_4_6
			output += "Android.use24BitDepthBuffer," + PlayerSettings.Android.use24BitDepthBuffer + "\n";
#else
			output += "Android.disableDepthAndStencilBuffers," + PlayerSettings.Android.disableDepthAndStencilBuffers + "\n";
#endif
			output += "Android.bundleVersionCode," + PlayerSettings.Android.bundleVersionCode + "\n";
			output += "Android.minSdkVersion," + PlayerSettings.Android.minSdkVersion + "\n";
			output += "Android.preferredInstallLocation," + PlayerSettings.Android.preferredInstallLocation + "\n";
			output += "Android.forceInternetPermission," + PlayerSettings.Android.forceInternetPermission + "\n";
			output += "Android.forceSDCardPermission," + PlayerSettings.Android.forceSDCardPermission + "\n";
			output += "Android.targetDevice," + PlayerSettings.Android.targetDevice + "\n";
			output += "Android.splashScreenScale," + PlayerSettings.Android.splashScreenScale + "\n";
			output += "Android.keystoreName," + PlayerSettings.Android.keystoreName + "\n";
			output += "Android.keystorePass," + PlayerSettings.Android.keystorePass + "\n";
			output += "Android.keyaliasName," + PlayerSettings.Android.keyaliasName + "\n";
			output += "Android.keyaliasPass," + PlayerSettings.Android.keyaliasPass + "\n";
			output += "Android.useAPKExpansionFiles," + PlayerSettings.Android.useAPKExpansionFiles + "\n";
			output += "Android.showActivityIndicatorOnLoading," + PlayerSettings.Android.showActivityIndicatorOnLoading;
			#endregion
#if UNITY_4_6
		} else if(currentPlatform == BuildTargetGroup.iPhone) {
#else
		} else if(currentPlatform == BuildTargetGroup.iOS) {
#endif
			#region iOS
			output += "iOS.applicationDisplayName," + PlayerSettings.iOS.applicationDisplayName + "\n";
			output += "iOS.scriptCallOptimization," + PlayerSettings.iOS.scriptCallOptimization + "\n";
			output += "iOS.sdkVersion," + PlayerSettings.iOS.sdkVersion + "\n";
			output += "iOS.targetOSVersion," + PlayerSettings.iOS.targetOSVersion + "\n";
			output += "iOS.targetDevice," + PlayerSettings.iOS.targetDevice + "\n";
			//output += "iOS.targetResolution," + PlayerSettings.iOS.targetResolution + "\n";
			output += "iOS.prerenderedIcon," + PlayerSettings.iOS.prerenderedIcon + "\n";
			output += "iOS.requiresPersistentWiFi," + PlayerSettings.iOS.requiresPersistentWiFi + "\n";
			output += "iOS.statusBarStyle," + PlayerSettings.iOS.statusBarStyle + "\n";
#if UNITY_4_6
			output += "iOS.exitOnSuspend," + PlayerSettings.iOS.exitOnSuspend + "\n";
#else
			output += "iOS.appInBackgroundBehavior," + PlayerSettings.iOS.appInBackgroundBehavior + "\n";
#endif
			output += "iOS.showActivityIndicatorOnLoading," + PlayerSettings.iOS.showActivityIndicatorOnLoading;
			#endregion
		}
		return output;
	}
	
}
#endif


// end the abortion
#pragma warning restore
