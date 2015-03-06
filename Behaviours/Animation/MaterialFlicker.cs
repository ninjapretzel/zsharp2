using UnityEngine;
using System.Collections;

///Makes a material's color flicker.
///The target color by default is the emissive color, but another one can be specified.
///[0, 1) * randAdd is added to all of the starting color's channels
///if squared is set, it is (([0, 1) * randAdd) * ([0, 1) * randAdd))
///
public class MaterialFlicker : MonoBehaviour {
	public string targetChannel = "_Emission";
	public Color baseColor = Color.white;
	public float randAdd = 0.3f;
	public bool squared = false;
	
	public float flickerTime = 0.2f;
	public float time = 0.0f;
	
	void Start() {
		baseColor = GetComponent<Renderer>().material.GetColor(targetChannel);
		
	}
	
	void Update() {
		time += Time.deltaTime;
		if (time > flickerTime) {
			Flicker();
			time -= flickerTime;
		}
		
	}
	
	public void Flicker() {
		float rnd = 1 + randAdd * Random.value;
		if (squared) { rnd *= 1 + randAdd * Random.value; }
		Color c = baseColor;
		c *= rnd;
		c.a = baseColor.a;
		GetComponent<Renderer>().material.SetColor(targetChannel, c);
	}
	
	public void ResetColor() {
		GetComponent<Renderer>().material.SetColor(targetChannel, baseColor);
	}
	
}

