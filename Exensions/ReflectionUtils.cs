using UnityEngine;
using System;
using System.Text;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;

///Class containing various common reflection things.
///

public static class ReflectionUtils {
	///Get the 'code' name of a type ('float' instead of 'System.Single')
	public static string ShortName(this Type t) {
		if (t == typeof(void)) { return "void"; }
		else if (t == typeof(string)) { return "string"; }
		else if (t == typeof(float)) { return "float"; }
		else if (t == typeof(bool)) { return "bool"; }
		else if (t == typeof(int)) { return "int"; }
		else if (t == typeof(double)) { return "double"; }
		else if (t == typeof(long)) { return "long"; }
		else if (t == typeof(System.Object)) { return "Object"; }
		else if (t == typeof(UnityEngine.Object)) { return "UnityEngine.Object"; }
		else if (t == typeof(Event)) { return "Event"; }
		return t.ToString().FromLast('.').Replace('+', '.');
	}
	
	///Check that an object can be cast to a type.
	public static bool CanCastTo(this System.Object o, Type target) { return target.IsAssignableFrom(o.GetType()); }
	
	///Does this object have a property called name?
	public static bool HasProperty(this System.Object obj, string name) { return obj.GetProperty(name) != null; }
	///Get this object's property called name, optionally matching a Type or BindingFlags to that property.
	public static PropertyInfo GetProperty(this System.Object obj, string name) { return obj.GetType().GetProperty(name); }
	public static PropertyInfo GetProperty(this System.Object obj, string name, Type type) { return obj.GetType().GetProperty(name, type); }
	public static PropertyInfo GetProperty(this System.Object obj, string name, BindingFlags flags) { return obj.GetType().GetProperty(name, flags); }
	
	///Does this object have a field called name?
	public static bool HasField(this System.Object obj, string name) { return obj.GetField(name) != null; }
	///Get this object's field called name, optionally matching a Type or BindingFlags to that field.
	public static FieldInfo GetField(this System.Object obj, string name) { return obj.GetType().GetField(name); }
	public static FieldInfo GetField(this System.Object obj, string name, Type type) { return obj.GetType().GetField(name, type); }
	public static FieldInfo GetField(this System.Object obj, string name, BindingFlags flags) { return obj.GetType().GetField(name, flags); }
	
	///Does this object have a method called name?
	public static bool HasMethod(this System.Object obj, string name) { return obj.GetMethod(name) != null; }
	///Does this object have a void method called name?
	public static bool HasAction(this System.Object obj, string name) { return obj.HasMethod<Void>(name); }
	
	///Does this object have a method called name, that returns a value of type <T>?
	public static bool HasMethod<T>(this System.Object obj, string name) {
		MethodInfo info = obj.GetMethod(name);
		if (info != null) {
			return (info.ReturnType == typeof(T));
		}
		return false;
	}
	
	///Less safe than CallAction(), but faster.
	///Only use this when it is KNOWN that name exists, and the constructed params match the target method's signature.
	public static void CallActionQ(this System.Object obj, string name, params System.Object[] parameters) { obj.GetMethod(name).Invoke(obj, parameters); }
	
	///Call a function on an object by name. Does not return any information.
	///Safely looks up if the function exists before calling it.
	public static void CallAction(this System.Object obj, string name, params System.Object[] parameters) {
		MethodInfo info = obj.GetMethod(name);
		if (info != null) {
			ParameterInfo[] signature = info.GetParameters();
			if (signature.Length == parameters.Length) {
				for (int i = 0; i < signature.Length; i++) {
					if (!parameters[i].CanCastTo(signature[i].ParameterType)) {
						Debug.LogWarning("ReflectionF.CallAction: Function " +  name + " on instance of " + obj.GetType().ShortName() + " does not match given parameters.");
						return;
						
					}
				}
				info.Invoke(obj, parameters);
				
			} else {
				Debug.LogWarning("ReflectionF.CallAction: Function " +  name + " on instance of " + obj.GetType().ShortName() + " does not match given parameters.");
				
			}
			
		} else {
			Debug.LogWarning("ReflectionF.CallAction: Function " + name + " on instance of " + obj.GetType().ShortName() + " does not exist.");
		}
		
	}
	
	///Get this object's method called name, optionally matching a Type or BindingFlags.
	public static MethodInfo GetMethod(this System.Object obj, string name) { return obj.GetType().GetMethod(name); }
	public static MethodInfo GetMethod(this System.Object obj, string name, Type type) { return obj.GetType().GetMethod(name, type); }
	public static MethodInfo GetMethod(this System.Object obj, string name, BindingFlags flags) { return obj.GetType().GetMethod(name, flags); }
	
