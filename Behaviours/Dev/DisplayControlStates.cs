using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class DisplayControlStates : ZBehaviour {

#if DEVELOPMENT_BUILD || UNITY_EDITOR
	void OnGUI() {
		GUI.skin = Resources.Load<GUISkin>("standard");
		GUI.color = Color.white.Alpha(.5f);

		GUI.Box(Screen.Left(.5f), "");
		BeginVertical("box", Width(Screen.width / 2f), Height(Screen.height)); {
			int i = 0;
			foreach (var pair in ControlStates.GetAll()) {
				GUI.color = ((i++ % 2 == 0) ? Color.grey : Color.white).Alpha(.5f);

				BeginHorizontal("box"); {
					GUI.color = Color.white;
					Label(pair.Key);
					FlexibleSpace();
					Label(pair.Value);

				} EndHorizontal();

			}

		} EndVertical(); 


	}
#endif 

}
