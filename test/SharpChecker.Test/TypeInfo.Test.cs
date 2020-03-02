
using System.IO;

using Xunit;

namespace SharpChecker.Testing {
	public class TypeInfoTest {
		[Fact]
		public void CanCheckDll() {
			// Variables
			TypeInfo info;
			TypeInfo info2;
			
			TypeInfo.GenerateTypeInfo(
				new string[] { "Dummy.Library1.dll" },
				"Dummy.Dummy1",
				out info
			);
			Assert.NotNull(info);
			Assert.NotNull(Assert.ThrowsAny<System.IO.FileNotFoundException>(delegate() {
				TypeInfo.GenerateTypeInfo(
					new string[] {
						"Dummy.Libary1.dll",
						"Dummy.Libary2.dll",
						"Dummy.Libary3.dll",
						"Dummy.Libary4.dll"
					},
					"Dummy.Dummy4",
					out info2
				);
			}));
		}
		
		[Fact]
		public void GenerateJson() {
			// Variables
			TypeInfo info;
			string[] assemblies = new string[] {
				"Dummy.Library1.dll",
				"Dummy.Library2.dll",
				"Dummy.Library3.dll"
			};
			
			if(TypeInfo.GenerateTypeInfo(assemblies, "Dummy.Dummy1", out info)) {
				File.WriteAllText("type.json", info.GetJson());
			}
		}
	}
}