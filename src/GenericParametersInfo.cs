
using Mono.Cecil;
using Mono.Collections.Generic;

namespace SharpChecker {
	public class GenericParametersInfo {
		// Variables
		public string unlocalizedName;
		public string name;
		public QuickTypeInfo[] constraints;
		
		public static GenericParametersInfo[] GenerateInfoArray(Collection<GenericParameter> generics) {
			// Variables
			GenericParametersInfo[] results = new GenericParametersInfo[generics.Count];
			int i = 0;
			
			foreach(GenericParameter generic in generics) {
				results[i++] = GenerateInfo(generic);
			}
			
			return results;
		}
		
		public static GenericParametersInfo GenerateInfo(GenericParameter generic) {
			// Variables
			GenericParametersInfo info = new GenericParametersInfo();
			int i = 0;
			
			info.unlocalizedName = UnlocalizeName(generic.Name);
			info.name = QuickTypeInfo.MakeNameFriendly(generic.Name);
			info.constraints = new QuickTypeInfo[generic.Constraints.Count];
			foreach(GenericParameterConstraint constraint in generic.Constraints) {
				info.constraints[i++] = QuickTypeInfo.GenerateInfo(constraint.ConstraintType);
			}
			
			return info;
		}
		
		public static string UnlocalizeName(string name) {
			// Variables
			int lt = name.IndexOf('<');
			if(lt == -1) {
				return name;
			}
			int gt = name.LastIndexOf('>');
			int scope = 0;
			int count = 1;
			
			for(int i = lt + 1; i < gt; i++) {
				if(name[i] == '<') { scope++; }
				else if(name[i] == '>') { scope--; }
				else if(name[i] == ',' && scope == 0) { count++; }
			}
			
			return $"{ name.Substring(0, lt) }`{ count }";
		}
	}
}
