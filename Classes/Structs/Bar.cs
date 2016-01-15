using UnityEngine;
using System.Collections;
using System.Collections.Generic;



/// <summary> Class to represent a bar on the screen at some absolute position.
/// Mostly obsolete, but useful for testing.</summary>
[System.Serializable]
public class Bar {
	/// <summary> Ways to show a Bar</summary>
	public enum Mode { Normal, Fixed, Repeat, Icons }

	/// <summary> Mode of this bar </summary>
	public Mode mode = Mode.Normal;
	/// <summary> small texture with only white pixels</summary>
	public static Texture2D pixel { get { return Texture2D.whiteTexture; } }

	/// <summary> Graphic to display on the 'foreground' (filled) </summary>
	public Texture2D frontGraphic;
	/// <summary> Graphic to display on the 'background' (empty) </summary>
	public Texture2D backGraphic;

	/// <summary> Color of foreground </summary>
	public Color frontColor = Color.green;
	/// <summary> Color of background </summary>
	public Color backColor = Color.black;

	/// <summary> Number of repetitions horizontally/vertically for Icon mode </summary>
	public Vector2 iconRepeat {
		get { return new Vector2(repeat.width, repeat.height); }
		set { repeat.width = value.x; repeat.height = value.y; }
	}
	/// <summary> Normalized area on the screen to display in. </summary>
	public Rect normalizedArea;

	/// <summary> Pixel area on the screen to display in. </summary>
	public Rect area {
		get { return normalizedArea.Denormalized(); }
	}
	/// <summary> Repeat rectangle for Repeat/Icons modes. </summary>
	public Rect repeat;

	/// <summary> Padding for background texture</summary>
	public float padding;
	
	#region CONSTRUCTORS
	public Bar() {
		Init();
	}
	
	
	public Bar(Rect a, float pad = 2f) {
		Init();
		normalizedArea = a;
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
		normalizedArea = a;
		frontGraphic = front;
		backGraphic = back;
		padding = pad;
	}
	
	public Bar(Rect a, Texture2D front, Texture2D back, Color frontC, float pad = 2f) {
		normalizedArea = a;
		frontGraphic = front;
		backGraphic = back;
		frontColor = frontC;
		padding = pad;
	}
	
	
	public Bar(Rect a, Texture2D front, Texture2D back, Color frontC, Color backC, float pad = 2f) {
		normalizedArea = a;
		frontGraphic = front;
		backGraphic = back;
		frontColor = frontC;
		backColor = backC;
		padding = pad;
	}
	
	void Init() {
		normalizedArea = new Rect(0, .9f, 0, .1f);
		repeat = new Rect(0, 0, 1, 1);
	}
	
	#endregion

	/// <summary> Draw this bar in a given area, with a given fill amount. </summary>
	public void Draw(Rect area, float fill) {
		if (frontGraphic == null) { frontGraphic = pixel; }
		if (backGraphic == null) { backGraphic = pixel; }
		
		if (mode == Mode.Normal) { DrawNormal(area, fill); }
		else if (mode == Mode.Fixed) { DrawFixed(area, fill); }
		else if (mode == Mode.Repeat) { DrawRepeat(area, repeat, fill); }
		else if (mode == Mode.Icons) { DrawIcons(area, iconRepeat, fill); }
	}

	/// <summary> Draw this bar in its area, with a given fill amount.</summary>
	public void Draw(float fill) {
		if (mode == Mode.Normal) { DrawNormal(fill); }
		else if (mode == Mode.Fixed) { DrawFixed(fill); }
		else if (mode == Mode.Repeat) { DrawRepeat(fill); }
	}

	///<summary> Normal draw method. Paints the background, then the foreground over it. Textures are stretched to fit. </summary>
	public void DrawNormal(float fill) { DrawNormal(area, fill); }
	///<summary> Normal draw method. Paints the background, then the foreground over it. Textures are stretched to fit. </summary>
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
	
	///<summary> Draws the stuff with 'fixed' texture positions. 
	///The textures won't move relative to the left edges of the rectangles. 
	///Draws the foreground first, then paints the background over it.</summary>
	public void DrawFixed(float fill) { DrawFixed(area, fill); }
	///<summary> Draws the stuff with 'fixed' texture positions. 
	///The textures won't move relative to the left edges of the rectangles. 
	///Draws the foreground first, then paints the background over it.</summary>
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

	///<summary> Draws with textures repeated a number of times. </summary>
	public void DrawRepeat(float fill) { DrawRepeat(area, repeat, fill); }
	///<summary> Draws with textures repeated a number of times. </summary>
	public void DrawRepeat(Rect area, Rect repeat, float fill) {
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

	///<summary> Draws the bar as a number of icons, spaced out on a number of lines. </summary>
	public void DrawIcons(int count, int lines, float fill) {
		Vector2 reps = new Vector2(count/lines, lines);
		DrawIcons(area, reps, fill);
	}

	///<summary> Draws the bar as a number of icons, determined by the given Vector2 (as (cols, rows) ) </summary>
	public void DrawIcons(Vector2 iconRepeat, float fill) { DrawIcons(area, iconRepeat, fill); }
	///<summary> Draws the bar as a number of icons, determined by the given Vector2 (as (cols, rows) ) </summary>
	public void DrawIcons(Rect area, Vector2 iconRepeat, float fill) {
		float numRows = Mathf.Floor(iconRepeat.y);
		Rect row = new Rect(area.x, area.y, area.width, area.height / numRows);
		Rect iconReps = new Rect(0, 0, iconRepeat.x, 1);
		
		for (int i = (int)numRows-1; i >= 0; i--) {
			float p = (fill*numRows) - i;
			DrawRepeat(row, iconReps, p);
			
			row = row.MoveDown();
			
			
		}
	}
	
	
	
}
