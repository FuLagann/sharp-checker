
using Mono.Cecil;

using Newtonsoft.Json;

namespace SharpChecker {
	/// <summary>
	/// All the important information pertaining to a type
	/// </summary>
	public class TypeInfo {
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
		/// <summary>
		/// The assembly where the type came from
		/// </summary>
		public string assemblyName;
		/// <summary>
		/// The type of accessor the type grants access to (e.g. public or private)
		/// </summary>
		public string accessor;
		/// <summary>
		/// The types of modifiers the type has (e.g. static, abstract, or sealed)
		/// </summary>
		public string modifier;
		/// <summary>
		/// The type of object the type is (e.g. clas, struct, enum, or interface)
		/// </summary>
		public string objectType;
		/// <summary>
		/// The declaration of the type as if it was found in the code (without any inheritance)
		/// (e.g. public static class Math)
		/// </summary>
		public string declaration;
		/// <summary>
		/// The declaration of the type as if it was found in the code
		/// (e.g. public abstract class Graphics : MarshalByRefObjects, IDisposable, IDeviceContext)
		/// </summary>
		public string fullDeclaration;
		/// <summary>
		/// The list of generic parameters in their full name as found in the code
		/// </summary>
		public string[] genericParameters;
		
		#endregion // Field Variables
		
		#region Public Static Methods
		
		/// <summary>
		/// Generates a type information from the list of assemblies and the type path
		/// </summary>
		/// <param name="assemblies">The list of assemblies to search through</param>
		/// <param name="typePath">The type path to search for</param>
		/// <param name="info">The resulting information of the type</param>
		/// <returns>Returns true if the information has successfully been found</returns>
		public static bool GenerateTypeInfo(string[] assemblies, string typePath, out TypeInfo info) {
			foreach(string assembly in assemblies) {
				// Variables
				AssemblyDefinition asm = AssemblyDefinition.ReadAssembly(assembly);
				
				foreach(ModuleDefinition module in asm.Modules) {
					// Variables
					TypeDefinition type = module.GetType(typePath);
					
					if(type != null) {
						info = CreateTypeInfo(asm, type);
						return true;
					}
				}
			}
			info = null;
			return false;
		}
		
		/// <summary>
		/// Creates the type information
		/// </summary>
		/// <param name="asm">The assembly definition to get more information out of</param>
		/// <param name="type">The type definition to get information from</param>
		/// <returns>Returns the type information</returns>
		public static TypeInfo CreateTypeInfo(AssemblyDefinition asm, TypeDefinition type) {
			// Variables
			TypeInfo info = new TypeInfo();
			string[] generics = GetGenericParametersString(type.GenericParameters.ToArray());
			
			info.unlocalizedName = type.FullName;
			info.name = LocalizeName(type.Name, generics);
			info.fullName = LocalizeName(type.FullName, generics);
			info.namespaceName = type.Namespace;
			info.assemblyName = asm.Name.Name;
			info.accessor = type.IsPublic ? "public" : "private";
			// ObjectType
			if(type.IsValueType) { info.objectType = "struct"; }
			else if(type.IsEnum) { info.objectType = "enum"; }
			else if(type.IsInterface) { info.objectType = "interface"; }
			else { info.objectType = "class"; }
			// Modifier
			if(type.IsValueType || type.IsInterface) { info.modifier = ""; }
			else if(type.IsAnsiClass) { info.modifier = "static"; }
			else {
				info.modifier = (type.IsSealed ?
					"sealed" :
					type.IsAbstract ? "abstract" : ""
				);
			}
			info.declaration = (
				$"{ info.accessor } " +
				$"{ (info.modifier != "" ? info.modifier + " " : "") }" +
				$"{ info.objectType } " +
				info.name
			);
			info.genericParameters = generics;
			
			return info;
		}
		
		/// <summary>
		/// Gets the array generic parameters as an array of strings
		/// </summary>
		/// <param name="generics">The generic parameters to convert</param>
		/// <returns>Returns an array of strings of the generic parameter names</returns>
		public static string[] GetGenericParametersString(GenericParameter[] generics) {
			// Variables
			string[] results = new string[generics.Length];
			
			for(int i = 0; i < generics.Length; i++) {
				results[i] = generics[i].Name;
			}
			
			return results;
		}
		
		public static string LocalizeName(string name, string[] generics) {
			if(generics.Length == 0) {
				return name;
			}
			
			return (
				name.Substring(0, name.LastIndexOf('`')) + "<" +
				string.Join(", ", generics) + ">"
			);
		}
		
		#endregion // Public Static Methods
		
		#region Public Methods
		
		/// <summary>
		/// Gets the json string of the object
		/// </summary>
		/// <returns>Returns the json string of the object</returns>
		public string GetJson() { return JsonConvert.SerializeObject(this, Formatting.Indented); }
		
		#endregion // Public Methods
	}
}
