using UnityEngine;
using System.Collections.Generic;
using System.Collections;

public class RandomMaterial : MonoBehaviour {
	public Material[] materials;
	public float[] weights;
	
	public Renderer[] alsoSetThese;
	
	public bool onAwake = false;
	public RandomType randomness = RandomType.Normal;	
	public static int seed = 125623;
	public static Vector3 fieldScale = new Vector3(.12112f, .32131f, .51241f);
	
	public bool useSeed {
		get { return randomness == RandomType.Seeded; }
	}
	
	public bool usePerlin {
		get { return randomness == RandomType.Perlin; }
	}
	
	void Awake() {
		if (onAwake) { SetMaterial(); }
	}
	
	void Start() {
		SetMaterial();
	}
	
	void SetMaterial() { 
		int index = 0;
		
		if (usePerlin) {
			Vector3 pos = Vector3.Scale(transform.position, fieldScale);
			
			pos.x += pos.z;
			pos.y += pos.z * .5f;
			
			float f = Noise(pos.x, pos.y);
			index = (int)(materials.Length * f * .99999f);
			
		} else if (useSeed) {
			Random.PushSeed(seed);
			
			if (weights.Length == 0) { index = materials.RandomIndex(); }
			else { index = Random.WeightedChoose(weights); }
			
			Random.PopSeed();
			seed++;
		} else {
			if (weights.Length == 0) { index = materials.RandomIndex(); }
			else { index = Random.WeightedChoose(weights); }
		}
		
		if (renderer != null) {
			renderer.material = materials[index];
		}
		foreach (Renderer r in alsoSetThese) {
			r.material = materials[index];
		}
		
		Destroy(this);
	}
	
	
	float Lerp(float min, float max, float f) { return min + (max-min) * f; }
	
	float Noise(float x, float y) {
		return PerlinNoise.GetValue(x, y);
	}
	
}
