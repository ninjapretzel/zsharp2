using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary> Generic Text Display settings, for both 2d and 3d text objects.</summary>
[System.Serializable]
public class TextDisplaySettings {

	public enum AnimMode {
		None,
		Slide,
		OShoot,
		Bounce,
		Wobble,
	}
	public enum EaseMode {
		None, In, Out
	}

	/// <summary> Factory property that generates a basic 2d object, based off a screen size of 720p </summary>
	public static TextDisplaySettings sets2d { get { return new TextDisplaySettings(); } }
	public static TextDisplaySettings sets3d { 
		get { return new TextDisplaySettings() {
			fixedSpacing = true,
			spacingRatio = .35f,
			baseSize = 8f,
			sizeScale = 4f,
			baseVelocity =	new Vector3(0f, 3f, 0f), 
			randVelocity =	new Vector3(1f, 0f, 1f), 
			gravity =		new Vector3(0f, -3f, 0f) }; 
		}
	}

	///<summary> Is the text outlined? (2d only) </summary>
	public bool outlined = true;
	///<summary> Delay for each character to appear </summary>
	public float delay = .03f;
	///<summary> Time of fade in/out for all text </summary>
	public float fadeTime = .2f;
	///<summary> Total lifetime of text </summary>
	public float lifetime = 1.5f;
	///<summary> Base size of text (points) at 720p, or raw size for 3d text </summary>
	public float baseSize = 14f;
	///<summary> Size of text, as it appears and fades in </summary>
	public float sizeScale = 4f;
	///<summary> Spacing ratio of text based on text size </summary>
	public float spacingRatio = .7f;
	///<summary> Velocity of new text </summary>
	public Vector3 baseVelocity = new Vector3(0, -500, 0);
	///<summary> Randomness to velocity of new text </summary>
	public Vector3 randVelocity = new Vector3(30, 0, 30);
	///<summary> Gravity of text </summary>
	public Vector3 gravity = new Vector3(0, 500, 0);
	///<summary> Get a random velocity </summary>
	public Vector3 velocity { get { return baseVelocity + randVelocity * Random.unit; } }

	/// <summary> Time it takes a single letter to slide into place </summary>
	public float animTime = 0f;
	/// <summary> Offset </summary>
	public Vector3 animOffset = Vector3.zero;

	/// <summary> Extra animation to add, if any. </summary>
	public AnimMode animMode = AnimMode.None;

	/// <summary> Easing to apply to animation, if any. </summary>
	public EaseMode easeMode = EaseMode.None;



	/// <summary> Is spacing fixed to a value, or dependant on screen size? </summary>
	public bool fixedSpacing = false;

	///<summary> Get adjusted spacing based on screen size </summary>
	public float spacing { get { return fixedSpacing ? spacingRatio : (baseSize * spacingRatio * ((float)Screen.height) / 720f); } }
		
}

[System.Serializable]
///<summary> Fancy text for Unity's Legacy GUI</summary>
public class FancyText {

	///<summary> Information for a single text display. </summary>
	public class Display {
		///<summary> Position on the screen </summary>
		public Vector2 position;
		///<summary> Speed on the screen </summary>
		public Vector2 velocity;
		///<summary> Settings object (Describes outline, gravity, fade, spacing, etc ) </summary>
		public TextDisplaySettings settings;

		///<summary> Value to display </summary>
		public float value;
		///<summary> Text to display </summary>
		public string text;
		///<summary> Color of display </summary>
		public Color color = Color.white;

		///<summary> Time this display has lived for </summary>
		public float timeout = 0;

		///<summary> Wrapthrough to settings object </summary>
		public bool outlined { get { return settings.outlined; } }
		///<summary> Wrapthrough to settings object </summary>
		public float lifetime { get { return settings.lifetime; } }
		///<summary> Wrapthrough to settings object </summary>
		public float spacing { get { return settings.spacing; } }
		///<summary> Wrapthrough to settings object </summary>
		public float delay { get { return settings.delay; } }
		///<summary> Wrapthrough to settings object </summary>
		public float fadeTime { get { return settings.fadeTime; } }
		///<summary> Wrapthrough to settings object </summary>
		public float baseSize { get { return settings.baseSize; } }
		///<summary> Wrapthrough to settings object </summary>
		public float sizeScale { get { return settings.sizeScale; } }
		///<summary> Wrapthrough to settings object </summary>
		public Vector2 gravity { get { return settings.gravity; } }

		///<summary> Constructor for displaying a text string </summary>
		public Display(string str, Vector2 pos, TextDisplaySettings sets, Color col, float timeOffset = 0f) {
			text = str;
			position = pos;
			settings = sets;
			velocity = settings.velocity;
			color = col;
			position.x -= text.Length * spacing / 2f;
			timeout = timeOffset;
		}

