using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;


public partial class GSS : MonoBehaviour {
	public string tagClass = "";
	string curTag = "";

	public string styleClass = "";
	string curStyle = "";

}

#if XtoJSON


public partial class GSS : MonoBehaviour {

	[NonSerialized] private static int lastDefaultHash = 0;
	[NonSerialized] private static int lastStyleHash = 0;
	static JsonObject styles = LoadStyles();
	public static bool loaded { get { return styles != null; } }

	static JsonObject LoadStyles() {
		try {
			if (!Application.isPlaying) {
				//Debug.Log("Reloaded styles when not playing... checking for load issues");
					var test = Resources.Load<TextAsset>("defaultStyles");
					//Debug.Log("success!");
			}
		} catch {
			Debug.Log("GSS: Failed! Could not load stylesheets!");
			return null;
		}


		string defaultStyleJson = Resources.Load<TextAsset>("defaultStyles").text;
		lastDefaultHash = defaultStyleJson.GetHashCode();

		JsonObject obj = Json.Parse(defaultStyleJson) as JsonObject;
		JsonArray cas = obj.Get<JsonArray>("cascades");

		TextAsset styleFile = Resources.Load<TextAsset>("styles");
		if (styleFile != null) {
			string styleJson = styleFile.text;
			lastStyleHash = styleJson.GetHashCode();

			JsonObject loadedStyles = Json.Parse(styleJson) as JsonObject;
			obj.SetRecursively(loadedStyles);

			JsonArray loadedCas = loadedStyles.Get<JsonArray>("cascades");
			if (loadedCas != null) {
				cas.AddAll(loadedCas);
				obj["cascades"] = cas;
			}
				

		}



		Debug.Log("GSS: Loaded/Reloaded Stylesheets");
		return obj;
	}

	public static bool ReloadStyles() {
		if (styles == null) { 
			styles = LoadStyles();
			return true;
		}

		string defaultStyleJson = Resources.Load<TextAsset>("defaultStyles").text;
		bool reload = defaultStyleJson.GetHashCode() != lastDefaultHash;

		TextAsset styleFile = Resources.Load<TextAsset>("styles");
		if (styleFile != null) {
			string styleJson = styleFile.text;
			reload |= styleJson.GetHashCode() != lastStyleHash;

		}

		if (reload) { styles = LoadStyles(); }
		return reload;
	}

	static float scaleRatio { get { return ((float)Screen.height) / 720f; } }
	
	public static JsonArray cascades { get { return styles["cascades"] as JsonArray; } }

	void Start() {
		if (styleClass != curStyle || tagClass != curTag) {
			//Debug.Log("Updating style on " + name);
			UpdateStyle(tagClass, styleClass);
			curStyle = styleClass;
			curTag = tagClass;
		}
		
	}
	
	void Update() {
		if (styleClass != curStyle || tagClass != curTag) {
			//Debug.Log("Updating style on " + name);
			UpdateStyle(tagClass, styleClass);
			curStyle = styleClass;
			curTag = tagClass;
		}

		
	}


	void UpdateStyle(string tag, string style) { ApplyStyle(this, tag, style); }
	void ApplyStyle(JsonObject style) { ApplyStyle(this, style); }

