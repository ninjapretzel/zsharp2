using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Kongregate : MonoBehaviour {
	
	public static GameObject watcher;
	public static bool loaded = false;
	public static bool isGuest = false;
	public static int userID = 0;
	public static string username = "Guest";
	public static string gameAuthToken = "";
	
	
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
	
	//Callback linked in the AttemptConnection() function
	public void OnKongregateAPILoaded(string info) {
		loaded = true;
		
		string[] content = info.Split('|');
		userID = content[0].ParseInt();
		username = content[1];
		gameAuthToken = content[2];
		
		isGuest = userID == 0;
		
	}
	
	
	public static void SendScore(string score, int v) {
		Application.ExternalCall("kongregate.stats.submit",score,v);
	}
	
	
}
	
