using UnityEngine;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class RefTest {
	public static string thingy = "Hullo everynyan";
	public static string thingyProperty { get { return thingy; } }
	public static string GetThingy() { return thingy; }

}

public class TextMeshReflectionFill : MonoBehaviour {

	object lastValue = null;

    private Component targetComponent = null;

	public GameObject targetObject = null;
    public string target = "RefTest.thingy";
    private FieldInfo field = null;
    private MethodInfo method = null;
    private PropertyInfo prop = null;

	string[] assemblies = new string[] {
		",UnityEngine",
		",Assembly-UnityScript",
		",Assembly-CSharp",
		",Assembly-UnityScript-firstpass",
        ",Assembly-CSharp-firstpass",
	};

	void Start() {
		Type targetClass = null;
		//string[] splits = target.Split('.');
		string targetName = target.UpToLast('.');
		string targetThing = target.FromLast('.');
		
		foreach (string assembly in assemblies) {
			targetClass = System.Type.GetType(targetName + assembly);
			if (targetClass != null) { break; }
		}

		if (targetClass == null) {
			Debug.LogWarning("TextMeshReflectionFill.Start: Could not find class by the name of " + targetName);
			return;
		}
        if (targetObject != null && typeof(Component).IsAssignableFrom(targetClass)) {
            targetComponent = targetObject.GetComponent(targetClass);
        }

        field = targetClass.GetField(targetThing);
        method = targetClass.GetMethod(targetThing);
        if ((method != null) && (method.GetParameters().Length != 0)) {
            Debug.LogWarning("TextMeshReflectionFill.Start: " + target + " should not have parameters.");
            return;
        }
        prop = targetClass.GetProperty(targetThing);
        if ((field == null) && (method == null) && (prop == null)) {
            Debug.LogWarning("TextMeshReflectionFill.Start: Could not find property/field/method by name of " + targetThing);
            return;
        }

	}

    void Update() {
        if ((field == null) && (method == null) && (prop == null)) {
            return;
        }
        object value = null;
        if (field != null) {
            value = field.GetValue(targetComponent);
        }
        if (method != null) {
            value = method.Invoke(targetComponent,null);
        }
        if (prop != null) {
            MethodInfo propMethod = prop.GetGetMethod();
            value = propMethod.Invoke(targetComponent,null);
        }
        if (value != lastValue) {
            GetComponent<Text>().text = value.ToString();
        }
    }
	
	
}
