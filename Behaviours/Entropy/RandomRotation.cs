using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RandomRotation : MonoBehaviour {
	public bool x = false;
	public bool y = false;
	public bool z = false;
	
	public Vector3 min = Vector3.zero;
	public Vector3 max = Vector3.one * 360;
	
	public bool onAwake = false;
	public RandomType randomness = RandomType.Normal;	
	public static int seed = 12236921;
	public static Vector3 fieldScale = new Vector3(.12112f, .32131f, .51241f);
	
	public bool useSeed {
		get { return randomness == RandomType.Seeded; }
	}
	public bool usePerlin {
		get { return randomness == RandomType.Perlin; }
	}
	
	void Awake() {
		if (onAwake) { SetRotation(); }
	}
	
	void Start() {
		SetRotation();
	}
	
	void SetRotation() {
		Transform oldParent = transform.parent;
		transform.parent = null;
		
		Vector3 rotation = transform.rotation.eulerAngles;
		if (usePerlin) {
			Vector3 pos = Vector3.Scale(transform.position, fieldScale);
			if (x) { rotation.x = Lerp(min.x, max.x, Noise(pos.y, pos.z)); }
			if (y) { rotation.y = Lerp(min.y, max.y, Noise(pos.x, pos.z)); }
			if (z) { rotation.z = Lerp(min.z, max.z, Noise(pos.x, pos.y)); }
			
			
		} else {
			if (useSeed) { Random.PushSeed(seed); }
			
			if (x) { rotation.x = Random.Range(min.x, max.x); }
			if (y) { rotation.y = Random.Range(min.y, max.y); }
			if (z) { rotation.z = Random.Range(min.z, max.z); }
			
			if (useSeed) { Random.PopSeed(); seed++; }
		}
		
		transform.rotation = Quaternion.Euler(rotation);
		transform.parent = oldParent;
		Destroy(this);
	}
	
	float Lerp(float min, float max, float f) { return min + (max-min) * f; }
	
	float Noise(float x, float y) {
		return PerlinNoise.GetValue(x, y);
	}
	
}
