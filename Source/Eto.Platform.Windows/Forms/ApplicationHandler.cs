using System;
using System.Runtime.InteropServices;
using SWF = System.Windows.Forms;
using Eto.Forms;
using System.Diagnostics;

namespace Eto.Platform.Windows
{
	public class ApplicationHandler : WidgetHandler<object, Application>, IApplication
	{
		public static bool EnableScrollingUnderMouse = true;
		
		public void RunIteration()
		{
			SWF.Application.DoEvents();
		}

		public void Restart ()
		{
			SWF.Application.Restart ();
		}
		
		public void Run(string[] args)
		{
			SWF.Application.EnableVisualStyles();
			if (!Eto.Misc.Platform.IsMono)
				SWF.Application.DoEvents();
			
			Application app = ((Application)Widget);
			app.OnInitialized(EventArgs.Empty);

			if (EnableScrollingUnderMouse)
				SWF.Application.AddMessageFilter (new ScrollMessageFilter ());
			
			//SWF.Application.AddMessageFilter(new KeyFilter());
			if (app.MainForm != null) SWF.Application.Run((SWF.Form)app.MainForm.ControlObject);
			else SWF.Application.Run();
		}

		public void Quit()
		{
			SWF.Application.Exit();
		}
		
		public void Open (string url)
		{
			var info = new ProcessStartInfo(url);
			Process.Start(info);
		}
		
		public void GetSystemActions (GenerateActionArgs args)
		{
			
		}
		
		public Key CommonModifier {
			get {
				return Key.Control;
			}
		}

		public Key AlternateModifier {
			get {
				return Key.Alt;
			}
		}
		
	}
}
