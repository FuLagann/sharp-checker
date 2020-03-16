
using Mono.Cecil;
using Mono.Collections.Generic;

namespace SharpChecker {
	/// <summary>All the information relevant to parameters</summary>
	public class ParameterInfo {
		#region Field Variables
		// Variables
		/// <summary>The name of the parameter</summary>
		public string name;
		/// <summary>The default value of the parameter (if it exists)</summary>
		public string defaultValue;
		/// <summary>The list of attributes that the parameter contains</summary>
		public AttributeInfo[] attributes;
		/// <summary>Any modifiers to the parameter (such as ref, in, out, params, etc.)</summary>
		public string modifier;
		/// <summary>Set to true if the parameter is optional and can be left out when calling the method</summary>
		public bool isOptional;
		/// <summary>The information of the parameter's type</summary>
		public QuickTypeInfo typeInfo;
		/// <summary>The list of types used for the generic parameters</summary>
		public string[] genericParameterDeclarations;
		/// <summary>The full declaration of the parameter as it would be found within the code</summary>
		public string fullDeclaration;
		
		#endregion // Field Variables
		
		#region Public Static Methods
		
		/// <summary>Generates an array of parameter informations from the given collection of parameter definition</summary>
		/// <param name="parameters">The collection of parameters to look into</param>
		/// <returns>Returns the array of parameter informations generated from the collection of parameter definitions</returns>
		public static ParameterInfo[] GenerateInfoArray(Collection<ParameterDefinition> parameters) {
			// Variables
			ParameterInfo[] results = new ParameterInfo[parameters.Count];
			int i = 0;
			
			foreach(ParameterDefinition parameter in parameters) {
				results[i++] = GenerateInfo(parameter);
			}
			
			return results;
		}
		
		/// <summary>Generates the information for the parameter given the parameter definition</summary>
		/// <param name="parameter">The parameter definition to look into</param>
		/// <returns>Returns the parameter information generated from the parameter definition</returns>
		public static ParameterInfo GenerateInfo(ParameterDefinition parameter) {
			// Variables
			ParameterInfo info = new ParameterInfo();
			
			info.name = parameter.Name;
			info.typeInfo = QuickTypeInfo.GenerateInfo(parameter.ParameterType);
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
			info.fullDeclaration = GetFullDeclaration(info);
			
			return info;
		}
		
		#endregion // Public Static Methods
		
		#region Private Static Methods
		
		/// <summary>Gets the full declaration of the parameter</summary>
		/// <param name="parameter">The parameter info to look into</param>
		/// <returns>Returns the full declaration of the parameter</returns>
		private static string GetFullDeclaration(ParameterInfo parameter) {
			// Variables
			string decl = parameter.typeInfo.unlocalizedName;
			int index = decl.LastIndexOf('`');
			
			if(index != -1) {
				decl = TypeInfo.LocalizeName(decl, parameter.genericParameterDeclarations);
			}
			decl = QuickTypeInfo.DeleteNamespaceFromType(QuickTypeInfo.MakeNameFriendly(decl));
			if(parameter.modifier != "") {
				decl = parameter.modifier + " " + decl;
			}
			decl += $" { parameter.name }";
			if(parameter.defaultValue != "") {
				if(parameter.typeInfo.name == "string") {
					decl += $@" = ""{ parameter.defaultValue }""";
				}
				else {
					decl += $" = { parameter.defaultValue }";
				}
			}
			
			return decl;
		}
		
		/// <summary>Finds if the parameter has the params attribute (meaning that the parameter is a "params type[] name" kind of parameter)</summary>
		/// <param name="attrs">The list of attributes the parameter has</param>
		/// <returns>Returns true if the parameter contains the params attribute</returns>
		private static bool HasParamsAttribute(AttributeInfo[] attrs) {
			foreach(AttributeInfo attr in attrs) {
				if(attr.typeInfo.unlocalizedName == "System.ParamArrayAttribute") {
					return true;
				}
			}
			
			return false;
		}
		
		#endregion // Private Static Methods
	}
}
