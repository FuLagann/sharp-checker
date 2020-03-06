
using Mono.Cecil;
using Mono.Collections.Generic;

using System.Collections.Generic;

namespace SharpChecker {
	public class PropertyInfo {
		// Variables
		public string name;
		public string accessor;
		public string modifier;
		public QuickTypeInfo typeInfo;
		public AttributeInfo[] attributes;
		public bool hasGetter;
		public bool hasSetter;
		public MethodInfo getter;
		public MethodInfo setter;
		public string getSetDeclaration;
		public string declaration;
		public string fullDeclaration;
		public ParameterInfo[] parameters;
		internal bool shouldDelete = false;
		
		public static PropertyInfo[] GenerateInfoArray(Collection<PropertyDefinition> properties) {
			// Variables
			List<PropertyInfo> results = new List<PropertyInfo>();
			PropertyInfo info;
			
			foreach(PropertyDefinition property in properties) {
				info = GenerateInfo(property);
				if(info.shouldDelete) {
					continue;
				}
				results.Add(info);
			}
			
			return results.ToArray();
		}
		
		/// <summary>
		/// Generates a method info from the given method definition
		/// </summary>
		/// <param name="method">The method information to look into</param>
		/// <returns>Returns a method info of the method definition provided</returns>
		public static PropertyInfo GenerateInfo(PropertyDefinition property) {
			// Variables
			PropertyInfo info = new PropertyInfo();
			
			info.hasGetter = (property.GetMethod != null);
			info.hasSetter = (property.SetMethod != null);
			info.getter = (info.hasGetter ? MethodInfo.GenerateInfo(property.GetMethod) : null);
			info.setter = (info.hasSetter ? MethodInfo.GenerateInfo(property.SetMethod) : null);
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
			info.typeInfo = QuickTypeInfo.GenerateInfo(property.PropertyType);
			info.attributes = AttributeInfo.GenerateInfoArray(property.CustomAttributes);
			info.parameters = ParameterInfo.GenerateInfoArray(property.Parameters);
			info.accessor = GetAccessor(info.getter, info.setter);
			if(!property.HasThis) { info.modifier = "static"; }
			else { info.modifier = GetModifier(info.getter, info.setter); }
			
			info.getSetDeclaration = GetGetSetDeclaration(info.getter, info.setter, info.accessor);
			info.declaration = (
				info.accessor + " " +
				(info.modifier != "" ? info.modifier + " " : "") +
				info.typeInfo.name + " " +
				info.name
			);
			info.fullDeclaration = $"{ info.declaration } {{ { info.getSetDeclaration } }}";
			
			return info;
		}
		
		public static string GetGetSetDeclaration(MethodInfo getter, MethodInfo setter, string accessor) {
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
		
		public static string GetAccessor(MethodInfo getter, MethodInfo setter) {
			// Variables
			int getterId = (getter != null ? GetAccessorId(getter.accessor) : 0);
			int setterId = (setter != null ? GetAccessorId(setter.accessor) : 0);
			
			return GetAccessorFromId(System.Math.Max(getterId, setterId));
		}
		
		public static string GetAccessorFromId(int id) {
			switch(id) {
				case 1: return "internal";
				case 2: return "private";
				case 3: return "protected";
				case 4: return "public";
			}
			
			return "";
		}
		
		public static int GetAccessorId(string accessor) {
			switch(accessor) {
				case "internal": return (TypeInfo.ignorePrivate ? 0 : 1);
				case "private": return (TypeInfo.ignorePrivate ? 0 : 2);
				case "protected": return 3;
				case "public": return 4;
			}
			return 0;
		}
		
		public static string GetModifier(MethodInfo getter, MethodInfo setter) {
			// Variables
			int getterId = (getter != null ? GetAccessorId(getter.accessor) : 0);
			int setterId = (setter != null ? GetAccessorId(setter.accessor) : 0);
			
			if(getterId != 0 && getterId >= setterId) { return getter.modifier; }
			else if(setterId != 0 && setterId >= getterId) { return setter.modifier; }
			
			return "";
		}
	}
}
