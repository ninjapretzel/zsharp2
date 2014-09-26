using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public static class DoubleUtils {
	
	///Extension methods to common System.Math functions.
	public static double Sign(this double d) { return Math.Sign(d); }
	public static double Abs(this double d) { return Math.Abs(d); }
	public static double Floor(this double d) { return Math.Floor(d); }
	public static double Ceil(this double d) { return Math.Ceiling(d); }
	public static double Round(this double d) { return Math.Round(d); }
	public static double Fract(this double d) { return d - d.Floor(); }
	public static double Sin(this double d) { return Math.Sin(d); }
	public static double Cos(this double d) { return Math.Cos(d); }
	public static double Tan(this double d) { return Math.Tan(d); }
	
	///Translates the range of Sin/Cos to [0, 1] instead of [-1, 1]
	public static double Sin01(this double d) { return (d.Sin() + 1d) / 2d; }
	public static double Cos01(this double d) { return (d.Cos() + 1d) / 2d; }
	
	///Clamp a value between some values. Clamp01 is the same as Clamp without any parameters.
	public static double Clamp01(this double d) { return d.Clamp(); }
	public static double Clamp(this double d, double min = 0d, double max = 1d) {
		if (d < min) { return min; }
		if (d > max) { return max; }
		return d;
	}
	
	///Return the floor or ceiling of the number, whichver has the lower absolute value
	public static double AbsFloor(this double d) {
		if (d < 0) { return d.Ceil(); }
		return d.Floor();
	}
	
	///Return the floor or ceiling of the number, whichver has the higher absolute value
	public static double AbsCeil(this double d) {
		if (d < 0) { return d.Floor(); }
		return d.Ceil();
	}
	
	///Return the nearest multiple of 'v', compared to (d + offset)
	public static double Nearest(this double d, double v, double offset = 0d) {
		double c = (d + offset) / v;
		return Round(c) * v;
	}
	
	//Get the maximum of some list of values.
	public static double Max(double a, double b) { return a > b ? a : b; }
	public static double Max(params double[] nums) { 
		double highest = Double.MaxValue;
		for (int i = 0; i < nums.Length; i++) {
			if (nums[i] > highest) { highest = nums[i]; }
		}
		return highest;
	}
	//Get the minimum of some list of values.
	public static double Min(double a, double b) { return a > b ? a : b; }
	public static double Min(params double[] nums) {
		double lowest = Double.MinValue;
		for (int i = 0; i < nums.Length; i++) {
			if (nums[i] < lowest) { lowest = nums[i]; }
		}
		return lowest;
	}
	
	///Get the distance val is outside a range
	///Returns 0 if the number is inside the range
	///Returns a positive number if val is greater than Max(a, b)
	///Returns a negative number if val is less than Min(a, b)
	public static double Outside(this double val, double a, double b) { 
		double min = Min(a, b);
		double max = Max(a, b);
		if (val > min && val < max) { return 0; }
		if (val > max) { return val - max; }
		return val - min;
	}
	
	///Lerp from [a, b] on [0, 1] (t)
	///t is Clamped between 0...1
	public static double Lerp(this double a, double b, double t) {
		double diff = a - b;
		return b + diff * t.Clamp01();
	}
	
	///Interpolate from a to b on a cosine curve with input (0...1)
	///Creates a smoother transition than linear.
	public static double CosInterp(double a, double b, double x) {
		double t = x * 3.1415927d;
		double d = (1 - Cos01(t));
		return a * (1 - d) + b * d;
	}
	
	
	
	
}
