
using Mono.Cecil;
using Mono.Collections.Generic;

namespace SharpChecker {
	public class InterfaceInfo {
		#region Field Variables
		// Variables
		/// <summary>
		/// The unlocalized name that can be found in the dll or xml documentation
		/// </summary>
		public string unlocalizedName;
		/// <summary>
		/// A human readable name of the type
		/// </summary>
		public string name;
		/// <summary>
		/// A human readable name of the type including it's namespace
		/// </summary>
		public string fullName;
		/// <summary>
		/// The namespace where the type came from
		/// </summary>
		public string namespaceName;
		
		#endregion // Field Variables
		
		#region Public Static Methods
		
		/// <summary>
		/// Generates an array of interface infos
		/// </summary>
		/// <param name="interfaces">The collection of interfaces</param>
		/// <returns>Returns an array of interface infos</returns>
		public static InterfaceInfo[] GenerateInterfaceInfoArray(Collection<InterfaceImplementation> interfaces) {
			// Variables
			InterfaceInfo[] results = new InterfaceInfo[interfaces.Count];
			int i = 0;
			
			foreach(InterfaceImplementation iface in interfaces) {
				results[i] = GenerateInterfaceInfo(iface);
				i++;
			}
			
			return results;
		}
		
		/// <summary>
		/// Generates the interface information
		/// </summary>
		/// <param name="iface">The interface implementation used to gather the information</param>
		/// <returns>Returns the interface information</returns>
		public static InterfaceInfo GenerateInterfaceInfo(InterfaceImplementation iface) {
			// Variables
			InterfaceInfo info = new InterfaceInfo();
			TypeReference type = iface.InterfaceType;
			string[] generics = TypeInfo.GetGenericParametersString(type.GenericParameters.ToArray());
			
			info.unlocalizedName = type.FullName;
			info.name = TypeInfo.LocalizeName(type.Name, generics);
			info.namespaceName = type.Namespace;
			info.fullName = TypeInfo.LocalizeName(type.FullName, generics);
			
			return info;
		}
		
		#endregion // Public Static Methods
	}
}
