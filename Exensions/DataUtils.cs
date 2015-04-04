using UnityEngine;
using System;
using System.Text;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Reflection;

public static class DataUtils {
	
	public static void LogEach<T>(this T[] array) { array.LogEach(1); }
	public static void LogEach<T>(this T[] array, int perLine) {
		StringBuilder str = new StringBuilder();
		
		for (int i = 0; i < array.Length; i++) {
			str.Append(array[i].ToString());
			str.Append(", ");
			if ((1+i)%perLine == 0) { str.Append('\n'); }
		}
		Debug.Log(str.ToString());
	}
	
	//Thanks, William
	public static int SimpleHash(this byte[] data) {
		if (data != null) {
			int res = data.Length * data.Length * 31337;
			for (int i=0;i<data.Length;i++) {
				res ^= (data[i] << ((i % 4) * 8));
			}
			return res;
		} else {
			return 0;
		}
	}
	
	public static byte[] Chop(this byte[] array, int size, int start = 0) {
		if (start >= array.Length) { return null; }
		if (size + start > array.Length) {
			size = array.Length - start;
		}
		byte[] chopped = new byte[size];
		for (int i = 0; i < size; i++) {
			chopped[i] = array[i + start];
		}
		return chopped;
	}
	
	
	public static string ToHex(this byte b) {
		string s = "";
		
		byte large = (byte)(b >> 4);
		byte small = (byte)(b % 16);
		
		char a = 'A';
		char zero = '0';
		
		if (large >= 10) { 
			s += (char)(a + large-10);
		} else {
			s += (char)(zero + large);
		}
		
		if (small >= 10) { 
			s += (char)(a + small-10);
		} else {
			s += (char)(zero + small);
		}
		
		return s;
	}
	
	public static List<int> Permutation(int max) { return Permutation(max, max); }
	public static List<int> Permutation(int max, int length) {
		List<int> nums = new List<int>(max);
		for (int i = 0; i < max; i++) { nums.Add(i); }
		
		List<int> chosen = new List<int>(length);
		for (int i = 0; i < length; i++) {
			int j = nums.RandomIndex();
			chosen.Add(nums[j]);
			nums.RemoveAt(j);
		}
		
		return chosen;
	}
	
	public static List<T> TrimToLength<T>(this List<T> list, int length) {
		List<T> l = new List<T>();
		for (int i = 0; i < length; i++) {
			l.Add(list[i]);
		}
		return l;
	}
	
	public static void AddAll<T>(this List<T> list, IEnumerable<T> stuff) {
		foreach (T t in stuff) { list.Add(t); }
	}
	
	public static float ToFloat(this byte[] b, int i) { return BitConverter.ToSingle(b, i); }
	public static int ToInt(this byte[] b, int i) { return BitConverter.ToInt32(b, i); }
	
	 public static T DeepCopy<T>(T obj) {
		MemoryStream ms = new MemoryStream();
		BinaryFormatter bf = new BinaryFormatter();
		
		bf.Serialize(ms, obj);
		ms.Seek(0, SeekOrigin.Begin);
		T retval = (T)bf.Deserialize(ms);
		
		ms.Close();
		return retval;
	}
	
	public static Vector3 ToVector3(this byte[] b, int i) {
		Vector3 v = Vector3.zero;
		v.x = b.ToFloat(i);
		v.y = b.ToFloat(i+4);
		v.z = b.ToFloat(i+8);
		return v;
	}
	
	public static Quaternion ToQuaternion(this byte[] b, int i) {
		Quaternion q = Quaternion.identity;
		q.x = b.ToFloat(i);
		q.y = b.ToFloat(i+4);
		q.z = b.ToFloat(i+8);
		q.w = b.ToFloat(i+12);
		return q;
	}
	
	//Quick stupid accessor functions
	public static T LastElement<T>(this List<T> list) { if (list.Count == 0) { return default(T); } return list[list.Count-1]; }
	public static T FirstElement<T>(this List<T> list) { return list[0]; }
	
	//Get the nth element from the end of the list
	public static T FromEnd<T>(this List<T> list, int offset) { return list[list.Count-1-offset]; }
	
	//Add a list to the end of this list.
	public static void Append<T>(this List<T> list, List<T> add) { foreach (T o in add) { list.Add(o); } }
	public static void Append<T>(this List<T> list, T[] add) { foreach (T o in add) { list.Add(o); } }
	