	public static void ApplyStyle(Component c, string tag, string style) {
		if ((tag != null && styles.ContainsKey(tag)) || (style != null && styles.ContainsKey(style))) {
			JsonObject styleObj = styles.Get<JsonObject>(style);
			JsonObject tagObj = styles.Get<JsonObject>(tag);

			JsonObject combObj = new JsonObject();
			if (tagObj != null) { combObj.Set(tagObj); }
			if (styleObj != null && tag != style) { combObj.Set(styleObj); }

			//Debug.Log("Built Style [" + tag + "." + style + "] :" + combObj.PrettyPrint());
			ApplyStyle(c, combObj);
		}

	}
	public static void ApplyStyle(Component c, JsonObject style) {
		string inherit = style.Get<string>("inherit");
		if (inherit != null && inherit != "" && styles.ContainsKey(inherit)) {
			JsonObject inheritedStyle = styles.Get<JsonObject>(inherit);
			ApplyStyle(c, inheritedStyle);
		}

		Image img = c.GetComponent<Image>();
		Button btn = c.GetComponent<Button>();
		Text txt = c.GetComponent<Text>();
		ScrollRect scr = c.GetComponent<ScrollRect>();
		InputField inf = c.GetComponent<InputField>();

		if (img != null) { ApplyImageStyle(img, style); }
		if (btn != null) { ApplyButtonStyle(btn, style); }
		if (txt != null) { ApplyTextStyle(txt, style); }
		if (scr != null) { ApplyScrollRectStyle(scr, style); }
		if (inf != null) { ApplyInputFieldStyle(inf, style); }

		foreach (var pair in style) {
			Transform target = c.transform.Find(pair.Key);
			if (target != null) { 
				GSS gss = target.GetComponent<GSS>();
				if (gss != null) { Destroy(gss); }
				if (pair.Value.isObject) { ApplyStyle(target, pair.Value as JsonObject); }
				if (pair.Value.isString) { ApplyStyle(target, pair.Value.stringVal, ""); }
			}
		}

	}

	static void ApplyImageStyle(Image img, JsonObject style) {
		if (style.ContainsKey("image")) { img.sprite = Resources.Load<Sprite>(style.Get<string>("image")); }


		if (style.ContainsKey("color")) { img.color = GetColor(style, "color"); }
		if (style.ContainsKey("material")) { img.material = Resources.Load<Material>(style.Get<string>("material")); }
		if (style.ContainsKey("raycastTarget")) { img.raycastTarget = style.Get<bool>("raycastTarget"); }
		if (style.ContainsKey("imageType")) { img.type = style.Get<Image.Type>("imageType"); }
		if (style.ContainsKey("fillCenter")) { img.fillCenter = style.Get<bool>("fillCenter"); }
		if (style.ContainsKey("preserveAspect")) { img.preserveAspect = style.Get<bool>("preserveAspect"); }
		//TBD: Figure out fillOrigin and what the numbers mean
		//if (style.ContainsKey("fillMethod")) { img.fillMethod = style.Get<Image.FillMethod>("fillMethod"); }
		//if (style.ContainsKey("fillOrigin")) { img.fillOrigin = style.Get<Image.

		
	}



	static void ApplyButtonStyle(Button btn, JsonObject style) {
		ApplySelectableStyle(btn, style);
	}

	static void ApplyScrollbarStyle(Scrollbar scb, JsonObject style) {
		ApplySelectableStyle(scb, style);
	}

	static void ApplyInputFieldStyle(InputField inf, JsonObject style) {
		ApplySelectableStyle(inf, style);
		if (style.ContainsKey("selectionColor")) { inf.selectionColor = GetColor(style, "selectionColor"); }

	}

	static void ApplyTextStyle(Text txt, JsonObject style) {
		if (style.ContainsKey("font")) { txt.font = Resources.Load<Font>(style.Get<string>("font")); }
		if (style.ContainsKey("fontStyle")) { txt.fontStyle = style.Get<FontStyle>("fontStyle"); }

		bool scale = style.Extract("sizeScale", true);
		if (style.ContainsKey("fontSize")) { txt.fontSize = (int)(style.Get<float>("fontSize") * (scale ? scaleRatio : 1)); }
		if (style.ContainsKey("richText")) { txt.supportRichText = style.Get<bool>("richText"); }
		if (style.ContainsKey("alignment")) { txt.alignment = style.Get<TextAnchor>("alignment"); }
		if (style.ContainsKey("lineSpacing")) { txt.lineSpacing = style.Get<float>("lineSpacing"); }
		if (style.ContainsKey("alignByGeometry")) { txt.alignByGeometry = style.Get<bool>("alignByGeometry"); }
		if (style.ContainsKey("horizontalOverflow")) { txt.horizontalOverflow = style.Get<HorizontalWrapMode>("horizontalOverflow"); }
		if (style.ContainsKey("verticalOverflow")) { txt.verticalOverflow = style.Get<VerticalWrapMode>("verticalOverflow"); }

		if (style.ContainsKey("bestFit")) { txt.resizeTextForBestFit = style.Get<bool>("bestFit"); }
		if (style.ContainsKey("color")) { txt.color = GetColor(style, "color"); }
		if (style.ContainsKey("material")) { txt.material = Resources.Load<Material>(style.Get<string>("material")); }

		if (style.ContainsKey("raycastTarget")) { txt.raycastTarget = style.Get<bool>("raycastTarget"); }

	}

