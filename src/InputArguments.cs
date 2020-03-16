
using System.Collections.Generic;

namespace SharpChecker {
	/// <summary>A managed version of the input arguments</summary>
	public class InputArguments {
		#region Field Variables
		// Variables
		/// <summary>Set to true if the user is querying for the program to produce the help menu</summary>
		public bool isHelp;
		/// <summary>Set to true if the user is querying for the program to produce a list</summary>
		public bool isList;
		/// <summary>Set to true to include private members</summary>
		public bool includePrivate;
		/// <summary>The output file that the program will save to</summary>
		public string output;
		/// <summary>The path to look into the type</summary>
		public string typePath;
		/// <summary>The list of assemblies that the program should look into</summary>
		public List<string> assemblies;
		
		#endregion // Field Variables
		
		#region Public Constructors
		
		/// <summary>A base constructor for creating the managed input arguments</summary>
		public InputArguments() {
			this.isHelp = false;
			this.isList = false;
			this.includePrivate = false;
			this.output = null;
			this.typePath = "";
			this.assemblies = new List<string>();
		}
		
		#endregion // Public Constructors
		
		#region Public Static Methods
		
		/// <summary>Creates a managed version of input arguments from the array of arguments</summary>
		/// <param name="args">The arguments the program started with</param>
		/// <returns>Returns a managed version of the input arguments</returns>
		public static InputArguments Create(string[] args) {
			// Variables
			InputArguments input = new InputArguments();
			
			for(int i = 0; i < args.Length; i++) {
				switch(args[i].ToLower()) {
					case "-h": case "--help": { input.isHelp = true; } break;
					case "-l": case "--list": { input.isList = true; } break;
					case "-o": case "--out": { input.output = args[++i]; } break;
					case "-p": case "--include-private": { input.includePrivate = true; } break;
					default: {
						if(input.typePath == "" && !input.isList) {
							input.typePath = args[i];
						}
						else {
							input.assemblies.Add(args[i]);
						}
					} break;
				}
			}
			
			if(input.output == null) {
				input.output = (input.isList ? "listTypes.json" : "type.json");
			}
			input.isHelp = (input.isHelp || (!input.isList && input.typePath == ""));
			if(input.isList && input.typePath != "") {
				input.assemblies.Add(input.typePath);
				input.typePath = "";
			}
			
			return input;
		}
		
		#endregion // Public Static Methods
	}
}
