#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;

public class ControlBinder : EditorWindow {
	
	public static string path { get { return Application.dataPath + "/Data/Controls/Resources/"; } }
	public static string target { get { return "Controls.csv"; } }
	public static string ouyaTarget { get { return "OuyaControls.csv"; } }
	
	public static List<string> targets;// = new List<string>();
	
	private static Vector2 scrollPos = Vector2.zero;
	private static string clipboard = null;
	
	private int currentTab = 0;
	
	private static List<string> controls;// = new List<string>();
	private static List<string> keys;// = new List<string>();
	
	[MenuItem ("Window/Control Bindings")]
	public static void ShowWindow() {
		targets = new List<string>();
		ControlBinder main = (ControlBinder)EditorWindow.GetWindow(typeof(ControlBinder));
		main.minSize = new Vector2(398,0);
		main.autoRepaintOnSceneChange = true;
		UnityEngine.Object.DontDestroyOnLoad(main);
		main.Start();
	}
	
	public void Start() {
		if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
		string[] targets = Directory.GetFiles(path, "*.csv");
		if(targets.Length == 0) {
			ControlBinder.targets = new List<string>(1);
			ControlBinder.targets[0] = path+"Controls.csv";
		} else {
			ControlBinder.targets = Enumerable.ToList(targets);
		}
		LoadDatabase(targets[0]);
	}
	
	public void OnProjectChange() {
		if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
		string[] currentTargets = Directory.GetFiles(path, "*.csv");
		targets = Enumerable.ToList(currentTargets);
	}
	
	public void ReloadTargets() {
		if (!Directory.Exists(path)) { Directory.CreateDirectory(path); }
		string[] currentTargets = Directory.GetFiles(path, "*.csv");
		targets = Enumerable.ToList(currentTargets);
	}
	
	public void OnInspectorUpdate() {
		if(targets == null) {
			ReloadTargets();
		}
	}
	
	public void OnGUI() {
		if(targets == null) {
			ReloadTargets();
		}
		EditorGUILayout.BeginVertical(); {
			EditorGUILayout.BeginHorizontal(); {
				Color backup = GUI.color;
				for(int i=0;i<targets.Count;i++) {
					string filename = targets[i];
					try {
						filename = filename.Substring(filename.LastIndexOf("\\")+1);
					} catch { ; }
					try {
						filename = filename.Substring(filename.LastIndexOf("/")+1);
					} catch { ; }
					if(currentTab == i) {
						GUI.color = backup;
					} else {
						GUI.color = Color.gray;
					}
					if(GUILayout.Button(filename.Substring(0,filename.Length-4), GUILayout.Width(100))) {
						SaveCurrent();
						currentTab = i;
						LoadCurrent();
					}
				}
				GUI.color = backup;
			} EditorGUILayout.EndHorizontal();
			EditorGUILayout.BeginVertical("box"); {
				BindTableGUI();
			} EditorGUILayout.EndVertical();
			EditorGUILayout.BeginHorizontal(); {
				if(GUILayout.Button("Apply & Save", GUILayout.Width(100))) {
					SaveCurrent();
				}
				if(GUILayout.Button("Load defaults", GUILayout.Width(100))) {
					LoadDefault();
				}
				if(GUILayout.Button("Copy sheet", GUILayout.Width(100))) {
					CopyAll();
				}
				if(GUILayout.Button("Paste sheet", GUILayout.Width(100))) {
					PasteAll();
				}
			} EditorGUILayout.EndHorizontal();
		} EditorGUILayout.EndVertical();
	}
	
	public void BindTableGUI() {
		EditorGUILayout.BeginHorizontal(); {
			scrollPos = EditorGUILayout.BeginScrollView(scrollPos); {
				EditorGUILayout.BeginVertical(); {
					EditorGUILayout.BeginHorizontal("box"); {
						GUILayout.Label("", GUILayout.Width(66));
						GUILayout.Box("#", GUILayout.Width(20));
						GUILayout.Box("Control", GUILayout.Width(200));
						GUILayout.Box("Keys/Axes", GUILayout.Width(Mathf.Max(70, position.width - 328)));
					} EditorGUILayout.EndHorizontal();
					if(controls == null) {
						LoadCurrent();
					} else {
						for(int i=0;i<controls.Count;i++) {
							BindTableItem(i);
						}
					}
					if(GUILayout.Button("+", GUILayout.Width(20))) {
						controls.Add("New");
						keys.Add("None");
					}
				} EditorGUILayout.EndVertical();
			} EditorGUILayout.EndScrollView();
		} EditorGUILayout.EndHorizontal();
	}
	
