
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

///Class holding all extension methods for Unity's 'Bounds' class
public static class BoundsUtils {
	
	///Check a single axis not being contained in the bounds
	public static bool IsXout(this Bounds b, Vector3 v) { return v.x < b.min.x || v.x > b.max.x; }
	public static bool IsYout(this Bounds b, Vector3 v) { return v.y < b.min.y || v.y > b.max.y; }
	public static bool IsZout(this Bounds b, Vector3 v) { return v.z < b.min.z || v.z > b.max.z; }
	
	///Get the distance a single axis is outside of the bounds
	///Is zero if the point is inside the bounds.
	public static float	Xout(this Bounds b, Vector3 v) { return v.x.Outside(b.min.x, b.max.x); }
	public static float	Yout(this Bounds b, Vector3 v) { return v.y.Outside(b.min.y, b.max.y); }
	public static float	Zout(this Bounds b, Vector3 v) { return v.z.Outside(b.min.z, b.max.z); }

	
	///Get various points on the bounds
	public static Vector3 TopCenter(this Bounds b) { return new Vector3(b.center.x, b.max.y, b.center.z); }
	public static Vector3 BottomCenter(this Bounds b) { return new Vector3(b.center.x, b.min.y, b.center.z); }
	public static Vector3 LeftCenter(this Bounds b) { return new Vector3(b.min.x, b.center.y, b.center.z); }
	public static Vector3 RightCenter(this Bounds b) { return new Vector3(b.min.x, b.center.y, b.center.z); }
	public static Vector3 FrontCenter(this Bounds b) { return new Vector3(b.center.x, b.center.y, b.max.z); }
	public static Vector3 BackCenter(this Bounds b) { return new Vector3(b.center.x, b.center.y, b.min.z); }
	
	///Get various coordinates from the bounds
	public static float Top(this Bounds b) { return b.max.y; }
	public static float Bottom(this Bounds b) { return b.min.y; }
	public static float Left(this Bounds b) { return b.min.x; }
	public static float Right(this Bounds b) { return b.max.x; }
	public static float Front(this Bounds b) { return b.max.z; }
	public static float Back(this Bounds b) { return b.min.z; }
	
	///Get a random (evenly distributed) position inside the bounds
	public static Vector3 RandomInside(this Bounds b) {
		Vector3 pos = b.center - b.extents;
		pos.x += b.size.x * Random.value;
		pos.y += b.size.y * Random.value;
		pos.z += b.size.z * Random.value;
		return pos;
	}
	
}
