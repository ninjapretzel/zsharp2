using UnityEngine;
using System.Collections;

[System.Serializable]
public class SimplexNoise {
	
	//default permutation.
	//Same list repeated twice. 256 ints long.
	//Just a random list of ints.
	public static int[] stdPerm = { 
		151,160,137,91,90,15,131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,
		8,99,37,240,21,10,23,190,6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,
		35,11,32,57,177,33,88,237,149,56,87,174,20,125,136,171,168,68,175,74,165,71,
		134,139,48,27,166,77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,
		55,46,245,40,244,102,143,54,65,25,63,161,1,216,80,73,209,76,132,187,208, 89,
		18,169,200,196,135,130,116,188,159,86,164,100,109,198,173,186,3,64,52,217,226,
		250,124,123,5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,
		189,28,42,223,183,170,213,119,248,152,2,44,154,163,70,221,153,101,155,167,43,
		172,9,129,22,39,253,19,98,108,110,79,113,224,232,178,185,112,104,218,246,97,
		228,251,34,242,193,238,210,144,12,191,179,162,241,81,51,145,235,249,14,239,
		107,49,192,214,31,181,199,106,157,184,84,204,176,115,121,50,45,127,4,150,254,
		138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180,
		
		151,160,137,91,90,15,131,13,201,95,96,53,194,233,7,225,140,36,103,30,69,142,
		8,99,37,240,21,10,23,190,6,148,247,120,234,75,0,26,197,62,94,252,219,203,117,
		35,11,32,57,177,33,88,237,149,56,87,174,20,125,136,171,168,68,175,74,165,71,
		134,139,48,27,166,77,146,158,231,83,111,229,122,60,211,133,230,220,105,92,41,
		55,46,245,40,244,102,143,54,65,25,63,161,1,216,80,73,209,76,132,187,208, 89,
		18,169,200,196,135,130,116,188,159,86,164,100,109,198,173,186,3,64,52,217,226,
		250,124,123,5,202,38,147,118,126,255,82,85,212,207,206,59,227,47,16,58,17,182,
		189,28,42,223,183,170,213,119,248,152,2,44,154,163,70,221,153,101,155,167,43,
		172,9,129,22,39,253,19,98,108,110,79,113,224,232,178,185,112,104,218,246,97,
		228,251,34,242,193,238,210,144,12,191,179,162,241,81,51,145,235,249,14,239,
		107,49,192,214,31,181,199,106,157,184,84,204,176,115,121,50,45,127,4,150,254,
		138,236,205,93,222,114,67,29,24,72,243,141,128,195,78,66,215,61,156,180
	};
	
	
	static int[][] grad3;
	
	
	static Vector3[] grad3_Vectors = {
		new Vector3( 1, 1, 0), new Vector3(-1, 1, 0), new Vector3( 1,-1, 0), new Vector3(-1,-1, 0),
		new Vector3( 1, 0, 1), new Vector3(-1, 0, 1), new Vector3( 1, 0,-1), new Vector3(-1, 0,-1),
		new Vector3( 0, 1, 1), new Vector3( 0,-1, 1), new Vector3( 0, 1,-1), new Vector3( 0,-1,-1)
	};
	
	//Why recalculate this every fucking time it's called?
	const float SQRT3 = 1.73205080757f;
	
	public int octaves = 4;
	public float persistence = .5f;
	public float scale = 1;
	public float octaveScale = 2;
	int[] perm = stdPerm;
	public int[] perms { get { return perm; } set { perm = value; } }
	
	//Stupid goddamn C# doesn't support 2d array initializing, so I have to cheat.
	static SimplexNoise() {
		grad3 = new int[12][];
		for (int i = 0; i < grad3.Length; i++) { 
			grad3[i] = new int[3]; 
			grad3[i][0] = (int)grad3_Vectors[i].x;
			grad3[i][1] = (int)grad3_Vectors[i].y;
			grad3[i][2] = (int)grad3_Vectors[i].z;
		}
		
		
	}
	
	public SimplexNoise() {
		octaves = 4;
		persistence = .5f;
		scale = 1;
		octaveScale = 2;
		perm = stdPerm;
	}
	
	public SimplexNoise(SimplexNoise other) {
		octaves = other.octaves;
		persistence = other.persistence;
		scale = other.scale;
		octaveScale = other.octaveScale;
		perm = other.perm;
		
	}
	
