
using Mono.Cecil;
using Mono.Collections.Generic;

namespace SharpChecker {
	/// <summary>
	/// All the important information pertaining to a method
	/// </summary>
	public class MethodInfo {
		#region Field Variables
		// Variables
		/// <summary>
		/// A human readble name of the method
		/// </summary>
		public string name;
		/// <summary>
		/// A full human readble name of the method
		/// </summary>
		public string fullName;
		
		#endregion // Field Variables
		
		#region Public Static Methods
		
		/// <summary>
		/// Generates an array of method infos from the collection of method definitions
		/// </summary>
		/// <param name="methods">The collection of method definitions</param>
		/// <returns>Returns an array of method infos</returns>
		public static MethodInfo[] GenerateMethodInfoArray(Collection<MethodDefinition> methods) {
			// Variables
			MethodInfo[] results = new MethodInfo[methods.Count];
			int i = 0;
			
			foreach(MethodDefinition method in methods) {
				results[i] = GenerateMethodInfo(method);
				i++;
			}
			
			return results;
		}
		
		/// <summary>
		/// Generates a method info from the given method definition
		/// </summary>
		/// <param name="method">The method information to look into</param>
		/// <returns>Returns a method info of the method definition provided</returns>
		public static MethodInfo GenerateMethodInfo(MethodDefinition method) {
			// Variables
			MethodInfo info = new MethodInfo();
			
			info.name = method.Name;
			info.fullName = method.FullName;
			
			return info;
		}
		
		#endregion // Public Static Methods
	}
}
