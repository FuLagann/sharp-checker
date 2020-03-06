
namespace SharpChecker {
	public class InputArguments {
		// Variables
		public bool isHelp;
		
		public InputArguments(bool isHelp) {
			this.isHelp = isHelp;
		}
		
		public static InputArguments Create(string[] args) {
			// Variables
			InputArguments input = new InputArguments(args.Length < 2);
			
			for(int i = 0; i < args.Length; i++) {
				switch(args[i].ToLower()) {
					case "--help": { input.isHelp = true; } return input;
				}
			}
			
			return input;
		}
	}
}
