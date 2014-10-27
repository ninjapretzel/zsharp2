using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RandomLightColor : MonoBehaviour {
	//public Color baseColor = Color.white;
	public float hueShift = .1f;
	public float satShift = .1f;
	public float valShift = .1f;
	
	
	void Start() {
		Color baseColor = light.color.RGBtoHSV();
		baseColor.r += Random.Range(-hueShift, hueShift);
		baseColor.g += Random.Range(-satShift, satShift);
		baseColor.b += Random.Range(-valShift, valShift);
		light.color = baseColor.HSVtoRGB();
		Destroy(this);
	}
	
	void Update() {
		
	}
	
}
