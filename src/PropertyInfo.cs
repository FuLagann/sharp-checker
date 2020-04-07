
using Mono.Cecil;
using Mono.Collections.Generic;

using System.Collections.Generic;

namespace SharpChecker {
	/// <summary>All the information relevant to the property</summary>
	public class PropertyInfo : BaseInfo {
		#region Field Variables
		// Variables
		/// <summary>The name of the property</summary>
		public string name;
		/// <summary>Set to true if the property is static</summary>
		public bool isStatic;
		/// <summary>Set to true if the property has a getter method</summary>
		public bool hasGetter;
		/// <summary>Set to true if the property has a setter method</summary>
		public bool hasSetter;
		/// <summary>The list of attributes associated with the property</summary>
		public AttributeInfo[] attributes;
		/// <summary>The accessor of the property (such as internal, private, protected, public)</summary>
		public string accessor;
		/// <summary>Any modifiers to the property (such as static, virtual, override, etc.)</summary>
		public string modifier;
		/// <summary>The information of the property's type</summary>
		public QuickTypeInfo typeInfo;
		/// <summary>The information of where the property was implemented</summary>
		public QuickTypeInfo implementedType;
		/// <summary>The parameters the property has (if any)</summary>
		public ParameterInfo[] parameters;
		/// <summary>The getter method of the property (this can be null, you must check the hasGetter variable)</summary>
		public MethodInfo getter;
		/// <summary>The setter method of the property (this can be null, you must check the hasSetter variable)</summary>
		public MethodInfo setter;
		/// <summary>The partial declaration of the property as can be found in the code</summary>
		public string declaration;
		/// <summary>The partial declaration of the property's parameters (if any) as can be found in the code</summary>
		public string parameterDeclaration;
		/// <summary>The partial declaration of the property that determines the accessibility of the get and set methods as can be found in the code</summary>
		public string getSetDeclaration;
		/// <summary>The full declaration of the property as can be found in the code</summary>
		public string fullDeclaration;
		// Used to find duplicates
		private string partialFullName;
		// Set to true to delete the property when able to
		internal bool shouldDelete = false;
		internal static int isGeneric = -1;
		internal static TypeDefinition _type = null;
		internal static TypeDefinition _currType = null;
		internal static TypeReference _currTypeRef = null;
		
		#endregion // Field Variables
		
		#region Public Static Methods
		
		/// <summary>Generates an array of property informations from the given type and booleans</summary>
		/// <param name="type">The type to gather information from</param>
		/// <param name="recursive">Set to true to recursively look through base types</param>
		/// <param name="isStatic">Set to true to record only static members</param>
		/// <returns>Returns an array of property informations</returns>
		public static PropertyInfo[] GenerateInfoArray(TypeDefinition type, bool recursive, bool isStatic) {
			if(!recursive) {
				PropertyInfo[] results = GenerateInfoArray(type.Properties);
				
				RemoveUnwanted(ref results, isStatic, true);
				
				return results;
			}
			
			// Variables
			List<PropertyInfo> properties = new List<PropertyInfo>();
			PropertyInfo[] temp;
			TypeDefinition currType = type;
			TypeReference currTypeRef = type.Resolve();
			TypeReference baseType;
			bool isOriginal = true;
			
			while(currType != null) {
				_type = type;
				_currType = currType;
				_currTypeRef = currTypeRef;
				isGeneric = currTypeRef.IsGenericInstance ? 1 : 0;
				temp = GenerateInfoArray(currType.Properties);
				RemoveUnwanted(ref temp, isStatic, isOriginal);
				if(currType != type) {
					RemoveDuplicates(ref temp, properties);
				}
				properties.AddRange(temp);
				baseType = currType.BaseType;
				if(baseType == null) {
					break;
				}
				currTypeRef = baseType;
				currType = baseType.Resolve();
				isOriginal = false;
			}
			
			return properties.ToArray();
		}
		
		/// <summary>Generates an array of property informations from the given collection of property definitions</summary>
		/// <param name="properties">The collection of property definitions</param>
		/// <returns>Returns an array of property informations</returns>
		public static PropertyInfo[] GenerateInfoArray(Collection<PropertyDefinition> properties) {
			// Variables
			List<PropertyInfo> results = new List<PropertyInfo>();
			PropertyInfo info;
			int genericId = isGeneric;
			
			foreach(PropertyDefinition property in properties) {
				isGeneric = genericId;
				info = GenerateInfo(property);
				if(info.shouldDelete) {
					continue;
				}
				results.Add(info);
			}
			
			return results.ToArray();
		}
		
