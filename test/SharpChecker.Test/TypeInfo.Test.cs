
using Newtonsoft.Json;

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
				"SchoolSys.IMember",
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
			
			if(File.Exists("type.json")) { File.Delete("type.json"); }
			
			if(TypeInfo.GenerateTypeInfo(assemblies, "SchoolSys.BaseMember", out info)) {
				File.WriteAllText("type.json", JsonConvert.SerializeObject(info, Formatting.Indented));
			}
			Assert.True(File.Exists("type.json"));
		}
		
		[Fact]
		public void GenerateJsonForList() {
			// Variables
			TypeList list;
			string[] assemblies = new string[] {
				"Dummy.Library1.dll",
				"Dummy.Library2.dll",
				"Dummy.Library3.dll"
			};
			
			if(File.Exists("listTypes.json")) { File.Delete("listTypes.json"); }
			
			list = TypeList.GenerateList(assemblies);
			
			File.WriteAllText("listTypes.json", JsonConvert.SerializeObject(list, Formatting.Indented));
			Assert.True(File.Exists("listTypes.json"));
		}
	}
}
