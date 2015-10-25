using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MusicVolume : MonoBehaviour {
	
	void Update() {
		GetComponent<AudioSource>().volume = Settings.instance.musicVolume;
	}
	
}




















