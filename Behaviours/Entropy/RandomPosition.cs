using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RandomPosition : MonoBehaviour {
	
	
	public Bounds change;
	
	public bool onAwake;
	public RandomType randomness = RandomType.Seeded;	
	
	public static Vector3 fieldScale = new Vector3(.12112f, .32131f, .51241f);
	static int seed = 123498359;
	
	public bool useSeed {
		get { return randomness == RandomType.Seeded; }
	}
	
	public bool usePerlin {
		get { return randomness == RandomType.Perlin; }
	}
	
	
	void Awake() { 
		if (onAwake) { SetPosition(); }
	}
	
	void Start() {
		SetPosition();
	}
	
	
	void SetPosition() {
		Vector3 offset;
		
		if (usePerlin) {
			Vector3 pos = Vector3.Scale(transform.position, fieldScale);
			
			offset.x = Noise(pos.y, pos.z);
			offset.y = Noise(pos.x, pos.z);
			offset.z = Noise(pos.x, pos.y);
		} else if (useSeed) {
			Random.Push(seed);
			
			offset.x = Random.value;
			offset.y = Random.value;
			offset.z = Random.value;
			
			Random.Pop();
			seed++;
		} else {
			offset.x = Random.value;
			offset.y = Random.value;
			offset.z = Random.value;
			
		}
		
		offset.x = Lerp(change.min.x, change.max.x, offset.x);
		offset.y = Lerp(change.min.y, change.max.y, offset.y);
		offset.z = Lerp(change.min.z, change.max.z, offset.z);
		
		transform.position += offset;
		
		Destroy(this);
	}
	
	float Lerp(float min, float max, float f) { return min + (max-min) * f; }
	
	float Noise(float x, float y) {
		return PerlinNoise.GetValue(x, y);
	}
	
}




















