
namespace SchoolSys {
	internal class HiddenMember : BaseMember<string> {
		public override string Id { get { return "hidden"; } }
		
		public class HiddenPublicMember : BaseMember<string> {
			public override string Id { get { return ""; } }
			
			public class SuperHiddenMember {
				public class SuperDuperHiddenMember {}
			}
		}
	}
}