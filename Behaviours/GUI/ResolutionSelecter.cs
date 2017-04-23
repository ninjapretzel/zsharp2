using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class ResolutionSelecter : OptionSelecter {
	public static int width = 1280;
	public static int height = 720;
	public static int refresh = 60;
	public static bool fullScreen = false;
	public static bool borderless = false;
	
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
	private static bool applyBorderless = false;
	private static int frameCounter = 0;
#endif

#if XtoJSON

	public override void Set(string setting, string value) {

		var splits = value.Split('x');
		width = splits[0].ParseInt();
		height = splits[1].ParseInt();

		var obj = new JsonObject();
		obj["width"] = width;
		obj["height"] = height;
		obj["fullScreen"] = fullScreen;
		obj["borderless"] = borderless;

		Settings.instance["resolution"] = obj;

#if UNITY_EDITOR
		Debug.Log("ResolutionSelecter.Set: Would set resolution to " + width + "x" + height + " on apply");
#else

#endif

	}

	public override string Get(string setting) {
		string str = "";
		var res = Settings.instance["resolution"];
		if (res.isObject) {
			str += res["width"] + "x" + res["height"];
		} else if (res.isString) {
			str += res.stringVal;
		}
		return str;
	}

#endif
	public static void ChangeSettingsToScreenSize() {
		width = Screen.width;
		height = Screen.height;
		fullScreen = Screen.fullScreen;

#if XtoJSON
		var obj = new JsonObject();
		obj["width"] = width;
		obj["height"] = height;
		obj["fullScreen"] = fullScreen;
		obj["borderless"] = borderless;

		Settings.instance["resolution"] = obj;
#else
		Debug.LogWarning("Settings does not have fields for screen settings at the moment");
#endif
	}

	public static bool IsCurrentResolution(Resolution res) {
		return res.width == Screen.currentResolution.width && res.height == Screen.currentResolution.height;
	}

	protected override void Awake() {
		base.Awake();

#if !UNITY_EDITOR

		options.Clear();
		int i = 0;
		foreach (var res in Screen.resolutions) {
			options.Add(res.width + "x" + res.height);

			if (IsCurrentResolution(res)) {
				currentIndex = i;
			}
			i++;
		}
			
#else

		width = Screen.width;
		height = Screen.height;
		fullScreen = false;
		borderless = false;

#endif

	}

	protected override void OnEnable() {
		base.OnEnable();

		width = Screen.width;
		height = Screen.height;
		fullScreen = Screen.fullScreen;

#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
		/*
		if (borderless && !fullScreen) {
			WindowHandler.SetBorderless(borderless);
		}
		//*/
#endif


		if (display == null) { display = transform.Find("Display").GetComponent<Text>(); }

		display.text = width +"x"+height;

	}
	
#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
	protected override void Update() {
		// Must wait one frame after switching out of fullscreen before applying borderless mode.
		base.Update();
		/*
		if (applyBorderless) {
			++frameCounter;
			if (frameCounter >= 2) {
				WindowHandler.SetBorderless(borderless);
				frameCounter = 0;
				applyBorderless = false;
			}
		}
		//*/
	}
#endif


	public static void Apply() {
#if UNITY_EDITOR
		Debug.Log("ResolutionSelecter.Apply: Would set resolution to " + width + "x" + height);
		if (fullScreen) {
			var gvWndType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.GameView");
			var selectedSizeIndexProp = gvWndType.GetProperty("selectedSizeIndex", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic);
			var gvWnd = UnityEditor.EditorWindow.GetWindow(gvWndType);
			// Select 0 for Free Aspect.
			selectedSizeIndexProp.SetValue(gvWnd, 0, null);
		} else {

		}
#else
		Screen.SetResolution(width, height, fullScreen);
#if UNITY_STANDALONE_WIN
		if (!fullScreen) {
			applyBorderless = true;
		}
#endif
#endif
	}
	
}
