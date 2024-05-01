using Eto.Mac.Forms;

namespace Eto.Mac
{
	interface IQueueAction
	{
		void Execute();
	}

	/// <summary>
	/// Queues multiple actions into one async call
	/// </summary>
	static class AsyncQueue
	{
		static Queue<IQueueAction> _actionQueue = new Queue<IQueueAction>();
		static HashSet<object> _items = new HashSet<object>();

		class ActionQueueAction : IQueueAction
		{
			public Action Action;
			public void Execute() => Action();
		}

		class SingleQueueAction : IQueueAction
		{
			public object Key;
			public IQueueAction Inner;
			public void Execute()
			{
				_items.Remove(Key);
				Inner.Execute();
			}
		}

		public static void Add(Action action)
		{
			Add(new ActionQueueAction { Action = action });
		}

		public static void Add(IQueueAction action, object key)
		{
			if (key != null)
			{
				if (_items.Contains(key))
					return;
				_items.Add(key);
			}
			Add(new SingleQueueAction { Inner = action, Key = key });
		}

		public static void Add(IQueueAction action)
		{
			_actionQueue.Enqueue(action);

			if (_actionQueue.Count == 1)
			{
				// first one added, schedule a layout!
				NSApplication.SharedApplication.BeginInvokeOnMainThread(FlushQueue);
			}
		}

		static void FlushQueue()
		{
			while (_actionQueue.Count > 0)
			{
				var action = _actionQueue.Dequeue();
				action.Execute();
			}
		}
	}
}
