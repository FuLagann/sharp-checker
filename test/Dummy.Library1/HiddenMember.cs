
namespace SchoolSys {
	internal class HiddenMember : BaseMember<int> {
		public override string Id { get { return "hidden"; } }
		
		public class HiddenPublicMember : BaseMember<int> {
			public override string Id { get { return ""; } }
			
			public class SuperHiddenMember {
				public class SuperDuperHiddenMember {}
			}
		}
	}
}