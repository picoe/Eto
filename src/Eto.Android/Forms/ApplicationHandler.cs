using a = Android;
using ao = Android.OS;

namespace Eto.Android.Forms
{
	/// <summary>
	/// Handler for <see cref="Application"/>
	/// </summary>
	/// <copyright>(c) 2013 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class ApplicationHandler : WidgetHandler<a.App.Application, Application, Application.ICallback>, Application.IHandler
	{
		private class LifecyleReceiver : Java.Lang.Object, a.App.Application.IActivityLifecycleCallbacks
		{
			private readonly ApplicationHandler handler;

			public LifecyleReceiver(ApplicationHandler handler)
			{
				this.handler = handler;
			}

			public void OnActivityCreated(a.App.Activity activity, ao.Bundle savedInstanceState) { }

			public void OnActivityStarted(a.App.Activity activity) => handler.OnActivityActivated(activity);

			public void OnActivityResumed(a.App.Activity activity) => handler.OnActivityActivated(activity);

			public void OnActivityPaused(a.App.Activity activity) => handler.OnActivityDeactivated(activity);

			public void OnActivityStopped(a.App.Activity activity) => handler.OnActivityDeactivated(activity);

			public void OnActivityDestroyed(a.App.Activity activity) { }
			
			public void OnActivitySaveInstanceState(a.App.Activity activity, ao.Bundle outState) { }
		}

		public static ApplicationHandler Instance { get { return Application.Instance.Handler as ApplicationHandler; } }

		private ao.Handler mainLooperHandler;

		[Obsolete("Use Eto's own Thread.Current.IsMain, when it works")]
		private Int32 _MainThread;
		public Boolean IsMainThread => _MainThread == Thread.CurrentThread.ManagedThreadId;

		public bool IsActive => true;

		// TODO: This should not be publicly settable
		public a.App.Activity MainActivity { get; set; }

		public a.App.Activity TopActivity
		{
			get
			{
				var CachedTopReference = _TopActivity;

				if (CachedTopReference != null && CachedTopReference.TryGetTarget(out var topActivity))
					return topActivity;

				return MainActivity;
			}
		} private WeakReference<a.App.Activity> _TopActivity;

		private void OnActivityActivated(a.App.Activity activity)
		{
			_TopActivity = new WeakReference<a.App.Activity>(activity);
		}

		private void OnActivityDeactivated(a.App.Activity activity)
		{
			if (TopActivity?.ToString() == activity.ToString())
				_TopActivity = null;
		}

		public ApplicationHandler()
		{
			_MainThread = Thread.CurrentThread.ManagedThreadId;

			mainLooperHandler = new ao.Handler(ao.Looper.MainLooper);
			Control = Platform.AppContext as a.App.Application;
			Control.RegisterActivityLifecycleCallbacks(new LifecyleReceiver(this));
		}

		public void OnMainFormChanged()
		{
			if (Widget.MainForm != null)
			{
				var window = Widget.MainForm.Handler as IAndroidWindow;
				if (window != null)
					window.Activity = MainActivity;
			}
		}

		public void Attach(object context)
		{
			var activity = context as a.App.Activity;
			if (activity != null)
			{
				MainActivity = activity;
			}
		}

		public void Run()
		{
			Callback.OnInitialized(Widget, EventArgs.Empty);
		}

		public void Quit()
		{
			throw new NotImplementedException();
		}

		public bool QuitIsSupported { get { return false; } }

		public void Open(string url)
		{
			throw new NotImplementedException();
		}

		public void Invoke(Action action)
		{
			if (!IsMainThread && TopActivity != null)
			{
				var ev = new ManualResetEvent(false);
				AsyncInvoke(() =>
				{
					try
					{
						action();
					}
					finally
					{
						ev.Set();
					}
				});
				ev.WaitOne();
			}

			else
			{
				action();
			}
		}

		public void AsyncInvoke(Action action)
		{
			mainLooperHandler.Post(action);
			/*
			Looper.MainLooper.
			if (MainActivity != null)
			{
				MainActivity.RunOnUiThread(action);
			}
			*/
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

		public void Restart()
		{
			throw new NotImplementedException();
		}

		public void RunIteration()
		{
			throw new NotImplementedException();
		}

		public Task<bool> RequestPermissionAsync(params String[] permissions)
		{
			var Request = new PermissionRequest(TopActivity, permissions);
			Request.Start();
			return Request.Task;
		}
	}

	public class PermissionRequest
	{
		private TaskCompletionSource<bool> tcs;
		private a.App.Activity activity;
		private string[] permissions;
		private int testCount;

		public Task<bool> Task => tcs.Task;

		internal PermissionRequest(a.App.Activity activity, string[] permissions)
		{
			this.permissions = permissions;
			this.activity = activity;
			tcs = new TaskCompletionSource<bool>();
		}

		internal void Start()
		{
			if(Test())
				return;

			activity.RequestPermissions(permissions, 0);

			ScheduleCheck();
		}

		private void ScheduleCheck()
		{
			System.Threading.Tasks.Task.Delay(500).ContinueWith(t => 
			{
				if (Test())
					return;

				ScheduleCheck();
			});
		}

		private bool Test()
		{
			try
			{
				foreach (var permission in permissions)
					if (activity.CheckSelfPermission(permission) != a.Content.PM.Permission.Granted)
					{
						if (++testCount > 120)
							tcs.SetResult(false);

						return false;
					}

				tcs.SetResult(true);
				return true;
			}

			catch
			{
				tcs.SetResult(false);
				return true;
			}
		}
	}
}