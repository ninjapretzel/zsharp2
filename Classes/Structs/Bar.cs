using UnityEngine;
using System.Collections;
using System.Collections.Generic;



//Class to represent a bar on the screen at some absolute position.
//Mostly obsolete, but useful for testing.
[System.Serializable]
public class Bar {
	
	public enum Mode { Normal, Fixed, Repeat, Icons }
	
	public Mode mode = Mode.Normal;
	public Texture2D pixel { get { return Resources.Load<Texture2D>("pixel"); } }
	
	//Standard textures
	public Texture2D frontGraphic;
	public Texture2D backGraphic;
	
	//Standard colors
	public Color frontColor = Color.green;
	public Color backColor = Color.black;
	
	
	public Rect area;
	public Rect repeat;
	
	//Padding for 'back'
	public float padding;
	
	#region CONSTRUCTORS
	public Bar() {
		Init();
	}
	
	
	public Bar(Rect a, float pad = 2f) {
		Init();
		area = a;
		padding = pad;
	}
	
	public Bar(Texture2D front, Texture2D back, float pad = 2f) {
		frontGraphic = front;
		backGraphic = back;
		padding = pad;
	}
	
	public Bar(Texture2D front, Texture2D back, Color frontC, float pad = 2f) {
		frontGraphic = front;
		backGraphic = back;
		frontColor = frontC;
		padding = pad;
	}
	
	public Bar(Texture2D front, Texture2D back, Color frontC, Color backC, float pad = 2f) {
		frontGraphic = front;
		backGraphic = back;
		frontColor = frontC;
		backColor = backC;
		padding = pad;
	}
	
	public Bar(Rect a, Texture2D front, Texture2D back, float pad = 2f) {
		area = a;
		frontGraphic = front;
		backGraphic = back;
		padding = pad;
	}
	
	public Bar(Rect a, Texture2D front, Texture2D back, Color frontC, float pad = 2f) {
		area = a;
		frontGraphic = front;
		backGraphic = back;
		frontColor = frontC;
		padding = pad;
	}
	
	
	public Bar(Rect a, Texture2D front, Texture2D back, Color frontC, Color backC, float pad = 2f) {
		area = a;
		frontGraphic = front;
		backGraphic = back;
		frontColor = frontC;
		backColor = backC;
		padding = pad;
	}
	
	void Init() {
		area = Screen.Bottom(.1f);
		repeat = new Rect(0, 0, 1, 1);
		frontGraphic = pixel;
		backGraphic = pixel;
	}
	
	#endregion
	
	
	public void Draw(Rect area, float fill) {
		if (mode == Mode.Normal) { DrawNormal(area, fill); }
		else if (mode == Mode.Fixed) { DrawFixed(area, fill); }
		else if (mode == Mode.Repeat) { DrawRepeat(area, fill); }
	}
	
	public void Draw(float fill) {
		if (mode == Mode.Normal) { DrawNormal(fill); }
		else if (mode == Mode.Fixed) { DrawFixed(fill); }
		else if (mode == Mode.Repeat) { DrawRepeat(fill); }
	}
	
	//Normal draw method.
	//Paints the background, then the foreground over it.
	//Textures are stretched to fit.
	public void DrawNormal(float fill) { DrawNormal(area, fill); }
	public void DrawNormal(Rect area, float fill) {
		Rect brush = area.Pad(padding);
		float p = Mathf.Clamp01(fill);
		
		GUI.color = backColor;
		GUI.DrawTexture(brush, backGraphic);
		
		brush = brush.Trim(padding);
		if (area.width > area.height) { brush.width *= p; }
		else { 
			brush.y += area.height * (1.0f - p);
			brush.height *= p;
		}
		GUI.color = frontColor;
		GUI.DrawTexture(brush, frontGraphic);
	}
	
	//Draws the stuff with 'fixed' texture positions
	//The textures won't move relative to the left edges of the rectangles.
	//Draws the foreground first, then paints the background over it.
	public void DrawFixed(float fill) { DrawFixed(area, fill); }
	public void DrawFixed(Rect area, float fill) {
		Rect brush = area;//.Pad(padding);
		float p = Mathf.Clamp01(fill);
		
		
		GUI.color = frontColor;
		GUI.DrawTexture(brush, frontGraphic);
		
		if (area.width > area.height) {
			brush.x += area.width * p;
			brush.width *= p;
		} else { brush.height *= (1.0f-p); }
		GUI.color = backColor;
		GUI.DrawTexture(brush, backGraphic);
	}
	
	public void DrawRepeat(float fill) { DrawRepeat(area, fill); }
	public void DrawRepeat(Rect area, float fill) {
		Rect brush = area.Pad(padding);
		float p = Mathf.Clamp01(fill);
		
		
		GUI.color = backColor;
		GUI.DrawTextureWithTexCoords(brush, backGraphic, repeat);
		brush = brush.Trim(padding);
		
		Rect filled = brush;
		Rect filledReps = repeat;
		Rect empty = brush;
		Rect emptyReps = repeat;
		
		if (area.width > area.height) {
			filled = filled.UpperLeft(p, 1);
			filledReps = filledReps.UpperLeft(p, 1);
			empty = empty.UpperRight(1.0f-p, 1);
			emptyReps = emptyReps.UpperRight(1.0f-p, 1);
		} else {
			filled = filled.BottomLeft(1, p);
			filledReps = filledReps.UpperLeft(1, p);
			empty = empty.UpperLeft(1, 1.0f-p);
			emptyReps = emptyReps.BottomLeft(1, 1.0f-p);
		}
		
		GUI.color = backColor;
		GUI.DrawTextureWithTexCoords(empty, backGraphic, emptyReps);
		GUI.color = frontColor;
		GUI.DrawTextureWithTexCoords(filled, frontGraphic, filledReps);
	}
	
	public void DrawIcons(float fill) { DrawIcons(area, fill); }
	public void DrawIcons(Rect area, float fill) {
		Bars.graphic = frontGraphic;
		Bars.vertical = frontGraphic;
		Bars.Draw(area, repeat, fill, frontColor, backColor);
	}
	
	
	
}