	public void BindTableItem(int index) {
		EditorGUILayout.BeginHorizontal(); {
			if(GUILayout.Button("-", GUILayout.Width(20))) {
				controls.RemoveAt(index);
				keys.RemoveAt(index);
			} else {
				if(index > 0 && GUILayout.Button("/\\", GUILayout.Width(20))) {
					controls.Swap<string>(index, index-1);
					keys.Swap<string>(index, index-1);
				} else {
					if(index == 0) {
						GUILayout.Label("", GUILayout.Width(20));
					}
					if(index < controls.Count-1 && GUILayout.Button("\\/", GUILayout.Width(20))) {
						controls.Swap<string>(index, index+1);
						keys.Swap<string>(index, index+1);
					} else {
						if(index == controls.Count-1) {
							GUILayout.Label("", GUILayout.Width(20));
						}
						EditorGUILayout.BeginHorizontal("box"); {
							GUILayout.Label(index.ToString(), GUILayout.Width(20));
							controls[index] = EditorGUILayout.TextField(controls[index], GUILayout.Width(200));
							if(keys[index].Length > 0) {
								string[] key = keys[index].Split(',');
								if(key.Length > 0) {
									if(GUILayout.Button("-", GUILayout.Width(20))) {
										string[] newList = new string[key.Length-1];
										for(int j=0;j<newList.Length;j++) {
											newList[j] = key[j+1];
										}
										key = newList;
									}
								}
								if(key.Length > 0) {
									keys[index] = EditorGUILayout.TextField(key[0], GUILayout.Width(120));
								} else {
									keys[index] = "";
								}
								for(int i=1;i<key.Length;i++) {
									if(GUILayout.Button("-", GUILayout.Width(20))) {
										string[] newList = new string[key.Length-1];
										for(int j=0;j<newList.Length;j++) {
											if(j<i) {
												newList[j] = key[j];
											} else {
												newList[j] = key[j+1];
											}
										}
										key = newList;
									}
									if(key.Length > i) {
										keys[index] += ","+EditorGUILayout.TextField(key[i], GUILayout.Width(120));
									}
								}
							}
							if(GUILayout.Button("+", GUILayout.Width(20))) {
								if(keys[index].Length > 0) {
									keys[index] += ",None";
								} else {
									keys[index] = "None";
								}
							}
						} EditorGUILayout.EndHorizontal();
					}
				}
			}
		} EditorGUILayout.EndHorizontal();
	}
	
	public void LoadDefault() {
		string[] bindings;
		bindings = (Resources.Load("DefaultControls", typeof(TextAsset)) as TextAsset).text.ConvertNewlines().Split('\n');
		controls = new List<string>(bindings.Length);
		keys = new List<string>(bindings.Length);
		for(int i=0;i<bindings.Length;i++) {
			if(bindings[i][0] != '#') {
				int ind = bindings[i].IndexOf(',');
				controls.Add(bindings[i].Substring(0, ind));
				keys.Add(bindings[i].Substring(ind+1));
			}
		}
	}
	
	public void LoadCurrent() {
		if(currentTab > targets.Count-1) {
			return;
		}
		LoadDatabase(targets[currentTab]);
	}
	
	public void LoadDatabase(string file) {
		string[] bindings;
		if (File.Exists(file)) {
			bindings = File.ReadAllLines(file);
		} else {
			bindings = (Resources.Load("DefaultControls", typeof(TextAsset)) as TextAsset).text.ConvertNewlines().Split('\n');
		}
		controls = new List<string>(bindings.Length);
		keys = new List<string>(bindings.Length);
		for(int i=0;i<bindings.Length;i++) {
			if(bindings[i][0] != '#') {
				int ind = bindings[i].IndexOf(',');
				controls.Add(bindings[i].Substring(0, ind));
				keys.Add(bindings[i].Substring(ind+1));
			}
		}
	}
	
	public void SaveCurrent() {
		if(currentTab > targets.Count-1) {
			return;
		}
		SaveDatabase(targets[currentTab]);
	}
	
	public void SaveDatabase(string file) {
		#if !UNITY_WEBPLAYER
		if(controls != null) {
			if (File.Exists(file)) {
				File.Delete(file);
			}
			string data = "#ControlName,HardwareName\n";
			for(int i=0;i<controls.Count;i++) {
				data += controls[i];
				data += ",";
				data += keys[i];
				if(i < controls.Count-1) {
					data += (char)0x0A;
				}
			}
			StreamWriter sw = File.CreateText(file);
			sw.Write(data);
			sw.Close();
		}
		#endif
	}
	
	public void CopyAll() {
		clipboard = "";
		for(int i=0;i<controls.Count;i++) {
			clipboard += controls[i];
			clipboard += ",";
			clipboard += keys[i];
			if(i < controls.Count-1) {
				clipboard += (char)0x0A;
			}
		}
	}
	
	public void PasteAll() {
		string[] bindings;
		bindings = clipboard.Split((char)0x0A);
		controls = new List<string>(bindings.Length);
		keys = new List<string>(bindings.Length);
		for(int i=0;i<bindings.Length;i++) {
			int ind = bindings[i].IndexOf(',');
			controls.Add(bindings[i].Substring(0, ind));
			keys.Add(bindings[i].Substring(ind+1));
		}
	}
}
#endif