using UnityEngine;
using System.Collections;

/// <summary> Represents a spring with a current position, equilibrium position, and eases towards equilibrium over time. </summary>
[System.Serializable]
public class Spring {
	/// <summary> Current position </summary>
	[Tooltip("Current position")]
	public float value;

	/// <summary> Equilibrium position </summary>
	[Tooltip("Equilibrium position")]
	public float target;

	/// <summary> Rate at which easing occurs</summary>
	[Tooltip("Rate at which easing occurs")] 
	public float dampening;
	
	public Spring() { Defaults(); }
	public Spring(float v) { Defaults(); value = v; target = v; }
	public Spring(float v, float d) { value = v; dampening = d; target = v; }
	public Spring(float v, float d, float t) {
		value = v;
		dampening = d;
		target = t;
	}
	
	/// <summary> Set the spring to default values (at 0, target 0, 1x dampening) </summary>
	public void Defaults() { value = 0; dampening = 1; target = 0; }
	
	/// <summary> If the distance to target is less than gap, set value to target.</summary>
	public void JumpGap(float gap = .02f) { 
		if (Mathf.Abs(target-value) < gap) { value = target; } 
	}
	
	/// <summary> Update the spring and get the next value. </summary>
	public float Update() { 
		value = Mathf.Lerp(value, target, dampening * Time.deltaTime); 
		JumpGap();
		return value; 
	}

	/// <summary> Update the spring towards a given value, and get the next value. </summary>
	public float Update(float t) { target = t; return Update(); }
	
}
