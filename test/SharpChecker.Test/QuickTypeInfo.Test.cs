
using Xunit;

namespace SharpChecker.Testing {
	public class QuickTypeInfoTest {
		// Variables
		private static string[] assemblies = new string[] {
			"Dummy.Library1.dll",
			"Dummy.Library2.dll",
			"Dummy.Library3.dll"
		};
			
		[Theory]
		[InlineData("SchoolSys.IMember", "SchoolSys.IMember", "SchoolSys", "IMember")]
		[InlineData("SchoolSys.ISchedule", "SchoolSys.ISchedule", "SchoolSys", "ISchedule")]
		[InlineData("SchoolSys.Guests.GuestMember`1", "SchoolSys.Guests.GuestMember`1", "SchoolSys.Guests", "GuestMember<T>")]
		public void CheckName(string typePath, string expectedUnlocalized, string expectedNamespace, string expectedName) {
			// Variables
			TypeInfo info;
			
			if(TypeInfo.GenerateTypeInfo(QuickTypeInfoTest.assemblies, typePath, out info)) {
				Assert.Equal(expectedUnlocalized, info.typeInfo.unlocalizedName);
				Assert.Equal(expectedNamespace, info.typeInfo.namespaceName);
				Assert.Equal(expectedName, info.typeInfo.name);
				if(expectedNamespace != "") {
					Assert.Equal(expectedNamespace + "." + expectedName, info.typeInfo.fullName);
				}
				else {
					Assert.Equal(expectedName, info.typeInfo.fullName);
				}
			}
			else {
				throw new System.Exception("Type is not found!");
			}
		}
		
		[Theory]
		[InlineData("SchoolSys.ISchedule", false, 0, "", "", null)]
		[InlineData("SchoolSys.Guests.GuestMember`1", true, 0, "T", "T", new string[] { "SchoolSys.IMember", "SchoolSys", "IMember" })]
		public void CheckGenericParameters(string typePath, bool hasParameters, int parameterIndex, string expectedUnlocalized, string expectedName, string[] constraints_uname_namesp_name) {
			// Variables
			TypeInfo info;
			
			if(TypeInfo.GenerateTypeInfo(QuickTypeInfoTest.assemblies, typePath, out info)) {
				Assert.Equal(hasParameters, info.typeInfo.genericParameters.Length > 0);
				if(hasParameters) {
					// Variables
					GenericParametersInfo generic = info.typeInfo.genericParameters[parameterIndex];
					
					Assert.Equal(expectedUnlocalized, generic.unlocalizedName);
					Assert.Equal(expectedName, generic.name);
					for(int i = 0; i < generic.constraints.Length; i++) {
						Assert.Equal(constraints_uname_namesp_name[3 * i], generic.constraints[i].unlocalizedName);
						Assert.Equal(constraints_uname_namesp_name[3 * i + 1], generic.constraints[i].namespaceName);
						Assert.Equal(constraints_uname_namesp_name[3 * i + 2], generic.constraints[i].name);
						if(constraints_uname_namesp_name[3 * i + 1] != "") {
							Assert.Equal(
								constraints_uname_namesp_name[3 * i + 1] + "." + constraints_uname_namesp_name[3 * i + 2],
								generic.constraints[i].fullName
							);
						}
						else {
							Assert.Equal(constraints_uname_namesp_name[3 * i + 2], info.typeInfo.fullName);
						}
					}
				}
			}
			else {
				throw new System.Exception("Type is not found!");
			}
		}
	}
}
