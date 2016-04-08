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

	#region Maximal Selectors
	
	/// <summary> Selectes the 'largest' element from an enumerable collection </summary>
	/// <typeparam name="T">Type of collection </typeparam>
	/// <param name="e">collection to enumerate</param>
	/// <param name="selector">function turning a T into a float</param>
	/// <returns>The object in the collection that has the largest value based on the selector</returns>
	public static T Maximal<T>(this IEnumerable<T> e, Func<T, float> selector) {
		T max = default(T);
		float val = float.MinValue;
		foreach (T t in e) {
			float tval = selector(t);
			if (tval > val) {
				val = tval;
				max = t;
			}
		}

		return max;
	}
	
	/// <summary> Selectes the 'largest' element from an enumerable collection </summary>
	/// <typeparam name="T">Type of collection </typeparam>
	/// <param name="e">collection to enumerate</param>
	/// <param name="selector">function turning a T into a double</param>
	/// <returns>The object in the collection that has the largest value based on the selector</returns>
	public static T Maximal<T>(this IEnumerable<T> e, Func<T, double> selector) {
		T max = default(T);
		double val = double.MinValue;
		foreach (T t in e) {
			double tval = selector(t);
			if (tval > val) {
				val = tval;
				max = t;
			}
		}

		return max;
	}
	
	/// <summary> Selectes the 'largest' element from an enumerable collection </summary>
	/// <typeparam name="T">Type of collection </typeparam>
	/// <param name="e">collection to enumerate</param>
	/// <param name="selector">function turning a T into a int</param>
	/// <returns>The object in the collection that has the largest value based on the selector</returns>
	public static T Maximal<T>(this IEnumerable<T> e, Func<T, int> selector) {
		T max = default(T);
		int val = int.MinValue;
		foreach (T t in e) {
			int tval = selector(t);
			if (tval > val) {
				val = tval;
				max = t;
			}
		}

		return max;
	}
	
	/// <summary> Selectes the 'largest' element from an enumerable collection </summary>
	/// <typeparam name="T">Type of collection </typeparam>
	/// <param name="e">collection to enumerate</param>
	/// <param name="selector">function turning a T into a long</param>
	/// <returns>The object in the collection that has the largest value based on the selector</returns>
	public static T Maximal<T>(this IEnumerable<T> e, Func<T, long> selector) {
		T max = default(T);
		long val = long.MinValue;
		foreach (T t in e) {
			long tval = selector(t);
			if (tval > val) {
				val = tval;
				max = t;
			}
		}

		return max;
	}
	
	/// <summary> Selectes the 'largest' element from an enumerable collection </summary>
	/// <typeparam name="T">Type of collection </typeparam>
	/// <param name="e">collection to enumerate</param>
	/// <param name="selector">function turning a T into a decimal</param>
	/// <returns>The object in the collection that has the largest value based on the selector</returns>
	public static T Maximal<T>(this IEnumerable<T> e, Func<T, decimal> selector) {
		T max = default(T);
		decimal val = decimal.MinValue;
		foreach (T t in e) {
			decimal tval = selector(t);
			if (tval > val) {
				val = tval;
				max = t;
			}
		}

		return max;
	}
	#endregion

	#region Minimal Selectors
	
	/// <summary> Selectes the 'smallest' element from an enumerable collection </summary>
	/// <typeparam name="T">Type of collection </typeparam>
	/// <param name="e">collection to enumerate</param>
	/// <param name="selector">function turning a T into a float</param>
	/// <returns>The object in the collection that has the smallest value based on the selector</returns>
	public static T Minimal<T>(this IEnumerable<T> e, Func<T, float> selector) {
		T min = default(T);
		float val = float.MaxValue;
		foreach (T t in e) {
			float tval = selector(t);
			if (tval < val) {
				val = tval;
				min = t;
			}
		}

		return min;
	}

	/// <summary> Selectes the 'smallest' element from an enumerable collection </summary>
	/// <typeparam name="T">Type of collection </typeparam>
	/// <param name="e">collection to enumerate</param>
	/// <param name="selector">function turning a T into a double</param>
	/// <returns>The object in the collection that has the smallest value based on the selector</returns>
	public static T Minimal<T>(this IEnumerable<T> e, Func<T, double> selector) {
		T min = default(T);
		double val = double.MaxValue;
		foreach (T t in e) {
			double tval = selector(t);
			if (tval < val) {
				val = tval;
				min = t;
			}
		}

		return min;
	}

	/// <summary> Selectes the 'smallest' element from an enumerable collection </summary>
	/// <typeparam name="T">Type of collection </typeparam>
	/// <param name="e">collection to enumerate</param>
	/// <param name="selector">function turning a T into a int</param>
	/// <returns>The object in the collection that has the smallest value based on the selector</returns>
	public static T Minimal<T>(this IEnumerable<T> e, Func<T, int> selector) {
		T min = default(T);
		int val = int.MaxValue;
		foreach (T t in e) {
			int tval = selector(t);
			if (tval < val) {
				val = tval;
				min = t;
			}
		}

		return min;
	}

	/// <summary> Selectes the 'smallest' element from an enumerable collection </summary>
	/// <typeparam name="T">Type of collection </typeparam>
	/// <param name="e">collection to enumerate</param>
	/// <param name="selector">function turning a T into a long</param>
	/// <returns>The object in the collection that has the smallest value based on the selector</returns>
	public static T Minimal<T>(this IEnumerable<T> e, Func<T, long> selector) {
		T min = default(T);
		long val = long.MaxValue;
		foreach (T t in e) {
			long tval = selector(t);
			if (tval < val) {
				val = tval;
				min = t;
			}
		}

		return min;
	}

	/// <summary> Selectes the 'smallest' element from an enumerable collection </summary>
	/// <typeparam name="T">Type of collection </typeparam>
	/// <param name="e">collection to enumerate</param>
	/// <param name="selector">function turning a T into a decimal</param>
	/// <returns>The object in the collection that has the smallest value based on the selector</returns>
	public static T Minimal<T>(this IEnumerable<T> e, Func<T, decimal> selector) {
		T min = default(T);
		decimal val = decimal.MaxValue;
		foreach (T t in e) {
			decimal tval = selector(t);
			if (tval < val) {
				val = tval;
				min = t;
			}
		}

		return min;
	}
	#endregion
	
	/// <summary> Quickly hashes a byte array into an int32 </summary>
	/// <param name="data">Byte array to hash</param>
	/// <returns>int32 hash based off of data </returns>
	public static int SimpleHash(this byte[] data) {
		if (data != null) {
			int result = data.Length * data.Length * 31337;
			for (int i = 0; i < data.Length; ++i) {
				result ^= (data[i] << ((i % 4) * 8));
			}
			return result;
		} else {
			return 0;
		}
	}
	
	/// <summary> Chops an array into a sub-array (analogous to substring) </summary>
	/// <typeparam name="T">Generic type</typeparam>
	/// <param name="array">Source data to chop</param>
	/// <param name="size">Size of sub-array</param>
	/// <param name="start">Start position in source data</param>
	/// <returns>Array of same type, of Length size, of elements in the source array </returns>
	public static T[] Chop<T>(this T[] array, int size, int start = 0) {
		if (start >= array.Length) { return null; }
		if (size + start > array.Length) {
			size = array.Length - start;
		}
		T[] chopped = new T[size];
		for (int i = 0; i < size; ++i) {
			chopped[i] = array[i + start];
		}
		return chopped;
	}
	
	/// <summary> 
	/// Converts a byte to its two character hex string.
	/// Ex. 
	/// 255 becomes "FF"
	/// 200 becomes "C8"
	/// 64 becomes "40"
	/// </summary>
	/// <param name="b">byte to convert</param>
	/// <returns>Byte converted to hex string</returns>
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

	/// <summary> Creates a permutation of numbers from [0...max) </summary>
	/// <param name="max">End of range to use, not inclusive</param>
	/// <returns>List of numbers from [0...max) shuffled into a random order </returns>
	public static IList<int> Permutation(int max) { return Permutation(max, max); }
	/// <summary> Creates a permutation of numbers from [0...max) that is length long</summary>
	/// <param name="max">End of range to use, not inclusive</param>
	/// <param name="length">Length of permutation. Should be <= max </param>
	/// <returns>Permutation length long, of numbers from [0...max)</returns>
	public static IList<int> Permutation(int max, int length) {
		List<int> nums = new List<int>(max);
		for (int i = 0; i < max; ++i) { nums.Add(i); }
		
		List<int> chosen = new List<int>(length);
		for (int i = 0; i < length; ++i) {
			int j = nums.RandomIndex();
			chosen.Add(nums[j]);
			nums.RemoveAt(j);
		}
		
		return chosen;
	}

	/// <summary> Adds all elements from an enumerable collection into a given list.</summary>
	/// <typeparam name="T">Generic type</typeparam>
	/// <param name="list">List to add elements into</param>
	/// <param name="stuff">Enumerable collection of elements to add to the list. </param>
	public static void AddAll<T>(this IList<T> list, IEnumerable<T> stuff) { foreach (T t in stuff) { list.Add(t); } }
	
	/// <summary> Performs a deep copy on a given object.</summary>
	/// <typeparam name="T">Generic type</typeparam>
	/// <param name="obj">Object to deep-copy</param>
	/// <returns>A deep copy of obj</returns>
	public static T DeepCopy<T>(T obj) {
		MemoryStream ms = new MemoryStream();
		BinaryFormatter bf = new BinaryFormatter();
		
		bf.Serialize(ms, obj);
		ms.Seek(0, SeekOrigin.Begin);
		T retval = (T)bf.Deserialize(ms);
		
		ms.Close();
		return retval;
	}
	
	/// <summary> Get the last element in a list</summary>
	/// <typeparam name="T">Generic Type</typeparam>
	/// <param name="list">List to grab the last element of</param>
	/// <returns>Last element of list</returns>
	public static T LastElement<T>(this IList<T> list) { if (list.Count == 0) { return default(T); } return list[list.Count - 1]; }
	
	/// <summary> Get the nth element from the end of the list </summary>
	/// <typeparam name="T">Generic Type</typeparam> 
	/// <param name="list">List to grab from</param>
	/// <param name="offset">elements from the end to grab. (0 gives the last element) </param>
	/// <returns>Element (offset) elements from the end of the list</returns>
	public static T FromEnd<T>(this IList<T> list, int offset) { return list[list.Count - 1 - offset]; }
	
	/// <summary> Get the first index of an element satisfying some criteria </summary>
	/// <typeparam name="T">Generic Type</typeparam>
	/// <param name="list">List to search in</param>
	/// <param name="search">Method to check each element</param>
	/// <returns>Index of first object that satisfies (search), or -1 if no elements do</returns>
	public static int IndexOf<T>(this IList<T> list, Func<T, bool> search) {
		for (int i = 0; i < list.Count; ++i) {
			if (search(list[i])) { return i; }
		}
		return -1;
	}
	
	/// <summary>Searches an array for the first element that satisfies a condition</summary>
	/// <typeparam name="T">Generic Type</typeparam>
	/// <param name="list"></param>
	/// <param name="search"></param>
	/// <returns></returns>
	public static T Find<T>(this IEnumerable<T> list, Func<T, bool> search) {
		foreach (T t in list) { 
			if (search(t)) { return t; } 
		}
		return default(T);
	}
	
	/// <summary> Returns a random valid index from an IList</summary>
	/// <typeparam name="T">Generic Type</typeparam>
	/// <param name="list">List to get a valid index from</param>
	/// <returns>A random integer between [0...list.Count)</returns>
	public static int RandomIndex<T>(this IList<T> list) { return (int)(Random.value * list.Count); }
	
	/// <summary> Chooses a random element from a given IList</summary>
	/// <typeparam name="T">Generic Type</typeparam>
	/// <param name="list">List to choose from </param>
	/// <returns>Randomly selected element inside of list </returns>
	public static T Choose<T>(this IList<T> list) { return list[list.RandomIndex()]; }
	
	/// <summary> Chooses an element from a list, given a set of weights to use to adjust the probabilities </summary>
	/// <typeparam name="T">Generic Type</typeparam>
	/// <param name="list">List to choose from</param>
	/// <param name="weights">List of weights to use for probabilities </param>
	/// <returns>An element from list, randomly selected based on probabilities in weights</returns>
	public static T Choose<T>(this IList<T> list, IList<float> weights) { return list[Random.WeightedChoose(weights)]; }
	
	/// <summary> Chooses a random key from an IDictionary based off of its value as a weight </summary>
	/// <typeparam name="T">Generic Type</typeparam>
	/// <param name="table">IDictionary to use as both choices and weights</param>
	/// <returns>Randomly selected key from Table, based off of values as weights</returns>
	public static T Choose<T>(this IDictionary<T, float> table) {
		List<float> weights = table.Values.ToList();
		List<T> list = table.Keys.ToList();
		return list.Choose(weights);
	}
	
	/// <summary> Create a comma separated string from a list</summary>
	/// <typeparam name="T">Generic type</typeparam>
	/// <param name="list">List of objects</param>
	/// <returns>String holding elements of list separated by commas </returns>
	public static string ListString<T>(this IList<T> list) { return list.ListString<T>(','); }
	public static string ListString<T>(this IList<T> list, char delim) {
		StringBuilder str = new StringBuilder();
		for (int i = 0; i < list.Count; ++i) {
			T t = list[i];
			str.Append(t);
			if (i != list.Count - 1) { str.Append("" + delim); }
		}
		return str.ToString();
	}
	
	/// <summary> Randomly choose num elements from a list</summary>
	/// <typeparam name="T">Generic Type</typeparam>
	/// <param name="list">List to choose from</param>
	/// <param name="num">Count of elements to choose</param>
	/// <returns>A new list, of size num, of elements randomly selected from list</returns>
	public static IList<T> Choose<T>(this IList<T> list, int num) {
		if (num >= list.Count) { return list.Shuffled(); }
		List<T> stuff = new List<T>(list);

		List<T> chosen = new List<T>();
		for (int i = 0; i < num; ++i) {
			int index = stuff.RandomIndex();
			chosen.Add(stuff[index]);
			stuff.RemoveAt(index);
		}
		
		return chosen;
	}
	
	
	/// <summary> Randomly choose num keys from a IDictionary </summary>
	/// <typeparam name="T">Generic Type</typeparam>
	/// <param name="table">IDictionary of weighted key/value pairs </param>
	/// <param name="num">Number of elements to choose </param>
	/// <returns>A list containing num keys from table</returns>
	public static List<T> Choose<T>(this IDictionary<T, float> table, int num) {
		List<float> weights = table.Values.ToList();
		List<T> list = table.Keys.ToList();
		return list.Choose(weights, num);
	}


	/// <summary> Weighted choose num elements from a list</summary>
	/// <typeparam name="T">Generic Type</typeparam>
	/// <param name="list">List of elements to choose from</param>
	/// <param name="weights">List of weights in parallel with list</param>
	/// <param name="num">Number of elements to choose</param>
	/// <returns>List of length num of elements randomly selected from list. </returns>
	public static List<T> Choose<T>(this IList<T> list, IList<float> weights, int num) {
		if (num >= list.Count) { return list.Shuffled(); }
		List<T> stuff = new List<T>(list);
		List<float> weightsCopy = new List<float>(weights);
		List<T> chosen = new List<T>();
		
		for (int i = 0; i < num; ++i) {
			int index = Random.WeightedChoose(weightsCopy);

			chosen.Add(stuff[index]);
			stuff.RemoveAt(index);
			weightsCopy.RemoveAt(index);
		}
		
		return chosen;
	}

	/// <summary> Quick shuffle of a list creates a new list with the elements of the given list shuffled into a random order</summary>
	/// <typeparam name="T">Generic Type</typeparam>
	/// <param name="list">List to shuffle</param>
	/// <returns>A copy of list with all of the elements in a random order </returns>
	public static List<T> Shuffled<T>(this IList<T> list) {
		List<T> stuff = new List<T>(list);
		List<T> shuffled = new List<T>(list.Count);
		for (int i = 0; i < list.Count; ++i) {
			int index = stuff.RandomIndex();
			shuffled[i] = stuff[index];
			stuff.RemoveAt(index);
		}
		return shuffled;
	}
	
	/// <summary> Swaps elements in a list without checking indexes</summary>
	/// <typeparam name="T">Generic Type</typeparam>
	/// <param name="list">List to swap elements in</param>
	/// <param name="a">index of element 1</param>
	/// <param name="b">index of element 2</param>
	public static void Swap<T>(this IList<T> list, int a, int b) {
		T temp = list[b];
		list[b] = list[a];
		list[a] = temp;
	}
	
	/// <summary>Removes all elements in a list that equal a given object</summary>
	/// <typeparam name="T">Generic Type</typeparam>
	/// <param name="list">List to remove elements from</param>
	/// <param name="toRemove">Element to remove</param>
	/// <returns>Copy of list with elements removed</returns>
	public static List<T> RemoveAll<T>(this IList<T> list, T toRemove) {
		List<T> l = new List<T>(list.Count);
		foreach (var element in list) {
			if (!element.Equals(toRemove)) { l.Add(element); }
		}
		return l;
	}
	
	/// <summary> Converts all elements in an IList to strings</summary>
	/// <typeparam name="T">Generic Type</typeparam>
	/// <param name="list">List to convert</param>
	/// <returns>Array of string objects, generated from elements in the list</returns>
	public static string[] ToStringArray<T>(this IList<T> list) {
		string[] strings = new string[list.Count];
		for (int i = 0; i < list.Count; ++i) { strings[i] = list[i].ToString(); }
		return strings;
	}

	/// <summary>Generate a list of strings from a list of objects</summary>
	/// <typeparam name="T">Generic Type</typeparam>
	/// <param name="list">Enumerable to run through</param>
	/// <returns>A List of strings generated from elements in the list</returns>
	public static List<string> ToStringList<T>(this IEnumerable<T> list) {
		List<string> strings = new List<string>();
		foreach (var element in list) { strings.Add(element.ToString()); }
		return strings;
	}

	/// <summary> Creates a shallow clone of a list. </summary>
	/// <typeparam name="T">Generic Type</typeparam>
	/// <param name="list">List to clone</param>
	/// <returns>List containing all the same elements as the given list </returns>
	public static List<T> Clone<T>(this List<T> list) { return new List<T>(list); }
	
	/// <summary> Calls a given function for each element in an IEnumerable </summary>
	/// <typeparam name="T">Generic Type</typeparam>
	/// <param name="list">IEnumerable of stuff to loop over</param>
	/// <param name="func">Action to call on each element in list</param>
	public static void Each<T>(this IEnumerable<T> list, Action<T> func) { foreach (T t in list) { func(t); } }

	/// <summary> Calls a given function for each element in an IList, and its index </summary>
	/// <typeparam name="T">Generic Type</typeparam>
	/// <param name="list">IList of stuff to loop over</param>
	/// <param name="func">Action to call on each element in the list, paired with its index</param>
	public static void Each<T>(this IList<T> list, Action<T, int> func) {
		int i = 0;
		foreach (T t in list) { func(t, i++); }
	}

	/// <summary> Calls a given function for each pair in an IDictionary</summary>
	/// <typeparam name="K">Generic type of Key</typeparam>
	/// <typeparam name="V">Generic type of Value</typeparam>
	/// <param name="dict">IDictionary of pairs to loop over</param>
	/// <param name="func">Action to call on each key,value pair in the dictionary</param>
	public static void Each<K, V>(this IDictionary<K, V> dict, Action<K, V> func) { foreach (var pair in dict) { func(pair.Key, pair.Value); } }

	/// <summary> Map elements from source type to destination type</summary>
	/// <typeparam name="SourceType">Source Generic Type</typeparam>
	/// <typeparam name="DestType">Destination Generic Type</typeparam>
	/// <param name="data">Collection to loop over</param>
	/// <param name="mapper">Function to map elements from SourceType to DestType</param>
	/// <returns>List of elements mapped from SourceType to DestType </returns>
	public static List<DestType> Map<SourceType, DestType>(this IEnumerable<SourceType> data, Func<SourceType, DestType> mapper) {
		List<DestType> mapped = new List<DestType>();
		foreach (var d in data) { mapped.Add(mapper(d)); }
		return mapped;
	}

	/// <summary> Filter elements from an IEnumerable based on a pass/fail filter. </summary>
	/// <typeparam name="T">Generic Type</typeparam>
	/// <param name="data">IEnumerable of elements to filter</param>
	/// <param name="filter">Function to call to see if an element should be filtered or not</param>
	/// <returns>List of all elements that actually pass the filter</returns>
	public static List<T> Filter<T>(this IEnumerable<T> data, Func<T, bool> filter) {
		List<T> passed = new List<T>();
		foreach (var d in data) {
			if (filter(d)) { passed.Add(d); }
		}
		return passed;
	}

	/// <summary>Reduce elements in an IEnumerable collection with a given function </summary>
	/// <typeparam name="T">Generic Type of collection</typeparam>
	/// <typeparam name="TResult">Generic Type of expected result</typeparam>
	/// <param name="data">IEnumerable collection to loop over</param>
	/// <param name="reducer">Function to use to 'reduce' each element in the collection </param>
	/// <param name="startingValue">Value to begin with to reduce the first element with </param>
	/// <returns>Entire collection reduced into one single value</returns>
	public static TResult Reduce<T, TResult>(this IEnumerable<T> data, Func<TResult, T, TResult> reducer, TResult startingValue) {
		var val = startingValue;
		foreach (var d in data) {
			val = reducer(val, d);
		}
		return val;
	}

	#region times methods
	/// <summary> Perform a function n times</summary>
	/// <param name="n">Count of times to perform the function </param>
	/// <param name="func">Function to perform </param>
	public static void Times(this int n, Action func) { for (int i = 0; i < n; i++) { func(); } }

	/// <summary> Perform a function n times, passing in a count each time </summary>
	/// <param name="n">Count of times to perform the function </param>
	/// <param name="func">Function to perform </param>
	public static void Times(this int n, Action<int> func) { for (int i = 0; i < n; i++) { func(i); } }

	/// <summary> Perform a function n times</summary>
	/// <param name="n">Count of times to perform the function </param>
	/// <param name="func">Function to perform </param>
	public static void Times(this long n, Action func) { for (long i = 0; i < n; i++) { func(); } }

	/// <summary> Perform a function n times, passing in a count each time </summary>
	/// <param name="n">Count of times to perform the function </param>
	/// <param name="func">Function to perform </param>
	public static void Times(this long n, Action<long> func) { for (long i = 0; i < n; i++) { func(i); } }
	#endregion 

	
	
	public static string LoadTextAsset(string filename) {
		TextAsset file = Resources.Load(filename, typeof(TextAsset)) as TextAsset;
		if (file == null) {
			Debug.LogFormat("Tried to load {0}.txt/{0}.csv - File does not exist", filename);
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
		string text = Load(filename).ConvertNewlines().Replace(",\n", "\n").Replace("\n", ",");
		return text.Split(',').ToList();
		
	}

	/// <summary>
	///  Attempts to parse the provided parameters into the specified type.
	/// This is VERY strict. Exactly the right number of parameters must be passed and they must all parse properly.
	/// The only thing that cannot possibly fail is String.
	/// Returns: object reference of the result. Null if improper parameters.
	/// </summary>
	/// <param name="typeName">The type of the returned object</param>
	/// <returns>The resulting object</returns>
	public static object ParseParameterListIntoType(this string[] parameters, string typeName) {
		Type targetType = ReflectionUtils.GetTypeInUnityAssemblies(typeName);
		if (targetType != null && targetType.IsEnum && parameters.Length == 1) {
			return Enum.Parse(targetType, parameters[0], true);
		}
		switch(typeName) {
			case "Vector2": {
				Vector2 targetV2;
				PropertyInfo vector2ByName = typeof(Vector2).GetProperty(parameters[0], BindingFlags.Public | BindingFlags.Static | BindingFlags.GetProperty);
				if (vector2ByName != null) {
					if (parameters.Length != 1) { return null; }
					targetV2 = (Vector2)vector2ByName.GetValue(null, null);
				} else {
					if (parameters.Length != 2) { return null; }
					float x = 0.0f;
					try {
						x = System.Single.Parse(parameters[0]);
					} catch (System.FormatException) { return null; }
					float y = 0.0f;
					try {
						y = System.Single.Parse(parameters[1]);
					} catch (System.FormatException) { return null; }
					targetV2 = new Vector2(x, y);
				}
				return targetV2;
			}
			case "Vector3": {
				Vector3 targetV3;
				PropertyInfo vector3ByName = typeof(Vector3).GetProperty(parameters[0], BindingFlags.Public | BindingFlags.Static | BindingFlags.GetProperty);
				if (vector3ByName != null) {
					if (parameters.Length != 1) { return null; }
					targetV3 = (Vector3)vector3ByName.GetValue(null, null);
				} else {
					if (parameters.Length != 3) { return null; }
					float x = 0.0f;
					try {
						x = System.Single.Parse(parameters[0]);
					} catch (System.FormatException) { return null; }
					float y = 0.0f;
					try {
						y = System.Single.Parse(parameters[1]);
					} catch (System.FormatException) { return null; }
					float z = 0.0f;
					try {
						z = System.Single.Parse(parameters[2]);
					} catch (System.FormatException) { return null; }
					targetV3 = new Vector3(x, y, z);
				}
				return targetV3;
			}
			case "Color": {
				Color targetColor;
				PropertyInfo colorByName = typeof(Color).GetProperty(parameters[0], BindingFlags.Public | BindingFlags.Static | BindingFlags.GetProperty);
				if (colorByName != null) {
					if (parameters.Length != 1) { return null; }
					targetColor = (Color)colorByName.GetValue(null, null);
				} else {
					if (parameters.Length != 4) { return null; }
					float r = 0.0f;
					try {
						r = System.Single.Parse(parameters[0]);
					} catch (System.FormatException) { return null; }
					float g = 0.0f;
					try {
						g = System.Single.Parse(parameters[1]);
					} catch (System.FormatException) { return null; }
					float b = 0.0f;
					try {
						b = System.Single.Parse(parameters[2]);
					} catch (System.FormatException) { return null; }
					float a = 1.0f;
					try {
						a = System.Single.Parse(parameters[3]);
					} catch (System.FormatException) { return null; }
					targetColor = new Color(r, g, b, a);
				}
				return targetColor;
			}
			case "Rect": {
				if (parameters.Length != 4) { return null; }
				Rect targetRect;
				float l = 0.0f;
				try {
					l = System.Single.Parse(parameters[0]);
				} catch (System.FormatException) { return null; }
				float t = 0.0f;
				try {
					t = System.Single.Parse(parameters[1]);
				} catch (System.FormatException) { return null; }
				float w = 0.0f;
				try {
					w = System.Single.Parse(parameters[2]);
				} catch (System.FormatException) { return null; }
				float h = 1.0f;
				try {
					h = System.Single.Parse(parameters[3]);
				} catch (System.FormatException) { return null; }
				targetRect = new Rect(l, t, w, h);
				return targetRect;
			}
			case "String": {
				System.Text.StringBuilder bob = new System.Text.StringBuilder();
				foreach (string st in parameters) {
					bob.Append(st + " ");
				}
				string allparams = bob.ToString();
				return allparams.Substring(0, allparams.Length - 1);
			}
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
			case "Double": {
				if (parameters.Length != 1) { return null; }
				try {
					// Use reflection to call the proper Parse method. Because I can.
					return System.Type.GetType("System." + typeName).GetMethod("Parse", BindingFlags.Public | BindingFlags.Static, null, new System.Type[] { typeof(string) }, null).Invoke(null, new string[] { parameters[0] });
				} catch (System.Reflection.TargetInvocationException) { // Is thrown in place of the Parse method's exceptions
					return null;
				}
			}
			case "Boolean": {
				if (parameters.Length != 1) { return null; }
				if (parameters[0] == "1" || parameters[0].Equals("on", System.StringComparison.InvariantCultureIgnoreCase) || parameters[0].Equals("yes", System.StringComparison.InvariantCultureIgnoreCase)) {
					return true;
				} else if (parameters[0] == "0" || parameters[0].Equals("off", System.StringComparison.InvariantCultureIgnoreCase) || parameters[0].Equals("no", System.StringComparison.InvariantCultureIgnoreCase)) {
					return false;
				} else {
					try {
						float val = System.Single.Parse(parameters[0]);
						return val >= 0.5f;
					} catch (System.FormatException) {
						try {
							return System.Boolean.Parse(parameters[0]);
						} catch (System.FormatException) {
							return null;
						}
					}
				}
			}
			default: {
				return null;
			}
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
		foreach (T item in a) {
			if (item.Equals(search)) {
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
		string key = i.ToString();
		while (table.ContainsColor(key)) {
			list.Add(table.GetColor(key));
			
			++i;
			key = i.ToString();
		}
		
		return list;
	}
	
	
	public static Table ToTable(this List<Color> list) {
		Table t = new Table();
		
		for (int i = 0; i < list.Count; ++i) {
			t.SetColor(i.ToString(), list[i]); 
		}
		
		return t;
	}	
	
	public static Table ToTable(this Color[] array) {
		Table t = new Table();
		
		for (int i = 0; i < array.Length; ++i) {
			t.SetColor(i.ToString(), array[i]);
		}
		
		return t;
	}
	
	
	
	
}

public static class DataFiles {
	
#if XtoJSON
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
				Debug.LogFormat("Converted {0} to json successfully", file);
			} catch (Exception e) {
				Debug.LogFormat("Tried to convert {0} to json. Unsuccessful: {1}", file, e.GetType());
			}
			
			
			return values;
		}
		
		return null;
	}
#endif
}
















