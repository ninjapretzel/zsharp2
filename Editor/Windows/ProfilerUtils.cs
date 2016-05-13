using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;

public class ProfilerUtils : EditorWindow {

	private int samples;

	public ProfilerUtils() {
		titleContent = new GUIContent("Profiler Utils");
		samples = Profiler.maxNumberOfSamplesPerFrame;
	}
	
	[MenuItem ("Utilities/Profiler Utils")]
	public static void ShowWindow() {
		ProfilerUtils main = EditorWindow.GetWindow<ProfilerUtils>();
		main.autoRepaintOnSceneChange = true;
		UnityEngine.Object.DontDestroyOnLoad(main);
		
	}
	
	protected void OnGUI() {
		EditorGUILayout.BeginVertical(GUILayout.ExpandWidth(true)); {
			samples = EditorGUILayout.IntField("Max samples", samples);
			if (GUILayout.Button("Set")) {
				Profiler.maxNumberOfSamplesPerFrame = samples;
			}
		}
		
	}
	
}
