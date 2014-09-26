using UnityEngine;
using System.Collections;

public class AutodestructSound : MonoBehaviour {
	void Awake() { DontDestroyOnLoad(gameObject); }
	void Start() { DontDestroyOnLoad(gameObject); }
	void Update() {
		if (audio) {
			if (!audio.isPlaying) { Destroy(gameObject); }
		} else { Destroy(gameObject); }
	}
}