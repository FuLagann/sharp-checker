
using Mono.Cecil;
using Mono.Collections.Generic;

using System.Collections.Generic;

namespace SharpChecker {
	/// <summary>All the information relevant to events</summary>
	public class EventInfo {
		#region Field Variables
		// Variables
		/// <summary>The name of the event</summary>
		public string name;
		/// <summary>Set to true if the event is static</summary>
		public bool isStatic;
		/// <summary>The accessor of the event (such as internal, private, protected, public)</summary>
		public string accessor;
		/// <summary>Any modifiers of the event (such as static, virtual, override, etc.)</summary>
		public string modifier;
		/// <summary>The information of the event's type</summary>
		public QuickTypeInfo typeInfo;
		/// <summary>The type the event is implemented in</summary>
		public QuickTypeInfo implementedType;
		/// <summary>The information of the event's adding method</summary>
		public MethodInfo adder;
		/// <summary>The information of the event's removing method</summary>
		public MethodInfo remover;
		/// <summary>The declaration of the event as it would be found in the code</summary>
		public string fullDeclaration;
		// Set to true to delete the event when looking to be removed
		internal bool shouldDelete = false;
		
		#endregion // Field Variables
		
		#region Public Static Methods
		
		/// <summary>Generates an array of event informations from the given type and booleans</summary>
		/// <param name="type">The type to look into</param>
		/// <param name="recursive">Set to true to recursively look into the base type of the type</param>
		/// <param name="isStatic">Set to true to look for only static members</param>
		/// <returns></returns>
		public static EventInfo[] GenerateInfoArray(TypeDefinition type, bool recursive, bool isStatic) {
			if(!recursive) {
				EventInfo[] results = GenerateInfoArray(type.Events);
				
				RemoveUnwanted(ref results, isStatic, true);
				
				return results;
			}
			
			// Variables
			List<EventInfo> events = new List<EventInfo>();
			EventInfo[] temp;
			TypeDefinition currType = type;
			TypeReference baseType;
			bool isOriginal = true;
			
			while(currType != null) {
				temp = GenerateInfoArray(currType.Events);
				RemoveUnwanted(ref temp, isStatic, isOriginal);
				if(currType != type) {
					RemoveDuplicates(ref temp, events);
				}
				events.AddRange(temp);
				baseType = currType.BaseType;
				if(baseType == null) {
					break;
				}
				currType = baseType.Resolve();
				isOriginal = false;
			}
			
			return events.ToArray();
		}
		
		/// <summary>Generates an array of event informations from the given collection of event definitions</summary>
		/// <param name="events">The collection of event definitions</param>
		/// <returns>Returns an array of event informations generated</returns>
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
		
		/// <summary>Generates an event information from the given event definition</summary>
		/// <param name="ev">The event definition to gather information from</param>
		/// <returns>Returns the event information generated</returns>
		public static EventInfo GenerateInfo(EventDefinition ev) {
			// Variables
			EventInfo info = new EventInfo();
			
			info.name = ev.Name;
			info.typeInfo = QuickTypeInfo.GenerateInfo(ev.EventType);
			info.implementedType = QuickTypeInfo.GenerateInfo(ev.DeclaringType);
			info.adder = MethodInfo.GenerateInfo(ev.AddMethod);
			info.remover = MethodInfo.GenerateInfo(ev.RemoveMethod);
			info.accessor = info.adder.accessor;
			info.modifier = info.adder.modifier;
			info.isStatic = info.adder.isStatic;
			info.fullDeclaration = (
				info.accessor + " " +
				(info.modifier != "" ? info.modifier + " " : "") +
				info.typeInfo.name + " " +
				info.name
			);
			if(TypeInfo.ignorePrivate && PropertyInfo.GetAccessorId(info.accessor) == 0) {
				info.shouldDelete = true;
			}
			
			return info;
		}
		
		#endregion // Public Static Methods
		
		#region Private Static Methods
		
		/// <summary>Removes any unwanted elements from the array of event informations</summary>
		/// <param name="temp">The array of event informations to remove from</param>
		/// <param name="isStatic">Set to true if non-static members should be removed</param>
		/// <param name="isOriginal">Set to false if it's a base type, this will remove any private members</param>
		public static void RemoveUnwanted(ref EventInfo[] temp, bool isStatic, bool isOriginal) {
			// Variables
			List<EventInfo> events = new List<EventInfo>(temp);
			
			for(int i = temp.Length - 1; i >= 0; i--) {
				if(events[i].shouldDelete) {
					events.RemoveAt(i);
				}
				else if(events[i].isStatic != isStatic) {
					events.RemoveAt(i);
				}
				else if(!isOriginal && events[i].accessor == "private") {
					events.RemoveAt(i);
				}
			}
			
			temp = events.ToArray();
		}
		
		/// <summary>Removes the duplicates from the given array of events</summary>
		/// <param name="temp">The array of event informations that will be removed from</param>
		/// <param name="listEvents">The list of recursiveorded event informations to determine if there is any duplicates</param>
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
		
		#endregion // Private Static Methods
	}
}
