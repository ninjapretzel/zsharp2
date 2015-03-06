using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Sound {
	public string name;
	public AudioSource overrideSource;
	public List<AudioClip> clips;
	
	public Sound() { clips = new List<AudioClip>(); }
	
	public Sound(string n, AudioClip c) {
		name = n;
		clips = new List<AudioClip>();
		clips.Add(c);
	}
	
	public AudioClip GetSound() {
		if (clips.Count == 1) { return clips[0]; }
		else if (clips.Count == 0) { return null; }
		return clips.Choose();
	}
	
	public void AddClips(List<AudioClip> list) {
		foreach (AudioClip clip in list) { clips.Add(clip); }
	}
	
}


public class Sounds : MonoBehaviour {
	public AudioSource audioSet;
	public List<Sound> soundSet;
	
	public static Dictionary<string, Sound> sounds = new Dictionary<string, Sound>();
	
	private static AudioSource audioSetStatic;
	public static AudioSource audioSettings {
		get { return audioSetStatic != null ? audioSetStatic : audioSettingsFactory; }
		set { audioSetStatic = value; }
	}
	
	public static bool started = false;
	
	public static AudioSource audioSettingsFactory {
		get {
			GameObject o = new GameObject("Audio");
			o.AddComponent<AudioSource>();
			o.AddComponent<SoundVolume>();
			o.AddComponent<AutodestructSound>();
			return o.GetComponent<AudioSource>();
		}
	}
	
	
	void Awake() {
		if (started) { return; }
		audioSettings = audioSet;
		foreach (Sound s in soundSet) { Add(s); }
		
		started = true;
	}
	
	public static AudioSource Play(AudioClip sc, AudioSource settings) {
		if (audioSettings == null) { return null; }
		if (sc == null) { return null; }
		Vector3 pos = Vector3.zero;
		if (Camera.main) { pos = Camera.main.transform.position; }
		return Play(sc, settings, pos);
	}
	
	public static AudioSource Play(AudioClip sc, AudioSource settings, Transform t) { return Play(sc, settings, t.position); } 
	public static AudioSource Play(AudioClip sc, AudioSource settings, Vector3 pos) {
		if (audioSettings == null && settings == null) { return null; }
		if (sc == null) { return null; }
		if (settings == null) { settings = audioSettings; }
		
		AudioSource source = Instantiate(settings, pos, Quaternion.identity) as AudioSource;
		source.clip = sc;
		source.volume = Settings.soundVolume;
		source.Play();
		return source;
	}
	
	public static AudioSource Play(string sc) { return Play(GetSound(sc), GetSettings(sc)); }
	public static AudioSource Play(string sc, Transform t) { return Play(GetSound(sc), GetSettings(sc), t.position); }
	public static AudioSource Play(string sc, Vector3 pos) { return Play(GetSound(sc), GetSettings(sc), pos); }
	
	public static AudioSource Play(string sc, string settings) { return Play(GetSound(sc), GetSettings(settings)); }
	public static AudioSource Play(string sc, string settings, Transform t) { return Play(GetSound(sc), GetSettings(settings), t.position); }
	public static AudioSource Play(string sc, string settings, Vector3 pos) { return Play(GetSound(sc), GetSettings(settings), pos); }
	
	public static AudioClip Get(string sc) { return GetSound(sc); }
	public static AudioClip GetSound(string sc) {
		if (audioSettings == null) { return null; }
		if (!sounds.ContainsKey(sc)) {
			AudioClip loaded = Resources.Load<AudioClip>(sc);
			return loaded;
		}
		return sounds[sc].GetSound();
	}
	
	public static AudioSource GetSettings(string sc) { 
		if (!sounds.ContainsKey(sc)) { return audioSettings; }
		Sound sound = sounds[sc];
		if (sound.overrideSource != null) {
			return sound.overrideSource;
		}
		return audioSettings;
	}
	
	public static bool Has(string sc) { return sounds.ContainsKey(sc); }
	
	public static bool Load(string sc) { 
		AudioClip ac = Resources.Load(sc, typeof(AudioClip)) as AudioClip;
		return (ac != null);
	}
	
	public static bool Add(string sc) {
		if (Has(sc)) { return true; }
		AudioClip ac = Resources.Load(sc, typeof(AudioClip)) as AudioClip;
		if (ac != null) { return Add(new Sound(sc, ac)); }
		return false;
		
	}
	
	public static bool Add(Sound sc) {
		if (sounds.ContainsKey(sc.name)) { sounds[sc.name].AddClips(sc.clips); }
		else { sounds.Add(sc.name, sc); }
		return true;
	}
	
}
