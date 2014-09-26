using UnityEngine;
using System.Collections;
using System.Collections.Generic;

///GameSys class provides properties that wrap common platform case testing
public static class GameSys {
	public static bool isMobile { get { return isAndroid || isIOS; } }
	public static bool isPC { get { return !isMobile; } }
	
	public static bool isAndroid { get { return Application.platform == RuntimePlatform.Android; } }
	public static bool isIOS { get { return Application.platform == RuntimePlatform.IPhonePlayer; } }
	
	///TBD: Add more device properties as needed
	
}
