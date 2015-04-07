using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Sound {
	public string name = "un-initialized Sound";
	public AudioSource overrideSource;
	private List<AudioClip> clips;
	
	public int Count { get { return clips.Count; } }
	public AudioClip nextSound { get { return GetSound(); } }
	
	public Sound() { clips = new List<AudioClip>(); }
	public Sound(string n) : this() { name = n; }
	public Sound(string n, AudioClip c) : this(n) { clips.Add(c); }
	
	public AudioClip GetSound() {
		if (clips.Count == 1) { return clips[0]; }
		if (clips.Count == 0) { return null; }
		return clips.Choose();
	}
	
	public void AddClips(List<AudioClip> list) { foreach (AudioClip clip in list) { clips.Add(clip); } }
	public void AddClips(Sound snd) { foreach (AudioClip clip in snd.clips) { clips.Add(clip); } }
	
}


public class Sounds : MonoBehaviour {
	public AudioSource audioSet;
	public List<Sound> soundSet;
	
	static Dictionary<string, Sound> sounds = new Dictionary<string, Sound>();
	
	private static AudioSource _audioSettings;
	public static AudioSource audioSettings {
		get { 
			if (_audioSettings == null) { _audioSettings = audioSettingsFactory; }
			return _audioSettings;
		}
		set { _audioSettings = value; }
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
			Debug.Log("first time playing " + sc);
			sounds[sc] = Load(sc);
			Debug.Log(sounds[sc].Count + " sounds loaded");
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
	
	// public static bool Load(string sc) { 
		// AudioClip ac = Resources.Load<AudioClip>(sc);
		// return (ac != null);
	// }
	
	public static Sound Load(string sc) {
		List<AudioClip> clips = new List<AudioClip>();
		AudioSource overrideSource = Resources.Load<AudioSource>(sc);
		
		Sound sound = new Sound(sc);
		sound.overrideSource = overrideSource;
		
		AudioClip single = Resources.Load<AudioClip>(sc);
		if (single != null) { 
			clips.Add(single);
		} else {
			AudioClip clip0 = Resources.Load<AudioClip>(sc+"0");
			if (clip0 != null) { clips.Add(clip0); }
			AudioClip clip1 = Resources.Load<AudioClip>(sc+"1");
			if (clip1 != null) { clips.Add(clip1); }
			
			
			if (clips.Count > 0) {
				int i = 2;
				AudioClip clip = Resources.Load<AudioClip>(sc+i);
				while (clip != null) {
					clips.Add(clip);
					i++;
					clip = Resources.Load<AudioClip>(sc+i);
				}
			}
			
			
		}
		
		sound.AddClips(clips);
		
		return sound;
	}
	
	public static bool Add(string sc) {
		if (Has(sc)) { return true; }
		AudioClip ac = Resources.Load<AudioClip>(sc);
		if (ac != null) { return Add(new Sound(sc, ac)); }
		return false;
	}
	
	public static bool Add(Sound sc) {
		if (sounds.ContainsKey(sc.name)) { sounds[sc.name].AddClips(sc); }
		else { sounds.Add(sc.name, sc); }
		return true;
	}
	
}
