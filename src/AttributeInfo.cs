
using Mono.Cecil;
using Mono.Collections.Generic;

namespace SharpChecker {
	public class AttributeInfo {
		// Variables
		public QuickTypeInfo typeInfo;
		public AttributeFieldInfo[] fields;
		
		public static AttributeInfo[] GenerateInfoArray(Collection<CustomAttribute> attrs) {
			// Variables
			AttributeInfo[] results = new AttributeInfo[attrs.Count];
			int i = 0;
			
			foreach(CustomAttribute attr in attrs) {
				results[i++] = GenerateInfo(attr);
			}
			
			return results;
		}
		
		public static AttributeInfo GenerateInfo(CustomAttribute attr) {
			// Variables
			AttributeInfo info = new AttributeInfo();
			int i = 0;
			
			info.typeInfo = QuickTypeInfo.GenerateInfo(attr.AttributeType);
			info.fields = new AttributeFieldInfo[attr.Fields.Count];
			foreach(CustomAttributeNamedArgument field in attr.Fields) {
				info.fields[i] = new AttributeFieldInfo();
				info.fields[i].typeInfo = QuickTypeInfo.GenerateInfo(field.Argument.Type);
				info.fields[i].name = field.Name;
				info.fields[i++].value = $"{ field.Argument.Value }";
			}
			
			return info;
		}
		
		public class AttributeFieldInfo {
			// Variables
			public QuickTypeInfo typeInfo;
			public string name;
			public string value;
		}
	}
}
