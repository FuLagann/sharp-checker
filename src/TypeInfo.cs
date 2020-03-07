
using Mono.Cecil;
using Mono.Collections.Generic;

using Newtonsoft.Json;

using System.Collections.Generic;
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
		public QuickTypeInfo baseType;
		public AttributeInfo[] attributes;
		/// <summary>
		/// The list of information of the interfaces the type is implementing
		/// </summary>
		public QuickTypeInfo[] interfaces;
		public MethodInfo[] constructors;
		public FieldInfo[] fields;
		public PropertyInfo[] properties;
		public PropertyInfo[] staticProperties;
		public EventInfo[] events;
		public EventInfo[] staticEvents;
		/// <summary>
		/// The list of information of the methods the type holds
		/// </summary>
		public MethodInfo[] methods;
		public MethodInfo[] staticMethods;
		public MethodInfo[] operators;
		internal static string assemblyUsed = "";
		internal static string[] assembliesUsed;
		internal static bool ignorePrivate = true;
		
		#endregion // Field Variables
		
		#region Public Static Methods
		
		public static bool IsTypePublic(string typePath, string[] assemblies) {
			foreach(string assembly in assemblies) {
				// Variables
				AssemblyDefinition asm = AssemblyDefinition.ReadAssembly(assembly);
				
				foreach(ModuleDefinition module in asm.Modules) {
					// Variables
					TypeDefinition type = module.GetType(typePath);
					
					if(type != null) {
						return type.IsPublic;
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
						return _type.IsPublic;
					}
				}
			} catch(System.Exception e) {
				System.Console.WriteLine(e);
			}
			
			return false;
		}
		
		/// <summary>
		/// Generates a type information from the list of assemblies and the type path
		/// </summary>
		/// <param name="assemblies">The list of assemblies to search through</param>
		/// <param name="typePath">The type path to search for</param>
		/// <param name="info">The resulting information of the type</param>
		/// <returns>Returns true if the information has successfully been found</returns>
		public static bool GenerateTypeInfo(string[] assemblies, string typePath, out TypeInfo info) {
			assembliesUsed = assemblies;
			foreach(string assembly in assemblies) {
				// Variables
				AssemblyDefinition asm = AssemblyDefinition.ReadAssembly(assembly);
				
				foreach(ModuleDefinition module in asm.Modules) {
					// Variables
					TypeDefinition type = module.GetType(typePath);
					
					if(type != null) {
						assemblyUsed = assembly;
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
			
			info.accessor = type.IsPublic ? "public" : "internal";
			info.typeInfo = QuickTypeInfo.GenerateInfo(type);
			info.assemblyName = asm.Name.Name;
			// ObjectType
			if(type.IsEnum) { info.objectType = "enum"; }
			else if(type.IsValueType) { info.objectType = "struct"; }
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
			info.attributes = AttributeInfo.GenerateInfoArray(type.CustomAttributes);
			if(type.BaseType != null) {
				switch(type.BaseType.FullName) {
					case "System.Enum":
					case "System.ValueType":
					case "System.Object": {
						info.baseType = new QuickTypeInfo();
						info.baseType.unlocalizedName = "";
						info.baseType.name = "";
						info.baseType.fullName = "";
						info.baseType.namespaceName = "";
						info.baseType.genericParameters = new GenericParametersInfo[0];
					} break;
					default: {
						info.baseType = QuickTypeInfo.GenerateInfo(type.BaseType);
					} break;
				}
			}
			else {
				info.baseType = new QuickTypeInfo();
				info.baseType.unlocalizedName = "";
				info.baseType.name = "";
				info.baseType.fullName = "";
				info.baseType.namespaceName = "";
				info.baseType.genericParameters = new GenericParametersInfo[0];
			}
			info.interfaces = GenerateInteraceInfoArray(type.Interfaces);
			info.constructors = MethodInfo.GenerateInfoArray(type, false, false, true);
			info.fields = FieldInfo.GenerateInfoArray(type.Fields);
			info.properties = PropertyInfo.GenerateInfoArray(type, true, false);
			info.staticProperties = PropertyInfo.GenerateInfoArray(type, false, true);
			info.events = EventInfo.GenerateInfoArray(type, true, false);
			info.staticEvents = EventInfo.GenerateInfoArray(type, false, true);
			info.methods = MethodInfo.GenerateInfoArray(type, true, false);
			info.staticMethods = MethodInfo.GenerateInfoArray(type, false, true);
			info.operators = MethodInfo.GenerateInfoArray(type, true, true, false, true);
			info.fullDeclaration = GetFullDeclaration(info, type);
			
			return info;
		}
		
		public static string GetFullDeclaration(TypeInfo info, TypeDefinition type) {
			// Variables
			bool hasInheritance = (info.baseType.fullName != "" || info.interfaces.Length > 0);
			string decl = info.declaration + (hasInheritance ? " : " : "");
			
			if(info.baseType.fullName != "") {
				decl += info.baseType.name + (info.interfaces.Length > 0 ? ", " : "");
			}
			if(info.interfaces.Length > 0) {
				for(int i = 0; i < info.interfaces.Length; i++) {
					decl += info.interfaces[i].name + (i != info.interfaces.Length - 1 ? ", " : "");
				}
				if(info.typeInfo.genericParameters.Length > 0) {
					foreach(GenericParametersInfo generic in info.typeInfo.genericParameters) {
						if(generic.constraints.Length == 0) {
							continue;
						}
						decl += $" where { generic.name } : ";
						for(int i = 0; i < generic.constraints.Length; i++) {
							decl += generic.constraints[i].name + (i != generic.constraints.Length - 1 ? ", " : "");
						}
					}
				}
			}
			
			return decl;
		}
		
		public static QuickTypeInfo[] GenerateInteraceInfoArray(Collection<InterfaceImplementation> interfaces) {
			// Variables
			List<QuickTypeInfo> results = new List<QuickTypeInfo>();
			QuickTypeInfo info;
			
			foreach(InterfaceImplementation iface in interfaces) {
				info = QuickTypeInfo.GenerateInfo(iface.InterfaceType);
				if(ignorePrivate && !IsTypePublic(info.unlocalizedName, assembliesUsed)) {
					continue;
				}
				results.Add(QuickTypeInfo.GenerateInfo(iface.InterfaceType));
			}
			
			return results.ToArray();
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
