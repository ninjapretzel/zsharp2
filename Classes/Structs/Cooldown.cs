using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

///<summary> Represents a real-time timer, for something that can be used once per time period. </summary>
public class Cooldown {
	
	#region Variables 
	///<summary> Date/Time of last use. </summary>
	public DateTime lastUsed;
	///<summary> Date/Time when ready again. </summary>
	public DateTime cooledAt;
	///<summary> Length of the cooldown. </summary>
	public TimeSpan duration;

	///<summary> Round times to days? </summary>
	public bool roundToDay = false;
	///<summary> Round times to hours? </summary>
	public bool roundToHour = false;
	///<summary> Round times to minutes? </summary>
	public bool roundToMinute = false;

	///<summary> Name of the cooldown </summary>
	public string name = "";
	
	#endregion
	
	
	#region Initialization
	public Cooldown(TimeSpan dur, bool setUsable = true, bool rDay = false, bool rHour = false, bool rMinute = false) { 
		roundToDay = rDay;
		roundToHour = rHour;
		roundToMinute = rMinute;
		
		Set(dur, setUsable);
	}
	
	public Cooldown(string n, TimeSpan dur, bool setUsable = true, bool rDay = false, bool rHour = false, bool rMinute = false) {
		name = n;
		roundToDay = rDay;
		roundToHour = rHour;
		roundToMinute = rMinute;
		
		Set(dur, setUsable);
	}

	///<summary> Set the duration on this cooldown, and if it is currently usable, or must completly run out to be used. </summary>
	public void Set(TimeSpan dur, bool setUsable = true) {
		duration = dur;
		
		DateTime checkTime = DateTime.Now;
		if (roundToDay) { checkTime = checkTime.RoundToDay(); }
		else if (roundToHour) { checkTime = checkTime.RoundToHour(); }
		else if (roundToMinute) { checkTime = checkTime.RoundToMinute(); }
		
		
		
		if (setUsable) { cooledAt = checkTime; }
		else { cooledAt = DateTime.Now.Add(duration); }
		
		
		lastUsed = cooledAt.Subtract(duration);
		
	}
	
	#endregion
	///<summary> is this cooldown currently usable? </summary>
	public bool usable { get { return DateTime.Now.CompareTo(cooledAt) > 0; } }
	///<summary> Use this cooldown (if useable). Returns true if used, and begun cooling down. Returns false if not used, as it was already cooling down. </summary>
	public bool Use() {
		if (!usable) { return false; }
		
		lastUsed = DateTime.Now;
		cooledAt = lastUsed.Add(duration);
		
		return true;
	}
	
	#region DataManagement
	///<summary> Save to PlayerPrefs</summary>
	public void Save(string key = "cooldown") {
		PlayerPrefs.SetString(key + "_" + name, lastUsed.DateToString());
		
	}
	///<summary> Load from PlayerPrefs</summary>
	public void Load(string key = "cooldown") {
		lastUsed = PlayerPrefs.GetString(key + "_" + name).ParseDate();
	}
	
	#endregion
	
}



























