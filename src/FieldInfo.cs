
using Mono.Cecil;
using Mono.Collections.Generic;

using System.Collections.Generic;
using Reflection = System.Reflection;

namespace SharpChecker {
	/// <summary>All the information relevant to fields</summary>
	public class FieldInfo {
		#region Field Variables
		// Variables
		/// <summary>The name of the field</summary>
		public string name;
		/// <summary>The value of the field (if it's a constant)</summary>
		public string value;
		/// <summary>Set to true if the field is constant</summary>
		public bool isConstant;
		/// <summary>Set to true if the field is static</summary>
		public bool isStatic;
		/// <summary>Set to true if the field is readonly</summary>
		public bool isReadonly;
		/// <summary>The list of attributes that the field contains</summary>
		public AttributeInfo[] attributes;
		/// <summary>The accessor of the field (such as internal, private, protected, public)</summary>
		public string accessor;
		/// <summary>Any modifiers to the field (such as static, const, static readonly, etc)</summary>
		public string modifier;
		/// <summary>The type information of the field's type</summary>
		public QuickTypeInfo typeInfo;
		/// <summary>The declaration of the field as it is found witihn the code</summary>
		public string declaration;
		/// <summary>If it's true then the info should not be printed out and should be deleted</summary>
		internal bool shouldDelete = false;
		
		#endregion // Field Variables
		
		#region Public Static Methods
		
		/// <summary>
		/// Generates an array of field informations by locating it recursively and
		/// excluding or including static members
		/// </summary>
		/// <param name="type">The type definition to look into the fields</param>
		/// <param name="recursive">
		/// Set to true if it should look into the base type recursively to find
		/// base members
		/// </param>
		/// <param name="isStatic">Set to true to look for only static members</param>
		/// <returns>Returns the list of field informations</returns>
		public static FieldInfo[] GenerateInfoArray(TypeDefinition type, bool recursive, bool isStatic) {
			if(!recursive) {
				FieldInfo[] results = GenerateInfoArray(type.Fields);
				
				RemoveUnwanted(ref results, isStatic, true);
				
				return results;
			}
			
			// Variables
			List<FieldInfo> methods = new List<FieldInfo>();
			FieldInfo[] temp;
			TypeDefinition currType = type;
			TypeReference baseType;
			bool isOriginal = true;
			
			while(currType != null) {
				temp = GenerateInfoArray(currType.Fields);
				RemoveUnwanted(ref temp, isStatic, isOriginal);
				if(currType != type) {
					RemoveDuplicates(ref temp, methods);
				}
				methods.AddRange(temp);
				baseType = currType.BaseType;
				if(baseType == null) {
					break;
				}
				currType = baseType.Resolve();
				isOriginal = false;
			}
			
			return methods.ToArray();
		}
		
		/// <summary>
		/// Generates an array of field informations from a collection of field definitions
		/// </summary>
		/// <param name="fields">The field definition to look into</param>
		/// <returns>Returns the list of field informations</returns>
		public static FieldInfo[] GenerateInfoArray(Collection<FieldDefinition> fields) {
			// Variables
			List<FieldInfo> results = new List<FieldInfo>();
			FieldInfo info;
			
			foreach(FieldDefinition field in fields) {
				if(field.Name == "value__") {
					continue;
				}
				else if(IsCompilerGenerated(field)) {
					continue;
				}
				info = GenerateInfo(field);
				if(info.shouldDelete) {
					continue;
				}
				results.Add(info);
			}
			
			return results.ToArray();
		}
		
		/// <summary>Generates the information for the field from the field definition</summary>
		/// <param name="field">The field definition to look into</param>
		/// <returns>Returns the information of the field</returns>
		public static FieldInfo GenerateInfo(FieldDefinition field) {
			// Variables
			FieldInfo info = new FieldInfo();
			string val = System.Text.ASCIIEncoding.ASCII.GetString(field.InitialValue);
			
			if(field.IsAssembly) { info.accessor = "internal"; }
			else if(field.IsFamily) { info.accessor = "protected"; }
			else if(field.IsPrivate) { info.accessor = "private"; }
			else { info.accessor = "public"; }
			if(TypeInfo.ignorePrivate && PropertyInfo.GetAccessorId(info.accessor) == 0) {
				info.shouldDelete = true;
				return info;
			}
			info.name = field.Name;
			info.typeInfo = QuickTypeInfo.GenerateInfo(field.FieldType);
			info.value = $"{ field.Constant ?? val }";
			info.isConstant = field.HasConstant;
			info.isStatic = field.IsStatic;
			info.isReadonly = field.IsInitOnly;
			info.attributes = AttributeInfo.GenerateInfoArray(field.CustomAttributes);
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
			if(info.isConstant) {
				info.declaration += $" = { info.value }";
			}
			
			return info;
		}
		
		#endregion // Public Static Methods
		
		#region Private Static Methods
		
		/// <summary>
		/// Finds if the field is compiler generated, meaning it's used for
		/// properties
		/// </summary>
		/// <param name="field">The field information to look into</param>
		/// <returns>Returns true if the field is compiler generated</returns>
		private static bool IsCompilerGenerated(FieldDefinition field) {
			foreach(CustomAttribute attr in field.CustomAttributes) {
				if(attr.AttributeType.FullName == "System.Runtime.CompilerServices.CompilerGeneratedAttribute") {
					return true;
				}
			}
			
			return false;
		}
		
		/// <summary>Removes any unwanted elements witihn the list</summary>
		/// <param name="temp">The list of field informations to look into</param>
		/// <param name="isStatic">Set to true to remove any non-static members</param>
		/// <param name="isOriginal">Set to false if it's a base type, this will remove any private members</param>
		private static void RemoveUnwanted(ref FieldInfo[] temp, bool isStatic, bool isOriginal) {
			// Variables
			List<FieldInfo> fields = new List<FieldInfo>(temp);
			
			for(int i = temp.Length - 1; i >= 0; i--) {
				if(fields[i].shouldDelete) {
					fields.RemoveAt(i);
				}
				else if(fields[i].isStatic != isStatic) {
					fields.RemoveAt(i);
				}
				else if(!isOriginal && fields[i].accessor == "private") {
					fields.RemoveAt(i);
				}
			}
			
			temp = fields.ToArray();
		}
		
		/// <summary>Removes any duplicates witihn the list</summary>
		/// <param name="temp">The list of field informations to remove duplicates from</param>
		/// <param name="listFields">The list of fields that have already been recorded</param>
		private static void RemoveDuplicates(ref FieldInfo[] temp, List<FieldInfo> listFields) {
			// Variables
			List<FieldInfo> fields = new List<FieldInfo>(temp);
			
			for(int i = temp.Length - 1; i >= 0; i--) {
				foreach(FieldInfo field in listFields) {
					if(fields[i].name == field.name) {
						fields.RemoveAt(i);
						break;
					}
				}
			}
			
			temp = fields.ToArray();
		}
		
		#endregion // Private Static Methods
	}
}