	public static int IndexOf<T>(this List<T> list, Func<T, bool> search) {
		for (int i = 0; i < list.Count; i++) {
			if (search(list[i])) { return i; }
		}
		return -1;
	}
	
	public static T Find<T>(this IEnumerable<T> list, Func<T, bool> search) {
		foreach (T t in list) { 
			if (search(t)) { return t; } 
		}
		return default(T);
	}
	
	public static List<T> SortChain<T>(this List<T> list, Comparison<T> comparison) {
		list.Sort(comparison);
		return list;
	}
	
	//Get a random valid index
	public static int RandomIndex<T>(this List<T> list) { return (int)(Random.value * list.Count); }
	
	//Choose a random element from the list
	public static T Choose<T>(this List<T> list) { return list[list.RandomIndex()]; }
	
	//Choose an element from the list using weights
	public static T Choose<T>(this List<T> list, List<float> weights) { return list[Random.WeightedChoose(weights)]; }
	public static T Choose<T>(this List<T> list, float[] weights) { return list[Random.WeightedChoose(weights)]; }
	
	public static T Choose<T>(this Dictionary<T, float> table) {
		List<float> weights = table.Values.ToList();
		List<T> list = table.Keys.ToList();
		return list.Choose(weights);
	}
	
	
	public static string ListString<T>(this List<T> list) { return list.ListString<T>(','); }
	public static string ListString<T>(this List<T> list, char delim) {
		StringBuilder str = new StringBuilder();
		for (int i = 0; i < list.Count; i++) {
			T t = list[i];
			str.Append(t);
			if (i != list.Count-1) { str.Append(""+delim); }
		}
		return str.ToString();
	}
	
	
	
	//Choose 'num' elements from the list
	public static List<T> Choose<T>(this List<T> list, int num) {
		if (num >= list.Count) { return list.Shuffled(); }
		List<T> stuff = list.Clone();
		List<T> chosen = new List<T>();
		for (int i = 0; i < num; i++) {
			int index = stuff.RandomIndex();
			chosen.Add(stuff[index]);
			stuff.RemoveAt(index);
		}
		
		return chosen;
	}
	
	
	
	public static List<T> Choose<T>(this Dictionary<T, float> table, int num) {
		List<float> weights = table.Values.ToList();
		List<T> list = table.Keys.ToList();
		return list.Choose(weights, num);
	}
	public static List<T> Choose<T>(this List<T> list, List<float> weights, int num) {
		//Debug.Log(list.Count + " : " + weights.Count + " : " + num);
		if (num >= list.Count) { return list.Shuffled(); }
		
		List<T> stuff = list.Clone();
		List<float> weightsCopy = weights.Clone();
		
		List<T> chosen = new List<T>();
		
		for (int i = 0; i < num; i++) {
			int index = Random.WeightedChoose(weightsCopy);
			//Debug.Log(index);
			
			chosen.Add(stuff[index]);
			stuff.RemoveAt(index);
			weightsCopy.RemoveAt(index);
		}
		
		return chosen;
	}
	
	//Shallow Shuffle
	//Generate a shuffled version of the list
	public static List<T> Shuffled<T>(this List<T> list) {
		List<T> stuff = list.Clone();
		List<T> shuffled = new List<T>(list.Count);
		for (int i = 0; i < list.Count; i++) {
			int index = stuff.RandomIndex();
			shuffled[i] = stuff[index];
			stuff.RemoveAt(index);
		}
		return shuffled;
	}
	
	//Swap two elements
	public static void QSwap<T>(this List<T> list, int a, int b) {
		T temp = list[b];
		list[b] = list[a];
		list[a] = temp;
	}
	
	public static void Swap<T>(this List<T> list, int a, int b) {
		if (a >= 0 && b >= 0 && a < list.Count && b < list.Count) {
			T temp = list[b];
			list[b] = list[a];
			list[a] = temp;
		}
	}
	
	//Generate a new list which has all instances of 'toRemove' removed from it.
	//Does not mutate original list.
	public static List<T> RemoveAll<T>(this List<T> list, T toRemove) {
		List<T> l = new List<T>(list.Count);
		for (int i = 0; i < list.Count; i++) {
			if (!list[i].Equals(toRemove)) {
				l.Add(list[i]);
			}
		}
		return l;
	}
	
