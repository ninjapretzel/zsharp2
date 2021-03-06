using UnityEngine;
using System.Text;
using System.Text.RegularExpressions;
using System.Collections;
using System.Collections.Generic;

public static class MarkdownReplacements {
	
	static Regex glyphs = new Regex( @"\[\w+\]" );
	/// <summary>
	/// Replaces glyph placeholders with the appropriate TextMeshPro sprite XML tags
	/// 
	/// Examples:
	///		The placeholder {abutton} will be translated to a sprite tag of 'abutton' in the default sheet.
	///		The placeholder {xbox_abutton} will be translated to a sprite tag for sheet 'xbox' and sprite 'abutton'
	///		
	/// </summary>
	/// <param name="markdown">markdown to replace.</param>
	/// <returns>Copy of <paramref name="markdown"/> with gylph placeholders replaced with the proper TextMeshPro sprite XML tags</returns>
	public static string ReplaceGlyphs(this string markdown) {
		StringBuilder str = markdown;

		int pos = 0;
		while (glyphs.IsMatch(str, pos)) {
			var match = glyphs.Match(str, pos);
			string val = match.Value.Substring(1, match.Length - 2);
			string sheet = null;
			string sprite = null;
			if (val.IndexOf("_") >= 0) {
				sheet = val.Substring(0, val.IndexOf("_"));
				sprite = val.Substring(val.IndexOf("_") + 1);

			} else {
				sprite = val;
			}

			StringBuilder rep = "<sprite";
			if (sheet != null) { rep += "=\"" + sheet + "\""; }
			if (sprite != null) { rep += " name=\"" + sprite + "\""; }
			rep += ">";

			if (rep.Length > 8) {
				str.Replace(match.Value, rep);
				pos = match.Index + rep.Length;
			} else {
				pos = match.Index + match.Length;
			}

		}

		return str;
	}

	static Regex bolds = new Regex( @"\*\*\ *?\S+?[\S \t]*?\*\*" );
	static Regex italics = new Regex( @"\*\ *?\S+?[\S \t]*?\*" );
	
	static Regex colorCode = new Regex( @"\\[A-Za-z]" );
	public static Dictionary<char, Color32> colorCodes = Defaults();
	
	public static Dictionary<char, Color32> Defaults() {
		Dictionary<char, Color32> codes = new Dictionary<char, Color32>();
		codes['r'] = Color.red;
		codes['o'] = new Color(1, .7f, 0);
		codes['y'] = Color.yellow;
		codes['g'] = Color.green;
		codes['b'] = new Color(.1f, .1f, 1);
		codes['i'] = new Color(.3f, 0, 1);
		codes['v'] = new Color(.8f, 0, 1);
		
		codes['c'] = Color.cyan;
		codes['w'] = Color.white;
		codes['k'] = Color.black;
		
		codes['u'] = Colors.HSV(76f/360f, .77f, .60f);
		
		codes['h'] = new Color(.5f, .5f, .5f);
		codes['q'] = new Color(.8f, .8f, .8f);
		codes['e'] = new Color(1, .6f, 0);
		codes['t'] = new Color(.8f, 1, .8f);
		codes['p'] = new Color(.8f, 1, .8f);
		codes['j'] = new Color(.5259f, .7098f, .8508f);
		codes['m'] = new Color(.8259f, .3214f, .8109f);
		codes['l'] = new Color(.5151f, .9131f, .3212f);
		
		return codes;
	}
	
	
	public static bool debugMode = false;
	
	public static string ReplaceMarkdown(this string markdown) {
		string str = markdown;
		str = str.ReplaceBolds();
		str = str.ReplaceItalics();
		
		str = str.ReplaceColors();
		
		return str;
	}
	
	public static string ReplaceColors(this string markdown) {
		string str = markdown;
		//gotta be able to be >implying things
		str = str.Replace(">", "\\u>");
		bool open = false;
		while (colorCode.IsMatch(str)) {
			Match match = colorCode.Match(str);
			
			string mat = match.Value;
			string rep = match.Value;
			rep = "";
			
			char ch = mat[1];
			if (colorCodes.ContainsKey(ch)) {
				string tag = "";
				if (open) {
					tag = "</color>";
				}
				tag += "<color=#";
				
				tag += colorCodes[ch].HexString();
				
				tag += ">";
				rep = tag;
				open = true;
			}
			
			
			if (debugMode) {
				Debug.Log("Matched " + mat + " : replacing with : " + rep);
			}
			str = colorCode.Replace(str, rep, 1);
			
		}
		
		if (open) { 
			str += "</color>";
		}
		return str;
	}
	
	public static string ReplaceBolds(this string markdown) {
		string str = markdown;
		
		while (bolds.IsMatch(str)) {
			Match match = bolds.Match(str);
			string mat = match.Value;
			string rep = match.Value;
			rep = rep.Substring(2, rep.Length-4);
			rep = "<b>" + rep + "</b>";
			
			if (debugMode) {
				Debug.Log("Matched " + mat + " : replacing with : " + rep);
			}
			str = bolds.Replace(str, rep, 1);
			
		}
		
		return str;
	}
	
	
	public static string ReplaceItalics(this string markdown) {
		string str = markdown;
		
		while (italics.IsMatch(str)) {
			Match match = italics.Match(str);
			string mat = match.Value;
			string rep = match.Value;
			rep = rep.Substring(1, rep.Length-2);
			rep = "<i>" + rep + "</i>";
			
			
			if (debugMode) {
				Debug.Log("Matched " + mat + " : replacing with : " + rep);
			}
			str = italics.Replace(str, rep, 1);
			
		}
		
		return str;
	}
	
}