	///Set an object's Property or Field by the provided name to a value
	public static bool SetObjectValue(this System.Object obj, string name, System.Object value) {
		if (obj.GetField(name) != null) {
			return obj.SetFieldValue(name, value);
		} else if (obj.GetProperty(name) != null) {
			return obj.SetPropertyValue(name, value);
		}
		
		Debug.LogWarning("ReflectionF.SetObjectValue: No field or property named " + name + " exists on instance of " + obj.GetType().ShortName());
		return false;
	}
	
	///Get an object's Property or Field value by a provided name and type.
	public static T GetObjectValue<T>(this System.Object obj, string name) {
		if (obj.GetField(name) != null) {
			return obj.GetFieldValue<T>(name);
		} else if (obj.GetProperty(name) != null) {
			return obj.GetPropertyValue<T>(name);
		}
		
		Debug.LogWarning("ReflectionF.GetObjectValue: No field or property named " + name + " exists on instance of " + obj.GetType().ShortName());
		return default(T);
	}
	
	public static System.Object GetRawObjectValue(this System.Object obj, string name) {
		if (obj.GetField(name) != null) {
			return obj.GetRawFieldValue(name); 
		} else if (obj.GetProperty(name) != null) {
			return obj.GetRawPropertyValue(name);
		}
		
		Debug.LogWarning("ReflectionF.GetRawObjectValue: No field or property named " + name + " exists on instance of " + obj.GetType().ShortName());
		return null;
	}
	
	///Get an object's Property value by a provided name and type.
	public static T GetPropertyValue<T>(this System.Object obj, string name) {
		PropertyInfo prop = obj.GetProperty(name);
		if (prop != null) {
			if (prop.PropertyType.IsAssignableFrom(typeof(T))) {
				MethodInfo method = prop.GetGetMethod();
				if (method != null) {
					return (T) method.Invoke(obj, null);
				}
				Debug.LogWarning("ReflectionF.GetPropertyValue: Property " + name + " on instance of " + obj.GetType().ShortName() + " does not have get method.");
				return default(T);
			}
			
			Debug.LogWarning("ReflectionF.GetPropertyValue: Property " + name + " on instance of " + obj.GetType().ShortName() + " does not match expected type.");
			return default(T);
		}
		
		Debug.LogWarning("ReflectionF.GetPropertyValue: Property " + name + " on instance of " + obj.GetType().ShortName() + " does not exist.");
		return default(T);
	}
	
	public static System.Object GetRawPropertyValue(this System.Object obj, string name) {
		PropertyInfo prop = obj.GetProperty(name);
		if (prop != null) {
			if (prop.PropertyType.IsAssignableFrom(typeof(System.Object))) {
				MethodInfo method = prop.GetGetMethod();
				if (method != null) {
					return method.Invoke(obj, null);
				}
				
				Debug.LogWarning("ReflectionF.GetRawPropertyValue: Property " + name + " on instance of " + obj.GetType().ShortName() + " does not have get method.");
				return null;
			}
			
			Debug.LogWarning("ReflectionF.GetRawPropertyValue: Property " + name + " on instance of " + obj.GetType().ShortName() + " does not match expected type.");
			return null;
		}
		
		Debug.LogWarning("ReflectionF.GetRawPropertyValue: Property " + name + " on instance of " + obj.GetType().ShortName() + " does not exist.");
		return null;
	}
	
	///Set an object's Property value by the provided name and type.
	public static bool SetPropertyValue(this System.Object obj, string name, System.Object value) {
		PropertyInfo prop = obj.GetProperty(name);
		if (prop != null) {
			MethodInfo method = prop.GetSetMethod();
			if (method != null) {
				if (prop.PropertyType.IsAssignableFrom(value.GetType())) {
					method.Invoke(obj, new System.Object[] { value } );
					
					return true;
				}
				
				//Debug.LogWarning("ReflectionF.SetPropertyValue: Cannot assign " + value + " to property " + name + " on instance of " + obj.GetType().ShortName());
				Debug.LogWarning("ReflectionF>SetPropertyValue: Property " + name + " on instance of " + obj.GetType().ShortName() + " does not match expected type.");
				return false;
			}
			
			Debug.LogWarning("ReflectionF.SetPropertyValue: Property " + name + " on instance of " + obj.GetType().ShortName() + " does not have set method.");
			return false;
		}
		
		Debug.LogWarning("ReflectionF.SetPropertyValue: Property " + name + " on instance of " + obj.GetType().ShortName() + " does not exist.");
		return false;
	}
	