		/// <summary>Generates the property information from the given property definition</summary>
		/// <param name="property">The property information to gather information from</param>
		/// <returns>Returns the property information that's generated</returns>
		public static PropertyInfo GenerateInfo(PropertyDefinition property) {
			// Variables
			PropertyInfo info = new PropertyInfo();
			
			info.hasGetter = (property.GetMethod != null);
			info.hasSetter = (property.SetMethod != null);
			info.getter = (info.hasGetter ?
				(isGeneric != -1 ?
					MethodInfo.GetGenericMethodInfo(_type, _currType, _currTypeRef, property.GetMethod) :
					MethodInfo.GenerateInfo(property.GetMethod)
				) :
				null
			);
			info.setter = (info.hasSetter ?
				(isGeneric != -1 ?
					MethodInfo.GetGenericMethodInfo(_type, _currType, _currTypeRef, property.SetMethod) :
					MethodInfo.GenerateInfo(property.SetMethod)
				) :
				null
			);
			if(info.getter != null && GetAccessorId(info.getter.accessor) == 0) {
				info.getter = null;
				info.hasGetter = false;
			}
			if(info.setter != null && GetAccessorId(info.setter.accessor) == 0) {
				info.setter = null;
				info.hasSetter = false;
			}
			if(!info.hasGetter && !info.hasSetter) {
				info.shouldDelete = true;
				return info;
			}
			info.name = property.Name;
			info.partialFullName = property.FullName.Split("::")[1].Replace(",", ", ");
			info.isStatic = !property.HasThis;
			info.attributes = AttributeInfo.GenerateInfoArray(property.CustomAttributes);
			info.parameters = ParameterInfo.GenerateInfoArray(property.Parameters);
			info.accessor = GetAccessor(info.getter, info.setter);
			if(isGeneric == 1) {
				if(info.hasGetter) {
					info.typeInfo = info.getter.returnType;
					info.parameters = info.getter.parameters;
				}
				else {
					info.typeInfo = info.setter.parameters[info.setter.parameters.Length - 1].typeInfo;
					System.Array.Copy(
						info.setter.parameters,
						info.parameters,
						info.setter.parameters.Length - 1
					);
				}
			}
			else {
				info.typeInfo = QuickTypeInfo.GenerateInfo(property.PropertyType);
			}
			if(!property.HasThis) { info.modifier = "static"; }
			else { info.modifier = GetModifier(info.getter, info.setter); }
			info.implementedType = QuickTypeInfo.GenerateInfo(property.DeclaringType);
			info.getSetDeclaration = GetGetSetDeclaration(info.getter, info.setter, info.accessor);
			info.declaration = (
				info.accessor + " " +
				(info.modifier != "" ? info.modifier + " " : "") +
				info.typeInfo.name + " " +
				(info.parameters.Length == 0 ? info.name : "this")
			);
			info.parameterDeclaration = string.Join(", ", GetParameterDeclarations(info));
			info.fullDeclaration = (
				info.declaration +
				(info.parameterDeclaration != "" ? $"[{ info.parameterDeclaration }]" : "") +
				$" {{ { info.getSetDeclaration } }}"
			);
			
			isGeneric = -1;
			
			return info;
		}
		
		/// <summary>
		/// Gets a quantitative id with the given accessor, with:
		/// * 4 - public
		/// * 3 - protected
		/// * 2 - private
		/// * 1 - internal
		/// * 0 - non-existant
		/// </summary>
		/// <param name="accessor">The accessor to get an id from</param>
		/// <returns>Returns an integer that represents the visibility levels of the accessor</returns>
		public static int GetAccessorId(string accessor) {
			switch(accessor) {
				case "internal": return (TypeInfo.ignorePrivate ? 0 : 1);
				case "private": return (TypeInfo.ignorePrivate ? 0 : 2);
				case "protected": return 3;
				case "public": return 4;
			}
			return 0;
		}
		
		#endregion // Public Static Methods
		
		#region Private Static Methods
		
