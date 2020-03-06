
using System.IO;

namespace SharpChecker {
	/// <summary>
	/// A static class used as the entry point for the program
	/// </summary>
	public static class Start {
		#region Public Static Methods
		
		/// <summary>
		/// Starts the application
		/// </summary>
		/// <param name="args">The arguments to start with</param>
		public static void Main(string[] args) {
			// Variables
			InputArguments input = InputArguments.Create(args);
			
			if(input.isHelp) {
				DisplayHelp();
			}
			
			// Variables
			string typePath = args[0].Replace('-', '`');
			string[] assemblies = new string[args.Length - 1];
			TypeInfo info;
			
			System.Array.Copy(args, 1, assemblies, 0, assemblies.Length);
			
			if(TypeInfo.GenerateTypeInfo(assemblies, typePath, out info)) {
				System.Console.WriteLine(info.GetJson());
				File.WriteAllText("type.json", info.GetJson());
			}
			else {
				System.Console.WriteLine($"Type [{ typePath }] is not found!");
			}
		}
		
		public static void DisplayHelp() {
			System.Console.WriteLine("Use: SharpChecker [options] <type-path> <list-of-assemblies>");
			System.Console.WriteLine("Note: Instead of using ` you can use -.");
			System.Console.WriteLine("Options:");
			System.Console.WriteLine("--include-private\t\tInclude private members");
			System.Console.WriteLine("--out:<output-file>\t\tThe file to be outputted");
			System.Environment.Exit(0);
		}
		
		#endregion // Public Static Methods
	}
}
