using UnityEngine;
using System;
using System.Text;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;




public class Inventory : List<Item> {
	
	public Inventory() : base() { }
	public Inventory(string s) : base() { LoadString(s); }
	
	
	public void Add(Inventory other) {
		foreach (Item item in other) { Add(item); }
	}
	
	public new void Add(Item item) {
		if (Item.database == this) {
			List<Item> list = (List<Item>)this;
			list.Add(item);
			return;
		}
		
		if (item.stacks) {
			Item check = Get(item.id);
			if (check != null) {
				check.count += item.count;
			} else {
				List<Item> list = (List<Item>)this;
				list.Add(item);
			}
		} else {
			List<Item> list = (List<Item>)this;
			list.Add(item);
		}
	}
	
	
	
	public void Add(string id) { Add(id, 1); }
	public void Add(string id, int qty) {
		if (this == Item.database) { return; }
		if (ItemDatabase.Get(id) == null) {
			Debug.Log("Tried to add " + id + " to Inventory. Item did not exist");
			return;
		}
		
		
		Item item = ItemDatabase.Get(id).Clone();
		if (item.stacks) {
			Item check = Get(item.id);
			//Debug.Log(check);
			if (check != null) {
				if (check.stacks) {
					check.count += qty;
				} else {
					Debug.LogWarning("Something went wrong, there are two items with the same id that are different. One stacks, the other doesnt.");
				}
			} else {
				item.count = qty;
				Add(item);
				//Debug.Log(this.Count);
				
			}
			
		} else {
			for (int i = 0; i < qty; i++) {
				item.count = 1;
				Add(item);
			
			}
		
		}
		
	}
	
	
	public Item Get(string id) {
		foreach (Item i in this) {
			if (i.id.Equals(id)) { return i; }
		}
		return null;
	}
	
	public Item GetNamed(string name) {
		foreach (Item i in this) { 	
			if (i.name.Equals(name)) { return i; } 
		}
		return null;
	}
	
	public List<Item> stackables { get { return this.Where(item => item.stacks).ToList(); } }
	public List<Item> nonStackables { get { return this.Where(item => !item.stacks).ToList(); } }
	
	public List<Item> unlocked { get { return this.Where(item => !item.locked).ToList(); } }
	public List<Item> locked { get { return this.Where(item => item.locked).ToList(); } }
	
	
	public List<Item> SelectType(string type) { return this.Where(item => item.type == type).ToList(); }
	
	public List<Item> SelectOverRarity(float rarity) { return this.Where(item => item.rarity > rarity).ToList(); }
	public List<Item> SelectUnderRarity(float rarity) { return this.Where(item => item.rarity < rarity).ToList(); }
	
	public void Save(string key) {
		PlayerPrefs.SetString(key, ToString());
		
	}
	
	
	
	public void Load(string key) {
		Clear();
		LoadString(PlayerPrefs.GetString(key));
		
	}
	
	public void SaveCounts(string key) {
		PlayerPrefs.SetString(key, CountsString());
		
		
	}
	
	public void LoadCounts(string key) {
		string str = PlayerPrefs.GetString(key);
		string[] lines = str.Split('\n');
		
		for (int i = 0; i < lines.Length; i++) {
			if (lines[i].Length <= 2) { continue; }
			if (lines[i][0] == '#') { continue; }
			string[] content = lines[i].Split(',');
			string id = content[0];
			int num = content[1].ParseInt();
			
			
			Item item = Get(id);
			if (item != null) { item.count = num; }
			else { Add(id, num); }
			
		}
		
		
		
	}
	
	public void LoadString(string str) {
		string[] lines = str.Split('\n');
		
		for (int i = 0; i < lines.Length; i++) {
			if (lines[i].Length <= 3) { continue; }
			if (lines[i][0] == '#') { continue; }
			Add(new Item(lines[i]));
		}
		
		
	}
	
	public string CountsString() {
		StringBuilder str = new StringBuilder("");
		
		for (int i = 0; i < Count; i++) {
			str.Append(this[i].id + "," +  this[i].count);
			if (i < Count-1) { str.Append("\n"); }
		}
		
		
		return str.ToString();
	}
	
	public override string ToString() {
		StringBuilder str = new StringBuilder("");
		for (int i = 0; i < Count; i++) {
			str.Append(this[i].ToString());
			if (i < Count-1) { str.Append("\n"); }
		}
		return str.ToString();
	}
	
	
	
	
	
}

