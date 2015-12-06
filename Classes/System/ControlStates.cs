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
					if (typeof(T) == typeof(int)) { ret = (T)(object)(int)val; }
					if (typeof(T) == typeof(float)) { ret = (T)(object)(float)val; }
					if (typeof(T) == typeof(double)) { ret = (T)(object)val; }
					if (typeof(T) == typeof(short)) { ret = (T)(object)(short)val; }
					if (typeof(T) == typeof(byte)) { ret = (T)(object)(byte)val; }
					if (typeof(T) == typeof(long)) { ret = (T)(object)(long)val; }
					
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
	

	public static float forwardAxis { get { return Get<float>("forwardAxis"); } }
	public static float lateralAxis { get { return Get<float>("lateralAxis"); } }
	public static float yawAxis { get { return Get<float>("yawAxis"); } }
	public static float pitchAxis { get { return Get<float>("pitchAxis"); } }
	public static float zoomAxis { get { return Get<float>("zoomAxis"); } }

	public static bool forward { get { return Get<bool>("forward"); } }
	public static bool backward { get { return Get<bool>("backward"); } }
	public static bool left { get { return Get<bool>("left"); } }
	public static bool right { get { return Get<bool>("right"); } }
	public static bool up { get { return Get<bool>("up"); } }
	public static bool down { get { return Get<bool>("down"); } }

	public static bool zoomIn { get { return Get<bool>("zoomIn"); } }
	public static bool zoomOut { get { return Get<bool>("zoomOut"); } }

	public static bool camUp { get { return Get<bool>("camUp"); } }
	public static bool camDown { get { return Get<bool>("camDown"); } }
	public static bool camLeft { get { return Get<bool>("camLeft"); } }
	public static bool camRight { get { return Get<bool>("camRight"); } }

	public static bool jump { get { return Get<bool>("jump"); } }
	public static bool powerup { get { return Get<bool>("powerup"); } }
	
}
