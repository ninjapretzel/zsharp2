using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class ClearPlayerPrefs {
	
	[MenuItem("ZSharp/Clear PlayerPrefs")]
	public static void Clear() {
		PlayerPrefs.DeleteAll();
		PlayerPrefs.Save();
		Debug.Log("Player Prefs Cleared");
	}
	
}
