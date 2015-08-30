using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

#if XtoJSON
public class UGUIzSkin {

	public static bool loaded = Load();
	public static bool Load() {
		defaultSkinInfo = Json.Parse(Resources.Load<TextAsset>("standard").text) as JsonObject;
		standard = new UGUIzSkin();
		return true;
	}
	static JsonObject defaultSkinInfo;
	public static UGUIzSkin standard { get; private set; }

	public Sprite this[string key] {
		get {
			if (sprites.ContainsKey(key)) { return sprites[key]; }
			return defaultSprite;
		}
	}

	public Sprite GetSprite(string key, string state = "", string aug = "") {
		if (sprites.ContainsKey(key + state + aug)) { return sprites[key + state + aug]; }
		else if (sprites.ContainsKey(key + aug)) { return sprites[key + aug]; }
		else if (sprites.ContainsKey(key + state)) { return sprites[key + state]; }
		else if (sprites.ContainsKey(key)) { return sprites[key]; }
		return defaultSprite;
	}

	public Sprite GetNormal(string key) { return GetSprite(key, off); }
	public Sprite GetDisabled(string key) { return GetSprite(key, off, disabled); }
	public Sprite GetActive(string key) { return GetSprite(key, off, active); }
	public Sprite GetHover(string key) { return GetSprite(key, off, hover); }
	
	public Sprite GetOnNormal(string key) { return GetSprite(key, on); }
	public Sprite GetOnDisabled(string key) { return GetSprite(key, on, disabled); }
	public Sprite GetOnActive(string key) { return GetSprite(key, on, active); }
	public Sprite GetOnHover(string key) { return GetSprite(key, on, hover); }

	public Color GetColor(string key, string state = "") {
		if (colors.ContainsKey(key + state)) { return colors[key + state]; }
		else if (colors.ContainsKey(state)) { return colors[state]; }
		else if (colors.ContainsKey(key)) { return colors[key]; }
		return colors["default"];
	}

	public Color GetNormalColor(string key) { return GetColor(key, "normal"); }
	public Color GetHighlightColor(string key) { return GetColor(key, "highlight"); }
	public Color GetPressedColor(string key) { return GetColor(key, "pressed"); }
	public Color GetDisabledColor(string key) { return GetColor(key, "disabled"); }
	
	public bool HasColorBlock(string key) { 
		return colors.ContainsKey(key+"normal")
				&& colors.ContainsKey(key+"highlight")
				&& colors.ContainsKey(key+"pressed")
				&& colors.ContainsKey(key+"disabled");
	}

	public ColorBlock GetColorBlock(string key) { 
		ColorBlock cb = new ColorBlock();
		cb.normalColor = GetNormalColor(key);
		cb.highlightedColor = GetHighlightColor(key);
		cb.pressedColor = GetPressedColor(key);
		cb.disabledColor = GetDisabledColor(key);
		cb.colorMultiplier = 1;
		cb.fadeDuration = .1f;
		return cb;
	}

	public bool HasSpriteBlock(string key) {
		return sprites.ContainsKey(key + off + hover) || sprites.ContainsKey(key + hover)
				|| sprites.ContainsKey(key + off + active) || sprites.ContainsKey(key + active);
	}

	public SpriteState GetSpriteBlock(string key) {
		SpriteState ss = new SpriteState();
		ss.pressedSprite = GetActive(key);
		ss.highlightedSprite = GetHover(key);
		ss.disabledSprite = GetDisabled(key);

		return ss;
	}

	public Sprite defaultSprite;
	public Dictionary<string, Sprite> sprites;
	public Dictionary<string, Color> colors;

	public string disabled = "_disabled";
	public string active = "_active";
	public string hover = "_hover";
	public string off = "Off";
	public string on = "On";

	public Font font;
	public int fontSize = 16;
	string path;
	
	public UGUIzSkin() : this(defaultSkinInfo) { }
	public UGUIzSkin(JsonObject info) {
		colors = new Dictionary<string,Color>();
		colors["default"] = Color.white;

		sprites = new Dictionary<string,Sprite>();
		sprites["background"] = Resources.Load<Sprite>("UIBackground");
		sprites["default"] = Resources.Load<Sprite>("UISprite");

		JsonObject spriteInfo = info.Get<JsonObject>("sprites");
		JsonObject colorInfo = info.Get<JsonObject>("colors");

		defaultSprite = Resources.Load<Sprite>(info.Extract<string>("defaultSprite", "UISprite"));
		
		path = info.Get<string>("path");
		disabled = info.Extract<string>("disabled", disabled);
		active = info.Extract<string>("active", active);
		hover = info.Extract<string>("hover", hover);
		off = info.Extract<string>("off", off);
		on = info.Extract<string>("on", on);
		fontSize = info.Extract<int>("fontSize", fontSize);

		font = Resources.Load<Font>(info.Get<string>("font"));

		string[] mods = new string[] {
			"", disabled, active, hover,
			off, off + disabled, off + active, off + hover,
			on, on + disabled, on + active, on + hover,
		};


		foreach (var pair in spriteInfo) {
			string key = pair.Key.stringVal;
			string val = pair.Value.stringVal;
			
			foreach (string mod in mods) { LoadSprite(key, val, mod); }
		}

		foreach (var pair in colorInfo) {
			string key = pair.Key.stringVal;
			string val = pair.Value.stringVal;

			Color c = val.ParseHex();
			if (c != Color.clear) { colors[key] = c; }
		}

	}
	
	
	void LoadSprite(string key, string val, string mod = "") {
		Sprite loaded = Resources.Load<Sprite>(path + val + mod);
		if (loaded != null) {
			sprites[key + mod] = loaded;
		}
	}
	
	



	
}
#endif 
