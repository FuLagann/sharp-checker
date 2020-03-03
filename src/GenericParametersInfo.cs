
using Mono.Cecil;
using Mono.Collections.Generic;

namespace SharpChecker {
	public class GenericParametersInfo {
		// Variables
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
			
			info.name = generic.Name;
			info.constraints = new QuickTypeInfo[generic.Constraints.Count];
			foreach(GenericParameterConstraint constraint in generic.Constraints) {
				info.constraints[i++] = QuickTypeInfo.GenerateInfo(constraint.ConstraintType);
			}
			
			return info;
		}
	}
}
