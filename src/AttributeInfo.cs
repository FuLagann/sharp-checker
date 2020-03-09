
using Mono.Cecil;
using Mono.Collections.Generic;

namespace SharpChecker {
	/// <summary>All the information relevant to an attribute</summary>
	public class AttributeInfo {
		#region Field Variables
		// Variables
		/// <summary>The information of the type that the attribute is</summary>
		public QuickTypeInfo typeInfo;
		/// <summary>The list of constructor arguments that the attribute is declaring</summary>
		public AttributeFieldInfo[] constructorArgs;
		/// <summary>The list of fields and properties that the attribute is declaring</summary>
		public AttributeFieldInfo[] properties;
		/// <summary>The declaration of parameters as seen if looking at the code</summary>
		public string parameterDeclaration;
		/// <summary>
		/// The declaration of the attribute as a whole, with name and parameters as seen if looking
		/// at the code
		/// </summary>
		public string fullDeclaration;
		
		#endregion // Field Variables
		
		#region Public Static Methods
		
		/// <summary>
		/// Generates an array of information for all the attributes given a collection of custom attributes
		/// </summary>
		/// <param name="attrs">The collection of custom attributes to gather all the information from</param>
		/// <returns>
		/// Returns the array of attribute information generated from the collection of custom attributes
		/// </returns>
		public static AttributeInfo[] GenerateInfoArray(Collection<CustomAttribute> attrs) {
			// Variables
			AttributeInfo[] results = new AttributeInfo[attrs.Count];
			int i = 0;
			
			foreach(CustomAttribute attr in attrs) {
				results[i++] = GenerateInfo(attr);
			}
			
			return results;
		}
		
		/// <summary>
		/// Generates the information for an attribute from the given Mono.Cecil custom attribute class
		/// </summary>
		/// <param name="attr">The attribute to gather the information from</param>
		/// <returns>Returns the attribute information generated from the custom attribute</returns>
		public static AttributeInfo GenerateInfo(CustomAttribute attr) {
			// Variables
			AttributeInfo info = new AttributeInfo();
			int i = 0;
			
			info.typeInfo = QuickTypeInfo.GenerateInfo(attr.AttributeType);
			info.constructorArgs = new AttributeFieldInfo[attr.ConstructorArguments.Count];
			foreach(CustomAttributeArgument arg in attr.ConstructorArguments) {
				info.constructorArgs[i] = new AttributeFieldInfo();
				info.constructorArgs[i].typeInfo = QuickTypeInfo.GenerateInfo(arg.Type);
				info.constructorArgs[i].name = attr.Constructor.Parameters[i].Name;
				info.constructorArgs[i].value = (info.constructorArgs[i].typeInfo.name != "bool" ?
					$"{ arg.Value }" :
					$"{ arg.Value }".ToLower()
				);
				i++;
			}
			i = 0;
			info.properties = new AttributeFieldInfo[
				attr.Fields.Count +
				attr.Properties.Count
			];
			foreach(CustomAttributeNamedArgument field in attr.Fields) {
				info.properties[i] = new AttributeFieldInfo();
				info.properties[i].typeInfo = QuickTypeInfo.GenerateInfo(field.Argument.Type);
				info.properties[i].name = field.Name;
				info.properties[i].value = (info.properties[i].typeInfo.name != "bool" ?
					$"{ field.Argument.Value }" :
					$"{ field.Argument.Value }".ToLower()
				);
				i++;
			}
			foreach(CustomAttributeNamedArgument property in attr.Properties) {
				info.properties[i] = new AttributeFieldInfo();
				info.properties[i].typeInfo = QuickTypeInfo.GenerateInfo(property.Argument.Type);
				info.properties[i].name = property.Name;
				info.properties[i].value = (info.properties[i].typeInfo.name != "bool" ?
					$"{ property.Argument.Value }" :
					$"{ property.Argument.Value }".ToLower()
				);
				i++;
			}
			info.parameterDeclaration = string.Join(", ", GetParameterDeclaration(info));
			info.fullDeclaration = (
				$"[{ info.typeInfo.fullName }" +
				(info.parameterDeclaration != "" ? $"({ info.parameterDeclaration })" : "") +
				"]"
			);
			
			return info;
		}
		
		#endregion // Public Static Methods
		
		#region Private Static Methods
		
		/// <summary>Gets the parameter declaration string from the given info</summary>
		/// <param name="info">The information used to retrieve the parameter declaration</param>
		/// <returns>Returns the parameter declaration as a string</returns>
		private static string[] GetParameterDeclaration(AttributeInfo info) {
			// Variables
			string[] declarations = new string[
				info.constructorArgs.Length +
				info.properties.Length
			];
			int i = 0;
			
			foreach(AttributeFieldInfo field in info.constructorArgs) {
				declarations[i++] = (field.typeInfo.name == "string" ? $@"""{ field.value }""" : field.value);
			}
			foreach(AttributeFieldInfo field in info.properties) {
				declarations[i++] = $"{ field.name } = " + (field.typeInfo.name == "string" ? $@"""{ field.value }""" : field.value);
			}
			
			return declarations;
		}
		
		#endregion // Private Static Methods
		
		#region Nested Types
		
		/// <summary>All the information relevant to the attribute's fields</summary>
		public class AttributeFieldInfo {
			// Variables
			/// <summary>The name of the attribute field</summary>
			public string name;
			/// <summary>The value of the attribute field</summary>
			public string value;
			/// <summary>The information of the attribute field's type</summary>
			public QuickTypeInfo typeInfo;
		}
		
		#endregion // Nested Types
	}
}
