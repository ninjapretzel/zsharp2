#if UNITY_EDITOR && !UNITY_WEBPLAYER
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class IconEditorWindow : ZEditorWindow {
	[MenuItem("Window/Define Icons")]
	public static void ShowWindow() {
		EditorWindow.GetWindow(typeof(IconEditorWindow));
	}

	class SheetInfo {
		public string sheet;
		public int size = 32;

		const int WIDTH = 160;
		public SheetInfo() {}
		public SheetInfo(string line) { 
			sheet = line.UpToFirst(','); 
			size = line.FromLast(',').ParseInt();
		}
		public SheetInfo Clone() { return new SheetInfo(ToString()); }
		public void Draw() {
			BeginHorizontal(); {
				Label("Sheet:", FixedWidth(WIDTH));
				Space(10);
				Label("Icon Size:", FixedWidth(WIDTH/2));
			} EndHorizontal();

			BeginHorizontal(); {
				sheet = TextField(sheet, FixedWidth(WIDTH));
				Space(10);
				size = IntField(size, FixedWidth(WIDTH/2));
			} EndHorizontal();
		}
		public override string ToString() { return sheet + "," + size; }
	}

	class IconInfo { 
		public string iconName;
		public string sheet;
		public int x, y;

		public const int WIDTH = 120;

		public IconInfo() {}
		public IconInfo(string line) {
			string[] splits = line.Split(',');
			iconName = splits[0];
			sheet = splits[1];
			x = splits[2].ParseInt();
			y = splits[3].ParseInt();
		}
		public IconInfo Clone() { return new IconInfo(ToString()); }
		public void Draw() {
			BeginHorizontal(); {
				Label("IconName:", FixedWidth(WIDTH));
				Space(10);
				Label("Sheet:", FixedWidth(WIDTH));
			} EndHorizontal();
			BeginHorizontal(); {
				iconName = TextField(iconName, FixedWidth(WIDTH));
				Space(10);
				sheet = TextField(sheet, FixedWidth(WIDTH));
			} EndHorizontal();

			BeginHorizontal(); {
				FixedLabel("Pos (x,y): (");
				x = IntField(x, FixedWidth(WIDTH/2));
				y = IntField(y, FixedWidth(WIDTH/2));
				FixedLabel(")");
			} EndHorizontal();
		}
		public override string ToString() { return iconName + "," + sheet + "," + x + "," + y; }
	}

	class RangeInfo : IconInfo {
		public int startNum;
		public int cnt;

		public RangeInfo() {}
		public RangeInfo(string line) {
			string[] splits = line.Split(',');
			iconName = splits[0];
			sheet = splits[1];
			x = splits[2].ParseInt();
			y = splits[3].ParseInt();
			startNum = splits[4].ParseInt();
			cnt = splits[5].ParseInt();
		}
		public new RangeInfo Clone() { return new RangeInfo(ToString()); }
		public new void Draw() {
			BeginHorizontal(); {
				Label("IconName:", FixedWidth(WIDTH));
				Space(10);
				Label("Sheet:", FixedWidth(WIDTH));
			} EndHorizontal();
			BeginHorizontal(); {
				iconName = TextField(iconName, FixedWidth(WIDTH));
				Space(10);
				sheet = TextField(sheet, FixedWidth(WIDTH));
			} EndHorizontal();

			BeginHorizontal(); {
				FixedLabel("Pos (x,y): (");
				x = IntField(x, FixedWidth(WIDTH/2));
				y = IntField(y, FixedWidth(WIDTH/2));
				FixedLabel(")");
			} EndHorizontal();

			BeginHorizontal(); {
				Label("Start: ", FixedWidth(WIDTH/2));
				startNum = IntField(startNum, FixedWidth(WIDTH/2));
				Label("Count: ", FixedWidth(WIDTH/2));
				cnt = IntField(cnt, FixedWidth(WIDTH/2));
			} EndHorizontal(); 

			
		}
		public override string ToString() { return iconName + "," + sheet + "," + x + "," + y + "," + startNum + "," + cnt; }
	}

	public static string path { get { return Application.dataPath + "/Data/Resources/"; } }
	public static string filePath { get { return path + "IconInfo.csv"; } }

	private List<SheetInfo> sheets;
	private List<IconInfo> icons;
	private List<RangeInfo> ranges;

	public bool sheetsExpanded = true;
	public bool iconsExpanded = true;
	public bool rangesExpanded = true;

	Vector2 leftScroll;
	Vector2 rightScroll;
	Vector2 pan;
	object selected;

	[NonSerialized] Color[] colors = new Color[] {
		new Color(1, 1, 1),
		new Color(1, .77f, .77f),
		new Color(.77f, 1, .77f),
		new Color(.77f, .77f, 1),
		new Color(1, 1, .77f),
		new Color(1, .77f, 1),
		new Color(.77f, 1, 1),
	};


	static Texture2D outline { get { return Resources.Load<Texture2D>("outline"); } }
	static Texture2D thickOutline { get { return Resources.Load<Texture2D>("thickOutline"); } }

	public IconEditorWindow() {
		if (!Directory.Exists(path)) { 
			Directory.CreateDirectory(path);
		}
		
		sheets = new List<SheetInfo>();
		icons = new List<IconInfo>();
		ranges = new List<RangeInfo>();

		if (File.Exists(filePath)) { Load(filePath); }
		pan = Vector2.zero;
		float _sat = .5f;
		float _val = .8f;

		colors = new Color[] {
			new Color(.0f, _sat, _val).HSVtoRGB(),
			new Color(.1f, _sat, _val).HSVtoRGB(),
			new Color(.2f, _sat, _val).HSVtoRGB(),
			new Color(.3f, _sat, _val).HSVtoRGB(),
			new Color(.4f, _sat, _val).HSVtoRGB(),
			new Color(.5f, _sat, _val).HSVtoRGB(),
			new Color(.6f, _sat, _val).HSVtoRGB(),
			new Color(.7f, _sat, _val).HSVtoRGB(),
			new Color(.8f, _sat, _val).HSVtoRGB(),
			new Color(.9f, _sat, _val).HSVtoRGB(),
		};

	}
	

	void OnGUI() {
		DrawSelected();
		GUI.color = new Color(.2f, .2f, .2f, 1);

		GUI.Box(new Rect(0, 0, 305, height), "");
		GUI.color = Color.white;
		BeginHorizontal(); {
			BeginVertical("box", Width(300)); {
				if (Button("Save")) { Save(filePath); }
				leftScroll = BeginScrollView(leftScroll, false, true, Height(height - 50)); {
				
					DrawSheets();
					Space(15);
					DrawIcons();
					Space(15); 
					DrawRanges();
				} EndScrollView();

			} EndVertical();

			
		} EndHorizontal();
	}

	void DrawSelected() {
		float remainingWidth = width-305;
		Rect area = new Rect(305, 0, remainingWidth, height);
		//Rect insideArea = area;
		GUI.color = new Color(.2f, .2f, .2f, 1);
		GUI.Box(area, "");
		Event e = Event.current;
		
		Vector2 pos = area.MiddleCenter();
		Rect a = area;
		

			

		

		if (selected != null) {
			if (selected is IconInfo) {
				IconInfo info = selected as IconInfo;
				SheetInfo sinfo; 
				try {
					sinfo = sheets.Where(x => x.sheet == info.sheet).First();
				}catch{
					GUI.Box(area, "Could not find SheetInfo " + info.sheet);
					return;
				}
				GUI.color = Color.white;
				Texture2D sheet = Resources.Load<Texture2D>(info.sheet);
				if (sheet == null) {
					GUI.Box(area, "Could not find texture for SheetInfo " + info.sheet);
					return;
				}

				float w = sheet.width;
				float h = sheet.height;
				a = new Rect(pos.x, pos.y, w, h).Move(pan - Vector2.one * .5f);

				GUI.DrawTexture(a, sheet);

				Rect corner = a;
				corner.width = sinfo.size;
				corner.height = sinfo.size;

				Rect b = corner.Move(info.x, info.y);
				GUI.color = Color.green;
				GUI.DrawTexture(b, thickOutline);

				float xcnt = sheet.width / sinfo.size;
				float ycnt = sheet.height / sinfo.size;

				for (int y = 0; y < ycnt; y++) {
					for (int x = 0; x < xcnt; x++) {
						Rect brush = corner.Move(x, y);
						if (e.type == EventType.MouseDown 
							&& e.button == 0 
							&& brush.Contains(e.mousePosition)
							&& area.Contains(e.mousePosition)) {

							info.x = x;
							info.y = y;
							Repaint();
						}

					}
				}


				if (selected is RangeInfo) {
					Color borderColor = Color.green.Alpha(.33f);
					RangeInfo range = selected as RangeInfo;
					int x = range.x;
					int y = range.y;
					//GUI.skin.label.normal.textColor = Color.black;
					GUIStyle style = GUI.skin.label.Clone();
					style.normal.textColor = Color.white;
					//Debug.Log("IsRange " + range.cnt);
					for (int i = 0; i < range.cnt; i++) {
						if (x >= xcnt) { 
							x = 0;
							y++;
							if (y >= ycnt) { Debug.Log("Done " + x + "," + y); break; }
						}
						Rect brush = corner.Move(x, y);
						GUI.color = borderColor;
						GUI.DrawTexture(brush, outline);
						GUI.color = Color.green;
						GUI.Label(brush, ""+(range.startNum+i), style);

						x++;
					}

				}

			}




			


		}

		if (area.Contains(e.mousePosition)) {

			if (e.type == EventType.MouseDrag && e.button == 1) {
				pan += e.delta*.001f;
				Repaint();
			}

			Vector2 maxOffset = Vector2.one;
			if (pan.x > maxOffset.x) { pan.x = maxOffset.x; }
			if (pan.x < -maxOffset.x) { pan.x = -maxOffset.x; }
			if (pan.y > maxOffset.y) { pan.y = maxOffset.y; }
			if (pan.y < -maxOffset.y) { pan.y = -maxOffset.y; }


		}
	}

	void DrawSheets() {
		
		BeginVertical("box"); {
			sheetsExpanded = ExpandCollapse(sheetsExpanded, "Sheets");
			if (sheetsExpanded) {
				for (int i = 0; i < sheets.Count; i++) {
					var info = sheets[i];
					AlternateColor(i, colors);
					BeginVertical("box"); {
						GUI.color = Color.white;
						BeginHorizontal(); {
							FixedLabel("#" + i);
							if (FixedButton("D")) { sheets.Insert(i, info.Clone()); }
							if (FixedButton("-")) { sheets.RemoveAt(i--); }

						} EndHorizontal();
						info.Draw();
					} EndVertical();
				}

				if (Button("+")) { sheets.Add(new SheetInfo()); }
			}

		} EndVertical(); 
		
	}

	void DrawIcons() {

		BeginVertical("box"); {
			iconsExpanded = ExpandCollapse(iconsExpanded, "Icons");
			if (iconsExpanded) {
				for (int i = 0; i < icons.Count; i++) {
					var info = icons[i];
					AlternateColor(i, colors);
					if (selected == info) { GUI.color = Color.Lerp(GUI.color, Color.white, .8f); }

					BeginVertical("box"); {
						GUI.color = Color.white;
						BeginHorizontal(); {
							FixedLabel("#" + i);
							if (FixedButton("D")) { icons.Insert(i, info.Clone()); }
							if (FixedButton("-")) { icons.RemoveAt(i--); }

						} EndHorizontal();

						info.Draw();
					} EndVertical();

					Rect area = GUILayoutUtility.GetLastRect();
					GUISkin skin = GUI.skin;
					GUI.skin = GUI.blankSkin;
					if (GUI.Button(area, "")) {
						selected = info;
					}
					GUI.skin = skin;

				}
				if (Button("+")) { icons.Add(new IconInfo()); }
			}

		} EndVertical(); 
		
	}
	

	void DrawRanges() {
		BeginVertical("box"); {
			rangesExpanded = ExpandCollapse(rangesExpanded, "Ranges");
			if (rangesExpanded) {
				for (int i = 0; i < ranges.Count; i++) {
					var info = ranges[i];
					AlternateColor(i, colors);
					if (selected == info) { GUI.color = Color.Lerp(GUI.color, Color.white, .8f); }

					BeginVertical("box"); {
						GUI.color = Color.white;
						BeginHorizontal(); {
							FixedLabel("#" + i);
							if (FixedButton("D")) { ranges.Insert(i, info.Clone()); }
							if (FixedButton("-")) { ranges.RemoveAt(i--); }

						} EndHorizontal();
						info.Draw();
					} EndVertical();
					Rect area = GUILayoutUtility.GetLastRect();
					GUISkin skin = GUI.skin;
					GUI.skin = GUI.blankSkin;
					if (GUI.Button(area, "")) {
						selected = info;
					}
					GUI.skin = skin;

				}
				if (Button("+")) { ranges.Add(new RangeInfo()); }
			}

		} EndVertical(); 
		
	}

	void Save(string filePath) {
		StringBuilder content = new StringBuilder();

		content += "#Sheets\n";
		foreach (var sheet in sheets) { content += "" + sheet + "\n"; }

		content += "#Icons\n";
		foreach (var icon in icons) { content += "" + icon + "\n"; }

		content += "#IconRange\n";
		foreach (var range in ranges) { content += "" + range + "\n"; }

		File.WriteAllText(filePath, content.ToString());
		AssetDatabase.Refresh();
	}

	void Load(string filePath) {
		string[] lines = File.ReadAllLines(filePath);
		string mode = "#Sheets";
		sheets.Clear();
		icons.Clear();
		ranges.Clear();

		for (int i = 0; i < lines.Length; i++) {
			string line = lines[i];

			if (line.Length < 1) { continue; }
			if (line.StartsWith("//")) { continue; }
			if (line.StartsWith("#")) { mode = line; continue; }

			if (mode == "#Sheets") {
				try {
					SheetInfo info = new SheetInfo(line);
					sheets.Add(info);
				} catch {
					Debug.LogWarning("IconEditor: Could not load sheet from line " + i);
					continue;
				}
			}


			if (mode == "#Icons") {
				try {
					IconInfo info = new IconInfo(line);
					icons.Add(info);
				} catch {
					Debug.LogWarning("IconEditor: Could not load icon from line " + i);
					continue;
				}
			}

			if (mode == "#IconRange") {
				try {
					RangeInfo info = new RangeInfo(line);
					ranges.Add(info);
				} catch {
					Debug.LogWarning("IconEditor: Could not load range from line " + i);
					continue;
				}
			}

		}
	}

}


#endif
