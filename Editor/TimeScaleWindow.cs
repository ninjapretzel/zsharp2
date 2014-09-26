#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;

public class TimeScaleWindow : ZEditorWindow {
	
	private float lastTimeScale;
	private float timeScale;
	private bool wasPlaying;
	[System.NonSerialized] private int frame;
	
	[System.NonSerialized] private static float snapTolerance = .075f;
	[System.NonSerialized] private static float[] snapTo = { .25f, .3333f, .5f, .6666f, 1f, 1.5f, 2f, 3f };
	
	public static TimeScaleWindow window;
	
	[MenuItem ("Utilities/Time Scale")]
	static void ShowWindow() {
		window = EditorWindow.GetWindow(typeof(TimeScaleWindow)) as TimeScaleWindow;
		
	}
	
	public TimeScaleWindow() : base() {
		maxSize = new Vector2(1600, 55);
		minSize = new Vector2(200, 55);
		timeScale = 1;
		lastTimeScale = 1;
		frame = 0;
		title = "Time Scale";
		wasPlaying = EditorApplication.isPlaying;
	}
	
	void Update() {
		if (EditorApplication.isPlayingOrWillChangePlaymode && !wasPlaying) {
			lastTimeScale = timeScale;
			Repaint();
		} else if (wasPlaying && !EditorApplication.isPlaying) {
			timeScale = lastTimeScale;
			Repaint();
		}
		
		if (EditorApplication.isPlaying) {
			Time.timeScale = timeScale;
		}
		
		wasPlaying = EditorApplication.isPlaying;
		
		if (++frame % 50 == 0) { Repaint(); }
	}
	
	void OnGUI() {
		//GUI.color = Color.white;
		if (EditorApplication.isPlaying) { 
			GUI.color = GUI.color.Blend(Color.red);
		}
		BeginVertical("box"); {
			BeginHorizontal("box"); {
				Label("Time Scale: ");
				timeScale = FloatField(timeScale);
				if (Button("Reset")) { timeScale = 1; }
			} EndHorizontal();
			
			float beforeSlider = timeScale;
			timeScale = HorizontalSlider(timeScale, .1f, 5);
			if (beforeSlider != timeScale) {
				foreach (float f in snapTo) {
					if ((timeScale-f).Abs() < snapTolerance) { timeScale = f; }
				}
				
			}
			
			
		} EndVertical();
	}
	
}

#endif