public static class ItemDatabase {
	
	
	public static Item Get(string id) { 
		//Item item = Item.databaseIDMap[id];
		Item item;
		if (Item.databaseIDMap.ContainsKey(id)) { item = Item.databaseIDMap[id]; }
		else if (Item.databaseNameMap.ContainsKey(id)) { item = Item.databaseNameMap[id]; }
		else { return null; }
		return item.Clone(); 
	}
	
	public static bool Has(string id) { 
		return Item.databaseIDMap.ContainsKey(id) || Item.databaseNameMap.ContainsKey(id);
	}
	
	public static IEnumerable<Item> SelectType(string type) { return Where(i => i.type == type); }
	public static IEnumerable<Item> Where(Func<Item, bool> func) { return Item.database.Where(func); }
	
	
}

[System.Serializable]
public class Item : IComparable<Item> {

	#region VARIABLES
	//These store all information for a single Item
	public Table stats;
	public Table properties;
	public StringMap strings;
	
	//Cached icon texture
	public Texture2D iconLoaded;
	
	
	public static Inventory database;
	public static Dictionary<string, Item> databaseIDMap;
	public static Dictionary<string, Item> databaseNameMap;
	public static List<string> DEFAULT_STRINGS { get { return _DEFAULT_STRINGS; } }
	static List<string> _DEFAULT_STRINGS;
	#endregion
	
	#region ITEM_SORTS
	public static class Sorts {
		public static int IDA(Item a, Item b) { return a.id.CompareTo(b.id); }
		public static int IDD(Item a, Item b) { return -a.id.CompareTo(b.id); }
		
		public static int NameA(Item a, Item b) { return a.name.CompareTo(b.name); }
		public static int NameD(Item a, Item b) { return -a.name.CompareTo(b.name); }
		
		public static int TypeA(Item a, Item b) { return a.type.CompareTo(b.type); }
		public static int TypeD(Item a, Item b) { return -a.type.CompareTo(b.type); }
		
		public static int RarityA(Item a, Item b) { return (int)(a.rarity - b.rarity); }
		public static int RarityD(Item a, Item b) { return (int)-(a.rarity - b.rarity); }
		
		public static int Compare(Item a, Item b, params Func<Item, Item, int>[] compares) {
			for (int i = 0; i < compares.Length; i++) {
				int check = compares[i](a, b);
				if (check != 0) { return check; }
			}
			return 0;
		}
		
		public static int NameRarityA(Item a, Item b) { return Compare(a, b, NameA, RarityA, TypeA); }
		public static int NameRarityD(Item a, Item b) { return Compare(a, b, NameD, RarityD, TypeD); }
		public static int TypeRarityA(Item a, Item b) { return Compare(a, b, TypeA, RarityA, NameA); }
		public static int TypeRarityD(Item a, Item b) { return Compare(a, b, TypeD, RarityD, NameD); }
		public static int RarityTypeA(Item a, Item b) { return Compare(a, b, RarityA, TypeA, NameA); }
		public static int RarityTypeD(Item a, Item b) { return Compare(a, b, RarityD, TypeD, NameD); }
		public static int IDTypeA(Item a, Item b) { return Compare(a, b, IDA, TypeA, RarityA); }
		public static int IDTypeD(Item a, Item b) { return Compare(a, b, IDD, TypeD, RarityD); }
		
		//Locked items Come first
		//Equipments come next
		//Items are sorted by 'type'
		//Items are then compared by rarity
		//Lastly, items are compared by name.	
		public static int Default(Item a, Item b) {
			if (a.locked == b.locked) {
				if (a.equip == b.equip) {
					if (a.type == b.type) {
						if (a.rarity == b.rarity) {
							if (a.name == b.name) {
							
								return 0;
						
							} else { return a.name.CompareTo(b.name); }
						} else { return (int)(a.rarity - b.rarity); }
					} else { return a.type.CompareTo(b.type); }
				} else { return (a.equip ? -1 : 1); }
			} else { return (a.locked ? -1 : 1); }
		}
		
	}
	#endregion
	
	static Item() {
		database = new Inventory();
		databaseIDMap = new Dictionary<string, Item>();
		databaseNameMap = new Dictionary<string, Item>();
		
		string[] defaultStrings = { "id", "name", "desc", "type", "baseName", "iconName" };
		_DEFAULT_STRINGS = defaultStrings.ToList();
		
		TextAsset file = Resources.Load("Items", typeof(TextAsset)) as TextAsset;
		if (file != null) {
			database = new Inventory(file.text);
			foreach (Item item in database) {
				databaseIDMap.Add(item.id, item);
				databaseNameMap.Add(item.name, item);
			}
		}
		
		
		
	}
	
