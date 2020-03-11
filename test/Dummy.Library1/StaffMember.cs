
namespace SchoolSys {
	public class StaffMember : BaseMember {
		protected string id;
		protected StudentMember studentHistory;
		
		public override string Id { get { return $"{ this.studentHistory.Id }-{ this.id }"; } }
		
		public StaffMember(StudentMember history, string id) {
			this.id = id;
			this.studentHistory = history;
		}
	}
}
