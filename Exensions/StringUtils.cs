using UnityEngine;
using System;
using System.Text;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

public static class StringUtils {
	
	static ASCIIEncoding encoder = new ASCIIEncoding();
	public static byte[] ToBytes(this string s) { 
		return encoder.GetBytes(s);
	}
	public static string GetString(this byte[] b, int length) {
		return encoder.GetString(b, 0, length);
	}

	///Misc functions
	public static bool IsNumber(this char c) { return c >= 48 && c < 57; }
	public static bool Contains(this string str, string s) { return str.IndexOf(s) != -1; }
	
	///Newline constants
	public static string WINDOWS_NEWLINE { get { return "" + (char)0x0d + (char)0x0a; } }
	public static string UNIX_NEWLINE { get { return "" + (char)0x0a; } }
	public static string MAC_NEWLINE { get { return "" + (char)0x0d; } }
	
	///Misc function to generate a string that has a space inserted every other character
	public static string Spacify(this string s) {
		StringBuilder str = new StringBuilder();
		for (int i = 0; i < s.Length; i++) {
			str.Append(s[i]);
			str.Append(" ");
		}
		return str.ToString();
	}

	public static bool ContainsAny(this string str, IEnumerable<string> col) {
		foreach (string s in col) {
			if (str.Contains(s)) { return true; }
		}
		return false;

	}
	
	///<summary>
	///Changes camel cased strings into uncamel cased strings for beautification
	///"exampleStupidStringThingy" -> "Example Stupid String Thingy"
	///"AnotherStupidExampleThingy" -> "Another Stupid Example Thingy"
	///</summary>
	public static string UnCamelCase(this string str) {
		string s = "" + char.ToUpper(str[0]);
		for (int i = 1; i < str.Length; i++) {
			if (char.IsUpper(str[i])) {
				s += " " + str[i];
			} else {
				s += str[i];
			}
		}
		return s;
	}
	
	public static bool EndsWith(this string s, string check) {
		if (s.Length < check.Length) { return false; }
		if (s.Length == check.Length) { return s == check; }
		string sub = s.Substring(s.Length - check.Length, check.Length);
		return sub == check;
	}
	
	///Simple convert string to bytes/from bytes
	public static byte[] GetBytes(this string s) {
		byte[] bytes = new byte[s.Length * sizeof(char)];
		System.Buffer.BlockCopy(s.ToCharArray(), 0, bytes, 0, bytes.Length);
		return bytes;
	}
	
	///Returns the given string with newline characters converted to the Unix standard
	public static string ConvertNewlines(this string s) {
		string ss = s.Replace(WINDOWS_NEWLINE, UNIX_NEWLINE);
		return ss.Replace(MAC_NEWLINE, UNIX_NEWLINE);
	}
	
	///<summary>Returns the substring of a string up to the first instance of a character.</summary>
	public static string UpToFirst(this string s, char c) {
		int index = s.IndexOf(c);
		if (index == -1) { return s; }
		return s.Substring(0, index);
	}
	///<summary>Returns the substring of a string up to the first instance of a string.</summary>
	public static string UpToFirst(this string s, string c) {
		int index = s.IndexOf(c);
		if (index == -1) { return s; }
		return s.Substring(0, index);
	}
	
	///<summary>Returns the substring of a string up to the last instance of a character.</summary>
	public static string UpToLast(this string s, char c) {
		int lastIndex = s.LastIndexOf(c);
		if (lastIndex == -1) { return s; }
		return s.Substring(0, lastIndex);
	}
	///<summary>Returns the substring of a string up to the last instance of a string.</summary>
	public static string UpToLast(this string s, string c) {
		int lastIndex = s.LastIndexOf(c);
		if (lastIndex == -1) { return s; }
		return s.Substring(0, lastIndex);
	}
	
	///<summary>Returns the substring of a string from the first instance of a character. (Not Inclusive)</summary>
	public static string FromFirst(this string s, char c) {
		int index = s.IndexOf(c);
		if (index == -1) { return s; }
		return s.Substring(index+1);
	}
	///<summary>Returns the substring of a string from the first instance of a string. (Not Inclusive)</summary>
	public static string FromFirst(this string s, string c) {
		int index = s.IndexOf(c);
		if (index == -1) { return s; }
		return s.Substring(index+c.Length);
	}
	
