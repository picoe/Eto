using System;
using System.Collections.Generic;

namespace Eto
{
	/// <summary>
	/// Helper for caching objects on a per-generator basis
	/// </summary>
	internal class ObjectCache<TKey, TValue>
	{
		Dictionary<Generator, Dictionary<TKey, TValue>> generatorLookup = new Dictionary<Generator, Dictionary<TKey, TValue>> ();

		/// <summary>
		/// Get the cached value for the specified key, or create a new one if it does not exist
		/// </summary>
		/// <param name="generator">Generator to get the cached value from</param>
		/// <param name="key">Key of the value to get</param>
		/// <param name="createNew">Function to create a new value for the specified <paramref name="key"/> if it does not exist in the cache</param>
		public TValue Get (Generator generator, TKey key, Func<TValue> createNew)
		{
			generator = generator ?? Generator.Current;
			Dictionary<TKey, TValue> lookup;
			lock (generatorLookup) {
				if (!generatorLookup.TryGetValue (generator, out lookup)) {
					lookup = new Dictionary<TKey, TValue>();
					generatorLookup [generator] = lookup;
				}
			}
			TValue value;
			if (!lookup.TryGetValue (key, out value)) {
				value = createNew ();
				lookup [key] = value;
			}
			return value;
		}
	}
}

