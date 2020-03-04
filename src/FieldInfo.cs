
using Mono.Cecil;
using Mono.Collections.Generic;

using System.Collections.Generic;

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
		public QuickTypeInfo typeInfo;
		public AttributeInfo[] attributes;
		
		public static FieldInfo[] GenerateInfoArray(Collection<FieldDefinition> fields) {
			// Variables
			List<FieldInfo> results = new List<FieldInfo>();
			
			foreach(FieldDefinition field in fields) {
				if(field.Name == "value__") {
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
			info.value = $"{ field.Constant ?? val }";
			info.isConstant = field.HasConstant;
			info.isStatic = field.IsStatic;
			info.isReadonly = field.IsInitOnly;
			info.attributes = AttributeInfo.GenerateInfoArray(field.CustomAttributes);
			info.typeInfo = QuickTypeInfo.GenerateInfo(field.FieldType);
			if(field.IsAssembly) { info.accessor = "internal"; }
			else if(field.IsFamily) { info.accessor = "protected"; }
			else if(field.IsPrivate) { info.accessor = "private"; }
			else { info.accessor = "public"; }
			if(field.HasConstant) { info.modifier = "const"; }
			else if(field.IsStatic && field.IsInitOnly) { info.modifier = "static readonly"; }
			else if(field.IsStatic) { info.modifier = "static"; }
			else if(field.IsInitOnly) { info.modifier = "readonly"; }
			else { info.modifier = ""; }
			
			return info;
		}
	}
}