	///<summary>Returns the substring of a string from the last instance of a character. (Not Inclusive)</summary>
	public static string FromLast(this string s, char c) {
		int lastIndex = s.LastIndexOf(c);
		if (lastIndex == -1) { return s; }
		return s.Substring(lastIndex+1);
	}
	///<summary>Returns the substring of a string from the last instance of a string. (Not Inclusive)</summary>
	public static string FromLast(this string s, string c) {
		int lastIndex = s.LastIndexOf(c);
		if (lastIndex == -1) { return s; }
		return s.Substring(lastIndex+c.Length);
	}
	
	
	///Converts all backslashes in a path to forward slashes.
	public static string ForwardSlashPath(this string path) {
		return path.Replace('\\', '/');
	}
	
	///Gets the last folder's name of a given string
	public static string DirectoryName(this string path) {
		return path.Substring(path.LastIndexOf("/") + 1);
	}
	
	///Moves a string representing a path to point one directory above.
	public static string PreviousDirectory(this string path) {
		return path.Substring(0, path.LastIndexOf("/"));
	}
	
	///Returns the given string with all spaces and tabs removed
	public static string RemoveWhitespace(this string s) {
		return s.Replace(" ", "").Replace("\t", "");
	}
	
	///Returns a given string with all characters from another given string removed.
	public static string RemoveAllChars(this string str, string s) {
		string ss = str;
		foreach (char c in s) { ss = ss.RemoveAll(c); }
		return ss;
	}
	
	///Retuns the same string if str Length is less than str.length
	///otherwise returns a substring from the begining of the string with an optional concatination
	public static string MinSubstring(this string str, int length, char concat = (char)0x00) {
		if (str.Length <= length) { return str; }
		return str.Substring(0, length) + ((concat != (char)0x00) ? ""+concat : "");
	}
	
	///Returns a string with all instances of strings (or characters) removed
	public static string RemoveAll(this string str, string s) { return str.Replace(s, ""); }
	public static string RemoveAll(this string str, char c) { return str.Replace(""+c, ""); }
	
	///Format a float to some number of decimal places
	public static string Format(this float f, int dec) {
		bool neg = f < 0;
		if (neg) { f *= -1; }
		
		int ir = (int)f;
		float fr = f - ir;
		string s = "";
		if (dec > 0 && ir == 0 && fr < Mathf.Pow(10, -dec)) {
			s = "0.";
			if (f < 0) { s = "-" + s; }
			for (int i = 0; i < dec; i++) { s += "0"; }
			return (neg ? "-" : "") + s;
		}
		if (dec > 0 && fr > 0) {
			s = "" + fr;
			if (s.Length <= dec+1) { s = s.Substring(1); }
			else { s = s.Substring(1, dec+1); }
		}
		return (neg ? "-" : "") + ir + s;
	}
	//Formats an int value so commas are inserted every 3 places.
	public static string Commify(this int i) {
		string str = "" + i;
		int ind = str.Length;
		ind -= 3;
		while (ind > 0) {
			str = str.Insert(ind, ",");
			ind -= 3;
		}
		return str;
	}
	
	public static string Commify(this float f) { return ((int)f).Commify(); }
	public static string Commify(this float f, int places) {
		string s = ((int)f).Commify();
		float fract = f.Fract();
		if (places > 0 && fract > 0) { s += "." + fract.Format(places).FromLast('.'); }
		return s;
	}
	
	static string[] notations = { "K", 
									"M",   "B",   "T",   "Qa",   "Qi",   "Sx",   "Sp",   "Oc",   "Nn",   "Dc", 
									"UDc", "DDc", "TDc", "QaDc", "QtDc", "SxDc", "SpDc", "OcDc", "NnDc", "Vg",
									"UVg", "DVg", "TVg", "QaVg", "QtVg", "SxVg", "SpVg", "OcVg", "NnVg", "Tg",
									"UTg", "DTg", "TTg", "QaTg", "QtTg", "SxTg", "SpTg", "OcTG", "NnTg", "Qd",
									"UQd", "DQd", "TQd", "QaQd", "QtQd", "SxQd", "SpQd", "OcQd", "NnQd", "Qq",
									"UQq", "DQq", "TQq", "QaQq", "QtQq", "SxQq", "SpQq", "OcQq", "NnQq", "Sg",
									"USg", "DSg", "TSg", "QaSg", "QtSg", "SxSg", "SpSg", "OcSg", "NnSg", "St",
									"USt", "DSt", "TSt", "QaSt", "QtSt", "SxSt", "SpSt", "OcSt", "NnSt", "Og",
									"UOg", "DOg", "TOg", "QaOg", "QtOg", "SxOg", "SpOg", "OcOg", "NnOg", "Ng",
									"UNg", "DNg", "TNg", "QaNg", "QtNg", "SxNg", "SpNg", "OcNg", "NnNg", "Cnt",
									"UCnt"
								};

