
namespace Dummy {
	public class DummyClass<T> : IDummy {
		#region Field Variables
		// Variables
		public string variable1;
		private object variable2;
		protected int variable3;
		internal byte variable4 = 255;
		sbyte[] variable5;
		byte a;
		object b;
		ushort c;
		short d;
		int e;
		uint f;
		long g;
		ulong h;
		float aa;
		double ab;
		decimal ac;
		string ad;
		public static readonly DummyClass<int> Unknown = new DummyClass<int>(10, 20);
		
		#endregion // Field Variables
		
		public string Variable2 { get { return this.variable2.ToString(); } }
		
		#region Public Constructors
		
		public DummyClass(double parameter1, T parameter2) {
			this.variable1 = parameter1.ToString("X2");
			this.variable2 = parameter2;
			this.variable3 = 10;
		}
		
		#endregion // Public Constructors
	}
}
