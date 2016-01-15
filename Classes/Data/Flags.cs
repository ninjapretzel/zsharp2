using UnityEngine;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

/// <summary> Extension of Dictionary(string, bool) with custom behaviour. Treats nonexistant entries as 'false' </summary>
public class Flags : Dictionary<string, bool> {

	/// <summary> Default constructor </summary>
	public Flags() : base() { }
	/// <summary> Constructor that loads a list of flags to set as true. Always uses ',' as the delimeter </summary>
	public Flags(string str) : base() { LoadString(str); }

	/// <summary> Custom indexer, treats nonexistant entries as false. </summary>
	public new bool this[string key] { 
		get {
			if (ContainsKey(key)) { 
				Dictionary<string, bool> bs = this as Dictionary<string, bool>;
				return bs[key];
			}
			return false;
		}
		
		set { 
			Dictionary<string, bool> bs = this as Dictionary<string, bool>;
			bs[key] = value;
		}
	}

	/// <summary> Returns a string representation of the flags set to 'true' as a list of comma separated strings </summary>
	public override string ToString() {
		StringBuilder str = new StringBuilder();
		foreach (var pair in this) {
			if (pair.Value == true) {
				str.Append(pair.Key);
				str.Append(",");
			}
		}
		return str.ToString();
	}

	/// <summary> Loads a comma separated list of strings into this flags. </summary>
	public void LoadString(string str) {
		string[] keys = str.Split(',');
		foreach (string key in keys) { 
			if (key == "") { continue; }
			this[key] = true; 
		}
	}

	/// <summary> Save to PlayerPrefs at 'key' </summary>
	public void Save(string key) {
		PlayerPrefs.SetString(key, ToString());
	}
	/// <summary> Load from PlayerPrefs at 'key' </summary>
	public void Load(string key) {
		LoadString(PlayerPrefs.GetString(key));
	}
	
}
