using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public static class Bars {
	public static int defaultPadding = 2;
	
	public static Texture2D graphic;
	public static Texture2D vertical;
	
	static Bars() {
		graphic = Resources.Load<Texture2D>("pixel");
		vertical = Resources.Load<Texture2D>("pixel");
		
	}
	
	//General drawing functions
	public static void Draw(Rect area, float pp) { Draw(area, pp, Color.white, Color.black, defaultPadding); }
	public static void Draw(Rect area, float pp, Color tint) { Draw(area, pp, tint, Color.black, defaultPadding); }
	public static void Draw(Rect area, float pp, Color tint, Color back) { Draw(area, pp, tint, back, defaultPadding); }
	public static void Draw(Rect area, float pp, Color tint, Color back, int padding) {
		Rect brush = area.Trim(-padding);
		float p = Mathf.Clamp01(pp);
		Texture2D g = GetGraphic(area);
		
		GUI.color = back;
		GUI.DrawTexture(brush, g);
		
		brush = brush.Trim(padding);
		if (area.width > area.height) { brush.width *= p; }
		else { 
			brush.y += area.height * (1.0f - p);
			brush.height *= p;
		}
		GUI.color = tint;
		GUI.DrawTexture(brush, g);
	}
	
	
	public static void DrawFixed(Rect area, float pp, Color tint, Color back, int padding) {
		Rect brush = area.Trim(-padding);
		float p = Mathf.Clamp01(pp);
		Texture2D g = GetGraphic(area);
		
		GUI.color = back;
		GUI.DrawTexture(brush, g);
		
		
		
		brush = brush.Trim(padding);
		GUI.color = tint;
		GUI.DrawTexture(brush, g);
		
		if (area.width > area.height) {
			brush.x += area.width * p;
			brush.width *= p;
		} else { brush.height *= (1.0f-p); }
		GUI.color = back;
		GUI.DrawTexture(brush, g);
	}
	
	
	//Draws a repeated texture
	public static void Draw(Rect area, Rect repeat, float pp) { Draw(area, repeat, pp, Color.white, Color.black, defaultPadding); }
	public static void Draw(Rect area, Rect repeat, float pp, Color tint) { Draw(area, repeat, pp, tint, Color.black, defaultPadding); }
	public static void Draw(Rect area, Rect repeat, float pp, Color tint, Color back) { Draw(area, repeat, pp, tint, back, defaultPadding); }
	public static void Draw(Rect area, Rect repeat, float pp, Color tint, Color back, int padding) {
		Rect brush = area.Pad(padding);
		float p = Mathf.Clamp01(pp);
		Texture2D g = GetGraphic(area);
		
		GUI.color = back;
		GUI.DrawTextureWithTexCoords(brush, g, repeat);
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
		
		GUI.color = tint;
		GUI.DrawTextureWithTexCoords(filled, g, filledReps);
		GUI.color = back;
		GUI.DrawTextureWithTexCoords(empty, g, emptyReps);
	}
	
	
	//Draws bars as icons
	public static void Draw(Rect area, Vector2 iconRepeat, float pp) { Draw(area, iconRepeat, pp, Color.white, Color.black); }
	public static void Draw(Rect area, Vector2 iconRepeat, float pp, Color tint) { Draw(area, iconRepeat, pp, tint, Color.black); }
	public static void Draw(Rect area, Vector2 iconRepeat, float pp, Color tint, Color back) {
		float numRows = Mathf.Floor(iconRepeat.y);
		Rect row = new Rect(area.x, area.y, area.width, area.height / numRows);
		Rect repeat = new Rect(0, 0, iconRepeat.x, 1);
		
		
		for (int i = (int)numRows-1; i >= 0; i--) {
			float p = (pp*numRows) - i;
			Draw(row, repeat, p, tint, back, 0);
			
			row = row.MoveDown();
			
			
		}
		
	}
	
	
	
	public static Texture2D GetGraphic(Rect area) {
		if (area.width > area.height || vertical == null) { return graphic; }
		return vertical;
	}
	
	
}
