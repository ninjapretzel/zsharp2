using UnityEngine;
using System.Collections;
using System.Collections.Generic;

//Wrap a number of features of UnityEngine.Random.
//Alias it as Rand for easier typing.
using Rand = UnityEngine.Random;

public static class Random {
	
	#region properties
	public static int seed { get { return Rand.seed; } set { Rand.seed = value; } }
	
	//value is adjusted to return approx [0, 1)
	public static float value { get { return Rand.value * .9999f; } }
	
	//unit has range [-1, 1) and even distribution
	public static float unit { get { return -1 + (2 * Rand.value); } }
	
	//normal is normally distributed from [0, 1)
	public static float normal { get { return (value + value + value) / 3.0f; } }
	
	public static char alpha { get { return (char)((int)'a'+(int)(value * 26)); } }
	public static char ALPHA { get { return (char)((int)'A'+(int)(value * 26)); } }
	public static char lowercase { get { return (char)((int)'a'+(int)(value * 26)); } }
	public static char uppercase { get { return (char)((int)'A'+(int)(value * 26)); } }
	public static char numeric { get { return (char)((int)'0'+(int)(value * 10)); } }
	
	
	public static Vector3 insideUnitCube { get { return new Vector3(Range(-.5f, .5f), Range(-.5f, .5f), Range(-.5f, .5f)); } }
	
	public static Vector3 insideUnitSphere { get { return Rand.insideUnitSphere; } }
	public static Vector3 onUnitSphere { get { return Rand.onUnitSphere; } }
	public static Vector2 insideUnitCircle { get { return Rand.insideUnitCircle; } }
	public static Vector2 onUnitCircle { get { return insideUnitCircle.normalized; } }
	#endregion
	
	#region basic functions 
	public static int Range(int a, int b) { return (a + (int)((float)(b-a) * value)); }
	public static float Range(float a, float b) { return (a + (b-a) * value); }
	public static float Normal(float a, float b) { return (a + (b-a) * normal); }
	
	
	#endregion
	
	#region Choosing
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
	private static Stack<int> seedStack = new Stack<int>();
	
	public static void PushSeed(int newSeed) {
		seedStack.Push(seed);
		seed = newSeed;
	}
	
	public static int PopSeed() {
		int oldSeed = seed;
		if (seedStack.Count > 0) { seed = seedStack.Pop(); }
		else { Debug.LogWarning("Random: Tried to pop seed when no seed was present"); }
		return oldSeed;
	}
	#endregion
	
	
	
	
	
}
