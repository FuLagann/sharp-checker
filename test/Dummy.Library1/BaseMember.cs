
using System.Collections.Generic;

namespace SchoolSys {
	public abstract class BaseMember<T> : Dictionary<Dictionary<string, List<string>>, string> {
		public abstract string Id { get; }
		
		public event System.EventHandler OnMesseged;
		public event System.EventHandler OnMessege;
		
		public bool SignIn() { return true; }
		public bool SignOut() { return true; }
		public ISchedule[] GetSchedule() { return new ISchedule[0]; }
		public void Talk(string message) {}
		public void SendMessage<T, K>(T message, K data) where K : struct {}
		private class HiddenBaseMember {}
		public class PublicBaseMember {}
	}
}
