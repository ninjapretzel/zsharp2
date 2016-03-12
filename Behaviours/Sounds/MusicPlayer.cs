using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Music {
	public static Dictionary<string, List<AudioClip> > songs;
	
	public static void Add(string name, AudioClip clip) {
		if (songs == null) { songs = new Dictionary<string, List<AudioClip> >(); }
		if (songs.ContainsKey(name)) { 
			songs[name].Add(clip);
		} else {
			songs.Add(name, new List<AudioClip>() );
			songs[name].Add(clip);
		}
	}
	
	public static List<AudioClip> Get(string name) { return songs[name]; }
	public static AudioClip[] GetArray(string name) { return Get(name).ToArray(); }
}

[System.Serializable]
public class SongInfo {
	public string name;
	public AudioClip[] songs;
}


public class MusicPlayer : MonoBehaviour {
	public AudioClip[] songs;
	
	public SongInfo[] trackSets;

	public string activeSet;
	public bool playOnStart = false;
	
	int curIndex;
	public bool shuffle = true;
	public static MusicPlayer main;
	
	bool fade = false;
	bool fadein = false;
	string fadeTo = "";
	float time = 0;
	public float fadeTime = .5f;
	public float volume = .6f;
	

	public bool followCamera = true;
	
	void Awake() {
		if (main != null) { 
			Destroy(gameObject);
			return;
		}
		main = this;
		DontDestroyOnLoad(gameObject);
		
		foreach (SongInfo s in trackSets) {
			foreach (AudioClip clip in s.songs) {
				Music.Add(s.name, clip);
			}
		}
		fadein = true;
		
		if (playOnStart) {
			string set = activeSet;
			activeSet = "";
			Switch(set);

		} else {
			activeSet = "";
		}

	}
	
	void Update() {
		UpdateFade();
		if (fade) { return; }
		
		
		
		if (!GetComponent<AudioSource>().isPlaying) { 
			if (shuffle) {
				PlayRandom(true);
			} else {
				PlayNext();
			}
		}
		
		if (followCamera && Camera.main) { transform.position = Camera.main.transform.position; }
	}
	
	public void UpdateFade() {
		time = Mathf.Clamp(time, 0, fadeTime);
		if (fade) {
			if (fadein) {
				time += Time.deltaTime;
				if (time > fadeTime) { fade = false; }
				
			} else {
				time -= Time.deltaTime;
				
				if (time <= 0) {
					if (shuffle) { SwitchShuffle(fadeTo); }
					else { Switch(fadeTo); }
					fadein = true;
				}
			}
		} else {
			if (fadein) {
				time += Time.deltaTime;
			} else {
				time -= Time.deltaTime;
			}
		}
		
		GetComponent<AudioSource>().volume = time / fadeTime * Settings.instance.musicVolume * volume;
	}
	
	public static void FadeIn() {
		main.fadein = true;
	}
	
	public static void FadeOut() {
		main.fadein = false;
	}
	
	public static void Fade(string name) { main.FadeTo(name); }
	
	public void FadeTo(string name) {
		if (activeSet == name) { return; }
		fade = true;
		fadeTo = name;
		fadein = false;
	}
	
	public static void SwitchTracks(string name) { main.Switch(name); }
	public static void SwitchTracks(string name, int track) { main.Switch(name, track); }
	public static void SwitchTracksShuffle(string name) { main.SwitchShuffle(name); }
	
	public void Switch(string name) {
		if (activeSet == name) { return; }
		songs = Music.GetArray(name);
		activeSet = name;
		if (shuffle) {
			Play(Shuffle());
		} else {
			Play(0);
		}
	}
	
	public void Switch(string name, int track) {
		if (activeSet == name) { return; }
		Switch(name);
		Play(track);
	}
	
	public void SwitchShuffle(string name) {
		if (activeSet == name) { return; }
		Switch(name);
		shuffle = true;
		PlayRandom(false);
	}
	
	public void Play(int index) {
		curIndex = index;
		GetComponent<AudioSource>().clip = songs[index];
		GetComponent<AudioSource>().Play();
	}
	
	public void PlayRandom(bool avoidSame) {
		int index = Shuffle();
		if (avoidSame) {
			if (songs.Length > 1) {
				while (index == curIndex) {
					index = Shuffle();
				}
			}
		} else {
			index = Shuffle();
		}
		
		Play(index);
	}
	
	void PlayNext() {
		int i = curIndex + 1;
		if (i >= songs.Length) { i = 0; }
		Play(i);
	}	
	
	int Shuffle() {
		return (int)(Random.value * songs.Length);
	}
	
}




















