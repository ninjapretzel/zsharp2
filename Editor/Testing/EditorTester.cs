#if UNITY_EDITOR && !UNITY_WEBPLAYER
using UnityEngine;
using Shouldly;
using System;
using System.Linq;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
sealed class ZTest : Attribute { }

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
sealed class ZTestTests : Attribute { }

public class EditorTester : ZEditorWindow {

	public const string PASS = "Pass!";
	public const string FAILED = "Failed";
	public const string UNEXPECTED_FAIL = "Unexpected Exception";

	public static Color orange = new Color(1, .4f, .1f);

	private class TestResult {
		public string name;
		
		public string status;
		public string message;
		public string timeMsg;

		public Thread thread;
		public bool expanded;


		public bool done { get { return !thread.IsAlive; } }

		public bool passed { get { return status.StartsWith(PASS); } }
		public bool failed { get { return status.StartsWith(FAILED) || status.StartsWith(UNEXPECTED_FAIL); } }
		


		public TestResult(MethodInfo mi, string n = null) {
			name = n;
			if (name == null) { name = mi.Name; }
			expanded = false;
			status = "Not Started";
			message = "";
			timeMsg = "";
			long before = DateTime.Now.Ticks;

			ThreadStart ts = new ThreadStart(() => {
				status = "Running";

				try {
					mi.Invoke(null, null);
					status = PASS;
				} catch (Exception e) {
					if (e.InnerException.GetType() == typeof(ShouldAssertException)) {
						status = FAILED;
						message = e.InnerException.Message;
					} else {
						status = UNEXPECTED_FAIL;
						message = e.InnerException.GetType() + "\n" + e.InnerException.Message; 
					}
					message += "\n" + e.InnerException.StackTrace;
					expanded = true;
				}
				
				long after = DateTime.Now.Ticks;
				long diff = after - before;
				TimeSpan dtime = new TimeSpan(diff);

				timeMsg = "Completed in " + dtime.TotalMilliseconds + "ms";

			});
			thread = new Thread(ts);
			thread.Start();
			

		}

	}


	[MenuItem("ZSharp/Testing/Testing Window")]
	public static void ShowWindow() {
		EditorWindow.GetWindow(typeof(Tester));
	}

	List<TestResult> testResults;
	Vector2 scroll;
	public bool runTestTests = false;
	bool wasCompiling = false;
	DateTime lastRun;
	float timer = 0;

	void Start() {
		titleContent = new GUIContent("Tester");
		RunTests();

		
		
	}

	void Update() {
		if (wasCompiling && !EditorApplication.isCompiling) {
			RunTests();
		}
		
		wasCompiling = EditorApplication.isCompiling;
		timer += 1;
		if (timer > 10) {
			timer = 0;
			Repaint();
		}

	}


	void OnGUI() {
		GUI.skin = Resources.Load<GUISkin>("Standard");
		GUI.Box(new Rect(0, 0, width, height), "");
		if (testResults == null) {
			if (Button("Run Tests", Height(height))) { RunTests(); }
			return; 
		}

		int totalTests = testResults.Count;
		int doneTests = testResults.Where((t) => t.done).Count();
		//int runningTests = totalTests - doneTests;

		int passedTests = testResults.Where((t) => t.passed).Count();
		int failedTests = testResults.Where((t) => t.failed).Count();

		Box("Last Run: " + lastRun);
		if (Button("Re-run tests")) { 
			RunTests();
		}

		GUI.color = Color.white;
		BeginHorizontal("box"); {
			Label("Tests Run: " + doneTests + " / " + totalTests);
			GUI.color = Color.green;
			Label("Passed: " + passedTests + " / " + totalTests);
			GUI.color = orange;
			Label("Failed: " + failedTests + " / " + totalTests);
			GUI.color = Color.white;
		} EndHorizontal();

		scroll = BeginScrollView(scroll, false, false, Width(Screen.width) ); {

			BeginVertical("box", Width(Screen.width - 24)); {
				foreach (var result in testResults) {
					Draw(result);
				}


			} EndVertical();
		} EndScrollView();
	}

	void Draw(TestResult result) {
		GUI.color = Color.yellow;
		if (result.status.StartsWith(PASS)) { GUI.color = Color.green; }
		if (result.status.StartsWith(FAILED)) { GUI.color = Color.red; }
		if (result.status.StartsWith(UNEXPECTED_FAIL)) { GUI.color = orange; }

		BeginVertical("box"); {
			BeginHorizontal(); {
				if (Button(result.expanded ? "V" : ">", Width(30))) { result.expanded = !result.expanded; }
				Label(result.name);
				FlexibleSpace();
				Label(result.status);
			} EndHorizontal();

			if (result.expanded){
				Label(result.timeMsg);
				if (result.message.Length > 0) {
					Label(result.message);
				}
			}
			
		} EndVertical();

	}
	

	List<MethodInfo> GetTestMethods() {
		Assembly assem = this.GetType().Assembly;
		Type[] types = assem.GetTypes();
		List<Type> testTypes = new List<Type>();
		List<MethodInfo> testMethods = new List<MethodInfo>();
		foreach (var t in types) {
			var atts = t.GetCustomAttributes(false);
			bool add = false;
			foreach (var att in atts) {
				if (typeof(ZTest).IsAssignableFrom(att.GetType())) { add = true; break; }

				if (runTestTests) {
					if (typeof(ZTestTests).IsAssignableFrom(att.GetType())) { add = true; break; }
				}
			}
			if (add) { testTypes.Add(t); }
		}

		foreach (var t in testTypes) {
			var methods = t.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

			foreach (var m in methods) {
				var atts = m.GetCustomAttributes(false);
				bool add = false;
				foreach (var att in atts) {
					if (typeof(ZTest).IsAssignableFrom(att.GetType())) { add = true; break; }
				}
				if (add) { testMethods.Add(m); }
			}

		}
		return testMethods;
	}
	
	void RunTests() {
		lastRun = DateTime.Now;
		testResults = new List<TestResult>();
		var methods = GetTestMethods();
		
		foreach (var method in methods) {
			testResults.Add(new TestResult(method));

		}

	}


	
}

[ZTestTests]
public class TestsTests {
	
	[ZTest] public static void TestShouldPass() {
		int x = 3;
		x *= x;

		x.ShouldBe(9);
		x.ShouldNotBe(20);

	}

	[ZTest] public static void TestShouldFail() {
		int x = 5;
		x.ShouldNotBe(5);
		x.ShouldNotBe(0);

	}

	[ZTest] public static void TestShouldFailUnexpectedly() {
		throw new InvalidOperationException("Stay calm, it is just a test");
	}	


}
#endif
