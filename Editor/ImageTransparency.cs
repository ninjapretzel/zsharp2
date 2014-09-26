#if UNITY_EDITOR && !UNITY_WEBPLAYER
using UnityEditor;
using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class ImageTransparency : ZEditorWindow {
	
	public Texture2D tex;
	public string path; 
	public Vector2 pixelPosition;
	
	
	public UnityEngine.Object o;
	public static ImageTransparency window;
	
	public string basePath { 
		get {
			return Application.dataPath.PreviousDirectory();
			
			
		}
		
	}
	
	[MenuItem ("Window/Texture Transparency")]
	static void ShowWindow() {
		window = (ImageTransparency)EditorWindow.GetWindow (typeof (ImageTransparency));
		
	}
	
	void OnGUI() {
		GUILayout.BeginVertical("box"); {
			o = EditorGUILayout.ObjectField("Drag a folder here: ", o, typeof(UnityEngine.Object), false);
			if (o != null) {
				string s = AssetDatabase.GetAssetPath(o);
				
				
				
				path = basePath + "/" + s;
				o = null;
			}
			
			
			GUILayout.BeginHorizontal("box"); {
				GUILayout.Label("Path: ", GUILayout.ExpandWidth(false));
				path = EditorGUILayout.TextField(path);
			} GUILayout.EndHorizontal();
			
			pixelPosition = EditorGUILayout.Vector2Field("Position: ", pixelPosition);
			
			if (GUILayout.Button("GO")) {
				ConvertFiles();
			}
			
		} GUILayout.EndVertical();
		
		
	}
	
	void ConvertFiles() {
		string[] pngList = Directory.GetFiles(path, "*.png");
		//Debug.Log(pngList.Length);
		
		foreach (string f in pngList) {
			string file = f.Replace("\\", "/");
			//Debug.Log(file);
			
			Texture2D tex = new Texture2D(4, 4, TextureFormat.ARGB32, false);
			
			
			tex.LoadImage(File.ReadAllBytes(file));
			
			Color transparency = tex.GetPixel((int)pixelPosition.x, (int)pixelPosition.y);
			
			for (int x = 0; x < tex.width; x++) {
				for (int y = 0; y < tex.height; y++) {
					if (tex.GetPixel(x, y) == transparency) {
						tex.SetPixel(x, y, Color.clear);
						
					}
					
					
				}
			}
			
			tex.Apply();
			
			File.WriteAllBytes(file, tex.EncodeToPNG() );
			
			
		}
		
		
		
	}
}




#endif