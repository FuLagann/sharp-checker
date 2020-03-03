
using Mono.Cecil;

using Newtonsoft.Json;

using System.IO;

using Reflection = System.Reflection;

namespace SharpChecker {
	/// <summary>
	/// All the important information pertaining to a type
	/// </summary>
	public class TypeInfo {
		#region Field Variables
		// Variables
		/// <summary>
		/// All the type information to identify the type
		/// </summary>
		public QuickTypeInfo typeInfo;
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
		/// The base type that the type is derived from
		/// </summary>
		public string baseType;
		/// <summary>
		/// The list of generic parameters in their full name as found in the code
		/// </summary>
		public string[] genericParameters;
		/// <summary>
		/// The list of information of the interfaces the type is implementing
		/// </summary>
		public InterfaceInfo[] interfaces;
		/// <summary>
		/// The list of information of the methods the type holds
		/// </summary>
		public MethodInfo[] methods;
		public MethodInfo[] staticMethods;
		
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
						info = GenerateInfo(asm, type);
						return true;
					}
				}
			}
			try {
				// Variables
				System.Type sysType = System.Type.GetType(typePath, true);
				AssemblyDefinition _asm = AssemblyDefinition.ReadAssembly(
					sysType.Assembly.CodeBase.Replace("file:///", "")
				);
				
				foreach(ModuleDefinition _module in _asm.Modules) {
					// Variables
					TypeDefinition _type = _module.GetType(typePath);
					
					if(_type != null) {
						info = GenerateInfo(_asm, _type);
						return true;
					}
				}
			} catch(System.Exception e) {
				System.Console.WriteLine(e);
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
		public static TypeInfo GenerateInfo(AssemblyDefinition asm, TypeDefinition type) {
			// Variables
			TypeInfo info = new TypeInfo();
			string[] generics = GetGenericParametersString(type.GenericParameters.ToArray());
			
			info.typeInfo = QuickTypeInfo.GenerateInfo(type);
			info.assemblyName = asm.Name.Name;
			info.accessor = type.IsPublic ? "public" : "private";
			// ObjectType
			if(type.IsValueType) { info.objectType = "struct"; }
			else if(type.IsEnum) { info.objectType = "enum"; }
			else if(type.IsInterface) { info.objectType = "interface"; }
			else { info.objectType = "class"; }
			// Modifier
			if(type.IsValueType || type.IsInterface) { info.modifier = ""; }
			else if(type.IsSealed && type.IsAbstract) { info.modifier = "static"; }
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
				info.typeInfo.name
			);
			info.baseType = type.BaseType != null ? type.BaseType.FullName : "";
			switch(info.baseType) {
				case "System.Object": { info.baseType = ""; } break;
			}
			info.genericParameters = generics;
			info.interfaces = InterfaceInfo.GenerateInfoArray(type.Interfaces);
			info.methods = MethodInfo.GenerateInfoArray(type, true, false);
			info.staticMethods = MethodInfo.GenerateInfoArray(type, false, true);
			
			return info;
		}
		
		/// <summary>
		/// Gets the array generic parameters as an array of strings
		/// </summary>
		/// <param name="generics">The generic parameters to convert</param>
		/// <returns>Returns an array of strings of the generic parameter names</returns>
		public static string[] GetGenericParametersString(GenericParameter[] generics) {
			if(generics == null) {
				return new string[0];
			}
			
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
