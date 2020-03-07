
using Mono.Cecil;
using Mono.Collections.Generic;

using System.Collections.Generic;

namespace SharpChecker {
	public class EventInfo {
		// Variables
		public string name;
		public string accessor;
		public string modifier;
		public bool isStatic;
		public QuickTypeInfo typeInfo;
		public MethodInfo adder;
		public MethodInfo remover;
		public string declaration;
		public string fullDeclaration;
		internal bool shouldDelete = false;
		
		public static EventInfo[] GenerateInfoArray(TypeDefinition type, bool rec, bool isStatic) {
			if(!rec) {
				EventInfo[] results = GenerateInfoArray(type.Events);
				
				RemoveUnwanted(ref results, isStatic);
				
				return results;
			}
			
			// Variables
			List<EventInfo> events = new List<EventInfo>();
			EventInfo[] temp;
			TypeDefinition currType = type;
			TypeReference baseType;
			
			while(currType != null) {
				temp = GenerateInfoArray(currType.Events);
				RemoveUnwanted(ref temp, isStatic);
				if(currType != type) {
					RemoveDuplicates(ref temp, events);
				}
				events.AddRange(temp);
				baseType = currType.BaseType;
				if(baseType == null) {
					break;
				}
				currType = baseType.Resolve();
			}
			
			return events.ToArray();
		}
		
		public static void RemoveUnwanted(ref EventInfo[] temp, bool isStatic) {
			// Variables
			List<EventInfo> events = new List<EventInfo>(temp);
			
			for(int i = temp.Length - 1; i >= 0; i--) {
				if(events[i].shouldDelete) {
					events.RemoveAt(i);
				}
				else if(events[i].isStatic != isStatic) {
					events.RemoveAt(i);
				}
			}
			
			temp = events.ToArray();
		}
		
		public static void RemoveDuplicates(ref EventInfo[] temp, List<EventInfo> listEvents) {
			// Variables
			List<EventInfo> events = new List<EventInfo>(temp);
			
			for(int i = temp.Length - 1; i >= 0; i--) {
				foreach(EventInfo ev in listEvents) {
					if(events[i].name == ev.name) {
						events.RemoveAt(i);
						break;
					}
				}
			}
			
			temp = events.ToArray();
		}
		
		public static EventInfo[] GenerateInfoArray(Collection<EventDefinition> events) {
			// Variables
			List<EventInfo> results = new List<EventInfo>();
			EventInfo info;
			
			foreach(EventDefinition ev in events) {
				info = GenerateInfo(ev);
				if(info.shouldDelete) {
					continue;
				}
				results.Add(info);
			}
			
			return results.ToArray();
		}
		
		/// <summary>
		/// Generates a method info from the given method definition
		/// </summary>
		/// <param name="method">The method information to look into</param>
		/// <returns>Returns a method info of the method definition provided</returns>
		public static EventInfo GenerateInfo(EventDefinition ev) {
			// Variables
			EventInfo info = new EventInfo();
			
			info.name = ev.Name;
			info.typeInfo = QuickTypeInfo.GenerateInfo(ev.EventType);
			info.adder = MethodInfo.GenerateInfo(ev.AddMethod);
			info.remover = MethodInfo.GenerateInfo(ev.RemoveMethod);
			info.accessor = info.adder.accessor;
			info.modifier = info.adder.modifier;
			info.isStatic = info.adder.isStatic;
			info.declaration = (
				info.accessor + " " +
				(info.modifier != "" ? info.modifier + " " : "") +
				info.typeInfo.name + " " +
				info.name
			);
			info.fullDeclaration = $"{ info.declaration }";
			if(TypeInfo.ignorePrivate && PropertyInfo.GetAccessorId(info.accessor) == 0) {
				info.shouldDelete = true;
			}
			
			return info;
		}
	}
}
