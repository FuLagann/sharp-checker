
using Mono.Cecil;
using Mono.Collections.Generic;

namespace SharpChecker {
	public class InterfaceInfo {
		#region Field Variables
		// Variables
		/// <summary>
		/// The information of the type
		/// </summary>
		public QuickTypeInfo typeInfo;
		public QuickTypeInfo[] genericsTypeInfo;
		
		#endregion // Field Variables
		
		#region Public Static Methods
		
		/// <summary>
		/// Generates an array of interface infos
		/// </summary>
		/// <param name="interfaces">The collection of interfaces</param>
		/// <returns>Returns an array of interface infos</returns>
		public static InterfaceInfo[] GenerateInfoArray(Collection<InterfaceImplementation> interfaces) {
			// Variables
			InterfaceInfo[] results = new InterfaceInfo[interfaces.Count];
			int i = 0;
			
			foreach(InterfaceImplementation iface in interfaces) {
				results[i++] = GenerateInfo(iface);
			}
			
			return results;
		}
		
		/// <summary>
		/// Generates the interface information
		/// </summary>
		/// <param name="iface">The interface implementation used to gather the information</param>
		/// <returns>Returns the interface information</returns>
		public static InterfaceInfo GenerateInfo(InterfaceImplementation iface) {
			// Variables
			InterfaceInfo info = new InterfaceInfo();
			TypeDefinition type = iface.InterfaceType.Resolve();
			
			info.typeInfo = QuickTypeInfo.GenerateInfo(type);
			
			
			return info;
		}
		
		#endregion // Public Static Methods
	}
}
