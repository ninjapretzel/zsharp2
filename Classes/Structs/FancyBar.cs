using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class FancyBar {

	public static Texture2D _noise { get { return Resources.Load<Texture2D>("noise"); } }
	public static Texture2D _bar { get { return Resources.Load<Texture2D>("bar"); } } 


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

#if XtoJSON
	public FancyBar(JsonObject data) {
		Json.ReflectInto(data, this);
		noiseA = Resources.Load<Texture2D>(data.GetString("noiseA"));
		noiseB = Resources.Load<Texture2D>(data.GetString("noiseB"));
		bar = Resources.Load<Texture2D>(data.GetString("bar"));
	}
#endif

	public FancyBar(FancyBar s) { 
		bar = s.bar;
		noiseA = s.noiseA;
		noiseB = s.noiseB;

		timeScale = s.timeScale;
		timeOffset = s.timeOffset;
		padding = s.padding;
		hueShiftA = s.hueShiftA;
		hueShiftB = s.hueShiftB;

		leftToRight = s.leftToRight;

		darkening = s.darkening;
		alpha = s.alpha;

		scaleA = s.scaleA;
		scaleB = s.scaleB;
	}

	public FancyBar Clone() { return new FancyBar(this); }

	public void Draw(Rect pos, float fill) {
		float time = Time.time + timeOffset;
		fill = fill.Clamp01();

		Rect padded = pos.Pad(padding);
		float ratio = pos.width / pos.height;

		GUI.PushColor(Color.black);
		GUI.DrawTexture(padded, GUI.pixel);
		GUI.PopColor();
		
		Rect filled = leftToRight ? pos.Left(fill) : pos.Right(fill);
		Rect empty = !leftToRight ? pos.Left(1-fill) : pos.Right(1-fill);

		Color baseColor = GUI.color.Alpha(1);
		Color colorA = baseColor.ShiftHue( hueShiftA).Alpha(alpha);
		Color colorB = baseColor.ShiftHue( hueShiftB).Alpha(alpha);

		Rect coordsA = new Rect(scaleA.x * time, scaleA.y * Mathf.Sin(time * scaleA.width), ratio * scaleA.height, scaleA.height);
		Rect coordsAfill = leftToRight ? coordsA.Left(fill) : coordsA.Right(fill);
		Rect coordsAempty = !leftToRight ? coordsA.Left(1 - fill) : coordsA.Right(1 - fill);

		Rect coordsB = new Rect(scaleB.x * time, scaleB.y * Mathf.Sin(time * scaleB.width), ratio * scaleB.height, scaleB.height);
		Rect coordsBfill = leftToRight ? coordsB.Left(fill) : coordsB.Right(fill);
		Rect coordsBempty = !leftToRight ? coordsB.Left(1 - fill) : coordsB.Right(1 - fill);

		GUI.PushColor(baseColor.MultRGB(darkening.Clamp01()));
		GUI.DrawTexture(empty, bar);
		GUI.PopColor();
		GUI.DrawTexture(filled, bar);

		GUI.PushColor(colorA);
		GUI.DrawTextureWithTexCoords(filled, noiseA, coordsAfill);
		GUI.PopColor();

		GUI.PushColor(colorA.MultRGB(darkening));
		GUI.DrawTextureWithTexCoords(empty, noiseA, coordsAempty);
		GUI.PopColor();
		
		GUI.PushColor(colorB);
		GUI.DrawTextureWithTexCoords(filled, noiseB, coordsBfill);
		GUI.PopColor();

		GUI.PushColor(colorB.MultRGB(darkening));
		GUI.DrawTextureWithTexCoords(empty, noiseB, coordsBempty);
		GUI.PopColor();

	}


	
}
