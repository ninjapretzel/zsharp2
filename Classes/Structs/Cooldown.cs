using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;


public class Cooldown {
	
	#region Variables 
	
	public DateTime lastUsed;
	public DateTime cooledAt;
	public TimeSpan duration;
	
	public bool roundToDay = false;
	public bool roundToHour = false;
	public bool roundToMinute = false;
	
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
	
	public bool usable { get { return DateTime.Now.CompareTo(cooledAt) > 0; } }
	public bool Use() {
		if (!usable) { return false; }
		
		lastUsed = DateTime.Now;
		cooledAt = lastUsed.Add(duration);
		
		return true;
	}
	
	#region DataManagement
	
	public void Save() { Load("cooldown"); }
	public void Save(string key) {
		PlayerPrefs.SetString(key + "_" + name, lastUsed.DateToString());
		
	}
	
	public void Load() { Load("cooldown"); }
	public void Load(string key) {
		lastUsed = PlayerPrefs.GetString(key + "_" + name).ParseDate();
	}
	
	#endregion
	
}



























