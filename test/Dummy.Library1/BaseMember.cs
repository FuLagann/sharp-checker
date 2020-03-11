
namespace SchoolSys {
	public abstract class BaseMember : IMember {
		public abstract string Id { get; }
		
		public event System.EventHandler OnMesseged;
		public event System.EventHandler OnMessege;
		
		public bool SignIn() { return true; }
		public bool SignOut() { return true; }
		public ISchedule[] GetSchedule() { return new ISchedule[0]; }
		public void Talk(string message) {}
	}
}
