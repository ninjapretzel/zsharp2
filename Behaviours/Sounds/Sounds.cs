using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Sound {
	public string name = "un-initialized Sound";
	public AudioSource overrideSource;
	public string channel;
	public List<AudioClip> clips;
	
	public int Count { get { return clips.Count; } }
	public AudioClip nextSound { get { return GetSound(); } }
	
	public Sound() { clips = new List<AudioClip>(); }
	public Sound(string n) : this() { name = n; }
	public Sound(string n, AudioClip c) : this(n) { clips.Add(c); }
	public Sound(string n, Sound parent) : this(n) {
		channel = parent.channel;
		overrideSource = parent.overrideSource;
	}
	
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
	
	public AudioMixer audioMixer;
	static Dictionary<string, Sound> sounds = new Dictionary<string, Sound>();

	static AudioMixer mixer;
	
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
			//o.AddComponent<PlaySoundOnStart>();
			o.AddComponent<AutodestructSound>();
			return o.GetComponent<AudioSource>();
		}
	}
	
	
	void Awake() {
		if (started) { return; }
		audioSettings = audioSet;
		mixer = audioMixer;
		
		foreach (Sound s in soundSet) { Add(s); }
		
		started = true;
	}

	/// <summary> Plays the given sound by name. Used via UGUI </summary>
	/// <param name="sound">Sound to look up and play. </param>
	public void Play_(string sound) { Play(sound); }

	/// <summary> Play a audio clip, in a new AudioSource, with a given AudioSource's settings. Positions sound on the camera. </summary>
	/// <param name="sc">Source Clip to play</param>
	/// <param name="settings">AudioSource to copy settings from </param>
	/// <returns>AudioSource created with copy of settings, playing sc</returns>
	public static AudioSource Play(AudioClip sc, AudioSource settings) {
		if (audioSettings == null) { return null; }
		if (sc == null) { return null; }
		if (Camera.main != null) { return Play(sc, settings, Camera.main.transform); }
		return Play(sc, settings, Vector3.zero);
	}

	/// <summary> Play a audio clip, in a new AudioSource, with a given AudioSource's settings. Positions sound at a given transform's position and parents it. </summary>
	/// <param name="sc">Source Clip to play</param>
	/// <param name="settings">AudioSource to copy settings from </param>
	/// <param name="t">Transform to play at, and parent to </param>
	/// <returns>AudioSource created with copy of settings, playing sc</returns>
	public static AudioSource Play(AudioClip sc, AudioSource settings, Transform t) {
		AudioSource snd = Play(sc, settings, t.position); 
		if (snd != null) { snd.transform.parent = t; }
		return snd;
	}

	
	/// <summary> Play a audio clip, in a new AudioSource, with a given AudioSource's settings. Positions sound at the given position. </summary>
	/// <param name="sc">Source Clip to play</param>
	/// <param name="settings">AudioSource to copy settings from </param>
	/// <param name="pos">Position to play the sound at</param>
	/// <returns>AudioSource created with copy of settings, playing sc</returns>
	public static AudioSource Play(AudioClip sc, AudioSource settings, Vector3 pos) {
		if (audioSettings == null && settings == null) { return null; }
		if (sc == null) { return null; }
		if (settings == null) { settings = audioSettings; }
		
		AudioSource source = Instantiate(settings, pos, Quaternion.identity) as AudioSource;
		source.clip = sc;
		source.volume = Settings.instance.soundVolume;
		source.Play();
		
		
		return source;
	}


	/// <summary> Load and play a sound by name </summary>
	/// <param name="sc"> Name of sound to play </param>
	/// <returns>AudioSource playing the given sound sc at the camera</returns>
	public static AudioSource Play(string sc) { return Play(GetSound(sc), GetSettings(sc)); }

	/// <summary> Load and play a sound by name, at a position</summary>
	/// <param name="sc">Name of sound to play</param>
	/// <param name="pos">Position to play sound at</param>
	/// <returns>AudioSource playing the given sound sc at the given position pos</returns>
	public static AudioSource Play(string sc, Vector3 pos) { return Play(GetSound(sc), GetSettings(sc), pos); }

	/// <summary> Load and play a sound by name, following a given object </summary>
	/// <param name="sc">Name of sound to play</param>
	/// <param name="t">Transform of object to follow</param>
	/// <returns>AudioSource playing the given sound, on an object attached to the given Transform</returns>
	public static AudioSource Play(string sc, Transform t) { return Play(GetSound(sc), GetSettings(sc), t); }

	/// <summary> Load and play a sound by name, using settings by name </summary>
	/// <param name="sc"> Name of sound to play </param>
	/// <param name="settings">Name of sound settings to load</param>
	/// <returns>AudioSource playing the given sound sc, with the given settings, playing at the camera</returns>
	public static AudioSource Play(string sc, string settings) { return Play(GetSound(sc), GetSettings(settings)); }
	
	/// <summary> Load and play a sound by name, using settings by name </summary>
	/// <param name="sc"> Name of sound to play </param>
	/// <param name="settings">Name of sound settings to load</param>
	/// <param name="pos">Position to play sound at</param>
	/// <returns>AudioSource playing the given sound sc, with the given settings, at the given position</returns>
	public static AudioSource Play(string sc, string settings, Vector3 pos) { return Play(GetSound(sc), GetSettings(settings), pos); }

	/// <summary> Load and play a sound by name, using settings by name </summary>
	/// <param name="sc"> Name of sound to play </param>
	/// <param name="settings">Name of sound settings to load</param>
	/// <param name="t">Transform of object to follow</param>
	/// <returns>AudioSource playing the given sound sc, with the given settings, attached to the given Transform</returns>
	public static AudioSource Play(string sc, string settings, Transform t) { return Play(GetSound(sc), GetSettings(settings), t); }
	
	public static AudioClip Get(string sc) { return GetSound(sc); }
	public static AudioClip GetSound(string sc) {
		if (!sounds.ContainsKey(sc)) { sounds[sc] = Load(sc); }
		
		if (sounds[sc] != null) { return sounds[sc].GetSound(); }
		return null;
	}
	
	public static AudioSource GetSettings(string sc) { 
		if (!sounds.ContainsKey(sc) || sounds[sc] == null) { return audioSettings; }
		Sound sound = sounds[sc];	
		
		AudioSource settings = audioSettings;
		if (sound.overrideSource != null) {
			settings = sound.overrideSource;
		}
		var targetGroup = mixer.Get(sound.channel);
		if (targetGroup != null) {
			settings.outputAudioMixerGroup = targetGroup;
		}

		return settings;
	}
	
	public static bool Has(string sc) { 
		return sounds.ContainsKey(sc) ? sounds[sc].Count > 0 : false; 
	}
	
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
		
		if (clips.Count > 0) { 
			sound.AddClips(clips);
			return sound;
		}
		return null;
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

public static class AudioMixerHelpers {
	public static AudioMixerGroup Get(this AudioMixer mixer, string name) {
		if (name == null || name == "") { return null; }
		var groups = mixer.FindMatchingGroups(name);
		foreach (var g in groups) {
			if (g.name == name) { return g; }
		}
		return null;
	}
}