	//Generate an array of strings from a list of objects
	public static string[] ToStringArray<T>(this List<T> list) {
		string[] strings = new string[list.Count];
		for (int i = 0; i < list.Count; i++) { strings[i] = list[i].ToString(); }
		return strings;
	}
	
	//Generate a list of strings from a list of objects
	public static List<string> ToStringList<T>(this List<T> list) {
		List<string> strings = new List<string>(list.Count);
		for (int i = 0; i < list.Count; i++) { strings.Add(list[i].ToString()); }
		return strings;
	}
	
	//Generate an array of strings from an array of objects
	public static string[] ToStringArray<T>(this T[] list) {
		string[] strings = new string[list.Length];
		for (int i = 0; i < list.Length; i++) { strings[i] = list[i].ToString(); }
		return strings;
	}
	
	//Creates a shallow clone of a list.
	//Another list containing all of the elements in the same order as another list
	public static List<T> Clone<T>(this List<T> list) {
		List<T> clone = new List<T>(list.Count);
		for (int i = 0; i < list.Count; i++) { clone.Add(list[i]); }
		return clone;
	}
	
	//Quick stupid accessors for arrays
	public static T LastElement<T>(this T[] list) { return list[list.Length-1]; }
	public static T FirstElement<T>(this T[] list) { return list[0]; }
	
	//Grab the nth element from the end of the list
	public static T FromEnd<T>(this T[] list, int offset) { return list[list.Length-1-offset]; }
	
	//Get a random valid index
	public static int RandomIndex<T>(this T[] array) { return (int)(Random.value * array.Length); }
	
	//Choose a random element from an array
	public static T Choose<T>(this T[] array) { 
		if (array == null) { return default(T); }
		if (array.Length == 0) { return default(T); }
		return array[array.RandomIndex()]; 
	}
	
	//Choose an element from an array using weights
	public static T Choose<T>(this T[] array, float[] weights) {
		int index = (int)Mathf.Clamp(Random.WeightedChoose(weights), 0, array.Length-1);
		return array[index];
	}
	
	
	
	public static string LoadTextAsset(string filename) {
		TextAsset file = Resources.Load(filename, typeof(TextAsset)) as TextAsset;
		if (file == null) { 
			Debug.Log("Tried to load " + filename + ".txt/" + filename + ".csv - File does not exist");
			return "";
		}
		return file.text;
	}
	
	//Returns the lines as a string array from a csv formatted TextAsset (.txt)
	//removes all tabs. If you don't want tabs removed, just use the above function.
	public static string Load(string filename) { return LoadTextAsset(filename).Replace("\t", ""); }
	
	//Removes all tabs and splits the file by newlines. 
	public static string[] LoadLines(string filename) {
		return Load(filename).Replace("\t", "").Split('\n');
	}
	
	//Further converts it into a list by newlines and ','
	public static List<string> LoadList(string filename) {
		string text = Load(filename).ConvertNewlines().Replace(",\n","\n").Replace("\n", ",");
		return text.Split(',').ToList();
		
	}

