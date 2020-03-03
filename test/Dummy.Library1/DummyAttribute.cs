
using System;

namespace Dummy {
	[AttributeUsage(AttributeTargets.All)]
	public class DummyAttribute : Attribute {
		// Variables
		public string val;
		public string temp;
		public string val2;
		
		public bool HasValue { get { return this.val != ""; } set { this.val = $"{ value }"; } }
		public string Dummy { get; }
		public string Temp { get; set; }
		
		public DummyAttribute(string temp, string notemp) {}
		
		public void TempMethod() {}
	}
}
