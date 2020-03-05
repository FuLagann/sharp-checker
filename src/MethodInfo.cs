
using Mono.Cecil;
using Mono.Collections.Generic;

using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SharpChecker {
	/// <summary>
	/// All the important information pertaining to a method
	/// </summary>
	public class MethodInfo {
		#region Field Variables
		// Variables
		/// <summary>
		/// A human readable name of the method
		/// </summary>
		public string name;
		public string accessor;
		public string modifier;
		public bool isStatic;
		public bool isVirtual;
		public bool isAbstract;
		public bool isOverriden;
		public bool isConstructor;
		public QuickTypeInfo implementedType;
		/// <summary>
		/// The information of the return type
		/// </summary>
		public QuickTypeInfo returnType;
		/// <summary>
		/// All the parameters that the method contains
		/// </summary>
		public ParameterInfo[] parameters;
		public AttributeInfo[] attributes;
		public string declaration;
		public string parameterDeclaration;
		public string fullDeclaration;
		private bool isProperty;
		private string partialFullName;
		
		#endregion // Field Variables
		
		#region Public Static Methods
		
		public static MethodInfo[] GenerateInfoArray(TypeDefinition type, bool rec, bool isStatic) {
			if(!rec) {
				MethodInfo[] results = GenerateInfoArray(type.Methods);
				
				RemoveUnwanted(ref results, isStatic);
				
				return results;
			}
			
			// Variables
			List<MethodInfo> methods = new List<MethodInfo>();
			MethodInfo[] temp;
			TypeDefinition currType = type;
			TypeReference baseType;
			
			while(currType != null) {
				temp = GenerateInfoArray(currType.Methods);
				RemoveUnwanted(ref temp, isStatic);
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
		
		public static void RemoveUnwanted(ref MethodInfo[] temp, bool isStatic) {
			// Variables
			List<MethodInfo> methods = new List<MethodInfo>(temp);
			
			for(int i = temp.Length - 1; i >= 0; i--) {
				if(methods[i].isProperty) {
					methods.RemoveAt(i);
				}
				else if(methods[i].isStatic != isStatic) {
					methods.RemoveAt(i);
				}
			}
			
			temp = methods.ToArray();
		}
		
		/// <summary>
		/// Removes the duplicates from the given array
		/// </summary>
		/// <param name="temp">The array to remove the duplicates from</param>
		/// <param name="type">The type to check if theres any duplicates</param>
		public static void RemoveDuplicates(ref MethodInfo[] temp, List<MethodInfo> listMethods) {
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
		
		/// <summary>
		/// Generates an array of method infos from the collection of method definitions
		/// </summary>
		/// <param name="methods">The collection of method definitions</param>
		/// <returns>Returns an array of method infos</returns>
		public static MethodInfo[] GenerateInfoArray(Collection<MethodDefinition> methods) {
			// Variables
			MethodInfo[] results = new MethodInfo[methods.Count];
			int i = 0;
			
			foreach(MethodDefinition method in methods) {
				results[i++] = GenerateInfo(method);
			}
			
			return results;
		}
		
		/// <summary>
		/// Generates a method info from the given method definition
		/// </summary>
		/// <param name="method">The method information to look into</param>
		/// <returns>Returns a method info of the method definition provided</returns>
		public static MethodInfo GenerateInfo(MethodDefinition method) {
			// Variables
			MethodInfo info = new MethodInfo();
			
			info.isProperty = method.IsGetter || method.IsSetter;
			info.name = method.Name;
			info.partialFullName = method.FullName.Split("::")[1].Replace(",", ", ");
			info.returnType = QuickTypeInfo.GenerateInfo(method.ReturnType);
			info.parameters = ParameterInfo.GenerateInfoArray(method.Parameters);
			info.attributes = AttributeInfo.GenerateInfoArray(method.CustomAttributes);
			info.isStatic = method.IsStatic;
			info.isVirtual = method.IsVirtual;
			info.isConstructor = method.IsConstructor;
			if(method.IsAssembly) { info.accessor = "internal"; }
			else if(method.IsFamily) { info.accessor = "protected"; }
			else if(method.IsPublic) { info.accessor = "public"; }
			else { info.accessor = "private"; }
			if(method.IsStatic) { info.modifier = "static"; }
			else if(method.IsAbstract) { info.modifier = "abstract"; }
			else if(method.IsFinal) { info.modifier = "sealed override"; }
			else if(method.IsVirtual && method.IsReuseSlot) { info.modifier = "override"; }
			// TODO: Write in 'new'
			// Go through the base types to check if the method already exists, then set the info.modier to "new"
			else if(method.IsVirtual) { info.modifier = "virtual"; }
			else { info.modifier = ""; }
			info.implementedType = QuickTypeInfo.GenerateInfo(method.DeclaringType);
			info.isAbstract = method.IsAbstract;
			info.isOverriden = method.IsReuseSlot;
			info.declaration = (
				info.accessor + " " +
				(info.modifier != "" ? info.modifier + " " : "") +
				info.returnType.name + " " +
				info.name
			);
			info.parameterDeclaration = string.Join(", ", GetParameterDeclaration(info));
			info.fullDeclaration = $"{ info.declaration }({ info.parameterDeclaration })";
			
			return info;
		}
		
		public static string[] GetParameterDeclaration(MethodInfo method) {
			// Variables
			string[] declarations = new string[method.parameters.Length];
			int i = 0;
			int index;
			
			foreach(ParameterInfo parameter in method.parameters) {
				declarations[i] = parameter.typeInfo.unlocalizedName;
				index = declarations[i].LastIndexOf('`');
				if(index != -1) {
					declarations[i] = (
						declarations[i].Substring(0, index) + "<" +
						string.Join(", ", parameter.genericParameterDeclarations) + ">"
					);
				}
				declarations[i] = QuickTypeInfo.DeleteNamespacesInGenerics(declarations[i]);
				declarations[i++] += $" { parameter.name }";
			}
			
			return declarations;
		}
		
		#endregion // Public Static Methods
	}
}
