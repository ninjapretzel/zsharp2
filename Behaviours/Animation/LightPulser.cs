using UnityEngine;
using System.Collections;

///Animates a light's intensity and range over time.
public class LightPulser : MonoBehaviour {
	public Oscillator intensity = new Oscillator(1.0f, 3.0f, 1.0f);
	public Oscillator range = new Oscillator(5.0f, 15.0f, 1.0f);
	public Light target;
	
	public void Awake() {
		if (!target) { target = GetComponent<Light>(); }
	}
	
	public void Update() {
		if (!target) { return; }
		target.range = range.Update();
		target.intensity = intensity.Update();
	}
	
}