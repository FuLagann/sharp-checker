
using Mono.Cecil;
using Mono.Collections.Generic;

using Newtonsoft.Json;

using System.Collections.Generic;
using System.IO;

using Reflection = System.Reflection;

namespace SharpChecker {
	/// <summary>All the information relevant to types</summary>
	public class TypeInfo {
		#region Field Variables
		// Variables
		/// <summary>The quick look at the information of the type (including name, namespace, generic parameters)</summary>
		public QuickTypeInfo typeInfo;
		/// <summary>The name of the assembly where the type is found in</summary>
		public string assemblyName;
		/// <summary>The accessor of the type (such as internal, private, protected, public)</summary>
		public string accessor;
		/// <summary>Any modifiers that the type contains (such as static, sealed, abstract, etc.)</summary>
		public string modifier;
		/// <summary>The object type of the type (such as class, struct, enum, or interface)</summary>
		public string objectType;
		/// <summary>The partial declaration of the class within the inheritance declaration that can be found within the code</summary>
		public string declaration;
		/// <summary>The full declaration of the type as it would be found within the code</summary>
		public string fullDeclaration;
		/// <summary>The information of the base type that the type inherits</summary>
		public QuickTypeInfo baseType;
		/// <summary>The array of attributes that the type contains</summary>
		public AttributeInfo[] attributes;
		/// <summary>The array of type information of interfaces that the type implements</summary>
		public QuickTypeInfo[] interfaces;
		/// <summary>The array of constructors that the type contains</summary>
		public MethodInfo[] constructors;
		/// <summary>The array of fields that the type contains</summary>
		public FieldInfo[] fields;
		/// <summary>The array of static fields that the type contains</summary>
		public FieldInfo[] staticFields;
		/// <summary>The array of properties that the type contains</summary>
		public PropertyInfo[] properties;
		/// <summary>The array of static properties that the type contains</summary>
		public PropertyInfo[] staticProperties;
		/// <summary>The array of events that the type contains</summary>
		public EventInfo[] events;
		/// <summary>The array of static events that the type contains</summary>
		public EventInfo[] staticEvents;
		/// <summary>The array of static methods that the type contains</summary>
		public MethodInfo[] methods;
		/// <summary>The array of static methods that the type contains</summary>
		public MethodInfo[] staticMethods;
		/// <summary>The array of operators that the type contains</summary>
		public MethodInfo[] operators;
		// The name of the assembly used
		internal static string assemblyUsed = "";
		// The array of assemblies that the user wanted to look into
		internal static string[] assembliesUsed;
		// Set to true to ignore all private members
		internal static bool ignorePrivate = false;
		
		#endregion // Field Variables
		
		#region Public Static Methods
		
		/// <summary>Sets whether the program should ignore private methods</summary>
		/// <param name="isPrivate">Set to true to ignore all private members</param>
		public static void SetIgnorePrivate(bool isPrivate) { ignorePrivate = isPrivate; }
		
		/// <summary>Generates the type information from a list of assemblies with a safe check</summary>
		/// <param name="assemblies">The list of assemblies to look into</param>
		/// <param name="typePath">The type path to look into</param>
		/// <param name="info">The resulting type information that is generated</param>
		/// <returns>Returns true if the type information is found</returns>
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
		
		/// <summary>Generates a type information from the given type definition</summary>
		/// <param name="asm">The assembly definition where the type came from</param>
		/// <param name="type">The type definition to look into</param>
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
			info.fields = FieldInfo.GenerateInfoArray(type, true, false);
			info.staticFields = FieldInfo.GenerateInfoArray(type, false, true);
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
		
		/// <summary>Localizes the name using the list of generic parameter names</summary>
		/// <param name="name">The name of the type</param>
		/// <param name="generics">The array of generic parameter names</param>
		/// <returns>Returns the localized name</returns>
		public static string LocalizeName(string name, string[] generics) {
			if(generics.Length == 0) {
				return name;
			}
			
			return (
				name.Substring(0, name.LastIndexOf('`')) + "<" +
				string.Join(", ", generics) + ">"
			);
		}
		
		/// <summary>Gets an array of generic parameter names from the given array of generic parameters</summary>
		/// <param name="generics">The array of generic parameters</param>
		/// <returns>Returns an array of generic parameter names</returns>
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
		
		#endregion // Public Static Methods
		
		#region Private Static Methods
		
		/// <summary>Gets the full declaration of the type</summary>
		/// <param name="info">The type information to look into</param>
		/// <param name="type">The type definition to look into</param>
		/// <returns>Returns the full declaration of the type</returns>
		private static string GetFullDeclaration(TypeInfo info, TypeDefinition type) {
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
		
		/// <summary>Generates an array of interface informations</summary>
		/// <param name="interfaces">The collection of interface implementations</param>
		/// <returns>Returns an array of interface informations</returns>
		private static QuickTypeInfo[] GenerateInteraceInfoArray(Collection<InterfaceImplementation> interfaces) {
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
		
		/// <summary>Finds if the type is a public type</summary>
		/// <param name="typePath">The type path to look into</param>
		/// <param name="assemblies">The list of assemblies to look into</param>
		/// <returns>Returns true if the type is public</returns>
		private static bool IsTypePublic(string typePath, string[] assemblies) {
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
		
		#endregion // Private Static Methods
	}
}