	/// <summary>
	///  Attempts to parse the provided parameters into the specified type.
	/// This is VERY strict. Exactly the right number of parameters must be passed and they must all parse properly.
	/// The only thing that cannot possibly fail is String.
	/// Returns: object reference of the result. Null if improper parameters.
	/// </summary>
	/// <param name="typeName">The type of the returned object</param>
	/// <returns></returns>
	public static object ParseParameterListIntoType(this List<string> parameters, string typeName) {
		switch(typeName) {
			case "Vector2":
				Vector2 targetV2;
				PropertyInfo vector2ByName = typeof(Vector2).GetProperty(parameters[0], BindingFlags.Public | BindingFlags.Static | BindingFlags.GetProperty);
				if(vector2ByName != null) {
					if(parameters.Count != 1) { return null; }
					targetV2 = (Vector2)vector2ByName.GetValue(null, null);
				} else {
					if(parameters.Count != 2) { return null; }
					float x = 0.0f;
					try {
						x = System.Single.Parse(parameters[0]);
					} catch(System.FormatException) { return null; }
					float y = 0.0f;
					try {
						y = System.Single.Parse(parameters[1]);
					} catch(System.FormatException) { return null; }
					targetV2 = new Vector2(x, y);
				}
				return targetV2;
			case "Vector3":
				Vector3 targetV3;
				PropertyInfo vector3ByName = typeof(Vector3).GetProperty(parameters[0], BindingFlags.Public | BindingFlags.Static | BindingFlags.GetProperty);
				if(vector3ByName != null) {
					if(parameters.Count != 1) { return null; }
					targetV3 = (Vector3)vector3ByName.GetValue(null, null);
				} else {
					if(parameters.Count != 3) { return null; }
					float x = 0.0f;
					try {
						x = System.Single.Parse(parameters[0]);
					} catch(System.FormatException) { return null; }
					float y = 0.0f;
					try {
						y = System.Single.Parse(parameters[1]);
					} catch(System.FormatException) { return null; }
					float z = 0.0f;
					try {
						z = System.Single.Parse(parameters[2]);
					} catch(System.FormatException) { return null; }
					targetV3 = new Vector3(x, y, z);
				}
				return targetV3;
			case "Color":
				Color targetColor;
				PropertyInfo colorByName = typeof(Color).GetProperty(parameters[0], BindingFlags.Public | BindingFlags.Static | BindingFlags.GetProperty);
				if(colorByName != null) {
					if(parameters.Count != 1) { return null; }
					targetColor = (Color)colorByName.GetValue(null, null);
				} else {
					if(parameters.Count != 4) { return null; }
					float r = 0.0f;
					try {
						r = System.Single.Parse(parameters[0]);
					} catch(System.FormatException) { return null; }
					float g = 0.0f;
					try {
						g = System.Single.Parse(parameters[1]);
					} catch(System.FormatException) { return null; }
					float b = 0.0f;
					try {
						b = System.Single.Parse(parameters[2]);
					} catch(System.FormatException) { return null; }
					float a = 1.0f;
					try {
						a = System.Single.Parse(parameters[3]);
					} catch(System.FormatException) { return null; }
					targetColor = new Color(r, g, b, a);
				}
				return targetColor;
			case "Rect":
				if(parameters.Count != 4) { return null; }
				Rect targetRect;
				float l = 0.0f;
				try {
					l = System.Single.Parse(parameters[0]);
				} catch(System.FormatException) { return null; }
				float t = 0.0f;
				try {
					t = System.Single.Parse(parameters[1]);
				} catch(System.FormatException) { return null; }
				float w = 0.0f;
				try {
					w = System.Single.Parse(parameters[2]);
				} catch(System.FormatException) { return null; }
				float h = 1.0f;
				try {
					h = System.Single.Parse(parameters[3]);
				} catch(System.FormatException) { return null; }
				targetRect = new Rect(l, t, w, h);
				return targetRect;
			case "String":
				System.Text.StringBuilder bob = new System.Text.StringBuilder();
				foreach(string st in parameters) {
					bob.Append(st + " ");
				}
				string allparams = bob.ToString();
				return allparams.Substring(0, allparams.Length - 1);
			case "Char":
			case "SByte":
			case "Int16":
			case "Int32":
			case "Int64":
			case "Byte":
			case "UInt16":
			case "UInt32":
			case "UInt64":
			case "Single":
			case "Double":
				if(parameters.Count != 1) { return null; }
				try {
					// Use reflection to call the proper Parse method. Because I can.
					return System.Type.GetType("System."+typeName).GetMethod("Parse", BindingFlags.Public | BindingFlags.Static, null, new System.Type[] { typeof(string) }, null).Invoke(null, new string[] { parameters[0] });
				} catch(System.Reflection.TargetInvocationException) { // Is thrown in place of the Parse method's exceptions
					return null;
				}
			case "Boolean":
				if(parameters.Count != 1) { return null; }
				if(parameters[0] == "1" || parameters[0].Equals("on", System.StringComparison.InvariantCultureIgnoreCase) || parameters[0].Equals("yes", System.StringComparison.InvariantCultureIgnoreCase)) {
					return true;
				} else if(parameters[0] == "0" || parameters[0].Equals("off", System.StringComparison.InvariantCultureIgnoreCase) || parameters[0].Equals("no", System.StringComparison.InvariantCultureIgnoreCase)) {
					return false;
				} else {
					try {
						float val = System.Single.Parse(parameters[0]);
						return val >= 0.5f;
					} catch(System.FormatException) {
						try {
							return System.Boolean.Parse(parameters[0]);
						} catch(System.FormatException) {
							return null;
						}
					}
				}
			default:
				return null;
		}

	}
	
	#if !UNITY_WEBPLAYER
	public static void SaveToFile(this byte[] b, string path) {
		Directory.CreateDirectory(path.PreviousDirectory());
		
		File.WriteAllBytes(path, b);
		
		
	}
	