	///Get an object's Field value by a provided name and type.
	public static T GetFieldValue<T>(this System.Object obj, string name) {
		FieldInfo field = obj.GetField(name);
		if (field != null) {
			if (field.FieldType.IsAssignableFrom(typeof(T))) {
				return (T) field.GetValue(obj);
			}
			
			Debug.LogWarning("ReflectionF.GetFieldValue: Field " + name + " on instance of " + obj.GetType().ShortName() + " does not match expected type.");
			return default(T);
		}
		
		Debug.LogWarning("ReflectionF.GetFieldValue: Field " + name + " on instance of " + obj.GetType().ShortName() + " does not exist.");
		return default(T);
	}
	
	public static System.Object GetRawFieldValue(this System.Object obj, string name) {
		FieldInfo field = obj.GetField(name);
		if (field != null) {
			if (field.FieldType.IsAssignableFrom(typeof(System.Object))) {
				return field.GetValue(obj);
			}
			
			Debug.LogWarning("ReflectionF.GetRawFieldValue: Field " + name + " on instance of " + obj.GetType().ShortName() + " does not match expected type.");
			return null;
		}
		Debug.LogWarning("ReflectionF.GetRawFieldValue: Field " + name + " on instance of " + obj.GetType().ShortName() + " does not exist.");
		return null;
	}
	
	///Set an object's Property value by the provided name and type.
	public static bool SetFieldValue(this System.Object obj, string name, System.Object value) {
		FieldInfo field = obj.GetField(name);
		if (field != null) {
			if (field.FieldType.IsAssignableFrom(value.GetType())) {
				field.SetValue(obj, value);
				return true;
			}
			
			Debug.LogWarning("ReflectionF.SetFieldValue: Field " + name + " on instance of " + obj.GetType().ShortName() + " does not match expected type.");
			return true;
		}
		
		Debug.LogWarning("ReflectionF.SetFieldValue: Field " + name + " on instance of " + obj.GetType().ShortName() + " does not exist.");
		return false;
	}
	
	///Check if some MemberInfo is inherited or not;
	public static bool IsInherited(this MemberInfo info) { return info.DeclaringType != info.ReflectedType; }
	
	///
	public static bool IsPublic(this PropertyInfo info) {
		MethodInfo getter = info.GetGetMethod();
		MethodInfo setter = info.GetSetMethod();
		if (getter == null && setter == null) { return false; }
		
		if (getter != null && setter != null) {
			return getter.IsPublic && setter.IsPublic;
			
		} else if (setter == null) {
			return getter.IsPublic;
		} else {
			return setter.IsPublic;
		}
	}
	public static bool IsPrivate(this PropertyInfo info) {
		MethodInfo getter = info.GetGetMethod();
		MethodInfo setter = info.GetSetMethod();
		if (getter == null && setter == null) { return false; }
		
		if (getter != null && setter != null) {
			return getter.IsPrivate && setter.IsPrivate;
			
		} else if (setter == null) {
			return getter.IsPrivate;
		} else {
			return setter.IsPrivate;
		}
	}
	
	public static bool IsStatic(this PropertyInfo info) {
		MethodInfo getter = info.GetGetMethod();
		MethodInfo setter = info.GetSetMethod();
		if (getter == null && setter == null) { return false; }
		
		if (getter != null && setter != null) {
			return getter.IsStatic && setter.IsStatic;
			
		} else if (setter == null) {
			return getter.IsStatic;
		} else {
			return setter.IsStatic;
		}
	}
	
	
	
