
using System;
using System.Collections.Generic;

namespace Dummy {
	public struct DummyStruct : IEquatable<DummyStruct> {
		public bool Equals(DummyStruct other) {
			return true;
		}
		
		public static void Create(ref DummyClass<DummyStruct> s, out DummyStruct b) {
			
		}
		
		[System.Obsolete]
		[System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Fill)]
		public static void Display(int id = 1, params object[] objs) {}
		public static void Consume(List<List<Dictionary<IDummy, DummyStruct>>> lists, Dictionary<string, IDummy> dictionary) {}
		public static void Kill(Dictionary<List<Dictionary<IDummy, string>>, List<DummyStruct>> lists, Dictionary<string, IDummy> dictionary) {}
		
		public static DummyStruct Create() {
			return new DummyStruct();
		}
	}
}