	public SimplexNoise Clone() { return new SimplexNoise(this); }
	
	public Texture2D GetSplatMap(Vector2 start, Vector2 end, int size) {
		Texture2D splatmap = new Texture2D(size+1, size+1, TextureFormat.ARGB32, true);
		
		splatmap.SetPixels(GetSplats(start, end, size));
		splatmap.wrapMode = TextureWrapMode.Clamp;
		splatmap.filterMode = FilterMode.Bilinear;
		splatmap.Apply();
		splatmap.filterMode = FilterMode.Trilinear;
		splatmap.Apply();
		return splatmap;
	}
	
	
	
	public Color[] GetSplats(Vector2 start, Vector2 end, int size) {
		Color[] splats = new Color[(size + 1) * (size + 1)];
		
		float sz = (float)size;
		Vector2 dist = (end - start) / sz;
		int i = 0;
		
		for (int yy = 0; yy <= size; yy++) {
			for (int xx = 0; xx <= size; xx++) {
				
				//Make the grids for each color a different size, to make them seem more 'random'
				Vector2 pr = start + new Vector2(dist.x * xx, dist.y * yy);
				if (pr.magnitude < .01f) { pr = new Vector2(.1f, .1f); }
				Vector2 pg = pr * 2f;
				Vector2 pb = pr * 3f;
				Vector2 pa = pr * 4f;
				
				
				//Grab a number representing each color for this location
				float r = OctaveNoise2D(pr);
				float g = OctaveNoise2D(pg);
				float b = OctaveNoise2D(pb);
				float a = OctaveNoise2D(pa);
				
				//Square all color amounts to make sure they are positive, and make the transitions smoother.
				r = r * r; g = g * g; b = b * b; a = a * a;
				
				//Normalize all values
				float t = r + g + b + a;
				r /= t;
				g /= t;
				b /= t;
				a /= t;
				
				splats[i++] = new Color(r, g, b, a);
				
			}
		}
		
		return splats;
	}
	
	
	public Vector3[,] GetNormals(Vector2 start, Vector2 end, int size) {
		Vector3[,] normals = new Vector3[size+1, size+1];
		
		float sz = (float)size;
		Vector2 dist = (end - start) / sz;
		
		for (int yy = 0; yy <= size; yy++) {
			for (int xx = 0; xx <= size; xx++) {
				Vector2 p01 = start + new Vector2((xx-1) * dist.x, yy * dist.y);
				Vector2 p21 = start + new Vector2((xx+1) * dist.x, yy * dist.y);
				Vector2 p10 = start + new Vector2(xx * dist.x, (yy-1) * dist.y);
				Vector2 p12 = start + new Vector2(xx * dist.x, (yy+1) * dist.y);
				
				float s01 = OctaveNoise2D(p01);
				float s21 = OctaveNoise2D(p21);
				float s10 = OctaveNoise2D(p10);
				float s12 = OctaveNoise2D(p12);
				Vector3 va = new Vector3(.5f, s21 - s01, 0).normalized;
				Vector3 vb = new Vector3(0, s12 - s10, .5f).normalized;
				
				normals[xx, yy] = Vector3.Cross(vb, va);
			}
		}
		
		return normals;
	}
	
	
	public float[,] GetHeights(Vector2 start, Vector2 end, int size) {
		float[,] heights = new float[size+1, size+1];
		
		float sz = (float)size;
		Vector2 dist = (end - start) / sz;
		
		for (int yy = 0; yy <= size; yy++) {
			for (int xx = 0; xx <= size; xx++) {
				Vector2 position = start + new Vector2(xx * dist.x, yy * dist.y);
				
				heights[xx, yy] = OctaveNoise2D(position, 0, 1);
			}
		}
		
		return heights;
	}
	
	public Texture2D GetTexture(Vector2 start, Vector2 end, int size) {
		Texture2D tex = new Texture2D(size, size);
		
		float sz = (float)size;
		Color[] colors = new Color[size*size];
		int i = 0;
		for (int yy = 0; yy < size; yy++) {
			for (int xx = 0; xx < size; xx++) {
				Vector2 position = new Vector2(xx, yy) / sz;
				float val = OctaveNoise2D(position, 0, 1);
				colors[i] = new Color(val, val, val, 1);
				i++;
			}
		}
		
		tex.SetPixels(colors);
		tex.Apply();
		return tex;
		
	}
	
