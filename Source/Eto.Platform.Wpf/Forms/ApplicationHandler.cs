using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Eto.Forms;
using System.Diagnostics;

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

		public void Run()
		{
			Widget.OnInitialized (EventArgs.Empty);
			if (Widget.MainForm != null)
				Control.Run((System.Windows.Window)Widget.MainForm.ControlObject);
			else
				Control.Run();
		}

		public void Quit()
		{
			Control.Shutdown();
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
	}
}
