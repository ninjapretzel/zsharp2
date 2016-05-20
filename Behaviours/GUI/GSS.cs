using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
#if TMPRO
using TMPro;

//Extensions to GSS for supporting TextMeshPro UI Text objects
public partial class GSS {
	/// <summary> Mappings of alignments from Unity's TextAnchor to TextMeshPro's TextAlignmentOptions </summary>
	static Dictionary<TextAnchor, TextAlignmentOptions> alignmentMappings = new Dictionary<TextAnchor,TextAlignmentOptions>() {
		{TextAnchor.UpperLeft,		TextAlignmentOptions.TopLeft},
		{TextAnchor.UpperCenter,	TextAlignmentOptions.Top},
		{TextAnchor.UpperRight,		TextAlignmentOptions.TopRight},
		{TextAnchor.MiddleLeft,		TextAlignmentOptions.Left},
		{TextAnchor.MiddleCenter,	TextAlignmentOptions.Center},
		{TextAnchor.MiddleRight,	TextAlignmentOptions.Right},
		{TextAnchor.LowerLeft,		TextAlignmentOptions.BottomLeft},
		{TextAnchor.LowerCenter,	TextAlignmentOptions.Bottom},
		{TextAnchor.LowerRight,		TextAlignmentOptions.BottomRight},
	};

	/// <summary> Mappings of styles from Unity's FontStyle to TextMeshPro's FontStyles</summary>
	static Dictionary<FontStyle, FontStyles> styleMappings = new Dictionary<FontStyle,FontStyles>() {
		{FontStyle.Normal,			FontStyles.Normal},
		{FontStyle.Bold,			FontStyles.Bold},
		{FontStyle.Italic,			FontStyles.Italic},
		{FontStyle.BoldAndItalic,	FontStyles.Bold & FontStyles.Italic},
	};

	/// <summary> Resources paths to check when loading TextMeshPro Font assets/materials </summary>
	static string[] fontAssetPaths = { "", "Font", "Fonts", "Fonts & Materials", };
	/// <summary> Loads a TextMeshPro TMP_FontAsset object. Checks paths given in fontAssetPaths, and checks for fonts named (fontName + " SDF") as well </summary>
	/// <param name="fontName"> Font name to check for </param>
	/// <returns> Font named fontName or (fontName + " SDF") located in Resources, Resources/Font, Resources/Fonts, or Resources/Fonts & Materials </returns>
	static TMP_FontAsset GetTMPFontAsset(string fontName) {
		TMP_FontAsset check;
		foreach (var path in fontAssetPaths) {
			var p = path + ((path.Length > 0) ? "/" : "");
			check = Resources.Load<TMP_FontAsset>(p + fontName);
			if (check != null) { return check; }
			check = Resources.Load<TMP_FontAsset>(p + fontName + " SDF");
			if (check != null) { return check; }
		}
		return null;
	}
}
#endif

public partial class GSS : MonoBehaviour {
	public string tagClass = "";
	[NonSerialized] string curTag = "";

	public string styleClass = "";
	[NonSerialized] string curStyle = "";

}

#if XtoJSON

[ExecuteInEditMode]
/// <summary> 
///	GUI StyleSheets behaviour.
/// Applies styles to UGUI behaviours based on the name of the object,
/// as well as a given set of stylesheets in the project.
/// 
/// Provides detection of changes on the stylesheets, and will reload the stylesheets source files have changed.
/// </summary>
public partial class GSS : MonoBehaviour {
	
	/// <summary> Hash of the last 'Built-In' stylesheet file </summary>
	[NonSerialized] private static int lastDefaultHash = 0;
	/// <summary> Hash of the last 'Custom' stylesheet file </summary>
	[NonSerialized] private static int lastStyleHash = 0;

	/// <summary> Currently loaded stylesheets. </summary>
	static JsonObject styles = LoadStyles();

	
	//Yay for dynamic programming optimizations
	/// <summary> Cached styles, used for efficiency, since many elements will share the same styles. When styles are reloaded, this object is purged. </summary>
	static JsonObject cachedStyles = new JsonObject();

	//Yay for dynamic programming optimizations
	/// <summary> Cached inheritance, used for efficiency, since many styles inherit from the same set of styles. When styles are reloaded, this object is purged. </summary>
	static JsonObject cachedInherit = new JsonObject();

	/// <summary> Global flag to restyle everything on the next frame</summary>
	public static bool restyleEverything = false;

	/// <summary> Are stylesheets loaded? </summary>
	public static bool loaded { get { return styles != null; } }

	/// <summary> Load StyleSheet files from resources, and return the loaded sheets as a JsonObject </summary>
	/// <returns> JsonObject containing stylesheets</returns>
	static JsonObject LoadStyles() {
		try {
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

#if UNITY_EDITOR
			//Print out this message if stylesheets have been loaded or re-loaded
			Debug.Log("GSS: Loaded/Reloaded Stylesheets");
#endif
			cachedStyles = new JsonObject();
			cachedInherit = new JsonObject();

			
			return obj;
		} catch {
			return null;
		}
	}

