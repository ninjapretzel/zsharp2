using UnityEngine;
using System.Collections;

public class AutodestructSound : MonoBehaviour {
	void Awake() { DontDestroyOnLoad(gameObject); }
	void Start() { DontDestroyOnLoad(gameObject); }
	void LateUpdate() {
		if (GetComponent<AudioSource>()) {
			if (!GetComponent<AudioSource>().isPlaying) { Destroy(gameObject); }
		} else { Destroy(gameObject); }
	}
}