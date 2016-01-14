using UnityEngine;
using System.Collections;

/// <summary> Modes of Oscillation for the Oscillator class.</summary>
public enum OscillatorMode { Cos, Sin, Line, Square }

/// <summary> Oscilates a value over time. </summary>
[System.Serializable]
public class Oscillator {
	/// <summary> Mode of Oscillation between minVal and maxVal </summary>
	[Tooltip("Mode of Oscillation between minVal and maxVal")]
	public OscillatorMode mode = OscillatorMode.Cos;

	[Tooltip("Time of oscillation")]
	/// <summary> Time of oscillation </summary>
	public float maxTime = 1.0f;

	/// <summary> Current time in oscillation</summary>
	[System.NonSerialized] public float curTime;

	/// <summary> Value at curTime == 0 </summary>
	[Tooltip("Value at curTime == 0")]
	public float minVal = 0.0f;
	/// <summary> Value at curTime == maxTime </summary>
	[Tooltip("Value at curTime == maxTime")]
	public float maxVal = 1.0f;

	/// <summary> Current value of the oscillator </summary>
	public float value { get; private set; }
	/// <summary> Direction (up/down) </summary>
	private bool up = false;
	
	public Oscillator() {
		minVal = 0.0f;
		maxVal = 1.0f;
		maxTime = 1.0f;
	}
	
	public Oscillator(float min, float max, float time) {
		minVal = min;
		maxVal = max;
		maxTime = time;
	}
	
	public Oscillator(float min, float max, float time, float start) {
		minVal = min;
		maxVal = max;
		maxTime = time;
		curTime = start;
	}
	
	/// <summary> Update the oscillator and get the next value. </summary>
	public float Update() {
		if (up) {
			curTime += Time.deltaTime;
			if (curTime > maxTime) {
				up = !up;
				curTime = maxTime * 2 - curTime;
			}
		} else {
			curTime -= Time.deltaTime;
			if (curTime < 0) {
				up = !up;
				curTime = -curTime;
			}
		}
		
		float p = curTime / maxTime;
		if (mode == OscillatorMode.Square) {
			p *= p;
		} else if (mode == OscillatorMode.Sin) {
			p = Mathf.Sin(p * Mathf.PI);
		} else if (mode == OscillatorMode.Cos) {
			p = (1.0f + Mathf.Cos(p * Mathf.PI)) / 2.0f;
		}
		
		value = (minVal + p * (maxVal - minVal));
		return value;
	}
}

