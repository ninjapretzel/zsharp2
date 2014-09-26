using UnityEngine;
using System.Collections;
using System;

public enum LookDirection { Forward, Backward, Up, Down, Left, Right }

public static class LookInDirectionUtils { 
	public static Action<Transform> Rotation(this LookDirection d) { 
		if (d == LookDirection.Forward) { return LookForward; }
		else if (d == LookDirection.Backward) { return LookBackward; }
		else if (d == LookDirection.Up) { return LookUp; }
		else if (d == LookDirection.Down) { return LookDown; }
		else if (d == LookDirection.Left) { return LookLeft; }
		else if (d == LookDirection.Right) { return LookRight; }
		
		return LookForward;
	}
	
	public static void LookForward(this Transform t) { t.localRotation = Quaternion.Euler(new Vector3(0, 0, 0)); }
	public static void LookLeft(this Transform t) { t.localRotation = Quaternion.Euler(new Vector3(0, 90, 0)); }
	public static void LookBackward(this Transform t) { t.localRotation = Quaternion.Euler(new Vector3(0, 180, 0)); }
	public static void LookRight(this Transform t) { t.localRotation = Quaternion.Euler(new Vector3(0, 270, 0)); }
	public static void LookUp(this Transform t) { t.localRotation = Quaternion.Euler(new Vector3(-90, 0, 0)); }
	public static void LookDown(this Transform t) { t.localRotation = Quaternion.Euler(new Vector3(90, 0, 0)); }
}

public class LookInDirection : MonoBehaviour {
	
	public LookDirection direction;
	public Action<Transform> lookInDirection { get { return direction.Rotation(); } }
	
	void Start() {
		lookInDirection(transform);
		Destroy(this);
	}
	
}
