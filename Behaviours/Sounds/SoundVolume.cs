using UnityEngine;
using System.Collections;

public class SoundVolume : MonoBehaviour {
	float baseVolume;
	
	void Awake() {
		baseVolume = audio.volume;
	}
	
	void Start() {
		SetVolume();
	}
	
	
	void Update() {
		SetVolume();
	}
	
	void SetVolume() {
		audio.volume = baseVolume * Settings.soundVolume;
	}
	
}