	public static string ShortString(this float f, int places = 0) {
		if (f.IsNAN()) { return "NaN"; }
		if (f == 0) { return "0"; }

		string notationValue = "";
		float val = f;
		bool neg = val < 0;
		if (neg) { val *= -1; }

		if (f >= 1000f) {
			val /= 1000f;
			int b = 0;
			
			while(val.Round() >= 1000) {
				val /= 1000f;
				b++;
			}
			if (b >= notations.Length) { return "WAY TOO HIGH"; }
			else { notationValue = notations[b]; }
			
		}
		
		if (notationValue == "") { return f.Commify(places); }
		return (neg ? "-" : "") +  Mathf.Round(val * 1000f) / 1000f + notationValue;
	}

	

	public static string ShortStringD(this double d, int places = 3) {
		if (d == double.NaN) { return "NaN"; }
		if (d == 0) { return "0"; }

		string notationValue = "";
		double val = d;
		bool neg = val < 0;
		if (neg) { val *= -1; }

		if (d >= 1000d) {
			val /= 1000d;
			int b = 0;

			while (val.Round() >= 1000) {
				val /= 1000d;
				b++;
			}
			if (b >= notations.Length) { return "WAY TOO HIGH"; } else { notationValue = notations[b]; }

		}

		if (notationValue == "") { return ((float)d).Commify(places); }
		return Math.Round(val * 1000d) / 1000d + notationValue;
	}
	
	
	//Formats a float as if it represents time in seconds
	public static string TimeFormat(this float f) { return f.TimeFormat(2); }
	public static string TimeFormat(this float f, int places) {
		float hr = Mathf.Floor((f / 3600.0f));
		float min = Mathf.Floor((f / 60.0f) % 60.0f);
		
		float sec = (f % 60.0f);
		
		string s = "";
		if (hr > 0) { 
			s += hr + ":"; 
			if (min < 10) { s += "0"; }
		}
		
		s += min + ":";
		if (sec < 10) { s += "0"; }
		s += sec.Format(places);
		
		return s;
	}
	
	
	
	//Parsing functions
	public static float ParseFloat(this string s) { return float.Parse(s); }
	public static int ParseInt(this string s) { return int.Parse(s); }
	public static byte ParseByte(this string s) { return byte.Parse(s, NumberStyles.HexNumber); }
	public static Color ParseColor(this string s) { return Colors.FromString(s); }
	public static Color ParseColor(this string s, char delim) { return Colors.FromString(s, delim); }
	public static Table ParseTable(this string s) { return Table.CreateFromLine(s); }
	public static Table ParseTable(this string s, char delim) { return Table.CreateFromLine(s, delim); }
	public static List<string> ParseStringList(this string s) { return s.ParseStringList(','); }
	public static List<string> ParseStringList(this string s, char delim) { return s.Split(delim).ToList(); }
	
	public static StringMap ParseStringMap(this string s) { return s.ParseStringMap(','); }
	public static StringMap ParseStringMap(this string s, char delim) { return StringMap.CreateFromLine(s, delim); }
	
	
	//Parse a date from a string
	public static System.DateTime ParseDate(this string s) {
		char[] splits = new char[3];
		splits[0] = '/';
		splits[1] = ' ';
		splits[2] = ':';
		
		
		string[] strs = s.Split(splits);
		if (strs.Length < 6) { return System.DateTime.Now.AddDays(-1); }
		int year = int.Parse(strs[2]);
		int month = int.Parse(strs[0]);
		int day = int.Parse(strs[1]);
		
		int hr = int.Parse(strs[3]);
		int min = int.Parse(strs[4]);
		int sec = int.Parse(strs[5]);
		
		if (strs[6] == "PM") { hr += 12; }
		
		System.DateTime dt = new System.DateTime(year, month, day, hr, min, sec);
		
		return dt;
	}
	
	//Convert a System.DateTime object to a string with a consistant representation.
	//Can be safely used for storing/loading dates across platforms and locales
	public static string DateToString(this System.DateTime dt) {
		string str = "";
		
		int hr = dt.Hour;
		int min = dt.Minute;
		int sec = dt.Second;
		bool am = true;
		if (hr > 12) { hr -= 12; am = false; }
		
		int year = dt.Year;
		int month = dt.Month;
		int day = dt.Day;
		
		str = "" + month + "/" + day + "/" + year + " ";
		if (hr < 10) { str += "0"; }
		str += "" + hr + ":";
		
		if (min < 10) { str += "0"; }
		str += "" + min + ":";
		
		if (sec < 10) { str += "0"; }
		str += "" + sec + " ";
		if (am) { str += "AM"; }
		else { str += "PM"; }
		
		return str;
	}
	
	//Wrap PlayerPrefs.SetString
	public static void Save(this string s, string key) {
		PlayerPrefs.SetString(key, s);
	}
	
