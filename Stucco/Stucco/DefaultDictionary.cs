using System;
using System.Collections.Generic;
using System.Collections;

namespace Stucco
{
	public class DefaultDictionary<TKey, TValue> : IDictionary<TKey,TValue>
	{
		private readonly Func<TValue> _defaultSelector;
		private readonly Dictionary<TKey, TValue> _values = new Dictionary<TKey, TValue>();

		public DefaultDictionary(Func<TValue> defaultSelector)
		{
			_defaultSelector = defaultSelector;
		}

		public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
		{
			return _values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public void Add(KeyValuePair<TKey, TValue> item)
		{
			((IDictionary<TKey,TValue>)_values).Add(item);
		}

		public void Clear()
		{
			_values.Clear();
		}

		public bool Contains(KeyValuePair<TKey, TValue> item)
		{
			return ((IDictionary<TKey,TValue>)_values).Contains(item);
		}

		public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
		{
			((IDictionary<TKey, TValue>)_values).CopyTo(array, arrayIndex);
		}

		public bool Remove(KeyValuePair<TKey, TValue> item)
		{
			return ((IDictionary<TKey, TValue>)_values).Remove(item);
		}

		public int Count { get { return _values.Count; } }

		public bool IsReadOnly { get { return ((IDictionary<TKey, TValue>)_values).IsReadOnly; } }

		public bool ContainsKey(TKey key)
		{
			return _values.ContainsKey(key);
		}

		public void Add(TKey key, TValue value)
		{
			_values.Add(key, value);
		}

		public bool Remove(TKey key)
		{
			return _values.Remove(key);
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			return _values.TryGetValue(key, out value);
		}

		public TValue this [TKey key] {
			get {
				if (!_values.ContainsKey(key)) {
					_values.Add(key, _defaultSelector());
				}
				return _values[key];
			}
			set {
				if (!_values.ContainsKey(key)) {
					_values.Add(key, _defaultSelector());
				}
				_values[key] = value;
			}
		}

		public ICollection<TKey> Keys { get { return _values.Keys; } }

		public ICollection<TValue> Values { get { return _values.Values; } }

		public Dictionary<TKey, TValue> ToDictionary()
		{
			return new Dictionary<TKey, TValue>(_values);
		}
	}
}

