using UnityEngine;
using System;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;


/// <summary> Delagate type for callback for checking an achievement. Using this instead of Func(string) makes doing multicast delegates easier. </summary>
public delegate Achievable AchievableAction(string args);

[System.Serializable]
public class Achievable {
	
	/// <summary> The ID of this achievable within some API </summary>
	public string id = "achievement_dummy";
	
	/// <summary> The display name of this achievement </summary>
	public string display = "Dummy Achievable";

	/// <summary> Has this achievable been unlocked? </summary>
	public bool unlocked = false;
	
	/// <summary> Is this achievable visible to the users? </summary>
	public bool visible = true;

	/// <summary> Callback for when the achievement is earned (or lost) </summary>
	public System.Action<bool> earnedCallback = DummyEarnedResponse;
	
	/// <summary> Was this achievable JUST earned this past frame? </summary>
	public bool justEarned { 
		get { 
			if (unlocked) { return false; }
			unlocked = earned;
			return unlocked;
		}
	}
	
	/// <summary> Property to wrap Earned() function. </summary>
	public bool earned { get { return Earned(); } }
	
	/// <summary> Basic earned response that does nothing. </summary>
	public static void DummyEarnedResponse(bool success) {}

	/// <summary> When earned (or, when taken away) call the </summary>
	public void OnEarnedResponse(bool success) {
		if (!success) {
			unlocked = false;
		} else {
			unlocked = true;
		}
		earnedCallback(success);
	}	
	
	/// <summary> Helper function for inside Register() </summary>
	public void Register(string name, AchievableAction action) { Achievables.Register(name, action); }

	/// <summary> Save to PlayerPrefs </summary>
	public void SaveData() { 
		//Save();
		//PlayerPrefs.SetInt(id + "_unlocked", unlocked ? 0 : 1);
	}

	/// <summary> Load from PlayerPrefs </summary>
	public void LoadData() {
		//Load();
		//unlocked = (PlayerPrefs.GetInt(id + "_unlocked") == 1);
	}
	
	/// <summary> Override this function with the proper logic for checking if the achievable has been unlocked. </summary>
	public virtual bool Earned() { return false; }
	
	/// <summary> Override this function to register all events with the Achievables class </summary>
	public virtual void Register() { }
	
	/// <summary> Overload with logic to save the achievable's progress. </summary>
	public virtual void Save() { }
	/// <summary> Overload with logic to load the achievable's progress. </summary>
	public virtual void Load() { }
	
}

#region EXAMPLES

/// <summary> Example Achievement class. Simply awarded when the message "Trigger" is sent to the static Achievables class through the 'Event' function.</summary>
public class ExampleAchievableA : Achievable {
	
	/// <summary> Was this achievement triggered? </summary>
	bool wasTriggered = false;
	
	/// <summary> Override to 'Earned' with logic for this specific achievement type. </summary>
	public override bool Earned() { 
		return wasTriggered;
	}
	
	/// <summary> This is how the achievement is registered with the system, and all needed delegates are added. </summary>
	public override void Register() {
		display = "Trigger Finger";
		
		Register("Trigger", Triggered);	
	}

	/// <summary> Function that unlocks this achievement. </summary>
	public Achievable Triggered(string args) {
		Debug.Log("A: " + args);
		wasTriggered = true;
		return this;
	}	
	
}

/// <summary> Example Achievement class. Has a counter that tracks the achievement progress. </summary>
public class ExampleAchievableB : Achievable {
	
	/// <summary> Counter </summary>
	public int poopCount = 0;

	/// <summary> Override to 'Earned' with logic for this specific achievement. </summary>
	public override bool Earned() { 
		return poopCount >= 10;
	}
	
	/// <summary> This is how the achievement is registered with the system, and all needed delegates are added. </summary>
	public override void Register() {
		display = "Sir Poopy Pants";
		
		//Call Trigger on "Poop" or "Trigger" events 
		//Achievements can track from multiple events. 
		//Sometimes an event needs to trigger multiple achievements, and other events target specific ones.
		//
		//Ex. Maybe you have 'EliteKill' and 'TrashKill', both need to give progress towards 'TotalKill' achievements.
		//But, only 'EliteKill' affects 'Kill 1,000 Elite Mobs'
		//And there is another achievement to 'Kill 30,000 Trash Mobs', which is only affected by the specific 'TrashKill' event
		Register("Poop", Triggered);
		Register("Trigger", Triggered);
		
		//Call Reset on "CleanUpPoop" event 
		Register("CleanUpPoop", Reset);
		
	}

