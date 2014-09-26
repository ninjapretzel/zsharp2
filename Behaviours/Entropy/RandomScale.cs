using UnityEngine;
using System.Collections;

public class RandomScale : MonoBehaviour {
	public Vector3 min = Vector3.one;
	public Vector3 max = Vector3.one;
	public bool uniformXY;
	public bool uniformXZ;
	
	public bool onAwake = false;
	public RandomType randomness = RandomType.Normal;
	public static int seed = 1234211;
	public static Vector3 fieldScale = new Vector3(.12112f, .32131f, .51241f);
	
	public bool useSeed {
		get { return randomness == RandomType.Seeded; }
	}
	public bool usePerlin {
		get { return randomness == RandomType.Perlin; }
	}
	
	void Awake() {
		if (onAwake) { SetScales(); }
	}
	
	void Start() {
		SetScales();
	}
	
	void SetScales() {
	
		if (usePerlin) {
			Vector3 pos = Vector3.Scale(transform.position, fieldScale);
			
			
			Vector3 scale = Vector3.zero;
			
			scale.x = Lerp(min.x, max.x, Noise(pos.y, pos.z));
			scale.y = Lerp(min.y, max.y, Noise(pos.x, pos.z));
			scale.z = Lerp(min.z, max.z, Noise(pos.x, pos.y));
			
			if (uniformXY) { scale.y = scale.x; }
			if (uniformXZ) { scale.z = scale.x; }
			
			
			transform.localScale = Vector3.Scale(transform.localScale, scale);
			
		} else {
			if (useSeed) { Random.PushSeed(seed); }

			Vector3 scales = Vector3.zero;
			
			scales.x = Random.Range(min.x, max.x);
			
			if (uniformXY) { scales.y = scales.x; }
			else { scales.y = Random.Range(min.y, max.y); }
			
			if (uniformXZ) { scales.z = scales.x; }
			else { scales.z = Random.Range(min.z, max.z); }
			
			transform.localScale = Vector3.Scale(transform.localScale, scales);
			
			if (useSeed) { Random.PopSeed(); seed++; }
		}
		Destroy(this);
	}
	
	float Lerp(float min, float max, float f) { return min + (max-min) * f; }
	
	float Noise(float x, float y) {
		return PerlinNoise.GetValue(x, y);
	}
	
}
