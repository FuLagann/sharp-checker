
public class ModAttribute : System.Attribute {
	public ModAttribute(bool isMod) {}
}

internal class Test1 {}
internal interface ITest {
	void Below();
}
public class Test2 : ITest {
	public void Below() {}
}

namespace Dummy {
	[Mod(false)]
	[Dummy("Hello", "World", val="Testing", HasValue=true)]
	public class DummyClass<T> : DummyStruct, IDummy, ITest where T : struct {
		#region Field Variables
		// Variables
		public string variable1;
		private object variable2;
		protected int variable3;
		internal byte variable4 = 255;
		public static readonly DummyClass<int> Unknown = new DummyClass<int>(10, 20);
		
		#endregion // Field Variables
		
		public string Variable2 { get { return this.variable2.ToString(); } }
		
		public virtual string Yepper { get; }
		public static string Yep { get; }
		public virtual string Yep3 { get; set; }
		protected virtual string Nope { get; private set; }
		public static string Nope2 { get; internal set; }
		internal string Nope3 { private get; set; }
		//public override string V1 { get { return "1"; } set {}}
		
		public event System.EventHandler OnLog;
		
		#region Public Constructors
		
		public DummyClass(double parameter1, T parameter2) {
			this.variable1 = parameter1.ToString("X2");
			this.variable2 = parameter2;
			this.variable3 = 10;
		}
		public DummyClass() {}
		private DummyClass(IDummy dummy) {}
		
		#endregion // Public Constructors
		
		public string Temp2(DummyClass<int> a, DummyStruct b) { return ""; }
		protected sealed override void Temporary() {}
		public void Yoyoyo() {}
		public new void Yoyo() {}
		public void Below() {}
		//public new bool Equals(DummyStruct other) { return true; }
		
		public static bool operator ==(DummyClass<T> left, DummyClass<T> right) { return true; }
		public static bool operator !=(DummyClass<T> left, DummyClass<T> right) { return false; }
		public static explicit operator DummyStructer(DummyClass<T> obj) { return new DummyStructer(); }
		public static implicit operator Duma(DummyClass<T> obj) { return new Duma(); }
	}
}