		///<summary> Constructor for displaying a value </summary>
		public Display(float val, Vector2 pos, TextDisplaySettings sets, Color col, float timeOffset = 0f) {
			text = val.ShortString();
			
			position = pos;
			settings = sets;
			velocity = settings.velocity;
			color = col;
			position.x -= text.Length * spacing / 2f;
			timeout = timeOffset;
		}

		///<summary> Update by default scaled Time.deltaTime </summary>
		///<returns> True, if this display is finished. </returns>
		public bool Update() { return Update(Time.deltaTime); }
		///<summary> Update by a given amount of time </summary>
		///<param name="time"> Time to elapse </param>
		///<returns> True, if this Display is finished. </returns>
		public bool Update(float time) {
			timeout += time;
			if (timeout < 0) { return false; }
			position += velocity * time * screenRatio;
			velocity += gravity * time;
			return timeout >= lifetime;
		}

		///<summary> Draw this display on the screen. </summary>
		public void Draw() {
			Rect brush = RectUtils.Centered(position, Vector2.one * 200 * scale);
			for (int i = 0; i < text.Length; i++) {
				float fadePosition = (timeout - (i * delay)) / fadeTime;
				if (fadePosition <= 0) { return; }
				if (fadePosition > 1) { fadePosition = 1; }

				GUI.skin.label.fontSize = (int) (Mathf.Lerp(baseSize * sizeScale, baseSize, fadePosition) * screenRatio);

				float alpha = fadePosition;
				if (timeout > lifetime - fadeTime) {
					alpha = (lifetime - timeout) / fadeTime;
				}
				
				GUI.color = color.Alpha(alpha);
				
				if (outlined) { GUI.QOutlinedLabel(brush, ""+text[i]); }
				else { GUI.Label(brush, ""+text[i]); }

				brush.x += spacing;

			}

		}
	}

	///<summary> Skin to use to display text </summary>
	public GUISkin skin;
	///<summary> Settings to display text with </summary>
	public TextDisplaySettings settings;

	///<summary> Scale of screen, based on vertical resolution of 720p </summary>
	public static float scale { get { return ((float)Screen.height) / 720f; } }
	///<summary> cache of the screen ratio </summary>
	static float screenRatio;
	///<summary> All live texts </summary>
	List<Display> displays;

	///<summary> Constructor </summary>
	public FancyText() {
		displays = new List<Display>();
		settings = TextDisplaySettings.sets2d;
	}

	///<summary> Make a new text with a value. Defaults to center of screen. </summary>
	public void Add(float val, float timeOffset = 0f) { Add(val, Screen.MiddleCenter(), settings, Color.white, timeOffset); }
	///<summary> Make a new text with a value at a position. </summary>
	public void Add(float val, Vector2 position, float timeOffset = 0f) { Add(val, position, settings, Color.white, timeOffset); }
	///<summary> Make a new text with a value at a position, with a given color </summary>
	public void Add(float val, Vector2 position, Color col, float timeOffset = 0f) { Add(val, position, settings, col, timeOffset); }
	///<summary> Make a new text with a value at a position, completely overriding its settings. </summary>
	public void Add(float val, Vector2 position, TextDisplaySettings sets, float timeOffset = 0f) { Add(val, position, sets, Color.white, timeOffset); }
	///<summary> Make a new text with a value at a position, overriding its settings and color. </summary>
	public void Add(float val, Vector2 position, TextDisplaySettings sets, Color col, float timeOffset = 0f) {
		Display d = new Display(val, position, sets, col, timeOffset);
		displays.Add(d);
	}

	
	///<summary> Add a text with a given string to the center of the screen. </summary>
	public void Add(string str, float timeOffset = 0f) { Add(str, Screen.MiddleCenter(), settings, Color.white, timeOffset); }
	///<summary> Add a text with a given string at a position </summary>
	public void Add(string str, Vector2 position, float timeOffset = 0f) { Add(str, position, settings, Color.white, timeOffset); }
	///<summary> Add a text with a given string at a position, with a given color. </summary>
	public void Add(string str, Vector2 position, Color col, float timeOffset = 0f) { Add(str, position, settings, col, timeOffset); }
	///<summary> Add a text with a given string at a position, completely overriding its settings. </summary>
	public void Add(string str, Vector2 position, TextDisplaySettings sets, float timeOffset = 0f) { Add(str, position, sets, Color.white, timeOffset); }
	///<summary> Add a text with a given string at a position, overriding its settings and color. </summary>
	public void Add(string str, Vector2 position, TextDisplaySettings sets, Color col, float timeOffset = 0f) {
		Display d = new Display(str, position, sets, col, timeOffset);
		displays.Add(d);
	}

	///<summary> Update all live texts, purge any 'dead' ones. </summary>
	public void Update() {
		screenRatio = scale;
		for (int i = 0; i < displays.Count; i++) {
			if (displays[i].Update()) {
				displays.RemoveAt(i);
				i--;
			}
		}
	}

	///<summary> Draw all live texts. </summary>
	public void Draw() {
		GUI.skin = skin;
		foreach (Display d in displays) {
			d.Draw();
		}
	}


}
