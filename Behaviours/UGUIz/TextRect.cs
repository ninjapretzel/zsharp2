using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class TextRect : MonoBehaviour {

	void Update() {
		RectTransform rect = GetComponent<RectTransform>();
		Text text = GetComponent<Text>();

		rect.sizeDelta = new Vector2(rect.sizeDelta.x, text.preferredHeight);
		
	}
	
}
