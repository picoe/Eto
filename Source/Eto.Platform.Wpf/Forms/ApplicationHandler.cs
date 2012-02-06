using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using System.Diagnostics;
using sw = System.Windows;

namespace Eto.Platform.Wpf.Forms
{
	public class ApplicationHandler : WidgetHandler<System.Windows.Application, Application>, IApplication
	{
		public ApplicationHandler ()
		{
			Control = new System.Windows.Application ();
		}

		public void RunIteration()
		{
		}

		public void Quit()
		{
			Control.Shutdown();
		}

		public void InvokeOnMainThread (Action action)
		{
			Control.Dispatcher.Invoke (action);
		}

		public void GetSystemActions(GenerateActionArgs args)
		{
		}

		public Key CommonModifier
		{
			get { return Key.Control; }
		}

		public Key AlternateModifier
		{
			get { return Key.Alt; }
		}

		public void Open(string url)
		{
			Process.Start(url);	
		}

		public void Run (string[] args)
		{
			Widget.OnInitialized (EventArgs.Empty);
			if (Widget.MainForm != null)
				Control.Run ((System.Windows.Window)Widget.MainForm.ControlObject);
			else
				Control.Run ();
		}

		public void Restart ()
		{
			System.Diagnostics.Process.Start (System.Windows.Application.ResourceAssembly.Location);
			System.Windows.Application.Current.Shutdown ();
		}

		public override void AttachEvent (string handler)
		{
			switch (handler) {
				case Application.TerminatingEvent:
					// handled by WpfWindow
					break;
				default:
					base.AttachEvent (handler);
					break;
			}
		}
	}
}
