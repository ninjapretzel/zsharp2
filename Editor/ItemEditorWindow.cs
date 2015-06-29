#if UNITY_EDITOR && !UNITY_WEBPLAYER
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Linq;
using System.Collections;
using System.Collections.Generic;


#if LEGACY_ITEMS
public class ItemEditorWindow : ZEditorWindow {
	
	public static string path { get { return Application.dataPath + "/Data/Resources/"; } } 
	public static string target { get { return path + "Items.csv"; } }
	
	[System.NonSerialized] private Table equipSlots;
	
	[System.NonSerialized] private List<Item> items;
	[System.NonSerialized] private List<Item> visibleList;
	
	[System.NonSerialized] private List<OptionEntry> stats;
	[System.NonSerialized] private StringMap strings;
	[System.NonSerialized] private int numOptions;
	
	[System.NonSerialized] private int removeAt = -1;
	[System.NonSerialized] private int moveUp = -1;
	[System.NonSerialized] private int moveDown = -1;
	
	[System.NonSerialized] private Vector4 selectScroll;
	[System.NonSerialized] private Vector2 editScroll;
	
	[System.NonSerialized] private Item editing;
	
	[System.NonSerialized] private bool listChanged = false;
	[System.NonSerialized] private bool justRecompiledOrCreated = true;
	

	
	
	private int selection;
	private Vector2 listScroll = Vector2.zero;
	private bool searchMode = NAME;
	private string searchString = "";
	
	const bool NAME = true;
	const bool TYPE = false;
	
	bool forceRepaint = false;
	
	
	public override float fieldWidth { get { return .33f * position.width; } }
	
	public Color selectedColor { get { return new Color(.5f, .5f, 1); } }
	
	bool wasPlaying = false;
	
	//private int numStringOptions;
	
	[MenuItem ("Window/Item Database")]
	static void ShowWindow() {
		EditorWindow.GetWindow(typeof(ItemEditorWindow));
		
		
		
	}
	
	//
	public ItemEditorWindow() : base() {
		Init();
	}
	
	

	
	void Init() {
		items = new List<Item>();
		stats = new List<OptionEntry>();
		numOptions = 0;
		removeAt = -1;
		moveUp = -1;
		moveDown = -1;
		
		editing = new Item();
		listChanged = false;
		changed = false;
		selection = 0;
		
		items.Add(editing);
		
		TextAsset equipCSV = Resources.Load<TextAsset>("EquipSlots");
		if (equipCSV != null) { 
			equipSlots = new Table(equipCSV);
		} else {
			equipSlots = new Table();
		}
		
		LoadDatabase();
		
		if (items.Count == 0) { items.Add(new Item()); }
		
		LoadSelection();
		
		justRecompiledOrCreated = true;
		
		//autoRepaintOnSceneChange = true;
	}
	
	void Update() {
		//if (EditorApplication.isPlaying && frame++ % 10 == 0) { Repaint(); }
		
		if (forceRepaint) {
			forceRepaint = false;
			Repaint();
		}
		
		if (justRecompiledOrCreated) {
			UpdateVisibleList();
			LoadSelection();
			Repaint();
			justRecompiledOrCreated = false;
		}
		//*/
		
		if (wasPlaying && !isPlaying) { Repaint(); }
		if (!wasPlaying && isPlaying) { Repaint(); }
		wasPlaying = isPlaying;
		
		selection = (int)selection.Clamp(0, items.Count-1);
		
	}
	
	
	void OnGUI() {
		if (EditorApplication.isPlaying) {
			PlayModeWarning();
			return;
		}
		
		if (KeyPress(KeyCode.F1)) {
			Unfocus();
			ApplySelection();
			WriteDatabase();
			forceRepaint = true;
		}
		
		
		checkChanges = false;
		BeginHorizontal("box"); {
			ListScrollArea();
			
			EditSelection();
			
			
		} EndHorizontal();
		
		
		
	}
	
	
	void Sort(Comparison<Item> sortFunc) {
		Item item = items[selection];
		listChanged = true;
		
		items.Sort(sortFunc);
		
		selection = items.IndexOf(_item => _item.name == item.name);
		
	}
	