	public void ReloadIcon() { iconLoaded = Resources.Load(iconName, typeof(Texture2D)) as Texture2D; }
	public Texture2D icon {
		get {
			if (iconLoaded != null) { return iconLoaded; }
			Texture2D i = Resources.Load(iconName, typeof(Texture2D)) as Texture2D;
			if (i != null) { iconLoaded = i; }
			return i;
		}
	}
	
	#region BASIC PROPERTIES
	public string id { get { return strings["id"]; } set { strings["id"] = value; } }
	public string name { get { return strings["name"]; } set { strings["name"] = value; } }
	public string desc { get { return strings["desc"]; } set { strings["desc"] = value; } }
	public string type { get { return strings["type"]; } set { strings["type"] = value; } }
	public string baseName { get { return strings["baseName"]; } set { strings["baseName"] = value; } }
	public string iconName { get { return strings["iconName"]; } set { strings["iconName"] = value; } }
	
	public Color color { get { return properties.GetColor("color"); } set { properties.SetColor("color", value); }  }
	public bool locked { get { return properties["locked"] == 1; } set { properties["locked"] = value ? 1 : 0; } }
	public bool stacks { get { return properties["stacks"] == 1; } set { properties["stacks"] = value ? 1 : 0; } }
	public bool equip { get { return properties["equip"] == 1; } set { properties["equip"] = value ? 1 : 0; } }
	public bool equipInRange { get { return properties["equipInRange"] == 1; } set { properties["equipInRange"] = value ? 1 : 0; } }
	public bool equipInWholeRange { get { return properties["equipInWholeRange"] == 1; } set { properties["equipInWholeRange"] = value ? 1 : 0; } }
	
	
	public int count { 
		get { 
			if (!stacks) { properties["count"] = 1; }
			//if (!properties.ContainsKey("count") || properties["count"] <= 0) { properties["count"] = 1; }
			
			return (int)properties["count"]; 
		}
		
		set { 
			if (stacks) {
				properties["count"] = value;
				if (maxStack > 0 && count > maxStack) {
					properties["count"] = maxStack;
				}
			}
			
		} 
		
	}
	
	
	public int maxStack { get { return (int)properties["maxStack"]; } set { properties["maxStack"] = value; } }
	public int equipSlot { get { return (int)properties["equipSlot"]; } set { properties["equipSlot"] = value; } }
	public int minSlot { get { return (int)properties["equipSlot"]; } set { properties["equipSlot"] = value; } }
	public int maxSlot { get { return (int)properties["maxSlot"]; } set { properties["maxSlot"] = value; } }
	
	public float value { get { return properties["value"]; } set { properties["value"] = value; } }
	public float rarity { get { return properties["rarity"]; } set { properties["rarity"] = value; } }
	public float quality { get { return properties["quality"]; } set { properties["quality"] = value; } }
	public float speed { get { return properties["speed"]; } set { properties["speed"] = value; } }
	public float mpCost { get { return properties["mpCost"]; } set { properties["mpCost"] = value; } }
	public float power { get { return properties["power"]; } set { properties["power"] = value; } }
	public float size { get { return properties["size"]; } set { properties["size"] = value; } }
	
	public float blendAmount { get { return properties["blendAmount"]; } set { properties["blendAmount"] = value.Clamp01(); } }
	//public float xxxx { get { return properties["xxxx"]; } set { properties["xxxx"] = value; } }
	#endregion
	
