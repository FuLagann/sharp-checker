
namespace SharpChecker {
	public abstract class BaseInfo : System.IComparable {
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
	}
}
