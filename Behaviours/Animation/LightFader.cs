using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LightFader : MonoBehaviour {
	
	public float endRange = 20;
	public float endIntensity = 0;
	
	public float fadeTime = 1;
	
	float startRange;
	float startIntensity;
	float timeout;
	
	void Start() {
		startIntensity = GetComponent<Light>().intensity;
		startRange = GetComponent<Light>().range;
		timeout = 0;
		
	}
	
	void Update() {
		timeout += Time.deltaTime;
		
		float percent = timeout / fadeTime;
		
		GetComponent<Light>().intensity = startIntensity.Lerp(endIntensity, percent);
		GetComponent<Light>().range = startRange.Lerp(endRange, percent);
		
		
	}
	
}
