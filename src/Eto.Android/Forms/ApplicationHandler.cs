using System;
using a = Android;
using Eto.Forms;
using System.Collections.Generic;
using System.Threading;

namespace Eto.Android.Forms
{
	/// <summary>
	/// Handler for <see cref="Application"/>
	/// </summary>
	/// <copyright>(c) 2013 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class ApplicationHandler : WidgetHandler<a.App.Application, Application, Application.ICallback>, Application.IHandler
	{
		public static ApplicationHandler Instance { get { return Application.Instance.Handler as ApplicationHandler; } }

		public a.App.Activity MainActivity { get; private set; }

		public ApplicationHandler()
		{
			Control = a.App.Application.Context as a.App.Application;
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
			if (MainActivity != null)
			{
				var ev = new ManualResetEvent(false);
				MainActivity.RunOnUiThread(() =>
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
		}

		public void AsyncInvoke(Action action)
		{
			if (MainActivity != null)
			{
				MainActivity.RunOnUiThread(action);
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


		public void Restart()
		{
			throw new NotImplementedException();
		}

		public void RunIteration()
		{
			throw new NotImplementedException();
		}
	}
}