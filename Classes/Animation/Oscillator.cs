using UnityEngine;
using System.Collections;

public enum OscillatorMode { Cos, Sin, Line, Square }

[System.Serializable]
public class Oscillator {
	public OscillatorMode mode = OscillatorMode.Cos;
	public float maxTime = 1.0f;
	public float curTime = 0.0f;
	public float minVal = 0.0f;
	public float maxVal = 1.0f;
	
	public float value { get; private set; }
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

