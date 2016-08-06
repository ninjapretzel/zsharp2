#if UNITY_EDITOR && !UNITY_WEBPLAYER
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine.UI;
#if TMPRO
using TMPro;

public class TextMeshMacros : ZEditorWindow {
	
	[UnityEditor.MenuItem("Macros/Replace Text with TextMeshPro &l")]
	public static void ReplaceTextWithPro() {
		var selection = Selection.transforms;
		foreach (var obj in selection) {
			Text text = obj.GetComponent<Text>();
			TextMesh textMesh = obj.GetComponent<TextMesh>();

			if (text != null) {
				string txt = text.text;
				var fontStyle = text.fontStyle;
				int fontSize = text.fontSize;
				float lineSpacing = text.lineSpacing;
				TextAnchor alignment = text.alignment;

				
				var gobj = text.gameObject;
				Undo.RecordObject(gobj, "Replace Texts with TextPros");

				DestroyImmediate(text);
				var tmpro = gobj.AddComponent<TextMeshProUGUI>();
				tmpro.text = txt;
				tmpro.lineSpacing = lineSpacing;
				tmpro.fontSize = fontSize;
				tmpro.fontStyle = GSS.styleMappings[fontStyle];
				tmpro.alignment = GSS.alignmentMappings[alignment];

				
			}

			if (textMesh != null) {
				var gobj = textMesh.gameObject;
				Undo.RecordObject(gobj, "Replace Texts with TextPros");
				string txt = textMesh.text;
				var fontStyle = textMesh.fontStyle;
				int fontSize = textMesh.fontSize;
				float lineSpacing = textMesh.lineSpacing;
				TextAlignment alignment = textMesh.alignment;
				
				DestroyImmediate(textMesh);
				var tmpro = gobj.AddComponent<TextMeshPro>();
				tmpro.text = txt;
				tmpro.lineSpacing = lineSpacing;
				tmpro.fontSize = fontSize;
				tmpro.fontStyle = GSS.styleMappings[fontStyle];
				if (alignment == TextAlignment.Left) { tmpro.alignment = TextAlignmentOptions.Left; }
				if (alignment == TextAlignment.Center) { tmpro.alignment = TextAlignmentOptions.Center; }
				if (alignment == TextAlignment.Right) { tmpro.alignment = TextAlignmentOptions.Right; }

				
			}

		}

		Undo.IncrementCurrentGroup();

	}

}
#endif 

#endif
