
namespace SchoolSys {
	public sealed class StudentMember : BaseMember {
		private string id;
		
		public override string Id { get { return id; } }
		
		public StudentMember(string id) { this.id = id; }
	}
}