	void ListScrollArea() {
	
		if (visibleList == null) { 
			UpdateVisibleList(); 
		}
		
		
		if (visibleList != items) {
			GUI.color = Color.yellow;
		}
		
		BeginVertical("box", Width(350)); {
			
			GUI.color = Color.white;
			BeginVertical("box", Width(350)); {
				
				FixedLabel("Sort:");
				
				BeginHorizontal(); {
					
					FixedLabel("Name");
					if (FixedButton("/\\")) { Sort(Item.Sorts.NameRarityA); }
					if (FixedButton("\\/")) { Sort(Item.Sorts.NameRarityD); }
					
					FixedLabel("Type");
					if (FixedButton("/\\")) { Sort(Item.Sorts.TypeRarityA); }
					if (FixedButton("\\/")) { Sort(Item.Sorts.TypeRarityD); }
					
					FixedLabel("Rarity");
					if (FixedButton("/\\")) { Sort(Item.Sorts.RarityTypeA); }
					if (FixedButton("\\/")) { Sort(Item.Sorts.RarityTypeD); }
					
					FixedLabel("ID");
					if (FixedButton("/\\")) { Sort(Item.Sorts.IDTypeA); }
					if (FixedButton("\\/")) { Sort(Item.Sorts.IDTypeD); }
					
				} EndHorizontal();
				
			} EndVertical();
			
			
			BeginHorizontal("box", Width(350)); {
				FixedLabel("Search:");
				bool lastSearchMode = searchMode;
				searchMode = ToggleButton(searchMode, "Name", "Type", Width(65));
				
				if (lastSearchMode != searchMode && searchString != "") {
					UpdateVisibleList();
				}
				
				string lastSearchString = searchString;
				searchString = TextField(searchString);
				
				if (KeyPress(KeyCode.Escape)) {
					searchString = "";
					Unfocus();
					Event.current.Use();
				}
				
				if (lastSearchString != searchString) {
					
					UpdateVisibleList();
					
				}
				
			} EndHorizontal();
			
			
			
			listScroll = BeginScrollView(listScroll, false, true, Width(350), Height(height - 160) ); {
				
				int lastSelection = selection;
				
				if (visibleList != items) {
					GUI.color = Color.yellow;
				}
				
				BeginVertical("box"); {
					
					if (visibleList.Count == 0) {
						Label(items.Count + " Items searched.");
						Label("No Items Found!");
						Label("Change your search string or mode");
						
					} else {
						for (int i = 0; i < visibleList.Count; i++) {
							Item item = visibleList[i];
							int index = 0;
							index = items.IndexOf(item);
							
							/* I forget why I had this...
							if (searchString == "") {
								index = items.IndexOf(item);
							} else {
								//A stupid way to search for items...
								//But there was a reason I had it this way...
								index = items.IndexOf(_item => _item.name == item.name);
							}
							//*/
							
							if (items[selection] == item) {
								GUI.color = selectedColor;
							} else {
								GUI.color = Color.white;
							}
							if (i % 2 == 1) { GUI.color = GUI.color.Blend(Color.black, .2f); }
							Color backColor = GUI.color;
							
							Rect area = BeginVertical("button"); {
								
								BeginHorizontal("box"); {
									Box(""+index, Width(35));
									Box("[" + item.name.MinSubstring(10, '-') + "]", Width(100));
									
									FlexibleSpace();
									Box("[" + item.type + "]", Width(100));
									FlexibleSpace();
									
									
									GUI.color = item.rarityColor;
									Box("" + item.rarity, Width(40));
									
									GUI.color = Color.white;
									
								} EndHorizontal();
								
								BeginHorizontal(); {
									if (FixedButton("-")) { listChanged = true; items.RemoveAt(index); }
									if (FixedButton("D")) { listChanged = true; items.Insert(index, items[index].Clone()); }
									
									if (FixedButton("\\/")) { listChanged = true; items.Swap(index, index+1); }
									if (FixedButton("/\\")) { listChanged = true; items.Swap(index, index-1); }
									
									GUI.color = backColor;
									
									Box("ID: [" + item.id.MinSubstring(25, '-') + "]");
								} EndHorizontal();
								
								
								
								if (BlankButton(area)) { 
									selection = index; 
									LoadSelection();
								}
								
							} EndHorizontal();
							
						}
						
						
					}
					
					GUI.color = Color.white;
					if (Button("+")) {
						items.Add(new Item());
						listChanged = true;
					}
					
				} EndVertical();
				
				if (selection != lastSelection) {
					Unfocus();
					LoadSelection();
				}
				
				lastSelection = selection;
				
				//Draw controls at the bottom
				
				
				
			} EndScrollView();
			
			
			GUI.color = Color.white;
			if (Button("Revert")) {
				Init();
				Unfocus();
			}
			
			SetChangedColor();
			if (Button("Apply and Save")) {
				ApplySelection();
				WriteDatabase();
				Unfocus();
			}
			
			SetChangedColor(listChanged);
			if (Button("Save")) {
				WriteDatabase();
				Unfocus();
			}
			
			
			
			GUI.color = Color.white;
			
		} EndVertical();
		
	}
	
	
	
	
	void EditSelection() {
		checkChanges = true;
		BeginVertical("box"); {
	
			editScroll = BeginScrollView(editScroll); {
			
				BeginVertical("box"); {
					
					BasicSettingsBox();
						
					PropertiesBox();
					
					
					
					StatsBox();
					
				} EndVertical();
				
			} EndScrollView();
			
			Space(10);
			SetChangedColor(listChanged);
			if (Button("Apply and Save")) {
				ApplySelection();
				WriteDatabase();
				Unfocus();
			}
			
			
			SetChangedColor();
			if (Button("Apply Item")) {
				listChanged = changed || listChanged;
				ApplySelection();
				Unfocus();
			}
			
			
			GUI.color = Color.white;
			
		} EndVertical();
	}
	
	
	void BasicSettingsBox() {
		Color lastColor = GUI.color;
		
		BeginVertical("box"); {
		
			Label("Basic Settings");
			strings["id"] = TextField("ID", strings["id"], .5f);
			strings["name"] = TextField("Name", strings["name"], .5f);
			BeginHorizontal(); {
				Space(150);
				if (Button("\\/ Set \\/", Width(150))) {
					Unfocus();
					if (strings["baseName"] != strings["name"]) { changed = true; }
					strings["baseName"] = strings["name"];
				}
			} EndHorizontal();
			
			strings["baseName"] = TextField("Base Name", strings["baseName"], .5f);
			strings["type"] = TextField("Type", strings["type"], .5f);
			strings["desc"] = TextArea("Description", strings["desc"]);
			
			BeginHorizontal("box", ExpandWidth(false)); {
				Label("Icon", Width(50));
				BeginVertical(Width(200)); {
					//string lastIconName = strings["iconName"];
					strings["iconName"] = TextField("Icon Name", strings["iconName"], .3f);
					//if (lastIconName != strings["iconName"]) { editing.ReloadIcon(); }
					
					BeginHorizontal(); {
						if (Button("Use Name")) {
							Unfocus();
							
							if (strings["name"] != strings["iconName"]) { changed = true; }
							strings["iconName"] = strings["name"] ;
							//editing.ReloadIcon();
						}
						FlexibleSpace();
					} EndHorizontal();
					
					editing.color = ColorField("Color", editing.color);
					
					editing.blendAmount = FloatField("Blend Amount", editing.blendAmount, .3f); 
				} EndVertical();
				
				
				
				Texture2D icon = Resources.Load<Texture2D>(strings["iconName"]);
				GUI.color = editing.color;
				if (icon != null) {
					GUIStyle iconStyle = blankSkin.label.Aligned(TextAnchor.MiddleCenter);
					iconStyle.fixedHeight = 64;
					iconStyle.fixedWidth = 64;
					
					GUILayout.Label(icon, iconStyle, ExpandWidth(false));
				} else {
					Label("Icon\nNot\nFound", Width(64));
				}
				
			} EndHorizontal();
			
			GUI.color = lastColor;
		
		} EndVertical();
		
	}
	
	
	void PropertiesBox() {
		Color lastColor = GUI.color;
		
		BeginVertical("box", ExpandWidth(false)); {
			FixedLabel("Properties");
			
			//Stack Settings
			BeginHorizontal("box"); {
				BeginHorizontal("box", ExpandWidth(false)); {
					editing.stacks = ToggleField(editing.stacks, "Stacks");
					
				} EndHorizontal();
				editing.maxStack = IntField("Max Stack", editing.maxStack);
				
			} EndHorizontal();
			
			//Equipment Settings
			BeginHorizontal("box"); {
				BeginHorizontal("box"); { 
					
					editing.equip = ToggleField(editing.equip, "Equippable");
					
				} EndHorizontal();
				
				
				BeginHorizontal("box"); {
					editing.equipInRange = ToggleField(editing.equipInRange, "Equips To Range");
				} EndHorizontal();
				
				if (editing.equipInRange) {
					BeginHorizontal("box"); {
						editing.equipInWholeRange = ToggleField(editing.equipInWholeRange, "Exclusive");
						FixedLabel("Min Slot: ");
						editing.minSlot = EditorGUILayout.IntField(editing.minSlot, ExpandWidth(false));
						FixedLabel("Max Slot: ");
						editing.maxSlot = EditorGUILayout.IntField(editing.maxSlot, ExpandWidth(false));
					} EndHorizontal();
				} else {
					editing.equipSlot = IntField("Equip Slot", editing.equipSlot, .5f);
					
				}
				
				
				
			} EndHorizontal();
			
			if (equipSlots != null) {
				BeginHorizontal(ExpandWidth(false)); {
					Space(250);
					FixedLabel("Slot: ");
					Space(5);
					
					FixedLabel(equipSlots.GetKey(editing.minSlot));
					if (editing.equipInRange) {
						FixedLabel(" - " + equipSlots.GetKey(editing.maxSlot));
					}
				} EndHorizontal();
			}
			
			
			
			//Space(fieldWidth*.3f);
			
			
			
			BeginHorizontal(); {
				editing.value = FloatField("Value", editing.value, .3f);
				GUI.color = editing.rarityColor;
				editing.rarity = FloatField("Rarity", editing.rarity, .3f);
				GUI.color = lastColor;
				editing.quality = FloatField("Quality", editing.quality, .3f);
			} EndHorizontal();
			
			
		} EndVertical();
		
	}
	