	public float GetValue(float x, float y) { return OctaveNoise2D(new Vector2(x, y), 0, 1); }
	public float GetValue(Vector2 position) { return OctaveNoise2D(position, 0, 1); }
	public float OctaveNoise2D(Vector2 position, float min, float max) {
		float val = OctaveNoise2D(position);
		return min + ((val + 1f) / 2f) * (max-min);
	}
	
	
	
	public float OctaveNoise2D(Vector2 position) {
		float total = 0;
		float frequency = scale;
		float amplitude = 1;
		
		float maxAmplitude = 0;
		
		for (int i = 0; i < octaves; i++) {
			total += RawNoise2D(position * frequency) * amplitude;
			
			frequency *= octaveScale;
			maxAmplitude += amplitude;
			amplitude *= persistence;
		}
		
		return total / maxAmplitude;
	}
	
	
	//2D simplex noise function
	//Uses a triangle grid.
	const float F2 = .5f * (SQRT3 - 1.0f);
	const float G2 = (3.0f - SQRT3) / 6.0f;
	public float RawNoise2D(Vector2 position) {
		//Noise contributions from the corners
		float x = position.x;
		float y = position.y;
		
		float n0 = 0, n1 = 0, n2 = 0;
		
		//Skew the intput space to determine what simplex cell it is in.
		//Hairy factor for 2d
		float s = (x + y) * F2;
		
		int i = FastFloor(x + s);
		int j = FastFloor(y + s);
		
		float t = (i + j) * G2;
		//Unskew back into normal space
		float X0 = i-t;
		float Y0 = j-t;
		//The x,y distance from the cell's origin
		float x0 = x-X0;
		float y0 = y-Y0;
		
		// For the 2D case, the simplex shape is an equilateral triangle.
		// Determine which simplex we are in.
		int i1, j1; // Offsets for second (middle) corner of simplex in (i,j) coords
		if(x0>y0) {i1=1; j1=0;} // lower triangle, XY order: (0,0)->(1,0)->(1,1)
		else {i1=0; j1=1;} // upper triangle, YX order: (0,0)->(0,1)->(1,1)

		// A step of (1,0) in (i,j) means a step of (1-c,-c) in (x,y), and
		// a step of (0,1) in (i,j) means a step of (-c,1-c) in (x,y), where
		// c = (3-sqrt(3))/6
		float x1 = x0 - i1 + G2; // Offsets for middle corner in (x,y) unskewed coords
		float y1 = y0 - j1 + G2;
		float x2 = x0 - 1.0f + 2.0f * G2; // Offsets for last corner in (x,y) unskewed coords
		float y2 = y0 - 1.0f + 2.0f * G2;

		// Work out the hashed gradient indices of the three simplex corners
		int ii = i & 255;
		int jj = j & 255;
		int gi0 = perm[ii+perm[jj]] % 12;
		int gi1 = perm[ii+i1+perm[jj+j1]] % 12;
		int gi2 = perm[ii+1+perm[jj+1]] % 12;
		
		
		// Calculate the contribution from the three corners
		float t0 = 0.5f - x0*x0-y0*y0;
		if(t0>0) {
			t0 *= t0;
			n0 = t0 * t0 * Dot(grad3[gi0], x0, y0); // (x,y) of grad3 used for 2D gradient
		}

		float t1 = 0.5f - x1*x1-y1*y1;
		if(t1>0) {
			t1 *= t1;
			n1 = t1 * t1 * Dot(grad3[gi1], x1, y1);
		}

		float t2 = 0.5f - x2*x2-y2*y2;
		if(t2>0) {
			t2 *= t2;
			n2 = t2 * t2 * Dot(grad3[gi2], x2, y2);
		}
		
		//Add contributions from each corner to get the final noise value.
		//The result is scaled to return values in the interval [-1,1].
		return 70.0f * (n0 + n1 + n2);
	}
	
	int FastFloor(float f) { return f > 0 ? (int) f : (int) f - 1; }
	
	//Dot with some gradient.
	//In this case, it's using the x and y values of the 3d gradient.
	float Dot(int[] g, float x, float y) { return g[0]*x + g[1] * y; }
}



































