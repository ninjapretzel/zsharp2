using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class Icons {
	
	class SheetInfo {
		public int iconSize;
		public Texture2D atlas;
	}

	static Dictionary<string, SheetInfo> sheets;
	static Dictionary<string, SpriteInfo> icons;

	public static SpriteInfo GetIcon(string iconName) {
		if (icons.ContainsKey(iconName)) { return icons[iconName]; }
		return null;
	}

	public static void Draw(this Sprite sprite, Rect area) {
		float w = sprite.texture.width; float h = sprite.texture.height;
		Rect texCoords = sprite.rect;
		texCoords.x /= w; texCoords.width /= w;
		texCoords.y /= h; texCoords.height /= h;
		GUI.DrawTextureWithTexCoords(area, sprite.texture, texCoords); 
	}

	public static bool loaded = Load();
	public static bool Load() {
		sheets = new Dictionary<string,SheetInfo>();
		icons = new Dictionary<string,SpriteInfo>();
		string iconInfo;
		try {
			iconInfo = Resources.Load<TextAsset>("IconInfo").text;
		} catch {
			Debug.LogWarning("Icons: File Resources/IconInfo.csv is missing, but you are trying to access Icons class");
			return false;
		}


		string[] lines = iconInfo.Split('\n');

		string mode = "#Sheets";
		for (int i = 0; i < lines.Length; i++) {
			string line = lines[i];
			if (line.Length < 1) { continue; }
			if (line.StartsWith("//")) { continue; }
			if (line.StartsWith("#")) { mode = line; continue; }
			string[] splits = line.Split(',');

			if (mode == "#Sheets") {
				SheetInfo info = new SheetInfo();
				
				string sheetName = splits[0];
				info.atlas = Resources.Load<Texture2D>(sheetName);
				if (info.atlas == null) { 
					Debug.LogWarning("Icons: Texture for sheet " + sheetName + " not found. Did not create sheet");
					continue;
				}

				try {
					info.iconSize = splits[1].ParseInt();
				} catch { 
					Debug.LogWarning("Icons: Couldn't parse size number when parsing information at line " + i + ", using default size of 32"); 
					info.iconSize = 32;
				}

				sheets[sheetName] = info;
				continue;
			}

			if (mode == "#Icons") {
				string iconName, sheetName;
				int x, y;

				try {
					iconName = splits[0];
					sheetName = splits[1];
					x = splits[2].ParseInt();
					y = splits[3].ParseInt();
				} catch {
					Debug.LogWarning("Icons: Could not parse icon information at line " + i);
					continue;
				}

				if (!sheets.ContainsKey(sheetName)) {
					Debug.LogWarning("Icons: Sheet " + sheetName + " was requested but not found on line " + i);
					continue;
				}
				SheetInfo sheetInfo = sheets[sheetName];
				int size = sheetInfo.iconSize;
				Rect r = new Rect();
				r.x = x * size;
				//r.y = y * size;
				r.y = sheetInfo.atlas.height - (y + 1) * size;
				//r.y = sheetInfo.atlas.height - y * size;
				r.width = size;
				r.height = size;

				//Sprite icon = Sprite.Create(sheetInfo.atlas, r, Vector2.one * size/2);
				SpriteInfo icon = new SpriteInfo(sheetInfo.atlas, r);
				icons[iconName] = icon;

			}


			if (mode == "#IconRange") {
				string iconBaseName, sheetName;
				int x, y, startNum, cnt;
				try {
					iconBaseName = splits[0];
					sheetName = splits[1];
					x = splits[2].ParseInt();
					y = splits[3].ParseInt();
					startNum = splits[4].ParseInt();
					cnt = splits[5].ParseInt();
				} catch {
					Debug.LogWarning("Icons: Could not parse range information at line " + i);
					continue;
				}

				if (!sheets.ContainsKey(sheetName)) {
					Debug.LogWarning("Icons: Sheet " + sheetName + " was requested but not found on line " + i);
					continue;
				}
				SheetInfo sheetInfo = sheets[sheetName];
				Texture2D atlas = sheetInfo.atlas;
				int size = sheetInfo.iconSize;
				//Vector2 pivot = Vector2.one * size/2;
				Rect r = new Rect();
				r.x = x * size;
				r.y = sheetInfo.atlas.height - (y+1) * size;
				r.width = size;
				r.height = size;

				for (int k = 0; k < cnt; k++) {
					if (r.x >= atlas.width) { 
						r.x = 0;
						r.y -= size;
						if (r.y < 0) {
							Debug.LogWarning("Icons: Range defined on line " + i + " goes off the sheet after " + k + " icons are made");
							break;
						}
					}
					//Sprite icon = Sprite.Create(atlas, r, pivot);
					SpriteInfo icon = new SpriteInfo(sheetInfo.atlas, r);
					string iconName = iconBaseName + (k+startNum);
					icons[iconName] = icon;

					r.x += r.width;
				}

			}


		}
		string str = "Icons: Loaded " + sheets.Count + " sheets and " + icons.Count + " icons.";
		foreach (var pair in icons) {
			str += "\n" + pair.Key;
		}
		Debug.Log(str);


		return true;
	}

	

	
}
