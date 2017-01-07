using UnityEngine;
using Shouldly;
using System;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

/// <summary> Used to mark classes that hold the normal tests that should be run, as well as methods within those classes that serve as test methods. </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
sealed class Test : Attribute { }

/// <summary> Used to mark the tests for testing the tester. Very Testy. </summary>
[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = false, AllowMultiple = true)]
sealed class TestTests : Attribute { }

public class Tester : ZBehaviour {

	private class TestClassResult {
		/// <summary> Name of the type this results object represents </summary>
		public string name { get; private set; }
		/// <summary> Current status of this test result. </summary>
		public string status { get; private set; }
		/// <summary> Results of individual test methods </summary>
		public TestResult[] tests;
		/// <summary> Number of tests that passed. </summary>
		public int numPassed;

		public int numTests { get { return tests.Length; } }

		public bool expanded;

		public string progMsg { get; private set; }
		public string timeMsg { get; private set; }

		public bool done { get; private set; }

		public TestClassResult(Type type) {
			var testMethods = GetTestMethods(type);

			tests = new TestResult[testMethods.Count];
			name = type.FullName;
			status = "Not Started";
			numPassed = 0;
			expanded = false;

			for (int i = 0; i < testMethods.Count; i++) {
				tests[i] = new TestResult(testMethods[i]);
			}
		}

		public void Run() {
			
			ThreadStart st = new ThreadStart(()=>{
				status = "Started";
				long before = DateTime.Now.Ticks;
				long now;
				long diff;
				TimeSpan dtime;

				numPassed = 0;
				expanded = false;
				int run = 0;
				foreach (var test in tests) {
					test.Run();
					if (test.passed) { numPassed++; }
					else { expanded = true; }
					progMsg = "( " + numPassed + " / " + (++run) + " / " + numTests + " )";
					now = DateTime.Now.Ticks;
					diff = now - before;
					dtime = new TimeSpan(diff);
					timeMsg = progMsg + " in " + dtime.TotalMilliseconds + "ms";

					if (run == numTests) {
						if (numPassed == numTests) {
							status = "All Passed!";
						} else {
							status = "Failure!";
						}

					}
				}
				now = DateTime.Now.Ticks;
				diff = now - before;
				dtime = new TimeSpan(diff);
				

				done = true;

			});
			Thread thread = new Thread(st);
			thread.Start();

		}

	}

	private class TestResult {
		/// <summary> Name of method </summary>
		public string name;

		// <summary> The class this TestResult belongs to </summary>
		//public TestClassResult parent;

		/// <summary> Status of test (Running, pass, fail, etc) </summary>
		public string status;
		/// <summary> Internal message of test (Exception, line numbers, etc) </summary>
		public string message;
		/// <summary> How long did the test run for? </summary>
		public string timeMsg;

		/// <summary> Is the result expanded? </summary>
		public bool expanded;
		/// <summary> Info object representing the method to call. </summary>
		public MethodInfo testMethod;

		/// <summary> Has the test been started, and is the test done? </summary>
		public bool done { get; private set; }
		public bool passed { get; private set; } 

		public TestResult(MethodInfo mi, string n = null) {
			name = n;
			if (name == null) { name = mi.Name; }

			expanded = false;
			status = "Not Started";
			done = false;
			message = "";
			timeMsg = "";
			testMethod = mi;
			

		}

		public bool Run() {
			long before = DateTime.Now.Ticks;
			status = "Running";
			done = false;
			passed = false;

			try {
				testMethod.Invoke(null, null);
				status = "Pass!";
				passed = true;
			} catch (Exception e) {
				if (e.InnerException.GetType() == typeof(ShouldAssertException)) {
					status = "Failed (Assertion Failed)";
					message = e.InnerException.Message;
				} else {
					status = "Failed (Unexpected Exception)";
					message = e.InnerException.GetType() + "\n" + e.InnerException.Message;
				}
				message += "\n" + e.InnerException.StackTrace;
				expanded = true;
			}
			long after = DateTime.Now.Ticks;
			long diff = after - before;
			TimeSpan dtime = new TimeSpan(diff);

			done = true;
			timeMsg = "Completed in " + dtime.TotalMilliseconds + "ms";
			return passed;
		}

	}

