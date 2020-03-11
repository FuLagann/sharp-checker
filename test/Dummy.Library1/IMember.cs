
namespace SchoolSys {
	public interface IMember {
		string Id { get; }
		bool SignIn();
		bool SignOut();
		ISchedule[] GetSchedule();
	}
}
