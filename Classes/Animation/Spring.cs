using UnityEngine;
using System.Collections;

[System.Serializable]
public class Spring {
	public float value;
	public float target;
	public float dampening;
	
	public Spring() { Defaults(); }
	public Spring(float v) { Defaults(); value = v; target = v; }
	public Spring(float v, float d) { value = v; dampening = d; target = v; }
	public Spring(float v, float d, float t) {
		value = v;
		dampening = d;
		target = t;
	}
	
	public void Defaults() { value = 0; dampening = 1; target = 0; }
	
	public void JumpGap(float gap = .02f) { 
		if (Mathf.Abs(target-value) < gap) { value = target; } 
	}
	
	public float Update() { 
		value = Mathf.Lerp(value, target, dampening * Time.deltaTime); 
		JumpGap();
		return value; 
	}
	public float Update(float t) { target = t; return Update(); }
	
}
