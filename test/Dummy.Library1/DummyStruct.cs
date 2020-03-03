
using System;
using System.Collections.Generic;

namespace Dummy {
	public struct DummyStruct : IDummy, IDummer<DummyStruct>, IEquatable<DummyStruct> {
		public string Variable2 { get; set; }
		
		public bool Equals(DummyStruct other) {
			return true;
		}
		
		[Dummy("Henlo", "Goobey", val="Hello", HasValue=true)]
		public static void Create(ref DummyClass<DummyStruct> s, out DummyStruct b) {
			b = new DummyStruct();
		}
		
		[System.Obsolete]
		[System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Fill)]
		public static void Display([Dummy("10", "20")]int id = 1, params object[] objs) {}
		public static void Consume(List<List<Dictionary<IDummy, DummyStruct>>> lists, Dictionary<string, IDummy> dictionary) {}
		public static void Kill(Dictionary<List<Dictionary<IDummy, string>>, List<DummyStruct>> lists, Dictionary<string, IDummy> dictionary) {}
		
		public static DummyStruct Create() {
			return new DummyStruct();
		}
	}
}
