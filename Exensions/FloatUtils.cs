using UnityEngine;
using System.Collections;
using System.Linq;
using System;

//Class: FloatF
//Holds extension methods for float data type.
public static class FloatUtils {
	
	//Get the byte array of a float value
	public static byte[] GetBytes(this float f) { return BitConverter.GetBytes(f); }
	
	//Lerp based off of Time.deltaTime.
	//Lerps a value 'f' towards 'target' based off of ('v' * Time.deltaTime)
	public static float TLerp(this float f, float target) { return f.TLerp(target, 1); }
	public static float TLerp(this float f, float target, float v) { return f.Lerp(target, Time.deltaTime * v); }
	
	//Standard lerp function. Lerps a value 'f' towards 'target' by 'v'
	public static float Lerp(this float f, float target, float v) { return Mathf.Lerp(f, target, v); }
	
	//Get the position of a value relative to two other values
	public static float Normalize(this float f, float a, float b) { return f.Position(a, b); }
	public static float Position(this float f, float a, float b) {
		float min = Min(a, b);
		float max = Max(a, b);
		float dist = max - min;
		return (f-min) / dist;
	}
	
	//Clamping functions. Clamp a value between two values, contained in a vector2, or passed separately, or between [0...1]
	public static float Clamp(this float f, float a, float b) { return Mathf.Clamp(f, Min(a, b), Max(a, b)); }
	public static float Clamp(this float f, Vector2 v) { return f.Clamp(v.x, v.y); }
	public static float Clamp01(this float f) { return Mathf.Clamp01(f); }
	
	//Rounding functions
	public static float Floor(this float f) { return Mathf.Floor(f); }
	public static float Ceil(this float f) { return Mathf.Ceil(f); }
	public static float Round(this float f) { return Mathf.Round(f); }
	public static float RoundDown(this float f) { return f.Floor(); }
	public static float RoundUp(this float f) { return f.Ceil(); }
	
	//Fractional part of the number
	public static float Fract(this float f) { return f - f.Floor(); }
	
	//Get the lowest value of a list of floats
	public static float Min(float a, float b) { return a < b ? a : b; }
	public static float Min(params float[] nums) {
		float lowest = Single.MaxValue;
		for (int i = 0; i < nums.Length; i++) {
			if (nums[i] < lowest) { lowest = nums[i]; }
		}
		return lowest;
	}
	
	//Get the highest value of a list of floats
	public static float Max(float a, float b) { return a > b ? a : b; }
	public static float Max(params float[] nums) {
		float highest = Single.MinValue;
		for (int i = 0; i < nums.Length; i++) {
			if (nums[i] > highest) { highest = nums[i]; }
		}
		return highest;
	}
	
	//Return if the number is NAN
	public static bool IsNAN(this float f) { return float.IsNaN(f); }
	public static bool IsNaN(this float f) { return float.IsNaN(f); }
	
	
	//Round a value (f) to the nearest multiple of 'v'
	public static float Nearest(this float f, float v) { return f.Nearest(v, 0); }
	public static float Nearest(this float f, float v, float offset) {
		float d = (f + offset) / v;
		return Mathf.Round(d) * v;
	}
	
	//Get the sign/absolute value of a number
	public static float Sign(this float f) { return Mathf.Sign(f); }
	public static float Abs(this float f) { return Mathf.Abs(f); }
	
	//Get the distance val is outside the range a,b
	public static float Outside(this float val, float a, float b) {
		float min = Mathf.Min(a, b);
		float max = Mathf.Max(a, b);
		if (val > min && val < max) { return 0; }
		if (val > max) { return val - max; }
		return val - min;
	}
	
	//Interpolate between two values with a cosine curve
	public static float CosInterp(float a, float b, float x) {
		float ft = x * 3.1415927f;
		float f = (1 - Mathf.Cos(ft)) * .5f;
		return  a * (1-f) + b * f;
	}
	
	public static bool IsInside(this float f, float a, float b, float epsilon = .0001f) {
		return (f >= Mathf.Min(a, b)-epsilon && f <= Mathf.Max(a, b)+epsilon);
	}
	
	public static bool IsBetween(this float f, float a, float b) {
		return (f > Mathf.Min(a, b) && f < Mathf.Max(a, b));
	}
	
	public static float Sin(this float f) { return Mathf.Sin(f); }
	public static float Cos(this float f) { return Mathf.Cos(f); }
	public static float Tan(this float f) { return Mathf.Tan(f); }
	
	public static float Sin01(this float f) { return (f.Sin() + 1f) / 2f; } 
	public static float Cos01(this float f) { return (f.Cos() + 1f) / 2f; } 
	
	
	
	
}
	