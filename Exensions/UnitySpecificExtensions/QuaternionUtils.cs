using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

public static class QuaternionUtils {
	
	//Get the rotation that would be multiplied by to rotate a to b
	public static Quaternion To(this Quaternion a, Quaternion b) { return Quaternion.Inverse(a) * b; }
	
	
	/// <summary> Returns a copy of the quaternion with the eulerAngles Z component set to zero</summary>
	/// <param name="q">Source rotation</param>
	/// <returns>Rotation built from source rotation with the eulerAngles Z component set to zero</returns>
	public static Quaternion FlattenZ(this Quaternion q) {
		Vector3 e = q.eulerAngles;
		e.x = 0;
		return Quaternion.Euler(e);
	}
	
	/// <summary> Returns a copy of the quaternion with the eulerAngles Y component set to zero</summary>
	/// <param name="q">Source rotation</param>
	/// <returns>Rotation built from source rotation with the eulerAngles Y component set to zero</returns>
	public static Quaternion FlattenY(this Quaternion q) {
		Vector3 e = q.eulerAngles;
		e.x = 0;
		return Quaternion.Euler(e);
	}

	/// <summary> Returns a copy of the quaternion with the eulerAngles X component set to zero</summary>
	/// <param name="q">Source rotation</param>
	/// <returns>Rotation built from source rotation with the eulerAngles X component set to zero</returns>
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
