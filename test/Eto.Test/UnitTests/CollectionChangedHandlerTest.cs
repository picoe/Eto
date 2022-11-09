using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using NUnit.Framework;
using Eto;

namespace Eto.Test.UnitTests
{
	[TestFixture]
	public class CollectionChangedHandlerTest : TestBase
	{
		class Collection : IEnumerable<string>, INotifyCollectionChanged
		{
			private IEnumerable<string> _data;

			public Collection(IEnumerable<string> data)
			{
				_data = data;
			}

			public event NotifyCollectionChangedEventHandler CollectionChanged;

			public IEnumerator<string> GetEnumerator()
			{
				return _data.GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return _data.GetEnumerator();
			}

			public void Update(IEnumerable<string> data)
			{
				_data = data;
				CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
			}
		}

		class CollectionHandler : EnumerableChangedHandler<string>
		{
			public override void AddItem(string item)
			{
			}

			public override void InsertItem(int index, string item)
			{
			}

			public override void RemoveAllItems()
			{
			}

			public override void RemoveItem(int index)
			{
			}
		}

		[Test]
		public void LookupCacheShouldBeClearedWhenEnumerableChanges()
		{
			var handler = new CollectionHandler();
			var collection = new Collection(new[] {"1"});
			handler.Register(collection);
			Assert.AreEqual(1, handler.Count);
			collection.Update(new[] { "1", "2" });
			Assert.AreEqual(2, handler.Count);
		}
	}
}

