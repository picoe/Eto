using System;
using Eto.Forms;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Threading;
using System.Reflection;

namespace Eto.Test.UnitTests.Handlers
{
	public class TestApplicationHandler : TestWidgetHandler, Application.IHandler
	{
		new Application.ICallback Callback { get { return (Application.ICallback)base.Callback; } }
		new Application Widget { get { return (Application)base.Widget; } }
		bool running = true;
		Queue<Action> actions = new Queue<Action>();
		TaskCompletionSource<bool> tcsInvoke = new TaskCompletionSource<bool>();
		object invoke_lock = new object();
		int mainThread;

		int GetThreadId()
		{
			#if PCL
			// use reflection to get current thread's ID
			var threadType = Type.GetType("System.Threading.Thread");
			var typeInfo = threadType.GetTypeInfo();
			var getCurrentThread = typeInfo.GetDeclaredProperty("CurrentThread");
			var currentThread = getCurrentThread.GetValue(null);
			var prop = typeInfo.GetDeclaredProperty("ManagedThreadId");
			return (int)prop.GetValue(currentThread);
			#else
			return Thread.CurrentThread.ManagedThreadId;
			#endif
		}

		public void Attach(object context)
		{
			throw new NotImplementedException();
		}

		async Task<Action> GetNextAction()
		{
			Action action;
			tcsInvoke.Task.Wait();
			lock (invoke_lock)
			{
				tcsInvoke = new TaskCompletionSource<bool>(false);
				return actions.Count > 0 ? actions.Dequeue() : null;
			}
		}


		public async void Run(string[] args)
		{
			Callback.OnInitialized(Widget, EventArgs.Empty);
			mainThread = GetThreadId();
			while (running)
			{
				var action = await GetNextAction();
				if (action != null)
					action();
			}
		}

		public void Quit()
		{
			lock (invoke_lock)
			{
				running = false;
				tcsInvoke.SetResult(true);
			}
		}

		public IEnumerable<Command> GetSystemCommands()
		{
			throw new NotImplementedException();
		}

		public void Open(string url)
		{
			throw new NotImplementedException();
		}

		public void Invoke(Action action)
		{
			// if already in the main thread, we invoke directly otherwise we'd block indefinitely
			if (GetThreadId() == mainThread)
				action();
			else
			{
				var tcs = new TaskCompletionSource<bool>(false);
				AsyncInvoke(() =>
				{ 
					try
					{
						action();
					}
					finally
					{
						tcs.SetResult(true);
					}
				});
				tcs.Task.Wait();
			}
		}

		public void AsyncInvoke(Action action)
		{
			lock (invoke_lock)
			{
				actions.Enqueue(action);
				tcsInvoke.SetResult(true);
			}
		}

		public void OnMainFormChanged()
		{
		}

		public void Restart()
		{
			throw new NotImplementedException();
		}

		public void RunIteration()
		{
			throw new NotImplementedException();
		}

		public void CreateStandardMenu(MenuItemCollection menuItems, IEnumerable<Command> commands)
		{
			throw new NotImplementedException();
		}

		public bool QuitIsSupported
		{
			get
			{
				throw new NotImplementedException();
			}
		}

		public Keys CommonModifier
		{
			get { return Keys.Control; }
		}

		public Keys AlternateModifier
		{
			get { return Keys.Alt; }
		}

		public string BadgeLabel
		{
			get
			{
				throw new NotImplementedException();
			}
			set
			{
				throw new NotImplementedException();
			}
		}
	}
}

