
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
		public bool isOperator;
		public bool isExtension;
		public bool isConversionOperator;
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
		private bool isEvent;
		private string partialFullName;
		internal bool shouldDelete = false;
		
		#endregion // Field Variables
		
		#region Public Static Methods
		
		public static MethodInfo[] GenerateInfoArray(
			TypeDefinition type, bool rec, bool isStatic,
			bool isConstructor = false, bool isOperator = false
		) {
			if(!rec) {
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
		
		public static void RemoveUnwanted(
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
		
		/// <summary>
		/// Generates a method info from the given method definition
		/// </summary>
		/// <param name="method">The method information to look into</param>
		/// <returns>Returns a method info of the method definition provided</returns>
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
		
		public static bool HasExtensionAttribute(MethodInfo method) {
			foreach(AttributeInfo attr in method.attributes) {
				if(attr.typeInfo.fullName == "System.Runtime.CompilerServices.ExtensionAttribute") {
					return true;
				}
			}
			
			return false;
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
				declarations[i] = QuickTypeInfo.DeleteNamespaceFromType(
					QuickTypeInfo.MakeNameFriendly(declarations[i])
				);
				declarations[i++] += $" { parameter.name }";
			}
			
			return declarations;
		}
		
		#endregion // Public Static Methods
	}
}
