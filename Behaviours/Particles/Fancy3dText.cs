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

	public static Fancy3dText Create(string str, float timeOffset = 0f) { return Create(str, camPos, defaultSettings, Color.white); }
	public static Fancy3dText Create(string str, Vector3 pos, float timeOffset = 0f) { return Create(str, pos, defaultSettings, Color.white); }

	public static Fancy3dText Create(string str, TextDisplaySettings sets, float timeOffset = 0f) { return Create(str, camPos, sets, Color.white); }
	public static Fancy3dText Create(string str, Color col, float timeOffset = 0f) { return Create(str, camPos, defaultSettings, col); }
	public static Fancy3dText Create(string str, TextDisplaySettings sets, Color col, float timeOffset = 0f) { return Create(str, camPos, sets, col); }

	public static Fancy3dText Create(string str, Vector3 pos, TextDisplaySettings sets, float timeOffset = 0f) { return Create(str, pos, sets, Color.white); }
	public static Fancy3dText Create(string str, Vector3 pos, Color col, float timeOffset = 0f) { return Create(str, pos, defaultSettings, col); }
	public static Fancy3dText Create(string str, Vector3 pos, TextDisplaySettings sets, Color col, float timeOffset = 0f) {
		var ftext = Factory();
		ftext.settings = sets;
		ftext.transform.position = pos;
		ftext.text = str;
		ftext.color = col;
		ftext.timeout = timeOffset;
		return ftext;
	}


	public Fancy3dText NoFollow() { followCamera = false; return this; }

	public Fancy3dText Sound(AudioClip clip) { sound = clip; return this; }

	public AudioClip sound;

	public static float defaultDistance = 10f;
	public static TextDisplaySettings defaultSettings = TextDisplaySettings.sets3d;

	public string text = "Fancy3dText";

	public Color color = Color.white;
	public Vector3 velocity;
	public TextDisplaySettings settings = TextDisplaySettings.sets3d;
	
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
	public Vector3 gravity { get { return settings.gravity; } }
	///<summary> Wrapthrough to settings object </summary>
	public Vector3 animOffset { get { return settings.animOffset; } }
	///<summary> Wrapthrough to settings object </summary>
	public float animTime { get { return settings.animTime; } }
	///<summary> Wrapthrough to settings object </summary>
	public TextDisplaySettings.AnimMode animMode { get { return settings.animMode; } }
	///<summary> Wrapthrough to settings object </summary>
	public TextDisplaySettings.EaseMode easeMode { get { return settings.easeMode; } }

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
		float spacer = baseSize * spacing;
		float left = spacer * text.Length / -2f;

		for (int i = 0; i < text.Length; i++) {
			letters[i] = Instantiate<TextMeshPro>(baseText);
			letters[i].text = "" + text[i];
			letters[i].color = Color.clear;
			var t = letters[i].transform;
			t.SetParent(transform);
			t.localPosition = new Vector3(left + spacer * i, 0, 0);
		}


	}

	void Update() {
		
		timeout += Time.deltaTime;
		if (timeout >= lifetime) { Destroy(gameObject); return; }

		if (billboard && Camera.main != null) { 
			transform.LookAt(Camera.main.transform, -Physics.gravity);
			transform.Rotate(0, 180, 0);
		}
		velocity += gravity * Time.deltaTime;
		transform.position += transform.rotation * velocity * Time.deltaTime;

		float spacer = baseSize * spacing;
		float left = spacer * text.Length / -2f;
		for (int i = 0; i < letters.Length; i++) {
			var letter = letters[i];
			var t = letter.transform;

			
			t.localPosition = new Vector3(left + spacer * i, 0, 0);


			float fadePosition = (timeout - (i * delay)) / fadeTime;
			if (fadePosition <= 0) { continue; }
			if (fadePosition > 1) { fadePosition = 1; }

			Vector3 animPos = AnimationPosition(i);
			t.localPosition += animPos;

			float alpha = fadePosition;
			if (timeout > lifetime - fadeTime) {
				alpha = (lifetime - timeout) / fadeTime;
			}
			letter.color = color.Alpha(alpha);
			letter.fontSize = Mathf.Lerp(baseSize * sizeScale, baseSize, fadePosition);

		}

	}


	Vector3 AnimationPosition(int i) {
		if (animMode == TextDisplaySettings.AnimMode.None) { return Vector3.zero; }
		float animPosition = (timeout - (i * delay)) / animTime;
		if (animPosition <= 0 || animPosition >= 1) { return Vector3.zero; }
		
		if (easeMode == TextDisplaySettings.EaseMode.Out) { animPosition *= animPosition;}
		if (easeMode == TextDisplaySettings.EaseMode.In) { animPosition = 1f - ((1f-animPosition) * (1f - animPosition)); }
		
		if (animMode == TextDisplaySettings.AnimMode.Slide) {
			return animOffset * (1.0f - animPosition);
		}

		if (animMode == TextDisplaySettings.AnimMode.OShoot) {
			float pi = Mathf.PI;
			float x = animPosition;
			animPosition = Mathf.Cos(x * pi) * (pi-x*pi)/pi;
			return animOffset * animPosition;
		}
		
		if (animMode == TextDisplaySettings.AnimMode.Bounce) {

		}


		return Vector3.zero;
	}
	
}


#endif
