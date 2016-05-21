using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class HoverRotationEntropy : MonoBehaviour {
	public static int seed = 123919541;

	public BMM time = new BMM();
	public BMM curTime = new BMM(false, 0, 1);
	public BMM scale = new BMM();
	public BMM offsetx = new BMM();
	public BMM offsety = new BMM();
	public BMM offsetz = new BMM();
	
	public RandomType randomness = RandomType.Seeded;

	public bool useSeed { get { return randomness == RandomType.Seeded; } }
	public bool usePerlin { get { return randomness == RandomType.Perlin; } }
	
	void Start() {
		HoverRotation hover = GetComponent<HoverRotation>();

		if (hover != null) {
			if (usePerlin) {
				int num = Mathf.Min(hover.rotations.Length);
				Vector3 p = transform.position;
				p.x *= .12112f;
				p.y *= .32131f;
				p.z *= .51241f;

				p.x += p.z;
				p.y += p.z * .5f;

				for (int i = 0; i < num; i++) {
					var pos = p + new Vector3(i * .01337f, i * .31337f, i * .73313f);
					Oscillator osci = hover.oscis[i];
					
					if (curTime.randomize) { osci.SetTime(curTime.Perlin(pos.z, pos.x), PerlinNoise.GetValue(pos.y, pos.z) < .5f); }

					osci.maxTime *= time.Perlin(pos.x, pos.y);
					osci.curTime = osci.maxTime * new BMM(true, 0, 1).Perlin(pos.z, pos.x);
					float val = scale.Perlin(pos.y, pos.z);
					osci.minVal *= val;
					osci.maxVal *= val;

					Vector3 offset = hover.rotations[i];
					Vector3 scales = new Vector3(offsetx.Perlin(pos.y, pos.z), offsety.Perlin(pos.z, pos.x), offsetz.Perlin(pos.z, pos.y));
					hover.rotations[i] = Vector3.Scale(offset, scales);

					if (PerlinNoise.GetValue(pos.x, pos.z) < .5) { hover.rotations[i] *= -1; }

				}

			} else {
				if (useSeed) { Random.PushSeed(seed); }
				int num = Mathf.Min(hover.rotations.Length, hover.oscis.Length);
				for (int i = 0; i < num; i++) {
					Oscillator osci = hover.oscis[i];

					osci.maxTime *= time.value;
					osci.curTime = osci.maxTime * Random.value;
					float val = scale.value;
					osci.minVal *= val;
					osci.maxVal *= val;

					Vector3 offset = hover.rotations[i];
					Vector3 scales = new Vector3(offsetx.value, offsety.value, offsetz.value);
					hover.rotations[i] = Vector3.Scale(offset, scales);
					if (Random.value < .5) { hover.rotations[i] *= -1; }
				}
				if (useSeed) { Random.PopSeed(); seed++; }
			}
			
		}

		Destroy(this);
	}
	
}
