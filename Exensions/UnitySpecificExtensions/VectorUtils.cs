using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System;

public static class VectorUtils {

	public static float Nearest(float f, float value) {
		int m = (int)(f / value);
		float frac = (f % value) / value;
		
		float floor = m * value;
		float ceil = (m+1) * value;
		return (frac < .5f) ? floor : ceil;
	}

	public static Quaternion Round(this Quaternion qt, float amt = 90f) {
		Vector3 euler = qt.eulerAngles;
		
		euler = new Vector3(Nearest(euler.x, amt), Nearest(euler.y, amt), Nearest(euler.z, amt));

		return Quaternion.Euler(euler);
	}
	
	
	//returns a Vector2 at a 45 degree angle rotated by 90 degrees i times.
	public static Vector2 SpinVector2(int i) {
		float x = (i 	 % 4 <= 1) ? 1 : -1;
		float y = ((1+i) % 2 == 1) ? 1 : -1;
		return new Vector2(x, y);
	}

	public static float DistanceTo(this Vector3 a, Vector3 b) {
		return (b - a).magnitude;
	}
	
	public static int Hash(this Vector2 v) {
		int x = v.x.GetHashCode();
		int y = v.y.GetHashCode();
		y = (y << 16) | ((y > 0 ? y : -y) >> 16);
		return ~x ^ y;
	}
	
	public static Vector2 BiggestDifferenceTo(this Vector2 v, Vector2 other) {
		Vector2 diff = other - v;
		if (diff.x.Abs() > diff.y.Abs()) { diff.y = 0; }
		else { diff.x = 0; }
		return diff;
	}
	
	public static Vector2 ApplyFriction(this Vector2 v, float f) {
		float m = v.magnitude;
		m = Mathf.Max(m-f, 0);
		return v.normalized * m;
	}
	
	public static Vector2 TLerp(this Vector2 v, float t) { return v.TLerp(Vector2.zero, t); }
	public static Vector2 TLerp(this Vector2 v, Vector2 target) { return v.TLerp(target, 1); }
	public static Vector2 TLerp(this Vector2 v, Vector2 target, float t) { return v.Lerp(target, Time.deltaTime * t); }
	public static Vector2 Lerp(this Vector2 v, Vector2 target, float t) { return Vector2.Lerp(v, target, t); }
	
	
	public static Vector3 ApplyFriction(this Vector3 v, Vector3 f) {
		Vector3 m = v.Abs();
		m.x = v.x.Sign() * Mathf.Max(m.x-f.x, 0);
		m.y = v.y.Sign() * Mathf.Max(m.y-f.y, 0);
		m.z = v.z.Sign() * Mathf.Max(m.z-f.z, 0);
		return m;
	}
	
	public static Vector3 ApplyFriction(this Vector3 v, float f) {
		float m = v.magnitude;
		m = Mathf.Max(m-f, 0);
		return v.normalized * m;
	}
	
	public static byte[] GetBytes(this Vector3 v) {
		List<byte> b = new List<byte>();
		b.Append(BitConverter.GetBytes(v.x));
		b.Append(BitConverter.GetBytes(v.y));
		b.Append(BitConverter.GetBytes(v.z));
		
		//Debug.Log("Vector3 to byte[]: " + v + " - " + b.Count);
		
		return b.ToArray();
	}
	
	public static Vector3 Abs(this Vector3 v) { return new Vector3(v.x.Abs(), v.y.Abs(), v.z.Abs()); }
	public static Vector3 Sign(this Vector3 v) { return new Vector3(v.x.Sign(), v.y.Sign(), v.z.Sign()); }
	public static Vector3 Floor(this Vector3 v) { return new Vector3(v.x.Floor(), v.y.Floor(), v.z.Floor()); }
	
	public static Vector3 TLerp(this Vector3 v, float t) { return v.TLerp(Vector3.zero, t); }
	public static Vector3 TLerp(this Vector3 v, Vector3 target) { return v.TLerp(target, 1); }
	public static Vector3 TLerp(this Vector3 v, Vector3 target, float t) { return v.Lerp(target, Time.deltaTime * t); }
	public static Vector3 Lerp(this Vector3 v, Vector3 target, float t) { return Vector3.Lerp(v, target, t); }

	public static Vector2 Clamp(this Vector2 v, Vector2 min, Vector2 max) {
		return v.ClampX(min.x, max.x).ClampY(min.y, max.y);
	}
	public static Vector2 ClampX(this Vector2 v, float min, float max) { return new Vector2(Mathf.Clamp(v.x, min, max), v.y); }
	public static Vector2 ClampY(this Vector2 v, float min, float max) { return new Vector2(v.x, Mathf.Clamp(v.y, min, max)); }
	
	public static Vector3 ClampX(this Vector3 v, float min, float max) { return new Vector3(Mathf.Clamp(v.x, min, max), v.y, v.z); }
	public static Vector3 ClampY(this Vector3 v, float min, float max) { return new Vector3(v.x, Mathf.Clamp(v.y, min, max), v.z); }
	public static Vector3 ClampZ(this Vector3 v, float min, float max) { return new Vector3(v.x, v.y, Mathf.Clamp(v.z, min, max)); }
	
	public static Vector3 Clamp(this Vector3 v, Vector3 a, Vector3 b) {
		return new Vector3(v.x.Clamp(a.x, b.x), v.y.Clamp(a.y, b.y), v.z.Clamp(a.z, b.z));
	}
	
	public static Vector3 Clamp(this Vector3 v, Bounds bounds) {
		Vector3 c = v;
		if (c.x < bounds.min.x) { c.x = bounds.min.x; }
		if (c.y < bounds.min.y) { c.y = bounds.min.y; }
		if (c.z < bounds.min.z) { c.z = bounds.min.z; }
		
		if (c.x > bounds.max.x) { c.x = bounds.max.x; }
		if (c.y > bounds.max.y) { c.y = bounds.max.y; }
		if (c.z > bounds.max.z) { c.z = bounds.max.z; }
		return c;
	}
	
}
