using System;
using a = Android;
using Eto.Forms;
using System.Collections.Generic;

namespace Eto.Platform.Android.Forms
{
	/// <summary>
	/// Handler for <see cref="IApplication"/>
	/// </summary>
	/// <copyright>(c) 2013 by Curtis Wensley</copyright>
	/// <license type="BSD-3">See LICENSE for full terms</license>
	public class ApplicationHandler : WidgetHandler<a.App.Application, Application>, IApplication
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

		public void Run(string[] args)
		{
			Widget.OnInitialized(EventArgs.Empty);
		}

		public void Quit()
		{
			throw new NotImplementedException();
		}

		public void GetSystemActions(List<BaseAction> actions, ISubMenuWidget menu, ToolBar toolBar, bool addStandardItems)
		{
			throw new NotImplementedException();
		}

		public void Open(string url)
		{
			throw new NotImplementedException();
		}

		public void Invoke(Action action)
		{
			throw new NotImplementedException();
		}

		public void AsyncInvoke(Action action)
		{
			throw new NotImplementedException();
		}

		public Keys CommonModifier
		{
			get { return Key.Control; }
		}

		public Keys AlternateModifier
		{
			get { return Key.Alt; }
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