	public static byte[] LoadBytesFromFile(string path) {
		if (File.Exists(path)) {
			return File.ReadAllBytes(path);
		}
		return null;
		
	}
	
	#endif
	
}

///////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////

public static class DataFUnity {
	
	public static T[] AsArrayOf<T>(this Component[] comps) where T : Component { return comps.AsListOf<T>().ToArray(); }
	public static List<T> AsListOf<T>(this Component[] comps) where T : Component {
		List<T> list = new List<T>(comps.Length);
		foreach (Component comp in comps) {
			T t = comp.GetComponent<T>();
			if (t != null) { list.Add(t); }
		}
		return list;
	}
	
	public static T[] AsArrayOf<T>(this List<Component> comps) where T : Component { return comps.AsListOf<T>().ToArray(); }
	public static List<T> AsListOf<T>(this List<Component> comps) where T : Component {
		List<T> list = new List<T>(comps.Count);
		foreach (Component comp in comps) {
			T t = comp.GetComponent<T>();
			if (t != null) { list.Add(t); }
		}
		return list;
	}
	
	
	public static T[] AsArrayOf<T>(this GameObject[] gobs) where T : Component { return gobs.AsListOf<T>().ToArray(); }
	public static List<T> AsListOf<T>(this GameObject[] gobs) where T : Component {
		List<T> list = new List<T>(gobs.Length);
		foreach (GameObject gob in gobs) {
			T t = gob.GetComponent<T>();
			if (t != null) { list.Add(t); }
		}
		return list;
	}
	
	public static T[] AsArrayOf<T>(this List<GameObject> gobs) where T : Component { return gobs.AsListOf<T>().ToArray(); }
	public static List<T> AsListOf<T>(this List<GameObject> gobs) where T : Component {
		List<T> list = new List<T>(gobs.Count);
		foreach (GameObject gob in gobs) {
			T t = gob.GetComponent<T>();
			if (t != null) { list.Add(t); }
		}
		return list;
	}

	/*public static bool Contains<T>(this T[] a, T search) where T : IEquatable<T> {
		foreach(T item in a) {
			if(item.Equals(search)) {
				return true;
			}
		}
		return false;
	}*/
}

///////////////////////////////////////////////////////////////////////////////
//////////////////////////////////////////////////////////////////////////////
/////////////////////////////////////////////////////////////////////////////

public static class DataFTable {
	
	
	
	public static Color[] ToColorArray(this Table table) { return table.ToColorList().ToArray(); }
	public static List<Color> ToColorList(this Table table) {
		List<Color> list = new List<Color>();
		int i = 0;
		string key = ""+i;
		while (table.ContainsColor(key)) {
			list.Add(table.GetColor(key));
			
			i++;
			key = "" + i;
		}
		
		return list;
	}
	
	
	public static Table ToTable(this List<Color> list) {
		Table t = new Table();
		
		for (int i = 0; i < list.Count; i++) {
			t.SetColor("" + i, list[i]); 
		}
		
		return t;
	}	
	
	public static Table ToTable(this Color[] array) {
		Table t = new Table();
		
		for (int i = 0; i < array.Length; i++) {
			t.SetColor("" + i, array[i]);
		}
		
		return t;
	}
	
	
	
	
}

public static class DataFiles {
	
	public static JsonObject TryConvertCSVToJSON(string file) {
		string[] lines = File.ReadAllLines(file);
		string targetFile = file.UpToLast('.') + ".json";
		JsonObject values = new JsonObject();
		
		foreach (string line in lines) {
			if (line.Length < 3) { continue; }
			if (line.StartsWith("#")) { continue; }
			
			string[] split = line.Split(',');
			string key = split[0];
			string val = split[1];
			double numVal;
			if (double.TryParse(val, out numVal)) {
				values[key] = numVal;
			} 
			else if (val == "true") { values[key] = true; }
			else if (val == "false") { values[key] = false; }
			else {
				values[key] = val;
			}
			
		}
		
		if (values.Count > 0) {
			try {
				File.WriteAllText(targetFile, values.PrettyPrint());
				File.Delete(file);
				Debug.Log("Converted " + file + " to json successfully");
			} catch (Exception e) {
				Debug.Log("Tried to convert " + file + " to json. Unsuccessful: " + e.GetType());
			}
			
			
			return values;
		}
		
		return null;
	}
}
















