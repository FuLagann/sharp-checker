
using Mono.Cecil;
using Mono.Collections.Generic;

namespace SharpChecker {
	public class AttributeInfo {
		// Variables
		public QuickTypeInfo typeInfo;
		public AttributeFieldInfo[] constructorArgs;
		public AttributeFieldInfo[] properties;
		
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
			info.constructorArgs = new AttributeFieldInfo[attr.ConstructorArguments.Count];
			foreach(CustomAttributeArgument arg in attr.ConstructorArguments) {
				info.constructorArgs[i] = new AttributeFieldInfo();
				info.constructorArgs[i].typeInfo = QuickTypeInfo.GenerateInfo(arg.Type);
				info.constructorArgs[i].name = attr.Constructor.Parameters[i].Name;
				info.constructorArgs[i++].value = $"{ arg.Value }";
			}
			i = 0;
			info.properties = new AttributeFieldInfo[
				attr.Fields.Count +
				attr.Properties.Count
			];
			foreach(CustomAttributeNamedArgument field in attr.Fields) {
				info.properties[i] = new AttributeFieldInfo();
				info.properties[i].typeInfo = QuickTypeInfo.GenerateInfo(field.Argument.Type);
				info.properties[i].name = field.Name;
				info.properties[i++].value = $"{ field.Argument.Value }";
			}
			foreach(CustomAttributeNamedArgument property in attr.Properties) {
				info.properties[i] = new AttributeFieldInfo();
				info.properties[i].typeInfo = QuickTypeInfo.GenerateInfo(property.Argument.Type);
				info.properties[i].name = property.Name;
				info.properties[i++].value = $"{ property.Argument.Value }";
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
