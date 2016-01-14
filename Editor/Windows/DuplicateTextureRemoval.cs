#if UNITY_EDITOR && !UNITY_WEBPLAYER
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Collections;
using System.IO;
using System.Collections.Generic;

public class DuplicateTextureRemoval : ZEditorWindow {
	
	[MenuItem("Window/DuplicateTextureRemoval")]
	public static void ShowWindow() { 
		EditorWindow.GetWindow(typeof(DuplicateTextureRemoval));
	}
	
	public DuplicateTextureRemoval() {
		lumpsToFix = new HashSet<Lump>();
		log = new List<string>();
		fullLog = new StringBuilder();
		//textures = new List<Texture2D>();
		//materials = new List<Material>();
		lumps = new List<Lump>();
		texGUIDs = new string[0];
		matGUIDs = new string[0];
		textureGuidMap = new Dictionary<string,string>();

	}

	private Dictionary<string, string> textureGuidMap;

	[System.NonSerialized] private string excludes = "Standard Assets,Resources";
	[System.NonSerialized] private string[] excludeArray;

	[System.NonSerialized] private List<string> log;
	[System.NonSerialized] private StringBuilder fullLog;
	[System.NonSerialized] private Vector2 logPos;
	
	[System.NonSerialized] private Vector2 lumpPos;

	[System.NonSerialized] private HashSet<Lump> lumpsToFix;
	[System.NonSerialized] private List<Lump> lumps;
	//[System.NonSerialized] private List<Material> materials;
	//[System.NonSerialized] private List<Texture2D> textures;
	[System.NonSerialized] private string[] texGUIDs;
	[System.NonSerialized] private string[] matGUIDs;
	[System.NonSerialized] private int iTex = 0;
	[System.NonSerialized] private int iMat = 0;

	void OnGUI() {
		var buttonWidth = Width(240);
		var spacing = 15;

		BeginHorizontal(); {

			BeginVertical(Width(width/3f)); {

				BeginVertical("box"); {

					Label("Textures: " + texGUIDs.Length);

					Label("Exclude Directories(Separate by comma)");
					excludes = TextField(excludes);

					if (Button("1. Load/Search Textures", buttonWidth)) {
						LoadTextures();
					}
					if (texGUIDs.Length > 0 && iTex < texGUIDs.Length) {
						Space(spacing);
						Label("Loading and testing for duplicate texutres. (PLEASE BE PATIENT)");
						Label("Loaded: " + iTex + " / " + texGUIDs.Length);
						Label("Unique/Duplicate textures: " + lumps.Count + " / " + (iTex - lumps.Count) );
					}


					/*
					Space(spacing);
					Label("Texture Distances: " + textureGuidMap.Count);
					string path = Application.dataPath.PreviousDirectory() + "/textureGUIDS.dat";
					Label("Path for save/load:");
					Label(path);
					if (Button("Save Duplicate Texture GUIDs", buttonWidth)) {
						StringBuilder str = new StringBuilder();
						foreach (var pair in textureGuidMap) {
							str += pair.Key + "," + pair.Value + "\n";
						}
			
						File.WriteAllText(path, str.ToString());
					}
					if (Button("Load Duplicate Texture GUIDs", buttonWidth)) {
						if (File.Exists(path)) {
							textureGuidMap.Clear();
							string[] lines = File.ReadAllLines(path);
							foreach (string line in lines) {
								string[] content = line.Split(',');
								textureGuidMap.Add(content[0], content[1]);
							}
						}
					}
					//*/
					Space(spacing);
					Label("2. Set up job using Lumps panel below");

					Space(spacing);
					Label("Materials: " + matGUIDs.Length);
					if (Button("3. Load/Fix materials", buttonWidth)) {
						LoadMaterials();
					}

					if (matGUIDs.Length > 0 && iMat < matGUIDs.Length) {
						Space(spacing);
						Label("Adjusting material texture references.");
						Label("Checked: " + iMat + " / " + matGUIDs.Length);
					}

					Space(spacing);
					if (Button("4. Delete duplicate textures", buttonWidth)) {
						DeleteDuplicateTextures();
					}

					Space(spacing);

					if (Button("5. Restore textures to non read/write.")) {
						RestoreTextures();
					}

				} EndVertical();

				BeginVertical("box"); {
					Label("Lumps: ");
					lumpPos = BeginScrollView(lumpPos); {
						foreach (Lump l in lumps) {
							GUI.color = l.color;

							BeginHorizontal("box"); {
								bool contained = lumpsToFix.Contains(l);
								bool now = Toggle(contained, "");
								if (contained != now) {
									if (now) { lumpsToFix.Add(l); }
									else { lumpsToFix.Remove(l); }
								}
								
								Label(l.name + " : " + l.size);
							} EndHorizontal();

						}
						GUI.color = Color.white;
					} EndScrollView();



				} EndVertical(); 

			
			} EndVertical();
			
			BeginVertical("box", Width(width/2f)); {
				if (Button("Save Log", buttonWidth)) {
					string path = Application.dataPath.PreviousDirectory() + "/";
					path += DateTime.Now.ToFilenameString();
					path += ".txt";

					File.WriteAllText(path, fullLog.ToString());
				}
				logPos = BeginScrollView(logPos); {
					StringBuilder str = new StringBuilder();
					foreach (string s in log) { str += s; }
					Label(str.ToString());
				} EndScrollView();
			} EndVertical();

		} EndHorizontal();
	}
	
