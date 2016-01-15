using UnityEngine;
using System.Collections;
using System.Collections.Generic;

///<summary> Fancy version of Bar. Uses Unity's Legacy GUI</summary>
public class FancyBar {

	///<summary> Default noise texture </summary>
	public static Texture2D _noise { get { return Resources.Load<Texture2D>("noise"); } }
	///<summary> Default bar gradient texture </summary>
	public static Texture2D _bar { get { return Resources.Load<Texture2D>("bar"); } }

	///<summary> Primary noise pattern </summary>
	public Texture2D noiseA;
	///<summary> Secondary noise pattern </summary>
	public Texture2D noiseB;
	///<summary> Bar gradient pattern </summary>
	public Texture2D bar;

	///<summary> Scale of Time.time on animation </summary>
	public float timeScale = 1f;
	///<summary> Offset from time position </summary>
	public float timeOffset = 0;
	///<summary> Padding of edges </summary>
	public float padding = 2.0f;
	///<summary> Hue Shift for primary noise pattern </summary>
	public float hueShiftA = .1f;
	///<summary> Hue Shift for secondary noise pattern </summary>
	public float hueShiftB = .1f;

	///<summary> Display bar left-to-right / right-to-left </summary>
	public bool leftToRight = true;

	///<summary> How dark is the background of the bar compared to the foreground? </summary>
	public float darkening = .5f;
	///<summary> Transparency of each layer </summary>
	public float alpha = .25f;

	///<summary> Scaling/repeat for primary noise texture </summary>
	public Rect scaleA = new Rect(-.1f, .3f, .1f, .1f);
	///<summary> Scaling/repeat for secondary noise texture </summary>
	public Rect scaleB = new Rect(-.2f, .6f, .2f, .2f);
	
	///<summary> Default constructor </summary>
	public FancyBar() {
		bar = _bar;
		noiseA = _noise;
		noiseB = _noise;
	}

	///<summary> Constructor that takes Texutres </summary>
	public FancyBar(Texture2D back, Texture2D nA, Texture2D nB) {
		bar = back;
		noiseA = nA;
		noiseB = nB;
	}

#if XtoJSON
	public FancyBar(JsonObject data) {
		Json.ReflectInto(data, this);
		noiseA = Resources.Load<Texture2D>(data.Get<string>("noiseA"));
		noiseB = Resources.Load<Texture2D>(data.Get<string>("noiseB"));
		bar = Resources.Load<Texture2D>(data.Get<string>("bar"));
		scaleA = GetRect(data.Get<JsonObject>("scaleA"));
		scaleB = GetRect(data.Get<JsonObject>("scaleB"));
	}

	///<summary> Get a Rectangle from a JsonObject </summary>
	static Rect GetRect(JsonObject obj) {
		Rect r = new Rect();
		r.x = obj.Extract<float>("x", 0);
		r.y = obj.Extract<float>("y", 0);
		r.width = obj.Extract<float>("width", 1);
		r.height = obj.Extract<float>("height", 1);
		return r;
	}

#endif

	///<summary> Copy values from a source Object </summary>
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
	///<summary> Clone this FancyBar </summary>
	public FancyBar Clone() { return new FancyBar(this); }

	///<summary> Draw this FancyBar in a given position on the screen, with a given fill.</summary>
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
