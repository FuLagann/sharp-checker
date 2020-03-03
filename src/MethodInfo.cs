
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
		/// <summary>
		/// A full human readable name of the method
		/// </summary>
		public string fullName;
		/// <summary>
		/// A full human readable name of the method's declaration (e.g. WriteLine(System.String))
		/// </summary>
		public string partialFullName;
		/// <summary>
		/// A human readable name of the method's declaration (e.g. WriteLine(String))
		/// </summary>
		public string partialName;
		/// <summary>
		/// The information of the return type
		/// </summary>
		public QuickTypeInfo returnType;
		/// <summary>
		/// All the parameters that the method contains
		/// </summary>
		public ParameterInfo[] parameters;
		public bool isStatic;
		private bool isProperty;
		
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
				results[i] = GenerateInfo(method);
				i++;
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
			info.partialName = Regex.Replace(
				info.partialFullName,
				@"(?:[a-zA-Z0-9]*\.)*([a-zA-Z0-9]+)",
				"$1"
			);
			info.fullName = method.FullName;
			info.returnType = QuickTypeInfo.GenerateInfo(method.ReturnType);
			if(info.returnType.fullName == "System.Void") {
				info.returnType.fullName = "";
				info.returnType.name = "";
			}
			info.parameters = ParameterInfo.GenerateInfoArray(method, method.Parameters);
			info.isStatic = method.IsStatic;
			
			return info;
		}
		
		#endregion // Public Static Methods
	}
}
