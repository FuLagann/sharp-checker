
using Mono.Cecil;

using System.Collections.Generic;
using System.Text.RegularExpressions;

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
		public GenericParametersInfo[] genericParameters;
		private const string pattern = @"`\d+";
		
		#endregion // Field Variables
		
		#region Public Static Methods
		
		public static QuickTypeInfo GenerateInfo(TypeDefinition type) {
			// Variables
			QuickTypeInfo info = new QuickTypeInfo();
			string[] generics = TypeInfo.GetGenericParametersString(
				type.GenericParameters.ToArray()
			);
			
			GetNames(
				type.FullName,
				type.Namespace,
				generics,
				out info.unlocalizedName,
				out info.fullName,
				out info.namespaceName,
				out info.name
			);
			info.genericParameters = GenericParametersInfo.GenerateInfoArray(type.GenericParameters);
			if(info.genericParameters.Length == 0) {
				info.genericParameters = GetGenericParameters(type.FullName);
			}
			
			return info;
		}
		
		public static QuickTypeInfo GenerateInfo(TypeReference type) {
			// Variables
			QuickTypeInfo info = new QuickTypeInfo();
			string[] generics = TypeInfo.GetGenericParametersString(
				type.GenericParameters.ToArray()
			);
			
			GetNames(
				type.FullName,
				type.Namespace,
				generics,
				out info.unlocalizedName,
				out info.fullName,
				out info.namespaceName,
				out info.name
			);
			info.genericParameters = GenericParametersInfo.GenerateInfoArray(type.GenericParameters);
			if(info.genericParameters.Length == 0) {
				info.genericParameters = GetGenericParameters(type.FullName);
			}
			
			return info;
		}
		
		public static void GetNames(string typeFullName, string typeNamespace, string[] generics, out string unlocalizedName, out string fullName, out string namespaceName, out string name) {
			// Variables
			int index = typeFullName.IndexOf('<');
			
			unlocalizedName = (index == -1 ? typeFullName : typeFullName.Substring(0, index));
			fullName = Regex.Replace(TypeInfo.LocalizeName(typeFullName, generics), pattern, "");
			namespaceName = typeNamespace;
			name = DeleteNamespacesInGenerics(fullName);
			name = MakeNameFriendly(fullName);
		}
		
		public static string MakeNameFriendly(string name) {
			// Variables
			string temp = name;
			bool endsWith = name.EndsWith("[]");
			
			if(endsWith) {
				temp = temp.Substring(0, temp.Length - 2);
			}
			switch(temp) {
				case "System.Byte": { temp = "byte"; } break;
				case "System.SByte": { temp = "sbyte"; } break;
				case "System.UInt16": { temp = "ushort"; } break;
				case "System.Int16": { temp = "short"; } break;
				case "System.UInt32": { temp = "uint"; } break;
				case "System.Int32": { temp = "int"; } break;
				case "System.UInt64": { temp = "ulong"; } break;
				case "System.Int64": { temp = "long"; } break;
				case "System.Single": { temp = "float"; } break;
				case "System.Double": { temp = "double"; } break;
				case "System.Decimal": { temp = "decimal"; } break;
				case "System.String": { temp = "string"; } break;
				case "System.Object": { temp = "object"; } break;
				case "System.Void": return "void";
			}
			
			return temp + (endsWith ? "[]" : "");
		}
		
		public static string DeleteNamespacesInGenerics(string name) {
			// Variables
			const string _pattern = @"([a-zA-Z0-9]+\.)+";
			
			return Regex.Replace(name, _pattern, "").Replace(",", ", ");
		}
		
		public static string[] GetGenericParametersAsStrings(string fullName) {
			// Variables
			GenericParametersInfo[] infos = GetGenericParameters(fullName);
			string[] results = new string[infos.Length];
			int i = 0;
			
			foreach(GenericParametersInfo info in infos) {
				// TODO: Make friendly replacement for generics
				// Example: "System.Collections.Generic.List<System.Collections.Generic.Dictionary<Dummy.IDummy,System.String>>"
				results[i++] = MakeNameFriendly(info.name);
			}
			
			return results;
		}
		
		public static GenericParametersInfo[] GetGenericParameters(string fullName) {
			// Variables
			int lt = fullName.IndexOf('<');
			if(lt == -1) {
				return new GenericParametersInfo[0];
			}
			List<GenericParametersInfo> results = new List<GenericParametersInfo>();
			GenericParametersInfo info;
			int gt = fullName.LastIndexOf('>');
			int scope = 0;
			int curr = lt + 1;
			
			for(int i = curr; i < gt; i++) {
				if(fullName[i] == '<') { scope++; }
				else if(fullName[i] == '>') { scope--; }
				else if(fullName[i] == ',' && scope == 0) {
					// Variables
					info = new GenericParametersInfo();
					info.name = Regex.Replace(fullName.Substring(curr, i - curr), pattern, "");
					info.constraints = new QuickTypeInfo[0];
					info.unlocalizedName = GenericParametersInfo.UnlocalizeName(info.name);
					results.Add(info);
					curr = i + 1;
				}
			}
			
			info = new GenericParametersInfo();
			info.name = Regex.Replace(fullName.Substring(curr, gt - curr), pattern, "");
			info.constraints = new QuickTypeInfo[0];
			info.unlocalizedName = GenericParametersInfo.UnlocalizeName(info.name);
			results.Add(info);
			
			return results.ToArray();
		}
		
		#endregion // Public Static Methods
	}
}
