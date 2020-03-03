
namespace Dummy {
	public struct DummyStruct2<[Dummy("10", "20")]T, K> : IDummy2<K>, IDummer<T> where T : struct where K : class, IDummy, IDummer<T> {
		
	}
}
