using UnityEngine;
using System.Collections;

public class V3 {
	public float x;
	public float y;
	public float z;
	
	public static V3 zero { get { return new V3(0, 0, 0); } }
	public static V3 one { get { return new V3(1, 1, 1); } }
	
	public static V3 up { get { return new V3(0, 1, 0); } }
	public static V3 down { get { return new V3(0, -1, 0); } }
	
	public static V3 right { get { return new V3(1, 0, 0); } }
	public static V3 left { get { return new V3(-1, 0, 0); } }
	
	public static V3 forward { get { return new V3(0, 0, 1); } }
	public static V3 backward { get { return new V3(0, 0, -1); } }
	
	public V3() { x = 0; y = 0; z = 0; }
	public V3(float f) { x = f; y = f; z = f; }
	public V3(float a, float b, float c) { x = a; y = b; z = c; }
	public V3(V3 v) { x = v.x; y = v.y; z = v.z; }
	
	public static V3 operator + (V3 a, V3 b) { return new V3(a.x + b.x, a.y + b.y, a.z + b.z); }
	public static V3 operator - (V3 a, V3 b) { return new V3(a.x - b.x, a.y - b.y, a.z - b.z); }
	public static V3 operator * (V3 a, float b) { return new V3(a.x * b, a.y * b, a.z * b); }
	public static V3 operator / (V3 a, float b) { return new V3(a.x / b, a.y / b, a.z / b); }
	
	public V3 normalized {
		get {
			float m = magnitude;
			if (m > 0) { return new V3(x/m, y/m, z/m); }
			return this;
		}
		
		set {
			float m = magnitude;
			V3 v = value.normalized;
			x = v.x * m;
			y = v.y * m;
			z = v.z * m;
		}
	}
	
	public float magnitude {
		get {
			return Mathf.Sqrt(x*x + y*y + z*z);
		}
		
		set {
			V3 n = normalized;
			x = value * n.x;
			y = value * n.y;
			z = value * n.z;
		}
	}
	
	
	public void Normalize() {
		float m = magnitude;
		if (m > 0) {
			x /= m;
			y /= m;
			z /= m;
		}
	}
	
	public float Dot(V3 other) { return Dot(this, other); }
	public static float Dot(V3 a, V3 b) { return (a.x * b.x + a.y * b.y + a.z * b.z); }
	
	public V3 Cross(V3 other) { return Cross(this, other); }
	public static V3 Cross(V3 a, V3 b) {
		return new V3(	a.y * b.z - a.z * b.y,
						a.z * b.x - a.x * b.z,
						a.x * b.y - a.y * b.x);
	}
	
	public static V3 Scale(V3 a, V3 b) { return new V3(a.x * b.x, a.y * b.y, a.z * b.z); }
	public static V3 Scale(V3 a, float f) { return new V3(a.x * f, a.y * f, a.z * f); }
	
	public void Scale(float f) { Scale(new V3(f)); }
	public void Scale(V3 other) { 
		x *= other.x;
		y *= other.y;
		z *= other.z;
	}
	
	public float Angle(V3 other) { return Angle(this, other); }
	public static float Angle(V3 a, V3 b) {
		return Mathf.Acos(Dot(a, b) / Mathf.Pow(a.magnitude, 2));
	}

	public V3 Ortho(V3 other) { return Ortho(this, other); }
	public static V3 Ortho(V3 a, V3 b) {
		return a * (a.Dot(b) / a.Dot(a));
	}
	
	public V3 Project(V3 other) { return Project(this, other); }
	public static V3 Project(V3 a, V3 b) {
		return a.normalized * (a.Dot(b) / a.magnitude);
	}
	
	public static V3 Normal(V3 a, V3 b, V3 c) {
		return Cross(a - b, a - c).normalized;
	}
	
	public bool OnPlane(V3 a, V3 n) { return OnPlane(this, a, n); }
	public static bool OnPlane(V3 p, V3 a, V3 n) {
		return Mathf.Abs(Dot(n, p - a)) < .0001;
	}
	
	public static V3 Lerp(V3 a, V3 b, float amount) {
		float p = Mathf.Clamp01(amount);
		V3 dir = b - a;
		return a + (dir * p);
	}
	
}