	#region COLOR_STUFF
	public Color rarityColor { get { return RarityColor(rarity); } }
	public static Color RarityColor(float v) { return RarityColor(v, 10, 0); }
	public static Color RarityColor(float v, float s) { return RarityColor(v, s, 0); }
	public static Color RarityColor(float v, float s, float o) {
		if (v+o < 0) { return new Color(0.4f, 0.4f, 0.4f); }
		else if (v+o < s * 01) { return new Color(0.7f, 0.7f, 0.7f); }
		else if (v+o < s * 02) { return new Color(1.0f, 1.0f, 1.0f); }
		else if (v+o < s * 03) { return new Color(0.5f, 1.0f, 0.5f); }
		else if (v+o < s * 04) { return new Color(0.0f, 1.0f, 0.0f); }
		else if (v+o < s * 05) { return new Color(0.0f, 1.0f, 0.5f); }
		else if (v+o < s * 06) { return new Color(0.0f, 1.0f, 1.0f); }
		else if (v+o < s * 07) { return new Color(0.0f, 0.5f, 1.0f); }
		else if (v+o < s * 08) { return new Color(0.0f, 0.0f, 1.0f); }
		else if (v+o < s * 09) { return new Color(0.5f, 0.0f, 1.0f); }
		else if (v+o < s * 10) { return new Color(1.0f, 0.0f, 1.0f); }
		else if (v+o < s * 11) { return new Color(1.0f, 0.0f, 0.5f); }
		else if (v+o < s * 12) { return new Color(1.0f, 0.0f, 0.0f); }
		else if (v+o < s * 13) { return new Color(1.0f, 0.5f, 0.0f); }
		else if (v+o < s * 14) { return new Color(1.0f, 1.0f, 0.0f); }
		return new Color(1.0f, 1.0f, .75f);
	}
	#endregion
	
	#region CONSTRUCTORS_AND_LOADING
	public Item() {
		stats = new Table();
		properties = new Table();
		strings = new StringMap();
		
		count = 1;
		
		id = "";
		name = "Crystalized Error";
		desc = "Somewhere, something went wrong";
		type = "Loot";
		baseName = "";
		iconName = "";
		
		color = Color.white;
	}
	
	public Item(string str) {
		stats = new Table();
		properties = new Table();
		strings = new StringMap();
		color = Color.white;
		
		count = 1;
		
		id = "";
		name = "Crystalized Error";
		desc = "Somewhere, something went wrong";
		type = "Loot";
		baseName = "";
		iconName = "";
		color = Color.white;
		
		LoadFromString(str);
		
		
	}
	
	public Item(Item other) {
		stats = other.stats.Clone();
		properties = other.properties.Clone();
		strings = other.strings.Clone();
	}
	
	//Creates a deep clone of an item.
	public Item Clone() { return new Item(this); }
	
	
	public static Item FromString(string s) { return FromString(s, '|'); }
	public static Item FromString(string s, char delim) {
		Item it = new Item();
		it.LoadFromString(s, delim);
		return it;
	}
	
	public void LoadFromString(string s) { LoadFromString(s, '|'); }
	public void LoadFromString(string s, char delim) {
		string[] content = s.Split(delim);
		if (content.Length < 3) {
			Debug.LogWarning("Tried to load a malformed string as an item.\nDelim: " + delim + "\n" + s);
			return;
		}
		
		strings = 		content[0].ParseStringMap('`');
		stats = 		content[1].ParseTable(',');
		properties =	content[2].ParseTable(',');
		
		//v1.0.1
		if (id == "") { id = name; }
		
	}
	#endregion
	
	
	#region ICOMPARABLE
	public static int Compare(Item a, Item b) {
		if (a.type == b.type) {
			if (a.rarity == b.rarity) { 
				return a.id.CompareTo(b.id);
			} else { return (int)(a.rarity - b.rarity); }
		} else { return a.type.CompareTo(b.type); }
	}
	
	public int CompareTo(Item other) { return Compare(this, other); }
	#endregion
	
	
	
	
	
	
	
	//Attempts to use the item on a target.
	//Returns true if successful, and false if unsucessful.
	public bool Use<T>(T target) { return UseOn(target); }
	public bool UseOn<T>(T target) {
		Type t = target.GetType();
		
		string methodName = "Use" + type;
		Type[] signature = { typeof(Item) };
		
		MethodInfo method = t.GetMethod(methodName, BindingFlags.Instance | BindingFlags.Public, null, signature, null);
		
		
		//Object[] parms = new Object[1];
		System.Object[] parms = { this };
		
		if (method != null) {
			method.Invoke(target, parms);
			return true;
		}
		
		return false;
	}
	
	#region SAVE_AND_LOAD
	public override string ToString() { return ToString('|'); }
	public string ToString(char delim) {
		StringBuilder str = new StringBuilder("");
		str.Append(strings.ToString('`') + delim);
		str.Append(stats.ToLine(',') + delim);
		str.Append(properties.ToLine(',') + delim);
		return str.ToString();
	}
	
	public void Save(string key) { PlayerPrefs.SetString(key, ToString()); }
	
	public void Load(string key) {
		if (PlayerPrefs.HasKey(key)) {
			LoadFromString(PlayerPrefs.GetString(key));
		}
	}
	#endregion
	
	
}
