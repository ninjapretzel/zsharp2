using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;

public enum Language {
	english,
	spanish,
	french,
	italian,
	german,
	russian,
	japanese,
}

public static class Localization {

	private static JsonObject _strs;

	public static Language language = Language.english;
	public static bool initialized = false;

	public static void Init() {
		string text = Resources.Load<TextAsset>("strings").text.ConvertNewlines();
		//JsonArray levelsArray = JsonArray.ParseCSV(text, '|');
		_strs = JsonObject.ParseCSV(text, '|');
		//Debug.Log(_strs.PrettyPrint());
		// Use a switch here instead of parsing the enum (we don't support all those langauges yet!)
		if (SteamManager.Initialized) {
			switch (SteamApps.GetCurrentGameLanguage()) {
				case "spanish": {
					language = Language.spanish;
					break;
				}
				case "english":
				default: {
					language = Language.english;
					break;
				}
			}
		}
		initialized = true;
	}

	public static string Localize(string name, params object[] args) {
		if (name.Length == 0) { return name; }
		if (_strs.ContainsKey(name)) {

			JsonObject localized = _strs.Get<JsonObject>(name);
			string st = localized.Get<string>(language.ToString());

			if (!string.IsNullOrEmpty(st)) {
#if UNITY_EDITOR
				return "<color=#00ff00ff>" + string.Format(st.Replace("\\n", "\n"), args) + "</color>";
#else
				return string.Format(st.Replace("\\n", "\n") , args);
#endif
			}
		}
#if UNITY_EDITOR
		return "<color=#ff00ffff>" + name + "</color>";
#else
		return name;
#endif
	}

}