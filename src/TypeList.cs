
using Mono.Cecil;

using System.Collections.Generic;

namespace SharpChecker {
	/// <summary>All the information of types with it's associated library or executable</summary>
	public class TypeList {
		// Variables
		#region Field Variables
		/// <summary>A hashmap of a library or executable mapping to a list of types it contains</summary>
		public Dictionary<string, List<string>> types = new Dictionary<string, List<string>>();
		
		#endregion // Field Variables
		
		#region Public Static Methods
		
		/// <summary>Generates a type list</summary>
		/// <param name="assemblies">The assemblies to look into</param>
		/// <returns>Returns the type list generated</returns>
		public static TypeList GenerateList(string[] assemblies) {
			// Variables
			TypeList list = new TypeList();
			
			foreach(string assembly in assemblies) {
				// Variables
				AssemblyDefinition asm = AssemblyDefinition.ReadAssembly(assembly);
				int index = System.Math.Max(assembly.LastIndexOf('/'), assembly.LastIndexOf('\\'));
				string asmName = (index == -1 ? assembly : assembly.Substring(index + 1));
				
				if(!list.types.ContainsKey(asmName)) {
					list.types.Add(asmName, new List<string>());
				}
				
				foreach(ModuleDefinition module in asm.Modules) {
					foreach(TypeDefinition type in module.GetTypes()) {
						if(type.FullName.Contains('<') && type.FullName.Contains('>')) {
							continue;
						}
						if(type.IsNotPublic && TypeInfo.ignorePrivate) {
							continue;
						}
						if(TypeInfo.ignorePrivate) {
							// Variables
							TypeDefinition nestedType = type;
							
							while(nestedType.IsNested) {
								nestedType = nestedType.DeclaringType;
							}
							
							if(nestedType.IsNotPublic) { continue; }
						}
						list.types[asmName].Add(type.FullName);
					}
				}
			}
			
			return list;
		}
		
		#endregion // Public Static Methods
	}
}
