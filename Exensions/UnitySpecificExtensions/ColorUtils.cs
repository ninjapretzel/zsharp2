using UnityEngine;
using System.Collections;
using System.Collections.Generic;

///Class for extension methods and static methods for adding additional features to Unity's 'Color' class

public static class Colors {

	/// <summary> Create a hex string from a Color, in the form #RRGGBBAA </summary>
	public static string HexString(this Color c) { return ((Color32)c).HexString(); }

	/// <summary> Create a hex string from a Color32, in the form #RRGGBBAA </summary>
	public static string HexString(this Color32 c) {
		string str = "";
		str += c.r.ToHex();
		str += c.g.ToHex();
		str += c.b.ToHex();
		if (c.a < 255) { str += c.a.ToHex(); }
		return str;
	}

	/// <summary>
	/// Parses a hex string into a Color object
	/// # is optional
	/// 'FF' is used for alpha if not present
	/// FF0000 = #FF0000 = #FF0000FF
	/// </summary>
	public static Color ToColorFromHex(this string s) { return (Color)s.ParseHex32(); }
	
	/// <summary>
	/// Parses a hex string into a Color object
	/// # is optional
	/// 'FF' is used for alpha if not present
	/// FF0000 = #FF0000 = #FF0000FF
	/// </summary>
	public static Color ParseHex(this string s) { return (Color)s.ParseHex32(); }

	/// <summary> 
	/// Parses a hex string into a Color32 object.
	/// # is optional
	/// 'FF' is used for alpha if not present.
	/// </summary>
	public static Color32 ParseHex32(this string s) {
		Color32 c = new Color32(0, 0, 0, 0);
		try {
			int pos = s.StartsWith("#") ? 1 : 0;

			string r = s.Substring(pos + 0, 2);
			string g = s.Substring(pos + 2, 2);
			string b = s.Substring(pos + 4, 2);
			string a = (s.Length > (pos + 6)) ? s.Substring(pos + 6, 2) : "FF";

			c = new Color32(r.ParseByte(), g.ParseByte(), b.ParseByte(), a.ParseByte());
		} catch { }

		return c;
	}
	
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
	
	///<summary>Construct a new RGB color using given HSV coordinates and alpha value.</summary
	public static Color HSV(float h, float s, float v, float a = 1) { return new Color(h, s, v, a).HSVtoRGB(); }
	///<summary>Create a RGB color with a randomized hue value, with given saturation and value.</summary>
	public static Color RandomHue(float s, float v, float a = 1) { return new Color(Random.Range(0, 1), s, v, a).HSVtoRGB(); }

	///<summary>Lerp between colors by HSV coordinates, rather than by RGB coordinates.
	///Returns an RGB color</summary>
	public static Color HSVLerp(Color a, Color b, float val) {
		Color ahsv = a.RGBtoHSV();
		Color bhsv = b.RGBtoHSV();
		return Color.Lerp(ahsv, bhsv, val).HSVtoRGB();
	}

	///<summary>Shift the hue of a color by shift percent across the spectrum.
	///Takes an RGB Color
	///Returns an RGB color.</summary>
	public static Color ShiftHue(this Color c, float shift) {
		Color hsv = c.RGBtoHSV();
		hsv.r = (hsv.r + shift) % 1f;
		return hsv.HSVtoRGB();
	}

	///<summary>Adds Saturation to an RGB color.
	///Keeps saturation between [0, 1]
	///Returns an RGB color.</summary>
	///<param name="c">Color to modify</param>
	///<param name="saturation">Saturation to add. Range [-1, 1]</param>
	///<returns>Input color with saturation modified</returns>
	public static Color Saturate(this Color c, float saturation) {
		Color hsv = c.RGBtoHSV();
		hsv.g = Mathf.Clamp01(hsv.g + saturation);
		return hsv.HSVtoRGB();
	}
	///<summary>Adds Saturation to an RGB color.
	///Lets saturation escape [0, 1]
	///Returns an RGB color.</summary>
	///<param name="c">Color to modify</param>
	///<param name="saturation">Saturation to add. Range [-1, 1]</param>
	///<returns>Input color with saturation modified</returns>
	public static Color Oversaturate(this Color c, float saturation) {
		Color hsv = c.RGBtoHSV();
		hsv.g = hsv.g + saturation;
		return hsv.HSVtoRGB();
	}

	/// <summary>
	/// Returns HSV color matching input RGB color
	/// </summary>
	/// <param name="c">RGB color to convert</param>
	/// <returns>HSV version of input</returns>
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
	
	/// <summary>
	/// Returns RGB color matching input HSV color
	/// </summary>
	/// <param name="c">HSV color to convert</param>
	/// <returns>RGB version of input</returns>
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
	
	///<summary>A rename of Color.Lerp as an extension, with an implicit 50% parameter.</summary>
	public static Color Blend(this Color a, Color b, float f = .5f) { return Color.Lerp(a, b, f); }

	///<summary>Make a new color that has RGB elements multiplied by some percentage, ignoring the alpha component</summary>
	public static Color MultRGB(this Color c, float f) { return new Color(c.r * f, c.g * f, c.b * f, c.a); }
	///<summary>Halves all RGB values, returns the resultant color.</summary>
	public static Color Half(this Color c) { return c.MultRGB(.5f); }

	///<summary>Returns a copy of this color with its alpha set to a given value</summary>
	public static Color Alpha(this Color c, float a) { return new Color(c.r, c.g, c.b, a); }

	///<summary>Pulses the alpha of a color by adding a value times the cosine of time.
	///Intended to not scale with Time.timeScale
	///Best use is to _not_ overwrite  variables with the return value.</summary>
	public static Color CosAlpha(this Color c, float change) { return c.CosAlpha(change, 1); }
	public static Color CosAlpha(this Color c, float change, float timeScale) {
		Color col = c;
		float pos = Mathf.Cos(Time.unscaledTime * timeScale);
		col.a += pos * change;
		return col;
	}

	///<summary>Get a simple string representing each component separated by a comma</summary>
	public static string ToString(this Color c, char delim) { 
		return "" + c.r + delim + c.g + delim + c.b + delim + c.a;
	}

	///<summary>Parse a string into a color</summary>
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

	///<summary>Get a color at some position across an array of colors.</summary>
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