	static void ApplyScrollRectStyle(ScrollRect scr, JsonObject style) {
		if (style.ContainsKey("horizontal")) {
			bool setHorizontal = style.Get<bool>("horizontal");
			scr.horizontal = setHorizontal;
			scr.horizontalScrollbarVisibility = setHorizontal ? ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport : ScrollRect.ScrollbarVisibility.AutoHide;
			scr.horizontalScrollbarSpacing = 0;
		}
		if (style.ContainsKey("vertical")) {
			bool setVertical = style.Get<bool>("vertical");
			scr.vertical = setVertical;
			scr.verticalScrollbarVisibility = setVertical ? ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport : ScrollRect.ScrollbarVisibility.AutoHide;
			scr.verticalScrollbarSpacing = 0;
		}

		if (style.ContainsKey("inertia")) { scr.inertia = style.Get<bool>("inertia"); }
		if (style.ContainsKey("decelerationRate")) { scr.decelerationRate = style.Get<float>("decelerationRate"); }
		if (style.ContainsKey("scrollSensitivity")) { scr.scrollSensitivity = style.Get<float>("scrollSensitivity"); }

	}





	static void ApplySelectableStyle(Selectable sct, JsonObject style) {
		if (style.ContainsKey("interactable")) { sct.interactable = style.Get<bool>("interactable"); }
		if (style.ContainsKey("transition")) { sct.transition = style.Get<Selectable.Transition>("transition"); }
		
		sct.spriteState = ApplySpriteStateStyle(sct.spriteState, style);
		sct.colors = ApplyColorBlockStyle(sct.colors, style);

	}
	static ColorBlock ApplyColorBlockStyle(ColorBlock block, JsonObject style) {
		if (style.ContainsKey("normalColor")) { block.normalColor = GetColor(style, "normalColor"); }
		if (style.ContainsKey("highlightedColor")) { block.highlightedColor = GetColor(style, "highlightedColor"); }
		if (style.ContainsKey("pressedColor")) { block.pressedColor = GetColor(style, "pressedColor"); }
		if (style.ContainsKey("disabledColor")) { block.disabledColor = GetColor(style, "disabledColor"); }
		if (style.ContainsKey("colorMultiplier")) { block.colorMultiplier = style.Get<float>("colorMultiplier"); }
		if (style.ContainsKey("fadeDuration")) { block.fadeDuration = style.Get<float>("fadeDuration"); }
		return block;
	}

	static SpriteState ApplySpriteStateStyle(SpriteState sprites, JsonObject style) {
		if (style.ContainsKey("highlightedSprite")) { sprites.highlightedSprite = Resources.Load<Sprite>(style.Get<string>("highlightedSprite")); }
		if (style.ContainsKey("pressedSprite")) { sprites.pressedSprite = Resources.Load<Sprite>(style.Get<string>("pressedSprite")); }
		if (style.ContainsKey("disabledSprite")) { sprites.disabledSprite = Resources.Load<Sprite>(style.Get<string>("disabledSprite")); }

		return sprites;
	}

	static Color GetColor(JsonObject o, string k) { return Colors.ParseHex(o.Get<string>(k)); }
}


#endif
