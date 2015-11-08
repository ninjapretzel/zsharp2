using UnityEngine;
using System.Collections;

/// <summary>
/// This class holds settings information for things.
/// </summary>
public partial class Settings {

	public static Settings instance = new Settings();

	public int qualityLevel = 3;
	
	public float musicVolume = 1;
	public float soundVolume = 1;
	public float overscanRatio = 0;

	public string userName = "New User";
	public Color color = Color.red;
	public float sensitivity = 5;
	

}
