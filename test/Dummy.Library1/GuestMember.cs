
namespace SchoolSys.Guests {
	public sealed class GuestMember<T> : IMember where T : IMember {
		// Variables
		private T member;
		
		public string Id { get { return this.member.Id; } }
		public bool SignIn() { return true; }
		public bool SignOut() { return true; }
		public ISchedule[] GetSchedule() { return new ISchedule[0]; }
		
		public GuestMember(T member) {
			this.member = member;
		}
	}
}
