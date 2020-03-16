
using Mono.Cecil;

using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace SharpChecker {
	/// <summary>A quick look into the information of the type</summary>
	public class QuickTypeInfo {
		#region Field Variables
		// Variables
		/// <summary>The name of the type as found within the library's IL code</summary>
		/// <remarks>The character ` means that it has generic parameters</remarks>
		public string unlocalizedName;
		/// <summary>The name of the type as found when looking at the code</summary>
		/// <remarks>
		/// If there are any generic parameters, it will display it as a developer would declare it
		/// </remarks>
		public string name;
		/// <summary>
		/// The full name of the type as found when looking at the code.
		/// Includes the namespace and the name within this variable
		/// </summary>
		public string fullName;
		/// <summary>The name of the namespace where the type is located in</summary>
		public string namespaceName;
		/// <summary>The list of generic parameters that the type contains</summary>
		public GenericParametersInfo[] genericParameters;
		// The pattern for removing the unlocalized generic parameters
		private const string pattern = @"`\d+";
		// The hash map of the changes from the managed types to primitives to make it easier to read for type
		private static readonly Dictionary<string, string> changes = new Dictionary<string, string>(new KeyValuePair<string, string>[] {
			new KeyValuePair<string, string>("System.Boolean", "bool"),
			new KeyValuePair<string, string>("System.Byte", "byte"),
			new KeyValuePair<string, string>("System.SByte", "sbyte"),
			new KeyValuePair<string, string>("System.UInt16", "ushort"),
			new KeyValuePair<string, string>("System.Int16", "short"),
			new KeyValuePair<string, string>("System.UInt32", "uint"),
			new KeyValuePair<string, string>("System.Int32", "int"),
			new KeyValuePair<string, string>("System.UInt64", "ulong"),
			new KeyValuePair<string, string>("System.Int64", "long"),
			new KeyValuePair<string, string>("System.Single", "float"),
			new KeyValuePair<string, string>("System.Double", "double"),
			new KeyValuePair<string, string>("System.Decimal", "decimal"),
			new KeyValuePair<string, string>("System.String", "string"),
			new KeyValuePair<string, string>("System.Object", "object"),
			new KeyValuePair<string, string>("System.ValueType", "struct"),
			new KeyValuePair<string, string>("System.Enum", "enum"),
			new KeyValuePair<string, string>("System.Void", "void"),
		});
		
		#endregion // Field Variables
		
		#region Public Static Methods
		
		/// <summary>Generates the information for a quick look into the type</summary>
		/// <param name="type">The type definition to look into</param>
		/// <returns>Returns a quick look at the type information</returns>
		public static QuickTypeInfo GenerateInfo(TypeDefinition type) {
			// Variables
			QuickTypeInfo info = new QuickTypeInfo();
			string[] generics = TypeInfo.GetGenericParametersString(
				type.GenericParameters.ToArray()
			);
			
			GetNames(
				type.FullName.Replace("&", ""),
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
		
		/// <summary>Generates the information for a quick look into the type</summary>
		/// <param name="type">The type reference to look into</param>
		/// <returns>Returns a quick look at the type information</returns>
		public static QuickTypeInfo GenerateInfo(TypeReference type) {
			// Variables
			QuickTypeInfo info = new QuickTypeInfo();
			string[] generics = TypeInfo.GetGenericParametersString(
				type.GenericParameters.ToArray()
			);
			
			GetNames(
				type.FullName.Replace("&", ""),
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
		
		/// <summary>Makes the names of managed types of primitives into the names of primitives</summary>
		/// <param name="name">The name of the type</param>
		/// <returns>
		/// Returns the name of the primitive or the type depending if it's a managed
		/// version of the type
		/// </returns>
		public static string MakeNameFriendly(string name) {
			// Variables
			string temp = name;
			
			foreach(KeyValuePair<string, string> keyval in changes) {
				temp = temp.Replace(keyval.Key, keyval.Value);
			}
			
			return temp;
		}
		
		/// <summary>Deletes the namespace full the given name</summary>
		/// <param name="name">The name of the type</param>
		/// <returns>Returns a string with any namespaces being removed</returns>
		public static string DeleteNamespaceFromType(string name) {
			// Variables
			const string pattern1 = @"(<[a-zA-Z0-9<>]+>)+(?=\.)";
			const string pattern2 = @"([a-zA-Z0-9]+\.)+";
			
			return Regex.Replace(Regex.Replace(Regex.Replace(name, pattern1, ""), pattern2, ""), @",(\w)", ", $1");
		}
		
		/// <summary>Gets the list of generic parameter names from the full name of the type</summary>
		/// <param name="fullName">The full name of the type</param>
		/// <returns>Returns the list of generic parameter names</returns>
		public static string[] GetGenericParametersAsStrings(string fullName) {
			// Variables
			GenericParametersInfo[] infos = GetGenericParameters(fullName);
			string[] results = new string[infos.Length];
			int i = 0;
			
			foreach(GenericParametersInfo info in infos) {
				results[i] = info.name.Replace(",", ", ");
				foreach(KeyValuePair<string, string> keyval in changes) {
					results[i] = Regex.Replace(results[i], keyval.Key, keyval.Value);
				}
				i++;
			}
			
			return results;
		}
		
		/// <summary>Gets the list of information of generic parameters from the full name of the type</summary>
		/// <param name="fullName">The full name of the type</param>
		/// <returns>Returns the list of information of generic parameters</returns>
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
					info.unlocalizedName = GenericParametersInfo.UnlocalizeName(info.name);
					info.name = MakeNameFriendly(info.name);
					info.constraints = new QuickTypeInfo[0];
					results.Add(info);
					curr = i + 1;
				}
			}
			
			info = new GenericParametersInfo();
			info.name = Regex.Replace(fullName.Substring(curr, gt - curr), pattern, "");
			info.unlocalizedName = GenericParametersInfo.UnlocalizeName(info.name);
			info.name = MakeNameFriendly(info.name);
			info.constraints = new QuickTypeInfo[0];
			results.Add(info);
			
			return results.ToArray();
		}
		
		#endregion // Public Static Methods
		
		#region Private Static Methods
		
		/// <summary>
		/// Gathers all the names for the type information using the type's full
		/// name and namespace
		/// </summary>
		/// <param name="typeFullName">The full name of the type</param>
		/// <param name="typeNamespace">The namespace of the type</param>
		/// <param name="generics">The list of generic strings</param>
		/// <param name="unlocalizedName">The resulting unlocalized name of the type</param>
		/// <param name="fullName">The resulting full name of the type</param>
		/// <param name="namespaceName">The resulting namespace of the type</param>
		/// <param name="name">The resulting name of the type</param>
		private static void GetNames(
			string typeFullName, string typeNamespace, string[] generics,
			out string unlocalizedName, out string fullName,
			out string namespaceName, out string name
		) {
			// Variables
			int index = typeFullName.IndexOf('<');
			
			unlocalizedName = (index == -1 ? typeFullName : typeFullName.Substring(0, index));
			fullName = Regex.Replace(TypeInfo.LocalizeName(typeFullName.Replace("/", "."), generics), pattern, "");
			namespaceName = typeNamespace;
			name = DeleteNamespaceFromType(MakeNameFriendly(fullName));
		}
		
		#endregion // Private Static Methods
	}
}
