using UnityEngine;
using Shouldly;
using System;
using System.Threading;
using System.Collections;
using System.Collections.Generic;

#if XtoJSON
[Test] public static class XtoJSON_Objects {

	////////////////////////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////////////
	[Test] public static void ObjectAddTest() {
		{
			JsonObject obj = new JsonObject();

			obj.Count.ShouldBe(0);

			obj.Add("what", "huh")
				.Add("okay", "alright");

			obj.Count.ShouldBe(2);
		}
	}

	

	////////////////////////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////////////
	[Test] public static void ObjectIndex() {
		{
			Dictionary<string, float> data = new Dictionary<string,float>() {
				{"str", 5},
				{"dex", 12},
				{"vit", 8},
			};
			JsonObject obj = new JsonObject();
			foreach (var pair in data) { obj[pair.Key] = pair.Value; }

			obj.Count.ShouldBe(3);
			obj["str"].numVal.ShouldBe(5);
			obj["vit"].numVal.ShouldBe(8);
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////////////
	[Test] public static void ObjectVectOps() {
		{
			JsonObject v1 = new JsonObject("x", 5, "y", 3, "z", 2);
			JsonObject v2 = new JsonObject("x", 3, "y", 1, "z", 4);
		
			var v3 = v1.Multiply(v2);
			v3["x"].numVal.ShouldBe(15);
			v3["y"].numVal.ShouldBe(3);
			v3["z"].numVal.ShouldBe(8);

			var v4 = v1.AddNumbers(v2);
			v4["x"].numVal.ShouldBe(8);
			v4["y"].numVal.ShouldBe(4);
			v4["z"].numVal.ShouldBe(6);
		}

		{
			JsonObject matrix = new JsonObject()
				.Add("maxHP", new JsonObject("str", 2, "vit", 5))
				.Add("maxMP", new JsonObject("int", 2, "wis", 2));

			JsonObject stats = new JsonObject("str", 10, "dex", 10, "vit", 10, "int", 10, "wis", 10);

			var result = stats.Multiply(matrix);

			result["maxHP"].numVal.ShouldBe(70);
			result["maxMP"].numVal.ShouldBe(40);
		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////////////
	[Test] public static void ObjectSet() {
		{
			JsonObject a = new JsonObject("x", 1, "y", 2, "z", 3);
			JsonObject b = new JsonObject("x", 4, "y", 5, "z", 6);

			a.Set(b);

			a["x"].numVal.ShouldBe(4);
			a["y"].numVal.ShouldBe(5);
			a["z"].numVal.ShouldBe(6);
		}

		{
			JsonObject a = new JsonObject()
				.Add("nested", new JsonObject("a", 1, "b", 2, "c", 3));

			JsonObject b = new JsonObject()
				.Add("nested", new JsonObject("x", 1, "y", 2, "z", 3, "c", 621));

			a.Set(b);

			a["nested"].Count.ShouldBe(4);
			a["nested"].ContainsKey("a").ShouldBeFalse();
			a["nested"].ContainsKey("x").ShouldBeTrue();
		}

		{
			JsonObject a = new JsonObject()
				.Add("nested", new JsonObject("a", 1, "b", 2, "c", 3));

			JsonObject b = new JsonObject()
				.Add("nested", new JsonObject("x", 1, "y", 2, "z", 3, "c", 621));
			
			a.SetRecursively(b);

			a["nested"].Count.ShouldBe(6);
			a["nested"].ContainsKey("a").ShouldBeTrue();
			a["nested"].ContainsKey("x").ShouldBeTrue();

		}

	}

	////////////////////////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////////////
	[Test] public static void ObjectEqual() {
		
		{
			JsonObject a = new JsonObject("x", 3);
			JsonObject b = new JsonObject("x", 3);
			JsonObject c = new JsonObject("x", 2);

			a.Equals(b).ShouldBeTrue();
			b.Equals(a).ShouldBeTrue();

			a.Equals(c).ShouldBeFalse();
		}

		{
			JsonObject a = new JsonObject("x", null, "y", false, "z", true);
			JsonObject b = new JsonObject()
				.Add("x", null)
				.Add("y", false)
				.Add("z", true);
			JsonObject c = new JsonObject("x", "something", "y", true, "z", false);


			a.Equals(b).ShouldBeTrue();
			b.Equals(a).ShouldBeTrue();

			a.Equals(c).ShouldBeFalse();
		}

		{
			JsonObject a = new JsonObject("x", 5, "y", 12, "z", 15, "tag", "blah")
				.Add("nested", new JsonObject("x", 3, "nestedNested", new JsonObject()))
				.Add("array", new JsonArray("a", "b", "c", 1, 2, 3))
				.Add("emptyObject", new JsonObject())
				.Add("emptyArray", new JsonArray());

			JsonObject b = new JsonObject("x", 5, "y", 12, "z", 15, "tag", "blah")
				.Add("emptyObject", new JsonObject())
				.Add("array", new JsonArray("a", "b", "c", 1, 2, 3))
				.Add("emptyArray", new JsonArray())
				.Add("nested", new JsonObject("x", 3, "nestedNested", new JsonObject()));

			JsonObject c = new JsonObject("x", 5, "y", 12, "z", 15, "tag", "blah")
				.Add("emptyObject", new JsonObject())
				.Add("array", new JsonArray("a", "b", "c", 1, 2, 3))
				.Add("emptyArray", new JsonArray())
				.Add("nested", new JsonObject("x", 3, "nestedNested", new JsonObject("x", 5)));

			a.Equals(b).ShouldBeTrue();
			b.Equals(a).ShouldBeTrue();

			a.Equals(c).ShouldBeFalse();
		}

		
	}
	////////////////////////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////////////
	[Test] public static void ObjectPrintParse() {
		{
			JsonObject obj = new JsonObject();

			string str = obj.ToString();
			str.ShouldBe<string>("{}");

			string pp = obj.PrettyPrint();
			pp.ShouldBe<string>("{\n}");

			JsonObject strParse = Json.Parse(str) as JsonObject;
			JsonObject ppParse = Json.Parse(pp) as JsonObject;

			true.ShouldBe(obj.Equals(strParse));
			true.ShouldBe(obj.Equals(ppParse));

		}

		{
			JsonObject obj = new JsonObject("x", 5, "y", 20, "str", "someString", "z", false);

			string str = obj.ToString();
			string expectedToString = "{'x':5,'y':20,'str':'someString','z':false}".Replace('\'', '\"');
			str.ShouldBe<string>(expectedToString);

			string pp = obj.PrettyPrint();
			string expectedPrettyPrint = @"{
	'x':5,
	'y':20,
	'str':'someString',
	'z':false
}".Replace('\'', '\"');

			pp.ShouldBe<string>(expectedPrettyPrint);

			JsonObject strParse = Json.Parse(str) as JsonObject;
			JsonObject ppParse = Json.Parse(pp) as JsonObject;

			true.ShouldBe(obj.Equals(strParse));
			true.ShouldBe(obj.Equals(ppParse));
		}

		{
			JsonObject obj = new JsonObject();
			obj["x"] = new JsonObject();
			obj["x"]["y"] = new JsonObject();
			obj["x"]["y"]["z"] = new JsonObject();

			string str = obj.ToString();
			string expectedToString = "{'x':{'y':{'z':{}}}}".Replace('\'', '\"');
			str.ShouldBe<string>(expectedToString);

			string pp = obj.PrettyPrint();
			string expectedPrettyPrint = @"{
	'x':
	{
		'y':
		{
			'z':
			{
			}
		}
	}
}".Replace('\'', '\"');
			pp.ShouldBe<string>(expectedPrettyPrint);

			JsonObject strParse = Json.Parse(str) as JsonObject;
			JsonObject ppParse = Json.Parse(pp) as JsonObject;

			true.ShouldBe(obj.Equals(strParse));
			true.ShouldBe(obj.Equals(ppParse));

		}

	}
}

[Test] public static class XtoJSON_Arrays {
	////////////////////////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////////////
	[Test] public static void ArrayGeneral() {
		{
			JsonArray x = new JsonArray();
			x.Count.ShouldBe(0);

			x[0] = "Test1";
			x["1"] = "test2";
			x[2.0] = "test3";

			x.Count.ShouldBe(3);
			x[1].stringVal.ShouldBe<string>("test2");
			x["2"].stringVal.ShouldBe<string>("test3");

		}

		{
			JsonArray x = new object[]{ 1, 2, 3 };
			JsonArray y = new JsonArray() {1, 2, 3};
			JsonArray z = new int[] { 1, 2, 3};

			x.Count.ShouldBe(y.Count);
			x.Equals(y).ShouldBeTrue();
			z.Equals(z).ShouldBeTrue();
			y.Equals(z).ShouldBeTrue();
		}

	}

	////////////////////////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////////////
	[Test] public static void ArrayAdd() {
		{
			JsonArray x = new JsonArray().Add(1).Add(2).Add(3);
			x.Count.ShouldBe(3);
		}
		
		{
			JsonArray x = new JsonArray().Add(1).Add(2).Add(3);
			JsonArray y = new JsonArray().Add(x);

			y.Count.ShouldBe(1);
			y[0].Count.ShouldBe(3);
		}

		{
			int[] nums = { 1, 2, 3};
			JsonArray x = new JsonArray().Add( (JsonArray)nums );
			x.Count.ShouldBe(1);
			x[0].Count.ShouldBe(3);
		}

	}

	////////////////////////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////////////
	[Test] public static void ArrayAddAll() {
		{
			JsonArray x = new JsonArray();
			JsonArray y = new JsonArray();
			x.AddAll(y);
			x.Count.ShouldBe(0);
		}

		{
			JsonArray x = new JsonArray();
			JsonArray y = new JsonArray() { 1, 2, 3 };

			x.AddAll(y);
			x.Count.ShouldBe(3);
		}

		{
			JsonArray x = new JsonArray();
			int[] y = { 1, 2, 3};
			
			x.AddAll(y);

		}
	}

	

	////////////////////////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////////////
	[Test] public static void ArrayPrintParse() {
		{
			JsonArray arr = new JsonArray();
			string str = arr.ToString();
			string pp = arr.PrettyPrint();

			str.ShouldBe<string>("[]");
			pp.ShouldBe<string>("[\n]");

			JsonArray strParse = Json.Parse(str) as JsonArray;
			JsonArray ppParse = Json.Parse(pp) as JsonArray;

			true.ShouldBe(arr.Equals(strParse));
			true.ShouldBe(arr.Equals(ppParse));

		}

		{
			JsonArray arr = new JsonArray(1,2,3,4,5,6);

			string str = arr.ToString();
			string pp = arr.PrettyPrint();

			string strExpected = "[1,2,3,4,5,6]";
			string ppExpected = @"[
	1,
	2,
	3,
	4,
	5,
	6
]".Replace('\'', '\"');

			str.ShouldBe<string>(strExpected);
			pp.ShouldBe<string>(ppExpected);

			JsonArray strParse = Json.Parse(str) as JsonArray;
			JsonArray ppParse = Json.Parse(pp) as JsonArray;

			true.ShouldBe(arr.Equals(strParse));
			true.ShouldBe(arr.Equals(ppParse));

		}

		{
			JsonArray arr = new JsonArray();
			arr.Add(new JsonArray());
			arr.Add(new JsonArray().Add(new JsonArray()));
			arr.Add(new JsonArray().Add(new JsonArray().Add(new JsonArray())));

			string str = arr.ToString();
			string pp = arr.PrettyPrint();

			string strExpected = "[[],[[]],[[[]]]]";
			string ppExpected = @"[
	[
	],
	[
		[
		]
	],
	[
		[
			[
			]
		]
	]
]";
			str.ShouldBe<string>(strExpected);
			pp.ShouldBe<string>(ppExpected);

			JsonArray strParse = Json.Parse(str) as JsonArray;
			JsonArray ppParse = Json.Parse(pp) as JsonArray;

			true.ShouldBe(arr.Equals(strParse));
			true.ShouldBe(arr.Equals(ppParse));

		}
	}

	////////////////////////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////////////
	[Test] public static void NestedPrintParseTest() {
		JsonObject obj = new JsonObject(
			"value", 20,
			"level", 5,
			"name", "Sword of Boom",
			"desc", "It goes boom.",
			"damage", new JsonArray(new JsonObject("power",25,"type","fire"), new JsonObject("power",10,"type","elec")),
			"proc", new JsonObject(
				"chance", .1, 
				"scripts", new JsonArray("explode", new JsonObject("name","stun","chance",.35,"duration",3))
			)

		);

		string str = obj.ToString();
		string pp = obj.PrettyPrint();

		string strExpected = "{'value':20,'level':5,'name':'Sword of Boom','desc':'It goes boom.','damage':[{'power':25,'type':'fire'},{'power':10,'type':'elec'}],'proc':{'chance':0.1,'scripts':['explode',{'name':'stun','chance':0.35,'duration':3}]}}".Replace('\'', '\"');
		string ppExpected = @"{
	'value':20,
	'level':5,
	'name':'Sword of Boom',
	'desc':'It goes boom.',
	'damage':
	[
		{
			'power':25,
			'type':'fire'
		},
		{
			'power':10,
			'type':'elec'
		}
	],
	'proc':
	{
		'chance':0.1,
		'scripts':
		[
			'explode',
			{
				'name':'stun',
				'chance':0.35,
				'duration':3
			}
		]
	}
}".Replace('\'', '\"');

		str.ShouldBe<string>(strExpected);
		//Debug.Log(pp);
		//Debug.Log(ppExpected);
		pp.ShouldBe<string>(ppExpected);

		JsonObject strParse = Json.Parse(str) as JsonObject;
		JsonObject ppParse = Json.Parse(pp) as JsonObject;

		true.ShouldBe(obj.Equals(strParse));
		true.ShouldBe(obj.Equals(ppParse));
	}

	////////////////////////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////////////
	[Test] public static void ListReflecting() {
		
		{
			List<string> list = new List<string>();
			for (int i = 0; i < 10; i++) { list.Add(""+(char)('a'+i)); }

			var reflect = Json.Reflect(list);
			//Debug.Log(reflect);

			true.ShouldBe(reflect.isArray);

			List<string> reflectBack = Json.GetValue<List<string>>(reflect);

			10.ShouldBe(reflectBack.Count);
			for (int i = 0; i < 10; i++) {
				string expect = ""+(char)('a'+i);
				reflectBack[i].ShouldBe<string>(expect);
			}


		}

	}

}

[Test] public static class XtoJSON_General {
	////////////////////////////////////////////////////////////////////////////////////////////////
	///////////////////////////////////////////////////////////////////////////////////////////////
	//////////////////////////////////////////////////////////////////////////////////////////////
	[Test] public static void Escapes() {
		{
			JsonObject obj = new JsonObject();

			string key = "scv:\"wark\"";
			string val = "balls:\"borf\"";
			obj[key] = val;
			1.ShouldBe(obj.Count);
			//foreach (var pair in obj) {
			//Debug.Log("{"+pair.Key.stringVal+"} : {"+pair.Value.stringVal+"}");
			//}


			true.ShouldBe(obj.ContainsKey(key));
			true.ShouldBe(obj[key] == val);

			string str = obj.ToString();
			string pp = obj.PrettyPrint();
			//Debug.Log(str);
			//Debug.Log(pp);

			JsonObject strParse = Json.Parse(str) as JsonObject;
			JsonObject ppParse = Json.Parse(pp) as JsonObject;
			//Debug.Log(strParse);
			//Debug.Log(ppParse);

			true.ShouldBe(obj.Equals(strParse));
			true.ShouldBe(obj.Equals(ppParse));

		}

	}
}
#endif
