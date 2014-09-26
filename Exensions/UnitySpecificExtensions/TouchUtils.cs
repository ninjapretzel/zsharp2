using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class TouchUtils {

	public static Vector2 ScreenPosition(this Touch t) {
		Vector2 v = t.position;
		v.y = Screen.height - v.y;
		return v;
	}
	
	public static bool IsPress(this Touch t) { return t.phase.IsPress(); }
	public static bool IsHold(this Touch t) { return t.phase.IsHold(); }
	public static bool IsRelease(this Touch t) { return t.phase.IsRelease(); }
	
	public static bool Began(this Touch t) { return t.phase.IsBegan(); }
	public static bool Moved(this Touch t) { return t.phase.IsMoved(); }
	public static bool Stationary(this Touch t) { return t.phase.IsStationary(); }
	public static bool Ended(this Touch t) { return t.phase.IsEnded(); }
	public static bool Canceled(this Touch t) { return t.phase.IsCanceled(); }
	
	public static bool IsPress(this TouchPhase t) { return t == TouchPhase.Began; }
	public static bool IsHold(this TouchPhase t) { return t == TouchPhase.Moved || t == TouchPhase.Stationary; } 
	public static bool IsRelease(this TouchPhase t) { return t == TouchPhase.Ended || t == TouchPhase.Canceled; } 
	

	public static bool IsBegan(this TouchPhase t) { return t == TouchPhase.Began; }
	public static bool IsMoved(this TouchPhase t) { return t == TouchPhase.Moved; }
	public static bool IsStationary(this TouchPhase t) { return t == TouchPhase.Stationary; }
	public static bool IsEnded(this TouchPhase t) { return t == TouchPhase.Ended; }
	public static bool IsCanceled(this TouchPhase t) { return t == TouchPhase.Canceled; }
	
	
}



















