using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Builder = System.Text.StringBuilder;

public class StringBuilder {
	
	private Builder str;
	
	public StringBuilder() { str = new Builder(); }
	public StringBuilder(string s) { str = new Builder(s); }
	public StringBuilder(int cap) { str = new Builder(cap); }
	
	public static implicit operator string(StringBuilder m) { return m.ToString(); }
	
	public static StringBuilder operator + (StringBuilder a, bool b) { return a.Append(b); }
	public static StringBuilder operator + (StringBuilder a, byte b) { return a.Append(b); }
	public static StringBuilder operator + (StringBuilder a, char b) { return a.Append(b); }
	public static StringBuilder operator + (StringBuilder a, char[] b) { return a.Append(b); }
	public static StringBuilder operator + (StringBuilder a, decimal b) { return a.Append(b); }
	public static StringBuilder operator + (StringBuilder a, double b) { return a.Append(b); }
	public static StringBuilder operator + (StringBuilder a, short b) { return a.Append(b); }
	public static StringBuilder operator + (StringBuilder a, int b) { return a.Append(b); }
	public static StringBuilder operator + (StringBuilder a, long b) { return a.Append(b); }
	public static StringBuilder operator + (StringBuilder a, object b) { return a.Append(b); }
	public static StringBuilder operator + (StringBuilder a, sbyte b) { return a.Append(b); }
	public static StringBuilder operator + (StringBuilder a, float b) { return a.Append(b); }
	public static StringBuilder operator + (StringBuilder a, string b) { return a.Append(b); }
	public static StringBuilder operator + (StringBuilder a, ushort b) { return a.Append(b); }
	public static StringBuilder operator + (StringBuilder a, uint b) { return a.Append(b); }
	public static StringBuilder operator + (StringBuilder a, ulong b) { return a.Append(b); }
	
	public int Capacity { get { return str.Capacity; } set { str.Capacity = value; } }
	public int Length { get { return str.Length; } set { str.Length = value; } }
	public int MaxCapacity { get { return str.MaxCapacity; } }
	
	#region Pass-Throughs
	public StringBuilder Append(bool a) { str.Append(a); return this; }
	public StringBuilder Append(byte a) { str.Append(a); return this; }
	public StringBuilder Append(char a) { str.Append(a); return this; }
	public StringBuilder Append(char[] a) { str.Append(a); return this; }
	public StringBuilder Append(decimal a) { str.Append(a); return this; }
	public StringBuilder Append(double a) { str.Append(a); return this; }
	public StringBuilder Append(short a) { str.Append(a); return this; }
	public StringBuilder Append(int a) { str.Append(a); return this; }
	public StringBuilder Append(long a) { str.Append(a); return this; }
	public StringBuilder Append(object a) { str.Append(a); return this; }
	public StringBuilder Append(sbyte a) { str.Append(a); return this; }
	public StringBuilder Append(float a) { str.Append(a); return this; }
	public StringBuilder Append(string a) { str.Append(a); return this; }
	public StringBuilder Append(ushort a) { str.Append(a); return this; }
	public StringBuilder Append(uint a) { str.Append(a); return this; }
	public StringBuilder Append(ulong a) { str.Append(a); return this; }
	public StringBuilder Append(char a, int b) { str.Append(a, b); return this; }
	public StringBuilder Append(char[] a, int b, int c) { str.Append(a, b, c); return this; }
	public StringBuilder Append(string a, int b, int c) { str.Append(a, b, c); return this; }
	
	//public StringBuilder AppendFormat(string a, object b) { str.AppendFormat(a, b); return this; }
	public StringBuilder AppendFormat(string a, params object[] b) { str.AppendFormat(a, b); return this; }
	public StringBuilder AppendLine() { str.AppendLine(); return this; }
	public StringBuilder AppendLine(string a) { str.AppendLine(a); return this; }
	
	public StringBuilder Clear() { Length = 0; return this; }
	
	public void CopyTo(int sourceIndex, char[] dest, int destIndex, int count) {
		str.CopyTo(sourceIndex, dest, destIndex, count);
	}
	
	public int EnsureCapacity(int capacity) { return str.EnsureCapacity(capacity); }
	public override bool Equals(object other) { return str.Equals(other); }
	public bool Equals(Builder other) { return str.Equals(other); }
	public bool Equals(StringBuilder other) { return str.Equals(other.str); }
	public override int GetHashCode() { return str.GetHashCode(); }
	
	
	public StringBuilder Insert(int a, bool b) { str.Insert(a, b); return this; }
	public StringBuilder Insert(int a, byte b) { str.Insert(a, b); return this; }
	public StringBuilder Insert(int a, char b) { str.Insert(a, b); return this; }
	public StringBuilder Insert(int a, char[] b) { str.Insert(a, b); return this; }
	public StringBuilder Insert(int a, decimal b) { str.Insert(a, b); return this; }
	public StringBuilder Insert(int a, double b) { str.Insert(a, b); return this; }
	public StringBuilder Insert(int a, short b) { str.Insert(a, b); return this; }
	public StringBuilder Insert(int a, int b) { str.Insert(a, b); return this; }
	public StringBuilder Insert(int a, long b) { str.Insert(a, b); return this; }
	public StringBuilder Insert(int a, object b) { str.Insert(a, b); return this; }
	public StringBuilder Insert(int a, sbyte b) { str.Insert(a, b); return this; }
	public StringBuilder Insert(int a, float b) { str.Insert(a, b); return this; }
	public StringBuilder Insert(int a, string b) { str.Insert(a, b); return this; }
	public StringBuilder Insert(int a, ushort b) { str.Insert(a, b); return this; }
	public StringBuilder Insert(int a, uint b) { str.Insert(a, b); return this; }
	public StringBuilder Insert(int a, ulong b) { str.Insert(a, b); return this; }
	public StringBuilder Insert(int a, string b, int c) { str.Insert(a, b, c); return this; }
	public StringBuilder Insert(int a, char[] b, int c, int d) { str.Insert(a, b, c, d); return this; }
	
	public StringBuilder Remove(int start, int length) { str.Remove(start, length); return this; }
	
	public StringBuilder Replace(char a, char b) { str.Replace(a, b); return this; }
	public StringBuilder Replace(char a, char b, int c, int d) { str.Replace(a, b, c, d); return this; }
	public StringBuilder Replace(string a, string b) { str.Replace(a, b); return this; }
	public StringBuilder Replace(string a, string b, int c, int d) { str.Replace(a, b, c, d); return this; }
	
	public override string ToString() { return str.ToString(); }
	public string ToString(int a, int b) { return str.ToString(a, b); }
	
	#endregion
	
	
}