	//Extracts a section between matching '{' and '}' characters
	public static string ExtractSection(this string s) { return s.ExtractSection(0); }
	public static string ExtractSection(this string s, int st) {
		int start = s.IndexOf('{', st);
		Stack<int> stack = new Stack<int>();
		
		stack.Push(start);
		
		int end = s.Length;
		int i = start;
		while (stack.Count > 0) {
			int open = s.IndexOf('{', i+1);
			int close = s.IndexOf('}', i+1);
			
			if (open == -1 || close == -1) {
				if (close == -1) {
					return s.Substring(start);
				} else {
					stack.Pop();
					if (stack.Count == 0) { end = close; }
					i = close;
				}
			
			} else {
				if (open < close) {
					stack.Push(open);
					i = open;
				} else {
					stack.Pop();
					if (stack.Count == 0) { end = close; }
					i = close;
				}
			}
			
		}
		
		
		return s.Substring(start+1, (end-start-1));
	}
	
	//Parse a string and replace all escape character newlines with the real thing
	//used on strings that are expected to be modified from the inspector so they can support newline characters.
	public static string ParseNewlines(this string input) {
		return input.Replace("\\n", "\n");
	}

	/// <summary>
	/// Splits a <c>string</c> using a Unicode character, unless that character is between two instances of a container.
	/// </summary>
	/// <param name="st">The <c>string</c> to split</param>
	/// <param name="separator">Unicode character that delimits the substrings in this instance</param>
	/// <param name="container">Container character. Any <paramref name="separator"/> characters that occur between two instances of this character will be ignored</param>
	/// <returns>Array of <c>string</c> objects that are the resulting substrings</returns>
	public static string[] SplitUnlessInContainer(this string st, char separator, char container, StringSplitOptions options = StringSplitOptions.None) {
		List<string> results = new List<string>();
		bool inContainer = false;
		StringBuilder current = new StringBuilder();
		foreach (char c in st) {
			if (c == container) {
				inContainer = !inContainer;
				continue;
			}

			if (!inContainer) {
				if (c == separator) {
					switch (options) {
						case StringSplitOptions.RemoveEmptyEntries: {
							if (current.Length > 0) {
								results.Add(current.ToString());
							}
							current.Length = 0;
							break;
						}
						case StringSplitOptions.None: {
							results.Add(current.ToString());
							current.Length = 0;
							break;
						}
					}
				} else {
					current.Append(c);
				}
			} else {
				current.Append(c);
			}
		}

		if (current.Length > 0) {
			results.Add(current.ToString());
		}

		return results.ToArray<string>();
	}

	// Similar to the previous method, this will replace one character with another, but only outside of the container character.
	// ReplaceMe and ReplaceWith should not be the same as Container, or this will produce somewhat unpredictable results.
	public static string ReplaceUnlessInContainer(this string st, char replaceMe, char replaceWith, char container) {
		StringBuilder bob = new StringBuilder(st.Length);
		bool inContainer = false;
		foreach(char currentChar in st) {
			if(currentChar == container) { inContainer = !inContainer; }
			if(currentChar == replaceMe && !inContainer) {
				bob.Append(replaceWith);
			} else {
				bob.Append(currentChar);
			}
		}
		return bob.ToString();
	}

	// Replaces only first instance of a certain character.
	public static string ReplaceFirst(this string st, char replaceMe, char replaceWith) {
		StringBuilder bob = new StringBuilder(st.Length);
		int index = st.IndexOf(replaceMe);
		if(index >= 0) {
			bob.Append(st, 0, index);
			bob.Append(replaceWith);
			if(index < st.Length) {
				bob.Append(st.Substring(index + 1));
			}
			//Debug.Log(st.Substring(0, index));
			return bob.ToString();
		} else {
			return st;
		}
	}

	// Replaces only the last instance of a certain character.
	public static string ReplaceLast(this string st, char replaceMe, char replaceWith) {
		StringBuilder bob = new StringBuilder(st.Length);
		int index = st.LastIndexOf(replaceMe);
		if(index >= 0) {
			bob.Append(st, 0, index);
			bob.Append(replaceWith);
			if(index < st.Length) {
				bob.Append(st.Substring(index + 1));
			}
			//Debug.Log(st.Substring(0, index));
			return bob.ToString();
		} else {
			return st;
		}
	}

	// Replaces the first and last instance of a certain character.
	public static string ReplaceFirstAndLast(this string st, char replaceMe, char replaceWith) {
		return st.ReplaceFirst(replaceMe, replaceWith).ReplaceLast(replaceMe, replaceWith);
	}
	
}





