	void Update() { 
		if (iTex < texGUIDs.Length) {
			LoadNextTexture();
			Repaint();
		} else if (iTex == texGUIDs.Length) {
			lumps.Sort();
			lumps.Reverse();
			Repaint();
			iTex++;
		}

		if (iMat < matGUIDs.Length) {
			LoadNextMaterial();
			Repaint();
		}

	}
	void OnInspectorUpdate() { }
	
	void OnFocus() { }
	void OnLostFocus() { }

	void OnSelectionChange() { }
	void OnHierarchyChange() { }
	void OnProjectChange() { }
	
	void OnDestroy() { }
	
	void RestoreTextures() {
		texGUIDs = AssetDatabase.FindAssets("t:texture2D");
		excludeArray = excludes.Split(',');

		//List<string> keepGUIDs = new List<string>();

		foreach (string guid in texGUIDs) {
			string path = AssetDatabase.GUIDToAssetPath(guid);

			TextureImporter importer = TextureImporter.GetAtPath(path) as TextureImporter;
			if (importer != null) {
				importer.isReadable = false;
				importer.textureFormat = TextureImporterFormat.ARGB32;
				Log("Resetting " + path);
				AssetDatabase.WriteImportSettingsIfDirty(path);
				//EditorUtility.SetDirty(importer);
			}

		}
		AssetDatabase.Refresh();
	}

	void LoadTextures() {

		texGUIDs = AssetDatabase.FindAssets("t:texture2D");
		excludeArray = excludes.Split(',');
		
		List<string> keepGUIDs = new List<string>();

		foreach (string guid in texGUIDs) {
			string path = AssetDatabase.GUIDToAssetPath(guid);
			if (path.ContainsAny(excludeArray)) { continue; }

			keepGUIDs.Add(guid);
		}
		texGUIDs = keepGUIDs.ToArray();



		foreach (string guid in texGUIDs) {
			string path = AssetDatabase.GUIDToAssetPath(guid);
			
			TextureImporter importer = TextureImporter.GetAtPath(path) as TextureImporter;
			if (importer != null) {
				importer.isReadable = true;
				importer.textureFormat = TextureImporterFormat.ARGB32;
				AssetDatabase.WriteImportSettingsIfDirty(path);
				
				//EditorUtility.SetDirty(importer);
			}

		}
		AssetDatabase.Refresh();


		iTex = 0;

	}

	void LoadMaterials() {
		BuildTextureGUIDMap(lumpsToFix);
		matGUIDs = AssetDatabase.FindAssets("t:material");

		iMat = 0;
	}

	void BuildTextureGUIDMap(IEnumerable<Lump> job) {
		textureGuidMap.Clear();
		foreach (Lump l in job) { AddLumpToJob(l); }
	}

	void BuildTextureGUIDMap(Lump lump) {
		textureGuidMap.Clear();
		AddLumpToJob(lump);
	}

	void AddLumpToJob(Lump l) {
		foreach (string guid in l) {
			textureGuidMap[guid] = l.guid;
		}
	}

	

	void Log(object o) {
		string s = o.ToString() + "\n";
		fullLog += s;
		log.Add(s);
		if (log.Count > 100) { log.RemoveAt(0); }
		
		logPos.y = float.MaxValue;
	}

