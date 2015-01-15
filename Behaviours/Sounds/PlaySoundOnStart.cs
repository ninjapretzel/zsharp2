using UnityEngine;
using System.Collections;

public class PlaySoundOnStart : MonoBehaviour {
	public string sound = "";
	
	void Start() { Sounds.Play(sound, transform.position); }

}
