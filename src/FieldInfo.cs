
using Mono.Cecil;
using Mono.Collections.Generic;

using System.Collections.Generic;
using Reflection = System.Reflection;

namespace SharpChecker {
	public class FieldInfo {
		// Variables
		public string name;
		public string value;
		public bool isConstant;
		public bool isStatic;
		public bool isReadonly;
		public string accessor;
		public string modifier;
		public string declaration;
		public QuickTypeInfo typeInfo;
		public AttributeInfo[] attributes;
		
		public static FieldInfo[] GenerateInfoArray(Collection<FieldDefinition> fields) {
			// Variables
			List<FieldInfo> results = new List<FieldInfo>();
			
			foreach(FieldDefinition field in fields) {
				if(field.Name == "value__") {
					continue;
				}
				if(IsCompilerGenerated(field)) {
					continue;
				}
				results.Add(GenerateInfo(field));
			}
			
			return results.ToArray();
		}
		
		public static FieldInfo GenerateInfo(FieldDefinition field) {
			// Variables
			FieldInfo info = new FieldInfo();
			string val = System.Text.ASCIIEncoding.ASCII.GetString(field.InitialValue);
			
			info.name = field.Name;
			info.typeInfo = QuickTypeInfo.GenerateInfo(field.FieldType);
			info.value = $"{ field.Constant ?? val }";
			info.isConstant = field.HasConstant;
			info.isStatic = field.IsStatic;
			info.isReadonly = field.IsInitOnly;
			info.attributes = AttributeInfo.GenerateInfoArray(field.CustomAttributes);
			if(field.IsAssembly) { info.accessor = "internal"; }
			else if(field.IsFamily) { info.accessor = "protected"; }
			else if(field.IsPrivate) { info.accessor = "private"; }
			else { info.accessor = "public"; }
			if(field.HasConstant) { info.modifier = "const"; }
			else if(field.IsStatic && field.IsInitOnly) { info.modifier = "static readonly"; }
			else if(field.IsStatic) { info.modifier = "static"; }
			else if(field.IsInitOnly) { info.modifier = "readonly"; }
			else { info.modifier = ""; }
			info.declaration = (
				$"{ info.accessor } " +
				(info.modifier != "" ? info.modifier + " " : "") +
				$"{ info.typeInfo.name } " +
				info.name
			);
			
			return info;
		}
		
		public static bool IsCompilerGenerated(FieldDefinition field) {
			foreach(CustomAttribute attr in field.CustomAttributes) {
				if(attr.AttributeType.FullName == "System.Runtime.CompilerServices.CompilerGeneratedAttribute") {
					return true;
				}
			}
			
			return false;
		}
	}
}