	void LoadNextTexture() {
		if (iTex >= texGUIDs.Length) { return; }

		string guid = texGUIDs[iTex];
		string path = AssetDatabase.GUIDToAssetPath(guid);
		iTex++;

		
		try {
			Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
			
			float tolerance = 5;
			foreach (Lump lump in lumps) {

				float dist = QDist(lump.tex, tex, tolerance);
				//log += "\n" + lump.name + " : " + dist;
				if (dist < tolerance) {
					lump.AddGUID(guid);
					//lumps.Sort();
					//log = "Added to lump: " + lump.name + log;
					//log += log);
					Log("[" + lump.name + "]++");
					return;
				}
			}
			
			Log("Made [" + tex.name + "]");
			Lump l = new Lump(tex, guid);
			lumps.Add(l);
			//lumps.Sort();

		} catch {
			Log("Failed [" + path + "]");
		}
		//log = "Added new lump: " + tex.name + log;
	}
		
	
	public void LoadNextMaterial() {
		string guid = matGUIDs[iMat];
		string path = AssetDatabase.GUIDToAssetPath(guid);
		iMat++;
		
		Material mat = AssetDatabase.LoadAssetAtPath<Material>(path);
		Shader s = mat.shader;
		
		string[] sTexes = TextureProperties(s);
		foreach (string tex in sTexes) {
			Texture2D t = mat.GetTexture(tex) as Texture2D;
			if (t != null) {
				string tpath = AssetDatabase.GetAssetPath(t);
				string tguid = AssetDatabase.AssetPathToGUID(tpath);

				if (textureGuidMap.ContainsKey(tguid)) {
					string replacementPath = AssetDatabase.GUIDToAssetPath(textureGuidMap[tguid]);
					Texture2D replacement = AssetDatabase.LoadAssetAtPath<Texture2D>(replacementPath);
					mat.SetTexture(tex, replacement);
					Log("Fixed " + tex + " on " + mat.name + " at [" + path + "]");
				}

			}
		}
		AssetDatabase.SaveAssets();


	}

	public void DeleteDuplicateTextures() {
		foreach (var pair in textureGuidMap) {
			if (pair.Key != pair.Value) {
				string path = AssetDatabase.GUIDToAssetPath(pair.Value);

				Log("Deleting [" + path + "]");
				AssetDatabase.DeleteAsset(path);

			}
		}
	}

	static string[] TextureProperties(Shader s) {
		int properties = ShaderUtil.GetPropertyCount(s);
		List<string> textureProperties = new List<string>();
		for (int i = 0; i < properties; i++) {
			if (ShaderUtil.GetPropertyType(s, i) == ShaderUtil.ShaderPropertyType.TexEnv) {
				textureProperties.Add(ShaderUtil.GetPropertyName(s, i));
			}
		}

		return textureProperties.ToArray();
	}

	public class Lump : IEnumerable<string>, IComparable<Lump> {
		public Texture2D tex { get; private set; }
		public string name { get { return tex.name; } }
		public string guid { get { return assets[0]; } }
		public int size { get { return assets.Count; } }

		public Color color {
			get {
				if (size == 1) { return Color.white; }
				
				float f = (size - 1f) / 32f;
				Color c = (size < 32) ? Color.Lerp(Color.yellow, Color.red, f) : Color.red;
				
				return Color.Lerp(c, Color.white, .5f);
				
			}
		}


		List<string> assets;
		
		public Lump() { assets = new List<string>(); }
		public Lump(Texture2D t) : this() { tex = t; }
		public Lump(Texture2D t, string guid) : this(t) { assets.Add(guid); }
		
		public void AddGUID(string guid) { assets.Add(guid); }
		
		int IComparable<Lump>.CompareTo(Lump other) { return size - other.size; }

		IEnumerator<string> IEnumerable<string>.GetEnumerator() { return assets.GetEnumerator(); }
		IEnumerator IEnumerable.GetEnumerator() { return assets.GetEnumerator(); }
	}

	public static float QDist(Texture2D a, Texture2D b, float tolerance = 5) {
		if (a.width != b.width || a.height != b.height) { return float.MaxValue; }
		float d = 0;

		Color[] acs = a.GetPixels();
		Color[] bcs = b.GetPixels();

		for (int i = 0; i < acs.Length; i++) {
			Color ac = acs[i];
			Color bc = bcs[i];

			Vector4 diff = ac - bc;
			float mag = diff.sqrMagnitude;

			d += mag;
			if (d > tolerance) { return d; }
		}



		return d;
	}

	public static float Distance(Texture2D a, Texture2D b) {
		if (a.width != b.width || a.height != b.height) { return float.MaxValue; }
		float d = 0;

		Color[] acs = a.GetPixels();
		Color[] bcs = b.GetPixels();

		for (int i = 0; i < acs.Length; i++) {
			Color ac = acs[i];
			Color bc = bcs[i];

			Vector4 diff = ac - bc;
			float mag = diff.sqrMagnitude;

			d += mag;
		}



		return d;
	}

}


/// <summary>
/// Class for helper methods
/// </summary>
internal static class DuplicateTextureRemovalHelpers {
	static string[] months = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };


	internal static string ToFilenameString(this DateTime date) {
		string str = "";
		str += "" + date.Year;
		str += months[date.Month - 1];
		str += "." + date.Day.TwoDigitString();
		str += "_" + date.Hour.TwoDigitString();
		str += "-" + date.Minute.TwoDigitString();
		str += "-" + date.Second.TwoDigitString();

		return str;
	}

	internal static string TwoDigitString(this int i) {
		i = i % 100;
		if (i < 10) {
			return "0" + i;
		}

		return "" + i;
	}
}
#endif
