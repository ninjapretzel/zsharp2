using UnityEngine;
using System.Collections;

[System.Serializable]
public class PerlinNoise {
	////////////////////////////////////////////////////////////////////////////
	//Variables
	public int yShift = 57;
	public int seed1 = 15731;
	public int seed2 = 789221;
	public int seed3 = 1376312589;
	public float highVal = 1073741824.0f;
	public float freqBase = 2.71231f;
	
	public Vector2 gridOffset = Vector2.zero;
	public float gridScale = 10;
	
	public float persistance = .5f;
	public int octaves = 5;
	
	public int textureSize = 64;
	
	public float baseValue = .5f;
	public bool abs = false;
	public bool invert = false;
	
	
	public bool filter = false;
	public float cutoff = .3f;

	private static PerlinNoise main;
	static PerlinNoise() {
		main = new PerlinNoise();
	}
	
	////////////////////////////////////////////////////////////////////////////
	//Static Functions
	public static float GetValue(float x, float y) { 
		return Mathf.Clamp01(main.Get(x, y));
	}
	public static void NewSeeds() { main.Reseed(); }
	public static void NewSeeds(int[] seeds) {
		main.yShift = seeds[0];
		main.seed1 = seeds[1];
		main.seed2 = seeds[2];
		main.seed3 = seeds[3];
	}
	
	public static void NewSeeds(int ys, int s1, int s2, int s3) {
		main.yShift = ys;
		main.seed1 = s1;
		main.seed2 = s2;
		main.seed3 = s3;
	}
	
	public static int[] GetSeeds() { 
		int[] seeds = new int[4];
		seeds[0] = main.yShift;
		seeds[1] = main.seed1;
		seeds[2] = main.seed2;
		seeds[3] = main.seed3;
		return seeds;
	}
	
	public void Reseed() {
		yShift = PrimesList.Choose0();
		seed1 = PrimesList.Choose1();
		seed2 = PrimesList.Choose2();
		seed3 = PrimesList.Choose3();
	}
	
	////////////////////////////////////////////////////////////////////////////
	//Member Functions
	float Noise(float x, float y) { return Noise((int)x, (int)y); }
	float Noise(int x, int y) {
		int n = x + y * yShift;	
		n = (n << 13) ^ n;
		return ( 1.0f - ( (n * (n * n * seed1 + seed2) + seed3) & 2147483647) / highVal); 
	}
	
	float SmoothNoise(float x, float y) { return SmoothNoise((int)x, (int)y); }
	float SmoothNoise(int x, int y) {
		float corners = (Noise(x-1, y-1) + Noise(x+1, y-1) + Noise(x-1, y+1) + Noise(x+1, y+1) ) / 16.0f;
		float sides = (Noise(x-1, y) + Noise(x+1, y) + Noise(x, y-1)  + Noise(x, y+1) ) / 8.0f;
		float center = Noise(x, y) / 4.0f;
		return corners + sides + center;
	}
	
	float InterpNoise(float x, float y) {
		int xi = (int)x;
		int yi = (int)y;
		float xf = x.Fract();
		float yf = y.Fract();
		
		float v1 = SmoothNoise(xi, yi);
		float v2 = SmoothNoise(xi + 1, yi);
		float v3 = SmoothNoise(xi, yi + 1);
		float v4 = SmoothNoise(xi + 1, yi + 1);
		
		float v5 = FloatUtils.CosInterp(v1, v2, xf);
		float v6 = FloatUtils.CosInterp(v3, v4, xf);

		return FloatUtils.CosInterp(v5, v6, yf);
	}
	
	public float PNoise(float x, float y) {
		float t = 0;
		float p = persistance;
		int n = octaves;
		
		float freq = 1;
		float amp = 1;
		for (int i = 0; i < n; i++) {
			freq = Mathf.Pow(freqBase, i);
			if (p > 1.02 || p < .98) { amp = Mathf.Pow(p, i); } else { amp = 1; }
			t += InterpNoise(x * freq, y * freq) * amp;
		}
		
		if (abs && t < 0) { t *= -1; }
		t = baseValue + t;
		if (invert) { t = 1.0f - t; }
		return t;
	}
	
	
	public float GetX(Vector3 v) { return PNoise(v.y, v.z); }
	public float GetXI(Vector3 v) { return PNoise(v.z, v.y); }
	
	public float GetY(Vector3 v) { return PNoise(v.x, v.z); }
	public float GetYI(Vector3 v) { return PNoise(v.z, v.x); }
	
	public float GetZ(Vector3 v) { return PNoise(v.x, v.y); }
	public float GetZI(Vector3 v) { return PNoise(v.y, v.x); }
	
	public float Get(Vector3 v) { return PNoise(v.x, v.y); }
	public float Get(Vector2 v) { return PNoise(v.x, v.y); }
	public float Get(float x, float y) { return PNoise(x, y); }
	
	public Texture2D GetTexture() { return GetTexture(textureSize); }
	public Texture2D GetTexture(int size) {
		Texture2D texture = new Texture2D(size, size, TextureFormat.ARGB32, false);
		texture.filterMode = FilterMode.Point;
		texture.wrapMode = TextureWrapMode.Clamp;
		
		Color[] colors = texture.GetPixels();
		int i = 0;
		float v = 0;
		Vector2 pos = Vector2.zero;
		float pixelScale = 1.0f / ((float)size);
		
		for (int yy = 0; yy < size; yy++) {
			for (int xx = 0; xx < size; xx++) {
				pos = new Vector2(xx, yy);
				pos *= pixelScale * gridScale;
				pos += gridOffset;
				
				v = PNoise(pos.x, pos.y);
				
				
				colors[i] = new Color(v, v, v);
				i++;
			}
		}
		
		if (filter) { colors = Filter(colors); }
		
		texture.SetPixels(colors);
		texture.Apply();
		
		return texture;
	}
	
	
	
	Color[] Filter(Color[] colors) {
		float t = 0;
		Color[] ccs = new Color[colors.Length];
		
		for (int i = 0; i < colors.Length; i++) {
			t = colors[i].r + colors[i].g + colors[i].b;
			t /= 3;
			if (t > cutoff) { ccs[i] = Color.white; } else { ccs[i] = Color.black; }
		}
		
		return ccs;
	}
	
}




























