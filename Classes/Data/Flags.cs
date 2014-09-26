using UnityEngine;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

public class Flags : Dictionary<string, bool> {
	
	public Flags() : base() { }
	public Flags(string str) : base() { LoadString(str); }	
	
	
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
	
	public void LoadString(string str) {
		string[] keys = str.Split(',');
		foreach (string key in keys) { 
			if (key == "") { continue; }
			this[key] = true; 
		}
	}
	
	public void Save(string key) {
		PlayerPrefs.SetString(key, ToString());
	}
	
	public void Load(string key) {
		LoadString(PlayerPrefs.GetString(key));
	}
	
}