	void StatsBox() {
		BeginVertical("box"); {
			Label("Item Stats Options");
			numOptions = IntField("Number", numOptions);
			
			OptionsButtons();
			numOptions = (int)numOptions.Clamp(0, 100);
			
			removeAt = -1;
			moveDown = -1;
			moveUp = -1;
			
			while (numOptions > stats.Count) { stats.Add(new OptionEntry("blank", 0)); }
			while (numOptions < stats.Count) { stats.RemoveAt(stats.Count-1); }
			for (int i = 0; i < stats.Count; i++) { DrawOption(i); }
			
			if (removeAt >= 0) { 
				changed = true; 
				stats.RemoveAt(removeAt); 
			}
			if (moveDown >= 0 && moveDown < stats.Count-1) { 
				changed = true; 
				stats.Swap(moveDown, moveDown+1);
			}
			if (moveUp >= 1) { 
				changed = true; 
				stats.Swap(moveUp, moveUp-1); 
			}
			
			numOptions = stats.Count;
			
			OptionsButtons();
			
		} EndVertical();
		
		
		
		BeginVertical("box"); {
			Label("Item String Options");
			
			strings = StringMapField(strings, Item.DEFAULT_STRINGS);
			
			
		} EndVertical();
		
		
	}
	
	
	
	
	
	
	
