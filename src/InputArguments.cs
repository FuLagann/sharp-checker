
using System.Collections.Generic;

namespace SharpChecker {
	public class InputArguments {
		// Variables
		public bool isHelp;
		public bool isList;
		public bool includePrivate;
		public string output;
		public string typePath;
		public List<string> assemblies;
		
		public InputArguments() {
			this.isHelp = false;
			this.isList = false;
			this.includePrivate = false;
			this.output = null;
			this.typePath = "";
			this.assemblies = new List<string>();
		}
		
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
			
			return input;
		}
	}
}
