using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class FancyText {


	public class Display {
		public Vector2 position;
		public Vector2 velocity;
		public DisplaySettings settings;
		public float value;
		public string text;
		public Color color = Color.white;

		public float timeout = 0;

		public bool outlined { get { return settings.outlined; } }
		public float lifetime { get { return settings.lifetime; } }
		public float spacing { get { return settings.spacing; } }
		public float delay { get { return settings.delay; } }
		public float fadeTime { get { return settings.fadeTime; } }
		public float baseSize { get { return settings.baseSize; } }
		public float sizeScale { get { return settings.sizeScale; } }
		public Vector2 gravity { get { return settings.gravity; } }

		public Display(string str, Vector2 pos, DisplaySettings sets, Color col) {
			text = str;
			position = pos;
			settings = sets;
			velocity = settings.velocity;
			color = col;
			position.x -= text.Length * spacing / 2f;
		}

		public Display(float val, Vector2 pos, DisplaySettings sets, Color col) {
			text = val.ShortString(0);
			
			position = pos;
			settings = sets;
			velocity = settings.velocity;
			color = col;
			position.x -= text.Length * spacing / 2f;
		}

		public bool Update() { return Update(Time.deltaTime); }
		public bool Update(float time) {
			position += velocity * time;
			velocity += gravity * time;
			timeout += time;
			return timeout >= lifetime;
		}

		public void Draw() {
			Rect brush = RectUtils.Centered(position, Vector2.one * 200);
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

	[System.Serializable]
	public class DisplaySettings {
		public bool outlined = true;
		public float delay = .05f;
		public float fadeTime = .1f;
		public float lifetime = 3;
		public float baseSize = 14;
		public float sizeScale = 4;
		public float spacingRatio = .7f;
		public Vector2 baseVelocity = new Vector2(0, -500);
		public Vector2 randVelocity = new Vector2(10, 0);
		public Vector2 gravity = new Vector2(0, 50);
		public Vector2 velocity { get { return baseVelocity + randVelocity * Random.unit; } } 
		public float spacing { get { return baseSize * spacingRatio * screenRatio; } }
		
	}

	public GUISkin skin;
	public DisplaySettings settings;

	static float screenRatio;
	List<Display> displays;
	
	public FancyText() {
		displays = new List<Display>();
		settings = new DisplaySettings();
	}

	public void Add(float val) { Add(val, Screen.MiddleCenter(), settings, Color.white); }
	public void Add(float val, Vector2 position) { Add(val, position, settings, Color.white); }
	public void Add(float val, Vector2 position, Color col) { Add(val, position, settings, col); }
	public void Add(float val, Vector2 position, DisplaySettings sets) { Add(val, position, sets, Color.white); }

	public void Add(float val, Vector2 position, DisplaySettings sets, Color col) {
		Display d = new Display(val, position, sets, col);
		displays.Add(d);
	}

	public void Add(string str) { Add(str, Screen.MiddleCenter(), settings, Color.white); }
	public void Add(string str, Vector2 position) { Add(str, position, settings, Color.white); }
	public void Add(string str, Vector2 position, Color col) { Add(str, position, settings, col); }
	public void Add(string str, Vector2 position, DisplaySettings sets) { Add(str, position, sets, Color.white); }

	public void Add(string str, Vector2 position, DisplaySettings sets, Color col) {
		Display d = new Display(str, position, sets, col);
		displays.Add(d);
	}
	
	public void Update() {
		screenRatio = (Screen.height / 720f);
		for (int i = 0; i < displays.Count; i++) {
			if (displays[i].Update()) {
				displays.RemoveAt(i);
				i--;
			}
		}
	}

	public void Draw() {
		GUI.skin = skin;
		foreach (Display d in displays) {
			d.Draw();
		}
	}


}
