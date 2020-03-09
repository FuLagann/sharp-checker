
using System;

namespace Dummy {
	[AttributeUsage(AttributeTargets.All)]
	public class DummyAttribute : Attribute {
		// Variables
		public string val;
		public string guuid;
		public string val2;
		
		public bool HasValue { get { return this.val != ""; } set { this.val = $"{ value }"; } }
		public string DummyVal { get; }
		public string Guuid { get; set; }
		
		public DummyAttribute(string hash1, string hash2) {}
		
		public void TempMethod() {}
	}
}
