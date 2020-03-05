
using System;
using System.Collections.Generic;

namespace Dummy {
	public abstract class DummyStruct : IDummy, IDummer<List<DummyStruct>>, IEquatable<DummyStruct> {
		public string Variable1;
		public const int ab = 10;
		public List<string> strings;
		[System.Obsolete]
		public object[][] grid;
		
		public string Variable2 { get; }
		public string Variable3 { get; set; }
		public string Variable4 { set {} }
		
		public override string ToString() { return ""; }
		public new int GetHashCode() { return 1; }
		public virtual void Yoyo() {}
		public void Ayo() {}
		protected abstract void Temporary();
		
		public virtual bool Equals(DummyStruct other) {
			return true;
		}
		
		[Dummy("Henlo", "Goobey", val="Hello", HasValue=true)]
		public static void Create(ref DummyClass<DummyStruct> s, out DummyStruct b) {
			b = (DummyStruct)s;
		}
		
		[System.Obsolete]
		[System.ComponentModel.DataObjectMethod(System.ComponentModel.DataObjectMethodType.Fill)]
		public static void Display([Dummy("10", "20")]int id = 1, params object[] objs) {}
		public static void Consume(List<List<Dictionary<IDummy, DummyStruct>>> lists, Dictionary<string, IDummy> dictionary) {}
		public static void Kill(Dictionary<List<Dictionary<IDummy, string>>, List<DummyStruct>> lists, Dictionary<string, IDummy> dictionary) {}
		
		public static DummyStruct Create() {
			return new DummyClass<int>(1, 2);
		}
	}
}
