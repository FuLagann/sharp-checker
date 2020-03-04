
using System.Collections.Generic;

namespace Dummy {
	public abstract class DummyStruct2<[Dummy("10", "20")]T, K> : IDummy2<Dictionary<IDummy2<T>, IDummy2<K>>>, IDummer<IDummy2<T>> where T : struct where K : class, IDummy, IDummer<T> {
		
	}
}
