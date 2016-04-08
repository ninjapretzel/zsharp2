using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public static class QuaternionUtils {
	
	//Get the rotation that would be multiplied by to rotate a to b
	public static Quaternion To(this Quaternion a, Quaternion b) { return Quaternion.Inverse(a) * b; }
	
	public static Quaternion FlattenZ(this Quaternion q) {
		Vector3 e = q.eulerAngles;
		e.x = 0;
		return Quaternion.Euler(e);
	}
	
	public static Quaternion FlattenY(this Quaternion q) {
		Vector3 e = q.eulerAngles;
		e.x = 0;
		return Quaternion.Euler(e);
	}
	
	public static Quaternion FlattenX(this Quaternion q) {
		Vector3 e = q.eulerAngles;
		e.z = 0;
		return Quaternion.Euler(e);
	}
	
	public static byte[] GetBytes(this Quaternion q) {
		List<byte> b = new List<byte>();
		
		b.AddAll(q.x.GetBytes());
		b.AddAll(q.y.GetBytes());
		b.AddAll(q.z.GetBytes());
		b.AddAll(q.w.GetBytes());
		
		return b.ToArray();
	}
	
	
	
}