	void WriteDatabase() {
		string str = "";
		
		for (int i = 0; i < items.Count; i++) {
			str += items[i].ToString();
			if (i < items.Count-1) { str += "\n"; }
		}
		
		StreamWriter sr = File.CreateText(target);
		sr.WriteLine(str);
		sr.Close();
		
		listChanged = false;
		AssetDatabase.Refresh();
		
	}
	
	void LoadDatabase() {
		if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
		if (File.Exists(target)) {
			items = (new Inventory(File.ReadAllText(target))) as List<Item>;
			
		} else {
			WriteDatabase();
		}
		listChanged = false;
		
	}
	
	void ApplySelection() {
		//int visibleIndex = visibleList.IndexOf(editing);
		
		editing.stats = stats.ToTable();
		editing.strings = strings.Clone();
		
		items[selection] = editing.Clone();
		
		//visibleList[visibleIndex] = items[selection];
		
		editing = items[selection].Clone();
		changed = false;
		
		UpdateVisibleList();
	}
	
	void LoadSelection() {
		editing = items[selection].Clone();
		stats = editing.stats.ToListOfOptions();
		strings = editing.strings.Clone();
		
		numOptions = stats.Count;
		//numStringOptions = strings.Count;
		
		changed = false;
	}
	
	
	void OptionsButtons() {
		BeginHorizontal();
		Space(20);
		if (Button("+", Width(20))) { 
			numOptions++; 
			changed = changed || checkChanges;
		}
		EndHorizontal();
	}
	
