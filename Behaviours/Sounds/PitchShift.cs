using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PitchShift : MonoBehaviour {

	public float min = -.1f;
	public float max = .1f;

	void Awake() {
		GetComponent<AudioSource>().pitch += Random.Range(min, max);
		Destroy(this);
	}
	
}
