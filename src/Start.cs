
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
			if(args.Length < 2) {
				System.Console.WriteLine("Use: SharpChecker <type-path> <list-of-assemblies>");
				System.Console.WriteLine("Note: Instead of using ` you can use -.");
				System.Environment.Exit(0);
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
			
		}
		
		#endregion // Public Static Methods
	}
}
