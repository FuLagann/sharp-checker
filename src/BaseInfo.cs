
namespace SharpChecker {
	public abstract class BaseInfo : System.IComparable {
		#region Public Methods
		
		/// <summary>Compares the info with other infos for sorting</summary>
		/// <param name="other">The other object to look into</param>
		/// <returns>Returns a number that finds if it should be shifted or not (-1 and 0 for no shift; 1 for shift)</returns>
		public int CompareTo(object other) {
			if(other is FieldInfo) {
				return (this as FieldInfo).name.CompareTo((other as FieldInfo).name);
			}
			if(other is PropertyInfo) {
				return (this as PropertyInfo).name.CompareTo((other as PropertyInfo).name);
			}
			if(other is MethodInfo) {
				return (this as MethodInfo).name.CompareTo((other as MethodInfo).name);
			}
			if(other is EventInfo) {
				return (this as EventInfo).name.CompareTo((other as EventInfo).name);
			}
			return 0;
		}
		
		#endregion // Public Methods
	}
}
