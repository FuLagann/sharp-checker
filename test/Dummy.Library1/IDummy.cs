
namespace Dummy {
	[Dummy("Hello", "World")]
	public interface IDummy {
		string Variable2 { get; }
	}
	
	public interface IDummy2<T> {
		
	}

}

public interface IDummy3 : System.IDisposable {
	
}
