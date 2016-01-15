using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary> Extension on List(string) with functionality for a log with a maxiumumSize (for displaying to the user)</summary>
public class TextLog : List<string> {
	
	/// <summary> Maximum number of elements in the log </summary>
	public int maxSize = 30;
	/// <summary> compact converting to a regular List(string) </summary>
	public List<string> asList { get { return (List<string>)this; } }

	public TextLog() : base() { }
	public TextLog(IEnumerable<string> coll) : base(coll) { }

	/// <summary> Custom Add(string) method checks for maxLength </summary>
	public new void Add(string item) {
		if (Count > maxSize) {
			RemoveAt(0);
		}
		asList.Add(item);
	}

	/// <summary> Create a copy of this text log, add one element to it, and return the new element. </summary>
	public TextLog AddDirty(string item) {
		TextLog txt = new TextLog(this);

		txt.Add(item);

		return txt;
	}

	/// <summary> Convert this textLog to a single string, with each entry on its own line. </summary>
	public override string ToString() {
		string str = "";
		for (int i = 0; i < Count; i++) {
			str += this[i] + (i < Count-1 ? "\n" : "");
		}

		return str;
	}
	
}
