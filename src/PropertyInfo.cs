
using Mono.Cecil;
using Mono.Collections.Generic;

namespace SharpChecker {
	public class PropertyInfo {
		// Variables
		public string name;
		public string fullName;
		public string accessor;
		public string modifier;
		public QuickTypeInfo typeInfo;
		public AttributeInfo[] attributes;
		public bool hasGetter;
		public bool hasSetter;
		public MethodInfo getter;
		public MethodInfo setter;
		public string getSetDeclaration;
		public string declaration;
		public string fullDeclaration;
		public ParameterInfo[] parameters;
		
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
			info.fullName = property.FullName;
			info.typeInfo = QuickTypeInfo.GenerateInfo(property.PropertyType);
			info.attributes = AttributeInfo.GenerateInfoArray(property.CustomAttributes);
			info.hasGetter = (property.GetMethod != null);
			info.hasSetter = (property.SetMethod != null);
			info.getter = (info.hasGetter ? MethodInfo.GenerateInfo(property.GetMethod) : null);
			info.setter = (info.hasSetter ? MethodInfo.GenerateInfo(property.SetMethod) : null);
			info.parameters = ParameterInfo.GenerateInfoArray(property.Parameters);
			info.getSetDeclaration = (
				(info.hasGetter ? "get;" + (info.hasSetter ? " " : "") : "") +
				(info.hasSetter ? "set;" : "")
			);
			
			return info;
		}
	}
}
