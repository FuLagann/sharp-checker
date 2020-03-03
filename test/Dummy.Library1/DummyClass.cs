
namespace Dummy {
	public class DummyClass<T> : IDummy {
		#region Field Variables
		// Variables
		public string variable1;
		private object variable2;
		protected int variable3;
		
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
