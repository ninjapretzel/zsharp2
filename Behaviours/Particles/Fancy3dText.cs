using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
#if TMPRO
using TMPro;

public class Fancy3dText : MonoBehaviour {
	public static Fancy3dText Factory() {
		GameObject obj = new GameObject("Fancy3dText");
		var ftext = obj.AddComponent<Fancy3dText>();
		return ftext;
	}

	public static Vector3 camPos { get { return (Camera.main != null) ? Camera.main.transform.position + Camera.main.transform.forward * defaultDistance : Vector3.zero; } }

	public static Fancy3dText Create(string str) { return Create(str, camPos, defaultSettings, Color.white); }
	public static Fancy3dText Create(string str, Vector3 pos) { return Create(str, pos, defaultSettings, Color.white); }

	public static Fancy3dText Create(string str, DisplaySettings sets) { return Create(str, camPos, sets, Color.white); }
	public static Fancy3dText Create(string str, Color col) { return Create(str, camPos, defaultSettings, col); }
	public static Fancy3dText Create(string str, DisplaySettings sets, Color col) { return Create(str, camPos, sets, col); }

	public static Fancy3dText Create(string str, Vector3 pos, DisplaySettings sets) { return Create(str, pos, sets, Color.white); }
	public static Fancy3dText Create(string str, Vector3 pos, Color col) { return Create(str, pos, defaultSettings, col); }
	public static Fancy3dText Create(string str, Vector3 pos, DisplaySettings sets, Color col) {
		var ftext = Factory();
		ftext.settings = sets;
		ftext.transform.position = pos;
		ftext.text = str;
		ftext.color = col;
		return ftext;
	}




	public static float defaultDistance = 10f;
	public static DisplaySettings defaultSettings = new DisplaySettings();


	[Serializable] public class DisplaySettings {
		
		/// <summary> Delay for each character to appear </summary>
		public float delay = .05f;
		/// <summary> Time of fade for all text </summary>
		public float fadeTime = .3f;
		/// <summary> Total lifetime of text </summary>
		public float lifetime = 1.5f;
		/// <summary> Size of text in the world (scales) </summary>
		public float baseSize = 8f;
		/// <summary> Relative size of text when it appears </summary>
		public float sizeScale = 4f;
		/// <summary> Distance between each letter</summary>
		public float spacing = .35f;

		/// <summary> Velocity of a new text </summary>
		public Vector3 baseVelocity = new Vector3(0, 3, 0);
		/// <summary> Randomness to velocity of new text </summary>
		public Vector3 randVelocity = new Vector3(1, 0, 0);
		/// <summary> Gravity of text </summary>
		public float gravity = 3;

		/// <summary> Get a random velocity based on baseVelocity and randVelocity </summary>
		public Vector3 velocity { get { return baseVelocity + randVelocity * Random.unit; } }

	}

	public string text = "Fancy3dText";

	public Color color = Color.white;
	public Vector3 velocity;
	public DisplaySettings settings;
	
	public bool billboard = true;
	public bool followCamera = true;

	public float timeout;

	public TextMeshPro baseText;

	public TextMeshPro[] letters;

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
	public float gravity { get { return settings.gravity; } }

	void Start() {
		if (baseText == null) { baseText = Resources.Load<TextMeshPro>("Fancy3dText"); }
		if (baseText == null) { 
			Debug.LogWarning("Fancy3dText: 'baseText' must be assigned, or default 'Fancy3dText' TextMeshPro object must be placed in /Resources/");
			return;
		}
		if (followCamera && Camera.main != null) { 
			transform.SetParent(Camera.main.transform);
		}
		velocity = settings.velocity;
		letters = new TextMeshPro[text.Length];
		float left = spacing * text.Length / -2f;

		for (int i = 0; i < text.Length; i++) {
			letters[i] = Instantiate<TextMeshPro>(baseText);
			letters[i].text = ""+text[i];
			letters[i].color = Color.clear;
			var t = letters[i].transform;
			t.SetParent(transform);
			t.localPosition = new Vector3(left + spacing * i, 0, 0);
		}


	}

	void Update() {
		
		timeout += Time.deltaTime;

		if (timeout >= lifetime) { Destroy(gameObject); return; }

		if (billboard && Camera.main != null) { 
			transform.LookAt(Camera.main.transform, -Physics.gravity);
			transform.Rotate(0, 180, 0);
		}
		velocity.y -= gravity * Time.deltaTime;
		transform.position += transform.rotation * velocity * Time.deltaTime;

		float left = spacing * text.Length / -2f;
		for (int i = 0; i < letters.Length; i++) {
			var letter = letters[i];
			var t = letter.transform;
			t.localPosition = new Vector3(left + spacing * i, 0, 0);
			float fadePosition = (timeout - (i * delay)) / fadeTime;
			if (fadePosition <= 0) { continue; }
			if (fadePosition > 1) { fadePosition = 1; }


			float alpha = fadePosition;
			if (timeout > lifetime - fadeTime) {
				alpha = (lifetime - timeout) / fadeTime;
			}
			letter.color = color.Alpha(alpha);
			letter.fontSize = Mathf.Lerp(baseSize * sizeScale, baseSize, fadePosition);

		}

	}
	
}


#endif
