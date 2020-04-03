
namespace SchoolSys {
	internal class HiddenMember : BaseMember {
		public override string Id { get { return "hidden"; } }
		
		public class HiddenPublicMember : BaseMember {
			public override string Id { get { return ""; } }
			
			public class SuperHiddenMember {
				public class SuperDuperHiddenMember {}
			}
		}
	}
}