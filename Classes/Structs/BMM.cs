using UnityEngine;
using System.Collections;

///<summary> Stands for 'bool min max', stores a bool and a range. Used for randomizations. </summary>
[System.Serializable]
public class BMM {
	///<summary> Randomize this field? </summary>
	public bool randomize = false;
	///<summary> Lowest possible value. </summary>
	public float min = 0.9f;
	///<summary> Largest possible value. </summary>
	public float max = 1.1f;

	///<summary> Get the (next) value, evenly distributed </summary>
	public float value {
		get {
			if (!randomize) { return 1.0f; }
			return Random.Range(min, max);
		}
	}

	///<summary> Get the (next) value, normally distributed </summary>
	public float normal {
		get {
			if (!randomize) { return 1.0f; }
			return Random.Normal(min, max);
		}
	}

	///<summary> Get the (next) value, distributed with perlin noise </summary>
	public float Perlin(float x, float y) {
		float f = PerlinNoise.GetValue(x, y);
		return min + (max-min) * f; 
	}

	///<summary> Default constructor </summary>
	public BMM() {
		randomize = false;
		min = 0.9f;
		max = 1.1f;
	}

	///<summary> Parametered constructor </summary>
	public BMM(bool bb, float mmin, float mmax) {
		randomize = bb;
		min = mmin;
		max = mmax;
	}
	
}