	void UpdateVisibleList() {
		if (searchString == "") { 
			visibleList = items; 
			
		} else if (searchMode == NAME) {
			visibleList = items.Where(i => i.name.Contains(searchString)).ToList();
		
		} else {
			visibleList = items.Where(i => i.type.Contains(searchString)).ToList();
		
		}
	}
	
	
	void PlayModeWarning() {
		GUI.color = Color.red;
			
		BeginHorizontal("box"); {
			
			GUI.color = Color.gray;
			BeginVertical("box", Width(350)); {
				Label("STOP THE GAME BEFORE EDITING");
				FlexibleSpace();
			} EndVertical();
			
			BeginVertical("box"); {
				Label("STOP THE GAME BEFORE EDITING");
				FlexibleSpace();
			} EndVertical();
			
		} EndHorizontal();
	}
	
	void DrawOption(int i) {
		OptionEntry o = stats[i];
		BeginHorizontal("box");
			OptionEntry temp = new OptionEntry(o);
			
			if (Button("-", Width(20))) { removeAt = i; }
			if (Button("/\\", Width(20))) { moveUp = i; }
			if (Button("\\/", Width(20))) { moveDown = i; }
			
			
			o.name = TextField(o.name, Width(200));
			o.value = EditorGUILayout.FloatField(o.value, Width(200));
			
			changed = changed || (!o.Equals(temp));
		EndHorizontal();
	}
	
	[System.Serializable]
	public class OptionEntry {
		public string name;
		public float value;
		
		public OptionEntry(string s, float f) { name = s; value = f; }
		public OptionEntry(OptionEntry o) { name = o.name; value = o.value; }
		
		public new bool Equals(System.Object other) {
			if (other == null) { return false; } 
			if (other.GetType() != GetType()) { return false; }
			OptionEntry o = other as OptionEntry;
			return o.name == name && o.value == value;
		}
		
	}
	
	
}






public static class ItemEditorWindowUtils {
	public static Table ToTable(this List<ItemEditorWindow.OptionEntry> list) {
		Table t = new Table();
		foreach (ItemEditorWindow.OptionEntry o in list) { t[o.name] = o.value; }
		return t;
	}
	
	public static List<ItemEditorWindow.OptionEntry> ToListOfOptions(this Table t) {
		List<ItemEditorWindow.OptionEntry> list = new List<ItemEditorWindow.OptionEntry>();
		foreach (string s in t.Keys) { list.Add(new ItemEditorWindow.OptionEntry(s, t[s])); }
		return list;
	}
}

#endif


#endif









