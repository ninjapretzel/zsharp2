using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Console {
	
	public static void WriteLine(object o) {
		Debug.Log(o.ToString());
	}
	
}
