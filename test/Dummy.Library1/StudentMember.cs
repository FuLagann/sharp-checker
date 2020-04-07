
namespace SchoolSys {
	public sealed class StudentMember : BaseMember, System.ICloneable {
		private string id;
		
		public override string Id { get { return id; } }
		
		public StudentMember(string id) { this.id = id; }
		
		public static string GenerateId(int seed = 10203040) {
			(new StudentMember("")).SendMessage("Hello", 1);
			return $"{ 2 * seed }w{ seed * seed }";
		}
		
		public object Clone() { return this; }
		
		public static implicit operator StaffMember(StudentMember obj) {
			return new StaffMember(obj, "0");
		}
	}
}
