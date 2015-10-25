using UnityEngine;
using System.Collections;

public class SoundVolume : MonoBehaviour {
	float baseVolume;
	
	void Awake() {
		baseVolume = GetComponent<AudioSource>().volume;
	}
	
	void Start() {
		SetVolume();
	}
	
	
	void Update() {
		SetVolume();
	}
	
	void SetVolume() {
		GetComponent<AudioSource>().volume = baseVolume * Settings.instance.soundVolume;
	}
	
}
