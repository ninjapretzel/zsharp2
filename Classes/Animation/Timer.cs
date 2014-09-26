using UnityEngine;
using System.Collections;

[System.Serializable]
public class Timer {
	
	public float time = 3;
	float timeout = 0;
	
	public Timer() {
		time = 3;
		timeout = 0;
	}
	
	public Timer(float t) {
		time = t;
		timeout = 0;
	}
	
	public virtual bool Update() {
		float t = timeout;
		timeout += Time.deltaTime;
		
		return (t < time && timeout >= time);
	}
	
	public void Reset() {
		timeout = 0;
	}
	
	
}