		/// <summary>Gets the get / set declaration of the property</summary>
		/// <param name="getter">The getter method (can be null)</param>
		/// <param name="setter">The setter method (can be null)</param>
		/// <param name="accessor">The accessor of the property (sohuld be highest visible accessor)</param>
		/// <returns>Returns the get / set declaration of the property</returns>
		private static string GetGetSetDeclaration(MethodInfo getter, MethodInfo setter, string accessor) {
			// Variables
			int infoId = GetAccessorId(accessor);
			int getterId = (getter != null ? GetAccessorId(getter.accessor) : 0);
			int setterId = (setter != null ? GetAccessorId(setter.accessor) : 0);
			string declaration = "";
			
			if(getterId >= infoId) { declaration = "get;"; }
			else if(getterId > 0) { declaration = $"{ GetAccessorFromId(getterId) } get;"; }
			
			if(setterId >= infoId) {
				declaration += (declaration != "" ? " " : "") + "set;";
			}
			else if(setterId > 0) {
				declaration += (declaration != "" ? " " : "") + $"{ GetAccessorFromId(setterId) } set;";
			}
			
			return declaration;
		}
		
		/// <summary>Removes any unwanted elements within the array of property informations</summary>
		/// <param name="temp">The array of property informations to remove from</param>
		/// <param name="isStatic">Set to true to remove all non-static members</param>
		/// <param name="isOriginal">Set to false if it's a base type, this will remove any private members</param>
		private static void RemoveUnwanted(ref PropertyInfo[] temp, bool isStatic, bool isOriginal) {
			// Variables
			List<PropertyInfo> properties = new List<PropertyInfo>(temp);
			
			for(int i = temp.Length - 1; i >= 0; i--) {
				if(properties[i].shouldDelete) {
					properties.RemoveAt(i);
				}
				else if(properties[i].isStatic != isStatic) {
					properties.RemoveAt(i);
				}
				else if(!isOriginal && properties[i].accessor == "private") {
					properties.RemoveAt(i);
				}
			}
			
			temp = properties.ToArray();
		}
		
		/// <summary>Removes any duplicates from the array of property informations</summary>
		/// <param name="temp">The array of property informations to remove from</param>
		/// <param name="listProperties">The list of properties recursiveorded to reference if there are any duplicates</param>
		private static void RemoveDuplicates(ref PropertyInfo[] temp, List<PropertyInfo> listProperties) {
			// Variables
			List<PropertyInfo> properties = new List<PropertyInfo>(temp);
			
			for(int i = temp.Length - 1; i >= 0; i--) {
				foreach(PropertyInfo property in listProperties) {
					if(properties[i].partialFullName == property.partialFullName) {
						properties.RemoveAt(i);
						break;
					}
				}
			}
			
			temp = properties.ToArray();
		}
		
		/// <summary>Gets the accessor from the getter or setter methods</summary>
		/// <param name="getter">The getter method (can be null)</param>
		/// <param name="setter">The setter method (can be null)</param>
		/// <returns>Returns the accessor</returns>
		private static string GetAccessor(MethodInfo getter, MethodInfo setter) {
			// Variables
			int getterId = (getter != null ? GetAccessorId(getter.accessor) : 0);
			int setterId = (setter != null ? GetAccessorId(setter.accessor) : 0);
			
			return GetAccessorFromId(System.Math.Max(getterId, setterId));
		}
		
		/// <summary>Gets the accessor string from a given id (ranging from 0 to 4)</summary>
		/// <param name="id">The id to get the accessor with</param>
		/// <returns>Returns the accessor</returns>
		private static string GetAccessorFromId(int id) {
			switch(id) {
				case 1: return "internal";
				case 2: return "private";
				case 3: return "protected";
				case 4: return "public";
			}
			
			return "";
		}
		
		/// <summary>Gets the modifier of the property from either the getter or the setting</summary>
		/// <param name="getter">The getter method (can be null)</param>
		/// <param name="setter">The setter method (can be null)</param>
		/// <returns>Returns the modifier of the property</returns>
		private static string GetModifier(MethodInfo getter, MethodInfo setter) {
			// Variables
			int getterId = (getter != null ? GetAccessorId(getter.accessor) : 0);
			int setterId = (setter != null ? GetAccessorId(setter.accessor) : 0);
			
			if(getterId != 0 && getterId >= setterId) { return getter.modifier; }
			else if(setterId != 0 && setterId >= getterId) { return setter.modifier; }
			
			return "";
		}
		
		/// <summary>Gets an array of parameter declarations</summary>
		/// <param name="property">The property information to look into</param>
		/// <returns>Returns the array of parameter declarations</returns>
		private static string[] GetParameterDeclarations(PropertyInfo property) {
			// Variables
			string[] declarations = new string[property.parameters.Length];
			int i = 0;
			
			foreach(ParameterInfo parameter in property.parameters) {
				declarations[i++] = parameter.fullDeclaration;
			}
			
			return declarations;
		}
		
		#endregion // Private Static Methods
	}
}
