
using Mono.Cecil;
using Mono.Collections.Generic;

using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SharpChecker {
	/// <summary>All the information relevant to methods</summary>
	public class MethodInfo {
		#region Field Variables
		// Variables
		/// <summary>The name of the method</summary>
		public string name;
		/// <summary>The accessor of the method (such as internal, private, protected, public)</summary>
		public string accessor;
		/// <summary>Any modifiers of the method (such as static, virtual, override, etc.)</summary>
		public string modifier;
		/// <summary>Set to true if the method is abstract</summary>
		public bool isAbstract;
		/// <summary>Set to true if the method is a constructor</summary>
		public bool isConstructor;
		/// <summary>Set to true if the method is a conversion operator</summary>
		public bool isConversionOperator;
		/// <summary>Set to true if the method is an extension</summary>
		public bool isExtension;
		/// <summary>Set to true if the method is an operator</summary>
		public bool isOperator;
		/// <summary>Set to true if the method is overriden</summary>
		public bool isOverriden;
		/// <summary>Set to true if the method is static</summary>
		public bool isStatic;
		/// <summary>Set to true if the method is virtual</summary>
		public bool isVirtual;
		/// <summary>The type that the method is implemented in</summary>
		public QuickTypeInfo implementedType;
		/// <summary>The type that the method returns</summary>
		public QuickTypeInfo returnType;
		/// <summary>The attributes of the methods</summary>
		public AttributeInfo[] attributes;
		/// <summary>The parameters that the methods contains</summary>
		public ParameterInfo[] parameters;
		/// <summary>The partial declaration of the method (without parameters) that can be found in the code</summary>
		public string declaration;
		/// <summary>The partial declaration of the parameters that can be found in the code</summary>
		public string parameterDeclaration;
		/// <summary>The full declaration of the method that can be found in the code</summary>
		public string fullDeclaration;
		// Tells if the method is a property, used to remove it when it's irrelevant
		private bool isProperty;
		// Tells if the method is an event, used to remove it when it's irrelevant
		private bool isEvent;
		// The partial name of the method, used to check for base type duplicates
		private string partialFullName;
		// Set to true if the method should be deleted / ignored
		internal bool shouldDelete = false;
		
		#endregion // Field Variables
		
		#region Public Static Methods
		
		/// <summary>Generates an array of method informations from the given type and booleans</summary>
		/// <param name="type">The type definition to look into</param>
		/// <param name="recursive">Set to true to recursively look through the base types of the method</param>
		/// <param name="isStatic">Set to true to generate only static methods</param>
		/// <param name="isConstructor">Set to true to generate only constructors. Defaults to false</param>
		/// <param name="isOperator">Set to true to generate only operators. Defaults to false</param>
		/// <returns></returns>
		public static MethodInfo[] GenerateInfoArray(
			TypeDefinition type, bool recursive, bool isStatic,
			bool isConstructor = false, bool isOperator = false
		) {
			if(!recursive) {
				MethodInfo[] results = GenerateInfoArray(type.Methods);
				
				RemoveUnwanted(ref results, isStatic, isConstructor, isOperator);
				
				return results;
			}
			
			// Variables
			List<MethodInfo> methods = new List<MethodInfo>();
			MethodInfo[] temp;
			TypeDefinition currType = type;
			TypeReference baseType;
			
			while(currType != null) {
				temp = GenerateInfoArray(currType.Methods);
				RemoveUnwanted(ref temp, isStatic, isConstructor, isOperator);
				if(currType != type) {
					RemoveDuplicates(ref temp, methods);
				}
				methods.AddRange(temp);
				baseType = currType.BaseType;
				if(baseType == null) {
					break;
				}
				currType = baseType.Resolve();
			}
			
			return methods.ToArray();
		}
		
		/// <summary>Generates an array of method informations from the given collection of method definitions</summary>
		/// <param name="methods">The collection of methods to look into</param>
		/// <returns>Returns an array of method informations</returns>
		public static MethodInfo[] GenerateInfoArray(Collection<MethodDefinition> methods) {
			// Variables
			List<MethodInfo> results = new List<MethodInfo>();
			MethodInfo info;
			
			foreach(MethodDefinition method in methods) {
				info = GenerateInfo(method);
				if(info.shouldDelete) {
					continue;
				}
				results.Add(info);
			}
			
			return results.ToArray();
		}
		
		/// <summary>Generates the method information from the given method definition</summary>
		/// <param name="method">The method definition to look into</param>
		/// <returns>Returns the method information</returns>
		public static MethodInfo GenerateInfo(MethodDefinition method) {
			// Variables
			MethodInfo info = new MethodInfo();
			int index;
			
			info.isStatic = method.IsStatic;
			info.isVirtual = method.IsVirtual;
			info.isConstructor = method.IsConstructor;
			if(method.IsAssembly) { info.accessor = "internal"; }
			else if(method.IsFamily) { info.accessor = "protected"; }
			else if(method.IsPublic) { info.accessor = "public"; }
			else { info.accessor = "private"; }
			info.isProperty = method.IsGetter || method.IsSetter;
			info.isEvent = method.IsAddOn || method.IsRemoveOn;
			info.isOperator = method.Name.StartsWith("op_");
			info.isConversionOperator = (
				method.Name == "op_Explicit" ||
				method.Name == "op_Implicit"
			);
			info.implementedType = QuickTypeInfo.GenerateInfo(method.DeclaringType);
			info.returnType = QuickTypeInfo.GenerateInfo(method.ReturnType);
			if(info.isConstructor) {
				info.name = info.implementedType.name;
				index = info.name.IndexOf('<');
				if(index != -1) {
					info.name = info.name.Substring(0, index);
				}
			}
			else if(info.isConversionOperator) {
				info.name = method.Name + "__" + info.returnType.name;
			}
			else if(info.isOperator) {
				info.name = method.Name.Substring(3);
			}
			else {
				info.name = method.Name;
			}
			info.partialFullName = method.FullName.Split("::")[1].Replace(",", ", ");
			if(info.isOperator) {
				info.partialFullName = info.name;
			}
			info.parameters = ParameterInfo.GenerateInfoArray(method.Parameters);
			info.attributes = AttributeInfo.GenerateInfoArray(method.CustomAttributes);
			if(info.isConversionOperator) { info.modifier = $"static { method.Name.Substring(3).ToLower() } operator"; }
			else if(info.isOperator) { info.modifier = "static operator"; }
			else if(method.IsStatic) { info.modifier = "static"; }
			else if(method.IsAbstract) { info.modifier = "abstract"; }
			else if(method.IsVirtual && method.IsReuseSlot) { info.modifier = "override"; }
			else if(method.IsVirtual) { info.modifier = "virtual"; }
			else { info.modifier = ""; }
			info.isExtension = HasExtensionAttribute(info);
			info.isAbstract = method.IsAbstract;
			info.isOverriden = method.IsReuseSlot;
			info.declaration = (
				info.accessor + " " +
				(info.modifier != "" ? info.modifier + " " : "") +
				(!info.isConstructor && !info.isConversionOperator ? info.returnType.name + " " : "") +
				(!info.isConversionOperator ? info.name : info.returnType.name)
			);
			info.parameterDeclaration = string.Join(", ", GetParameterDeclaration(info));
			if(info.isExtension) {
				info.parameterDeclaration = $"this { info.parameterDeclaration }";
			}
			info.fullDeclaration = $"{ info.declaration }({ info.parameterDeclaration })";
			if(TypeInfo.ignorePrivate && PropertyInfo.GetAccessorId(info.accessor) == 0) {
				info.shouldDelete = true;
			}
			
			return info;
		}
		
		#endregion // Public Static Methods
		
		#region Private Static Methods
		
		/// <summary>Removes any unwanted methods from the given types of booleans</summary>
		/// <param name="temp">The list of methods to remove from</param>
		/// <param name="isStatic">Set to true if non-static methods should be removed</param>
		/// <param name="isConstructor">Set to false if constructors should be removed</param>
		/// <param name="isOperator">Set to false if operators should be removed</param>
		private static void RemoveUnwanted(
			ref MethodInfo[] temp, bool isStatic,
			bool isConstructor, bool isOperator
		) {
			// Variables
			List<MethodInfo> methods = new List<MethodInfo>(temp);
			
			for(int i = temp.Length - 1; i >= 0; i--) {
				if(methods[i].shouldDelete) {
					methods.RemoveAt(i);
				}
				else if(methods[i].name == ".cctor") {
					methods.RemoveAt(i);
				}
				else if(methods[i].isProperty || methods[i].isEvent) {
					methods.RemoveAt(i);
				}
				else if(methods[i].isStatic != isStatic) {
					methods.RemoveAt(i);
				}
				else if(methods[i].isConstructor != isConstructor) {
					methods.RemoveAt(i);
				}
				else if(methods[i].isOperator != isOperator) {
					methods.RemoveAt(i);
				}
			}
			
			temp = methods.ToArray();
		}
		
		/// <summary>Removes all the duplicates from the list of methods</summary>
		/// <param name="temp">The list of methods to remove duplicates from</param>
		/// <param name="listMethods">The list of recursiveorded methods to reference which ones are duplicates</param>
		private static void RemoveDuplicates(ref MethodInfo[] temp, List<MethodInfo> listMethods) {
			// Variables
			List<MethodInfo> methods = new List<MethodInfo>(temp);
			
			for(int i = temp.Length - 1; i >= 0; i--) {
				foreach(MethodInfo method in listMethods) {
					if(methods[i].partialFullName == method.partialFullName) {
						methods.RemoveAt(i);
						break;
					}
				}
			}
			
			temp = methods.ToArray();
		}
		
		/// <summary>Finds if the method is an extension</summary>
		/// <param name="method">The method to look into</param>
		/// <returns>Returns true if the method is an extension by having the extension attribute</returns>
		private static bool HasExtensionAttribute(MethodInfo method) {
			foreach(AttributeInfo attr in method.attributes) {
				if(attr.typeInfo.fullName == "System.Runtime.CompilerServices.ExtensionAttribute") {
					return true;
				}
			}
			
			return false;
		}
		
		/// <summary>Generates the parameter declaration from the given method</summary>
		/// <param name="method">The method info to look into</param>
		/// <returns>Returns an array of parameter declaration</returns>
		private static string[] GetParameterDeclaration(MethodInfo method) {
			// Variables
			string[] declarations = new string[method.parameters.Length];
			int i = 0;
			
			foreach(ParameterInfo parameter in method.parameters) {
				declarations[i++] = parameter.fullDeclaration;
			}
			
			return declarations;
		}
		
		#endregion // Private Static Methods
	}
}