	public static void ListAllMembers(this Type type, bool showPrivate = false, bool showHidden = false) { Debug.Log(type.Summary(showPrivate, showHidden)); }
	public static string Summary(this Type type, bool showPrivate = false, bool showHidden = false) {
		//BindingFlags allPublic = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.SetField | BindingFlags.GetField | BindingFlags.GetProperty | BindingFlags.SetProperty;
		BindingFlags flags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public;
		if (showPrivate) {
			flags = flags | BindingFlags.NonPublic;
		}
		
		List<FieldInfo> fields = type.GetFields(flags).ToList();
		
		List<ConstructorInfo> constructors = type.GetConstructors(flags).ToList();
		
		List<PropertyInfo> properties = type.GetProperties(flags).ToList();
		
		List<MethodInfo> methods = type.GetMethods(flags).ToList();
		
		fields.Sort(CompareFields);
		methods.Sort(CompareMethods);
		
		Type lastInheritedType = null;
		
		StringBuilder output = new StringBuilder("");
		
		if (type.IsPublic) {
			output.Append("public ");
		}
		
		if (type.IsInterface) {
			output.Append("interface ");
		} else {
			if (type.IsAbstract) {
				output.Append("abstract ");
			}
			output.Append("class ");
		}
		
		output.Append(type.ShortName());
		
		
		Type[] interfaces = type.GetInterfaces();
		if (type.BaseType == typeof(System.Object) || type.BaseType == null) {
			if (interfaces.Length > 0) {
				output.Append(" : ");
			}
		} else {
			output.Append(" : " + type.BaseType.ShortName());
			if (interfaces.Length > 0) {
				output.Append(", ");
			}
			
		}

		for (int i = 0; i < interfaces.Length; i++) {
			output.Append(interfaces[i].ShortName());
			if (i != interfaces.Length-1) { 
				output.Append(", ");
			}
			
		}
		
		
		
		
		output.Append(" {");
		
		output.Append("\n\n\t//Fields:----------------------------------------------\n");
		foreach (FieldInfo info in fields) {
			if (info.IsInherited()) {
				if (info.DeclaringType != lastInheritedType) {
					lastInheritedType = info.DeclaringType;
					output.Append("\n\t//Inherited from <" + info.DeclaringType.ToString() + ">\n");
					
				}
			}
			output.Append("\t" + info.Summary() + "\n");
		}
		
		lastInheritedType = null;
		output.Append("\n\n\t//Properties:----------------------------------------------\n");
		foreach (PropertyInfo info in properties) {
			if (info.IsInherited()) {
				if (info.DeclaringType != lastInheritedType) {
					lastInheritedType = info.DeclaringType;
					output.Append("\n\t//Inherited from <" + info.DeclaringType.ToString() + ">\n");
					
				}
			}
			output.Append("\t" + info.Summary() + "\n");
		}
		
		output.Append("\n\n\t//Constructors:----------------------------------------------\n");
		foreach (ConstructorInfo info in constructors) {
			output.Append("\t" + info.Summary() + "\n");
		}
		
		
		//Give summary of each method info
		lastInheritedType = null;
		output.Append("\n\n\t//Methods:----------------------------------------------\n");
		foreach (MethodInfo info in methods) {
			if (info.IsSpecialName && !showHidden) { continue; }
			if (info.IsInherited()) {
				if (info.DeclaringType != lastInheritedType) {
					lastInheritedType = info.DeclaringType;
					output.Append("\n\t//Inherited from <" + info.DeclaringType.ToString() + ">\n");
					
				}
			}
			output.Append("\t" + info.Summary() + "\n");
		}
		
		
		output.Append("\n}");
		
		return output.ToString();
		
		
		
		
	}
	
	public static int CompareMethods(this MethodInfo info, MethodInfo other) {
		if (info.IsInherited() == other.IsInherited()) {
			if (info.DeclaringType == other.DeclaringType) {
				if (info.IsStatic == other.IsStatic) { 
					if (info.IsPublic == other.IsPublic) {
						if (info.IsPrivate == other.IsPrivate) {
							if (info.IsAbstract == other.IsAbstract) {
								if (info.IsVirtual == other.IsVirtual) {
									if (info.ReturnType == other.ReturnType) {
										
										return 0;
										
									} else { return info.ReturnType.ShortName().CompareTo(other.ReturnType.ShortName()); }
								} else { return (info.IsVirtual ? 1 : -1); }
							} else { return (info.IsAbstract ? 1 : -1); }
						} else { return (info.IsPrivate ? -1 : 1); }
					} else { return (info.IsPublic ? -1 : 1); }
				} else { return (info.IsStatic ? -1 : 1); }
			} else { return info.DeclaringType.ShortName().CompareTo(other.DeclaringType.ShortName()); }
		} else { return (info.IsInherited() ? 1 : -1); }
		//return 0;
	}
	
	public static int CompareFields(this FieldInfo info, FieldInfo other) {
		if (info.IsInherited() == other.IsInherited()) {
			if (info.DeclaringType == other.DeclaringType) {
				if (info.IsStatic == other.IsStatic) { 
					if (info.IsPublic == other.IsPublic) {
						if (info.IsNotSerialized == other.IsNotSerialized) {
							if (info.IsPrivate == other.IsPrivate) {
								
								return 0;
								
							} else { return (info.IsPrivate ? -1 : 1); }
						} else { return (info.IsNotSerialized ? -1 : 1); }
					} else { return (info.IsPublic ? -1 : 1); }
				} else { return (info.IsStatic ? -1 : 1); }
			} else { return info.DeclaringType.ShortName().CompareTo(other.DeclaringType.ShortName()); }
		} else { return (info.IsInherited() ? 1 : -1); }
	}
	
