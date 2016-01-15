using UnityEngine;
using System.Collections;
using System.Collections.Generic;

///<summary> Fancy text for Unity's Legacy GUI</summary>
[System.Serializable]
public class FancyText {

	///<summary> Information for a single text display. </summary>
	public class Display {
		///<summary> Position on the screen </summary>
		public Vector2 position;
		///<summary> Speed on the screen </summary>
		public Vector2 velocity;
		///<summary> Settings object (Describes outline, gravity, fade, spacing, etc ) </summary>
		public DisplaySettings settings;

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
		public Display(string str, Vector2 pos, DisplaySettings sets, Color col) {
			text = str;
			position = pos;
			settings = sets;
			velocity = settings.velocity;
			color = col;
			position.x -= text.Length * spacing / 2f;
		}

		///<summary> Constructor for displaying a value </summary>
		public Display(float val, Vector2 pos, DisplaySettings sets, Color col) {
			text = val.ShortString();
			
			position = pos;
			settings = sets;
			velocity = settings.velocity;
			color = col;
			position.x -= text.Length * spacing / 2f;
		}

		///<summary> Update by deltaTime </summary>
		public bool Update() { return Update(Time.deltaTime); }
		///<summary> Update by a given amount of time </summary>
		public bool Update(float time) {
			position += velocity * time * screenRatio;
			velocity += gravity * time;
			timeout += time;
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

	///<summary> Settings to apply to a single Display object </summary>
	[System.Serializable]
	public class DisplaySettings {
		///<summary> Is the text outlined? </summary>
		public bool outlined = true;
		///<summary> Delay for each character ot appear </summary>
		public float delay = .05f;
		///<summary> Time of fade for all text </summary>
		public float fadeTime = .1f;
		///<summary> Total lifetime of text </summary>
		public float lifetime = 3;
		///<summary> Base size of text (points) at 720p </summary>
		public float baseSize = 14;
		///<summary> Size of text as it appears </summary>
		public float sizeScale = 4;
		///<summary> Spacing ratio of text based on its size on screen </summary>
		public float spacingRatio = .7f;
		///<summary> Velocity of new text </summary>
		public Vector2 baseVelocity = new Vector2(0, -500);
		///<summary> Randomness to velocity of new text </summary>
		public Vector2 randVelocity = new Vector2(10, 0);
		///<summary> Gravity of text </summary>
		public Vector2 gravity = new Vector2(0, 50);
		///<summary> Get a random velocity </summary>
		public Vector2 velocity { get { return baseVelocity + randVelocity * Random.unit; } }
		///<summary> Get adjusted spacing based on screen size </summary>
		public float spacing { get { return baseSize * spacingRatio * screenRatio; } }
		
	}

	///<summary> Skin to use to display text </summary>
	public GUISkin skin;
	///<summary> Settings to display text with </summary>
	public DisplaySettings settings;

	///<summary> Scale of screen, based on vertical resolution of 720p </summary>
	public static float scale { get { return ((float)Screen.height) / 720f; } }
	///<summary> cache of the screen ratio </summary>
	static float screenRatio;
	///<summary> All live texts </summary>
	List<Display> displays;

	///<summary> Constructor </summary>
	public FancyText() {
		displays = new List<Display>();
		settings = new DisplaySettings();
	}

	///<summary> Make a new text with a value. Defaults to center of screen. </summary>
	public void Add(float val) { Add(val, Screen.MiddleCenter(), settings, Color.white); }
	///<summary> Make a new text with a value at a position. </summary>
	public void Add(float val, Vector2 position) { Add(val, position, settings, Color.white); }
	///<summary> Make a new text with a value at a position, with a given color </summary>
	public void Add(float val, Vector2 position, Color col) { Add(val, position, settings, col); }
	///<summary> Make a new text with a value at a position, completely overriding its settings. </summary>
	public void Add(float val, Vector2 position, DisplaySettings sets) { Add(val, position, sets, Color.white); }
	///<summary> Make a new text with a value at a position, overriding its settings and color. </summary>
	public void Add(float val, Vector2 position, DisplaySettings sets, Color col) {
		Display d = new Display(val, position, sets, col);
		displays.Add(d);
	}

	
	///<summary> Add a text with a given string to the center of the screen. </summary>
	public void Add(string str) { Add(str, Screen.MiddleCenter(), settings, Color.white); }
	///<summary> Add a text with a given string at a position </summary>
	public void Add(string str, Vector2 position) { Add(str, position, settings, Color.white); }
	///<summary> Add a text with a given string at a position, with a given color. </summary>
	public void Add(string str, Vector2 position, Color col) { Add(str, position, settings, col); }
	///<summary> Add a text with a given string at a position, completely overriding its settings. </summary>
	public void Add(string str, Vector2 position, DisplaySettings sets) { Add(str, position, sets, Color.white); }
	///<summary> Add a text with a given string at a position, overriding its settings and color. </summary>
	public void Add(string str, Vector2 position, DisplaySettings sets, Color col) {
		Display d = new Display(str, position, sets, col);
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
