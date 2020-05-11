
namespace SchoolSys {
	public class StaffMember : BaseMember<int> {
		protected string id;
		protected StudentMember studentHistory;
		private GuestStaffMember member;
		
		public override string Id { get { return $"{ this.studentHistory.Id }-{ this.id }"; } }
		
		public StaffMember(StudentMember history, string id) {
			this.id = id;
			this.studentHistory = history;
		}
		
		private class GuestStaffMember : BaseMember<int> {
			public override string Id { get { return "1"; } }
		}
	}
}
