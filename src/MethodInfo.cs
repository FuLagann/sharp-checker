
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
		/// The name of the return type
		/// </summary>
		public string returnTypeName;
		/// <summary>
		/// The full name of the return type
		/// </summary>
		public string returnTypeFullName;
		private bool isProperty;
		
		#endregion // Field Variables
		
		#region Public Static Methods
		
		/// <summary>
		/// Generates an array of method infos from the collection of method definitions with recursive inheritance checking
		/// </summary>
		/// <param name="methods">The collection of method definitions</param>
		/// <param name="type">The type to check</param>
		/// <returns>Returns an array of method infos</returns>
		public static MethodInfo[] GenerateMethodInfoArray(TypeDefinition type) {
			// Variables
			List<MethodInfo> methods = new List<MethodInfo>();
			MethodInfo[] temp;
			TypeDefinition currType = type;
			TypeReference baseType;
			
			while(currType != null) {
				temp = GenerateMethodInfoArray(currType.Methods);
				RemoveProperties(ref temp);
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
		
		public static void RemoveProperties(ref MethodInfo[] temp) {
			// Variables
			List<MethodInfo> methods = new List<MethodInfo>(temp);
			
			for(int i = temp.Length - 1; i >= 0; i--) {
				if(methods[i].isProperty) {
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
		public static MethodInfo[] GenerateMethodInfoArray(Collection<MethodDefinition> methods) {
			// Variables
			MethodInfo[] results = new MethodInfo[methods.Count];
			int i = 0;
			
			foreach(MethodDefinition method in methods) {
				results[i] = GenerateMethodInfo(method);
				i++;
			}
			
			return results;
		}
		
		/// <summary>
		/// Generates a method info from the given method definition
		/// </summary>
		/// <param name="method">The method information to look into</param>
		/// <returns>Returns a method info of the method definition provided</returns>
		public static MethodInfo GenerateMethodInfo(MethodDefinition method) {
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
			info.returnTypeFullName = method.ReturnType.FullName;
			info.returnTypeName = method.ReturnType.Name;
			if(info.returnTypeFullName == "System.Void") {
				info.returnTypeFullName = "";
				info.returnTypeName = "";
			}
			// TODO: Get Parameter info
			
			return info;
		}
		
		#endregion // Public Static Methods
	}
}
