using UnityEngine;
using System.Collections;

public class PlaySoundOnStart : MonoBehaviour {
	public string sound = "";
	// Use this for initialization
	void Start() { SoundMaster.Play(sound); }

}
