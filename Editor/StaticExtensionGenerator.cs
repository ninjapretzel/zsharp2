#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Reflection;
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Text;

public class StaticExtensionGenerator : EditorWindow {

	public static string path { get { return Application.dataPath + "/Standard Assets/"; } }

	private string parentClassAlias;
	public Type type = null;
	public string typeName = "UnityEngine.Physics,UnityEngine";

	[MenuItem("ZSharp/Windows/Static Class Extension Generator")]
	public static void ShowWindow() {
		StaticExtensionGenerator main = (StaticExtensionGenerator)EditorWindow.GetWindow(typeof(StaticExtensionGenerator));
		main.minSize = new Vector2(300, 20);
		main.autoRepaintOnSceneChange = true;
		UnityEngine.Object.DontDestroyOnLoad(main);
		main.Start();
	}

	public void Start() {
		
	}

	public void OnGUI() {
		typeName = GUILayout.TextField(typeName);
		if(GUILayout.Button("Generate class")) {
			type = Type.GetType(typeName);
			parentClassAlias = "Original" + type.Name;
			if(File.Exists(path + type.Name + ".cs")) {
				File.Delete(path + type.Name + ".cs");
			}
			byte[] newClassBytes = Encoding.ASCII.GetBytes(GenerateWrapperClass(type));
			FileStream fs = File.Create(path + type.Name + ".cs");
			fs.Write(newClassBytes, 0, newClassBytes.Length);
			fs.Close();
			Debug.Log("class written to " + (path + type.Name + ".cs"));
		}
	}

	public string GenerateWrapperClass(Type type) {
		StringBuilder result = new StringBuilder();
		result.Append("using UnityEngine;\n\nusing Original" + type.Name + " = " + type.FullName + ";\n\npublic static class " + type.Name + " {\n\n");
		foreach(FieldInfo finfo in type.GetFields(BindingFlags.Public | BindingFlags.Static)) {
			if(!IsObsolete(finfo)) {
				result.Append("\t");
				result.Append(GenerateFieldWrapper(finfo));
				result.Append("\n");
			}
		}
		result.Append("\n");
		foreach(PropertyInfo pinfo in type.GetProperties(BindingFlags.Public | BindingFlags.Static)) {
			if(!IsObsolete(pinfo)) {
				result.Append("\t");
				result.Append(GeneratePropertyWrapper(pinfo));
				result.Append("\n");
			}
		}
		result.Append("\n");
		foreach(MethodInfo minfo in type.GetMethods(BindingFlags.Public | BindingFlags.Static)) {
			if(!IsObsolete(minfo) && !minfo.IsSpecialName) {
				result.Append("\t");
				result.Append(GenerateMethodWrapper(minfo));
				result.Append("\n");
			}
		}
		result.Append("}");
		return result.ToString();
	}

	public string GenerateFieldWrapper(FieldInfo fieldInfo) {
		StringBuilder result = new StringBuilder();
		//result.Append(GenerateCustomAttributes(fieldInfo));
		result.Append("public static " + Primitivify(fieldInfo.FieldType.Name) + " " + fieldInfo.Name + " { ");
		result.Append("get { return " + parentClassAlias + "." + fieldInfo.Name + ";" + " } ");
		if(!fieldInfo.IsLiteral) {
			result.Append("set { " + parentClassAlias + "." + fieldInfo.Name + " = value; } ");
		}
		result.Append("}");
		return result.ToString();
	}

	public string GeneratePropertyWrapper(PropertyInfo propertyInfo) {
		StringBuilder result = new StringBuilder();
		//result.Append(GenerateCustomAttributes(propertyInfo));
		result.Append("public static " + Primitivify(propertyInfo.PropertyType.Name) + " " + propertyInfo.Name + " { ");
		if(propertyInfo.GetGetMethod() != null) {
			result.Append("get { return " + parentClassAlias + "." + propertyInfo.Name + ";" + " } ");
		}
		if(propertyInfo.GetSetMethod() != null) {
			result.Append("set { " + parentClassAlias + "." + propertyInfo.Name + " = value; } ");
		}
		result.Append("}");
		return result.ToString();
	}

