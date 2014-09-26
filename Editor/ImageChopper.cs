#if UNITY_EDITOR && !UNITY_WEBPLAYER
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections;

public class ImageChopper : EditorWindow {
	
	public Texture2D tex;
	public static ImageChopper window;
	public Vector2 size;
	
	[MenuItem ("Window/Texture Chopper")]
	static void ShowWindow() {
		window = (ImageChopper)EditorWindow.GetWindow (typeof (ImageChopper));
	}
	
	void OnGUI() {
		GUILayout.BeginVertical("box"); {
			GUILayout.BeginHorizontal("box"); {
				Texture2D lastTex = tex;
				tex = EditorGUILayout.ObjectField(tex, typeof(Texture2D), false) as Texture2D;
				
				if (tex != null && tex != lastTex) {
					NewTextureAssigned();
				}
				
			} GUILayout.EndHorizontal();
			
			if (tex != null) {
				GUILayout.Label("Texture: [" + tex.width + " x " + tex.height + "]");
				
				GUILayout.BeginHorizontal("box"); {
					GUILayout.Label("Icon Size:");
					size.x = EditorGUILayout.FloatField(size.x);
					GUILayout.Label("x");
					size.y = EditorGUILayout.FloatField(size.y);
					
					
				} GUILayout.EndHorizontal();
				
				if (GUILayout.Button("Go")) {
					ChopTex();
				}
				
			} else {
				GUILayout.Label("Please assign a texture...");
			}
				
		} GUILayout.EndVertical();
		
		
	}
	
	void NewTextureAssigned() {
		string path = AssetDatabase.GetAssetPath(tex);
		TextureImporter importer = AssetImporter.GetAtPath(path) as TextureImporter;
		
		if (importer != null) {
			importer.textureType = TextureImporterType.Advanced;
			importer.textureFormat = TextureImporterFormat.RGBA32;
			importer.maxTextureSize = 4096;
			importer.npotScale = TextureImporterNPOTScale.None;
			importer.grayscaleToAlpha = false;
			importer.generateCubemap = TextureImporterGenerateCubemap.None;
			importer.isReadable = true;
			importer.mipmapEnabled = false;
			importer.normalmap = false;
			importer.lightmap = false;
			
			AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
		}
		
		size.x = tex.height;
		size.y = tex.height;
		
	}
	
	void ChopTex() {
		if (tex == null) { return; }
		int sx = (int)size.x;
		int sy = (int)size.y;
		
		int w = tex.width / sx;
		int h = tex.height / sy;
		
		string file;
		string path = Application.dataPath;
		path = path.Substring(0, path.Length - "Assets".Length);
		path += AssetDatabase.GetAssetPath(tex);
		
		file = path.Substring(path.LastIndexOf("/")+1);
		file = file.Substring(0, file.LastIndexOf("."));
		path = path.Substring(0, path.LastIndexOf("/"));
		
		Debug.Log(path);
		Debug.Log(file);
		
		string outpath = path + "/Resources/"; 
		Directory.CreateDirectory(outpath);
		outpath += file;
		Debug.Log(outpath);
		
		//if (true) { return; }
		
		int i = 0;
		for (int y = h-1; y >= 0; y--) {
			for (int x = 0; x < w; x++) {
				Color[] colors = tex.GetPixels(x * sx, y * sy, sx, sy);
				
				Texture2D t = new Texture2D(sx, sy, TextureFormat.RGBA32, false);
				
				t.SetPixels(colors);
				t.Apply();
				
				File.WriteAllBytes(outpath + i + ".png", t.EncodeToPNG());
				
				
				i++;
			}
			
		}
		
		AssetDatabase.Refresh();
	
	}
	
	
}




















#endif
