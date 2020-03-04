
using Mono.Cecil;
using Mono.Collections.Generic;

namespace SharpChecker {
	public class PropertyInfo {
		// Variables
		public string name;
		public QuickTypeInfo typeInfo;
		
		public static PropertyInfo[] GenerateInfoArray(Collection<PropertyDefinition> properties) {
			// Variables
			PropertyInfo[] results = new PropertyInfo[properties.Count];
			int i = 0;
			
			foreach(PropertyDefinition property in properties) {
				results[i++] = GenerateInfo(property);
			}
			
			return results;
		}
		
		/// <summary>
		/// Generates a method info from the given method definition
		/// </summary>
		/// <param name="method">The method information to look into</param>
		/// <returns>Returns a method info of the method definition provided</returns>
		public static PropertyInfo GenerateInfo(PropertyDefinition property) {
			// Variables
			PropertyInfo info = new PropertyInfo();
			
			info.name = property.Name;
			info.typeInfo = QuickTypeInfo.GenerateInfo(property.PropertyType);
			
			return info;
		}
	}
}
