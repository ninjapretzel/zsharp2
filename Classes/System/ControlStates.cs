using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class ControlStates {

	private static Dictionary<string, string> values = new Dictionary<string,string>();

	public static void Set(string thing, string val) { values[thing] = val; }
	public static void Unset(string thing) { if (values.ContainsKey(thing)) { values.Remove(thing); } }
	public static T Get<T>(string thing) {
		if (values.ContainsKey(thing)) {
			object ret = default(T);

			string data = values[thing];
			
			if (typeof(T) == typeof(string)) { ret = data; }

			if (typeof(T) == typeof(int)
				|| typeof(T) == typeof(float)
				|| typeof(T) == typeof(double)
				|| typeof(T) == typeof(short)
				|| typeof(T) == typeof(byte)
				|| typeof(T) == typeof(long)) {
				if (data.ToLower() == "true") { ret = 1; }
				if (data.ToLower() == "false") { ret = 0; }

				double val;
				if (double.TryParse(data, out val)) {
					ret = (T)(object)val;
				}

			}

			if (typeof(T) == typeof(bool)) {
				if (data.ToLower() == "true") { ret = true; }
				if (data.ToLower() == "false") { ret = false; }
				if (data == "1") { ret = true; }
				if (data == "0") { ret = false; }
			}

			return (T)ret;
		}

		if (typeof(T) == typeof(string)) { return (T)(object)""; }

		return default(T);
	}
	
	public static float forwardAxis = 0.0f;
	public static float lateralAxis = 0.0f;
	public static float yawAxis = 0.0f;
	public static float pitchAxis = 0.0f;
	
	public static bool forward = false;
	public static bool back = false;
	public static bool moveleft = false;
	public static bool moveright = false;
	public static bool moveup = false;
	public static bool movedown = false;
	
	public static bool left = false;
	public static bool right = false;
	public static bool lookup = false;
	public static bool lookdown = false;

	public static bool attack = false;
	public static bool jump = false;
	public static bool crouch = false;
	public static bool sprint = false;
	public static bool voicerecord = false;
	
	public static bool altInput = false;
	
	public static bool sightHold = false;
	public static bool sightToggle = false;
	
	public static bool reload = false;
	public static bool showScores = false;
	public static bool showChatWindow = false;
	public static bool use = false;
	
	public static string rcon_password = "dickbutts";
	public static string rcon_commands = "";

	public static string changeToMap = "";

	public static bool selectWeapon1 = false;
	public static bool selectWeapon2 = false;
	public static bool selectWeapon3 = false;
	public static bool selectWeapon4 = false;
	
}