	/// <summary> Finds all of the classes that are marked with the Test type attribute </summary>
	/// <returns> a List of Type objects found in the current assembly </returns>
	private static List<Type> GetTestClasses() {
		Assembly assem = typeof(Tester).Assembly;
		Type[] types = assem.GetTypes();
		List<Type> testTypes = new List<Type>();
		foreach (var t in types) {
			var atts = t.GetCustomAttributes(false);
			bool add = false;
			foreach (var att in atts) {
				if (typeof(Test).IsAssignableFrom(att.GetType())) { add = true; break; }

				if (runTestTests) {
					if (typeof(TestTests).IsAssignableFrom(att.GetType())) { add = true; break; }
				}
			}
			if (add) { testTypes.Add(t); }
		}
		return testTypes;
	}

	private static List<MethodInfo> GetTestMethods(Type type) {
		List<MethodInfo> testMethods = new List<MethodInfo>();
		
		var methods = type.GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);

		foreach (var m in methods) {
			var atts = m.GetCustomAttributes(false);
			bool add = false;
			foreach (var att in atts) {
				if (typeof(Test).IsAssignableFrom(att.GetType())) { add = true; break; }
			}
			if (add) { testMethods.Add(m); }
		}

		return testMethods;
	}

	void RunTests() {
		testClassResults = new List<TestClassResult>();
		
		var testClasses = GetTestClasses();
		foreach (var testClass in testClasses) {
			TestClassResult tcr = new TestClassResult(testClass);

			testClassResults.Add(tcr);
			tcr.Run();
		}



	}

	List<TestClassResult> testClassResults;
	Vector2 scroll;
	public bool runTesterTests = false;
	public static bool runTestTests { get { return main.runTesterTests; } }
	public static Tester main;

	void Awake() {
		if (main != null) {
			Destroy(gameObject);
			return;
		} 
		main = this;
		
		

	}

	void Start() {
		RunTests();
		
	}


	void OnGUI() {
		if (testClassResults == null) { return; }
		GUI.skin = Resources.Load<GUISkin>("Standard");
		GUI.Box(Screen.all, "");
		scroll = BeginScrollView(scroll, false, false, Width(Screen.width) ); {
			BeginVertical("box", Width(Screen.width - 24)); {
				foreach (var result in testClassResults) {
					Draw(result);
				}


			} EndVertical();
		} EndScrollView();
	}

	void Draw(TestClassResult result) {
		GUI.color = Color.yellow;
		if (result.done && result.numPassed < result.numTests) { GUI.color = Color.red; }
		if (result.numPassed == result.numTests) { GUI.color = Color.green; }

		BeginVertical("box"); {
			BeginHorizontal(); {
				if (Button(result.expanded ? "V" : ">", Width(30))) { result.expanded = !result.expanded; }
				Label(result.name);
				FlexibleSpace();
				Label(result.timeMsg);
				FlexibleSpace();
				Label(result.status);
			} EndHorizontal();

			if (result.expanded) {
				foreach (var test in result.tests) {
					Draw(test);
				}
			}

		} EndVertical();
	}

	void Draw(TestResult result) {
		GUI.color = Color.yellow;
		if (result.status.StartsWith("Pass!")) { GUI.color = Color.green; }
		if (result.status.Contains("Assertion Failed")) { GUI.color = Color.red; }
		if (result.status.Contains("Unexpected Exception")) { GUI.color = new Color(1, .4f, .1f); }

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
	
	void Update() {
		
	}

	



	
}

[TestTests] public class TestsGood {
	[Test] public static void ThingyA() {
		int x = 3;
		x.ToString().ShouldBe<string>("3");
	}
	[Test] public static void ThingyB() {
		"bobby".ShouldBe<string>("bobby");
	}
	[Test] public static void ThingyC() {
		"bobby".ShouldNotBe<string>("hank");
	}
}

[TestTests] public class TestsBad {
	
	[Test] public static void TestShouldPass() {
		int x = 3;
		x *= x;

		x.ShouldBe(9);
		x.ShouldNotBe(20);

	}

	[Test] public static void TestShouldFail() {
		int x = 5;
		x.ShouldNotBe(5);
		x.ShouldNotBe(0);

	}

	[Test] public static void TestShouldFailUnexpectedly() {
		throw new InvalidOperationException("Stay calm, it is just a test");
	}	


}
