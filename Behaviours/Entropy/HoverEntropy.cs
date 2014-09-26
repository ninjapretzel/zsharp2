using UnityEngine;
using System.Collections;

public class HoverEntropy : MonoBehaviour {
	public static int seed = 123919541;
	public BMM time = new BMM();
	public BMM scale = new BMM();
	public BMM offsetx = new BMM();
	public BMM offsety = new BMM();
	public BMM offsetz = new BMM();
	
	public RandomType randomness = RandomType.Seeded;	
	
	public bool useSeed {
		get { return randomness == RandomType.Seeded; }
	}
	
	public bool usePerlin {
		get { return randomness == RandomType.Perlin; }
	}
	
	
	void Start() {
		Hover hover = GetComponent<Hover>();
		
		if (usePerlin) {
			if (hover != null) {
				int num = Mathf.Min(hover.offsets.Length, hover.oscis.Length);
				Vector3 pos = transform.position;
				pos.x *= .12112f;
				pos.y *= .32131f;
				pos.z *= .51241f;
				
				pos.x += pos.z;
				pos.y += pos.z * .5f;
				
				
				for (int i = 0; i < num; i++) {
					Oscillator osci = hover.oscis[i];
					
					osci.maxTime *= time.Perlin(pos.x, pos.y);
					float val = scale.Perlin(pos.x, pos.y);
					osci.minVal *= val;
					osci.maxVal *= val;
					
					Vector3 offset = hover.offsets[i];
					Vector3 scales = new Vector3(offsetx.Perlin(pos.x, pos.y), offsety.Perlin(pos.x, pos.y), offsetz.Perlin(pos.x, pos.y));
					hover.offsets[i] = Vector3.Scale(offset, scales);
					
					if (PerlinNoise.GetValue(pos.x, pos.y) < .5) { hover.offsets[i] *= -1; }
					
				}
				
			}
			
		} else {
			if (useSeed) { Random.PushSeed(seed); }
			
			if (hover != null) {
				int num = Mathf.Min(hover.offsets.Length, hover.oscis.Length);
				for (int i = 0; i < num; i++) {
					Oscillator osci = hover.oscis[i];
					
					osci.maxTime *= time.value;
					float val = scale.value;
					osci.minVal *= val;
					osci.maxVal *= val;
					
					Vector3 offset = hover.offsets[i];
					Vector3 scales = new Vector3(offsetx.value, offsety.value, offsetz.value);
					hover.offsets[i] = Vector3.Scale(offset, scales);
					if (Random.value < .5) { hover.offsets[i] *= -1; }
				}
			}
			
			if (useSeed) { Random.PopSeed(); seed++; }
			
		}
		Destroy(this);
	}
	
	void Update() {
	
	}
	
	
	float Lerp(float min, float max, float f) { return min + (max-min) * f; }
	
	float Noise(float x, float y) {
		return PerlinNoise.GetValue(x, y);
	}
	
	
}
