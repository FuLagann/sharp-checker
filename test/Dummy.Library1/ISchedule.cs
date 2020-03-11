
namespace SchoolSys {
	public interface ISchedule {
		TimeBlock time { get; set; }
		string Description { get; set; }
		bool IsComplete { get; set; }
	}
}
