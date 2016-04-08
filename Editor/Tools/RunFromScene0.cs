using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;

[InitializeOnLoad]
public class RunFromScene0 {

	private static Scene current;

	[MenuItem("Tools/Play from scene 0 &p")]
	public static void Play() {
		if (!EditorApplication.isPlaying) {
			current = EditorSceneManager.GetActiveScene();
			if (current.path != EditorBuildSettings.scenes[0].path) {
				EditorSceneManager.OpenScene(EditorBuildSettings.scenes[0].path, OpenSceneMode.Single);
			}
			EditorApplication.isPlaying = true;
			EditorApplication.playmodeStateChanged += Stop;
		} else {
			EditorApplication.isPlaying = false;
			Stop();
		}
	}

	private static void Stop() {
		if (EditorApplication.isPlaying == false) {
			if (current != default(Scene)) {
				EditorSceneManager.OpenScene(current.path, OpenSceneMode.Single);
			}
			EditorApplication.playmodeStateChanged -= Stop;
		}
	}

}
