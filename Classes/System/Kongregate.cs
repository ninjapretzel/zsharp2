using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary> Behaviour to hook into Kongregate's environment </summary>
public class Kongregate : MonoBehaviour {

	/// <summary> Watcher object (if created) </summary>
	public static GameObject watcher;
	/// <summary> Has Kongregate loaded? </summary>
	public static bool loaded = false;
	/// <summary> Is the user signed in as guest? </summary>
	public static bool isGuest = false;
	/// <summary> User's ID if signed in </summary>
	public static int userID = 0;
	/// <summary> User's name if signed in </summary>
	public static string username = "Guest";
	/// <summary> This gets set if successfully connected to Kongregate's API. </summary>
	public static string gameAuthToken = "";

	/// <summary> Attempt to connect to Kongregate's services.</summary>
	public static void AttemptConnection() {
		watcher = new GameObject("KongregateWatcher");
		DontDestroyOnLoad(watcher);
		watcher.AddComponent<Kongregate>();
		
		Application.ExternalEval(
			"if(typeof(kongregateUnitySupport) != 'undefined'){" +
			"	kongregateUnitySupport.initAPI('KongregateWatcher', 'OnKongregateAPILoaded');" +
			"};"
		);
		
	}

	/// <summary> Callback linked in the AttemptConnection() function. Pulls in all information from the success. </summary>
	public void OnKongregateAPILoaded(string info) {
		loaded = true;
		
		string[] content = info.Split('|');
		userID = content[0].ParseInt();
		username = content[1];
		gameAuthToken = content[2];
		
		isGuest = userID == 0;
		
	}

	/// <summary> Send a Score to Kongregate. </summary>
	public static void SendScore(string score, int v) {
		Application.ExternalCall("kongregate.stats.submit",score,v);
	}
	
	
}
	