	/// <summary> Function that resets this achievement's progress. </summary>
	public Achievable Reset(string args) {
		poopCount = 0;
		return this;
	}
	
	/// <summary> Function that earns progress towards completing this achievement. </summary>
	public Achievable Triggered(string args) {
		poopCount += 1;
		Debug.Log("B POOPED: " + poopCount);
		return this;
	}	
	
}

/// <summary> Example Achievement class. Uses a constructor with a parameter, which is used in the logic for earning the achievement. </summary>
public class ExampleAchievableC : Achievable {
	
	/// <summary> Counter for tracking achievement progress </summary>
	public int poopCount = 0;
	/// <summary> Assigned in constructor. Used to see if the achievement is earned. </summary>
	public int poopTarget;

	/// <summary> Constructor. Takes a number that is used to check achievement progress against. </summary>
	public ExampleAchievableC(int target) {
		poopTarget = target;
		display = "poopcheck x" + poopTarget;
	}

	/// <summary> Check achievement progress against the number that was given to it in its constructor. </summary>
	public override bool Earned() { 
		return poopCount >= poopTarget;
	}

	/// <summary> Register it with the system... </summary>
	public override void Register() {
		Register("Trigger"+poopTarget, Triggered);
		
	}

	/// <summary> Trigger the achievement and add progress towards completion... </summary>
	public Achievable Triggered(string args) {
		poopCount += 1;
		Debug.Log("C" + poopTarget + " Triggered: " + poopCount);
		return this;
	}	
	
}

#endregion 

/// <summary> Static class for tracking achievements </summary>
public static class Achievables {

	/// <summary> Listing of all achievable pairs (id, achievable) </summary>
	public static Dictionary<string, Achievable> achievables = new Dictionary<string,Achievable>();
	/// <summary> Listing of all event pairs (name, callback) (</summary>
	public static Dictionary<string, AchievableAction> events = new Dictionary<string,AchievableAction>();
	/// <summary> Function to send achievement earned calls to some connected API (Steam, GooglePlay, iOS whatever...) </summary>
	public static System.Action<string, System.Action<bool>> sendFunc = DummySend;

	/// <summary> Dummy API callback that does nothing. </summary>
	public static void DummySend(string id, System.Action<bool> callback) {
		Debug.Log("Achievables.DummySend: No send callback registered. Sent Achievable <id:" + id + "> to achievable hell");
	}

	/// <summary> Register an achieveable with its API identifier. </summary>
	public static void Register(string id, Achievable achievable) {
		achievable.id = id;
		achievable.Register();
		achievables.Add(id, achievable);
	}

	/// <summary> Register an event name with an action. </summary>
	public static void Register(string name, AchievableAction action) {
		if (events.ContainsKey(name)) {
			events[name] += action;
		} else {
			events[name] = new AchievableAction(action);
		}
		
	}

	/// <summary> Call an event with the given 'name', with a given string as 'args' </summary>
	public static void Event(string name, string args = "") {
		if (events.ContainsKey(name)) {
			//int i = 0;
			
			Delegate[] eventActions = events[name].GetInvocationList();
			Debug.Log("Achievables.Event: Event " + name + " passed with args:\n[" + args + "]\nCalling a total of " + eventActions.Length + " actions.");
			
			foreach (AchievableAction action in eventActions) {
				Achievable achievable = action(args);
				
				if (achievable.justEarned) {
					sendFunc(achievable.id, achievable.OnEarnedResponse);
					Debug.Log("Achievables.Event: Achievable Earned " + achievable.display + "!");
				}
				
				
			}
		} else {
			Debug.Log("Achievables.Event: Event " + name + " passed with args:\n[" + args + "]\nBut " + name + " has not been registered.");
		}
	}
	
	/// <summary> Send all achievables that have been earned to whatever API is hooked up. </summary>
	public static void SendAllEarnedAchievables() {
		foreach (string id in achievables.Keys) {
			if (achievables[id].earned) { sendFunc(id, achievables[id].earnedCallback); }
		}
	}
	
	/// <summary> Save all achieveables to PlayerPrefs </summary>
	public static void Save() {
		foreach (Achievable achievable in achievables.Values) {
			achievable.SaveData();
		}
	}

	/// <summary> Load all achievables from PlayerPrefs </summary>
	public static void Load() {
		foreach (Achievable achievable in achievables.Values) {
			achievable.LoadData();
		}
	}
	
}
