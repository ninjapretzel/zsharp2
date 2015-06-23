using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class FancyBar {

	public static Texture2D _noise;
	public static Texture2D _bar;

	public static bool loaded = Load();
	public static bool Load() {
		_noise = Resources.Load<Texture2D>("noise");
		_bar = Resources.Load<Texture2D>("bar");
		return true;
	}


	public Texture2D noiseA;
	public Texture2D noiseB;
	public Texture2D bar;

	public float timeScale = 1f;
	public float timeOffset = 0;
	public float padding = 2.0f;
	public float hueShiftA = .1f;
	public float hueShiftB = .1f;

	public bool leftToRight = true;

	public float darkening = .5f;
	public float alpha = .25f;

	public Rect scaleA = new Rect(-.1f, .3f, .1f, .1f);
	public Rect scaleB = new Rect(-.2f, .6f, .2f, .2f);

	public FancyBar() {
		bar = _bar;
		noiseA = _noise;
		noiseB = _noise;
	}

	public FancyBar(Texture2D back, Texture2D nA, Texture2D nB) {
		bar = back;
		noiseA = nA;
		noiseB = nB;
	}

	public void Draw(Rect pos, float fill) {
		float time = Time.time + timeOffset;
		fill = fill.Clamp01();

		Rect padded = pos.Pad(padding);
		float ratio = pos.width / pos.height;

		GUI.PushColor(Color.black);
		GUI.DrawTexture(padded, GUI.pixel);
		GUI.PopColor();

		
		Rect filled = leftToRight ? pos.Left(fill) : pos.Right(fill);

		Color baseColor = GUI.color.Alpha(1);
		Color colorA = baseColor.ShiftHue( hueShiftA).Alpha(alpha);
		Color colorB = baseColor.ShiftHue( hueShiftB).Alpha(alpha);

		Rect coordsA = new Rect(scaleA.x * time, scaleA.y * Mathf.Sin(time * scaleA.width), ratio * scaleA.height, scaleA.height);
		Rect coordsB = new Rect(scaleB.x * time, scaleB.y * Mathf.Sin(time * scaleB.width), ratio * scaleB.height, scaleB.height);

		GUI.PushColor(baseColor.MultRGB(darkening.Clamp01()));
		GUI.DrawTexture(pos, bar);
		GUI.PopColor();

		GUI.DrawTexture(filled, bar);


		
		GUI.PushColor(colorA);
		GUI.DrawTextureWithTexCoords(pos, noiseA, coordsA);
		GUI.PopColor();
		
		GUI.PushColor(colorB);
		GUI.DrawTextureWithTexCoords(pos, noiseB, coordsB);
		GUI.PopColor();

	}


	
}
