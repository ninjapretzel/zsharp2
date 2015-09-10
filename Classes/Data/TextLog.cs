using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TextLog : List<string> {
	
	public int maxSize = 30;
	public List<string> asList { get { return (List<string>)this; } }

	public TextLog() : base() { }
	public TextLog(IEnumerable<string> coll) : base(coll) { }

	public new void Add(string item) {
		if (Count > maxSize) {
			RemoveAt(0);
		}
		asList.Add(item);
	}

	public TextLog AddDirty(string item) {
		TextLog txt = new TextLog(this);

		txt.Add(item);

		return txt;
	}

	

	public override string ToString() {
		string str = "";
		for (int i = 0; i < Count; i++) {
			str += this[i] + (i < Count-1 ? "\n" : "");
		}

		return str;
	}
	
}
