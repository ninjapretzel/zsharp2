using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary> GameSys class provides properties that wrap common platform case testing </summary>
public static class GameSys {

	/// <summary> Are we running on some mobile platform? </summary>
	public static bool isMobile { get { return isAndroid || isIOS; } }
	/// <summary> Are we running on a standalone or web? </summary>
	public static bool isPC { get { return !isMobile; } }

	/// <summary> Are we running on android? </summary>
	public static bool isAndroid { get { return Application.platform == RuntimePlatform.Android; } }
	/// <summary> Are we running on iOS? </summary>
	public static bool isIOS { get { return Application.platform == RuntimePlatform.IPhonePlayer; } }
	
	//TBD: Add more device properties as needed
	
}