	public string GenerateMethodWrapper(MethodInfo methodInfo) {
		StringBuilder result = new StringBuilder();
		//result.Append(GenerateCustomAttributes(methodInfo));
		result.Append("public static " + Primitivify(methodInfo.ReturnType.Name) + " " + methodInfo.Name);
		if(methodInfo.IsGenericMethod) {
			result.Append(GenerateGenericParameterList(methodInfo.GetGenericArguments()));
		}
		result.Append("(");
		ParameterInfo[] parameters = methodInfo.GetParameters();
		for(int i = 0; i < parameters.Length; i++) {
			if(parameters[i].IsIn) {
				result.Append("in ");
			}
			if(parameters[i].IsOut) {
				result.Append("out ");
			}
			Type parameterType = parameters[i].ParameterType;
			if(parameterType.IsGenericType) {
				result.Append(parameterType.Name + " ");
			} else {
				result.Append(Primitivify(parameterType.Name) + " ");
			}
			result.Append(parameters[i].Name);
			if(parameters[i].IsOptional) {
				result.Append(" = ");
				if(parameters[i] == null) {
					result.Append("null");
				} else {
					result.Append(parameters[i].DefaultValue.ToString());
				}
			}
			if(i != parameters.Length - 1) {
				result.Append(", ");
			}
		}
		result.Append(") { ");
		if(methodInfo.ReturnType != typeof(void)) {
			result.Append("return ");
		}
		result.Append(parentClassAlias + "." + methodInfo.Name);
		if(methodInfo.IsGenericMethod) {
			result.Append(GenerateGenericParameterList(methodInfo.GetGenericArguments()));
		}
		result.Append("(");
		for(int i = 0; i < parameters.Length; i++) {
			if(parameters[i].IsIn) {
				result.Append("in ");
			}
			if(parameters[i].IsOut) {
				result.Append("out ");
			}
			result.Append(parameters[i].Name);
			if(i != parameters.Length - 1) {
				result.Append(", ");
			}
		}
		result.Append("); }");
		return result.ToString();
	}

	public string GenerateCustomAttributes(MemberInfo info) {
		StringBuilder result = new StringBuilder();
		foreach(object attribute in info.GetCustomAttributes(true)) {
			if(attribute.GetType().IsPublic) {
				string attributeName = attribute.ToString();
				result.Append("[" + attributeName.Substring(0, attributeName.Length - 9) + "]");
			}
		}
		return result.ToString();
	}

	public string GenerateGenericParameterList(Type[] genericArguments) {
		StringBuilder result = new StringBuilder();
		result.Append("<");
		for(int i = 0; i < genericArguments.Length; i++) {
			Type genericType = genericArguments[i];
			result.Append(genericType.Name);
			if(i != genericArguments.Length - 1) {
				result.Append(", ");
			}
		}
		result.Append(">");
		return result.ToString();
	}

	public bool IsObsolete(MemberInfo info) {
		Attribute[] attrs = Attribute.GetCustomAttributes(info);
		foreach(Attribute attr in attrs) {
			if(attr is ObsoleteAttribute) {
				return true;
			}
		}
		return false;
	}

	public string Primitivify(string type) {
		return type.Replace("Boolean", "bool")
		           .Replace("SByte", "sbyte")
		           .Replace("Byte", "byte")
		           .Replace("Int16", "short")
		           .Replace("UInt16", "ushort")
		           .Replace("Int32", "int")
		           .Replace("UInt32", "uint")
		           .Replace("Int64", "long")
					  .Replace("UInt64", "ulong")
					  .Replace("Single", "float")
		           .Replace("Double", "double")
		           .Replace("String", "string")
		           .Replace("Char", "char")
		           .Replace("Void", "void")
		           .TrimEnd('&');
	}

}
#endif
