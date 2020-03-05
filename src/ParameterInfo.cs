
using Mono.Cecil;
using Mono.Collections.Generic;

using System.Collections.Generic;
using System.Text.RegularExpressions;

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
		/// <summary>
		/// The information of the type
		/// </summary>
		public QuickTypeInfo typeInfo;
		public string modifier;
		public bool isOptional;
		public string defaultValue;
		public string[] genericParameterDeclarations;
		public AttributeInfo[] attributes;
		
		#endregion // Field Variables
		
		#region Public Static Methods
		
		
		public static ParameterInfo[] GenerateInfoArray(Collection<ParameterDefinition> parameters) {
			// Variables
			ParameterInfo[] results = new ParameterInfo[parameters.Count];
			int i = 0;
			
			foreach(ParameterDefinition parameter in parameters) {
				results[i++] = GenerateInfo(parameter);
			}
			
			return results;
		}
		
		public static ParameterInfo GenerateInfo(ParameterDefinition parameter) {
			// Variables
			ParameterInfo info = new ParameterInfo();
			
			info.name = parameter.Name;
			info.typeInfo = QuickTypeInfo.GenerateInfo(
				parameter.ParameterType.Resolve() ?? parameter.ParameterType
			);
			info.attributes = AttributeInfo.GenerateInfoArray(parameter.CustomAttributes);
			
			if(parameter.IsIn) { info.modifier = "in"; }
			else if(parameter.IsOut) { info.modifier = "out"; }
			else if(parameter.ParameterType.IsByReference) {
				info.modifier = "ref";
			}
			else if(HasParamsAttribute(info.attributes)) {
				info.modifier = "params";
			}
			else { info.modifier = ""; }
			info.isOptional = parameter.IsOptional;
			info.defaultValue = $"{ parameter.Constant }";
			info.genericParameterDeclarations = QuickTypeInfo.GetGenericParametersAsStrings(parameter.ParameterType.FullName);
			
			return info;
		}
		
		public static bool HasParamsAttribute(AttributeInfo[] attrs) {
			foreach(AttributeInfo attr in attrs) {
				if(attr.typeInfo.unlocalizedName == "System.ParamArrayAttribute") {
					return true;
				}
			}
			
			return false;
		}
		
		#endregion // Public Static Methods
	}
}
