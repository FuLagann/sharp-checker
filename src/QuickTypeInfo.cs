
using Mono.Cecil;

namespace SharpChecker {
	/// <summary>
	/// All the important information on identifying a type
	/// </summary>
	public class QuickTypeInfo {
		#region Field Variables
		// Variables
		/// <summary>
		/// The unlocalized name that can be found in the dll or xml documentation
		/// </summary>
		public string unlocalizedName;
		/// <summary>
		/// A human readable name of the type
		/// </summary>
		public string name;
		/// <summary>
		/// A human readable name of the type including it's namespace
		/// </summary>
		public string fullName;
		/// <summary>
		/// The namespace where the type came from
		/// </summary>
		public string namespaceName;
		
		#endregion // Field Variables
		
		#region Public Static Methods
		
		public static QuickTypeInfo GenerateInfo(TypeDefinition type) {
			// Variables
			QuickTypeInfo info = new QuickTypeInfo();
			string[] generics = TypeInfo.GetGenericParametersString(
				type.GenericParameters.ToArray()
			);
			
			info.unlocalizedName = type.FullName;
			info.name = TypeInfo.LocalizeName(type.Name, generics);
			info.fullName = TypeInfo.LocalizeName(type.FullName, generics);
			info.namespaceName = type.Namespace;
			
			return info;
		}
		
		public static QuickTypeInfo GenerateInfo(TypeReference type) {
			// Variables
			QuickTypeInfo info = new QuickTypeInfo();
			string[] generics = TypeInfo.GetGenericParametersString(
				type.GenericParameters.ToArray()
			);
			
			info.unlocalizedName = type.FullName;
			info.name = TypeInfo.LocalizeName(type.Name, generics);
			info.fullName = TypeInfo.LocalizeName(type.FullName, generics);
			info.namespaceName = type.Namespace;
			
			return info;
		}
		
		#endregion // Public Static Methods
	}
}