	public static string Summary(this ConstructorInfo info) {
		StringBuilder str = new StringBuilder();
		
		if (info.IsPublic) {
			str.Append("public ");
		} else if (info.IsPrivate) {
			str.Append("private ");
		} 
		
		if (info.IsFamily) {
			str.Append("protected ");
		}
		if (info.IsAssembly) {
			str.Append("internal ");
		}
		
		
		str.Append(info.DeclaringType.Name + "(");
		
		ParameterInfo[] pinfos = info.GetParameters();
		for (int i = 0; i < pinfos.Length; i++) {
			ParameterInfo pinfo = pinfos[i];
			
			
			if (pinfo.IsOut) { str.Append("out "); }
			str.Append(pinfo.ParameterType.ShortName() + " " + pinfo.Name);
			/*
			//Unity's Mono does not have this functionality... :(
			if (pinfo.HasDefaultValue) {
				str.Append(" = " + pinfo.DefaultValue.ToString().RemoveAll("\n"));
			}
			//*///
			
			if (i < pinfos.Length-1) { str.Append(", "); }
			
		}
		
		str.Append(");");
		
		return str.ToString();
	}
	
	public static string Summary(this MethodInfo info) {
		StringBuilder str = new StringBuilder();
		
		
		
		if (info.IsPublic) {
			str.Append("public ");
		} else if (info.IsPrivate) {
			str.Append("private ");
		} 
		
		if (info.IsFamily) {
			str.Append("protected ");
		}
		if (info.IsAssembly) {
			str.Append("internal ");
		}
		
		if (info.IsSpecialName) {
			str.Append("hidden ");
		}
		
		
		if (info.IsStatic) {
			str.Append("static ");
		} else {
			
			if (info.IsVirtual) {
				str.Append("virtual ");
			}
			if (info.IsAbstract) {
				str.Append("abstract ");
			}
		}
		str.Append(info.ReturnType.ShortName() + " " + info.Name + "(");
		
		ParameterInfo[] pinfos = info.GetParameters();
		for (int i = 0; i < pinfos.Length; i++) {
			ParameterInfo pinfo = pinfos[i];
			
			
			if (pinfo.IsOut) { str.Append("out "); }
			str.Append(pinfo.ParameterType.ShortName() + " " + pinfo.Name);
			/*
			//Unity's Mono does not have this functionality... :(
			if (pinfo.HasDefaultValue) {
				str.Append(" = " + pinfo.DefaultValue.ToString().RemoveAll("\n"));
			}
			//*///
			
			if (i < pinfos.Length-1) { str.Append(", "); }
			
		}
		
		str.Append(");");
		
		return str.ToString();
	}
	
	public static string Summary(this PropertyInfo info) {
		StringBuilder str = new StringBuilder("");
		
		if (info.IsPublic()) {
			str.Append("public ");
		} else if (info.IsPrivate()) {
			str.Append("private ");
		} 
		
		/*
		if (info.IsFamily) {
			str.Append("protected ");
		}
		if (info.IsAssembly) {
			str.Append("internal ");
		}
		*/
		
		if (info.IsSpecialName) {
			str.Append("hidden ");
		}
		
		
		
		if (info.IsStatic()) {
			str.Append("static ");
		} else {
			
		}
		
		str.Append(info.PropertyType.ShortName() + " " + info.Name + " {");
		
		MethodInfo getter = info.GetGetMethod();
		MethodInfo setter = info.GetSetMethod();
		
		if (getter != null) { str.Append(" get; "); }
		if (setter != null) { str.Append(" set; "); }
		
		str.Append("}");
		
		
		return str.ToString();
	}
	
	//[ZDoc("Creates A summary of a FieldInfo as an extension method.")]
	public static string Summary(this FieldInfo info) {
		StringBuilder str = new StringBuilder("");
		
		if (info.IsNotSerialized) {
			str.Append("[System.NotSerialized] ");
		}
		
		if (info.IsPublic) {
			str.Append("public ");
		} else if (info.IsPrivate) {
			str.Append("private ");
		} 
		
		
		if (info.IsFamily) {
			str.Append("protected ");
		}
		if (info.IsAssembly) {
			str.Append("internal ");
		}
		
		if (info.IsSpecialName) {
			str.Append("hidden ");
		}
		
		
		
		if (info.IsStatic) {
			str.Append("static ");
		} else {
			
		}
		
		str.Append(info.FieldType.ShortName() + " " + info.Name + ";");
		
		
		
		return str.ToString();
	}
	
	
	
}
