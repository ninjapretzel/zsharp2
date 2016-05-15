using UnityEngine;
using System.Collections;

public class AutodestructSound : MonoBehaviour {
	void Awake() { if (transform.parent == null) { DontDestroyOnLoad(gameObject); } }
	void Start() { if (transform.parent == null) { DontDestroyOnLoad(gameObject); } }
	void LateUpdate() {
		if (GetComponent<AudioSource>()) {
			if (!GetComponent<AudioSource>().isPlaying) { Destroy(gameObject); }
		} else { Destroy(gameObject); }
	}
}
