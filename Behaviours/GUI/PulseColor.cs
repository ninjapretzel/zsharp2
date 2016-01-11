using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PulseColor : MonoBehaviour {

	Text text;
	Image image;
	Renderer rend;

	public Color color1 = Color.gray;
	public Color color2 = Color.white;

	public float speed = 5f;
	public bool scaleWithTimeScale = false;


	void Awake() {
		text = GetComponent<Text>();
		image = GetComponent<Image>();

		rend = GetComponent<Renderer>();
	}

	public void GrabColor() {
		if (image != null) { color1 = image.color; }
		if (text != null) { color1 = text.color; }
		if (rend != null) { color1 = rend.material.color; }
		if (color1 == Color.white) { color2 = Color.gray; }
	}

	
	void Update() {
		float val = .5f + .5f * Mathf.Sin(speed * (scaleWithTimeScale ? Time.time : Time.unscaledTime) );

		Color c = Color.Lerp(color1, color2, val);

		if (text != null) { text.color = c; }
		if (image != null) { image.color = c; }
		if (rend != null) { rend.material.color = c; }

	}
	
}
