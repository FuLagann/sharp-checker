
using System;

namespace DataStructures {
	public class DataTree<TKey> : IDisposable, IEquatable<DataTree<TKey>> where TKey : struct {
		private DataTree<TKey>[] children;
		private TKey value;
		
		public delegate DataTree<T> GatherHash<T>(T original, T control, DataTree<T> reference) where T : struct;
		
		public TKey Value { get { return this.value; } }
		public int Count { get { return 1 + this.children.Length; } }
		
		public DataTree(TKey key) { this.value = key; }
		
		public void Dispose() {}
		public bool Equals(DataTree<TKey> other) { return true; }
	}
}
