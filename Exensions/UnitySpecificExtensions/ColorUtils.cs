using UnityEngine;
using System.Collections;
using System.Collections.Generic;

///Class for extension methods and static methods for adding additional features to Unity's 'Color' class

public static class Colors {
	
	#region color making functions
	///These functions create a color by name with a given intensity.
	///Optionaly, an alpha value can also be specified.
	public static Color white 	{ get { return new Color(1, 1, 1, 1); } }
	public static Color gray 	{ get { return new Color(.5f, .5f, .5f, 1); } }
	public static Color red 	{ get { return new Color(1, 0, 0, 1); } }
	public static Color green 	{ get { return new Color(0, 1, 0, 1); } }
	public static Color blue 	{ get { return new Color(0, 0, 1, 1); } }
	public static Color yellow 	{ get { return new Color(1, 1, 0, 1); } }
	public static Color purple 	{ get { return new Color(1, 0, 1, 1); } }
	public static Color cyan 	{ get { return new Color(0, 1, 1, 1); } }
	
	public static Color White(float f) 	{ return White(f, 1); }
	public static Color Gray(float f) 	{ return Gray(f, 1); }
	public static Color Red(float f) 	{ return Red(f, 1); }
	public static Color Green(float f) 	{ return Green(f, 1); }
	public static Color Blue(float f) 	{ return Blue(f, 1); }
	public static Color Yellow(float f) { return Yellow(f, 1); }
	public static Color Purple(float f) { return Purple(f, 1); }
	public static Color Cyan(float f) 	{ return Cyan(f, 1); }
	
	public static Color White(float f, float a) 	{ return new Color(f, f, f, a); }
	public static Color Gray(float f, float a) 		{ return new Color(f, f, f, a); }
	public static Color Red(float f, float a) 		{ return new Color(f, 0, 0, a); }
	public static Color Green(float f, float a) 	{ return new Color(0, f, 0, a); }
	public static Color Blue(float f, float a) 		{ return new Color(0, 0, f, a); }
	public static Color Yellow(float f, float a) 	{ return new Color(f, f, 0, a); }
	public static Color Purple(float f, float a) 	{ return new Color(f, 0, f, a); }
	public static Color Cyan(float f, float a) 		{ return new Color(0, f, f, a); }
	#endregion
	
	#region HSV support
	///Has a number of functions which add support for the HSV color space to unity colors.
	///HSV colors are stored as standard Colors with the same range for values.
	///R maps Hue
	///G maps Saturation
	///B maps Value
	///A carries Alpha just the same.
	///Typical use would be to explicitly mark any variables that should carry HSV information as such
	///And convert to and from HSV space as needed.
	///Some common things (like shifting hue) are supported as extensions.
	
	///Construct a new RGB color using given HSV coordinates and alpha value.
	public static Color HSV(float h, float s, float v, float a = 1) { return new Color(h, s, v, a).HSVtoRGB(); }
	///Create a RGB color with a randomized hue value, with given saturation and value.
	public static Color RandomHue(float s, float v, float a = 1) { return new Color(Random.Range(0, 1), s, v, a).HSVtoRGB(); }
	
	///Lerp between colors by HSV coordinates, rather than by RGB coordinates.
	///Returns an RGB color
	public static Color HSVLerp(Color a, Color b, float val) {
		Color ahsv = a.RGBtoHSV();
		Color bhsv = b.RGBtoHSV();
		return Color.Lerp(ahsv, bhsv, val).HSVtoRGB();
	}
	
	///Shift the hue of a color by shift percent across the spectrum.
	//Returns an RGB color.
	public static Color ShiftHue(this Color c, float shift) {
		Color hsv = c.RGBtoHSV();
		hsv.r = (hsv.r + shift) % 1f;
		return hsv.HSVtoRGB();
	}
	
	//Adds Saturation to a color.
	//Keeps saturation between [0, 1]
	//Returns an RGB color.
	public static Color Saturate(this Color c, float saturation) {
		Color hsv = c.RGBtoHSV();
		hsv.g = Mathf.Clamp01(hsv.g + saturation);
		return hsv.HSVtoRGB();
	}
	
