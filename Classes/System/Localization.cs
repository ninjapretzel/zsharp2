using System;
using System.Reflection;
using System.Collections.Generic;
using UnityEngine;

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

	public static void Init() {
		string text = Resources.Load<TextAsset>("strings").text.ConvertNewlines();
		//JsonArray levelsArray = JsonArray.ParseCSV(text, '|');
		_strs = JsonObject.ParseCSV(text, '|');
		Debug.Log(_strs.PrettyPrint());

	}

	public static string Localize(string name, params object[] args) {
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
		return "<color=#ff00ffff>" + name + "</color>";
	}

}
