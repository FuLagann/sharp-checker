
using Mono.Cecil;
using Mono.Collections.Generic;

namespace SharpChecker {
	/// <summary>
	/// All the important information about a parameter
	/// </summary>
	public class ParameterInfo {
		#region Field Variables
		// Variables
		/// <summary>
		/// The name of the parameter
		/// </summary>
		public string name;
		
		#endregion // Field Variables
		
		#region Public Static Methods
		
		
		public static ParameterInfo[] GenerateParameterInfoArray(Collection<ParameterDefinition> parameters) {
			// Variables
			ParameterInfo[] results = new ParameterInfo[parameters.Count];
			int i = 0;
			
			foreach(ParameterDefinition parameter in parameters) {
				results[i] = GenerateParameterInfo(parameter);
				i++;
			}
			
			return results;
		}
		
		public static ParameterInfo GenerateParameterInfo(ParameterDefinition parameter) {
			// Variables
			ParameterInfo info = new ParameterInfo();
			
			info.name = parameter.Name;
			
			return info;
		}
		
		#endregion // Public Static Methods
	}
}
