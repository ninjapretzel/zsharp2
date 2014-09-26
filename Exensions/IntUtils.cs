using UnityEngine;
using System.Collections;
using System;

public static class IntUtils {
	public static bool IsOdd(this int i) { return i % 2 == 1; }
	public static bool IsEven(this int i) { return i % 2 == 0; }
	
	public static int MidA(this int i) { return -1 + (i + 1) / 2; }
	public static int MidB(this int i) { return i/2; }
	
	public static byte[] GetBytes(this int i) { return BitConverter.GetBytes(i); }
	
}
