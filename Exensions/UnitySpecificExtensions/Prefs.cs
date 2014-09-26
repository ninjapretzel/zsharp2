using UnityEngine;
using System.Collections;



public static class Prefs {
	
	#region basics
	public static void DeleteAll() { PlayerPrefs.DeleteAll(); }
	public static void DeleteKey(string key) { PlayerPrefs.DeleteKey(key); }
	
	public static float GetFloat(string key) { return PlayerPrefs.GetFloat(key); }
	public static int GetInt(string key) { return PlayerPrefs.GetInt(key); }
	public static string GetString(string key) { return PlayerPrefs.GetString(key); }
	
	public static bool HasKey(string key) { return PlayerPrefs.HasKey(key); }
	public static void Save() { PlayerPrefs.Save(); }
	
	public static void SetFloat(string key, float value) { PlayerPrefs.SetFloat(key, value); }
	public static void SetInt(string key, int value) { PlayerPrefs.SetFloat(key, value); }
	public static void SetString(string key, string value) { PlayerPrefs.SetString(key, value); }
	
	#endregion
	
	public static void Save(this string[] ray, string name) {
		PlayerPrefs.SetInt(name + "_length", ray.Length);
		for (int i = 0; i < ray.Length; i++) {
			PlayerPrefs.SetString(name + "_" + i, ray[i]);
		}
	}
	
	public static void Save(this float[] ray, string name) {
		PlayerPrefs.SetInt(name + "_length", ray.Length);
		for (int i = 0; i < ray.Length; i++) {
			PlayerPrefs.SetFloat(name + "_" + i, ray[i]);
		}
	}
	
	public static void Save(this int[] ray, string name) {
		PlayerPrefs.SetInt(name + "_length", ray.Length);
		for (int i = 0; i < ray.Length; i++) {
			PlayerPrefs.SetInt(name + "_" + i, ray[i]);
		}
	}
	
	public static void Save(this bool[] ray, string name) {
		PlayerPrefs.SetInt(name + "_length", ray.Length);
		for (int i = 0; i < ray.Length; i++) {
			int val = 0; if (ray[i]) { val = 1; }
			PlayerPrefs.SetInt(name + "_" + i, val);
		}
	}
	
	
	public static void SetBool(string name, bool b) { PlayerPrefs.SetInt(name, (b ? 1 : 0) ); }
	
	public static void SetIntArray(string name, int[] ray) { ray.Save(name); }
	public static void SetFloatArray(string name, float[] ray) { ray.Save(name); }
	public static void SetStringArray(string name, string[] ray) { ray.Save(name); }
	public static void SetBooleanArray(string name, bool[] ray) { ray.Save(name); }
	
	public static bool GetBool(string name) {
		return PlayerPrefs.GetInt(name) == 1;
	}
	
	public static int[] GetIntArray(string name, int[] target) {
		int[] test = GetIntArray(name);
		if (test != null && test.Length > 0) { return test; }
		return target;
	}
	public static int[] GetIntArray(string name) {
		if (!PlayerPrefs.HasKey(name + "_length")) { return null; }
		int[] ray = new int[PlayerPrefs.GetInt(name + "_length")];
		for (int i = 0; i < ray.Length; i++) {
			ray[i] = PlayerPrefs.GetInt(name + "_" + i);
		}
		return ray;
	}
	
	
	public static float[] GetFloatArray(string name, float[] target) {
		float[] test = GetFloatArray(name);
		if (test != null && test.Length > 0) { return test; }
		return target;
	}
	public static float[] GetFloatArray(string name) {
		if (!PlayerPrefs.HasKey(name + "_length")) { return null; }
		float[] ray = new float[PlayerPrefs.GetInt(name + "_length")];
		for (int i = 0; i < ray.Length; i++) {
			ray[i] = PlayerPrefs.GetFloat(name + "_" + i);
		}
		return ray;
	}
	
	
	public static bool[] GetBooleanArray(string name, bool[] target) {
		bool[] test = GetBooleanArray(name);
		if (test != null && test.Length > 0) { return test; }
		return target;
	}
	public static bool[] GetBooleanArray(string name) {
		if (!PlayerPrefs.HasKey(name + "_length")) { return null; }	
		bool[] ray = new bool[PlayerPrefs.GetInt(name + "_length")];
		for (int i = 0; i < ray.Length; i++) {
			ray[i] = PlayerPrefs.GetInt(name + "_" + i) == 1;
		}
		return ray;
	}
	
	public static string[] GetStringArray(string name, string[] target) {
		string[] test = GetStringArray(name);
		if (test != null && test.Length > 0) { return test; }
		return target;
	}
	public static string[] GetStringArray(string name) {
		if (!PlayerPrefs.HasKey(name + "_length")) { return null; }	
		string[] ray = new string[PlayerPrefs.GetInt(name + "_length")];
		for (int i = 0; i < ray.Length; i++) {
			ray[i] = PlayerPrefs.GetString(name + "_" + i);
		}
		return ray;
	}
	
}



























