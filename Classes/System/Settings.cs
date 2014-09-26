using UnityEngine;
using System.Collections;

///Enumeration of quality settings
public enum QualitySetting : int { Low = 0, Medium = 1, High = 2 }

///This class holds settings information for things.
///Custom information can be held in a custom table.
public static partial class Settings {
	public static float overscanRatio = 0;
	public static float musicVolume = 1;
	public static float soundVolume = 1;
	
	//Custom settings can be stored in this table.
	//This will be saved and loaded automatically.
	public static Table custom = new Table();
	
	public static bool hints = true;
	
	public static void Save() {
		Prefs.SetFloat("set_overscan", overscanRatio);
		Prefs.SetFloat("set_musicVolume", musicVolume);
		Prefs.SetFloat("set_soundVolume", soundVolume);
		
		custom.Save("set_custom");
		
		Prefs.SetBool("set_hints", hints);
	}
	
	public static void Load() {
		if (!Prefs.HasKey("set_musicVolume")) {
			musicVolume = .5f;
			soundVolume = .5f;
			overscanRatio = 0;
			return;
		}
		overscanRatio = Prefs.GetFloat("set_overscan");
		musicVolume = Prefs.GetFloat("set_musicVolume");
		soundVolume = Prefs.GetFloat("set_soundVolume");
		
		custom.Load("set_custom");
		
		hints = Prefs.GetBool("set_hints");
		
	}
	
	
}


public static class SettingsUtils {
	
}
