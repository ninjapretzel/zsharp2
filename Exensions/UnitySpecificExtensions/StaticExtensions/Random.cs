using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Wrap a number of features of UnityEngine.Random.
//Alias it as Rand for easier typing.
using Rand = UnityEngine.Random;

/// <summary> Wrapper class for UnityEngine.Random, with added functionality. </summary>
public static class Random {
	
	#region properties
	/// <summary> Random Seed. Wraps Through to UnityEngine.Random.seed </summary>
	public static int seed { get { return Rand.seed; } set { Rand.seed = value; } }

	/// <summary>Returns a float in range [0, 1) with even distribution</summary>
	public static float value { get { return Rand.value * .9999f; } }
	
	/// <summary>Returns a float in range [-1, 1) with even distribution. </summary>
	public static float unit { get { return -1 + (2 * Rand.value); } }

	/// <summary>Returns a value between [0, 1), with a slight bias towards a normal distribution. Implemented by (3 x Random.value) / 3</summary>
	public static float normal { get { return (value + value + value) / 3.0f; } }

	/// <summary> Returns a random lowercase character </summary>
	public static char alpha { get { return (char)((int)'a'+(int)(value * 26)); } }
	/// <summary> Returns a random UPPERCASE character </summary>
	public static char ALPHA { get { return (char)((int)'A'+(int)(value * 26)); } }
	/// <summary> Returns a random lowercase character. Same as alpha. </summary>
	public static char lowercase { get { return (char)((int)'a'+(int)(value * 26)); } }
	/// <summary> Returns a random UPPERCASE character. Same as ALPHA. </summary>
	public static char uppercase { get { return (char)((int)'A'+(int)(value * 26)); } }
	/// <summary> Returns a random numeric (1-0) character. </summary>
	public static char numeric { get { return (char)((int)'0'+(int)(value * 10)); } }

	/// <summary> Gets a random point inside a cube centered at (0, 0, 0) with sides (1, 1, 1). </summary>
	public static Vector3 insideUnitCube { get { return new Vector3(Range(-.5f, .5f), Range(-.5f, .5f), Range(-.5f, .5f)); } }

	/// <summary> Gets a random point inside a sphere centered at (0, 0, 0) with radius of 1. </summary>
	public static Vector3 insideUnitSphere { get { return Rand.insideUnitSphere; } }

	/// <summary> Gets a random point on the surface of a sphere centered at (0, 0, 0) with radius of 1. </summary>
	public static Vector3 onUnitSphere { get { return Rand.onUnitSphere; } }

	/// <summary> Gets a random point inside of a circle centered at (0, 0) with radius of 1. </summary>
	public static Vector2 insideUnitCircle { get { return Rand.insideUnitCircle; } }
	/// <summary> Gets a random point on the edge of a circle centered at (0, 0) with radius of 1. </summary>
	public static Vector2 onUnitCircle { get { return insideUnitCircle.normalized; } }
	#endregion
	
	#region basic functions 
	/// <summary> Returns an int with range [a, b). </summary>
	public static int Range(int a, int b) { return (a + (int)((float)(b-a) * value)); }
	/// <summary> Returns a float with range [a, b). </summary>
	public static float Range(float a, float b) { return (a + (b-a) * value); }
	/// <summary> Returns a float normally distributed inside of range [a, b). </summary>
	public static float Normal(float a, float b) { return (a + (b-a) * normal); }
	
	
	#endregion
	
	#region Choosing
	/// <summary> Takes a float array, sums up all weights, rolls a dice, and picks an index based on its weight, returns the chosen index. Assumes all values in array are positive. </summary>
	public static int WeightedChoose(float[] weights) {
		float total = 0;
		int i;
		for (i = 0; i < weights.Length; i++) { total += weights[i]; }
		
		float choose = value * total; 
		float check = 0;
		for (i = 0; i < weights.Length; i++) {
			check += weights[i];
			if (choose < check) { return i; }
		}
		return weights.Length-1;
	}

	/// <summary> Takes a float array, sums up all weights, rolls a dice, and picks an index based on its weight, returns the chosen index. Assumes all values in array are positive. </summary>
	public static int WeightedChoose(List<float> weights) {
		float total = 0;
		int i;
		for (i = 0; i < weights.Count; i++) { total += weights[i]; }
		
		float choose = value * total; 
		float check = 0;
		for (i = 0; i < weights.Count; i++) {
			check += weights[i];
			if (choose < check) { return i; }
		}
		return weights.Count-1;
	}
	
	#endregion
	
	#region Seed Push/Pop
	//Stores seeds for PushSeed/PopSeed
	/// <summary> Stores seed history, in case control over random events is needed. </summary>
	private static Stack<int> seedStack = new Stack<int>();

	/// <summary> Store the current seed on the stack, and start using a new seed. </summary>
	public static void PushSeed(int newSeed) {
		seedStack.Push(seed);
		seed = newSeed;
	}

	/// <summary> Restore the old seed, and remove it from the stack. </summary>
	public static int PopSeed() {
		int oldSeed = seed;
		if (seedStack.Count > 0) { seed = seedStack.Pop(); }
		else { Debug.LogWarning("Random: Tried to pop seed when no seed was present"); }
		return oldSeed;
	}
	#endregion
	
	
	
	
	
}
