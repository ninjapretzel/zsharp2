using UnityEngine;
using System.Collections;

public class PlaySoundOnStart : MonoBehaviour {
	public string sound = "";
	
	void Start() { 
		if (sound != null && sound != "") {
			Sounds.Play(sound, transform.position);
			
			AudioSource src = GetComponent<AudioSource>();
			if (src != null) {
				src.Play();
			}
			
		}
		
	}
	
}