	/// <summary> Check for styles being out-of-date, and reload if necessary.. </summary>
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

		if (reload) {
			cachedStyles = new JsonObject();
			cachedInherit = new JsonObject();
			styles = LoadStyles(); 
		}
		return reload;
	}

	/// <summary> Scale ratio for text size. </summary>
	static float scaleRatio { get { return ((float)Screen.height) / 720f; } }

	/// <summary>
	/// Quick acess to the 'cascades' defined in the stylesheet.
	/// This is an array of objects describing what object names
	///		to automatically apply styles to.
	/// </summary>
	public static JsonArray cascades { get { return styles["cascades"] as JsonArray; } }

	void Start() {
		if (styleClass != curStyle || tagClass != curTag) {
			//Debug.Log("Updating style on " + name);
			UpdateStyle(tagClass, styleClass);
			curStyle = styleClass;
			curTag = tagClass;
		}
		
	}

	int lastHeight = -1;
	int lastWidth = -1;

	void Update() {

		if (restyleEverything) {
			curTag = "";
			curStyle = "";
		} 

		if (lastHeight != Screen.height || lastWidth != Screen.width) {
			curTag = "";
			curStyle = "";
		}
		if (styleClass != curStyle || tagClass != curTag) {
			UpdateStyle(tagClass, styleClass);
			curStyle = styleClass;
			curTag = tagClass;
		}

		lastWidth = Screen.width;
		lastHeight = Screen.height;
	}

	void LateUpdate() {
		restyleEverything = false;
	}


	/// <summary> Update style based on the object's tag and style </summary>
	public void UpdateStyle() { UpdateStyle(curTag, curStyle); }

	/// <summary> Update style based on a given tag and style </summary>
	void UpdateStyle(string tag, string style) { ApplyStyle(this, tag, style); }

	/// <summary> Update style based on description in a JsonObject </summary>
	void ApplyStyle(JsonObject style) { ApplyStyle(this, style); }

	/// <summary> Apply styles to a given object. </summary>
	public static void ApplyStyle(Component c, string tag, string style) {
		JsonObject combObj = GetStyle(tag, style);
		if (combObj != null) { ApplyStyle(c, combObj); }

	}

	/// <summary> Get the JsonObject describing the style for a given tag and style</summary>
	public static JsonObject GetStyle(string tag, string style = null) {
		JsonObject combObj = null;
		if (tag == null) { tag = ""; }
		if (style == null) { style = ""; }
		string styleID = tag + "." + style;
		if (cachedStyles.ContainsKey(styleID)) { return cachedStyles.Get<JsonObject>(styleID); }

		if (styles.ContainsKey(tag) || styles.ContainsKey(style)) {
			JsonObject styleObj = styles.Get<JsonObject>(style);
			JsonObject tagObj = styles.Get<JsonObject>(tag);

			combObj = new JsonObject();
			if (tagObj != null) { combObj.Set(tagObj); }
			if (styleObj != null && tag != style) { combObj.Set(styleObj); }

			combObj = ApplyInheritance(combObj);

			//Debug.Log("GSS.GetStyle: Created style [" + styleID + "] : " + combObj.PrettyPrint());
			cachedStyles[styleID] = combObj;

		}

		return combObj;
	}

	/// <summary>Finds all JsonObjects under <paramref name="style"/> that inherit styles, and proccesses all of those inherited styles.</summary>
	/// <param name="style">Original style to process inheritance for</param>
	/// <returns><paramref name="style"/>, and all of its children, applied on top of the styles they inherit</returns>
	public static JsonObject ApplyInheritance(JsonObject style) {
		JsonObject clone = style.Clone();

		foreach (var pair in style) {
			if (pair.Value.isObject) {
				JsonObject obj = pair.Value as JsonObject;
				if (obj.ContainsKey("inherit")) {
					clone[pair.Key] = ApplyInheritance(obj);
				}
			}
		}

		string inherit = style.Get<string>("inherit");
		clone.Remove("inherit");
		if (inherit != null) {
			JsonObject inherited = GetInherited(inherit);

			return inherited.CombineRecursively(clone);
		}

		return clone;
	}


	/// <summary> Gets the COMPLETE inheritance tree of the style called <paramref name="inherit"/></summary>
	/// <param name="inherit">Name of style to calculate inheritance from </param>
	/// <returns>Complete style by name of <paramref name="inherit"/>, including all of the actual information from its parent styles </returns>
	public static JsonObject GetInherited(string inherit) {
		if (cachedInherit.ContainsKey(inherit)) { return cachedInherit.Get<JsonObject>(inherit); }

		JsonObject style = styles.Get<JsonObject>(inherit);
		if (style.ContainsKey("inherit")) {
			JsonObject inherited = GetInherited(style.Get<string>("inherit"));
			if (inherited != null) { return inherited.CombineRecursively(style); }
		}

		cachedInherit[inherit] = style;
		return style;
	}

	/// <summary> Apply style to all supported components on a given object. </summary>
	public static void ApplyStyle(Component c, JsonObject style) {
		Text txt = c.GetComponent<Text>();
		Image img = c.GetComponent<Image>();
		Button btn = c.GetComponent<Button>();
		ScrollRect scr = c.GetComponent<ScrollRect>();
		InputField inf = c.GetComponent<InputField>();
#if TMPRO
		TextMeshProUGUI tmp = c.GetComponent<TextMeshProUGUI>();

		if (tmp != null) { ApplyTextProStyle(tmp, style); }
#endif

		if (txt != null) { ApplyTextStyle(txt, style); }
		if (img != null) { ApplyImageStyle(img, style); }
		if (btn != null) { ApplyButtonStyle(btn, style); }
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

#if TMPRO
	static void ApplyTextProStyle(TextMeshProUGUI tmp, JsonObject style) {
		string language = Localization.language.ToString();

		if (style.ContainsKey("font")) {
			if (style["font"].isString) { tmp.font = GetTMPFontAsset(style.Get<string>("font")); }
			if (style["font"].isObject) {
				JsonObject languageFonts = style.Get<JsonObject>("font");
				if (languageFonts.ContainsKey(language)) {
					tmp.font = GetTMPFontAsset(languageFonts.Get<string>(language));
				} else {
					tmp.font = GetTMPFontAsset(languageFonts.Get<string>("english"));
				}
			}
		}
		
		if (style.ContainsKey("fontSize")) {
			bool scale = style.Extract("sizeScale", true);
			float screenScale = 1.0f * (scale ? scaleRatio : 1f);
			float languageScale = 1.0f;
			if (style.ContainsKey("fontScale")) {
				if (style["fontScale"].isNumber) { languageScale = style.Get<float>("fontScale"); }
				if (style["fontScale"].isObject) {
					JsonObject languageScales = style.Get<JsonObject>("fontScale");
					if (languageScales.ContainsKey(language)) {
						languageScale = languageScales.Get<float>(language);
					}
				}
			}


			float fontSize = (style.Get<float>("fontSize")) * languageScale * screenScale;
			tmp.fontSize = fontSize;
		}

		//Note: Rich text is forced on in TextMeshPro!
		if (style.ContainsKey("fontStyle")) { 
			var st = style.Get<FontStyle>("fontStyle");
			tmp.fontStyle = styleMappings[st];
		}

		if (style.ContainsKey("alignment")) { 
			var anchor = style.Get<TextAnchor>("alignment");
			tmp.alignment = alignmentMappings[anchor];
		}
		if (style.ContainsKey("lineSpacing")) { tmp.lineSpacing = style.Get<float>("lineSpacing"); }
		//Note: TextMeshPro has no 'alignByGeometry' property, and no equivlent options

		//Note: TextMeshPro uses different settings for for horizontalOverflow or verticalOverflow
		if (style.ContainsKey("verticalOverflow")) { 
			var ver = style.Get<VerticalWrapMode>("verticalOverflow");
			tmp.OverflowMode = (ver == VerticalWrapMode.Overflow) ? TextOverflowModes.Overflow : TextOverflowModes.Truncate;
		}

		if (style.ContainsKey("horizontalOverflow")) {
			var hor = style.Get<HorizontalWrapMode>("horizontalOverflow");
			tmp.enableWordWrapping = hor == HorizontalWrapMode.Wrap;
		}


	}
#endif

	static void ApplyTextStyle(Text txt, JsonObject style) {
		string language = Localization.language.ToString();

		if (style.ContainsKey("font")) { 
			if (style["font"].isString) { txt.font = Resources.Load<Font>(style.Get<string>("font")); }
			if (style["font"].isObject) {
				JsonObject languageFonts = style.Get<JsonObject>("font");
				if (languageFonts.ContainsKey(language)) {
					txt.font = Resources.Load<Font>(languageFonts.Get<string>(language) );
				} else {
					txt.font = Resources.Load<Font>(languageFonts.Get<string>("english"));
				}
			}
		}
		if (style.ContainsKey("fontStyle")) { txt.fontStyle = style.Get<FontStyle>("fontStyle"); }

		if (style.ContainsKey("fontSize")) { 
			bool scale = style.Extract("sizeScale", true);
			float screenScale = 1.0f * (scale ? scaleRatio : 1f);
			float languageScale = 1.0f;
			if (style.ContainsKey("fontScale")) {
				if (style["fontScale"].isNumber) { languageScale = style.Get<float>("fontScale"); }
				if (style["fontScale"].isObject) {
					JsonObject languageScales = style.Get<JsonObject>("fontScale");
					if (languageScales.ContainsKey(language)) {
						languageScale = languageScales.Get<float>(language);
					}
					//Debug.Log("Applying fontScale for " + language + " : " + languageScale + " : " + languageScales);
				}
			}

			//Debug.Log(txt.transform.GetRelativePath() + " Scales for " + language + " : " + languageScale + " * " + screenScale + " = " + languageScale*screenScale);
			float fontSize = (style.Get<float>("fontSize")) * languageScale * screenScale;
			txt.fontSize = (int)fontSize;
		}
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