	///RGB -> HSV logic.
	public static Color RGBtoHSV(this Color c) {
		Color hsv = new Color(0, 0, 0, c.a);
		
		float max = Mathf.Max(c.r, c.g, c.b);
		if (max <= 0) { return hsv; }
		//Value
		hsv.b = max;
		
		float r, g, b;
		r = c.r;
		g = c.g;
		b = c.b;
		float min = Mathf.Min(r, g, b);
		float delta = max - min;
		
		//Saturation
		hsv.g = delta/max;
		
		//Hue
		float h;
		if (r == max) {
			h = (g - b) / delta;
		} else if (g == max) {
			h = 2 + (b - r) / delta;
		} else {
			h = 4 + (r - g) / delta;
		}
		
		h /= 6f; // convert h (0...6) space to (0...1) space
		if (h < 0) { h += 1; }
		
		hsv.r = h;
		
		
		return hsv;
	}
	
	//HSV -> RGB Logic
	public static Color HSVtoRGB(this Color c) {
		int i;
		
		float a = c.a;
		float h, s, v;
		float f, p, q, t;
		h = c.r;
		s = c.g;
		v = c.b;
		
		if (s == 0) {
			return new Color(v, v, v, a);
		}
		
		//convert h from (0...1) space to (0...6) space
		h *= 6f;
		i = (int)Mathf.Floor(h);
		f = h - i;
		p = v * (1 - s);
		q = v * (1 - s * f);
		t = v * (1 - s * (1 - f) );
		
		if (i == 0) {
			return new Color(v, t, p, a);
		} else if (i == 1) {
			return new Color(q, v, p, a);
		} else if (i == 2) {
			return new Color(p, v, t, a);
		} else if (i == 3) {
			return new Color(p, q, v, a);
		} else if (i == 4) {
			return new Color(t, p, v, a);
		} 
		
		return new Color(v, p, q, a);
		
	}
	#endregion
	
	#region extra functions
	///Extra functions dealing with standard colors
	
	///A rename of Color.Lerp as an extension, with an implicit 50% parameter.
	public static Color Blend(this Color a, Color b, float f = .5f) { return Color.Lerp(a, b, f); }
	
	///Make a new color that has RGB elements multiplied by some percentage, ignoring the alpha component
	public static Color MultRGB(this Color c, float f) { return new Color(c.r * f, c.g * f, c.b * f, c.a); }
	///Halves all RGB values, returns the resultant color.
	public static Color Half(this Color c) { return c.MultRGB(.5f); }
	
	///Pulses the alpha of a color by adding a value times the cosine of time.
	///Intended to not scale with Time.timeScale
	///Best use is to _not_ overwrite  variables with the return value.
	public static Color CosAlpha(this Color c, float change) { return c.CosAlpha(change, 1); }
	public static Color CosAlpha(this Color c, float change, float timeScale) {
		Color col = c;
		float pos = Mathf.Cos(Time.unscaledTime * timeScale);
		col.a += pos * change;
		return col;
	}
	
	///Get a simple string representing each component separated by a comma
	public static string ToString(this Color c, char delim) { 
		return "" + c.r + delim + c.g + delim + c.b + delim + c.a;
	}
	
	///Parse a string into a color
	public static Color FromString(string s, char delim = ',') {
		string[] strs = s.Split(delim);
		Color c = Color.white;
		if (strs.Length < 3) {
			Debug.LogWarning("Tried to load color from malformed string.\nDelim:" + delim + "\n" + s); 
			return c; 
		}
		c.r = strs[0].ParseFloat();
		c.g = strs[1].ParseFloat();
		c.b = strs[2].ParseFloat();
		if (strs.Length >= 4) { c.a = strs[3].ParseFloat(); }
		return c;
	}
	
	///Get a color at some position across an array of colors.
	public static Color Lerp(this Color[] colors, float position) {
		if (colors.Length == 0) { return Color.white; }
		else if (colors.Length == 1) { return colors[0]; }
		
		int segments = colors.Length-1;
		int segment = (int)(0f + Random.value * ((float)segments));
		float f = (position * segments) % 1f;
		
		return Color.Lerp(colors[segment], colors[segment+1], f);
	}
	
	#endregion
	
}
